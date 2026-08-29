using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using App_Rental_Proyek.Helper;

namespace App_Rental_Proyek.UserControls.Admin.UserManagement
{
    public partial class EditUser : Form
    {
        public bool InitializationSucceeded { get; private set; } = true;
        public string InitializationErrorMessage { get; private set; }
        private ulong _userId;
        private UserModel _userData;
        private string selectedRole = "user";
        private bool _isClosing = false;

        // ❌ HAPUS field ini - tidak perlu lagi
        // private UserModel _currentLoggedInUser;

        public EditUser(ulong userId)
        {
            InitializeComponent();
            _userId = userId;
            InitializeForm();
            LoadUserData();
        }

        private void EditUser_Load(object sender, EventArgs e)
        {
            SetupDefaultValues();

            // ✅ Gunakan SessionManager untuk cek session
            if (!SessionManager.IsLoggedIn)
            {
                System.Diagnostics.Debug.WriteLine("Warning: EditUser - Tidak ada user login!");
                // Tampilkan peringatan tapi tetap lanjutkan
                MessageBox.Show("Session user tidak ditemukan. Aktivitas tidak akan dicatat ke log.",
                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #region Initialization Methods

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            txtPassword.PasswordChar = '●';
            txtConfirmPassword.PasswordChar = '●';

            txtPassword.PlaceholderText = "Kosongkan jika tidak ingin mengubah password";
            txtConfirmPassword.PlaceholderText = "Kosongkan jika tidak ingin mengubah password";

            SetDefaultNumericValues();
        }

        private void SetDefaultNumericValues()
        {
            try
            {
                // Cek dan set Guna2ProgressBar jika ada
                // if (guna2ProgressBar1 != null)
                // {
                //     guna2ProgressBar1.Minimum = 0;
                //     guna2ProgressBar1.Maximum = 100;
                //     guna2ProgressBar1.Value = 1; // JANGAN 0!
                // }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting numeric defaults: {ex.Message}");
            }
        }

        private void SetupDefaultValues()
        {
            SetupStatusComboBox();
        }

        private void SetupStatusComboBox()
        {
            try
            {
                guna2ComboBox1.Items.Clear();
                guna2ComboBox1.Items.Add("aktif");
                guna2ComboBox1.Items.Add("nonaktif");

                if (guna2ComboBox1.Items.Count > 0)
                {
                    guna2ComboBox1.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up combo box: {ex.Message}");
            }
        }

        private void LoadUserData()
        {
            try
            {
                string query = @"
                    SELECT id, nama, username, email, password, no_telepon, alamat, role, status, created_at, updated_at
                    FROM users 
                    WHERE id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _userId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    _userData = new UserModel
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        Nama = row["nama"]?.ToString() ?? "",
                        Username = row["username"]?.ToString() ?? "",
                        Email = row["email"]?.ToString() ?? "",
                        Password = row["password"]?.ToString() ?? "",
                        NoTelepon = row["no_telepon"]?.ToString(),
                        Alamat = row["alamat"]?.ToString(),
                        Role = row["role"]?.ToString() ?? "user",
                        Status = row["status"]?.ToString() ?? "aktif",
                        CreatedAt = row["created_at"] as DateTime?,
                        UpdatedAt = row["updated_at"] as DateTime?
                    };

                    SetControlValuesSafely();
                    selectedRole = _userData.Role;
                }
                else
                {
                    InitializationSucceeded = false;
                    InitializationErrorMessage = "Data user tidak ditemukan!";
                    MessageBox.Show(InitializationErrorMessage, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                InitializationSucceeded = false;
                InitializationErrorMessage = $"Error loading user data: {ex.Message}";
                MessageBox.Show(InitializationErrorMessage, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void SetControlValuesSafely()
        {
            try
            {
                SetGuna2TextBoxSafely(txtNamaLengkap, _userData.Nama, "txtNamaLengkap");
                SetGuna2TextBoxSafely(txtUsername, _userData.Username, "txtUsername");
                SetGuna2TextBoxSafely(txtEmail, _userData.Email, "txtEmail");
                SetGuna2TextBoxSafely(txtNoTelpon, _userData.NoTelepon ?? "", "txtNoTelpon");
                SetGuna2TextBoxSafely(txtAlamat, _userData.Alamat ?? "", "txtAlamat");
                SetGuna2TextBoxSafely(txtPassword, "", "txtPassword");
                SetGuna2TextBoxSafely(txtConfirmPassword, "", "txtConfirmPassword");

                SetRoleSafely();
                SetStatusComboBoxSafely();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error setting control values: {ex.Message}", ex);
            }
        }

        private void SetGuna2TextBoxSafely(Guna.UI2.WinForms.Guna2TextBox control, string value, string controlName)
        {
            try
            {
                if (control != null)
                {
                    control.Text = value ?? "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting {controlName}: {ex.Message}");
                throw new Exception($"Error pada kontrol '{controlName}': {ex.Message}", ex);
            }
        }

        private void SetTextBoxSafely(TextBox control, string value, string controlName)
        {
            try
            {
                if (control != null)
                {
                    control.Text = value ?? "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting {controlName}: {ex.Message}");
                throw new Exception($"Error pada kontrol '{controlName}': {ex.Message}", ex);
            }
        }

        private void SetNumericControlSafely(dynamic control, int value, string controlName)
        {
            try
            {
                if (control != null)
                {
                    try
                    {
                        if (value == 0 && control.Minimum == 0)
                        {
                            value = 1;
                            System.Diagnostics.Debug.WriteLine($"Warning: {controlName} value changed from 0 to 1");
                        }
                        control.Value = value;
                    }
                    catch (Exception innerEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error setting {controlName}: {innerEx.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting {controlName}: {ex.Message}");
                throw new Exception($"Error pada kontrol '{controlName}': {ex.Message}", ex);
            }
        }

        private void SetRoleSafely()
        {
            try
            {
                switch (_userData.Role)
                {
                    case "admin":
                        rdAdmin.Checked = true;
                        break;
                    case "petugas":
                        rdPetugas.Checked = true;
                        break;
                    default:
                        rdUser.Checked = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting role: {ex.Message}");
                throw new Exception($"Error pada role radio buttons: {ex.Message}", ex);
            }
        }

        private void SetStatusComboBoxSafely()
        {
            try
            {
                if (guna2ComboBox1.Items.Count > 0)
                {
                    if (_userData.Status == "aktif")
                    {
                        guna2ComboBox1.SelectedIndex = 0;
                    }
                    else if (_userData.Status == "nonaktif")
                    {
                        guna2ComboBox1.SelectedIndex = 1;
                    }
                    else
                    {
                        guna2ComboBox1.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting status combo box: {ex.Message}");
                throw new Exception($"Error pada status combo box: {ex.Message}", ex);
            }
        }

        #endregion

        #region Password Visibility Toggle Methods

        private void btnShowPass_Click(object sender, EventArgs e)
        {
            if (btnShowPass.Text == "🔒")
            {
                txtPassword.PasswordChar = '\0';
                btnShowPass.Text = "🔓";
            }
            else
            {
                txtPassword.PasswordChar = '●';
                btnShowPass.Text = "🔒";
            }
        }

        private void btnShowConfirPass_Click(object sender, EventArgs e)
        {
            if (btnShowConfirPass.Text == "🔒")
            {
                txtConfirmPassword.PasswordChar = '\0';
                btnShowConfirPass.Text = "🔓";
            }
            else
            {
                txtConfirmPassword.PasswordChar = '●';
                btnShowConfirPass.Text = "🔒";
            }
        }

        #endregion

        #region Role Selection Events

        private void rdAdmin_CheckedChanged(object sender, EventArgs e)
        {
            if (rdAdmin.Checked) selectedRole = "admin";
        }

        private void rdUser_CheckedChanged(object sender, EventArgs e)
        {
            if (rdUser.Checked) selectedRole = "user";
        }

        private void rdPetugas_CheckedChanged(object sender, EventArgs e)
        {
            if (rdPetugas.Checked) selectedRole = "petugas";
        }

        #endregion

        #region Validation Methods

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtNamaLengkap.Text))
            {
                MessageBox.Show("Nama lengkap wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNamaLengkap.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (txtUsername.Text.Length < 3)
            {
                MessageBox.Show("Username minimal 3 karakter!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                txtUsername.SelectAll();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Email wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Format email tidak valid!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                txtEmail.SelectAll();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNoTelpon.Text))
            {
                MessageBox.Show("Nomor telepon wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNoTelpon.Focus();
                return false;
            }

            if (txtNoTelpon.Text.Length < 10)
            {
                MessageBox.Show("Nomor telepon minimal 10 digit!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNoTelpon.Focus();
                txtNoTelpon.SelectAll();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtPassword.Text) && txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password minimal 6 karakter!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                txtPassword.SelectAll();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtPassword.Text) &&
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                MessageBox.Show("Konfirmasi password wajib diisi jika mengubah password!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtPassword.Text) &&
                txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Password tidak cocok!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                txtConfirmPassword.SelectAll();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAlamat.Text))
            {
                MessageBox.Show("Alamat wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAlamat.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsUsernameExists(string username, ulong excludeId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM users WHERE username = @username AND id != @id";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@username", username),
                    new MySqlParameter("@id", excludeId)
                };

                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                return Convert.ToInt64(result) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking username: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool IsEmailExists(string email, ulong excludeId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM users WHERE email = @email AND id != @id";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@email", email),
                    new MySqlParameter("@id", excludeId)
                };

                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                return Convert.ToInt64(result) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking email: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region Update User with Activity Log - Combined Method

        /// <summary>
        /// Update user dan catat aktivitas dalam satu transaction
        /// </summary>
        private bool UpdateUserWithActivityLog(UserModel user, bool updatePassword, string oldData)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                // 1. Update user di tabel users
                string updateQuery;

                if (updatePassword)
                {
                    updateQuery = @"
                        UPDATE users
                        SET nama = @nama,
                            username = @username,
                            email = @email,
                            password = @password,
                            no_telepon = @no_telepon,
                            alamat = @alamat,
                            role = @role,
                            status = @status,
                            updated_at = NOW()
                        WHERE id = @id";
                }
                else
                {
                    updateQuery = @"
                        UPDATE users
                        SET nama = @nama,
                            username = @username,
                            email = @email,
                            no_telepon = @no_telepon,
                            alamat = @alamat,
                            role = @role,
                            status = @status,
                            updated_at = NOW()
                        WHERE id = @id";
                }

                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection, transaction))
                {
                    updateCmd.Parameters.AddWithValue("@id", user.Id);
                    updateCmd.Parameters.AddWithValue("@nama", user.Nama);
                    updateCmd.Parameters.AddWithValue("@username", user.Username);
                    updateCmd.Parameters.AddWithValue("@email", user.Email);

                    if (updatePassword)
                    {
                        updateCmd.Parameters.AddWithValue("@password", user.Password);
                    }

                    updateCmd.Parameters.AddWithValue("@no_telepon", (object)user.NoTelepon ?? DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@alamat", (object)user.Alamat ?? DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@role", user.Role);
                    updateCmd.Parameters.AddWithValue("@status", user.Status);

                    int updateResult = updateCmd.ExecuteNonQuery();

                    if (updateResult <= 0)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                // ✅ Gunakan SessionManager untuk mendapatkan user ID
                ulong currentUserId = SessionManager.GetCurrentUserId();

                // ✅ Jika user tidak login, skip log
                if (currentUserId == 0)
                {
                    transaction.Commit();
                    return true;
                }

                // 2. Catat aktivitas update ke activity_logs
                string logQuery = @"
                    INSERT INTO activity_logs 
                    (user_id, aktivitas, modul, referensi_id, ip_address, created_at) 
                    VALUES 
                    (@userId, @aktivitas, @modul, @referensiId, @ipAddress, NOW())";

                using (MySqlCommand logCmd = new MySqlCommand(logQuery, connection, transaction))
                {
                    string ipAddress = GetClientIpAddress();

                    string activityDescription;
                    if (updatePassword)
                    {
                        activityDescription = $"Mengupdate user '{user.Username}' (dengan perubahan password)";
                    }
                    else
                    {
                        activityDescription = $"Mengupdate user '{user.Username}'";
                    }

                    logCmd.Parameters.AddWithValue("@userId", currentUserId);
                    logCmd.Parameters.AddWithValue("@aktivitas", activityDescription);
                    logCmd.Parameters.AddWithValue("@modul", "User Management");
                    logCmd.Parameters.AddWithValue("@referensiId", user.Id);
                    logCmd.Parameters.AddWithValue("@ipAddress", ipAddress);

                    int logResult = logCmd.ExecuteNonQuery();

                    if (logResult <= 0)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try { transaction.Rollback(); } catch { }
                }

                MessageBox.Show($"Error updating user: {ex.Message}", "Error",
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

        /// <summary>
        /// Get Client IP Address
        /// </summary>
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

        #endregion

        #region Button Click Events

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                if (_userData == null)
                {
                    MessageBox.Show("Data user belum dimuat!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!ValidateInputs())
                {
                    return;
                }

                if (IsUsernameExists(txtUsername.Text.Trim(), _userId))
                {
                    MessageBox.Show("Username sudah digunakan oleh user lain! Silakan gunakan username lain.",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsername.Focus();
                    txtUsername.SelectAll();
                    return;
                }

                if (IsEmailExists(txtEmail.Text.Trim(), _userId))
                {
                    MessageBox.Show("Email sudah terdaftar oleh user lain! Silakan gunakan email lain.",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    txtEmail.SelectAll();
                    return;
                }

                bool updatePassword = !string.IsNullOrWhiteSpace(txtPassword.Text);
                string password = updatePassword ? txtPassword.Text : _userData.Password;
                string status = guna2ComboBox1.SelectedItem?.ToString() ?? "aktif";

                // Simpan data lama untuk activity log
                string oldUsername = _userData.Username;

                // Update data user
                _userData.Nama = txtNamaLengkap.Text.Trim();
                _userData.Username = txtUsername.Text.Trim();
                _userData.Email = txtEmail.Text.Trim();
                _userData.Password = password;
                _userData.NoTelepon = txtNoTelpon.Text.Trim();
                _userData.Alamat = txtAlamat.Text.Trim();
                _userData.Role = selectedRole;
                _userData.Status = status;

                if (UpdateUserWithActivityLog(_userData, updatePassword, oldUsername))
                {
                    string message = updatePassword
                        ? "User berhasil diupdate dengan password baru!"
                        : "User berhasil diupdate! (Password tidak diubah)";

                    MessageBox.Show(message,
                        "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gagal mengupdate user. Silakan coba lagi.",
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
            if (_userData != null && HasFormChanges())
            {
                DialogResult result = MessageBox.Show(
                    "Perubahan yang dibuat belum disimpan. Yakin ingin kembali?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        #endregion

        #region Helper Methods

        private bool HasFormChanges()
        {
            if (_userData == null) return false;

            if (txtNamaLengkap.Text != _userData.Nama) return true;
            if (txtUsername.Text != _userData.Username) return true;
            if (txtEmail.Text != _userData.Email) return true;
            if (txtNoTelpon.Text != (_userData.NoTelepon ?? "")) return true;
            if (txtAlamat.Text != (_userData.Alamat ?? "")) return true;
            if (!string.IsNullOrWhiteSpace(txtPassword.Text)) return true;
            if (selectedRole != _userData.Role) return true;

            string currentStatus = guna2ComboBox1.SelectedItem?.ToString() ?? "aktif";
            if (currentStatus != _userData.Status) return true;

            return false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            base.OnFormClosing(e);
        }

        #endregion

        #region Other Events

        private void txtNamaLengkap_TextChanged(object sender, EventArgs e) { }
        private void txtUsername_TextChanged(object sender, EventArgs e) { }
        private void txtEmail_TextChanged(object sender, EventArgs e) { }
        private void txtNoTelpon_TextChanged(object sender, EventArgs e) { }
        private void txtAlamat_TextChanged(object sender, EventArgs e) { }
        private void txtPassword_TextChanged(object sender, EventArgs e) { }
        private void txtConfirmPassword_TextChanged(object sender, EventArgs e) { }
        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        #endregion
    }
}