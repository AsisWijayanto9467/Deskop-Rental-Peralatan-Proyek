using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;

namespace App_Rental_Proyek.UserControls.Admin.KategoriManagement
{
    public partial class CreateKategori : Form
    {
        public CreateKategori()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void CreateKategori_Load(object sender, EventArgs e)
        {
            SetupStatusComboBox();
        }

        private void SetupStatusComboBox()
        {
            cbStatus.Items.Clear();
            cbStatus.Items.Add("aktif");
            cbStatus.Items.Add("nonaktif");
            cbStatus.SelectedIndex = 0;
        }

        private bool IsNamaExists(string nama, ulong? excludeId = null)
        {
            try
            {
                string query = excludeId.HasValue
                    ? "SELECT COUNT(*) FROM kategoris WHERE LOWER(nama_kategori) = LOWER(@nama) AND id != @id"
                    : "SELECT COUNT(*) FROM kategoris WHERE LOWER(nama_kategori) = LOWER(@nama)";

                MySqlParameter[] parameters;
                if (excludeId.HasValue)
                {
                    parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@nama", nama),
                        new MySqlParameter("@id", excludeId.Value)
                    };
                }
                else
                {
                    parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@nama", nama)
                    };
                }

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

        private bool CreateKategoriInDatabase(KategoriModel kategori)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                // 1. Insert kategori ke tabel kategoris
                string insertQuery = @"
                    INSERT INTO kategoris (nama_kategori, deskripsi, status, created_at, updated_at)
                    VALUES (@nama, @deskripsi, @status, NOW(), NOW());
                    SELECT LAST_INSERT_ID();";

                ulong newKategoriId;

                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection, transaction))
                {
                    insertCmd.Parameters.AddWithValue("@nama", kategori.NamaKategori);
                    insertCmd.Parameters.AddWithValue("@deskripsi", (object)kategori.Deskripsi ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@status", kategori.Status);

                    newKategoriId = Convert.ToUInt64(insertCmd.ExecuteScalar());

                    if (newKategoriId == 0)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                // 2. Catat aktivitas penambahan kategori ke activity_logs
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
                        string activityDescription = $"Menambah kategori baru '{kategori.NamaKategori}' dengan status {kategori.Status}";

                        logCmd.Parameters.AddWithValue("@userId", currentUserId);
                        logCmd.Parameters.AddWithValue("@aktivitas", activityDescription);
                        logCmd.Parameters.AddWithValue("@modul", "Kategori");
                        logCmd.Parameters.AddWithValue("@referensiId", newKategoriId);
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

                MessageBox.Show($"Error menambahkan kategori: {ex.Message}", "Error",
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
                    MessageBox.Show("Nama kategori sudah digunakan. Silakan gunakan nama lain.",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNama.Focus();
                    txtNama.SelectAll();
                    return;
                }

                string status = cbStatus.SelectedItem?.ToString() ?? "aktif";
                string deskripsi = txtDeskripsi.Text.Trim();

                var kategori = new KategoriModel
                {
                    NamaKategori = nama,
                    Deskripsi = string.IsNullOrEmpty(deskripsi) ? null : deskripsi,
                    Status = status
                };

                if (CreateKategoriInDatabase(kategori))
                {
                    MessageBox.Show("Kategori berhasil ditambahkan!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gagal menambahkan kategori. Silakan coba lagi.",
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
    }
}