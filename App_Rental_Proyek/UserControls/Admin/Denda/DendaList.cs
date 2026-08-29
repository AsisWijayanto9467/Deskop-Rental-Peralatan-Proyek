using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using App_Rental_Proyek.UserControls.Admin.Denda;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.Denda
{
    public class DendaViewItem
    {
        public DendaModel Denda { get; set; } = new DendaModel();
        public string KodeSewa { get; set; } = "";
        public string NamaCustomer { get; set; } = "";
    }

    public partial class DendaList : System.Windows.Forms.UserControl
    {
        private List<DendaViewItem> _allData = new List<DendaViewItem>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 1;
        private string _currentSearch = "";
        private string _currentStatus = ""; // "" = Semua

        private readonly string[] _statusFilterLabels =
        {
            "Semua", "Belum Dibayar", "Dibayar", "Ditangguhkan"
        };

        private readonly string[] _statusFilterKeys =
        {
            "", "pending", "dibayar", "ditangguhkan"
        };

        public DendaList()
        {
            InitializeComponent();
            InitializeGridView();
            InitializeFilters();
            LoadData();
        }

        private void DendaList_Load(object sender, EventArgs e)
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
            guna2DataGridView1.Columns.Add("Jenis", "Jenis Denda");
            guna2DataGridView1.Columns.Add("Nominal", "Nominal");
            guna2DataGridView1.Columns.Add("Status", "Status Pembayaran");
            guna2DataGridView1.Columns.Add("Tanggal", "Tanggal Denda");
            guna2DataGridView1.Columns.Add("Alasan", "Alasan Denda");

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
            guna2DataGridView1.Columns["Customer"].MinimumWidth = 160;
            guna2DataGridView1.Columns["Jenis"].MinimumWidth = 130;
            guna2DataGridView1.Columns["Nominal"].MinimumWidth = 130;
            guna2DataGridView1.Columns["Status"].MinimumWidth = 130;
            guna2DataGridView1.Columns["Tanggal"].MinimumWidth = 130;
            guna2DataGridView1.Columns["Alasan"].MinimumWidth = 200;
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
        private List<DendaViewItem> GetAllFromDatabase()
        {
            var list = new List<DendaViewItem>();
            try
            {
                string query = @"
                    SELECT d.id, d.penyewaan_id, d.pengembalian_id, d.jenis_denda,
                           d.jumlah, d.alasan, d.status, d.created_at, d.updated_at,
                           p.kode_penyewaan,
                           u.nama AS nama_customer
                    FROM dendas d
                    LEFT JOIN penyewaans p ON p.id = d.penyewaan_id
                    LEFT JOIN users u ON u.id = p.user_id
                    ORDER BY d.created_at DESC, d.id DESC";

                DataTable dt = DatabaseConnection.GetData(query);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new DendaViewItem
                    {
                        Denda = new DendaModel
                        {
                            Id = Convert.ToUInt64(row["id"]),
                            PenyewaanId = row["penyewaan_id"] != DBNull.Value ? Convert.ToUInt64(row["penyewaan_id"]) : 0,
                            PengembalianId = row["pengembalian_id"] != DBNull.Value ? Convert.ToUInt64(row["pengembalian_id"]) : (ulong?)null,
                            JenisDenda = row["jenis_denda"]?.ToString() ?? "",
                            Jumlah = row["jumlah"] != DBNull.Value ? Convert.ToDecimal(row["jumlah"]) : 0m,
                            Alasan = row["alasan"]?.ToString() ?? "",
                            Status = row["status"]?.ToString() ?? "pending",
                            CreatedAt = row["created_at"] as DateTime?,
                            UpdatedAt = row["updated_at"] as DateTime?
                        },
                        KodeSewa = row["kode_penyewaan"]?.ToString() ?? "-",
                        NamaCustomer = row["nama_customer"]?.ToString() ?? "-"
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengambil data denda: {ex.Message}\n\nPastikan tabel 'denda' sudah dibuat di database.",
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
                if (_allData == null) _allData = new List<DendaViewItem>();
                ApplyFilters();
                UpdateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data denda: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _allData = new List<DendaViewItem>();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (_allData == null) _allData = new List<DendaViewItem>();

            var filtered = new List<DendaViewItem>(_allData);

            if (!string.IsNullOrEmpty(_currentSearch))
            {
                string s = _currentSearch.ToLower();
                filtered = filtered.FindAll(p =>
                    (p.KodeSewa?.ToLower().Contains(s) ?? false) ||
                    (p.NamaCustomer?.ToLower().Contains(s) ?? false) ||
                    (FormatJenisLabel(p.Denda.JenisDenda)?.ToLower().Contains(s) ?? false) ||
                    (p.Denda.Alasan?.ToLower().Contains(s) ?? false));
            }

            if (!string.IsNullOrEmpty(_currentStatus))
            {
                filtered = filtered.FindAll(p => p.Denda.Status == _currentStatus);
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
                decimal totalNominal = _allData.Sum(p => p.Denda.Jumlah);
                lblStat1Value.Text = _allData.Count.ToString();
                lblStat2Value.Text = _allData.Count(p => p.Denda.Status == "pending").ToString();
                lblStat3Value.Text = _allData.Count(p => p.Denda.Status == "dibayar").ToString();
                lblStat4Value.Text = _allData.Count(p => p.Denda.Status == "ditangguhkan").ToString();

                lblStat1Caption.Text = $"Total Denda ({FormatRupiahRingkas(totalNominal)})";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error update statistik: {ex.Message}");
            }
        }

        private void DisplayData(List<DendaViewItem> list)
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
                    p.Denda.Id,
                    p.KodeSewa,
                    p.NamaCustomer,
                    FormatJenisLabel(p.Denda.JenisDenda),
                    "Rp " + p.Denda.Jumlah.ToString("N0"),
                    FormatStatusLabel(p.Denda.Status),
                    p.Denda.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                    string.IsNullOrWhiteSpace(p.Denda.Alasan) ? "-" : p.Denda.Alasan,
                    ""
                );

                ApplyRowColor(p, rowIndex);
            }
        }

        private void ApplyRowColor(DendaViewItem p, int rowIndex)
        {
            switch (p.Denda.Status)
            {
                case "pending":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(230, 126, 34);
                    break;
                case "dibayar":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                    break;
                case "ditangguhkan":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(52, 152, 219);
                    break;
            }
        }

        private string FormatJenisLabel(string jenis)
        {
            switch (jenis)
            {
                case "terlambat": return "Keterlambatan";
                case "kerusakan": return "Kerusakan";
                case "kehilangan": return "Kehilangan";
                case "kekurangan": return "Kekurangan Komponen";
                default: return string.IsNullOrWhiteSpace(jenis) ? "-" : jenis;
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "pending": return "Belum Dibayar";
                case "dibayar": return "Dibayar";
                case "ditangguhkan": return "Ditangguhkan";
                default: return status;
            }
        }

        private string FormatRupiahRingkas(decimal value)
        {
            if (value >= 1000000000) return "Rp " + (value / 1000000000m).ToString("0.#") + " M";
            if (value >= 1000000) return "Rp " + (value / 1000000m).ToString("0.#") + " jt";
            if (value >= 1000) return "Rp " + (value / 1000m).ToString("0.#") + " rb";
            return "Rp " + value.ToString("N0");
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            lbTotal.Text = $"Total: {totalFiltered} denda";
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
            DendaViewItem item = _allData.FirstOrDefault(p => p.Denda.Id == id);
            if (item == null) return;

            using (var form = new DendaDetail(item))
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