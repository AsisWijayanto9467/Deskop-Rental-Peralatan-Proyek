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
using BCrypt.Net;

namespace App_Rental_Proyek.UserControl.Admin
{
    public partial class CreateUser : Form
    {
        private string selectedRole = "user";

        public CreateUser()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void CreateUser_Load(object sender, EventArgs e)
        {
            // Setup default values
            SetupDefaultValues();
        }

        #region Initialization Methods

        private void InitializeForm()
        {
            // Set form properties
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Setup password fields to show dots initially (like Login form)
            txtPassword.PasswordChar = '●';
            txtConfirmPassword.PasswordChar = '●';
        }

        private void SetupDefaultValues()
        {
            // Set default role to "user"
            rdUser.Checked = true;
            selectedRole = "user";

            // Setup Status ComboBox
            SetupStatusComboBox();

            // Clear any error messages
            ClearErrorMessages();
        }

        private void SetupStatusComboBox()
        {
            // Clear existing items
            guna2ComboBox1.Items.Clear();

            // Add status options
            guna2ComboBox1.Items.Add("aktif");
            guna2ComboBox1.Items.Add("nonaktif");

            // Set default selection
            guna2ComboBox1.SelectedIndex = 0; // "aktif"
        }

        private void ClearErrorMessages()
        {
            // Remove error provider if you're using it
            // or just clear any manual error labels
        }

        #endregion

        #region Password Visibility Toggle Methods (Like Login Form)

        private void btnShowPass_Click(object sender, EventArgs e)
        {
            if (btnShowPass.Text == "🔒")
            {
                // Show password
                txtPassword.PasswordChar = '\0';
                btnShowPass.Text = "🔓";
            }
            else
            {
                // Hide password
                txtPassword.PasswordChar = '●';
                btnShowPass.Text = "🔒";
            }
        }

        private void btnShowConfirPass_Click(object sender, EventArgs e)
        {
            if (btnShowConfirPass.Text == "🔒")
            {
                // Show password
                txtConfirmPassword.PasswordChar = '\0';
                btnShowConfirPass.Text = "🔓";
            }
            else
            {
                // Hide password
                txtConfirmPassword.PasswordChar = '●';
                btnShowConfirPass.Text = "🔒";
            }
        }

        #endregion

        #region Role Selection Events

        private void rdAdmin_CheckedChanged(object sender, EventArgs e)
        {
            if (rdAdmin.Checked)
            {
                selectedRole = "admin";
            }
        }

        private void rdUser_CheckedChanged(object sender, EventArgs e)
        {
            if (rdUser.Checked)
            {
                selectedRole = "user";
            }
        }

        private void rdPetugas_CheckedChanged(object sender, EventArgs e)
        {
            if (rdPetugas.Checked)
            {
                selectedRole = "petugas";
            }
        }

        #endregion

        #region Validation Methods

        private bool ValidateInputs()
        {
            bool isValid = true;

            // Validate Nama Lengkap
            if (string.IsNullOrWhiteSpace(txtNamaLengkap.Text))
            {
                MessageBox.Show("Nama lengkap wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNamaLengkap.Focus();
                isValid = false;
            }
            // Validate Username
            else if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                isValid = false;
            }
            else if (txtUsername.Text.Length < 3)
            {
                MessageBox.Show("Username minimal 3 karakter!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                txtUsername.SelectAll();
                isValid = false;
            }
            // Validate Email
            else if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Email wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                isValid = false;
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Format email tidak valid!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                txtEmail.SelectAll();
                isValid = false;
            }
            // Validate No Telepon
            else if (string.IsNullOrWhiteSpace(txtNoTelpon.Text))
            {
                MessageBox.Show("Nomor telepon wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNoTelpon.Focus();
                isValid = false;
            }
            else if (txtNoTelpon.Text.Length < 10)
            {
                MessageBox.Show("Nomor telepon minimal 10 digit!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNoTelpon.Focus();
                txtNoTelpon.SelectAll();
                isValid = false;
            }
            // Validate Password
            else if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                isValid = false;
            }
            else if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password minimal 6 karakter!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                txtPassword.SelectAll();
                isValid = false;
            }
            // Validate Confirm Password
            else if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                MessageBox.Show("Konfirmasi password wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                isValid = false;
            }
            else if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Password tidak cocok!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                txtConfirmPassword.SelectAll();
                isValid = false;
            }
            // Validate Alamat
            else if (string.IsNullOrWhiteSpace(txtAlamat.Text))
            {
                MessageBox.Show("Alamat wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAlamat.Focus();
                isValid = false;
            }

            return isValid;
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

        private bool IsUsernameExists(string username)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM users WHERE username = @username";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@username", username)
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

        private bool IsEmailExists(string email)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM users WHERE email = @email";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@email", email)
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

        #region Save/Create User Method

