using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using App_Rental_Proyek.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.AlatProyek
{
    public partial class CreateAlatProyek : Form
    {
        private string _gambarSourcePath;

        public CreateAlatProyek()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void CreateAlatProyek_Load(object sender, EventArgs e)
        {
            SetupComboboxes();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void SetupComboboxes()
        {
            // Kategori
            try
            {
                DataTable dt = DatabaseConnection.GetData(
                    "SELECT id, nama_kategori FROM kategoris WHERE status = 'aktif' ORDER BY nama_kategori ASC");
                cbKategori.Items.Clear();
                cbKategori.Tag = dt;
                foreach (DataRow row in dt.Rows)
                {
                    cbKategori.Items.Add(row["nama_kategori"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat data kategori: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Lokasi
            try
            {
                DataTable dt = DatabaseConnection.GetData(
                    "SELECT id, nama_lokasi FROM lokasis WHERE status = 'aktif' ORDER BY nama_lokasi ASC");
                cbLokasi.Items.Clear();
                cbLokasi.Tag = dt;
                foreach (DataRow row in dt.Rows)
                {
                    cbLokasi.Items.Add(row["nama_lokasi"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat data lokasi: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Kondisi
            cbKondisi.Items.Clear();
            cbKondisi.Items.Add("baik");
            cbKondisi.Items.Add("rusak_ringan");
            cbKondisi.Items.Add("rusak_berat");
            cbKondisi.SelectedIndex = 0;

            // Status
            cbStatus.Items.Clear();
            cbStatus.Items.Add("tersedia");
            cbStatus.Items.Add("disewa");
            cbStatus.Items.Add("maintenance");
            cbStatus.Items.Add("tidak_aktif");
            cbStatus.SelectedIndex = 0;
        }

        private ulong GetSelectedId(Guna.UI2.WinForms.Guna2ComboBox combo)
        {
            int idx = combo.SelectedIndex;
            if (idx < 0)
            {
                MessageBox.Show("Silakan pilih data pada daftar yang tersedia!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }
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

        private string GenerateKodeAlat()
        {
            try
            {
                string prefix = "ALT";
                string query = "SELECT COUNT(*) FROM alat_proyeks";
                object count = DatabaseConnection.ExecuteScalar(query);
                int next = Convert.ToInt32(count) + 1;
                return $"{prefix}{DateTime.Now:yyyyMMdd}{next:D4}";
            }
            catch
            {
                return "ALT" + DateTime.Now.Ticks.ToString();
            }
        }

        private bool IsKodeExists(string kode)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM alat_proyeks WHERE kode_alat = @kode";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@kode", kode)
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

        private bool CreateAlatInDatabase(AlatProyekModel alat)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                // 1. Insert alat proyek ke tabel alat_proyeks
                string insertQuery = @"
                    INSERT INTO alat_proyeks (kategori_id, lokasi_id, kode_alat, nama_alat, deskripsi,
                        harga_sewa_harian, stok, stok_tersedia, kondisi, status, gambar, created_at, updated_at)
                    VALUES (@kategori_id, @lokasi_id, @kode_alat, @nama_alat, @deskripsi,
                        @harga, @stok, @stok_tersedia, @kondisi, @status, @gambar, NOW(), NOW());
                    SELECT LAST_INSERT_ID();";

                ulong newAlatId;

                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection, transaction))
                {
                    insertCmd.Parameters.AddWithValue("@kategori_id", alat.KategoriId);
                    insertCmd.Parameters.AddWithValue("@lokasi_id", alat.LokasiId);
                    insertCmd.Parameters.AddWithValue("@kode_alat", alat.KodeAlat);
                    insertCmd.Parameters.AddWithValue("@nama_alat", alat.NamaAlat);
                    insertCmd.Parameters.AddWithValue("@deskripsi", (object)alat.Deskripsi ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@harga", alat.HargaSewaHarian);
                    insertCmd.Parameters.AddWithValue("@stok", alat.Stok);
                    insertCmd.Parameters.AddWithValue("@stok_tersedia", alat.StokTersedia);
                    insertCmd.Parameters.AddWithValue("@kondisi", alat.Kondisi);
                    insertCmd.Parameters.AddWithValue("@status", alat.Status);
                    insertCmd.Parameters.AddWithValue("@gambar", (object)alat.Gambar ?? DBNull.Value);

                    newAlatId = Convert.ToUInt64(insertCmd.ExecuteScalar());

                    if (newAlatId == 0)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                // 2. Catat aktivitas penambahan alat proyek ke activity_logs
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
                        string activityDescription = $"Menambah alat proyek baru '{alat.NamaAlat}' (kode {alat.KodeAlat}) dengan status {alat.Status}";

                        logCmd.Parameters.AddWithValue("@userId", currentUserId);
                        logCmd.Parameters.AddWithValue("@aktivitas", activityDescription);
                        logCmd.Parameters.AddWithValue("@modul", "Alat Proyek");
                        logCmd.Parameters.AddWithValue("@referensiId", newAlatId);
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

                MessageBox.Show($"Error menambahkan alat: {ex.Message}", "Error",
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
                if (string.IsNullOrWhiteSpace(txtNama.Text))
                {
                    MessageBox.Show("Nama alat wajib diisi!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNama.Focus();
                    return;
                }

                ulong kategoriId = GetSelectedId(cbKategori);
                if (kategoriId == 0) return;

                ulong lokasiId = GetSelectedId(cbLokasi);
                if (lokasiId == 0) return;

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

                string kode = txtKode.Text.Trim();
                if (string.IsNullOrEmpty(kode)) kode = GenerateKodeAlat();

                if (IsKodeExists(kode))
                {
                    MessageBox.Show("Kode alat sudah digunakan! Silakan gunakan kode lain.",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtKode.Focus();
                    txtKode.SelectAll();
                    return;
                }

                string kondisi = cbKondisi.SelectedItem?.ToString() ?? "baik";
                string status = cbStatus.SelectedItem?.ToString() ?? "tersedia";
                string deskripsi = txtDeskripsi.Text.Trim();

                string savedGambar = SaveSelectedGambar(kode);

                var alat = new AlatProyekModel
                {
                    KategoriId = kategoriId,
                    LokasiId = lokasiId,
                    KodeAlat = kode,
                    NamaAlat = txtNama.Text.Trim(),
                    Deskripsi = string.IsNullOrEmpty(deskripsi) ? null : deskripsi,
                    HargaSewaHarian = harga,
                    Stok = stok,
                    StokTersedia = stok,
                    Kondisi = kondisi,
                    Status = status,
                    Gambar = savedGambar
                };

                if (CreateAlatInDatabase(alat))
                {
                    MessageBox.Show("Alat berhasil ditambahkan!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    if (!string.IsNullOrEmpty(savedGambar))
                    {
                        AlatProyekGambarHelper.DeleteImageFile(savedGambar);
                    }
                    MessageBox.Show("Gagal menambahkan alat. Silakan coba lagi.",
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
                    try
                    {
                        picGambar.Image?.Dispose();
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
            picGambar.Image?.Dispose();
            picGambar.Image = null;
        }

        private string SaveSelectedGambar(string baseName)
        {
            if (string.IsNullOrWhiteSpace(_gambarSourcePath)) return null;
            return AlatProyekGambarHelper.SaveImageFile(_gambarSourcePath, baseName);
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
