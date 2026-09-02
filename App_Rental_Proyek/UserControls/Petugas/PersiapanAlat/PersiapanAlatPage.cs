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

namespace App_Rental_Proyek.UserControls.Petugas.PersiapanAlat
{
    public partial class PersiapanAlatPage : System.Windows.Forms.UserControl
    {
        private List<PenyewaanModel> _allPersiapan = new List<PenyewaanModel>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 1;
        private string _currentSearch = "";

        private class ActionButtonSpec
        {
            public string Text;
            public Color Color;
            public Rectangle Bounds;
        }

        public PersiapanAlatPage()
        {
            InitializeComponent();
            InitializeGridView();
            LoadPersiapan();
        }

        private void PersiapanAlatPage_Load(object sender, EventArgs e)
        {
        }

        private void InitializeGridView()
        {
            guna2DataGridView1.Columns.Clear();

            guna2DataGridView1.Columns.Add("Id", "ID");
            guna2DataGridView1.Columns.Add("Kode", "Kode Sewa");
            guna2DataGridView1.Columns.Add("Customer", "Customer");
            guna2DataGridView1.Columns.Add("TanggalMulai", "Tanggal Mulai");
            guna2DataGridView1.Columns.Add("TanggalSelesai", "Tanggal Selesai");
            guna2DataGridView1.Columns.Add("TotalHari", "Total Hari");
            guna2DataGridView1.Columns.Add("JumlahAlat", "Jumlah Alat");
            guna2DataGridView1.Columns.Add("Total", "Total Biaya");
            guna2DataGridView1.Columns.Add("Status", "Status");

            DataGridViewColumn colAction = new DataGridViewColumn();
            colAction.Name = "Action";
            colAction.HeaderText = "Aksi";
            colAction.CellTemplate = new DataGridViewTextBoxCell();
            colAction.Width = 250;
            colAction.MinimumWidth = 250;
            guna2DataGridView1.Columns.Add(colAction);

            guna2DataGridView1.Columns["Id"].Visible = false;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;

            guna2DataGridView1.Columns["Kode"].MinimumWidth = 120;
            guna2DataGridView1.Columns["Customer"].MinimumWidth = 160;
            guna2DataGridView1.Columns["JumlahAlat"].MinimumWidth = 80;
            guna2DataGridView1.Columns["Total"].MinimumWidth = 110;
            guna2DataGridView1.Columns["Status"].MinimumWidth = 100;
            guna2DataGridView1.Columns["Action"].Width = 250;

            guna2DataGridView1.CellPainting += Guna2DataGridView1_CellPainting;
            guna2DataGridView1.CellClick += Guna2DataGridView1_CellClick;
        }

        private List<PenyewaanModel> GetAllPersiapanFromDatabase()
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
                           (SELECT COUNT(*) FROM detail_penyewaans dp WHERE dp.penyewaan_id = p.id) AS jumlah_alat
                    FROM penyewaans p
                    LEFT JOIN users u ON u.id = p.user_id
                    WHERE p.status = 'dibayar'
                    ORDER BY p.tanggal_mulai ASC, p.created_at DESC";

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
                        Status = row["status"]?.ToString() ?? "dibayar",
                        Catatan = row["catatan"]?.ToString(),
                        ProcessedBy = row["processed_by"] != DBNull.Value ? Convert.ToUInt64(row["processed_by"]) : (ulong?)null,
                        CreatedAt = row["created_at"] as DateTime?,
                        UpdatedAt = row["updated_at"] as DateTime?,
                        NamaCustomer = row["nama_customer"]?.ToString() ?? "-",
                        EmailCustomer = row["email_customer"]?.ToString() ?? "",
                        NoTeleponCustomer = row["no_telepon_customer"]?.ToString() ?? "",
                        AlamatCustomer = row["alamat_customer"]?.ToString() ?? "",
                        JumlahAlat = row["jumlah_alat"] != DBNull.Value ? Convert.ToInt32(row["jumlah_alat"]) : 0
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengambil data persiapan alat: {ex.Message}\n\nPastikan tabel 'penyewaans' sudah dibuat di database.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }

        private void LogActivity(string aktivitas, string modul, ulong? referensiId = null)
        {
            ActivityLogHelper.LogForSession(SessionManager.GetCurrentUserId(), aktivitas, modul, referensiId);
        }

        private void LoadPersiapan()
        {
            try
            {
                _allPersiapan = GetAllPersiapanFromDatabase();
                if (_allPersiapan == null) _allPersiapan = new List<PenyewaanModel>();
                ApplyFilters();
                UpdateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data persiapan alat: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _allPersiapan = new List<PenyewaanModel>();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (_allPersiapan == null) _allPersiapan = new List<PenyewaanModel>();

            var filtered = new List<PenyewaanModel>(_allPersiapan);

            if (!string.IsNullOrEmpty(_currentSearch))
            {
                string s = _currentSearch.ToLower();
                filtered = filtered.FindAll(p =>
                    (p.KodePenyewaan?.ToLower().Contains(s) ?? false) ||
                    (p.NamaCustomer?.ToLower().Contains(s) ?? false));
            }

            _totalPages = (int)Math.Ceiling((double)filtered.Count / PageSize);
            if (_totalPages == 0) _totalPages = 1;

            if (_currentPage > _totalPages) _currentPage = _totalPages;

            var pageData = filtered
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            DisplayPersiapan(pageData);
            UpdatePaginationInfo(filtered.Count);
        }

        private void UpdateStats()
        {
            try
            {
                int total = _allPersiapan.Count;
                int hari_ini = _allPersiapan.Count(p => p.TanggalMulai.Date == DateTime.Today);
                int besok = _allPersiapan.Count(p => p.TanggalMulai.Date == DateTime.Today.AddDays(1));
                int minggu_ini = _allPersiapan.Count(p => p.TanggalMulai.Date >= DateTime.Today && p.TanggalMulai.Date <= DateTime.Today.AddDays(7));

                lblStat1Value.Text = total.ToString();
                lblStat2Value.Text = hari_ini.ToString();
                lblStat3Value.Text = besok.ToString();
                lblStat4Value.Text = minggu_ini.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error update statistik: {ex.Message}");
            }
        }

        private void DisplayPersiapan(List<PenyewaanModel> list)
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
                    p.KodePenyewaan,
                    p.NamaCustomer,
                    p.TanggalMulai != DateTime.MinValue ? p.TanggalMulai.ToString("dd/MM/yyyy") : "-",
                    p.TanggalSelesai != DateTime.MinValue ? p.TanggalSelesai.ToString("dd/MM/yyyy") : "-",
                    p.TotalHari,
                    p.JumlahAlat,
                    "Rp " + p.Total.ToString("N0"),
                    FormatStatusLabel(p.Status),
                    ""
                );

