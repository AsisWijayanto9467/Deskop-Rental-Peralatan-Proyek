using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using App_Rental_Proyek.UserControls.Petugas.Laporan;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Laporan
{
    public partial class LaporanPage : System.Windows.Forms.UserControl
    {
        private const int PageSize = 20;

        private TabState _penyewaan;
        private TabState _pembayaran;
        private TabState _pengembalian;
        private TabState _denda;

        private string _activeTab = "penyewaan";

        private class TabState
        {
            public DataTable Data;
            public string Search = "";
            public string Status = "";
            public int Page = 1;

            public Guna2DataGridView Grid;
            public Guna2TextBox SearchBox;
            public Guna2ComboBox StatusBox;
            public Guna2HtmlLabel LbTotal;
            public Guna2HtmlLabel LbHalaman;
            public Guna2Button BtnPrev;
            public Guna2Button BtnNext;

            public string[] ColumnNames;
            public string[] ColumnHeaders;
            public string[] SearchFields;
            public string StatusField;
            public string[] StatusFilterLabels;
            public string[] StatusFilterKeys;
            public Func<DataRow, string[]> CellBuilder;
        }

        public LaporanPage()
        {
            InitializeComponent();
            InitializeTabStates();
            ConfigureAllGrids();
            ConfigureAllFilters();
            LoadAll();
            SwitchTab("penyewaan");
        }

        private void LaporanPage_Load(object sender, EventArgs e)
        {
        }

        // ============================================
        // INISIALISASI STATE TAB
        // ============================================
        private void InitializeTabStates()
        {
            _penyewaan = new TabState
            {
                Grid = dgvPenyewaan,
                SearchBox = txtSearch1,
                StatusBox = cbStatus1,
                LbTotal = lbTotal1,
                LbHalaman = lbHalaman1,
                BtnPrev = btnPrev1,
                BtnNext = btnNext1,
                ColumnNames = new[] { "Kode", "User", "Tanggal", "Alat", "Total", "Status" },
                ColumnHeaders = new[] { "Kode Penyewaan", "User", "Tanggal", "Alat", "Total", "Status" },
                SearchFields = new[] { "kode_penyewaan", "user", "alat" },
                StatusField = "status",
                StatusFilterLabels = new[] { "Semua Status", "Menunggu", "Disetujui", "Dibayar", "Sedang Disewa", "Selesai", "Ditolak", "Dibatalkan" },
                StatusFilterKeys = new[] { "", "pending", "disetujui", "dibayar", "sedang_disewa", "selesai", "ditolak", "dibatalkan" },
                CellBuilder = row => new string[]
                {
                    row["kode_penyewaan"].ToString(),
                    row["user"].ToString(),
                    ToDate(row["tanggal_pengajuan"]),
                    row["alat"].ToString(),
                    "Rp " + ToDecimal(row["total"]).ToString("N0"),
                    FormatPenyewaanStatus(row["status"].ToString())
                }
            };

            _pembayaran = new TabState
            {
                Grid = dgvPembayaran,
                SearchBox = txtSearch2,
                StatusBox = cbStatus2,
                LbTotal = lbTotal2,
                LbHalaman = lbHalaman2,
                BtnPrev = btnPrev2,
                BtnNext = btnNext2,
                ColumnNames = new[] { "Kode", "User", "Penyewaan", "Jumlah", "Metode", "Status" },
                ColumnHeaders = new[] { "Kode Pembayaran", "User", "Penyewaan", "Jumlah", "Metode", "Status" },
                SearchFields = new[] { "kode_pembayaran", "user", "kode_penyewaan" },
                StatusField = "status",
                StatusFilterLabels = new[] { "Semua Status", "Pending", "Diverifikasi", "Ditolak" },
                StatusFilterKeys = new[] { "", "pending", "diverifikasi", "ditolak" },
                CellBuilder = row => new string[]
                {
                    row["kode_pembayaran"].ToString(),
                    row["user"].ToString(),
                    row["kode_penyewaan"].ToString(),
                    "Rp " + ToDecimal(row["jumlah"]).ToString("N0"),
                    FormatMetode(row["metode_pembayaran"].ToString()),
                    FormatPembayaranStatus(row["status"].ToString())
                }
            };

            _pengembalian = new TabState
            {
                Grid = dgvPengembalian,
                SearchBox = txtSearch3,
                StatusBox = cbStatus3,
                LbTotal = lbTotal3,
                LbHalaman = lbHalaman3,
                BtnPrev = btnPrev3,
                BtnNext = btnNext3,
                ColumnNames = new[] { "Penyewaan", "User", "Tanggal", "Kondisi", "Status", "Telat" },
                ColumnHeaders = new[] { "Penyewaan", "User", "Tanggal Kembali", "Kondisi", "Status", "Keterlambatan" },
                SearchFields = new[] { "kode_penyewaan", "user", "kondisi_alat" },
                StatusField = "status",
                StatusFilterLabels = new[] { "Semua Status", "Menunggu Inspeksi", "Diterima", "Perlu Perbaikan", "Ditolak" },
                StatusFilterKeys = new[] { "", "menunggu_inspeksi", "diterima", "perlu_perbaikan", "ditolak" },
                CellBuilder = row => new string[]
                {
                    row["kode_penyewaan"].ToString(),
                    row["user"].ToString(),
                    ToDate(row["tanggal_pengembalian"]),
                    row["kondisi_alat"].ToString(),
                    FormatPengembalianStatus(row["status"].ToString()),
                    ToInt(row["terlambat_hari"]).ToString() + " hari"
                }
            };

            _denda = new TabState
            {
                Grid = dgvDenda,
                SearchBox = txtSearch4,
                StatusBox = cbStatus4,
                LbTotal = lbTotal4,
                LbHalaman = lbHalaman4,
                BtnPrev = btnPrev4,
                BtnNext = btnNext4,
                ColumnNames = new[] { "Penyewaan", "Jenis", "Alasan", "Jumlah", "Status" },
                ColumnHeaders = new[] { "Penyewaan", "Jenis Denda", "Alasan", "Jumlah", "Status" },
                SearchFields = new[] { "kode_penyewaan", "alasan", "jenis_denda" },
                StatusField = "status",
                StatusFilterLabels = new[] { "Semua Status", "Pending", "Dibayar", "Ditangguhkan" },
                StatusFilterKeys = new[] { "", "pending", "dibayar", "ditangguhkan" },
                CellBuilder = row => new string[]
                {
                    row["kode_penyewaan"].ToString(),
                    FormatJenisDenda(row["jenis_denda"].ToString()),
                    row["alasan"].ToString(),
                    "Rp " + ToDecimal(row["jumlah"]).ToString("N0"),
                    FormatDendaStatus(row["status"].ToString())
                }
            };
        }

        private void ConfigureAllGrids()
        {
            ConfigureGrid(_penyewaan);
            ConfigureGrid(_pembayaran);
            ConfigureGrid(_pengembalian);
            ConfigureGrid(_denda);
        }

        private void ConfigureGrid(TabState tab)
        {
            tab.Grid.Columns.Clear();

            for (int i = 0; i < tab.ColumnNames.Length; i++)
            {
                tab.Grid.Columns.Add(tab.ColumnNames[i], tab.ColumnHeaders[i]);
            }

            tab.Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tab.Grid.AllowUserToAddRows = false;
            tab.Grid.ReadOnly = true;
            tab.Grid.RowHeadersVisible = false;
            tab.Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tab.Grid.MultiSelect = false;
            tab.Grid.Columns[tab.ColumnNames[0]].MinimumWidth = 150;
            tab.Grid.Columns[tab.ColumnNames[1]].MinimumWidth = 160;
        }

        private void ConfigureAllFilters()
        {
            ConfigureFilter(_penyewaan);
            ConfigureFilter(_pembayaran);
            ConfigureFilter(_pengembalian);
            ConfigureFilter(_denda);
        }

        private void ConfigureFilter(TabState tab)
        {
            tab.StatusBox.Items.Clear();
            foreach (string label in tab.StatusFilterLabels)
            {
                tab.StatusBox.Items.Add(label);
            }
            tab.StatusBox.SelectedIndex = 0;
        }

        // ============================================
        // LOAD DATA DARI DATABASE
        // ============================================
        private void LoadAll()
        {
            LoadPenyewaan();
            LoadPembayaran();
            LoadPengembalian();
            LoadDenda();
            UpdateStats();
        }

        private void LoadPenyewaan()
        {
            try
            {
                string query = @"
                    SELECT p.id, p.kode_penyewaan, u.nama AS user,
                           COALESCE(GROUP_CONCAT(CONCAT(ap.nama_alat, ' x', dp.jumlah) SEPARATOR ', '), '-') AS alat,
                           p.tanggal_pengajuan, p.total, p.status
                    FROM penyewaans p
                    LEFT JOIN users u ON u.id = p.user_id
                    LEFT JOIN detail_penyewaans dp ON dp.penyewaan_id = p.id
                    LEFT JOIN alat_proyeks ap ON ap.id = dp.alat_id
                    GROUP BY p.id, p.kode_penyewaan, u.nama, p.tanggal_pengajuan, p.total, p.status
                    ORDER BY p.created_at DESC";
                _penyewaan.Data = DatabaseConnection.GetData(query);
                ApplyFilters(_penyewaan);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat laporan penyewaan: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _penyewaan.Data = new DataTable();
                ApplyFilters(_penyewaan);
            }
        }

        private void LoadPembayaran()
        {
            try
            {
                string query = @"
                    SELECT pm.id, pm.kode_pembayaran, u.nama AS user, p.kode_penyewaan,
                           pm.jumlah, pm.metode_pembayaran, pm.status
                    FROM pembayarans pm
                    LEFT JOIN penyewaans p ON p.id = pm.penyewaan_id
                    LEFT JOIN users u ON u.id = p.user_id
                    ORDER BY pm.created_at DESC";
                _pembayaran.Data = DatabaseConnection.GetData(query);
                ApplyFilters(_pembayaran);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat laporan pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _pembayaran.Data = new DataTable();
                ApplyFilters(_pembayaran);
            }
        }

        private void LoadPengembalian()
        {
            try
            {
                string query = @"
                    SELECT pg.id, p.kode_penyewaan, u.nama AS user, pg.tanggal_pengembalian,
                           pg.kondisi_alat, pg.status, pg.terlambat_hari
                    FROM pengembalians pg
                    LEFT JOIN penyewaans p ON p.id = pg.penyewaan_id
                    LEFT JOIN users u ON u.id = p.user_id
                    ORDER BY pg.created_at DESC";
                _pengembalian.Data = DatabaseConnection.GetData(query);
                ApplyFilters(_pengembalian);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat laporan pengembalian: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _pengembalian.Data = new DataTable();
                ApplyFilters(_pengembalian);
            }
        }

        private void LoadDenda()
        {
            try
            {
                string query = @"
                    SELECT d.id, p.kode_penyewaan, d.jenis_denda, d.alasan, d.jumlah, d.status
                    FROM dendas d
                    LEFT JOIN penyewaans p ON p.id = d.penyewaan_id
                    ORDER BY d.created_at DESC";
                _denda.Data = DatabaseConnection.GetData(query);
                ApplyFilters(_denda);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat laporan denda: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _denda.Data = new DataTable();
                ApplyFilters(_denda);
            }
        }

        // ============================================
        // FILTER & PAGINASI GENERIK
        // ============================================
        private void ApplyFilters(TabState tab)
        {
            if (tab.Data == null) tab.Data = new DataTable();

            IEnumerable<DataRow> filtered = tab.Data.Rows.Cast<DataRow>();

            if (!string.IsNullOrEmpty(tab.Search))
            {
                string s = tab.Search.ToLower();
                filtered = filtered.Where(r =>
                    tab.SearchFields.Any(f =>
                        (r[f]?.ToString() ?? "").ToLower().Contains(s)));
            }

            if (!string.IsNullOrEmpty(tab.Status))
            {
                filtered = filtered.Where(r => (r[tab.StatusField]?.ToString() ?? "") == tab.Status);
            }

            var list = filtered.ToList();
            int totalPages = (int)Math.Ceiling((double)list.Count / PageSize);
            if (totalPages == 0) totalPages = 1;
            if (tab.Page > totalPages) tab.Page = totalPages;
            if (tab.Page < 1) tab.Page = 1;

            var pageData = list.Skip((tab.Page - 1) * PageSize).Take(PageSize).ToList();

            tab.Grid.Rows.Clear();
            foreach (var row in pageData)
            {
                tab.Grid.Rows.Add(tab.CellBuilder(row));
            }

            UpdatePagination(tab, list.Count);
        }

        private void UpdatePagination(TabState tab, int total)
        {
            int totalPages = (int)Math.Ceiling((double)total / PageSize);
            if (totalPages == 0) totalPages = 1;

            tab.LbTotal.Text = $"Total: {total} data";
            tab.LbHalaman.Text = $"Halaman {tab.Page} dari {totalPages}";
            tab.BtnPrev.Enabled = tab.Page > 1;
            tab.BtnNext.Enabled = tab.Page < totalPages;
        }

        // ============================================
        // PERPINDAHAN TAB
        // ============================================
        private void SwitchTab(string tab)
        {
            _activeTab = tab;

            pnlPenyewaan.Visible = tab == "penyewaan";
            pnlPembayaran.Visible = tab == "pembayaran";
            pnlPengembalian.Visible = tab == "pengembalian";
            pnlDenda.Visible = tab == "denda";

            SetTabButtonActive(btnTabPenyewaan, tab == "penyewaan");
            SetTabButtonActive(btnTabPembayaran, tab == "pembayaran");
            SetTabButtonActive(btnTabPengembalian, tab == "pengembalian");
            SetTabButtonActive(btnTabDenda, tab == "denda");

            UpdateStats();

            LogActivity($"Melihat laporan {GetTabLabel(tab)}", "Laporan");
        }

        private void SetTabButtonActive(Guna2Button btn, bool active)
        {
            if (active)
            {
                btn.FillColor = Color.FromArgb(23, 59, 99);
                btn.ForeColor = Color.White;
                btn.BorderColor = Color.FromArgb(23, 59, 99);
            }
            else
            {
                btn.FillColor = Color.White;
                btn.ForeColor = Color.FromArgb(23, 59, 99);
                btn.BorderColor = Color.FromArgb(200, 210, 220);
            }
        }

        private string GetTabLabel(string tab)
        {
            switch (tab)
            {
                case "penyewaan": return "Penyewaan";
                case "pembayaran": return "Pembayaran";
                case "pengembalian": return "Pengembalian";
                case "denda": return "Denda";
                default: return tab;
            }
        }

        private TabState GetActiveTab()
        {
            switch (_activeTab)
            {
                case "pembayaran": return _pembayaran;
                case "pengembalian": return _pengembalian;
                case "denda": return _denda;
                default: return _penyewaan;
            }
        }

        // ============================================
        // STATISTIK
        // ============================================
        private void UpdateStats()
        {
            try
            {
                switch (_activeTab)
                {
                    case "pembayaran":
                        UpdateStatsPembayaran();
                        break;
                    case "pengembalian":
                        UpdateStatsPengembalian();
                        break;
                    case "denda":
                        UpdateStatsDenda();
                        break;
                    default:
                        UpdateStatsPenyewaan();
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error update statistik: {ex.Message}");
            }
        }

        private int CountStatus(DataTable dt, string status)
        {
            if (dt == null || dt.Rows.Count == 0) return 0;
            return dt.Rows.Cast<DataRow>().Count(r => (r["status"]?.ToString() ?? "") == status);
        }

        private void UpdateStatsPenyewaan()
        {
            var dt = _penyewaan.Data;
            lblStat1Value.Text = (dt?.Rows.Count ?? 0).ToString();
            lblStat1Caption.Text = "Total Penyewaan";
            lblStat2Value.Text = CountStatus(dt, "selesai").ToString();
            lblStat2Caption.Text = "Selesai";
            lblStat3Value.Text = CountStatus(dt, "sedang_disewa").ToString();
            lblStat3Caption.Text = "Sedang Disewa";
            lblStat4Value.Text = CountStatus(dt, "dibatalkan").ToString();
            lblStat4Caption.Text = "Dibatalkan";
        }

        private void UpdateStatsPembayaran()
        {
            var dt = _pembayaran.Data;
            lblStat1Value.Text = (dt?.Rows.Count ?? 0).ToString();
            lblStat1Caption.Text = "Total Pembayaran";
            lblStat2Value.Text = CountStatus(dt, "diverifikasi").ToString();
            lblStat2Caption.Text = "Diverifikasi";
            lblStat3Value.Text = CountStatus(dt, "pending").ToString();
            lblStat3Caption.Text = "Pending";
            lblStat4Value.Text = CountStatus(dt, "ditolak").ToString();
            lblStat4Caption.Text = "Ditolak";
        }

        private void UpdateStatsPengembalian()
        {
            var dt = _pengembalian.Data;
            lblStat1Value.Text = (dt?.Rows.Count ?? 0).ToString();
            lblStat1Caption.Text = "Total Pengembalian";
            lblStat2Value.Text = CountStatus(dt, "diterima").ToString();
            lblStat2Caption.Text = "Diterima";
            lblStat3Value.Text = CountStatus(dt, "menunggu_inspeksi").ToString();
            lblStat3Caption.Text = "Menunggu Inspeksi";
            lblStat4Value.Text = CountStatus(dt, "perlu_perbaikan").ToString();
            lblStat4Caption.Text = "Perlu Perbaikan";
        }

        private void UpdateStatsDenda()
        {
            var dt = _denda.Data;
            lblStat1Value.Text = (dt?.Rows.Count ?? 0).ToString();
            lblStat1Caption.Text = "Total Denda";
            lblStat2Value.Text = CountStatus(dt, "pending").ToString();
            lblStat2Caption.Text = "Pending";
            lblStat3Value.Text = CountStatus(dt, "dibayar").ToString();
            lblStat3Caption.Text = "Dibayar";
            lblStat4Value.Text = CountStatus(dt, "ditangguhkan").ToString();
            lblStat4Caption.Text = "Ditangguhkan";
        }

        // ============================================
        // FORMATTING
        // ============================================
        private string ToDate(object value)
        {
            if (value == null || value == DBNull.Value) return "-";
            try { return Convert.ToDateTime(value).ToString("dd/MM/yyyy"); }
            catch { return "-"; }
        }

        private decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value) return 0m;
            try { return Convert.ToDecimal(value); }
            catch { return 0m; }
        }

        private int ToInt(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            try { return Convert.ToInt32(value); }
            catch { return 0; }
        }

        private string FormatPenyewaanStatus(string s)
        {
            switch (s)
            {
                case "pending": return "Menunggu";
                case "disetujui": return "Disetujui";
                case "ditolak": return "Ditolak";
                case "menunggu_pembayaran": return "Menunggu Pembayaran";
                case "dibayar": return "Dibayar";
                case "sedang_disewa": return "Sedang Disewa";
                case "selesai": return "Selesai";
                case "dibatalkan": return "Dibatalkan";
                default: return s;
            }
        }

        private string FormatPembayaranStatus(string s)
        {
            switch (s)
            {
                case "pending": return "Pending";
                case "diverifikasi": return "Diverifikasi";
                case "ditolak": return "Ditolak";
                default: return s;
            }
        }

        private string FormatMetode(string s)
        {
            switch (s)
            {
                case "cash": return "Cash";
                case "transfer": return "Transfer";
                case "qris": return "QRIS";
                default: return s;
            }
        }

        private string FormatPengembalianStatus(string s)
        {
            switch (s)
            {
                case "menunggu_inspeksi": return "Menunggu Inspeksi";
                case "diterima": return "Diterima";
                case "perlu_perbaikan": return "Perlu Perbaikan";
                case "ditolak": return "Ditolak";
                default: return s;
            }
        }

        private string FormatDendaStatus(string s)
        {
            switch (s)
            {
                case "pending": return "Pending";
                case "dibayar": return "Dibayar";
                case "ditangguhkan": return "Ditangguhkan";
                default: return s;
            }
        }

        private string FormatJenisDenda(string s)
        {
            switch (s)
            {
                case "terlambat": return "Terlambat";
                case "kerusakan": return "Kerusakan";
                case "kehilangan": return "Kehilangan";
                default: return s;
            }
        }

        // ============================================
        // ACTIVITY LOG
        // ============================================
        private void LogActivity(string aktivitas, string modul, ulong? referensiId = null)
        {
            ActivityLogHelper.LogForSession(SessionManager.GetCurrentUserId(), aktivitas, modul, referensiId);
        }

        // ============================================
        // EVENT HANDLERS TAB
        // ============================================
        private void btnTabPenyewaan_Click(object sender, EventArgs e) { SwitchTab("penyewaan"); }
        private void btnTabPembayaran_Click(object sender, EventArgs e) { SwitchTab("pembayaran"); }
        private void btnTabPengembalian_Click(object sender, EventArgs e) { SwitchTab("pengembalian"); }
        private void btnTabDenda_Click(object sender, EventArgs e) { SwitchTab("denda"); }

        // ============================================
        // EVENT HANDLERS SEARCH / FILTER
        // ============================================
        private void txtSearch1_TextChanged(object sender, EventArgs e)
        {
            _penyewaan.Search = txtSearch1.Text.Trim();
            _penyewaan.Page = 1;
            ApplyFilters(_penyewaan);
        }
        private void txtSearch2_TextChanged(object sender, EventArgs e)
        {
            _pembayaran.Search = txtSearch2.Text.Trim();
            _pembayaran.Page = 1;
            ApplyFilters(_pembayaran);
        }
        private void txtSearch3_TextChanged(object sender, EventArgs e)
        {
            _pengembalian.Search = txtSearch3.Text.Trim();
            _pengembalian.Page = 1;
            ApplyFilters(_pengembalian);
        }
        private void txtSearch4_TextChanged(object sender, EventArgs e)
        {
            _denda.Search = txtSearch4.Text.Trim();
            _denda.Page = 1;
            ApplyFilters(_denda);
        }

        private void cbStatus1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStatus1.SelectedIndex >= 0 && cbStatus1.SelectedIndex < _penyewaan.StatusFilterKeys.Length)
            {
                _penyewaan.Status = _penyewaan.StatusFilterKeys[cbStatus1.SelectedIndex];
                _penyewaan.Page = 1;
                ApplyFilters(_penyewaan);
            }
        }
        private void cbStatus2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStatus2.SelectedIndex >= 0 && cbStatus2.SelectedIndex < _pembayaran.StatusFilterKeys.Length)
            {
                _pembayaran.Status = _pembayaran.StatusFilterKeys[cbStatus2.SelectedIndex];
                _pembayaran.Page = 1;
                ApplyFilters(_pembayaran);
            }
        }
        private void cbStatus3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStatus3.SelectedIndex >= 0 && cbStatus3.SelectedIndex < _pengembalian.StatusFilterKeys.Length)
            {
                _pengembalian.Status = _pengembalian.StatusFilterKeys[cbStatus3.SelectedIndex];
                _pengembalian.Page = 1;
                ApplyFilters(_pengembalian);
            }
        }
        private void cbStatus4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStatus4.SelectedIndex >= 0 && cbStatus4.SelectedIndex < _denda.StatusFilterKeys.Length)
            {
                _denda.Status = _denda.StatusFilterKeys[cbStatus4.SelectedIndex];
                _denda.Page = 1;
                ApplyFilters(_denda);
            }
        }

        // ============================================
        // PAGINATION
        // ============================================
        private void btnPrev1_Click(object sender, EventArgs e)
        {
            if (_penyewaan.Page > 1) { _penyewaan.Page--; ApplyFilters(_penyewaan); }
        }
        private void btnNext1_Click(object sender, EventArgs e)
        {
            _penyewaan.Page++; ApplyFilters(_penyewaan);
        }
        private void btnPrev2_Click(object sender, EventArgs e)
        {
            if (_pembayaran.Page > 1) { _pembayaran.Page--; ApplyFilters(_pembayaran); }
        }
        private void btnNext2_Click(object sender, EventArgs e)
        {
            _pembayaran.Page++; ApplyFilters(_pembayaran);
        }
        private void btnPrev3_Click(object sender, EventArgs e)
        {
            if (_pengembalian.Page > 1) { _pengembalian.Page--; ApplyFilters(_pengembalian); }
        }
        private void btnNext3_Click(object sender, EventArgs e)
        {
            _pengembalian.Page++; ApplyFilters(_pengembalian);
        }
        private void btnPrev4_Click(object sender, EventArgs e)
        {
            if (_denda.Page > 1) { _denda.Page--; ApplyFilters(_denda); }
        }
        private void btnNext4_Click(object sender, EventArgs e)
        {
            _denda.Page++; ApplyFilters(_denda);
        }

        // ============================================
        // TOMBOL UTAMA
        // ============================================
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAll();
            LogActivity($"Menyegarkan halaman Laporan ({GetTabLabel(_activeTab)})", "Laporan");
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            using (var form = new DownloadLaporanForm())
            {
                form.ShowDialog(this);
            }
        }
    }
}
