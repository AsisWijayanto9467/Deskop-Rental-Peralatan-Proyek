using App_Rental_Proyek.Config;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace App_Rental_Proyek.Helper
{
    internal static class UserSeeder
    {
        // =========================================================
        // SEED USERS
        // =========================================================

        public static void SeedUsers()
        {
            try
            {
                Console.WriteLine("=================================");
                Console.WriteLine("        USER SEEDER");
                Console.WriteLine("=================================");

                CreateDefaultUsers();

                Console.WriteLine("=================================");
                Console.WriteLine("User seeder selesai.");
                Console.WriteLine("=================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error saat seeding users: {ex.Message}");

                throw;
            }
        }

        // =========================================================
        // DEFAULT USERS
        // =========================================================

        private static void CreateDefaultUsers()
        {
            // ADMIN
            CreateUserIfNotExists(
                nama: "Administrator",
                username: "admin",
                password: "admin123",
                role: "admin",
                email: "admin@rentalalat.com",
                noTelepon: "081234567890"
            );

            // PETUGAS
            CreateUserIfNotExists(
                nama: "Petugas Rental",
                username: "petugas",
                password: "petugas123",
                role: "petugas",
                email: "petugas@rentalalat.com",
                noTelepon: "081234567891"
            );
        }

        // =========================================================
        // CREATE USER JIKA USERNAME BELUM ADA
        // =========================================================

        private static void CreateUserIfNotExists(
            string nama,
            string username,
            string password,
            string role,
            string email,
            string noTelepon)
        {
            try
            {
                // -------------------------------------------------
                // CEK USERNAME
                // -------------------------------------------------

                string checkQuery = @"
                    SELECT COUNT(*)
                    FROM users
                    WHERE username = @username";

                MySqlParameter[] checkParameters =
                {
                    new MySqlParameter(
                        "@username",
                        username)
                };

                object result =
                    DatabaseConnection.ExecuteScalar(
                        checkQuery,
                        checkParameters);

                int count = Convert.ToInt32(result);

                // -------------------------------------------------
                // JIKA USERNAME SUDAH ADA
                // -------------------------------------------------

                if (count > 0)
                {
                    Console.WriteLine(
                        $"→ Username '{username}' sudah ada. Skip.");

                    return;
                }

                // -------------------------------------------------
                // HASH PASSWORD
                // -------------------------------------------------

                string hashedPassword =
                    BCrypt.Net.BCrypt.HashPassword(password);

                // -------------------------------------------------
                // INSERT USER
                // -------------------------------------------------

                string insertQuery = @"
                    INSERT INTO users
                    (
                        nama,
                        username,
                        email,
                        password,
                        no_telepon,
                        role,
                        status
                    )
                    VALUES
                    (
                        @nama,
                        @username,
                        @email,
                        @password,
                        @noTelepon,
                        @role,
                        'aktif'
                    )";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter(
                        "@nama",
                        nama),

                    new MySqlParameter(
                        "@username",
                        username),

                    new MySqlParameter(
                        "@email",
                        email),

                    new MySqlParameter(
                        "@password",
                        hashedPassword),

                    new MySqlParameter(
                        "@noTelepon",
                        noTelepon),

                    new MySqlParameter(
                        "@role",
                        role)
                };

                int affectedRows =
                    DatabaseConnection.ExecuteQuery(
                        insertQuery,
                        parameters);

                // -------------------------------------------------
                // HASIL INSERT
                // -------------------------------------------------

                if (affectedRows > 0)
                {
                    Console.WriteLine(
                        $"✓ User '{username}' berhasil dibuat " +
                        $"({role}).");
                }
                else
                {
                    Console.WriteLine(
                        $"✗ User '{username}' gagal dibuat.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"✗ Error membuat user '{username}': " +
                    $"{ex.Message}");

                throw;
            }
        }

        // =========================================================
        // RESET USERS
        // =========================================================

        // Hanya gunakan untuk DEVELOPMENT.
        // Akan menghapus seluruh user.

        public static void ResetUsers()
        {
            try
            {
                DatabaseConnection.ExecuteQuery(
                    "DELETE FROM users");

                DatabaseConnection.ExecuteQuery(
                    "ALTER TABLE users AUTO_INCREMENT = 1");

                Console.WriteLine(
                    "Semua user berhasil dihapus.");

                SeedUsers();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error saat reset users: {ex.Message}");

                throw;
            }
        }

        // =========================================================
        // UPDATE PASSWORD
        // =========================================================

        public static bool UpdatePassword(
            string username,
            string newPassword)
        {
            try
            {
                string hashedPassword =
                    BCrypt.Net.BCrypt.HashPassword(
                        newPassword);

                string query = @"
                    UPDATE users
                    SET password = @password
                    WHERE username = @username";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter(
                        "@password",
                        hashedPassword),

                    new MySqlParameter(
                        "@username",
                        username)
                };

                return DatabaseConnection.ExecuteQuery(
                    query,
                    parameters) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error update password: {ex.Message}");

                return false;
            }
        }

        // =========================================================
        // VERIFY PASSWORD
        // =========================================================

        public static bool VerifyPassword(
            string username,
            string password)
        {
            try
            {
                string query = @"
                    SELECT password
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
                    return false;

                string hashedPassword =
                    dt.Rows[0]["password"].ToString();

                return BCrypt.Net.BCrypt.Verify(
                    password,
                    hashedPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error verify password: {ex.Message}");

                return false;
            }
        }

        // =========================================================
        // GET USER BY USERNAME
        // =========================================================

        public static DataTable GetUserByUsername(
            string username)
        {
            string query = @"
                SELECT
                    id,
                    nama,
                    username,
                    email,
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

            return DatabaseConnection.GetData(
                query,
                parameters);
        }
    }
}