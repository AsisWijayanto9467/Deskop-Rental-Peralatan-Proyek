using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        // =========================================================
        // FORM LOAD
        // =========================================================

        private void Login_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtUsername;

            txtPassword.PasswordChar = '●';

            this.KeyPreview = true;

            // Jalankan jika ingin otomatis membuat user default
            // SeedDefaultUsers();
        }

        private void SeedDefaultUsers()
        {
            try
            {
                if (DatabaseConnection.TestConnection())
                {
                    UserSeeder.SeedUsers();

                    Console.WriteLine(
                        "Seeder selesai dijalankan.");
                }
                else
                {
                    MessageBox.Show(
                        "Tidak dapat terhubung ke database!\n\n" +
                        "Periksa konfigurasi koneksi database.",
                        "Database Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saat inisialisasi database:\n{ex.Message}",
                    "Seeder Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        

        // =========================================================
        // AUTHENTICATE USER
        // =========================================================

        private UserModel AuthenticateUser(
            string username,
            string password)
        {
            string query = @"
                SELECT
                    id,
                    nama,
                    username,
                    email,
                    password,
                    no_telepon,
                    alamat,
                    role,
                    status,
                    created_at,
                    updated_at
                FROM users
                WHERE username = @username
                LIMIT 1";

            MySqlParameter[] parameters =
            {
                new MySqlParameter(
                    "@username",
                    username)
            };

            DataTable dt =
                DatabaseConnection.GetData(
                    query,
                    parameters);

            // =====================================================
            // USER TIDAK DITEMUKAN
            // =====================================================

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dt.Rows[0];

            string hashedPassword =
                row["password"]?.ToString() ?? "";

            // =====================================================
            // VERIFIKASI PASSWORD
            // =====================================================

            bool passwordValid;

            try
            {
                passwordValid =
                    BCrypt.Net.BCrypt.Verify(
                        password,
                        hashedPassword);
            }
            catch
            {
                // Backward compatibility jika ada password
                // lama yang masih plain text.
                passwordValid =
                    hashedPassword == password;
            }

            if (!passwordValid)
            {
                return null;
            }

            // =====================================================
            // BUAT USER MODEL
            // =====================================================

            return new UserModel
            {
                Id = Convert.ToUInt64(row["id"]),

                Nama =
                    row["nama"]?.ToString() ?? "",

                Username =
                    row["username"]?.ToString() ?? "",

                Email =
                    row["email"]?.ToString() ?? "",

                Password =
                    hashedPassword,

                NoTelepon =
                    row["no_telepon"] == DBNull.Value
                        ? null
                        : row["no_telepon"]?.ToString(),

                Alamat =
                    row["alamat"] == DBNull.Value
                        ? null
                        : row["alamat"]?.ToString(),

                Role =
                    row["role"]?.ToString() ?? "user",

                Status =
                    row["status"]?.ToString() ?? "aktif",

                CreatedAt =
                    row["created_at"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(row["created_at"]),

                UpdatedAt =
                    row["updated_at"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(row["updated_at"])
            };
        }

        private void RedirectByRole(UserModel user)
        {
            Form dashboardForm = null;

            switch (user.Role.ToLower())
            {

                case "admin":
                    dashboardForm =
                        new AdminDashboard();
                    break;

                case "petugas":
                    dashboardForm =
                        new PetugasDashboard();
                    break;

                default:
                    MessageBox.Show(
                        $"Role '{user.Role}' tidak dikenali!",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
            }

            if (dashboardForm != null)
            {
                // Kirim informasi user ke dashboard
                dashboardForm.Tag = user;

                this.Hide();

                dashboardForm.ShowDialog();

                this.Close();
            }
        }


        protected override bool ProcessCmdKey(
            ref Message msg,
            Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                btnSignIn.PerformClick();

                return true;
            }

            return base.ProcessCmdKey(
                ref msg,
                keyData);
        }

        private void ckShowPassword_CheckedChanged_1(object sender, EventArgs e)
        {
            if (ckShowPassword.Checked)
            {
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar = '●';
            }
        }

        private void btnSignIn_Click_1(object sender, EventArgs e)
        {
            // =====================================================
            // VALIDASI USERNAME
            // =====================================================

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show(
                    "Username tidak boleh kosong!",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtUsername.Focus();

                return;
            }

            // =====================================================
            // VALIDASI PASSWORD
            // =====================================================

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show(
                    "Password tidak boleh kosong!",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPassword.Focus();

                return;
            }

            try
            {
                // =================================================
                // AUTHENTICATE USER
                // =================================================

                UserModel user = AuthenticateUser(
                    txtUsername.Text.Trim(),
                    txtPassword.Text);

                // =================================================
                // LOGIN BERHASIL
                // =================================================

                if (user != null)
                {
                    // =============================================
                    // CEK STATUS USER
                    // =============================================

                    if (user.Status.ToLower() != "aktif")
                    {
                        MessageBox.Show(
                            "Akun Anda nonaktif.\n" +
                            "Hubungi administrator!",
                            "Login Gagal",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    // =============================================
                    // SET USER YANG SEDANG LOGIN
                    // =============================================

                    Helper.Session.CurrentUser = user;

                    // =============================================
                    // LOGIN BERHASIL
                    // =============================================

                    MessageBox.Show(
                        $"Selamat datang, {user.Nama}!",
                        "Login Berhasil",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // =============================================
                    // REDIRECT BERDASARKAN ROLE
                    // =============================================

                    RedirectByRole(user);
                }
                else
                {
                    MessageBox.Show(
                        "Username atau password salah!",
                        "Login Gagal",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Terjadi kesalahan:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}