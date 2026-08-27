using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;

namespace App_Rental_Proyek.UserControls.Admin.KategoriManagement
{
    public partial class EditKategori : Form
    {
        public bool InitializationSucceeded { get; private set; } = true;
        public string InitializationErrorMessage { get; private set; }

        private ulong _kategoriId;
        private KategoriModel _kategoriData;

        public EditKategori(ulong kategoriId)
        {
            InitializeComponent();
            _kategoriId = kategoriId;
            InitializeForm();
            LoadKategoriData();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void EditKategori_Load(object sender, EventArgs e)
        {
            SetupStatusComboBox();
            SetControlValues();
        }

        private void SetupStatusComboBox()
        {
            cbStatus.Items.Clear();
            cbStatus.Items.Add("aktif");
            cbStatus.Items.Add("nonaktif");
        }

        private void LoadKategoriData()
        {
            try
            {
                string query = @"
                    SELECT id, nama_kategori, deskripsi, status, created_at, updated_at
                    FROM kategoris
                    WHERE id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _kategoriId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    _kategoriData = new KategoriModel
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        NamaKategori = row["nama_kategori"]?.ToString() ?? "",
                        Deskripsi = row["deskripsi"]?.ToString(),
                        Status = row["status"]?.ToString() ?? "aktif",
                        CreatedAt = row["created_at"] as DateTime?,
                        UpdatedAt = row["updated_at"] as DateTime?
                    };
                }
                else
                {
                    InitializationSucceeded = false;
                    InitializationErrorMessage = "Data kategori tidak ditemukan!";
                    MessageBox.Show(InitializationErrorMessage, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                InitializationSucceeded = false;
                InitializationErrorMessage = $"Error memuat data kategori: {ex.Message}";
                MessageBox.Show(InitializationErrorMessage, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void SetControlValues()
        {
            if (_kategoriData == null) return;

            txtNama.Text = _kategoriData.NamaKategori;
            txtDeskripsi.Text = _kategoriData.Deskripsi ?? "";

            if (cbStatus.Items.Count > 0)
            {
                if (_kategoriData.Status == "nonaktif")
                    cbStatus.SelectedIndex = 1;
                else
                    cbStatus.SelectedIndex = 0;
            }
        }

        private bool IsNamaExists(string nama)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM kategoris WHERE LOWER(nama_kategori) = LOWER(@nama) AND id != @id";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@nama", nama),
                    new MySqlParameter("@id", _kategoriId)
                };

                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                return Convert.ToInt64(result) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memeriksa nama kategori: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool UpdateKategoriInDatabase(KategoriModel kategori)
        {
            try
            {
                string query = @"
                    UPDATE kategoris
                    SET nama_kategori = @nama, deskripsi = @deskripsi, status = @status, updated_at = NOW()
                    WHERE id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", kategori.Id),
                    new MySqlParameter("@nama", kategori.NamaKategori),
                    new MySqlParameter("@deskripsi", (object)kategori.Deskripsi ?? DBNull.Value),
                    new MySqlParameter("@status", kategori.Status)
                };

                return DatabaseConnection.ExecuteQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengupdate kategori: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                if (_kategoriData == null)
                {
                    MessageBox.Show("Data kategori belum dimuat!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string nama = txtNama.Text.Trim();

                if (string.IsNullOrWhiteSpace(nama))
                {
                    MessageBox.Show("Nama kategori wajib diisi!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNama.Focus();
                    return;
                }

                if (IsNamaExists(nama))
                {
                    MessageBox.Show("Nama kategori sudah digunakan kategori lain. Silakan gunakan nama lain.",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNama.Focus();
                    txtNama.SelectAll();
                    return;
                }

                string status = cbStatus.SelectedItem?.ToString() ?? "aktif";
                string deskripsi = txtDeskripsi.Text.Trim();

                _kategoriData.NamaKategori = nama;
                _kategoriData.Deskripsi = string.IsNullOrEmpty(deskripsi) ? null : deskripsi;
                _kategoriData.Status = status;

                if (UpdateKategoriInDatabase(_kategoriData))
                {
                    MessageBox.Show("Kategori berhasil diupdate!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gagal mengupdate kategori. Silakan coba lagi.",
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