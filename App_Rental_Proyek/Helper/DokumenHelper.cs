using System;
using System.IO;
using System.Windows.Forms;

namespace App_Rental_Proyek.Helper
{
    public static class DokumenHelper
    {
        public const string DokumenFolder = @"D:\Cross_Storage\Sistem_Proyek\dokumen";

        public static string GetFullPath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "";
            string name = Path.GetFileName(fileName.Trim());
            return Path.Combine(DokumenFolder, name);
        }

        public static bool IsImage(string path)
        {
            string ext = Path.GetExtension(path)?.ToLowerInvariant();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif" || ext == ".webp";
        }

        public static void EnsureFolderExists()
        {
            try
            {
                if (!Directory.Exists(DokumenFolder))
                {
                    Directory.CreateDirectory(DokumenFolder);
                }
            }
            catch { }
        }

        public static string SaveDokumenFile(string sourcePath, string baseName)
        {
            try
            {
                EnsureFolderExists();
                if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                {
                    return null;
                }

                string ext = Path.GetExtension(sourcePath);
                string cleanBase = string.Join("_", baseName.Split(Path.GetInvalidFileNameChars()));
                string fileName = $"{cleanBase}{DateTime.Now:yyyyMMddHHmmss}{ext}";

                string destination = Path.Combine(DokumenFolder, fileName);
                File.Copy(sourcePath, destination, true);

                return fileName;
            }
            catch
            {
                return null;
            }
        }

        public static void DeleteDokumenFile(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName)) return;
                string fullPath = Path.Combine(DokumenFolder, Path.GetFileName(fileName.Trim()));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch { }
        }

        public static Image LoadImage(string fileName)
        {
            try
            {
                string fullPath = GetFullPath(fileName);
                if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
                {
                    return null;
                }

                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    return Image.FromStream(stream);
                }
            }
            catch
            {
                return null;
            }
        }

        public static bool FileExists(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            string fullPath = GetFullPath(fileName);
            return File.Exists(fullPath);
        }
    }
}