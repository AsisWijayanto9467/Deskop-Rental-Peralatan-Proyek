using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using App_Rental_Proyek.UserControls.Admin.Pengembalian;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.Pengembalian
{
    public class PengembalianViewItem
    {
        public ulong Id { get; set; }
        public ulong PenyewaanId { get; set; }
        public string KodeSewa { get; set; } = "";
        public string NamaCustomer { get; set; } = "";
        public DateTime TanggalPengembalian { get; set; }
        public int TerlambatHari { get; set; } = 0;
        public string KondisiAlat { get; set; } = "";
        public string DiterimaOlehNama { get; set; } = "";
        public string Status { get; set; } = "diterima";
        public string Catatan { get; set; } = "";
        public DateTime? CreatedAt { get; set; }
    }

    public partial class PengembalianList : System.Windows.Forms.UserControl
    {
        private List<PengembalianViewItem> _allData = new List<PengembalianViewItem>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 1;
        private string _currentSearch = "";
        private string _currentStatus = ""; // "" = Semua

        private readonly string[] _statusFilterLabels =
        {
            "Semua", "Diterima", "Perlu Perbaikan", "Ditolak"
        };

        private readonly string[] _statusFilterKeys =
        {
            "", "diterima", "perlu_perbaikan", "ditolak"
        };

        public PengembalianList()
        {
            InitializeComponent();
            InitializeGridView();
            InitializeFilters();
            LoadData();
        }

        private void PengembalianList_Load(object sender, EventArgs e)
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
            guna2DataGridView1.Columns.Add("Diterima", "Diterima Oleh");
            guna2DataGridView1.Columns.Add("Status", "Status");

            DataGridViewColumn colAction = new DataGridViewColumn();
            colAction.Name = "Action";
            colAction.HeaderText = "Aksi";
            colAction.CellTemplate = new DataGridViewTextBoxCell();
            colAction.Width = 120;
            colAction.MinimumWidth = 120;
            guna2DataGridView1.Columns.Add(colAction);

            guna2DataGridView1.Columns["Id"].Visible = false;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;

            guna2DataGridView1.Columns["Kode"].MinimumWidth = 130;
            guna2DataGridView1.Columns["Customer"].MinimumWidth = 170;
            guna2DataGridView1.Columns["Kondisi"].MinimumWidth = 180;
            guna2DataGridView1.Columns["Diterima"].MinimumWidth = 130;
            guna2DataGridView1.Columns["Status"].MinimumWidth = 120;
            guna2DataGridView1.Columns["Tanggal"].MinimumWidth = 130;
            guna2DataGridView1.Columns["Terlambat"].MinimumWidth = 80;
            guna2DataGridView1.Columns["Action"].Width = 120;

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
                           pg.diterima_oleh, pg.kondisi_alat, pg.terlambat_hari,
                           pg.catatan, pg.status, pg.created_at,
                           p.kode_penyewaan,
                           u.nama AS nama_customer,
                           pu.nama AS nama_diterima
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
                        TerlambatHari = row["terlambat_hari"] != DBNull.Value ? Convert.ToInt32(row["terlambat_hari"]) : 0,
                        KondisiAlat = row["kondisi_alat"]?.ToString() ?? "",
                        DiterimaOlehNama = row["nama_diterima"]?.ToString() ?? "-",
                        Status = row["status"]?.ToString() ?? "diterima",
                        Catatan = row["catatan"]?.ToString() ?? "",
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
                    (p.NamaCustomer?.ToLower().Contains(s) ?? false) ||
                    (p.DiterimaOlehNama?.ToLower().Contains(s) ?? false));
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
                lblStat1Value.Text = _allData.Count.ToString();
                lblStat2Value.Text = _allData.Count(p => p.Status == "diterima").ToString();
                lblStat3Value.Text = _allData.Count(p => p.Status == "perlu_perbaikan").ToString();
                lblStat4Value.Text = _allData.Count(p => p.Status == "ditolak").ToString();
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
                int rowIndex = guna2DataGridView1.Rows.Add(
                    p.Id,
                    p.KodeSewa,
                    p.NamaCustomer,
                    p.TanggalPengembalian != DateTime.MinValue ? p.TanggalPengembalian.ToString("dd/MM/yyyy") : "-",
                    p.TerlambatHari > 0 ? p.TerlambatHari.ToString() + " hr" : "Tepat",
                    string.IsNullOrWhiteSpace(p.KondisiAlat) ? "-" : p.KondisiAlat,
                    p.DiterimaOlehNama,
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
                case "perlu_perbaikan":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                    break;
                case "ditolak":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                    break;
                case "diterima":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(39, 139, 70);
                    break;
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
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
            return "diterima";
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            lbTotal.Text = $"Total: {totalFiltered} pengembalian";
            lbHalaman.Text = $"Halaman {_currentPage} dari {_totalPages}";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        // ============================================
        // ACTION BUTTON (Detail)
        // ============================================
        private void Guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                e.PaintBackground(e.CellBounds, true);

                Rectangle btnRect = new Rectangle(
                    e.CellBounds.X + (e.CellBounds.Width - 84) / 2,
                    e.CellBounds.Y + 3,
                    84,
                    e.CellBounds.Height - 6);

                using (Brush brush = new SolidBrush(Color.FromArgb(23, 59, 99)))
                using (Pen borderPen = new Pen(Color.FromArgb(200, 200, 200), 1))
                using (Font buttonFont = new Font("Segoe UI", 8, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(brush, btnRect);
                    e.Graphics.DrawRectangle(borderPen, btnRect);

                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString("Detail", buttonFont, textBrush, btnRect, sf);
                }

                e.Handled = true;
            }
        }

        private void Guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                if (guna2DataGridView1.Rows[e.RowIndex].Cells["Id"].Value == null)
                    return;

                ulong id = Convert.ToUInt64(guna2DataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                ShowDetail(id);
            }
        }

        private void ShowDetail(ulong id)
        {
            using (var form = new DetailPengembalian(id))
            {
                form.ShowDialog();
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
