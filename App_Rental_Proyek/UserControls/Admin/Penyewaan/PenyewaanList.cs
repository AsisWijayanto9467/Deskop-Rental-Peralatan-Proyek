using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using App_Rental_Proyek.UserControls.Admin.Penyewaan;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.Penyewaan
{
    public partial class PenyewaanList : System.Windows.Forms.UserControl
    {
        private List<PenyewaanModel> _allSewa = new List<PenyewaanModel>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 1;
        private string _currentSearch = "";
        private string _currentStatus = ""; // "" = Semua
        private ulong _currentPetugas = 0; // 0 = Semua

        private readonly string[] _statusFilterLabels =
        {
            "Semua", "Menunggu", "Dikonfirmasi", "Menunggu Pembayaran",
            "Disiapkan", "Sedang Disewa", "Selesai", "Ditolak", "Dibatalkan"
        };

        private readonly string[] _statusFilterKeys =
        {
            "", "pending", "disetujui", "menunggu_pembayaran",
            "dibayar", "sedang_disewa", "selesai", "ditolak", "dibatalkan"
        };

        private class ActionButtonSpec
        {
            public string Text;
            public Color Color;
            public Rectangle Bounds;
        }

        public PenyewaanList()
        {
            InitializeComponent();
            InitializeGridView();
            InitializeFilters();
            LoadPenyewaan();
        }

        private void PenyewaanList_Load(object sender, EventArgs e)
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
            guna2DataGridView1.Columns.Add("Kode", "Kode Sewa");
            guna2DataGridView1.Columns.Add("Customer", "Customer");
            guna2DataGridView1.Columns.Add("Pengajuan", "Tgl Pengajuan");
            guna2DataGridView1.Columns.Add("Periode", "Periode Sewa");
            guna2DataGridView1.Columns.Add("Hari", "Hari");
            guna2DataGridView1.Columns.Add("Alat", "Alat");
            guna2DataGridView1.Columns.Add("Total", "Total Biaya");
            guna2DataGridView1.Columns.Add("Status", "Status");
            guna2DataGridView1.Columns.Add("Petugas", "Petugas");

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
            guna2DataGridView1.Columns["Customer"].MinimumWidth = 160;
            guna2DataGridView1.Columns["Periode"].MinimumWidth = 185;
            guna2DataGridView1.Columns["Total"].MinimumWidth = 110;
            guna2DataGridView1.Columns["Status"].MinimumWidth = 120;
            guna2DataGridView1.Columns["Petugas"].MinimumWidth = 110;
            guna2DataGridView1.Columns["Action"].Width = 300;

            guna2DataGridView1.CellPainting += Guna2DataGridView1_CellPainting;
            guna2DataGridView1.CellClick += Guna2DataGridView1_CellClick;
        }

        private void InitializeFilters()
        {
            // Status filter
            cbStatus.Items.Clear();
            foreach (string label in _statusFilterLabels)
            {
                cbStatus.Items.Add(label);
            }
            cbStatus.SelectedIndex = 0;

            // Petugas filter
            LoadPetugasFilter();
        }

        private void LoadPetugasFilter()
        {
            try
            {
                DataTable dt = DatabaseConnection.GetData(
                    "SELECT id, nama FROM users WHERE role IN ('admin','petugas') ORDER BY nama ASC");

                cbPetugas.Items.Clear();
                cbPetugas.Items.Add("Semua");
                foreach (DataRow row in dt.Rows)
                {
                    cbPetugas.Items.Add(row["nama"].ToString());
                }
                cbPetugas.Tag = dt;
                cbPetugas.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                cbPetugas.Items.Clear();
                cbPetugas.Items.Add("Semua");
                cbPetugas.SelectedIndex = 0;
                System.Diagnostics.Debug.WriteLine($"Error load filter petugas: {ex.Message}");
            }
        }

        // ============================================
        // DATABASE OPERATIONS
        // ============================================
        private List<PenyewaanModel> GetAllPenyewaanFromDatabase()
        {
            var list = new List<PenyewaanModel>();
            try
            {
                string query = @"
                    SELECT p.id, p.kode_penyewaan, p.user_id, p.tanggal_pengajuan,
                           p.tanggal_mulai, p.tanggal_selesai, p.total_hari,
                           p.subtotal, p.denda, p.total, p.status, p.catatan,
                           p.processed_by, p.created_at, p.updated_at,
                           u.nama AS nama_customer, u.email AS email_customer,
                           u.no_telepon AS no_telepon_customer, u.alamat AS alamat_customer,
                           pu.nama AS nama_petugas,
                           (SELECT COUNT(*) FROM detail_penyewaans dp WHERE dp.penyewaan_id = p.id) AS jumlah_alat
                    FROM penyewaans p
                    LEFT JOIN users u ON u.id = p.user_id
                    LEFT JOIN users pu ON pu.id = p.processed_by
                    ORDER BY p.created_at DESC";

                DataTable dt = DatabaseConnection.GetData(query);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new PenyewaanModel
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        KodePenyewaan = row["kode_penyewaan"]?.ToString() ?? "",
                        UserId = row["user_id"] != DBNull.Value ? Convert.ToUInt64(row["user_id"]) : 0,
                        TanggalPengajuan = row["tanggal_pengajuan"] != DBNull.Value ? Convert.ToDateTime(row["tanggal_pengajuan"]) : DateTime.MinValue,
                        TanggalMulai = row["tanggal_mulai"] != DBNull.Value ? Convert.ToDateTime(row["tanggal_mulai"]) : DateTime.MinValue,
                        TanggalSelesai = row["tanggal_selesai"] != DBNull.Value ? Convert.ToDateTime(row["tanggal_selesai"]) : DateTime.MinValue,
                        TotalHari = row["total_hari"] != DBNull.Value ? Convert.ToInt32(row["total_hari"]) : 0,
                        Subtotal = row["subtotal"] != DBNull.Value ? Convert.ToDecimal(row["subtotal"]) : 0m,
                        Denda = row["denda"] != DBNull.Value ? Convert.ToDecimal(row["denda"]) : 0m,
                        Total = row["total"] != DBNull.Value ? Convert.ToDecimal(row["total"]) : 0m,
                        Status = row["status"]?.ToString() ?? "pending",
                        Catatan = row["catatan"]?.ToString(),
                        ProcessedBy = row["processed_by"] != DBNull.Value ? Convert.ToUInt64(row["processed_by"]) : (ulong?)null,
                        CreatedAt = row["created_at"] as DateTime?,
                        UpdatedAt = row["updated_at"] as DateTime?,
                        NamaCustomer = row["nama_customer"]?.ToString() ?? "-",
                        EmailCustomer = row["email_customer"]?.ToString() ?? "",
                        NoTeleponCustomer = row["no_telepon_customer"]?.ToString() ?? "",
                        AlamatCustomer = row["alamat_customer"]?.ToString() ?? "",
                        NamaPetugas = row["nama_petugas"]?.ToString() ?? "-",
                        JumlahAlat = row["jumlah_alat"] != DBNull.Value ? Convert.ToInt32(row["jumlah_alat"]) : 0
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengambil data penyewaan: {ex.Message}\n\nPastikan tabel 'penyewaans' dan 'detail_penyewaans' sudah dibuat di database.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }

        private bool UpdateStatusPenyewaan(ulong id, string newStatus)
        {
            try
            {
                ulong processedBy = GetCurrentUserId();
                string query = @"
                    UPDATE penyewaans
                    SET status = @status,
                        processed_by = @processed_by,
                        updated_at = NOW()
                    WHERE id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@status", newStatus),
                    new MySqlParameter("@processed_by", processedBy == 0 ? (object)DBNull.Value : processedBy),
                    new MySqlParameter("@id", id)
                };
                return DatabaseConnection.ExecuteQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengubah status penyewaan: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
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
        private void LoadPenyewaan()
        {
            try
            {
                _allSewa = GetAllPenyewaanFromDatabase();
                if (_allSewa == null) _allSewa = new List<PenyewaanModel>();
                ApplyFilters();
                UpdateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data penyewaan: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _allSewa = new List<PenyewaanModel>();
                ApplyFilters();
            }
        }

        private ulong GetSelectedPetugasId()
        {
            int idx = cbPetugas.SelectedIndex;
            if (idx <= 0) return 0;

            try
            {
                DataTable dt = cbPetugas.Tag as DataTable;
                if (dt != null && idx - 1 < dt.Rows.Count)
                {
                    return Convert.ToUInt64(dt.Rows[idx - 1]["id"]);
                }
            }
            catch { }
            return 0;
        }

        private void ApplyFilters()
        {
            if (_allSewa == null) _allSewa = new List<PenyewaanModel>();

            var filtered = new List<PenyewaanModel>(_allSewa);

            if (!string.IsNullOrEmpty(_currentSearch))
            {
                string s = _currentSearch.ToLower();
                filtered = filtered.FindAll(p =>
                    (p.KodePenyewaan?.ToLower().Contains(s) ?? false) ||
                    (p.NamaCustomer?.ToLower().Contains(s) ?? false) ||
                    (p.NamaPetugas?.ToLower().Contains(s) ?? false));
            }

            if (!string.IsNullOrEmpty(_currentStatus))
            {
                filtered = filtered.FindAll(p => p.Status == _currentStatus);
            }

            if (_currentPetugas > 0)
            {
                filtered = filtered.FindAll(p => p.ProcessedBy == _currentPetugas);
            }

            _totalPages = (int)Math.Ceiling((double)filtered.Count / PageSize);
            if (_totalPages == 0) _totalPages = 1;

            if (_currentPage > _totalPages) _currentPage = _totalPages;

            var pageData = filtered
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            DisplayPenyewaan(pageData);
            UpdatePaginationInfo(filtered.Count);
        }

        private void UpdateStats()
        {
            try
            {
                int total = _allSewa.Count;
                int menunggu = _allSewa.Count(p => p.Status == "pending");
                int aktif = _allSewa.Count(p => p.Status == "sedang_disewa");
                int selesai = _allSewa.Count(p => p.Status == "selesai");

                lblStat1Value.Text = total.ToString();
                lblStat2Value.Text = menunggu.ToString();
                lblStat3Value.Text = aktif.ToString();
                lblStat4Value.Text = selesai.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error update statistik: {ex.Message}");
            }
        }

        private void DisplayPenyewaan(List<PenyewaanModel> list)
        {
            guna2DataGridView1.Rows.Clear();

            if (list == null || list.Count == 0)
            {
                UpdatePaginationInfo(0);
                return;
            }

            foreach (var p in list)
            {
                string periode = "";
                if (p.TanggalMulai != DateTime.MinValue && p.TanggalSelesai != DateTime.MinValue)
                {
                    periode = $"{p.TanggalMulai:dd/MM/yyyy} - {p.TanggalSelesai:dd/MM/yyyy}";
                }

                int rowIndex = guna2DataGridView1.Rows.Add(
                    p.Id,
                    p.KodePenyewaan,
                    p.NamaCustomer,
                    p.TanggalPengajuan != DateTime.MinValue ? p.TanggalPengajuan.ToString("dd/MM/yyyy") : "-",
                    periode,
                    p.TotalHari,
                    p.JumlahAlat,
                    "Rp " + p.Total.ToString("N0"),
                    FormatStatusLabel(p.Status),
                    p.NamaPetugas,
                    ""
                );

                ApplyRowColor(p, rowIndex);
            }
        }

        private void ApplyRowColor(PenyewaanModel p, int rowIndex)
        {
            switch (p.Status)
            {
                case "sedang_disewa":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(230, 126, 34);
                    break;
                case "selesai":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(39, 139, 70);
                    break;
                case "dibatalkan":
                case "ditolak":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                    break;
                case "menunggu_pembayaran":
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                    break;
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "pending": return "Menunggu";
                case "disetujui": return "Dikonfirmasi";
                case "menunggu_pembayaran": return "Menunggu Pembayaran";
                case "dibayar": return "Disiapkan";
                case "sedang_disewa": return "Sedang Disewa";
                case "selesai": return "Selesai";
                case "ditolak": return "Ditolak";
                case "dibatalkan": return "Dibatalkan";
                default: return status;
            }
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            lbTotal.Text = $"Total: {totalFiltered} sewa";
            lbHalaman.Text = $"Halaman {_currentPage} dari {_totalPages}";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        // ============================================
        // ACTION BUTTONS
        // ============================================
        private bool IsTerminalStatus(string status)
        {
            return status == "selesai" || status == "ditolak" || status == "dibatalkan";
        }

        private string GetProsesText(string status)
        {
            switch (status)
            {
                case "pending": return "Konfirmasi";
                case "disetujui": return "Proses";
                case "menunggu_pembayaran": return "Bayar";
                case "dibayar": return "Mulai Sewa";
                case "sedang_disewa": return "Kembalikan";
                default: return "Proses";
            }
        }

        private string GetNextStatus(string currentStatus)
        {
            switch (currentStatus)
            {
                case "pending": return "disetujui";
                case "disetujui": return "menunggu_pembayaran";
                case "menunggu_pembayaran": return "dibayar";
                case "dibayar": return "sedang_disewa";
                case "sedang_disewa": return "selesai";
                default: return currentStatus;
            }
        }

        private List<ActionButtonSpec> GetActionButtons(string status, int cellWidth, int cellX, int cellY)
        {
            var buttons = new List<ActionButtonSpec>();
            int buttonHeight = 0;
            int gap = 3;

            ActionButtonSpec MakeBtn(string text, Color color, Rectangle rect)
            {
                return new ActionButtonSpec { Text = text, Color = color, Bounds = rect };
            }

            if (IsTerminalStatus(status))
            {
                buttonHeight = 30;
                buttons.Add(MakeBtn("Detail", Color.FromArgb(52, 152, 219),
                    new Rectangle(cellX + 2, cellY + 3, cellWidth - 4, buttonHeight)));
                return buttons;
            }

            if (status == "sedang_disewa")
            {
                buttonHeight = 30;
                int half = (cellWidth - (gap * 3)) / 2;
                buttons.Add(MakeBtn("Detail", Color.FromArgb(52, 152, 219),
                    new Rectangle(cellX + 2, cellY + 3, half, buttonHeight)));
                buttons.Add(MakeBtn("Kembalikan", Color.FromArgb(46, 204, 113),
                    new Rectangle(cellX + gap + 2 + half, cellY + 3, half, buttonHeight)));
                return buttons;
            }

            buttonHeight = 30;
            int part = (cellWidth - (gap * 4)) / 3;
            int x = cellX + 2;
            buttons.Add(MakeBtn("Detail", Color.FromArgb(52, 152, 219),
                new Rectangle(x, cellY + 3, part, buttonHeight)));
            x += part + gap;
            buttons.Add(MakeBtn(GetProsesText(status), Color.FromArgb(46, 204, 113),
                new Rectangle(x, cellY + 3, part, buttonHeight)));
            x += part + gap;
            buttons.Add(MakeBtn("Batalkan", Color.FromArgb(231, 76, 60),
                new Rectangle(x, cellY + 3, part, buttonHeight)));
            return buttons;
        }

        private void Guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                e.PaintBackground(e.CellBounds, true);

                string status = guna2DataGridView1.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "pending";
                // Raw status lookup: status column holds display text (e.g. "Menunggu").
                string rawStatus = MapDisplayToRawStatus(status);

                var buttons = GetActionButtons(rawStatus, e.CellBounds.Width, e.CellBounds.X, e.CellBounds.Y);

                foreach (var btn in buttons)
                {
                    DrawCellButton(e, btn.Bounds, btn.Text, btn.Color);
                }

                e.Handled = true;
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
                            ShowDetailPenyewaan(id);
                        }
                        else if (btn.Text == "Batalkan")
                        {
                            BatalkanPenyewaan(id, rawStatus);
                        }
                        else
                        {
                            ProsesPenyewaan(id, rawStatus);
                        }
                        break;
                    }
                }
            }
        }

        // ============================================
        // OPERASI
        // ============================================
        private void ShowDetailPenyewaan(ulong id)
        {
            using (var form = new DetailPenyewaan(id))
            {
                form.ShowDialog();
            }
        }

        private void ProsesPenyewaan(ulong id, string currentStatus)
        {
            var sewa = _allSewa.Find(p => p.Id == id);
            if (sewa == null) return;

            string nextStatus = GetNextStatus(currentStatus);
            if (nextStatus == currentStatus)
            {
                MessageBox.Show("Status penyewaan sudah berada pada tahap akhir.",
                    "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string konfirmasi = currentStatus switch
            {
                "pending" => $"Konfirmasi penyewaan '{sewa.KodePenyewaan}' dari {sewa.NamaCustomer}?\n\nStatus akan berubah menjadi 'Dikonfirmasi'.",
                "disetujui" => $"Proses penyewaan '{sewa.KodePenyewaan}'?\n\nStatus akan berubah menjadi 'Menunggu Pembayaran'.",
                "menunggu_pembayaran" => $"Konfirmasikan pembayaran untuk '{sewa.KodePenyewaan}'?\n\nStatus akan berubah menjadi 'Disiapkan'.",
                "dibayar" => $"Mulai sewa '{sewa.KodePenyewaan}'?\n\nStatus akan berubah menjadi 'Sedang Disewa'.",
                "sedang_disewa" => $"Konfirmasi pengembalian alat untuk '{sewa.KodePenyewaan}'?\n\nStatus akan berubah menjadi 'Selesai'.",
                _ => $"Lanjutkan proses penyewaan '{sewa.KodePenyewaan}'?"
            };

            DialogResult result = MessageBox.Show(konfirmasi, "Konfirmasi Proses",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (UpdateStatusPenyewaan(id, nextStatus))
                {
                    MessageBox.Show("Status penyewaan berhasil diperbarui!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPenyewaan();
                }
                else
                {
                    MessageBox.Show("Gagal memperbarui status penyewaan.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BatalkanPenyewaan(ulong id, string currentStatus)
        {
            var sewa = _allSewa.Find(p => p.Id == id);
            if (sewa == null) return;

            if (currentStatus == "selesai")
            {
                MessageBox.Show("Penyewaan yang sudah selesai tidak dapat dibatalkan.",
                    "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Apakah Anda yakin ingin membatalkan penyewaan '{sewa.KodePenyewaan}'?\n\n" +
                "Penyewaan yang dibatalkan tidak dapat diproses kembali.",
                "Konfirmasi Pembatalan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                if (UpdateStatusPenyewaan(id, "dibatalkan"))
                {
                    MessageBox.Show("Penyewaan berhasil dibatalkan!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPenyewaan();
                }
                else
                {
                    MessageBox.Show("Gagal membatalkan penyewaan.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void cbPetugas_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentPetugas = GetSelectedPetugasId();
            _currentPage = 1;
            ApplyFilters();
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
            LoadPenyewaan();
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            using (var form = new CreatePenyewaan())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadPenyewaan();
                }
            }
        }
    }
}