        private bool CreateUserInDatabase(UserModel user)
        {
            try
            {
                string query = @"
                    INSERT INTO users (nama, username, email, password, no_telepon, alamat, role, status, created_at, updated_at)
                    VALUES (@nama, @username, @email, @password, @no_telepon, @alamat, @role, @status, NOW(), NOW())";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@nama", user.Nama),
                    new MySqlParameter("@username", user.Username),
                    new MySqlParameter("@email", user.Email),
                    new MySqlParameter("@password", user.Password), // Sudah di-hash dengan BCrypt
                    new MySqlParameter("@no_telepon", user.NoTelepon ?? (object)DBNull.Value),
                    new MySqlParameter("@alamat", user.Alamat ?? (object)DBNull.Value),
                    new MySqlParameter("@role", user.Role),
                    new MySqlParameter("@status", user.Status)
                };

                return DatabaseConnection.ExecuteQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating user: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region Button Click Events

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (!ValidateInputs())
                {
                    return;
                }

                // Check if username already exists
                if (IsUsernameExists(txtUsername.Text))
                {
                    MessageBox.Show("Username sudah digunakan! Silakan gunakan username lain.",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsername.Focus();
                    txtUsername.SelectAll();
                    return;
                }

                // Check if email already exists
                if (IsEmailExists(txtEmail.Text))
                {
                    MessageBox.Show("Email sudah terdaftar! Silakan gunakan email lain.",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    txtEmail.SelectAll();
                    return;
                }

                // Get status from combo box
                string status = guna2ComboBox1.SelectedItem?.ToString() ?? "aktif";

                // Hash password menggunakan BCrypt
                string hashedPassword = HashPassword(txtPassword.Text);

                // Check if hashing failed
                if (string.IsNullOrEmpty(hashedPassword))
                {
                    MessageBox.Show("Gagal memproses password. Silakan coba lagi.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create user object
                var newUser = new UserModel
                {
                    Nama = txtNamaLengkap.Text.Trim(),
                    Username = txtUsername.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Password = hashedPassword, // Simpan password yang sudah di-hash
                    NoTelepon = txtNoTelpon.Text.Trim(),
                    Alamat = txtAlamat.Text.Trim(),
                    Role = selectedRole,
                    Status = status
                };

                // Save to database
                if (CreateUserInDatabase(newUser))
                {
                    MessageBox.Show("User berhasil ditambahkan!",
                        "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Set DialogResult to OK so parent form knows to refresh
                    this.DialogResult = DialogResult.OK;
                    this.Close(); // Tutup form setelah berhasil
                }
                else
                {
                    MessageBox.Show("Gagal menambahkan user. Silakan coba lagi.",
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
            // Ask for confirmation if form has data
            if (HasFormData())
            {
                DialogResult result = MessageBox.Show(
                    "Data yang dimasukkan belum disimpan. Yakin ingin kembali?",
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

        // Method untuk hash password menggunakan BCrypt
        private string HashPassword(string password)
        {
            try
            {
                // Generate salt dengan work factor 12 (semakin tinggi semakin aman tapi lambat)
                string salt = BCrypt.Net.BCrypt.GenerateSalt(12);

                // Hash password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

                return hashedPassword;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error hashing password: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        // Method untuk verifikasi password (untuk login nanti)
        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error verifying password: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ResetForm()
        {
            // Clear all text fields
            txtNamaLengkap.Clear();
            txtUsername.Clear();
            txtEmail.Clear();
            txtNoTelpon.Clear();
            txtAlamat.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();

            // Reset password visibility (hide passwords)
            txtPassword.PasswordChar = '●';
            txtConfirmPassword.PasswordChar = '●';
            btnShowPass.Text = "🔒";
            btnShowConfirPass.Text = "🔒";

            // Reset role selection
            rdUser.Checked = true;
            selectedRole = "user";

            // Reset status
            if (guna2ComboBox1.Items.Count > 0)
            {
                guna2ComboBox1.SelectedIndex = 0;
            }

            // Set focus to first field
            txtNamaLengkap.Focus();
        }

        private bool HasFormData()
        {
            return !string.IsNullOrWhiteSpace(txtNamaLengkap.Text) ||
                   !string.IsNullOrWhiteSpace(txtUsername.Text) ||
                   !string.IsNullOrWhiteSpace(txtEmail.Text) ||
                   !string.IsNullOrWhiteSpace(txtNoTelpon.Text) ||
                   !string.IsNullOrWhiteSpace(txtAlamat.Text) ||
                   !string.IsNullOrWhiteSpace(txtPassword.Text) ||
                   !string.IsNullOrWhiteSpace(txtConfirmPassword.Text);
        }

        #endregion

        #region Other Events

        private void txtNamaLengkap_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add real-time validation if needed
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add real-time validation if needed
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add real-time validation if needed
        }

        private void txtNoTelpon_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add real-time validation if needed
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add real-time validation if needed
        }

        private void txtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add real-time validation if needed
        }

        private void txtAlamat_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add real-time validation if needed
        }

        private void txtAlamat_TextChanged_1(object sender, EventArgs e)
        {
            // Optional: Add real-time validation if needed
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Status selection changed
        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e)
        {
            // For additional fields if needed
        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {
            // For additional fields if needed
        }

        #endregion
    }
}