using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using App_Rental_Proyek.UserControls.Admin.AlatProyek;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin
{
    public partial class AlatProyekList : System.Windows.Forms.UserControl
    {
        private List<AlatProyekModel> _allAlat = new List<AlatProyekModel>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 1;
        private string _currentSearch = "";
        private ulong _currentKategori = 0; // 0 = Semua
        private string _currentStatus = "Semua";

        public AlatProyekList()
        {
            InitializeComponent();
            InitializeGridView();
            InitializeFilters();
            LoadAlat();
        }

        private void AlatProyekList_Load(object sender, EventArgs e)
        {
        }

        // ============================================
        // INISIALISASI
        // ============================================
        private void InitializeGridView()
        {
            guna2DataGridView1.Columns.Clear();

            guna2DataGridView1.Columns.Add("Id", "ID");
            guna2DataGridView1.Columns.Add("Kode", "Kode Alat");
            guna2DataGridView1.Columns.Add("Nama", "Nama Alat");
            guna2DataGridView1.Columns.Add("Kategori", "Kategori");
            guna2DataGridView1.Columns.Add("Lokasi", "Lokasi");
            guna2DataGridView1.Columns.Add("Harga", "Harga Sewa");
            guna2DataGridView1.Columns.Add("Stok", "Stok");
            guna2DataGridView1.Columns.Add("StokTersedia", "Tersedia");
            guna2DataGridView1.Columns.Add("Kondisi", "Kondisi");
            guna2DataGridView1.Columns.Add("Status", "Status");

            DataGridViewColumn colAction = new DataGridViewColumn();
            colAction.Name = "Action";
            colAction.HeaderText = "Aksi";
            colAction.CellTemplate = new DataGridViewTextBoxCell();
            colAction.Width = 330;
            colAction.MinimumWidth = 330;
            guna2DataGridView1.Columns.Add(colAction);

            guna2DataGridView1.Columns["Id"].Visible = false;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;

            guna2DataGridView1.Columns["Kode"].MinimumWidth = 110;
            guna2DataGridView1.Columns["Nama"].MinimumWidth = 150;
            guna2DataGridView1.Columns["Kategori"].MinimumWidth = 100;
            guna2DataGridView1.Columns["Lokasi"].MinimumWidth = 120;
            guna2DataGridView1.Columns["Action"].Width = 330;

            guna2DataGridView1.CellPainting += Guna2DataGridView1_CellPainting;
            guna2DataGridView1.CellClick += Guna2DataGridView1_CellClick;
            guna2DataGridView1.CellContentClick -= guna2DataGridView1_CellContentClick;
        }

        private void InitializeFilters()
        {
            // Status filter
            cbStatus.Items.Clear();
            cbStatus.Items.Add("Semua");
            cbStatus.Items.Add("tersedia");
            cbStatus.Items.Add("disewa");
            cbStatus.Items.Add("maintenance");
            cbStatus.Items.Add("tidak_aktif");
            cbStatus.SelectedIndex = 0;

            // Kategori filter
            LoadKategoriFilter();
        }

        private void LoadKategoriFilter()
        {
            try
            {
                DataTable dt = DatabaseConnection.GetData(
                    "SELECT id, nama_kategori FROM kategoris ORDER BY nama_kategori ASC");

                cbKategori.Items.Clear();
                cbKategori.Items.Add("Semua");
                foreach (DataRow row in dt.Rows)
                {
                    cbKategori.Items.Add(row["nama_kategori"].ToString());
                }

                // Store id by index in tag
                cbKategori.Tag = dt;
                cbKategori.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                cbKategori.Items.Clear();
                cbKategori.Items.Add("Semua");
                cbKategori.SelectedIndex = 0;
                System.Diagnostics.Debug.WriteLine($"Error load kategori filter: {ex.Message}");
            }
        }

        // ============================================
        // DATABASE OPERATIONS
        // ============================================
        private List<AlatProyekModel> GetAllAlatFromDatabase()
        {
            var list = new List<AlatProyekModel>();
            try
            {
                string query = @"
                    SELECT a.id, a.kategori_id, a.lokasi_id, a.kode_alat, a.nama_alat, a.deskripsi,
                           a.harga_sewa_harian, a.stok, a.stok_tersedia, a.kondisi, a.status,
                           a.gambar, a.created_at, a.updated_at,
                           k.nama_kategori, l.nama_lokasi
                    FROM alat_proyeks a
                    LEFT JOIN kategoris k ON k.id = a.kategori_id
                    LEFT JOIN lokasis l ON l.id = a.lokasi_id
                    ORDER BY a.created_at DESC";

                DataTable dt = DatabaseConnection.GetData(query);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new AlatProyekModel
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        KategoriId = row["kategori_id"] != DBNull.Value ? Convert.ToUInt64(row["kategori_id"]) : 0,
                        LokasiId = row["lokasi_id"] != DBNull.Value ? Convert.ToUInt64(row["lokasi_id"]) : 0,
                        KodeAlat = row["kode_alat"]?.ToString() ?? "",
                        NamaAlat = row["nama_alat"]?.ToString() ?? "",
                        Deskripsi = row["deskripsi"]?.ToString(),
                        HargaSewaHarian = row["harga_sewa_harian"] != DBNull.Value ? Convert.ToDecimal(row["harga_sewa_harian"]) : 0m,
                        Stok = row["stok"] != DBNull.Value ? Convert.ToInt32(row["stok"]) : 0,
                        StokTersedia = row["stok_tersedia"] != DBNull.Value ? Convert.ToInt32(row["stok_tersedia"]) : 0,
                        Kondisi = row["kondisi"]?.ToString() ?? "baik",
                        Status = row["status"]?.ToString() ?? "tersedia",
                        Gambar = row["gambar"]?.ToString(),
                        CreatedAt = row["created_at"] as DateTime?,
                        UpdatedAt = row["updated_at"] as DateTime?,
                        NamaKategori = row["nama_kategori"]?.ToString(),
                        NamaLokasi = row["nama_lokasi"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengambil data alat: {ex.Message}\n\nPastikan tabel 'alat_proyeks', 'kategoris', dan 'lokasis' sudah dibuat di database.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }

        private bool ToggleStatusAlat(ulong id, string newStatus)
        {
            try
            {
                string query = "UPDATE alat_proyeks SET status = @status, updated_at = NOW() WHERE id = @id";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@status", newStatus),
                    new MySqlParameter("@id", id)
                };
                return DatabaseConnection.ExecuteQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengubah status alat: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool DeleteAlat(ulong id)
        {
            try
            {
                string query = "DELETE FROM alat_proyeks WHERE id = @id";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", id)
                };
                return DatabaseConnection.ExecuteQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error menghapus alat: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ============================================
        // LOAD DATA
        // ============================================
        private void LoadAlat()
        {
            try
            {
                _allAlat = GetAllAlatFromDatabase();
                if (_allAlat == null) _allAlat = new List<AlatProyekModel>();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data alat: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _allAlat = new List<AlatProyekModel>();
                ApplyFilters();
            }
        }

        private ulong GetSelectedKategoriId()
        {
            int idx = cbKategori.SelectedIndex;
            if (idx <= 0) return 0;

            try
            {
                DataTable dt = cbKategori.Tag as DataTable;
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
            if (_allAlat == null) _allAlat = new List<AlatProyekModel>();

            var filtered = new List<AlatProyekModel>(_allAlat);

            if (!string.IsNullOrEmpty(_currentSearch))
            {
                string s = _currentSearch.ToLower();
                filtered = filtered.FindAll(a =>
                    (a.KodeAlat?.ToLower().Contains(s) ?? false) ||
                    (a.NamaAlat?.ToLower().Contains(s) ?? false) ||
                    (a.NamaKategori?.ToLower().Contains(s) ?? false) ||
                    (a.NamaLokasi?.ToLower().Contains(s) ?? false));
            }

            if (_currentKategori > 0)
            {
                filtered = filtered.FindAll(a => a.KategoriId == _currentKategori);
            }

            if (_currentStatus != "Semua")
            {
                filtered = filtered.FindAll(a => a.Status == _currentStatus);
            }

            _totalPages = (int)Math.Ceiling((double)filtered.Count / PageSize);
            if (_totalPages == 0) _totalPages = 1;

            if (_currentPage > _totalPages) _currentPage = _totalPages;

            var pageData = filtered
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            DisplayAlat(pageData);
            UpdatePaginationInfo(filtered.Count);
        }

        private void DisplayAlat(List<AlatProyekModel> list)
        {
            guna2DataGridView1.Rows.Clear();

            if (list == null || list.Count == 0)
            {
                UpdatePaginationInfo(0);
                return;
            }

            foreach (var a in list)
            {
                int rowIndex = guna2DataGridView1.Rows.Add(
                    a.Id,
                    a.KodeAlat,
                    a.NamaAlat,
                    a.NamaKategori ?? "-",
                    a.NamaLokasi ?? "-",
                    "Rp " + a.HargaSewaHarian.ToString("N0"),
                    a.Stok,
                    a.StokTersedia,
                    FormatKondisi(a.Kondisi),
                    FormatStatus(a.Status),
                    ""
                );

                if (a.Status == "tidak_aktif")
                {
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                }
                else if (a.Status == "disewa")
                {
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(230, 126, 34);
                }
            }
        }

        private string FormatKondisi(string kondisi)
        {
            switch (kondisi)
            {
                case "rusak_ringan": return "Rusak Ringan";
                case "rusak_berat": return "Rusak Berat";
                default: return "Baik";
            }
        }

        private string FormatStatus(string status)
        {
            switch (status)
            {
                case "disewa": return "Disewa";
                case "maintenance": return "Maintenance";
                case "tidak_aktif": return "Tidak Aktif";
                default: return "Tersedia";
            }
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            lbTotal.Text = $"Total: {totalFiltered} alat";
            lbHalaman.Text = $"Halaman {_currentPage} dari {_totalPages}";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        // ============================================
        // CELL PAINTING - Empat tombol aksi
        // ============================================
        private void Guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                e.PaintBackground(e.CellBounds, true);

                int quarterWidth = e.CellBounds.Width / 4;
                int buttonHeight = e.CellBounds.Height - 6;
                int buttonY = e.CellBounds.Y + 3;

                Rectangle editRect = new Rectangle(e.CellBounds.X + 2, buttonY, quarterWidth - 3, buttonHeight);
                Rectangle riwayatRect = new Rectangle(e.CellBounds.X + quarterWidth + 1, buttonY, quarterWidth - 3, buttonHeight);
                Rectangle statusRect = new Rectangle(e.CellBounds.X + (2 * quarterWidth) + 1, buttonY, quarterWidth - 3, buttonHeight);
                Rectangle deleteRect = new Rectangle(e.CellBounds.X + (3 * quarterWidth) + 1, buttonY, quarterWidth - 3, buttonHeight);

                object statusObj = guna2DataGridView1.Rows[e.RowIndex].Cells["Status"].Value;
                string status = statusObj?.ToString() ?? "tersedia";
                string statusText = status == "tidak_aktif" ? "Aktifkan" : "Nonaktif";
                Color statusColor = status == "tidak_aktif" ? Color.FromArgb(230, 126, 34) : Color.FromArgb(241, 196, 15);

                DrawCellButton(e, editRect, "Edit", Color.FromArgb(52, 152, 219));
                DrawCellButton(e, riwayatRect, "Riwayat", Color.FromArgb(155, 89, 182));
                DrawCellButton(e, statusRect, statusText, statusColor);
                DrawCellButton(e, deleteRect, "Hapus", Color.FromArgb(231, 76, 60));

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
                var alatId = Convert.ToUInt64(guna2DataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                var status = guna2DataGridView1.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "tersedia";

                Rectangle cellRect = guna2DataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                Point clickPoint = guna2DataGridView1.PointToClient(Control.MousePosition);
                int clickX = clickPoint.X - cellRect.X;
                int quarterWidth = guna2DataGridView1.Columns[e.ColumnIndex].Width / 4;

                if (clickX < quarterWidth)
                {
                    ShowEditAlatForm(alatId);
                }
                else if (clickX < (2 * quarterWidth))
                {
                    ShowRiwayatAlat(alatId);
                }
                else if (clickX < (3 * quarterWidth))
                {
                    ToggleStatus(alatId, status);
                }
                else
                {
                    DeleteAlatLog(alatId);
                }
            }
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Fungsi ditangani oleh Guna2DataGridView1_CellClick
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
            if (cbStatus.SelectedItem != null)
            {
                _currentStatus = cbStatus.SelectedItem.ToString();
                _currentPage = 1;
                ApplyFilters();
            }
        }

        private void cbKategori_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentKategori = GetSelectedKategoriId();
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

        private void btnTambah_Click(object sender, EventArgs e)
        {
            using (var form = new CreateAlatProyek())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadAlat();
                }
            }
        }

        // ============================================
        // CRUD OPERATIONS
        // ============================================
        private void ShowEditAlatForm(ulong alatId)
        {
            using (var form = new EditAlatProyek(alatId))
            {
                try
                {
                    if (!form.InitializationSucceeded)
                    {
                        MessageBox.Show(form.InitializationErrorMessage ?? "Gagal memuat data alat.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadAlat();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error membuka form Edit Alat: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowRiwayatAlat(ulong alatId)
        {
            var alat = _allAlat.Find(a => a.Id == alatId);
            using (var form = new RiwayatAlatProyek(alatId, alat?.NamaAlat ?? ""))
            {
                form.ShowDialog();
            }
        }

        private void ToggleStatus(ulong alatId, string currentStatus)
        {
            var alat = _allAlat.Find(a => a.Id == alatId);
            string newStatus = currentStatus == "tidak_aktif" ? "tersedia" : "tidak_aktif";
            string actionText = newStatus == "tidak_aktif" ? "menonaktifkan" : "mengaktifkan";

            DialogResult result = MessageBox.Show(
                $"Apakah Anda yakin ingin {actionText} alat '{alat?.NamaAlat}'?",
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (ToggleStatusAlat(alatId, newStatus))
                {
                    MessageBox.Show($"Alat berhasil di{actionText}!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAlat();
                }
                else
                {
                    MessageBox.Show("Gagal mengubah status alat.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteAlatLog(ulong alatId)
        {
            var alat = _allAlat.Find(a => a.Id == alatId);

            DialogResult result = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus alat '{alat?.NamaAlat}'? Proses ini tidak dapat dibatalkan.",
                "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                if (DeleteAlat(alatId))
                {
                    MessageBox.Show("Alat berhasil dihapus!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAlat();
                }
                else
                {
                    MessageBox.Show("Alat tidak ditemukan atau gagal dihapus.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
