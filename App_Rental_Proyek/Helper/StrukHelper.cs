using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace App_Rental_Proyek.Helper
{
    public static class StrukHelper
    {
        private const string StrukFolder = @"D:\Cross_Storage\Sistem_Proyek\struk";

        static StrukHelper()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public static void EnsureFolderExists()
        {
            try
            {
                if (!Directory.Exists(StrukFolder))
                {
                    Directory.CreateDirectory(StrukFolder);
                }
            }
            catch { }
        }

        public static string GenerateStrukPembayaran(
            string kodePembayaran,
            string kodePenyewaan,
            DateTime tanggalPembayaran,
            decimal jumlah,
            string metodePembayaran,
            string namaCustomer,
            string noTelepon,
            string status,
            string namaVerifikator = null,
            DateTime? tanggalVerifikasi = null,
            string catatan = null)
        {
            try
            {
                EnsureFolderExists();

                string fileName = $"Struk_Pembayaran_{kodePembayaran}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string fullPath = Path.Combine(StrukFolder, fileName);

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(210, PageSizes.A4.Height, Unit.Millimetre);
                        page.Margin(15);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Column(column =>
                        {
                            column.Item().AlignCenter().Text("SISTEM RENTAL ALAT PROYEK").FontSize(14).Bold();
                            column.Item().AlignCenter().Text("STRUK PEMBAYARAN").FontSize(12).Bold();
                            column.Item().PaddingTop(5).LineHorizontal(1);
                        });

                        page.Content().PaddingTop(10).Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"Kode Pembayaran: {kodePembayaran}").Bold();
                                row.RelativeItem().AlignRight().Text($"Tanggal: {tanggalPembayaran:dd/MM/yyyy}");
                            });

                            column.Item().PaddingTop(10).LineHorizontal(0.5f);

                            column.Item().PaddingTop(10).Text("INFORMASI CUSTOMER").Bold();
                            column.Item().PaddingTop(5).Text($"Nama: {namaCustomer}");
                            column.Item().Text($"No. Telepon: {noTelepon}");
                            column.Item().Text($"Kode Sewa: {kodePenyewaan}");

                            column.Item().PaddingTop(10).LineHorizontal(0.5f);

                            column.Item().PaddingTop(10).Text("DETAIL PEMBAYARAN").Bold();
                            column.Item().PaddingTop(5).Row(row =>
                            {
                                row.RelativeItem().Text("Metode Pembayaran:");
                                row.RelativeItem().AlignRight().Text(FormatMetodePembayaran(metodePembayaran));
                            });
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Jumlah Pembayaran:").Bold();
                                row.RelativeItem().AlignRight().Text($"Rp {jumlah:N0}").FontSize(12).Bold();
                            });

                            column.Item().PaddingTop(10).LineHorizontal(0.5f);

                            column.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Text("Status:");
                                row.RelativeItem().AlignRight().Text(FormatStatusPembayaran(status)).Bold();
                            });

                            if (!string.IsNullOrEmpty(namaVerifikator))
                            {
                                column.Item().PaddingTop(5).Text($"Diverifikasi oleh: {namaVerifikator}");
                                if (tanggalVerifikasi.HasValue)
                                {
                                    column.Item().Text($"Tanggal Verifikasi: {tanggalVerifikasi.Value:dd/MM/yyyy HH:mm}");
                                }
                            }

                            if (!string.IsNullOrEmpty(catatan))
                            {
                                column.Item().PaddingTop(10).LineHorizontal(0.5f);
                                column.Item().PaddingTop(5).Text("Catatan:").Bold();
                                column.Item().Text(catatan);
                            }
                        });

                        page.Footer().AlignCenter().Column(column =>
                        {
                            column.Item().PaddingTop(20).LineHorizontal(0.5f);
                            column.Item().PaddingTop(5).Text("Terima kasih atas pembayaran Anda").FontSize(9);
                            column.Item().Text($"Dicetak pada: {DateTime.Now:dd/MM/yyyy HH:mm:ss}").FontSize(8);
                        });
                    });
                });

                document.GeneratePdf(fullPath);
                return fullPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error membuat struk pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static string GenerateStrukPengembalian(
            string kodePenyewaan,
            DateTime tanggalPengembalian,
            string namaCustomer,
            string noTelepon,
            string kondisiAlat,
            int terlambatHari,
            string namaPetugas,
            string status,
            string catatan = null)
        {
            try
            {
                EnsureFolderExists();

                string fileName = $"Struk_Pengembalian_{kodePenyewaan}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string fullPath = Path.Combine(StrukFolder, fileName);

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(210, PageSizes.A4.Height, Unit.Millimetre);
                        page.Margin(15);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Column(column =>
                        {
                            column.Item().AlignCenter().Text("SISTEM RENTAL ALAT PROYEK").FontSize(14).Bold();
                            column.Item().AlignCenter().Text("STRUK PENGEMBALIAN").FontSize(12).Bold();
                            column.Item().PaddingTop(5).LineHorizontal(1);
                        });

                        page.Content().PaddingTop(10).Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"Kode Sewa: {kodePenyewaan}").Bold();
                                row.RelativeItem().AlignRight().Text($"Tanggal: {tanggalPengembalian:dd/MM/yyyy}");
                            });

                            column.Item().PaddingTop(10).LineHorizontal(0.5f);

                            column.Item().PaddingTop(10).Text("INFORMASI CUSTOMER").Bold();
                            column.Item().PaddingTop(5).Text($"Nama: {namaCustomer}");
                            column.Item().Text($"No. Telepon: {noTelepon}");

                            column.Item().PaddingTop(10).LineHorizontal(0.5f);

                            column.Item().PaddingTop(10).Text("DETAIL PENGEMBALIAN").Bold();
                            column.Item().PaddingTop(5).Text($"Diterima oleh: {namaPetugas}");
                            column.Item().Text($"Kondisi Alat: {kondisiAlat}");
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Keterlambatan:");
                                row.RelativeItem().AlignRight().Text(terlambatHari > 0 ? $"{terlambatHari} hari" : "Tidak terlambat")
                                    .Bold()
                                    .FontColor(terlambatHari > 0 ? "#E74C3C" : "#27AE60");
                            });

                            column.Item().PaddingTop(10).LineHorizontal(0.5f);

                            column.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Text("Status:");
                                row.RelativeItem().AlignRight().Text(FormatStatusPengembalian(status)).Bold();
                            });

                            if (!string.IsNullOrEmpty(catatan))
                            {
                                column.Item().PaddingTop(10).LineHorizontal(0.5f);
                                column.Item().PaddingTop(5).Text("Catatan:").Bold();
                                column.Item().Text(catatan);
                            }
                        });

                        page.Footer().AlignCenter().Column(column =>
                        {
                            column.Item().PaddingTop(20).LineHorizontal(0.5f);
                            column.Item().PaddingTop(5).Text("Terima kasih telah menggunakan layanan kami").FontSize(9);
                            column.Item().Text($"Dicetak pada: {DateTime.Now:dd/MM/yyyy HH:mm:ss}").FontSize(8);
                        });
                    });
                });

                document.GeneratePdf(fullPath);
                return fullPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error membuat struk pengembalian: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static void OpenAndPrintStruk(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    MessageBox.Show("File struk tidak ditemukan!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true,
                    Verb = "print"
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error membuka struk: {ex.Message}\n\nFile tersimpan di: {filePath}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void DownloadStruk(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    MessageBox.Show("File struk tidak ditemukan!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                    saveFileDialog.FileName = Path.GetFileName(filePath);
                    saveFileDialog.DefaultExt = "pdf";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(filePath, saveFileDialog.FileName, true);
                        MessageBox.Show($"Struk berhasil disimpan ke:\n{saveFileDialog.FileName}", "Sukses",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        var result = MessageBox.Show("Apakah ingin membuka file?", "Konfirmasi",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = saveFileDialog.FileName,
                                UseShellExecute = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error menyimpan struk: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string FormatMetodePembayaran(string metode)
        {
            return metode switch
            {
                "cash" => "Cash",
                "transfer" => "Transfer Bank",
                "qris" => "QRIS",
                _ => metode
            };
        }

        private static string FormatStatusPembayaran(string status)
        {
            return status switch
            {
                "pending" => "Pending",
                "diverifikasi" => "Diverifikasi",
                "ditolak" => "Ditolak",
                _ => status
            };
        }

        private static string FormatStatusPengembalian(string status)
        {
            return status switch
            {
                "menunggu_inspeksi" => "Menunggu Inspeksi",
                "diterima" => "Diterima",
                "perlu_perbaikan" => "Perlu Perbaikan",
                "ditolak" => "Ditolak",
                _ => status
            };
        }
    }
}
