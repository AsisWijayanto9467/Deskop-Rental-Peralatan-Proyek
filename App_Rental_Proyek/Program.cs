using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            // =====================================================
            // INISIALISASI DATABASE & SEEDER
            // =====================================================

            try
            {
                if (!DatabaseConnection.TestConnection())
                {
                    MessageBox.Show(
                        "Tidak dapat terhubung ke database!\n\n" +
                        "Periksa konfigurasi koneksi database Anda.",
                        "Database Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    Application.Run(new Login());
                    return;
                }

                // Seeder otomatis
                UserSeeder.SeedUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saat inisialisasi database:\n\n{ex.Message}",
                    "Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Application.Run(new Login());
                return;
            }

            // MODE 1: LOGIN

            Application.Run(new Login());


            // MODE 2: AUTO LOGIN / DASHBOARD

            /*
            string usernameLogin = "admin";

            UserModel user = AutoLogin(usernameLogin);

            if (user != null)
            {
                OpenDashboardByRole(user);
            }
            else
            {
                Application.Run(new Login());
            }
            */
        }

        private static UserModel AutoLogin(string username)
        {
            try
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
                    AND status = 'aktif'
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

                if (dt.Rows.Count == 0)
                {
                    Console.WriteLine(
                        $"User '{username}' tidak ditemukan " +
                        "atau nonaktif.");

                    return null;
                }

                DataRow row = dt.Rows[0];

                UserModel user = new UserModel
                {
                    Id =
                        Convert.ToUInt64(row["id"]),

                    Nama =
                        row["nama"]?.ToString() ?? "",

                    Username =
                        row["username"]?.ToString() ?? "",

                    Email =
                        row["email"]?.ToString() ?? "",

                    Password =
                        row["password"]?.ToString() ?? "",

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
                            : Convert.ToDateTime(
                                row["created_at"]),

                    UpdatedAt =
                        row["updated_at"] == DBNull.Value
                            ? null
                            : Convert.ToDateTime(
                                row["updated_at"])
                };

                Console.WriteLine(
                    $"Auto login berhasil: " +
                    $"{user.Nama} ({user.Username}) - " +
                    $"Role: {user.Role}");

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error auto login: {ex.Message}");

                return null;
            }
        }


        private static void OpenDashboardByRole(UserModel user)
        {
            Form dashboardForm = null;

            switch (user.Role.ToLower())
            {
                case "admin":

                    Console.WriteLine(
                        "Membuka Admin Dashboard...");

                    dashboardForm =
                        new AdminDashboard();

                    break;

                case "petugas":

                    Console.WriteLine(
                        "Membuka Petugas Dashboard...");

                    dashboardForm =
                        new PetugasDashboard();

                    break;

                case "user":

                    MessageBox.Show(
                        "Dashboard User belum dibuat.",
                        "Informasi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    Application.Run(new Login());

                    return;

                default:

                    MessageBox.Show(
                        $"Role '{user.Role}' tidak dikenali!",
                        "Role Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    Application.Run(new Login());

                    return;
            }

            if (dashboardForm != null)
            {
                dashboardForm.Tag = user;

                Application.Run(dashboardForm);
            }
        }
    }
}