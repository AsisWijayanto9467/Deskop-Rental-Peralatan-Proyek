using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.ActivityLog
{
    public class ActivityLogViewItem
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public string UserNama { get; set; } = "";
        public string UserRole { get; set; } = "";
        public string Aktivitas { get; set; } = "";
        public string Modul { get; set; } = "";
        public string ReferensiId { get; set; } = "";
        public string IpAddress { get; set; } = "";
        public DateTime? CreatedAt { get; set; }

        public string CreatedAtDisplay => CreatedAt?.ToString("dd/MM/yyyy HH:mm:ss") ?? "-";
    }

    public partial class ActivityLogList : System.Windows.Forms.UserControl
    {
        private List<ActivityLogViewItem> _allLogs = new List<ActivityLogViewItem>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 0;
        private string _currentSearch = "";
        private string _currentModul = "Semua";
        private string _currentRole = "Semua";

        public ActivityLogList()
        {
            InitializeComponent();
            InitializeGridView();
            InitializeComboBoxes();
            LoadLogs();
        }

        private void ActivityLogList_Load(object sender, EventArgs e)
        {
        }

        // ============================================
        // INISIALISASI
        // ============================================
        private void InitializeGridView()
        {
            guna2DataGridView1.Columns.Clear();

            guna2DataGridView1.Columns.Add("Id", "ID");
            guna2DataGridView1.Columns.Add("Waktu", "Waktu");
            guna2DataGridView1.Columns.Add("User", "User");
            guna2DataGridView1.Columns.Add("Role", "Role");
            guna2DataGridView1.Columns.Add("Aktivitas", "Aktivitas");
            guna2DataGridView1.Columns.Add("Modul", "Modul");
            guna2DataGridView1.Columns.Add("Referensi", "Referensi ID");
            guna2DataGridView1.Columns.Add("Ip", "IP Address");

            DataGridViewColumn colDetail = new DataGridViewColumn();
            colDetail.Name = "Detail";
            colDetail.HeaderText = "Detail";
            colDetail.CellTemplate = new DataGridViewTextBoxCell();
            colDetail.Width = 100;
            colDetail.MinimumWidth = 100;
            guna2DataGridView1.Columns.Add(colDetail);

            guna2DataGridView1.Columns["Id"].Visible = false;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;

            guna2DataGridView1.Columns["Waktu"].MinimumWidth = 140;
            guna2DataGridView1.Columns["User"].MinimumWidth = 130;
            guna2DataGridView1.Columns["Role"].MinimumWidth = 70;
            guna2DataGridView1.Columns["Aktivitas"].MinimumWidth = 180;
            guna2DataGridView1.Columns["Modul"].MinimumWidth = 100;
            guna2DataGridView1.Columns["Ip"].MinimumWidth = 100;
            guna2DataGridView1.Columns["Detail"].Width = 100;

            guna2DataGridView1.CellPainting += Guna2DataGridView1_CellPainting;
            guna2DataGridView1.CellClick += Guna2DataGridView1_CellClick;
            guna2DataGridView1.CellContentClick -= guna2DataGridView1_CellContentClick_1;
        }

        private void InitializeComboBoxes()
        {
            cbModul.Items.Clear();
            cbModul.Items.Add("Semua");
            cbModul.Items.Add("Login");
            cbModul.Items.Add("User");
            cbModul.Items.Add("Alat Proyek");
            cbModul.Items.Add("Kategori");
            cbModul.Items.Add("Lokasi");
            cbModul.Items.Add("Penyewaan");
            cbModul.Items.Add("Pembayaran");
            cbModul.Items.Add("Pengembalian");
            cbModul.Items.Add("Denda");
            cbModul.SelectedIndex = 0;

            cbRole.Items.Clear();
            cbRole.Items.Add("Semua");
            cbRole.Items.Add("admin");
            cbRole.Items.Add("petugas");
            cbRole.Items.Add("user");
            cbRole.SelectedIndex = 0;
        }

        // ============================================
        // LOAD DATA DARI DATABASE
        // ============================================
        private List<ActivityLogViewItem> GetAllLogsFromDatabase()
        {
            var logs = new List<ActivityLogViewItem>();

            try
            {
                string query = @"
                    SELECT al.id, al.user_id, al.aktivitas, al.modul, al.referensi_id, al.ip_address, al.created_at,
                           u.nama, u.role
                    FROM activity_logs al
                    LEFT JOIN users u ON u.id = al.user_id
                    ORDER BY al.created_at DESC";

                DataTable dt = DatabaseConnection.GetData(query);

                foreach (DataRow row in dt.Rows)
                {
                    ulong referensi = 0;
                    string refText = "-";
                    if (row["referensi_id"] != DBNull.Value && row["referensi_id"] != null)
                    {
                        try
                        {
                            referensi = Convert.ToUInt64(row["referensi_id"]);
                            refText = referensi.ToString();
                        }
                        catch
                        {
                            refText = row["referensi_id"].ToString();
                        }
                    }

                    logs.Add(new ActivityLogViewItem
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        UserId = Convert.ToUInt64(row["user_id"]),
                        UserNama = row["nama"]?.ToString() ?? "Unknown",
                        UserRole = row["role"]?.ToString() ?? "-",
                        Aktivitas = row["aktivitas"]?.ToString() ?? "",
                        Modul = row["modul"]?.ToString() ?? "-",
                        ReferensiId = refText,
                        IpAddress = row["ip_address"]?.ToString() ?? "-",
                        CreatedAt = row["created_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["created_at"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to database: {ex.Message}\n\nPlease check your connection string and make sure the database is running.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return logs;
        }

        private void LoadLogs()
        {
            try
            {
                _allLogs = GetAllLogsFromDatabase() ?? new List<ActivityLogViewItem>();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading activity logs: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _allLogs = new List<ActivityLogViewItem>();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (_allLogs == null) _allLogs = new List<ActivityLogViewItem>();

            var filtered = new List<ActivityLogViewItem>(_allLogs);

            if (!string.IsNullOrEmpty(_currentSearch))
            {
                string search = _currentSearch.ToLower();
                filtered = filtered.FindAll(l =>
                    l.UserNama.ToLower().Contains(search) ||
                    l.Aktivitas.ToLower().Contains(search) ||
                    l.Modul.ToLower().Contains(search) ||
                    l.IpAddress.ToLower().Contains(search) ||
                    l.ReferensiId.ToLower().Contains(search)
                );
            }

            if (_currentModul != "Semua")
            {
                filtered = filtered.FindAll(l => l.Modul.Equals(_currentModul, StringComparison.OrdinalIgnoreCase));
            }

            if (_currentRole != "Semua")
            {
                filtered = filtered.FindAll(l => l.UserRole.Equals(_currentRole, StringComparison.OrdinalIgnoreCase));
            }

            _totalPages = (int)Math.Ceiling((double)filtered.Count / PageSize);
            if (_totalPages == 0) _totalPages = 1;

            if (_currentPage > _totalPages) _currentPage = _totalPages;

            var pageLogs = filtered
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            DisplayLogs(pageLogs);
            UpdatePaginationInfo(filtered.Count);
        }

        private void DisplayLogs(List<ActivityLogViewItem> logs)
        {
            guna2DataGridView1.Rows.Clear();

            if (logs == null || logs.Count == 0)
            {
                UpdatePaginationInfo(0);
                return;
            }

            foreach (var log in logs)
            {
                int rowIndex = guna2DataGridView1.Rows.Add(
                    log.Id,
                    log.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                    log.UserNama,
                    log.UserRole,
                    log.Aktivitas,
                    log.Modul,
                    log.ReferensiId,
                    log.IpAddress,
                    ""
                );

                if (log.Modul.Equals("Login", StringComparison.OrdinalIgnoreCase))
                {
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(0, 102, 204);
                }
                else if (log.Aktivitas.ToLower().Contains("hapus") || log.Aktivitas.ToLower().Contains("delete"))
                {
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                }
                else if (log.Aktivitas.ToLower().Contains("tambah") || log.Aktivitas.ToLower().Contains("simpan") ||
                         log.Aktivitas.ToLower().Contains("menu"))
                {
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                }
            }
        }

        // ============================================
        // CELL PAINTING - Tombol Detail
        // ============================================
        private void Guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Detail")
            {
                e.PaintBackground(e.CellBounds, true);

                Rectangle btnRect = new Rectangle(
                    e.CellBounds.X + (e.CellBounds.Width - 70) / 2,
                    e.CellBounds.Y + 3,
                    70,
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
                    e.Graphics.DrawString("Lihat", buttonFont, textBrush, btnRect, sf);
                }

                e.Handled = true;
            }
        }

        private void Guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Detail")
            {
                if (guna2DataGridView1.Rows[e.RowIndex].Cells["Id"].Value == null)
                    return;

                ulong logId = Convert.ToUInt64(guna2DataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                var log = _allLogs.Find(x => x.Id == logId);
                if (log != null)
                {
                    ShowDetail(log);
                }
            }
        }

        private void guna2DataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void ShowDetail(ActivityLogViewItem log)
        {
            using (var detail = new ActivityLogDetail(log))
            {
                detail.ShowDialog();
            }
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            lbTotalLog.Text = $"Total: {totalFiltered} log";
            lbPetunjukHalaman.Text = $"Halaman {_currentPage} dari {_totalPages}";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
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

        private void cbModul_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbModul.SelectedItem != null)
            {
                _currentModul = cbModul.SelectedItem.ToString();
                _currentPage = 1;
                ApplyFilters();
            }
        }

        private void cbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbRole.SelectedItem != null)
            {
                _currentRole = cbRole.SelectedItem.ToString();
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
            LoadLogs();
        }

        private void lbTotalLog_Click(object sender, EventArgs e)
        {
        }

        private void lbPetunjukHalaman_Click(object sender, EventArgs e)
        {
        }
    }
}
