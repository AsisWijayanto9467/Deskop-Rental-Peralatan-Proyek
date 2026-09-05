using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Laporan
{
    public partial class DownloadLaporanForm : Form
    {
        public DownloadLaporanForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            cbJenisLaporan.Items.Clear();
            cbJenisLaporan.Items.Add("Laporan Penyewaan");
            cbJenisLaporan.Items.Add("Laporan Pembayaran");
            cbJenisLaporan.Items.Add("Laporan Pengembalian");
            cbJenisLaporan.Items.Add("Laporan Denda");
            cbJenisLaporan.SelectedIndex = 0;

            cbFormat.Items.Clear();
            cbFormat.Items.Add("Excel (.xlsx)");
            cbFormat.Items.Add("PDF (.pdf)");
            cbFormat.Items.Add("CSV (.csv)");
            cbFormat.SelectedIndex = 0;
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dari = dtpDari.Value.Date;
                DateTime sampai = dtpSampai.Value.Date;

                if (sampai < dari)
                {
                    MessageBox.Show("Tanggal 'sampai' tidak boleh sebelum tanggal 'dari'.",
                        "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int index = cbJenisLaporan.SelectedIndex;
                if (index < 0) index = 0;

                string jenis = cbJenisLaporan.Text;
                string dataQuery;
                string[] headers;
                string defaultFileName;
                string dateColumn;

                switch (index)
                {
                    case 0:
                        dateColumn = "p.tanggal_pengajuan";
                        defaultFileName = "Laporan_Penyewaan";
                        headers = new[] { "Kode Penyewaan", "User", "Tanggal", "Alat", "Total", "Status" };
                        dataQuery = @"
                            SELECT p.kode_penyewaan, u.nama AS user, p.tanggal_pengajuan AS tanggal,
                                   COALESCE(GROUP_CONCAT(CONCAT(ap.nama_alat, ' x', dp.jumlah) SEPARATOR ', '), '-') AS alat,
                                   p.total, p.status
                            FROM penyewaans p
                            LEFT JOIN users u ON u.id = p.user_id
                            LEFT JOIN detail_penyewaans dp ON dp.penyewaan_id = p.id
                            LEFT JOIN alat_proyeks ap ON ap.id = dp.alat_id
                            WHERE DATE(" + dateColumn + @") BETWEEN @dari AND @sampai
                            GROUP BY p.id, p.kode_penyewaan, u.nama, p.tanggal_pengajuan, p.total, p.status
                            ORDER BY p.tanggal_pengajuan DESC";
                        break;
                    case 1:
                        dateColumn = "pm.tanggal_pembayaran";
                        defaultFileName = "Laporan_Pembayaran";
                        headers = new[] { "Kode Pembayaran", "User", "Penyewaan", "Jumlah", "Metode", "Status" };
                        dataQuery = @"
                            SELECT pm.kode_pembayaran, u.nama AS user, p.kode_penyewaan,
                                   pm.jumlah, pm.metode_pembayaran, pm.status, pm.tanggal_pembayaran AS tanggal
                            FROM pembayarans pm
                            LEFT JOIN penyewaans p ON p.id = pm.penyewaan_id
                            LEFT JOIN users u ON u.id = p.user_id
                            WHERE DATE(" + dateColumn + @") BETWEEN @dari AND @sampai
                            ORDER BY pm.tanggal_pembayaran DESC";
                        break;
                    case 2:
                        dateColumn = "pg.tanggal_pengembalian";
                        defaultFileName = "Laporan_Pengembalian";
                        headers = new[] { "Penyewaan", "User", "Tanggal Kembali", "Kondisi", "Status", "Keterlambatan" };
                        dataQuery = @"
                            SELECT p.kode_penyewaan, u.nama AS user, pg.tanggal_pengembalian AS tanggal,
                                   pg.kondisi_alat, pg.status, pg.terlambat_hari
                            FROM pengembalians pg
                            LEFT JOIN penyewaans p ON p.id = pg.penyewaan_id
                            LEFT JOIN users u ON u.id = p.user_id
                            WHERE DATE(" + dateColumn + @") BETWEEN @dari AND @sampai
                            ORDER BY pg.tanggal_pengembalian DESC";
                        break;
                    default:
                        dateColumn = "d.created_at";
                        defaultFileName = "Laporan_Denda";
                        headers = new[] { "Penyewaan", "Jenis Denda", "Alasan", "Jumlah", "Status" };
                        dataQuery = @"
                            SELECT p.kode_penyewaan, d.jenis_denda, d.alasan, d.jumlah, d.status,
                                   d.created_at AS tanggal
                            FROM dendas d
                            LEFT JOIN penyewaans p ON p.id = d.penyewaan_id
                            WHERE DATE(" + dateColumn + @") BETWEEN @dari AND @sampai
                            ORDER BY d.created_at DESC";
                        break;
                }

                DataTable dt = DatabaseConnection.GetData(dataQuery,
                    new MySql.Data.MySqlClient.MySqlParameter("@dari", dari.ToString("yyyy-MM-dd")),
                    new MySql.Data.MySqlClient.MySqlParameter("@sampai", sampai.ToString("yyyy-MM-dd")));

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Tidak ada data pada rentang tanggal tersebut.",
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Title = "Simpan Laporan";

                    int formatIndex = cbFormat.SelectedIndex;
                    if (formatIndex < 0) formatIndex = 0;

                    string ext;
                    switch (formatIndex)
                    {
                        case 1:
                            sfd.Filter = "PDF File (*.pdf)|*.pdf";
                            ext = ".pdf";
                            break;
                        case 2:
                            sfd.Filter = "CSV File (*.csv)|*.csv";
                            ext = ".csv";
                            break;
                        default:
                            sfd.Filter = "Excel File (*.xlsx)|*.xlsx";
                            ext = ".xlsx";
                            break;
                    }

                    sfd.FileName = defaultFileName + "_" + dari.ToString("yyyyMMdd") + "-" + sampai.ToString("yyyyMMdd") + ext;
                    sfd.DefaultExt = ext.TrimStart('.');

                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        switch (formatIndex)
                        {
                            case 1:
                                ExportToPdf(dt, headers, jenis, dari, sampai, sfd.FileName);
                                break;
                            case 2:
                                ExportToCsv(dt, headers, sfd.FileName);
                                break;
                            default:
                                ExportToExcel(dt, headers, jenis, dari, sampai, sfd.FileName);
                                break;
                        }

                        ActivityLogHelper.LogForSession(
                            SessionManager.GetCurrentUserId(),
                            $"Mengunduh {jenis} periode {dari:dd/MM/yyyy} - {sampai:dd/MM/yyyy} ({dt.Rows.Count} data)",
                            "Laporan");

                        MessageBox.Show("Laporan berhasil diunduh!", "Sukses",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengunduh laporan: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCsv(DataTable dt, string[] headers, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Join(",", headers));

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = new string[headers.Length];
                for (int i = 0; i < headers.Length; i++)
                {
                    fields[i] = CleanField(row[i]);
                }
                sb.AppendLine(string.Join(",", fields));
            }

            System.IO.File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
        }

        private void ExportToExcel(DataTable dt, string[] headers, string jenis, DateTime dari, DateTime sampai, string filePath)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Laporan");

            string judul = jenis;
            if (judul.StartsWith("Laporan ", StringComparison.OrdinalIgnoreCase))
            {
                judul = judul.Substring("Laporan ".Length);
            }

            ws.Cell(1, 1).Value = judul;
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#173B63");
            ws.Range(1, 1, 1, headers.Length).Merge();

            ws.Cell(2, 1).Value = $"Periode: {dari:dd/MM/yyyy} - {sampai:dd/MM/yyyy}";
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;
            ws.Range(2, 1, 2, headers.Length).Merge();

            int headerRow = 4;
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#173B63");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.DarkGray;
            }

            int row = headerRow + 1;
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = ws.Cell(row, i + 1);
                    object value = dr[i];

                    if (value == null || value == DBNull.Value)
                    {
                        cell.Value = "";
                    }
                    else if (value is DateTime d)
                    {
                        cell.Value = d;
                        cell.Style.DateFormat.Format = "dd/MM/yyyy";
                    }
                    else if (value is decimal || value is double || value is float || value is int || value is long)
                    {
                        cell.Value = Convert.ToDecimal(value);
                        if (headers[i].Contains("Total") || headers[i].Contains("Jumlah") || headers[i].Contains("Denda"))
                        {
                            cell.Style.NumberFormat.Format = "#,##0.00";
                        }
                    }
                    else
                    {
                        cell.Value = value.ToString();
                    }

                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.LightGray;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }

                if (row % 2 == 0)
                {
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F6FA");
                }

                row++;
            }

            ws.SheetView.FreezeRows(headerRow);
            ws.Columns(1, headers.Length).AdjustToContents();
            for (int i = 1; i <= headers.Length; i++)
            {
                if (ws.Column(i).Width < 12) ws.Column(i).Width = 12;
            }

            workbook.SaveAs(filePath);
        }

        private void ExportToPdf(DataTable dt, string[] headers, string jenis, DateTime dari, DateTime sampai, string filePath)
        {
            string judul = jenis;
            if (judul.StartsWith("Laporan ", StringComparison.OrdinalIgnoreCase))
            {
                judul = judul.Substring("Laporan ".Length);
            }

            QuestPDF.Settings.License = LicenseType.Community;

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);

                    page.Header().Column(column =>
                    {
                        column.Item().AlignCenter().Text(judul)
                            .FontSize(18).Bold().FontColor("#173B63");

                        column.Item().AlignCenter().PaddingBottom(4).Text(judul.ToUpper())
                            .FontSize(10).FontColor("#173B63");

                        column.Item().AlignCenter().Text($"Periode: {dari:dd/MM/yyyy} - {sampai:dd/MM/yyyy}")
                            .FontSize(11).FontColor(Colors.Grey.Darken1);

                        column.Item().PaddingVertical(8).LineHorizontal(1).LineColor("#173B63");
                    });

                    page.Content().PaddingTop(8).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            for (int i = 0; i < headers.Length; i++)
                            {
                                columns.ConstantColumn(i == 0 ? 90 :
                                    (headers[i].Contains("Total") || headers[i].Contains("Jumlah") ? 70 : 85));
                            }
                        });

                        table.Header(header =>
                        {
                            for (int i = 0; i < headers.Length; i++)
                            {
                                header.Cell().Background("#173B63").Padding(5)
                                    .Text(headers[i]).Bold().FontColor(Colors.White).FontSize(9);
                            }
                        });

                        int dataRow = 1;
                        foreach (DataRow dr in dt.Rows)
                        {
                            for (int i = 0; i < headers.Length; i++)
                            {
                                object value = dr[i];

                                string text;
                                if (value == null || value == DBNull.Value)
                                {
                                    text = "-";
                                }
                                else if (value is DateTime d)
                                {
                                    text = d.ToString("dd/MM/yyyy");
                                }
                                else if (value is decimal || value is double || value is float)
                                {
                                    text = Convert.ToDecimal(value).ToString("N2");
                                }
                                else
                                {
                                    text = value.ToString();
                                }

                                string bg = dataRow % 2 == 0 ? "#FFFFFF" : "#F2F6FA";
                                table.Cell().Background(bg).Padding(4)
                                    .Text(text).FontSize(8.5f).FontColor(Colors.Black);
                            }
                            dataRow++;
                        }
                    });

                    page.Footer().AlignRight().Text(text =>
                    {
                        text.DefaultTextStyle(style => style.FontSize(9).FontColor(Colors.Grey.Darken2));
                        text.Span($"Total Data: {dt.Rows.Count} data    |    Dicetak: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
                });
            });

            document.GeneratePdf(filePath);
        }

        private string CleanField(object value)
        {
            if (value == null || value == DBNull.Value) return "";

            string s;
            if (value is DateTime dt)
            {
                s = dt.ToString("dd/MM/yyyy");
            }
            else if (value is decimal || value is double || value is float)
            {
                s = Convert.ToDecimal(value).ToString("N2");
            }
            else
            {
                s = value.ToString();
            }

            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n") || s.Contains("\r"))
            {
                s = "\"" + s.Replace("\"", "\"\"") + "\"";
            }

            return s;
        }
    }
}
