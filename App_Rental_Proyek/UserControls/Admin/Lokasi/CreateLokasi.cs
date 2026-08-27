using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.Lokasi
{
    public partial class CreateLokasi : Form
    {
        public CreateLokasi()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void CreateLokasi_Load(object sender, EventArgs e)
        {
            SetupStatusComboBox();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void SetupStatusComboBox()
        {
            cbStatus.Items.Clear();
            cbStatus.Items.Add("aktif");
            cbStatus.Items.Add("nonaktif");
            cbStatus.SelectedIndex = 0;
        }

        private bool IsNamaExists(string nama)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM lokasis WHERE LOWER(nama_lokasi) = LOWER(@nama)";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@nama", nama)
                };
                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                return Convert.ToInt64(result) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memeriksa nama lokasi: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool CreateLokasiInDatabase(LokasiModel lokasi)
        {
            try
            {
                string query = @"
                    INSERT INTO lokasis (nama_lokasi, alamat, keterangan, status, created_at, updated_at)
                    VALUES (@nama, @alamat, @keterangan, @status, NOW(), NOW())";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@nama", lokasi.NamaLokasi),
                    new MySqlParameter("@alamat", lokasi.Alamat),
                    new MySqlParameter("@keterangan", (object)lokasi.Keterangan ?? DBNull.Value),
                    new MySqlParameter("@status", lokasi.Status)
                };

                return DatabaseConnection.ExecuteQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error menambahkan lokasi: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                string nama = txtNama.Text.Trim();
                string alamat = txtAlamat.Text.Trim();

                if (string.IsNullOrWhiteSpace(nama))
                {
                    MessageBox.Show("Nama lokasi wajib diisi!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNama.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(alamat))
                {
                    MessageBox.Show("Alamat wajib diisi!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAlamat.Focus();
                    return;
                }

                if (IsNamaExists(nama))
                {
                    MessageBox.Show("Nama lokasi sudah digunakan. Silakan gunakan nama lain.",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNama.Focus();
                    txtNama.SelectAll();
                    return;
                }

                string keterangan = txtKeterangan.Text.Trim();
                string status = cbStatus.SelectedItem?.ToString() ?? "aktif";

                var lokasi = new LokasiModel
                {
                    NamaLokasi = nama,
                    Alamat = alamat,
                    Keterangan = string.IsNullOrEmpty(keterangan) ? null : keterangan,
                    Status = status
                };

                if (CreateLokasiInDatabase(lokasi))
                {
                    MessageBox.Show("Lokasi berhasil ditambahkan!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gagal menambahkan lokasi. Silakan coba lagi.",
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
