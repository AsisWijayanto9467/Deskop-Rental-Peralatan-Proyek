using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.AlatProyek
{
    public partial class CreateAlatProyek : Form
    {
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
            try
            {
                string query = @"
                    INSERT INTO alat_proyeks (kategori_id, lokasi_id, kode_alat, nama_alat, deskripsi,
                        harga_sewa_harian, stok, stok_tersedia, kondisi, status, created_at, updated_at)
                    VALUES (@kategori_id, @lokasi_id, @kode_alat, @nama_alat, @deskripsi,
                        @harga, @stok, @stok_tersedia, @kondisi, @status, NOW(), NOW())";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@kategori_id", alat.KategoriId),
                    new MySqlParameter("@lokasi_id", alat.LokasiId),
                    new MySqlParameter("@kode_alat", alat.KodeAlat),
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
                MessageBox.Show($"Error menambahkan alat: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
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
                    Status = status
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
