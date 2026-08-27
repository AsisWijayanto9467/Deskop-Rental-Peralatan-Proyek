using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using App_Rental_Proyek.UserControls.Admin.KategoriManagement;
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
    public partial class KategoriList : System.Windows.Forms.UserControl
    {
        private List<KategoriModel> _allKategori = new List<KategoriModel>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 1;
        private string _currentSearch = "";

        public KategoriList()
        {
            InitializeComponent();
            InitializeGridView();
            LoadKategori();
        }

        private void KategoriList_Load(object sender, EventArgs e)
        {
        }

        // ============================================
        // INISIALISASI
        // ============================================
        private void InitializeGridView()
        {
            dgvKategori.Columns.Clear();

            dgvKategori.Columns.Add("Id", "ID");
            dgvKategori.Columns.Add("NamaKategori", "Nama Kategori");
            dgvKategori.Columns.Add("Deskripsi", "Deskripsi");
            dgvKategori.Columns.Add("JumlahAlat", "Jumlah Alat");
            dgvKategori.Columns.Add("Status", "Status");
            dgvKategori.Columns.Add("CreatedAt", "Tanggal Dibuat");

            DataGridViewColumn colAction = new DataGridViewColumn();
            colAction.Name = "Action";
            colAction.HeaderText = "Aksi";
            colAction.CellTemplate = new DataGridViewTextBoxCell();
            colAction.Width = 240;
            colAction.MinimumWidth = 240;
            dgvKategori.Columns.Add(colAction);

            dgvKategori.Columns["Id"].Visible = false;
            dgvKategori.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvKategori.AllowUserToAddRows = false;
            dgvKategori.ReadOnly = true;
            dgvKategori.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvKategori.MultiSelect = false;

            dgvKategori.Columns["NamaKategori"].MinimumWidth = 180;
            dgvKategori.Columns["Deskripsi"].MinimumWidth = 250;
            dgvKategori.Columns["Action"].Width = 240;

            dgvKategori.CellPainting += DgvKategori_CellPainting;
            dgvKategori.CellClick += DgvKategori_CellClick;
            dgvKategori.CellContentClick -= dgvKategori_CellContentClick;
        }

        // ============================================
        // DATABASE OPERATIONS
        // ============================================
        private List<KategoriModel> GetAllKategoriFromDatabase()
        {
            var list = new List<KategoriModel>();
            try
            {
                string query = @"
                    SELECT k.id, k.nama_kategori, k.deskripsi, k.status, k.created_at, k.updated_at,
                           (SELECT COUNT(*) FROM alat_proyeks a WHERE a.kategori_id = k.id) AS jumlah_alat
                    FROM kategoris k
                    ORDER BY k.nama_kategori ASC";

                DataTable dt = DatabaseConnection.GetData(query);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new KategoriModel
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        NamaKategori = row["nama_kategori"]?.ToString() ?? "",
                        Deskripsi = row["deskripsi"]?.ToString(),
                        Status = row["status"]?.ToString() ?? "aktif",
                        CreatedAt = row["created_at"] as DateTime?,
                        UpdatedAt = row["updated_at"] as DateTime?
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengambil data kategori: {ex.Message}\n\nPastikan tabel 'kategoris' dan 'alat_proyeks' sudah dibuat di database.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }

        private int GetJumlahAlat(ulong kategoriId)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM alat_proyeks WHERE kategori_id = @kategoriId";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@kategoriId", kategoriId)
                };
                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result);
            }
            catch
            {
                return 0;
            }
        }

        // ============================================
        // LOAD DATA
        // ============================================
        private void LoadKategori()
        {
            try
            {
                _allKategori = GetAllKategoriFromDatabase();
                if (_allKategori == null) _allKategori = new List<KategoriModel>();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat kategori: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _allKategori = new List<KategoriModel>();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (_allKategori == null) _allKategori = new List<KategoriModel>();

            var filtered = new List<KategoriModel>(_allKategori);

            if (!string.IsNullOrEmpty(_currentSearch))
            {
                filtered = filtered.FindAll(k =>
                    (k.NamaKategori?.ToLower().Contains(_currentSearch.ToLower()) ?? false) ||
                    (k.Deskripsi?.ToLower().Contains(_currentSearch.ToLower()) ?? false));
            }

            _totalPages = (int)Math.Ceiling((double)filtered.Count / PageSize);
            if (_totalPages == 0) _totalPages = 1;

            if (_currentPage > _totalPages) _currentPage = _totalPages;

            var pageData = filtered
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            DisplayKategori(pageData);
            UpdatePaginationInfo(filtered.Count);
        }

        private void DisplayKategori(List<KategoriModel> list)
        {
            dgvKategori.Rows.Clear();

            if (list == null || list.Count == 0)
            {
                UpdatePaginationInfo(0);
                return;
            }

            foreach (var k in list)
            {
                int rowIndex = dgvKategori.Rows.Add(
                    k.Id,
                    k.NamaKategori,
                    k.Deskripsi ?? "-",
                    GetJumlahAlat(k.Id),
                    k.Status,
                    k.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                    ""
                );

                if (k.Status == "nonaktif")
                {
                    dgvKategori.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            lblTotal.Text = $"Total: {totalFiltered} kategori";
            lblHalaman.Text = $"Halaman {_currentPage} dari {_totalPages}";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        // ============================================
        // CELL PAINTING - Tiga tombol aksi
        // ============================================
        private void DgvKategori_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                dgvKategori.Columns[e.ColumnIndex].Name == "Action")
            {
                e.PaintBackground(e.CellBounds, true);

                int thirdWidth = e.CellBounds.Width / 3;
                int buttonHeight = e.CellBounds.Height - 6;
                int buttonY = e.CellBounds.Y + 3;

                Rectangle editRect = new Rectangle(e.CellBounds.X + 2, buttonY, thirdWidth - 4, buttonHeight);
                Rectangle statusRect = new Rectangle(e.CellBounds.X + thirdWidth + 1, buttonY, thirdWidth - 4, buttonHeight);
                Rectangle deleteRect = new Rectangle(e.CellBounds.X + (2 * thirdWidth) + 1, buttonY, thirdWidth - 4, buttonHeight);

                object statusObj = dgvKategori.Rows[e.RowIndex].Cells["Status"].Value;
                string status = statusObj?.ToString() ?? "aktif";
                string statusText = status == "nonaktif" ? "Aktifkan" : "Nonaktif";
                Color statusColor = status == "nonaktif" ? Color.FromArgb(230, 126, 34) : Color.FromArgb(241, 196, 15);

                DrawCellButton(e, editRect, "Edit", Color.FromArgb(52, 152, 219));
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

        // ============================================
        // CELL CLICK - Menentukan tombol yang diklik
        // ============================================
        private void DgvKategori_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                dgvKategori.Columns[e.ColumnIndex].Name == "Action")
            {
                var kategoriId = Convert.ToUInt64(dgvKategori.Rows[e.RowIndex].Cells["Id"].Value);
                var status = dgvKategori.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "aktif";

                Rectangle cellRect = dgvKategori.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                Point clickPoint = dgvKategori.PointToClient(Control.MousePosition);
                int clickX = clickPoint.X - cellRect.X;
                int thirdWidth = dgvKategori.Columns[e.ColumnIndex].Width / 3;

                if (clickX < thirdWidth)
                {
                    ShowEditKategoriForm(kategoriId);
                }
                else if (clickX < (2 * thirdWidth))
                {
                    ToggleStatus(kategoriId, status);
                }
                else
                {
                    DeleteKategori(kategoriId);
                }
            }
        }

        private void dgvKategori_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kosongkan - fungsionalitas ditangani oleh DgvKategori_CellClick
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

        private void btnTambah_Click(object sender, EventArgs e)
        {
            ShowCreateKategoriForm();
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

        // ============================================
        // CRUD OPERATIONS
        // ============================================
        private void ShowCreateKategoriForm()
        {
            using (var form = new CreateKategori())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadKategori();
                }
            }
        }

        private void ShowEditKategoriForm(ulong kategoriId)
        {
            using (var form = new EditKategori(kategoriId))
            {
                try
                {
                    if (!form.InitializationSucceeded)
                    {
                        MessageBox.Show(form.InitializationErrorMessage ?? "Gagal memuat data kategori.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadKategori();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error membuka form Edit Kategori: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ToggleStatus(ulong kategoriId, string currentStatus)
        {
            var kategori = _allKategori.Find(k => k.Id == kategoriId);
            string newStatus = currentStatus == "nonaktif" ? "aktif" : "nonaktif";
            string actionText = newStatus == "nonaktif" ? "menonaktifkan" : "mengaktifkan";

            DialogResult result = MessageBox.Show(
                $"Apakah Anda yakin ingin {actionText} kategori '{kategori?.NamaKategori}'?",
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string query = "UPDATE kategoris SET status = @status, updated_at = NOW() WHERE id = @id";
                    MySqlParameter[] parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@status", newStatus),
                        new MySqlParameter("@id", kategoriId)
                    };

                    if (DatabaseConnection.ExecuteQuery(query, parameters) > 0)
                    {
                        MessageBox.Show($"Kategori berhasil di{actionText}!", "Sukses",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadKategori();
                    }
                    else
                    {
                        MessageBox.Show("Gagal mengubah status kategori.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteKategori(ulong kategoriId)
        {
            var kategori = _allKategori.Find(k => k.Id == kategoriId);
            int jumlahAlat = GetJumlahAlat(kategoriId);

            if (jumlahAlat > 0)
            {
                MessageBox.Show(
                    $"Kategori '{kategori?.NamaKategori}' tidak dapat dihapus karena masih digunakan oleh {jumlahAlat} alat proyek.\n\nSilakan nonaktifkan kategori atau pindahkan alat terlebih dahulu.",
                    "Kategori Digunakan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus kategori '{kategori?.NamaKategori}'?",
                "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM kategoris WHERE id = @id";
                    MySqlParameter[] parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@id", kategoriId)
                    };

                    if (DatabaseConnection.ExecuteQuery(query, parameters) > 0)
                    {
                        MessageBox.Show("Kategori berhasil dihapus!", "Sukses",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadKategori();
                    }
                    else
                    {
                        MessageBox.Show("Kategori tidak ditemukan atau gagal dihapus.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}