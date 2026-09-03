using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using App_Rental_Proyek.Model;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Pengembalian
{
    public class PengembalianViewItem
    {
        public ulong Id { get; set; }
        public ulong PenyewaanId { get; set; }
        public string KodeSewa { get; set; } = "";
        public string NamaCustomer { get; set; } = "";
        public DateTime TanggalPengembalian { get; set; }
        public DateTime TanggalSelesai { get; set; }
        public int TerlambatHari { get; set; } = 0;
        public string KondisiAlat { get; set; } = "";
        public string? Foto { get; set; }
        public string Catatan { get; set; } = "";
        public string Status { get; set; } = "menunggu_inspeksi";
        public string NamaInspektur { get; set; } = "";
        public decimal TotalSewa { get; set; } = 0;
        public DateTime? CreatedAt { get; set; }
    }

    public partial class PengembalianPage : System.Windows.Forms.UserControl
    {
        private List<PengembalianViewItem> _allData = new List<PengembalianViewItem>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 1;
        private string _currentSearch = "";
        private string _currentStatus = ""; // "" = Semua

        private readonly string[] _statusFilterLabels =
        {
            "Semua", "Menunggu Inspeksi", "Diterima", "Perlu Perbaikan", "Ditolak"
        };

        private readonly string[] _statusFilterKeys =
        {
            "", "menunggu_inspeksi", "diterima", "perlu_perbaikan", "ditolak"
        };

        private class ActionButtonSpec
        {
            public string Text;
            public Color Color;
            public Rectangle Bounds;
        }

        public PengembalianPage()
        {
            InitializeComponent();
            InitializeGridView();
            InitializeFilters();
            LoadData();
        }

        private void PengembalianPage_Load(object sender, EventArgs e)
        {
        }

        // ============================================
        // INISIALISASI
        // ============================================
        private void InitializeGridView()
        {
            guna2DataGridView1.Columns.Clear();

            guna2DataGridView1.Columns.Add("Id", "ID");
            guna2DataGridView1.Columns.Add("Kode", "Kode Sewa");
            guna2DataGridView1.Columns.Add("Customer", "Customer");
            guna2DataGridView1.Columns.Add("Tanggal", "Tgl Pengembalian");
            guna2DataGridView1.Columns.Add("Terlambat", "Terlambat");
            guna2DataGridView1.Columns.Add("Kondisi", "Kondisi Alat");
            guna2DataGridView1.Columns.Add("Catatan", "Catatan User");
            guna2DataGridView1.Columns.Add("Status", "Status");

            DataGridViewColumn colAction = new DataGridViewColumn();
            colAction.Name = "Action";
            colAction.HeaderText = "Aksi";
            colAction.CellTemplate = new DataGridViewTextBoxCell();
            colAction.Width = 300;
            colAction.MinimumWidth = 300;
            guna2DataGridView1.Columns.Add(colAction);

            guna2DataGridView1.Columns["Id"].Visible = false;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;

            guna2DataGridView1.Columns["Kode"].MinimumWidth = 130;
            guna2DataGridView1.Columns["Customer"].MinimumWidth = 160;
            guna2DataGridView1.Columns["Kondisi"].MinimumWidth = 160;
            guna2DataGridView1.Columns["Catatan"].MinimumWidth = 160;
            guna2DataGridView1.Columns["Status"].MinimumWidth = 130;
            guna2DataGridView1.Columns["Action"].Width = 300;

            guna2DataGridView1.CellPainting += Guna2DataGridView1_CellPainting;
            guna2DataGridView1.CellClick += Guna2DataGridView1_CellClick;
        }

        private void InitializeFilters()
        {
            cbStatus.Items.Clear();
            foreach (string label in _statusFilterLabels)
            {
                cbStatus.Items.Add(label);
            }
            cbStatus.SelectedIndex = 0;
        }

        // ============================================
        // DATABASE OPERATIONS
        // ============================================
        private List<PengembalianViewItem> GetAllFromDatabase()
        {
            var list = new List<PengembalianViewItem>();
            try
            {
                string query = @"
                    SELECT pg.id, pg.penyewaan_id, pg.tanggal_pengembalian,
                           pg.diterima_oleh, pg.kondisi_alat, pg.foto,
                           pg.terlambat_hari, pg.catatan, pg.status, pg.created_at,
                           p.kode_penyewaan, p.tanggal_selesai, p.total AS total_sewa,
                           u.nama AS nama_customer,
                           pu.nama AS nama_inspektur
                    FROM pengembalians pg
                    LEFT JOIN penyewaans p ON p.id = pg.penyewaan_id
                    LEFT JOIN users u ON u.id = p.user_id
                    LEFT JOIN users pu ON pu.id = pg.diterima_oleh
                    ORDER BY pg.created_at DESC";

                DataTable dt = DatabaseConnection.GetData(query);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new PengembalianViewItem
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        PenyewaanId = row["penyewaan_id"] != DBNull.Value ? Convert.ToUInt64(row["penyewaan_id"]) : 0,
                        KodeSewa = row["kode_penyewaan"]?.ToString() ?? "-",
                        NamaCustomer = row["nama_customer"]?.ToString() ?? "-",
                        TanggalPengembalian = row["tanggal_pengembalian"] != DBNull.Value ? Convert.ToDateTime(row["tanggal_pengembalian"]) : DateTime.MinValue,
                        TanggalSelesai = row["tanggal_selesai"] != DBNull.Value ? Convert.ToDateTime(row["tanggal_selesai"]) : DateTime.MinValue,
                        TerlambatHari = row["terlambat_hari"] != DBNull.Value ? Convert.ToInt32(row["terlambat_hari"]) : 0,
                        KondisiAlat = row["kondisi_alat"]?.ToString() ?? "",
                        Foto = row["foto"]?.ToString(),
                        Catatan = row["catatan"]?.ToString() ?? "",
                        Status = row["status"]?.ToString() ?? "menunggu_inspeksi",
                        NamaInspektur = row["nama_inspektur"]?.ToString() ?? "-",
                        TotalSewa = row["total_sewa"] != DBNull.Value ? Convert.ToDecimal(row["total_sewa"]) : 0,
                        CreatedAt = row["created_at"] as DateTime?
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengambil data pengembalian: {ex.Message}\n\nPastikan tabel 'pengembalians' sudah dibuat di database.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }

        // ============================================
        // ACTIVITY LOG
        // ============================================
        private void LogActivity(string aktivitas, string modul, ulong? referensiId = null)
        {
            ActivityLogHelper.LogForSession(SessionManager.GetCurrentUserId(), aktivitas, modul, referensiId);
        }

        // ============================================
        // LOAD DATA
        // ============================================
        private void LoadData()
        {
            try
            {
                _allData = GetAllFromDatabase();
                if (_allData == null) _allData = new List<PengembalianViewItem>();
                ApplyFilters();
                UpdateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data pengembalian: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _allData = new List<PengembalianViewItem>();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (_allData == null) _allData = new List<PengembalianViewItem>();

            var filtered = new List<PengembalianViewItem>(_allData);

            if (!string.IsNullOrEmpty(_currentSearch))
            {
                string s = _currentSearch.ToLower();
                filtered = filtered.FindAll(p =>
                    (p.KodeSewa?.ToLower().Contains(s) ?? false) ||
                    (p.NamaCustomer?.ToLower().Contains(s) ?? false));
            }

            if (!string.IsNullOrEmpty(_currentStatus))
            {
                filtered = filtered.FindAll(p => p.Status == _currentStatus);
            }

            _totalPages = (int)Math.Ceiling((double)filtered.Count / PageSize);
            if (_totalPages == 0) _totalPages = 1;

            if (_currentPage > _totalPages) _currentPage = _totalPages;

            var pageData = filtered
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            DisplayData(pageData);
            UpdatePaginationInfo(filtered.Count);
        }

        private void UpdateStats()
        {
            try
            {
                int total = _allData.Count;
                int menunggu = _allData.Count(p => p.Status == "menunggu_inspeksi");
                int diterima = _allData.Count(p => p.Status == "diterima");
                int ditolak = _allData.Count(p => p.Status == "ditolak");

                lblStat1Value.Text = total.ToString();
                lblStat2Value.Text = menunggu.ToString();
                lblStat3Value.Text = diterima.ToString();
                lblStat4Value.Text = ditolak.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error update statistik: {ex.Message}");
            }
        }

        private void DisplayData(List<PengembalianViewItem> list)
        {
            guna2DataGridView1.Rows.Clear();

            if (list == null || list.Count == 0)
            {
                UpdatePaginationInfo(0);
                return;
            }

            foreach (var p in list)
            {
                string kondisiDisplay = string.IsNullOrWhiteSpace(p.KondisiAlat) ? "-" : p.KondisiAlat;
                if (kondisiDisplay.Length > 30)
                    kondisiDisplay = kondisiDisplay.Substring(0, 27) + "...";

                string catatanDisplay = string.IsNullOrWhiteSpace(p.Catatan) ? "-" : p.Catatan;
                if (catatanDisplay.Length > 30)
                    catatanDisplay = catatanDisplay.Substring(0, 27) + "...";

                int rowIndex = guna2DataGridView1.Rows.Add(
                    p.Id,
                    p.KodeSewa,
                    p.NamaCustomer,
                    p.TanggalPengembalian != DateTime.MinValue ? p.TanggalPengembalian.ToString("dd/MM/yyyy") : "-",
                    p.TerlambatHari > 0 ? p.TerlambatHari.ToString() + " hr" : "Tepat Waktu",
                    kondisiDisplay,
                    catatanDisplay,
                    FormatStatusLabel(p.Status),
                    ""
                );

                ApplyRowColor(p, rowIndex);
            }
        }

        private void ApplyRowColor(PengembalianViewItem p, int rowIndex)
        {
            switch (p.Status)
            {
                case "menunggu_inspeksi":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                    break;
                case "diterima":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                    break;
                case "perlu_perbaikan":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                    break;
                case "ditolak":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                    break;
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "menunggu_inspeksi": return "Menunggu Inspeksi";
                case "diterima": return "Diterima";
                case "perlu_perbaikan": return "Perlu Perbaikan";
                case "ditolak": return "Ditolak";
                default: return status;
            }
        }

        private string MapDisplayToRawStatus(string display)
        {
            foreach (string key in _statusFilterKeys)
            {
                if (key == "") continue;
                if (FormatStatusLabel(key) == display) return key;
            }
            return "menunggu_inspeksi";
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            lbTotal.Text = $"Total: {totalFiltered} pengembalian";
            lbHalaman.Text = $"Halaman {_currentPage} dari {_totalPages}";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        // ============================================
        // ACTION BUTTONS
        // ============================================
        private List<ActionButtonSpec> GetActionButtons(string status, int cellWidth, int cellX, int cellY)
        {
            var buttons = new List<ActionButtonSpec>();
            int buttonHeight = 30;
            int gap = 3;

            ActionButtonSpec MakeBtn(string text, Color color, Rectangle rect)
            {
                return new ActionButtonSpec { Text = text, Color = color, Bounds = rect };
            }

            if (status == "menunggu_inspeksi")
            {
                int part = (cellWidth - (gap * 5)) / 4;
                int x = cellX + 2;
                buttons.Add(MakeBtn("Detail", Color.FromArgb(52, 152, 219),
                    new Rectangle(x, cellY + 3, part, buttonHeight)));
                x += part + gap;
                buttons.Add(MakeBtn("Foto", Color.FromArgb(155, 89, 182),
                    new Rectangle(x, cellY + 3, part, buttonHeight)));
                x += part + gap;
                buttons.Add(MakeBtn("Inspeksi", Color.FromArgb(46, 204, 113),
                    new Rectangle(x, cellY + 3, part + gap, buttonHeight)));
                return buttons;
            }

            int half = (cellWidth - (gap * 3)) / 2;
            buttons.Add(MakeBtn("Detail", Color.FromArgb(52, 152, 219),
                new Rectangle(cellX + 2, cellY + 3, half, buttonHeight)));
            buttons.Add(MakeBtn("Foto", Color.FromArgb(155, 89, 182),
                new Rectangle(cellX + gap + 2 + half, cellY + 3, half, buttonHeight)));
            return buttons;
        }

        private void Guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                e.PaintBackground(e.CellBounds, true);

                string status = guna2DataGridView1.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "Menunggu Inspeksi";
                string rawStatus = MapDisplayToRawStatus(status);

                var buttons = GetActionButtons(rawStatus, e.CellBounds.Width, e.CellBounds.X, e.CellBounds.Y);

                foreach (var btn in buttons)
                {
                    DrawCellButton(e, btn.Bounds, btn.Text, btn.Color);
                }

                e.Handled = true;
            }
        }

        private void DrawCellButton(DataGridViewCellPaintingEventArgs e, Rectangle rect, string text, Color color)
        {
            using (Brush brush = new SolidBrush(color))
            using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 1))
            using (Font font = new Font("Segoe UI", 8, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                e.Graphics.FillRectangle(brush, rect);
                e.Graphics.DrawRectangle(pen, rect);

                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.DrawString(text, font, textBrush, rect, sf);
            }
        }

        private void Guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                ulong id = Convert.ToUInt64(guna2DataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                string displayStatus = guna2DataGridView1.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "Menunggu Inspeksi";
                string rawStatus = MapDisplayToRawStatus(displayStatus);

                Rectangle cellRect = guna2DataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                Point clickPoint = guna2DataGridView1.PointToClient(Control.MousePosition);
                int clickX = clickPoint.X - cellRect.X;

                var buttons = GetActionButtons(rawStatus, cellRect.Width, 0, 0);

                foreach (var btn in buttons)
                {
                    if (clickX >= btn.Bounds.X && clickX <= btn.Bounds.Right)
                    {
                        if (btn.Text == "Detail")
                        {
                            ShowDetail(id);
                        }
                        else if (btn.Text == "Foto")
                        {
                            PreviewFoto(id);
                        }
                        else if (btn.Text == "Inspeksi")
                        {
                            OpenInspeksi(id);
                        }
                        break;
                    }
                }
            }
        }

        // ============================================
        // OPERASI
        // ============================================
        private void ShowDetail(ulong id)
        {
            var data = _allData.Find(p => p.Id == id);
            if (data == null) return;

            using (var form = new DetailPengembalian(id))
            {
                form.ShowDialog(this);
            }

            LogActivity($"Melihat detail pengembalian '{data.KodeSewa}' dari {data.NamaCustomer}",
                "Pengembalian", id);
        }

        private void PreviewFoto(ulong id)
        {
            var data = _allData.Find(p => p.Id == id);
            if (data == null) return;

            if (string.IsNullOrEmpty(data.Foto))
            {
                MessageBox.Show("Foto pengembalian tidak tersedia.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string fotoPath = System.IO.Path.Combine(
                    Application.StartupPath,
                    "Resources", "FotoPengembalian",
                    data.Foto);

                if (!System.IO.File.Exists(fotoPath))
                {
                    fotoPath = System.IO.Path.Combine(
                        "D:\\Cross_Storage\\Sistem_Proyek",
                        data.Foto);
                }

                if (System.IO.File.Exists(fotoPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = fotoPath,
                        UseShellExecute = true
                    });

                    LogActivity($"Melihat foto pengembalian '{data.KodeSewa}'",
                        "Pengembalian", id);
                }
                else
                {
                    MessageBox.Show($"File foto tidak ditemukan:\n{fotoPath}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error membuka foto: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenInspeksi(ulong id)
        {
            var data = _allData.Find(p => p.Id == id);
            if (data == null) return;

            using (var form = new InspeksiPengembalian(id))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        // ============================================
        // EVENT HANDLERS
        // ============================================
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _currentSearch = txtSearch.Text.Trim();
            _currentPage = 1;
            ApplyFilters();
        }

        private void cbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStatus.SelectedIndex >= 0 && cbStatus.SelectedIndex < _statusFilterKeys.Length)
            {
                _currentStatus = _statusFilterKeys[cbStatus.SelectedIndex];
                _currentPage = 1;
                ApplyFilters();
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyFilters();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                ApplyFilters();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
