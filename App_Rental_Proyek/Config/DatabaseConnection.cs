using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace App_Rental_Proyek.Config
{
    public static class DatabaseConnection
    {
        private static string server = "localhost";
        private static string port = "3306";
        private static string database = "db_rental_alat_proyek";
        private static string username = "root";
        private static string password = "";

        private static string connectionString = BuildConnectionString();

        private static string BuildConnectionString()
        {
            return $"Server={server};" +
                   $"Port={port};" +
                   $"Database={database};" +
                   $"User ID={username};" +
                   $"Password={password};" +
                   "Charset=utf8mb4;" +
                   "Convert Zero Datetime=True;" +
                   "Pooling=true;";
        }

        public static void SetConnection(
            string dbServer,
            string dbPort,
            string dbName,
            string dbUser,
            string dbPassword)
        {
            server = dbServer;
            port = dbPort;
            database = dbName;
            username = dbUser;
            password = dbPassword;

            connectionString = BuildConnectionString();
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error koneksi: {ex.Message}");
                return false;
            }
        }

        public static bool TestConnection(out string errorMessage)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    errorMessage =
                        $"Berhasil terhubung ke database '{database}'.";

                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage =
                    $"Gagal terhubung ke database '{database}': {ex.Message}";

                return false;
            }
        }

        public static int ExecuteQuery(
            string query,
            params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable GetData(
            string query,
            params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (MySqlDataAdapter adapter =
                           new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();

                        adapter.Fill(dt);

                        return dt;
                    }
                }
            }
        }

        public static object ExecuteScalar(
            string query,
            params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}