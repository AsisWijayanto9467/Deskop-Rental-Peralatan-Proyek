using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin
{
    public partial class Dashboard1 : System.Windows.Forms.UserControl
    {
        // ============================================
        // DATA GRAFIK (7 HARI TERAKHIR)
        // ============================================
        private string[] _chartLabels = new string[7];
        private int[] _chartSewa = new int[7];
        private decimal[] _chartPendapatan = new decimal[7];

        // ============================================
        // COUNTER UNTUK TOMBOL PERLU PERHATIAN
        // ============================================
        private int _pendingSewa = 0;
        private int _pendingPembayaran = 0;

        // ============================================
        // AKTIVITAS TERBARU
        // ============================================
        private readonly List<ActivityLogItem> _aktivitas = new List<ActivityLogItem>();

        private class ActivityLogItem
        {
            public string UserNama { get; set; }
            public string UserRole { get; set; }
            public string Aktivitas { get; set; }
            public string Modul { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public Dashboard1()
        {
            InitializeComponent();
            InitializeAktivitasGridView();
            LoadDashboard();
        }

        private void Dashboard1_Load(object sender, EventArgs e)
        {
            // Data sudah dimuat di constructor
        }

        // ============================================
        // LOAD SEMUA DATA DASHBOARD
        // ============================================
        private void LoadDashboard()
        {
            LoadStatsAlat();
            LoadStatsPenyewaanDanPembayaran();
            LoadPendapatan();
            LoadCharts();
            LoadAktivitas();
            UpdateAlertButton();
        }

        // ============================================
        // STATISTIK ALAT PROYEK
        // ============================================
        private void LoadStatsAlat()
        {
            lblAlatValue.Text = "0";
            lblTersediaValue.Text = "0";
            lblDisewaValue.Text = "0";
            lblMaintenanceValue.Text = "0";

            try
            {
                string query = "SELECT status, COUNT(*) AS jml FROM alat_proyeks GROUP BY status";

                DataTable dt = DatabaseConnection.GetData(query);

                int total = 0, tersedia = 0, disewa = 0, maintenance = 0;

                foreach (DataRow row in dt.Rows)
                {
                    int jml = Convert.ToInt32(row["jml"]);
                    total += jml;

                    string status = row["status"]?.ToString() ?? "";
                    switch (status)
                    {
                        case "tersedia": tersedia = jml; break;
                        case "disewa": disewa = jml; break;
                        case "maintenance": maintenance = jml; break;
                    }
                }

                lblAlatValue.Text = total.ToString("N0");
                lblTersediaValue.Text = tersedia.ToString("N0");
                lblDisewaValue.Text = disewa.ToString("N0");
                lblMaintenanceValue.Text = maintenance.ToString("N0");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error statistik alat: {ex.Message}");
            }
        }

        // ============================================
        // STATISTIK PENYEWAAN & PEMBAYARAN
        // ============================================
        private void LoadStatsPenyewaanDanPembayaran()
        {
            lblAktifValue.Text = "0";
            lblMenungguValue.Text = "0";
            lblPembayaranValue.Text = "0";
            _pendingSewa = 0;
            _pendingPembayaran = 0;

            try
            {
                string query = @"
                    SELECT
                        (SELECT COUNT(*) FROM penyewaans WHERE status = 'sedang_disewa') AS aktif,
                        (SELECT COUNT(*) FROM penyewaans WHERE status = 'pending') AS menunggu,
                        (SELECT COUNT(*) FROM pembayarans WHERE status = 'pending') AS pblm";

                DataTable dt = DatabaseConnection.GetData(query);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    lblAktifValue.Text = Convert.ToInt32(row["aktif"]).ToString("N0");
                    lblMenungguValue.Text = Convert.ToInt32(row["menunggu"]).ToString("N0");
                    lblPembayaranValue.Text = Convert.ToInt32(row["pblm"]).ToString("N0");

                    _pendingSewa = Convert.ToInt32(row["menunggu"]);
                    _pendingPembayaran = Convert.ToInt32(row["pblm"]);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error statistik penyewaan/pembayaran: {ex.Message}");
            }
        }

        // ============================================
        // PENDAPATAN PERIODE BULAN INI
        // ============================================
        private void LoadPendapatan()
        {
            lblPendapatanValue.Text = "Rp 0";

            try
            {
                string query = @"
                    SELECT COALESCE(SUM(jumlah), 0)
                    FROM pembayarans
                    WHERE status = 'diverifikasi'
                      AND DATE_FORMAT(tanggal_pembayaran, '%Y-%m') = DATE_FORMAT(CURDATE(), '%Y-%m')";

                object result = DatabaseConnection.ExecuteScalar(query);

                decimal total = result == null || result == DBNull.Value
                    ? 0m
                    : Convert.ToDecimal(result);

                lblPendapatanValue.Text = "Rp " + total.ToString("N0");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error pendapatan: {ex.Message}");
            }
        }

        // ============================================
        // GRAFIK PENYEWAAN & PENDAPATAN (7 HARI)
        // ============================================
        private void LoadCharts()
        {
            DateTime start = DateTime.Today.AddDays(-6);

            for (int i = 0; i < 7; i++)
            {
                _chartLabels[i] = start.AddDays(i).ToString("dd/MM");
                _chartSewa[i] = 0;
                _chartPendapatan[i] = 0m;
            }

            try
            {
                MySqlParameter[] prmSewa = new MySqlParameter[]
                {
                    new MySqlParameter("@start", start.ToString("yyyy-MM-dd"))
                };

                string qSewa = @"
                    SELECT DATE(tanggal_pengajuan) AS tgl, COUNT(*) AS jml
                    FROM penyewaans
                    WHERE tanggal_pengajuan >= @start
                    GROUP BY DATE(tanggal_pengajuan)";

                DataTable dtSewa = DatabaseConnection.GetData(qSewa, prmSewa);

                var mapSewa = new Dictionary<DateTime, int>();
                foreach (DataRow row in dtSewa.Rows)
                {
                    if (row["tgl"] != DBNull.Value)
                    {
                        DateTime key = Convert.ToDateTime(row["tgl"]).Date;
                        mapSewa[key] = Convert.ToInt32(row["jml"]);
                    }
                }

                for (int i = 0; i < 7; i++)
                {
                    DateTime day = start.AddDays(i);
                    if (mapSewa.TryGetValue(day, out int val))
                    {
                        _chartSewa[i] = val;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error grafik penyewaan: {ex.Message}");
            }

            try
            {
                MySqlParameter[] prmPend = new MySqlParameter[]
                {
                    new MySqlParameter("@start", start.ToString("yyyy-MM-dd"))
                };

                string qPend = @"
                    SELECT DATE(tanggal_pembayaran) AS tgl, COALESCE(SUM(jumlah), 0) AS total
                    FROM pembayarans
                    WHERE status = 'diverifikasi' AND tanggal_pembayaran >= @start
                    GROUP BY DATE(tanggal_pembayaran)";

                DataTable dtPend = DatabaseConnection.GetData(qPend, prmPend);

                var mapPend = new Dictionary<DateTime, decimal>();
                foreach (DataRow row in dtPend.Rows)
                {
                    if (row["tgl"] != DBNull.Value)
                    {
                        DateTime key = Convert.ToDateTime(row["tgl"]).Date;
                        mapPend[key] = Convert.ToDecimal(row["total"]);
                    }
                }

                for (int i = 0; i < 7; i++)
                {
                    DateTime day = start.AddDays(i);
                    if (mapPend.TryGetValue(day, out decimal val))
                    {
                        _chartPendapatan[i] = val;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error grafik pendapatan: {ex.Message}");
            }

            grafikSewa.Invalidate();
            grafikPendapatan.Invalidate();
        }

        // ============================================
        // GAMBAR GRAFIK BAR (GDI+)
        // ============================================
        private void grafikSewa_Paint(object sender, PaintEventArgs e)
        {
            decimal[] values = _chartSewa.Select(v => (decimal)v).ToArray();
            DrawBarChart(e, grafikSewa.ClientRectangle, _chartLabels, values,
                Color.FromArgb(52, 152, 219), false);
        }

        private void grafikPendapatan_Paint(object sender, PaintEventArgs e)
        {
            DrawBarChart(e, grafikPendapatan.ClientRectangle, _chartLabels, _chartPendapatan,
                Color.FromArgb(46, 204, 113), true);
        }

        private void DrawBarChart(PaintEventArgs e, Rectangle area, string[] labels,
            decimal[] values, Color barColor, bool currency)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int n = labels == null ? 0 : labels.Length;
            if (n == 0 || values == null || values.Length == 0)
            {
                using (Font f = new Font("Segoe UI", 9.5F))
                using (Brush b = new SolidBrush(Color.FromArgb(96, 110, 130)))
                {
                    g.DrawString("Belum ada data.", f, b, area.Left + 10, area.Top + 10);
                }
                return;
            }

            n = Math.Min(n, values.Length);
            decimal max = values.Take(n).Max();
            if (max <= 0) max = 1;

            int topLine = area.Top + 16;
            int baselineY = area.Bottom - 22;
            int left = area.Left + 8;
            int right = area.Right - 8;

            using (Pen gridPen = new Pen(Color.FromArgb(236, 240, 245)))
            {
                for (int i = 0; i <= 4; i++)
                {
                    float y = topLine + (baselineY - topLine) * i / 4f;
                    g.DrawLine(gridPen, left, y, right, y);
                }
            }

            using (Pen axisPen = new Pen(Color.FromArgb(200, 205, 212)))
            {
                g.DrawLine(axisPen, left, baselineY, right, baselineY);
            }

            float plotH = baselineY - topLine;
            float slotW = (right - left) / (float)n;
            float barW = Math.Max(4, slotW * 0.5f);

            using (Brush barBrush = new SolidBrush(barColor))
            using (Font labelFont = new Font("Segoe UI", 8F))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(96, 110, 130)))
            {
                for (int i = 0; i < n; i++)
                {
                    float barH = plotH * (float)(values[i] / max);
                    float x = left + slotW * i + (slotW - barW) / 2f;
                    float y = baselineY - barH;

                    if (barH > 0)
                    {
                        g.FillRectangle(barBrush, x, y, barW, barH);

                        string valText = currency ? FormatRupiahRingkas(values[i]) : values[i].ToString("N0");
                        SizeF ts = g.MeasureString(valText, labelFont);
                        float xText = Math.Max(left, x + (barW - ts.Width) / 2f);
                        float yText = y - ts.Height - 2;
                        if (yText < area.Top) yText = area.Top;
                        g.DrawString(valText, labelFont, textBrush, xText, yText);
                    }

                    string lbl = labels[i];
                    SizeF ls = g.MeasureString(lbl, labelFont);
                    float xLbl = Math.Max(left, x + (barW - ls.Width) / 2f);
                    g.DrawString(lbl, labelFont, textBrush, xLbl, baselineY + 6);
                }
            }
        }

        private string FormatRupiahRingkas(decimal value)
        {
            if (value >= 1000000000) return "Rp " + (value / 1000000000m).ToString("0.#") + " M";
            if (value >= 1000000) return "Rp " + (value / 1000000m).ToString("0.#") + " jt";
            if (value >= 1000) return "Rp " + (value / 1000m).ToString("0.#") + " rb";
            return "Rp " + value.ToString("N0");
        }

        // ============================================
        // AKTIVITAS TERBARU
        // ============================================
        private void InitializeAktivitasGridView()
        {
            dgvAktivitas.Columns.Clear();

            dgvAktivitas.Columns.Add("Waktu", "Waktu");
            dgvAktivitas.Columns.Add("User", "User");
            dgvAktivitas.Columns.Add("Role", "Role");
            dgvAktivitas.Columns.Add("Modul", "Modul");
            dgvAktivitas.Columns.Add("Aktivitas", "Aktivitas");

            dgvAktivitas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAktivitas.AllowUserToAddRows = false;
            dgvAktivitas.ReadOnly = true;
            dgvAktivitas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAktivitas.MultiSelect = false;

            dgvAktivitas.Columns["Waktu"].FillWeight = 15;
            dgvAktivitas.Columns["User"].FillWeight = 18;
            dgvAktivitas.Columns["Role"].FillWeight = 10;
            dgvAktivitas.Columns["Modul"].FillWeight = 15;
            dgvAktivitas.Columns["Aktivitas"].FillWeight = 100;
            dgvAktivitas.Columns["Waktu"].MinimumWidth = 120;
            dgvAktivitas.Columns["User"].MinimumWidth = 120;
            dgvAktivitas.Columns["Role"].MinimumWidth = 70;
            dgvAktivitas.Columns["Modul"].MinimumWidth = 90;
        }

        private void LoadAktivitas()
        {
            _aktivitas.Clear();

            try
            {
                string query = @"
                    SELECT al.aktivitas, al.modul, al.created_at, u.nama, u.role
                    FROM activity_logs al
                    LEFT JOIN users u ON u.id = al.user_id
                    WHERE al.aktivitas IS NOT NULL AND al.aktivitas <> ''
                    ORDER BY al.created_at DESC
                    LIMIT 8";

                DataTable dt = DatabaseConnection.GetData(query);

                foreach (DataRow row in dt.Rows)
                {
                    _aktivitas.Add(new ActivityLogItem
                    {
                        UserNama = row["nama"]?.ToString() ?? "Unknown",
                        UserRole = row["role"]?.ToString() ?? "-",
                        Aktivitas = row["aktivitas"]?.ToString() ?? "",
                        Modul = row["modul"]?.ToString() ?? "-",
                        CreatedAt = row["created_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["created_at"])
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aktivitas terbaru: {ex.Message}");
            }

            DisplayAktivitas();
        }

        private void DisplayAktivitas()
        {
            dgvAktivitas.Rows.Clear();

            if (_aktivitas.Count == 0)
            {
                dgvAktivitas.Rows.Add("-", "-", "-", "-", "Belum ada aktivitas tercatat.");
                return;
            }

            foreach (var item in _aktivitas)
            {
                int rowIndex = dgvAktivitas.Rows.Add(
                    item.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                    item.UserNama,
                    item.UserRole,
                    item.Modul,
                    item.Aktivitas
                );

                string aktivitas = item.Aktivitas.ToLower();

                if (aktivitas.Contains("hapus") || aktivitas.Contains("delete"))
                {
                    dgvAktivitas.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                }
                else if (aktivitas.Contains("tambah") || aktivitas.Contains("simpan") ||
                         aktivitas.Contains("buat") || aktivitas.Contains("proses") ||
                         aktivitas.Contains("verifikasi"))
                {
                    dgvAktivitas.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                }
                else if (aktivitas.Contains("login") || aktivitas.Contains("logout"))
                {
                    dgvAktivitas.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(0, 102, 204);
                }
            }
        }

        // ============================================
        // TOMBOL PERLU PERHATIAN
        // ============================================
        private void UpdateAlertButton()
        {
            btnAlert.Text = $"Perlu Perhatian ({_pendingSewa + _pendingPembayaran})";
        }

        // ============================================
        // EVENT HANDLERS
        // ============================================
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                LoadDashboard();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnAlert_Click(object sender, EventArgs e)
        {
            using (var popup = new DashboardAlertPopup())
            {
                popup.ShowDialog(this);
            }

            LoadDashboard();
        }
    }
}