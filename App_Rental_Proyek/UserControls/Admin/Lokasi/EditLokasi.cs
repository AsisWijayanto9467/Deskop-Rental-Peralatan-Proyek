using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.Lokasi
{
    public partial class EditLokasi : Form
    {
        public bool InitializationSucceeded { get; private set; } = true;
        public string InitializationErrorMessage { get; private set; }

        private ulong _lokasiId;
        private LokasiModel _lokasiData;

        public EditLokasi(ulong lokasiId)
        {
            InitializeComponent();
            _lokasiId = lokasiId;
            InitializeForm();
            LoadLokasiData();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void EditLokasi_Load(object sender, EventArgs e)
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

        private void LoadLokasiData()
        {
            try
            {
                string query = @"
                    SELECT id, nama_lokasi, alamat, keterangan, status, created_at, updated_at
                    FROM lokasis
                    WHERE id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _lokasiId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    _lokasiData = new LokasiModel
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        NamaLokasi = row["nama_lokasi"]?.ToString() ?? "",
                        Alamat = row["alamat"]?.ToString() ?? "",
                        Keterangan = row["keterangan"]?.ToString(),
                        Status = row["status"]?.ToString() ?? "aktif",
                        CreatedAt = row["created_at"] as DateTime?,
                        UpdatedAt = row["updated_at"] as DateTime?
                    };
                }
                else
                {
                    InitializationSucceeded = false;
                    InitializationErrorMessage = "Data lokasi tidak ditemukan!";
                    MessageBox.Show(InitializationErrorMessage, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                InitializationSucceeded = false;
                InitializationErrorMessage = $"Error memuat data lokasi: {ex.Message}";
                MessageBox.Show(InitializationErrorMessage, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void SetControlValues()
        {
            if (_lokasiData == null) return;

            txtNama.Text = _lokasiData.NamaLokasi;
            txtAlamat.Text = _lokasiData.Alamat;
            txtKeterangan.Text = _lokasiData.Keterangan ?? "";

            if (cbStatus.Items.Count > 0)
            {
                if (_lokasiData.Status == "nonaktif")
                    cbStatus.SelectedIndex = 1;
                else
                    cbStatus.SelectedIndex = 0;
            }
        }

        private bool IsNamaExists(string nama)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM lokasis WHERE LOWER(nama_lokasi) = LOWER(@nama) AND id != @id";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@nama", nama),
                    new MySqlParameter("@id", _lokasiId)
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

        private bool UpdateLokasiInDatabase(LokasiModel lokasi)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                // 1. Update lokasi di tabel lokasis
                string updateQuery = @"
                    UPDATE lokasis
                    SET nama_lokasi = @nama, alamat = @alamat, keterangan = @keterangan,
                        status = @status, updated_at = NOW()
                    WHERE id = @id";

                int affected;

                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection, transaction))
                {
                    updateCmd.Parameters.AddWithValue("@id", lokasi.Id);
                    updateCmd.Parameters.AddWithValue("@nama", lokasi.NamaLokasi);
                    updateCmd.Parameters.AddWithValue("@alamat", lokasi.Alamat);
                    updateCmd.Parameters.AddWithValue("@keterangan", (object)lokasi.Keterangan ?? DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@status", lokasi.Status);

                    affected = updateCmd.ExecuteNonQuery();

                    if (affected <= 0)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                // 2. Catat aktivitas update lokasi ke activity_logs
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
                        string activityDescription = $"Memperbarui lokasi '{lokasi.NamaLokasi}' dengan status {lokasi.Status}";

                        logCmd.Parameters.AddWithValue("@userId", currentUserId);
                        logCmd.Parameters.AddWithValue("@aktivitas", activityDescription);
                        logCmd.Parameters.AddWithValue("@modul", "Lokasi");
                        logCmd.Parameters.AddWithValue("@referensiId", lokasi.Id);
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

                MessageBox.Show($"Error mengupdate lokasi: {ex.Message}", "Error",
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
                if (_lokasiData == null)
                {
                    MessageBox.Show("Data lokasi belum dimuat!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

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
                    MessageBox.Show("Nama lokasi sudah digunakan lokasi lain. Silakan gunakan nama lain.",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNama.Focus();
                    txtNama.SelectAll();
                    return;
                }

                string keterangan = txtKeterangan.Text.Trim();
                string status = cbStatus.SelectedItem?.ToString() ?? "aktif";

                _lokasiData.NamaLokasi = nama;
                _lokasiData.Alamat = alamat;
                _lokasiData.Keterangan = string.IsNullOrEmpty(keterangan) ? null : keterangan;
                _lokasiData.Status = status;

                if (UpdateLokasiInDatabase(_lokasiData))
                {
                    MessageBox.Show("Lokasi berhasil diupdate!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gagal mengupdate lokasi. Silakan coba lagi.",
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
