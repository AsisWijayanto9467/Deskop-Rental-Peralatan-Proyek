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

namespace App_Rental_Proyek.UserControls.Petugas.Pembayaran
{
    public partial class PembayaranPage : System.Windows.Forms.UserControl
    {
        private List<PembayaranModel> _allPembayaran = new List<PembayaranModel>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 1;
        private string _currentSearch = "";
        private string _currentStatus = "";

        private readonly string[] _statusFilterLabels = { "Semua", "Pending", "Diverifikasi", "Ditolak" };
        private readonly string[] _statusFilterKeys = { "", "pending", "diverifikasi", "ditolak" };

        private class ActionButtonSpec
        {
            public string Text;
            public Color Color;
            public Rectangle Bounds;
        }

        public PembayaranPage()
        {
            InitializeComponent();
            InitializeGridView();
            InitializeFilters();
            LoadPembayaran();
        }

        private void PembayaranPage_Load(object sender, EventArgs e)
        {
        }

        private void InitializeGridView()
        {
            guna2DataGridView1.Columns.Clear();

            guna2DataGridView1.Columns.Add("Id", "ID");
            guna2DataGridView1.Columns.Add("KodePembayaran", "Kode Pembayaran");
            guna2DataGridView1.Columns.Add("KodePenyewaan", "Nomor Penyewaan");
            guna2DataGridView1.Columns.Add("Customer", "Customer");
            guna2DataGridView1.Columns.Add("TanggalPembayaran", "Tgl Pembayaran");
            guna2DataGridView1.Columns.Add("Jumlah", "Jumlah");
            guna2DataGridView1.Columns.Add("Metode", "Metode");
            guna2DataGridView1.Columns.Add("Status", "Status");
            guna2DataGridView1.Columns.Add("Verifikator", "Verifikator");

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

            guna2DataGridView1.Columns["KodePembayaran"].MinimumWidth = 120;
            guna2DataGridView1.Columns["KodePenyewaan"].MinimumWidth = 120;
            guna2DataGridView1.Columns["Customer"].MinimumWidth = 160;
            guna2DataGridView1.Columns["Jumlah"].MinimumWidth = 110;
            guna2DataGridView1.Columns["Status"].MinimumWidth = 110;
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

        private List<PembayaranModel> GetAllPembayaranFromDatabase()
        {
            var list = new List<PembayaranModel>();
            try
            {
                string query = @"
                    SELECT pb.id, pb.penyewaan_id, pb.kode_pembayaran, pb.tanggal_pembayaran,
                           pb.jumlah, pb.metode_pembayaran, pb.bukti_pembayaran, pb.status,
                           pb.diverifikasi_oleh, pb.tanggal_verifikasi, pb.catatan,
                           pb.created_at, pb.updated_at,
                           ps.kode_penyewaan, ps.total AS total_sewa, ps.status AS status_penyewaan,
                           u.nama AS nama_customer, u.email AS email_customer,
                           u.no_telepon AS no_telepon_customer, u.alamat AS alamat_customer,
                           v.nama AS nama_verifikator
                    FROM pembayarans pb
                    LEFT JOIN penyewaans ps ON ps.id = pb.penyewaan_id
                    LEFT JOIN users u ON u.id = ps.user_id
                    LEFT JOIN users v ON v.id = pb.diverifikasi_oleh
                    ORDER BY pb.created_at DESC";

                DataTable dt = DatabaseConnection.GetData(query);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new PembayaranModel
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        PenyewaanId = Convert.ToUInt64(row["penyewaan_id"]),
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
                        KodePenyewaan = row["kode_penyewaan"]?.ToString() ?? "",
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

        private void LogActivity(string aktivitas, string modul, ulong? referensiId = null)
        {
            ActivityLogHelper.LogForSession(SessionManager.GetCurrentUserId(), aktivitas, modul, referensiId);
        }

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

            DisplayPembayaran(pageData);
            UpdatePaginationInfo(filtered.Count);
        }

        private void UpdateStats()
        {
            try
            {
                int total = _allPembayaran.Count;
                int pending = _allPembayaran.Count(p => p.Status == "pending");
                int diverifikasi = _allPembayaran.Count(p => p.Status == "diverifikasi");
                int ditolak = _allPembayaran.Count(p => p.Status == "ditolak");

                lblStat1Value.Text = total.ToString();
                lblStat2Value.Text = pending.ToString();
                lblStat3Value.Text = diverifikasi.ToString();
                lblStat4Value.Text = ditolak.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error update statistik: {ex.Message}");
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
                    p.TanggalPembayaran != DateTime.MinValue ? p.TanggalPembayaran.ToString("dd/MM/yyyy") : "-",
                    "Rp " + p.Jumlah.ToString("N0"),
                    FormatMetodePembayaran(p.MetodePembayaran),
                    FormatStatusLabel(p.Status),
                    p.NamaVerifikator,
                    ""
                );

                ApplyRowColor(p, rowIndex);
            }
        }

        private void ApplyRowColor(PembayaranModel p, int rowIndex)
        {
            switch (p.Status)
            {
                case "ditolak":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                    break;
                case "diverifikasi":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                    break;
                case "pending":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                    break;
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "pending": return "Pending";
                case "diverifikasi": return "Diverifikasi";
                case "ditolak": return "Ditolak";
                default: return status;
            }
        }

        private string FormatMetodePembayaran(string metode)
        {
            switch (metode)
            {
                case "cash": return "Cash";
                case "transfer": return "Transfer";
                case "qris": return "QRIS";
                default: return metode;
            }
        }

        private string MapDisplayToRawStatus(string display)
        {
            switch (display)
            {
                case "Pending": return "pending";
                case "Diverifikasi": return "diverifikasi";
                case "Ditolak": return "ditolak";
                default: return "pending";
            }
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            lbTotal.Text = $"Total: {totalFiltered} pembayaran";
            lbHalaman.Text = $"Halaman {_currentPage} dari {_totalPages}";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        private List<ActionButtonSpec> GetActionButtons(string status, int cellWidth, int cellX, int cellY)
        {
            var buttons = new List<ActionButtonSpec>();
            int buttonHeight = 30;
            int gap = 3;

            ActionButtonSpec MakeBtn(string text, Color color, Rectangle rect)
            {
                return new ActionButtonSpec { Text = text, Color = color, Bounds = rect };
            }

            if (status == "pending")
            {
                int part = (cellWidth - (gap * 5)) / 4;
                int x = cellX + 2;
                buttons.Add(MakeBtn("Detail", Color.FromArgb(52, 152, 219),
                    new Rectangle(x, cellY + 3, part, buttonHeight)));
                x += part + gap;
                buttons.Add(MakeBtn("Bukti", Color.FromArgb(155, 89, 182),
                    new Rectangle(x, cellY + 3, part, buttonHeight)));
                x += part + gap;
                buttons.Add(MakeBtn("Verifikasi", Color.FromArgb(46, 204, 113),
                    new Rectangle(x, cellY + 3, part, buttonHeight)));
                x += part + gap;
                buttons.Add(MakeBtn("Tolak", Color.FromArgb(231, 76, 60),
                    new Rectangle(x, cellY + 3, part, buttonHeight)));
                return buttons;
            }

            int half = (cellWidth - (gap * 3)) / 2;
            buttons.Add(MakeBtn("Detail", Color.FromArgb(52, 152, 219),
                new Rectangle(cellX + 2, cellY + 3, half, buttonHeight)));
            buttons.Add(MakeBtn("Bukti", Color.FromArgb(155, 89, 182),
                new Rectangle(cellX + gap + 2 + half, cellY + 3, half, buttonHeight)));
            return buttons;
        }

        private void Guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                e.PaintBackground(e.CellBounds, true);

                string status = guna2DataGridView1.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "Pending";
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
                string displayStatus = guna2DataGridView1.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "Pending";
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
                        else if (btn.Text == "Bukti")
                        {
                            PreviewBukti(id);
                        }
                        else if (btn.Text == "Verifikasi")
                        {
                            OpenVerifikasi(id);
                        }
                        else if (btn.Text == "Tolak")
                        {
                            OpenTolak(id);
                        }
                        break;
                    }
                }
            }
        }

        private void ShowDetail(ulong id)
        {
            var pembayaran = _allPembayaran.Find(p => p.Id == id);
            if (pembayaran == null) return;

            using (var form = new DetailPembayaran(id))
            {
                form.ShowDialog(this);
            }

            LogActivity($"Melihat detail pembayaran '{pembayaran.KodePembayaran}' dari {pembayaran.NamaCustomer}",
                "Pembayaran", id);
        }

        private void PreviewBukti(ulong id)
        {
            var pembayaran = _allPembayaran.Find(p => p.Id == id);
            if (pembayaran == null) return;

            if (string.IsNullOrEmpty(pembayaran.BuktiPembayaran))
            {
                MessageBox.Show("Bukti pembayaran tidak tersedia.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string buktiPath = System.IO.Path.Combine(
                    Application.StartupPath,
                    "Resources", "BuktiPembayaran",
                    pembayaran.BuktiPembayaran);

                if (System.IO.File.Exists(buktiPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = buktiPath,
                        UseShellExecute = true
                    });

                    LogActivity($"Melihat bukti pembayaran '{pembayaran.KodePembayaran}'",
                        "Pembayaran", id);
                }
                else
                {
                    MessageBox.Show($"File bukti pembayaran tidak ditemukan:\n{buktiPath}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error membuka bukti pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenVerifikasi(ulong id)
        {
            var pembayaran = _allPembayaran.Find(p => p.Id == id);
            if (pembayaran == null) return;

            using (var form = new VerifikasiPembayaran(id))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadPembayaran();
                }
            }
        }

        private void OpenTolak(ulong id)
        {
            var pembayaran = _allPembayaran.Find(p => p.Id == id);
            if (pembayaran == null) return;

            using (var form = new TolakPembayaran(id))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadPembayaran();
                }
            }
        }

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
