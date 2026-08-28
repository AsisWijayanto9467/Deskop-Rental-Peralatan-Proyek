using System;
using System.IO;
using System.Windows.Forms;

namespace App_Rental_Proyek.Helper
{
    public static class BuktiPembayaranHelper
    {
        public static string ResolvePath(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";

            string path = raw.Trim();

            if (Path.IsPathRooted(path)) return path;

            string candidate = Path.Combine(Application.StartupPath, path);
            if (File.Exists(candidate)) return candidate;

            candidate = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            if (File.Exists(candidate)) return candidate;

            return path;
        }

        public static bool IsImage(string path)
        {
            string ext = Path.GetExtension(path)?.ToLowerInvariant();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif";
        }
    }
}