                if (p.TanggalMulai.Date == DateTime.Today)
                {
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 250, 205);
                }
                else if (p.TanggalMulai.Date == DateTime.Today.AddDays(1))
                {
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                }
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "dibayar": return "Siap Disiapkan";
                case "sedang_disewa": return "Sedang Disewa";
                default: return status;
            }
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            lbTotal.Text = $"Total: {totalFiltered} persiapan";
            lbHalaman.Text = $"Halaman {_currentPage} dari {_totalPages}";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        private List<ActionButtonSpec> GetActionButtons(int cellWidth, int cellX, int cellY)
        {
            var buttons = new List<ActionButtonSpec>();
            int buttonHeight = 30;
            int gap = 3;

            ActionButtonSpec MakeBtn(string text, Color color, Rectangle rect)
            {
                return new ActionButtonSpec { Text = text, Color = color, Bounds = rect };
            }

            int part = (cellWidth - (gap * 4)) / 3;
            int x = cellX + 2;
            buttons.Add(MakeBtn("Detail", Color.FromArgb(52, 152, 219),
                new Rectangle(x, cellY + 3, part, buttonHeight)));
            x += part + gap;
            buttons.Add(MakeBtn("Alat", Color.FromArgb(155, 89, 182),
                new Rectangle(x, cellY + 3, part, buttonHeight)));
            x += part + gap;
            buttons.Add(MakeBtn("Siap Disewa", Color.FromArgb(46, 204, 113),
                new Rectangle(x, cellY + 3, part, buttonHeight)));

            return buttons;
        }

        private void Guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                e.PaintBackground(e.CellBounds, true);

                var buttons = GetActionButtons(e.CellBounds.Width, e.CellBounds.X, e.CellBounds.Y);

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

                Rectangle cellRect = guna2DataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                Point clickPoint = guna2DataGridView1.PointToClient(Control.MousePosition);
                int clickX = clickPoint.X - cellRect.X;

                var buttons = GetActionButtons(cellRect.Width, 0, 0);

                foreach (var btn in buttons)
                {
                    if (clickX >= btn.Bounds.X && clickX <= btn.Bounds.Right)
                    {
                        if (btn.Text == "Detail")
                        {
                            ShowDetail(id);
                        }
                        else if (btn.Text == "Alat")
                        {
                            ShowDaftarAlat(id);
                        }
                        else if (btn.Text == "Siap Disewa")
                        {
                            ProsesKeDisewa(id);
                        }
                        break;
                    }
                }
            }
        }

        private void ShowDetail(ulong id)
        {
            var sewa = _allPersiapan.Find(p => p.Id == id);
            if (sewa == null) return;

            using (var form = new DetailPersiapan(id))
            {
                form.ShowDialog(this);
            }

            LogActivity($"Melihat detail persiapan alat untuk penyewaan '{sewa.KodePenyewaan}'",
                "Persiapan Alat", id);
        }

        private void ShowDaftarAlat(ulong id)
        {
            var sewa = _allPersiapan.Find(p => p.Id == id);
            if (sewa == null) return;

            using (var form = new DaftarAlatPersiapan(id))
            {
                form.ShowDialog(this);
            }

            LogActivity($"Melihat daftar alat untuk penyewaan '{sewa.KodePenyewaan}'",
                "Persiapan Alat", id);
        }

        private void ProsesKeDisewa(ulong id)
        {
            var sewa = _allPersiapan.Find(p => p.Id == id);
            if (sewa == null) return;

            DialogResult result = MessageBox.Show(
                $"Apakah alat untuk penyewaan '{sewa.KodePenyewaan}' sudah siap diserahkan?\n\n" +
                "Status akan berubah menjadi 'Sedang Disewa'.",
                "Konfirmasi Serah Terima",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    ulong processedBy = SessionManager.GetCurrentUserId();

                    string query = @"
                        UPDATE penyewaans
                        SET status = 'sedang_disewa',
                            processed_by = @processed_by,
                            updated_at = NOW()
                        WHERE id = @id";

                    MySqlParameter[] parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@processed_by", processedBy == 0 ? (object)DBNull.Value : processedBy),
                        new MySqlParameter("@id", id)
                    };

                    bool success = DatabaseConnection.ExecuteQuery(query, parameters) > 0;

                    if (success)
                    {
                        LogActivity($"Menyerahkan alat untuk penyewaan '{sewa.KodePenyewaan}' - Status berubah menjadi Sedang Disewa",
                            "Persiapan Alat", id);

                        MessageBox.Show("Alat berhasil diserahkan!\nStatus penyewaan berubah menjadi 'Sedang Disewa'.",
                            "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadPersiapan();
                    }
                    else
                    {
                        MessageBox.Show("Gagal memperbarui status penyewaan.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error memproses serah terima: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _currentSearch = txtSearch.Text.Trim();
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
            LoadPersiapan();
        }
    }
}
