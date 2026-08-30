using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using App_Rental_Proyek.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.AlatProyek
{
    public partial class EditAlatProyek : Form
    {
        public bool InitializationSucceeded { get; private set; } = true;
        public string InitializationErrorMessage { get; private set; }

        private ulong _alatId;
        private AlatProyekModel _alatData;
        private string _gambarSourcePath;
        private bool _gambarChanged;
        private bool _gambarRemoved;

        public EditAlatProyek(ulong alatId)
        {
            InitializeComponent();
            _alatId = alatId;
            InitializeForm();
            LoadAlatData();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void EditAlatProyek_Load(object sender, EventArgs e)
        {
            SetupComboboxes();
            SetControlValues();
        }

        private void SetupComboboxes()
        {
            // Kategori
            try
            {
                DataTable dt = DatabaseConnection.GetData(
                    "SELECT id, nama_kategori FROM kategoris ORDER BY nama_kategori ASC");
                cbKategori.Items.Clear();
                cbKategori.Tag = dt;
                foreach (DataRow row in dt.Rows)
                {
                    cbKategori.Items.Add(row["nama_kategori"].ToString());
                }
            }
            catch (Exception ex)
            {
                InitializationSucceeded = false;
                InitializationErrorMessage = $"Gagal memuat data kategori: {ex.Message}";
            }

            // Lokasi
            try
            {
                DataTable dt = DatabaseConnection.GetData(
                    "SELECT id, nama_lokasi FROM lokasis ORDER BY nama_lokasi ASC");
                cbLokasi.Items.Clear();
                cbLokasi.Tag = dt;
                foreach (DataRow row in dt.Rows)
                {
                    cbLokasi.Items.Add(row["nama_lokasi"].ToString());
                }
            }
            catch (Exception ex)
            {
                InitializationSucceeded = false;
                InitializationErrorMessage = $"Gagal memuat data lokasi: {ex.Message}";
            }

            // Kondisi
            cbKondisi.Items.Clear();
            cbKondisi.Items.Add("baik");
            cbKondisi.Items.Add("rusak_ringan");
            cbKondisi.Items.Add("rusak_berat");

            // Status
            cbStatus.Items.Clear();
            cbStatus.Items.Add("tersedia");
            cbStatus.Items.Add("disewa");
            cbStatus.Items.Add("maintenance");
            cbStatus.Items.Add("tidak_aktif");
        }

        private void LoadAlatData()
        {
            try
            {
                string query = @"
                    SELECT a.id, a.kategori_id, a.lokasi_id, a.kode_alat, a.nama_alat, a.deskripsi,
                           a.harga_sewa_harian, a.stok, a.stok_tersedia, a.kondisi, a.status, a.gambar,
                           a.created_at, a.updated_at
                    FROM alat_proyeks a
                    WHERE a.id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _alatId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    _alatData = new AlatProyekModel
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        KategoriId = row["kategori_id"] != DBNull.Value ? Convert.ToUInt64(row["kategori_id"]) : 0,
                        LokasiId = row["lokasi_id"] != DBNull.Value ? Convert.ToUInt64(row["lokasi_id"]) : 0,
                        KodeAlat = row["kode_alat"]?.ToString() ?? "",
                        NamaAlat = row["nama_alat"]?.ToString() ?? "",
                        Deskripsi = row["deskripsi"]?.ToString(),
                        HargaSewaHarian = row["harga_sewa_harian"] != DBNull.Value ? Convert.ToDecimal(row["harga_sewa_harian"]) : 0m,
                        Stok = row["stok"] != DBNull.Value ? Convert.ToInt32(row["stok"]) : 0,
                        StokTersedia = row["stok_tersedia"] != DBNull.Value ? Convert.ToInt32(row["stok_tersedia"]) : 0,
                        Kondisi = row["kondisi"]?.ToString() ?? "baik",
                        Status = row["status"]?.ToString() ?? "tersedia",
                        Gambar = row["gambar"]?.ToString(),
                        CreatedAt = row["created_at"] as DateTime?,
                        UpdatedAt = row["updated_at"] as DateTime?
                    };
                }
                else
                {
                    InitializationSucceeded = false;
                    InitializationErrorMessage = "Data alat tidak ditemukan!";
                    MessageBox.Show(InitializationErrorMessage, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                InitializationSucceeded = false;
                InitializationErrorMessage = $"Error memuat data alat: {ex.Message}";
                MessageBox.Show(InitializationErrorMessage, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void SetControlValues()
        {
            if (_alatData == null) return;

            txtKode.Text = _alatData.KodeAlat;
            txtNama.Text = _alatData.NamaAlat;
            txtDeskripsi.Text = _alatData.Deskripsi ?? "";
            txtHarga.Text = _alatData.HargaSewaHarian.ToString("N0");
            txtStok.Text = _alatData.Stok.ToString();
            txtStokTersedia.Text = _alatData.StokTersedia.ToString();

            SelectComboById(cbKategori, _alatData.KategoriId);
            SelectComboById(cbLokasi, _alatData.LokasiId);

            SelectComboByValue(cbKondisi, _alatData.Kondisi);
            SelectComboByValue(cbStatus, _alatData.Status);

            LoadGambarPreview();
        }

        private void LoadGambarPreview()
        {
            picGambar.Image?.Dispose();
            picGambar.Image = null;

            if (_alatData != null && !string.IsNullOrWhiteSpace(_alatData.Gambar))
            {
                picGambar.Image = AlatProyekGambarHelper.LoadImage(_alatData.Gambar);
            }
        }

        private void SelectComboById(Guna.UI2.WinForms.Guna2ComboBox combo, ulong id)
        {
            if (id == 0) { if (combo.Items.Count > 0) combo.SelectedIndex = 0; return; }
            try
            {
                DataTable dt = combo.Tag as DataTable;
                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Convert.ToUInt64(dt.Rows[i]["id"]) == id)
                        {
                            combo.SelectedIndex = i;
                            return;
                        }
                    }
                }
            }
            catch { }
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
        }

        private void SelectComboByValue(Guna.UI2.WinForms.Guna2ComboBox combo, string value)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i].ToString() == value)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
        }

        private ulong GetSelectedId(Guna.UI2.WinForms.Guna2ComboBox combo)
        {
            int idx = combo.SelectedIndex;
            if (idx < 0) return _alatData.KategoriId;
            try
            {
                DataTable dt = combo.Tag as DataTable;
                if (dt != null && idx < dt.Rows.Count)
                {
                    return Convert.ToUInt64(dt.Rows[idx]["id"]);
                }
            }
            catch { }
            return 0;
        }

        private bool IsKodeExists(string kode, ulong excludeId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM alat_proyeks WHERE kode_alat = @kode AND id != @id";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@kode", kode),
                    new MySqlParameter("@id", excludeId)
                };
                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                return Convert.ToInt64(result) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memeriksa kode alat: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool UpdateAlatInDatabase(AlatProyekModel alat)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                // 1. Update alat proyek di tabel alat_proyeks
                string updateQuery = @"
                    UPDATE alat_proyeks
                    SET kategori_id = @kategori_id,
                        lokasi_id = @lokasi_id,
                        nama_alat = @nama_alat,
                        deskripsi = @deskripsi,
                        harga_sewa_harian = @harga,
                        stok = @stok,
                        stok_tersedia = @stok_tersedia,
                        kondisi = @kondisi,
                        status = @status,
                        gambar = @gambar,
                        updated_at = NOW()
                    WHERE id = @id";

                int affected;

                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection, transaction))
                {
                    updateCmd.Parameters.AddWithValue("@id", alat.Id);
                    updateCmd.Parameters.AddWithValue("@kategori_id", alat.KategoriId);
                    updateCmd.Parameters.AddWithValue("@lokasi_id", alat.LokasiId);
                    updateCmd.Parameters.AddWithValue("@nama_alat", alat.NamaAlat);
                    updateCmd.Parameters.AddWithValue("@deskripsi", (object)alat.Deskripsi ?? DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@harga", alat.HargaSewaHarian);
                    updateCmd.Parameters.AddWithValue("@stok", alat.Stok);
                    updateCmd.Parameters.AddWithValue("@stok_tersedia", alat.StokTersedia);
                    updateCmd.Parameters.AddWithValue("@kondisi", alat.Kondisi);
                    updateCmd.Parameters.AddWithValue("@status", alat.Status);
                    updateCmd.Parameters.AddWithValue("@gambar", (object)alat.Gambar ?? DBNull.Value);

                    affected = updateCmd.ExecuteNonQuery();

                    if (affected <= 0)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                // 2. Catat aktivitas update alat proyek ke activity_logs
                ulong currentUserId = SessionManager.GetCurrentUserId();

                if (currentUserId > 0)
                {
                    string logQuery = @"
                        INSERT INTO activity_logs
                        (user_id, aktivitas, modul, referensi_id, ip_address, created_at)
                        VALUES
                        (@userId, @aktivitas, @modul, @referensiId, @ipAddress, NOW())";

                    using (MySqlCommand logCmd = new MySqlCommand(logQuery, connection, transaction))
                    {
                        string activityDescription = $"Memperbarui alat proyek '{alat.NamaAlat}' (kode {alat.KodeAlat}) dengan status {alat.Status}";

                        logCmd.Parameters.AddWithValue("@userId", currentUserId);
                        logCmd.Parameters.AddWithValue("@aktivitas", activityDescription);
                        logCmd.Parameters.AddWithValue("@modul", "Alat Proyek");
                        logCmd.Parameters.AddWithValue("@referensiId", alat.Id);
                        logCmd.Parameters.AddWithValue("@ipAddress", GetClientIpAddress());

                        int logResult = logCmd.ExecuteNonQuery();

                        if (logResult <= 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try { transaction.Rollback(); }
                    catch { }
                }

                MessageBox.Show($"Error mengupdate alat: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    connection.Dispose();
                }
            }
        }

        private string GetClientIpAddress()
        {
            try
            {
                string hostName = System.Net.Dns.GetHostName();
                var addresses = System.Net.Dns.GetHostAddresses(hostName);

                foreach (var address in addresses)
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return address.ToString();
                    }
                }

                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                if (_alatData == null)
                {
                    MessageBox.Show("Data alat belum dimuat!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNama.Text))
                {
                    MessageBox.Show("Nama alat wajib diisi!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNama.Focus();
                    return;
                }

                string kode = txtKode.Text.Trim();
                if (string.IsNullOrEmpty(kode))
                {
                    MessageBox.Show("Kode alat wajib diisi!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtKode.Focus();
                    return;
                }

                if (IsKodeExists(kode, _alatId))
                {
                    MessageBox.Show("Kode alat sudah digunakan alat lain. Silakan gunakan kode lain.",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtKode.Focus();
                    txtKode.SelectAll();
                    return;
                }

                ulong kategoriId = GetSelectedId(cbKategori);
                ulong lokasiId = GetSelectedId(cbLokasi);

                if (kategoriId == 0)
                {
                    MessageBox.Show("Silakan pilih kategori!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (lokasiId == 0)
                {
                    MessageBox.Show("Silakan pilih lokasi!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtHarga.Text.Replace("Rp ", "").Replace(".", ""), out decimal harga) || harga < 0)
                {
                    MessageBox.Show("Harga sewa harus berupa angka yang valid!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtHarga.Focus();
                    return;
                }

                if (!int.TryParse(txtStok.Text, out int stok) || stok < 0)
                {
                    MessageBox.Show("Stok harus berupa angka yang valid!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStok.Focus();
                    return;
                }

                if (!int.TryParse(txtStokTersedia.Text, out int stokTersedia) || stokTersedia < 0 || stokTersedia > stok)
                {
                    MessageBox.Show("Stok tersedia harus angka valid dan tidak melebihi total stok!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStokTersedia.Focus();
                    return;
                }

                string kondisi = cbKondisi.SelectedItem?.ToString() ?? "baik";
                string status = cbStatus.SelectedItem?.ToString() ?? "tersedia";
                string deskripsi = txtDeskripsi.Text.Trim();

                _alatData.KodeAlat = kode;
                _alatData.KategoriId = kategoriId;
                _alatData.LokasiId = lokasiId;
                _alatData.NamaAlat = txtNama.Text.Trim();
                _alatData.Deskripsi = string.IsNullOrEmpty(deskripsi) ? null : deskripsi;
                _alatData.HargaSewaHarian = harga;
                _alatData.Stok = stok;
                _alatData.StokTersedia = stokTersedia;
                _alatData.Kondisi = kondisi;
                _alatData.Status = status;

                string oldGambar = _alatData.Gambar;
                string newlySavedGambar = null;

                if (_gambarChanged && !string.IsNullOrWhiteSpace(_gambarSourcePath))
                {
                    newlySavedGambar = AlatProyekGambarHelper.SaveImageFile(_gambarSourcePath, kode);
                    if (string.IsNullOrEmpty(newlySavedGambar))
                    {
                        MessageBox.Show("Gagal menyimpan file gambar!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    _alatData.Gambar = newlySavedGambar;
                }
                else if (_gambarRemoved)
                {
                    _alatData.Gambar = null;
                }

                if (UpdateAlatInDatabase(_alatData))
                {
                    if (_gambarChanged && !string.IsNullOrWhiteSpace(oldGambar) && oldGambar != newlySavedGambar)
                    {
                        AlatProyekGambarHelper.DeleteImageFile(oldGambar);
                    }
                    else if (_gambarRemoved && !string.IsNullOrWhiteSpace(oldGambar))
                    {
                        AlatProyekGambarHelper.DeleteImageFile(oldGambar);
                    }

                    MessageBox.Show("Alat berhasil diupdate!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    if (!string.IsNullOrEmpty(newlySavedGambar))
                    {
                        AlatProyekGambarHelper.DeleteImageFile(newlySavedGambar);
                    }
                    MessageBox.Show("Gagal mengupdate alat. Silakan coba lagi.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi kesalahan: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPilihGambar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Pilih Gambar Alat";
                ofd.Filter = "File Gambar|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp|Semua File|*.*";
                ofd.CheckFileExists = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (!AlatProyekGambarHelper.IsImage(ofd.FileName))
                    {
                        MessageBox.Show("File yang dipilih bukan gambar yang valid!",
                            "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _gambarSourcePath = ofd.FileName;
                    _gambarChanged = true;
                    _gambarRemoved = false;

                    picGambar.Image?.Dispose();
                    try
                    {
                        picGambar.Image = System.Drawing.Image.FromFile(ofd.FileName);
                    }
                    catch
                    {
                        picGambar.Image = null;
                    }
                }
            }
        }

        private void btnHapusGambar_Click(object sender, EventArgs e)
        {
            _gambarSourcePath = null;
            _gambarChanged = false;
            _gambarRemoved = true;

            picGambar.Image?.Dispose();
            picGambar.Image = null;
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            base.OnFormClosing(e);
        }
    }
}
