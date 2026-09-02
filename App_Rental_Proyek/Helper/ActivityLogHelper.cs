using App_Rental_Proyek.Config;
using MySql.Data.MySqlClient;
using System;
using System.Net;

namespace App_Rental_Proyek.Helper
{
    public static class ActivityLogHelper
    {
        // =========================================================
        // INSERT LOG KE TABEL activity_logs
        // =========================================================

        public static void Log(
            string aktivitas,
            string modul,
            ulong? referensiId = null)
        {
            try
            {
                ulong userId = Session.CurrentUser != null
                    ? Session.CurrentUser.Id
                    : 0;

                if (userId == 0)
                {
                    return;
                }

                string query = @"
                    INSERT INTO activity_logs
                    (
                        user_id,
                        aktivitas,
                        modul,
                        referensi_id,
                        ip_address,
                        created_at
                    )
                    VALUES
                    (
                        @user_id,
                        @aktivitas,
                        @modul,
                        @referensi_id,
                        @ip_address,
                        NOW()
                    )";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@user_id", userId),
                    new MySqlParameter("@aktivitas", aktivitas),
                    new MySqlParameter("@modul", modul),
                    new MySqlParameter("@referensi_id", (object)referensiId ?? DBNull.Value),
                    new MySqlParameter("@ip_address", GetIpAddress())
                };

                DatabaseConnection.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error mencatat activity log: {ex.Message}");
            }
        }

        // =========================================================
        // LOG DENGAN USER DARI SessionManager
        // Dipakai saat session hanya tersedia di SessionManager
        // (misal auto-login di Program.cs tidak mengisi Helper.Session).
        // =========================================================

        public static void LogForSession(
            ulong userId,
            string aktivitas,
            string modul,
            ulong? referensiId = null)
        {
            if (userId == 0)
            {
                userId = Session.CurrentUser != null
                    ? Session.CurrentUser.Id
                    : 0;
            }

            if (userId == 0)
            {
                return;
            }

            try
            {
                string query = @"
                    INSERT INTO activity_logs
                    (
                        user_id,
                        aktivitas,
                        modul,
                        referensi_id,
                        ip_address,
                        created_at
                    )
                    VALUES
                    (
                        @user_id,
                        @aktivitas,
                        @modul,
                        @referensi_id,
                        @ip_address,
                        NOW()
                    )";

                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@user_id", userId),
                    new MySqlParameter("@aktivitas", aktivitas),
                    new MySqlParameter("@modul", modul),
                    new MySqlParameter("@referensi_id", (object)referensiId ?? DBNull.Value),
                    new MySqlParameter("@ip_address", GetIpAddress())
                };

                DatabaseConnection.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error mencatat activity log: {ex.Message}");
            }
        }

        // =========================================================
        // AMBIL IP ADDRESS
        // =========================================================

        private static string GetIpAddress()
        {
            try
            {
                string host = Dns.GetHostName();
                IPAddress[] addresses = Dns.GetHostAddresses(host);

                foreach (IPAddress ip in addresses)
                {
                    if (ip.AddressFamily ==
                        System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }

                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }
}
