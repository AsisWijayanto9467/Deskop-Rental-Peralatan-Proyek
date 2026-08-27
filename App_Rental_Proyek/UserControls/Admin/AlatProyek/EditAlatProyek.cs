using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
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
            try
            {
                string query = @"
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
                        updated_at = NOW()
                    WHERE id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", alat.Id),
                    new MySqlParameter("@kategori_id", alat.KategoriId),
                    new MySqlParameter("@lokasi_id", alat.LokasiId),
                    new MySqlParameter("@nama_alat", alat.NamaAlat),
                    new MySqlParameter("@deskripsi", (object)alat.Deskripsi ?? DBNull.Value),
                    new MySqlParameter("@harga", alat.HargaSewaHarian),
                    new MySqlParameter("@stok", alat.Stok),
                    new MySqlParameter("@stok_tersedia", alat.StokTersedia),
                    new MySqlParameter("@kondisi", alat.Kondisi),
                    new MySqlParameter("@status", alat.Status)
                };

                return DatabaseConnection.ExecuteQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengupdate alat: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
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

                if (UpdateAlatInDatabase(_alatData))
                {
                    MessageBox.Show("Alat berhasil diupdate!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
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
