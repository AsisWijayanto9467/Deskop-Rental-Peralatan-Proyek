using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using App_Rental_Proyek.Model;
using App_Rental_Proyek.UserControls.Admin.Pembayaran;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.Pembayaran
{
    public partial class PembayaranList : System.Windows.Forms.UserControl
    {
        private List<PembayaranModel> _allPembayaran = new List<PembayaranModel>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 1;
        private string _currentSearch = "";
        private string _currentStatus = ""; // "" = Semua

        private readonly string[] _statusFilterLabels = { "Semua", "Menunggu", "Lunas", "Gagal" };

        private readonly string[] _statusFilterKeys = { "", "pending", "diverifikasi", "ditolak" };

        private class ActionButtonSpec
        {
            public string Text;
            public Color Color;
            public Rectangle Bounds;
        }

        public PembayaranList()
        {
            InitializeComponent();
            InitializeGridView();
            InitializeFilters();
            LoadPembayaran();
        }

        private void PembayaranList_Load(object sender, EventArgs e)
        {
            // Sudah dimuat di constructor
        }

        // ============================================
        // INISIALISASI
        // ============================================
        private void InitializeGridView()
        {
            guna2DataGridView1.Columns.Clear();

            guna2DataGridView1.Columns.Add("Id", "ID");
            guna2DataGridView1.Columns.Add("Kode", "Kode Bayar");
            guna2DataGridView1.Columns.Add("KodeSewa", "Kode Sewa");
            guna2DataGridView1.Columns.Add("Customer", "Customer");
            guna2DataGridView1.Columns.Add("Metode", "Metode");
            guna2DataGridView1.Columns.Add("Tanggal", "Tgl Bayar");
            guna2DataGridView1.Columns.Add("Jumlah", "Nominal");
            guna2DataGridView1.Columns.Add("Status", "Status");
            guna2DataGridView1.Columns.Add("Verifikator", "Verifikasi");
            guna2DataGridView1.Columns.Add("CatatanVerif", "Catatan Verifikasi");

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

            guna2DataGridView1.Columns["Kode"].MinimumWidth = 120;
            guna2DataGridView1.Columns["KodeSewa"].MinimumWidth = 110;
            guna2DataGridView1.Columns["Customer"].MinimumWidth = 150;
            guna2DataGridView1.Columns["Metode"].MinimumWidth = 90;
            guna2DataGridView1.Columns["Tanggal"].MinimumWidth = 95;
            guna2DataGridView1.Columns["Jumlah"].MinimumWidth = 120;
            guna2DataGridView1.Columns["Status"].MinimumWidth = 95;
            guna2DataGridView1.Columns["Verifikator"].MinimumWidth = 100;
            guna2DataGridView1.Columns["CatatanVerif"].MinimumWidth = 120;
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
        private List<PembayaranModel> GetAllPembayaranFromDatabase()
        {
            var list = new List<PembayaranModel>();
            try
            {
                string query = @"
                    SELECT pm.id, pm.penyewaan_id, pm.kode_pembayaran, pm.tanggal_pembayaran,
                           pm.jumlah, pm.metode_pembayaran, pm.bukti_pembayaran, pm.status,
                           pm.diverifikasi_oleh, pm.tanggal_verifikasi, pm.catatan,
                           pm.created_at, pm.updated_at,
                           p.kode_penyewaan, p.total AS total_sewa, p.status AS status_penyewaan,
                           u.nama AS nama_customer, u.email AS email_customer,
                           u.no_telepon AS no_telepon_customer, u.alamat AS alamat_customer,
                           pt.nama AS nama_verifikator
                    FROM pembayarans pm
                    LEFT JOIN penyewaans p ON p.id = pm.penyewaan_id
                    LEFT JOIN users u ON u.id = p.user_id
                    LEFT JOIN users pt ON pt.id = pm.diverifikasi_oleh
                    ORDER BY pm.created_at DESC";

                DataTable dt = DatabaseConnection.GetData(query);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new PembayaranModel
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        PenyewaanId = row["penyewaan_id"] != DBNull.Value ? Convert.ToUInt64(row["penyewaan_id"]) : 0,
                        KodePembayaran = row["kode_pembayaran"]?.ToString() ?? "",
                        TanggalPembayaran = row["tanggal_pembayaran"] != DBNull.Value ? Convert.ToDateTime(row["tanggal_pembayaran"]) : DateTime.MinValue,
                        Jumlah = row["jumlah"] != DBNull.Value ? Convert.ToDecimal(row["jumlah"]) : 0m,
                        MetodePembayaran = row["metode_pembayaran"]?.ToString() ?? "cash",
                        BuktiPembayaran = row["bukti_pembayaran"]?.ToString(),
                        Status = row["status"]?.ToString() ?? "pending",
                        DiverifikasiOleh = row["diverifikasi_oleh"] != DBNull.Value ? Convert.ToUInt64(row["diverifikasi_oleh"]) : (ulong?)null,
                        TanggalVerifikasi = row["tanggal_verifikasi"] as DateTime?,
                        Catatan = row["catatan"]?.ToString(),
                        CreatedAt = row["created_at"] as DateTime?,
                        UpdatedAt = row["updated_at"] as DateTime?,
                        KodePenyewaan = row["kode_penyewaan"]?.ToString() ?? "-",
                        TotalSewa = row["total_sewa"] != DBNull.Value ? Convert.ToDecimal(row["total_sewa"]) : 0m,
                        StatusPenyewaan = row["status_penyewaan"]?.ToString() ?? "",
                        NamaCustomer = row["nama_customer"]?.ToString() ?? "-",
                        EmailCustomer = row["email_customer"]?.ToString() ?? "",
                        NoTeleponCustomer = row["no_telepon_customer"]?.ToString() ?? "",
                        AlamatCustomer = row["alamat_customer"]?.ToString() ?? "",
                        NamaVerifikator = row["nama_verifikator"]?.ToString() ?? "-"
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengambil data pembayaran: {ex.Message}\n\nPastikan tabel 'pembayarans' sudah dibuat di database.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }

        private ulong GetCurrentUserId()
        {
            try
            {
                Form form = this.FindForm();
                if (form != null && form.Tag is UserModel user)
                {
                    return user.Id;
                }
            }
            catch { }
            return 0;
        }

        // ============================================
        // LOAD DATA
        // ============================================
        private void LoadPembayaran()
        {
            try
            {
                _allPembayaran = GetAllPembayaranFromDatabase();
                if (_allPembayaran == null) _allPembayaran = new List<PembayaranModel>();
                ApplyFilters();
                UpdateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _allPembayaran = new List<PembayaranModel>();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (_allPembayaran == null) _allPembayaran = new List<PembayaranModel>();

            var filtered = new List<PembayaranModel>(_allPembayaran);

            if (!string.IsNullOrEmpty(_currentSearch))
            {
                string s = _currentSearch.ToLower();
                filtered = filtered.FindAll(p =>
                    (p.KodePembayaran?.ToLower().Contains(s) ?? false) ||
                    (p.KodePenyewaan?.ToLower().Contains(s) ?? false) ||
                    (p.NamaCustomer?.ToLower().Contains(s) ?? false) ||
                    (p.NamaVerifikator?.ToLower().Contains(s) ?? false) ||
                    (FormatMetodeLabel(p.MetodePembayaran)?.ToLower().Contains(s) ?? false));
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

            DisplayPembayaran(pageData);
            UpdatePaginationInfo(filtered.Count);
        }

        private void UpdateStats()
        {
            try
            {
                int total = _allPembayaran.Count;
                int menunggu = _allPembayaran.Count(p => p.Status == "pending");
                int lunas = _allPembayaran.Count(p => p.Status == "diverifikasi");
                int gagal = _allPembayaran.Count(p => p.Status == "ditolak");

                lblStat1Value.Text = total.ToString();
                lblStat2Value.Text = menunggu.ToString();
                lblStat3Value.Text = lunas.ToString();
                lblStat4Value.Text = gagal.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error update statistik: {ex.Message}");
            }
        }

        private string FormatMetodeLabel(string metode)
        {
            switch (metode)
            {
                case "cash": return "Tunai (Cash)";
                case "transfer": return "Transfer Bank";
                case "qris": return "QRIS";
                default: return metode;
            }
        }

        private void DisplayPembayaran(List<PembayaranModel> list)
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
                    p.KodePembayaran,
                    p.KodePenyewaan,
                    p.NamaCustomer,
                    FormatMetodeLabel(p.MetodePembayaran),
                    p.TanggalPembayaran != DateTime.MinValue ? p.TanggalPembayaran.ToString("dd/MM/yyyy") : "-",
                    "Rp " + p.Jumlah.ToString("N0"),
                    FormatStatusLabel(p.Status),
                    p.DiverifikasiOleh.HasValue ? p.NamaVerifikator : "-",
                    p.Catatan,
                    ""
                );

                ApplyRowColor(p, rowIndex);
            }
        }

        private void ApplyRowColor(PembayaranModel p, int rowIndex)
        {
            switch (p.Status)
            {
                case "pending":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                    break;
                case "diverifikasi":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(39, 139, 70);
                    break;
                case "ditolak":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                    break;
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "pending": return "Menunggu";
                case "diverifikasi": return "Lunas";
                case "ditolak": return "Gagal";
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
            return "pending";
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            lbTotal.Text = $"Total: {totalFiltered} transaksi";
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

            if (status == "diverifikasi")
            {
                int half = (cellWidth - (gap * 3)) / 2;
                buttons.Add(new ActionButtonSpec { Text = "Detail", Color = Color.FromArgb(52, 152, 219),
                    Bounds = new Rectangle(cellX + 2, cellY + 3, half, buttonHeight) });
                buttons.Add(new ActionButtonSpec { Text = "Bukti", Color = Color.FromArgb(155, 89, 182),
                    Bounds = new Rectangle(cellX + gap + 2 + half, cellY + 3, half, buttonHeight) });
                return buttons;
            }

            if (status == "ditolak")
            {
                buttons.Add(new ActionButtonSpec { Text = "Detail", Color = Color.FromArgb(52, 152, 219),
                    Bounds = new Rectangle(cellX + 2, cellY + 3, cellWidth - 4, buttonHeight) });
                return buttons;
            }

            int part = (cellWidth - (gap * 4)) / 3;
            int x = cellX + 2;
            buttons.Add(new ActionButtonSpec { Text = "Detail", Color = Color.FromArgb(52, 152, 219),
                Bounds = new Rectangle(x, cellY + 3, part, buttonHeight) });
            x += part + gap;
            buttons.Add(new ActionButtonSpec { Text = "Verifikasi", Color = Color.FromArgb(46, 204, 113),
                Bounds = new Rectangle(x, cellY + 3, part, buttonHeight) });
            x += part + gap;
            buttons.Add(new ActionButtonSpec { Text = "Tolak", Color = Color.FromArgb(231, 76, 60),
                Bounds = new Rectangle(x, cellY + 3, part, buttonHeight) });
            return buttons;
        }

        private void Guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                e.PaintBackground(e.CellBounds, true);

                string status = guna2DataGridView1.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "Menunggu";
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
                string displayStatus = guna2DataGridView1.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "Menunggu";
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
                            ShowDetailPembayaran(id);
                        }
                        else if (btn.Text == "Bukti")
                        {
                            BukaBukti(id);
                        }
                        else
                        {
                            ShowVerifikasi(id);
                        }
                        break;
                    }
                }
            }
        }

        // ============================================
        // OPERASI
        // ============================================
        private void ShowDetailPembayaran(ulong id)
        {
            using (var form = new DetailPembayaran(id))
            {
                form.ShowDialog();
            }
        }

        private void ShowVerifikasi(ulong id)
        {
            var pembayaran = _allPembayaran.Find(p => p.Id == id);
            if (pembayaran == null) return;

            using (var form = new VerifikasiPembayaran(pembayaran, GetCurrentUserId()))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadPembayaran();
                }
            }
        }

        private void BukaBukti(ulong id)
        {
            var pembayaran = _allPembayaran.Find(p => p.Id == id);
            if (pembayaran == null) return;

            string path = BuktiPembayaranHelper.ResolvePath(pembayaran.BuktiPembayaran);
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
            {
                MessageBox.Show("File bukti pembayaran tidak ditemukan.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal membuka bukti pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            LoadPembayaran();
        }
    }
}