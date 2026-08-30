using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using App_Rental_Proyek.UserControls.Admin.Lokasi;
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
    public partial class LokasiList : System.Windows.Forms.UserControl
    {
        private List<LokasiModel> _allLokasi = new List<LokasiModel>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 1;
        private string _currentSearch = "";
        private string _currentStatus = "Semua";

        public LokasiList()
        {
            InitializeComponent();
            InitializeGridView();
            InitializeFilters();
            LoadLokasi();
        }

        private void LokasiList_Load(object sender, EventArgs e)
        {
        }

        // ============================================
        // INISIALISASI
        // ============================================
        private void InitializeGridView()
        {
            dgvLokasi.Columns.Clear();

            dgvLokasi.Columns.Add("Id", "ID");
            dgvLokasi.Columns.Add("Nama", "Nama Lokasi");
            dgvLokasi.Columns.Add("Alamat", "Alamat");
            dgvLokasi.Columns.Add("Keterangan", "Keterangan");
            dgvLokasi.Columns.Add("JumlahAlat", "Jumlah Alat");
            dgvLokasi.Columns.Add("Status", "Status");
            dgvLokasi.Columns.Add("CreatedAt", "Tanggal Dibuat");

            DataGridViewColumn colAction = new DataGridViewColumn();
            colAction.Name = "Action";
            colAction.HeaderText = "Aksi";
            colAction.CellTemplate = new DataGridViewTextBoxCell();
            colAction.Width = 250;
            colAction.MinimumWidth = 250;
            dgvLokasi.Columns.Add(colAction);

            dgvLokasi.Columns["Id"].Visible = false;
            dgvLokasi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLokasi.AllowUserToAddRows = false;
            dgvLokasi.ReadOnly = true;
            dgvLokasi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLokasi.MultiSelect = false;

            dgvLokasi.Columns["Nama"].MinimumWidth = 160;
            dgvLokasi.Columns["Alamat"].MinimumWidth = 250;
            dgvLokasi.Columns["Action"].Width = 250;

            dgvLokasi.CellPainting += DgvLokasi_CellPainting;
            dgvLokasi.CellClick += DgvLokasi_CellClick;
            dgvLokasi.CellContentClick -= dgvLokasi_CellContentClick;
        }

        private void InitializeFilters()
        {
            cbStatus.Items.Clear();
            cbStatus.Items.Add("Semua");
            cbStatus.Items.Add("aktif");
            cbStatus.Items.Add("nonaktif");
            cbStatus.SelectedIndex = 0;
        }

        // ============================================
        // DATABASE OPERATIONS
        // ============================================
        private List<LokasiModel> GetAllLokasiFromDatabase()
        {
            var list = new List<LokasiModel>();
            try
            {
                string query = @"
                    SELECT l.id, l.nama_lokasi, l.alamat, l.keterangan, l.status, l.created_at, l.updated_at,
                           (SELECT COUNT(*) FROM alat_proyeks a WHERE a.lokasi_id = l.id) AS jumlah_alat
                    FROM lokasis l
                    ORDER BY l.nama_lokasi ASC";

                DataTable dt = DatabaseConnection.GetData(query);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new LokasiModel
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        NamaLokasi = row["nama_lokasi"]?.ToString() ?? "",
                        Alamat = row["alamat"]?.ToString() ?? "",
                        Keterangan = row["keterangan"]?.ToString(),
                        Status = row["status"]?.ToString() ?? "aktif",
                        CreatedAt = row["created_at"] as DateTime?,
                        UpdatedAt = row["updated_at"] as DateTime?,
                        JumlahAlat = row["jumlah_alat"] != DBNull.Value ? Convert.ToInt32(row["jumlah_alat"]) : 0
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mengambil data lokasi: {ex.Message}\n\nPastikan tabel 'lokasis' dan 'alat_proyeks' sudah dibuat di database.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }

        // ============================================
        // LOAD DATA
        // ============================================
        private void LoadLokasi()
        {
            try
            {
                _allLokasi = GetAllLokasiFromDatabase();
                if (_allLokasi == null) _allLokasi = new List<LokasiModel>();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat lokasi: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _allLokasi = new List<LokasiModel>();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (_allLokasi == null) _allLokasi = new List<LokasiModel>();

            var filtered = new List<LokasiModel>(_allLokasi);

            if (!string.IsNullOrEmpty(_currentSearch))
            {
                string s = _currentSearch.ToLower();
                filtered = filtered.FindAll(l =>
                    (l.NamaLokasi?.ToLower().Contains(s) ?? false) ||
                    (l.Alamat?.ToLower().Contains(s) ?? false) ||
                    (l.Keterangan?.ToLower().Contains(s) ?? false));
            }

            if (_currentStatus != "Semua")
            {
                filtered = filtered.FindAll(l => l.Status == _currentStatus);
            }

            _totalPages = (int)Math.Ceiling((double)filtered.Count / PageSize);
            if (_totalPages == 0) _totalPages = 1;

            if (_currentPage > _totalPages) _currentPage = _totalPages;

            var pageData = filtered
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            DisplayLokasi(pageData);
            UpdatePaginationInfo(filtered.Count);
        }

        private void DisplayLokasi(List<LokasiModel> list)
        {
            dgvLokasi.Rows.Clear();

            if (list == null || list.Count == 0)
            {
                UpdatePaginationInfo(0);
                return;
            }

            foreach (var l in list)
            {
                int rowIndex = dgvLokasi.Rows.Add(
                    l.Id,
                    l.NamaLokasi,
                    l.Alamat ?? "-",
                    l.Keterangan ?? "-",
                    l.JumlahAlat,
                    l.Status,
                    l.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                    ""
                );

                if (l.Status == "nonaktif")
                {
                    dgvLokasi.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            lbTotal.Text = $"Total: {totalFiltered} lokasi";
            lbHalaman.Text = $"Halaman {_currentPage} dari {_totalPages}";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        // ============================================
        // CELL PAINTING - Tiga tombol aksi
        // ============================================
        private void DgvLokasi_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                dgvLokasi.Columns[e.ColumnIndex].Name == "Action")
            {
                e.PaintBackground(e.CellBounds, true);

                int thirdWidth = e.CellBounds.Width / 3;
                int buttonHeight = e.CellBounds.Height - 6;
                int buttonY = e.CellBounds.Y + 3;

                Rectangle editRect = new Rectangle(e.CellBounds.X + 2, buttonY, thirdWidth - 4, buttonHeight);
                Rectangle statusRect = new Rectangle(e.CellBounds.X + thirdWidth + 1, buttonY, thirdWidth - 4, buttonHeight);
                Rectangle deleteRect = new Rectangle(e.CellBounds.X + (2 * thirdWidth) + 1, buttonY, thirdWidth - 4, buttonHeight);

                object statusObj = dgvLokasi.Rows[e.RowIndex].Cells["Status"].Value;
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

        private void DgvLokasi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                dgvLokasi.Columns[e.ColumnIndex].Name == "Action")
            {
                var lokasiId = Convert.ToUInt64(dgvLokasi.Rows[e.RowIndex].Cells["Id"].Value);
                var status = dgvLokasi.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "aktif";

                Rectangle cellRect = dgvLokasi.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                Point clickPoint = dgvLokasi.PointToClient(Control.MousePosition);
                int clickX = clickPoint.X - cellRect.X;
                int thirdWidth = dgvLokasi.Columns[e.ColumnIndex].Width / 3;

                if (clickX < thirdWidth)
                {
                    ShowEditLokasiForm(lokasiId);
                }
                else if (clickX < (2 * thirdWidth))
                {
                    ToggleStatus(lokasiId, status);
                }
                else
                {
                    DeleteLokasi(lokasiId);
                }
            }
        }

        private void dgvLokasi_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kosongkan - fungsionalitas ditangani oleh DgvLokasi_CellClick
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
            using (var form = new CreateLokasi())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadLokasi();
                }
            }
        }

        // ============================================
        // CRUD OPERATIONS
        // ============================================
        private void ShowEditLokasiForm(ulong lokasiId)
        {
            using (var form = new EditLokasi(lokasiId))
            {
                try
                {
                    if (!form.InitializationSucceeded)
                    {
                        MessageBox.Show(form.InitializationErrorMessage ?? "Gagal memuat data lokasi.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadLokasi();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error membuka form Edit Lokasi: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ToggleStatus(ulong lokasiId, string currentStatus)
        {
            var lokasi = _allLokasi.Find(l => l.Id == lokasiId);
            string newStatus = currentStatus == "nonaktif" ? "aktif" : "nonaktif";
            string actionText = newStatus == "nonaktif" ? "menonaktifkan" : "mengaktifkan";

            DialogResult result = MessageBox.Show(
                $"Apakah Anda yakin ingin {actionText} lokasi '{lokasi?.NamaLokasi}'?",
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (UpdateStatusWithActivityLog(lokasiId, newStatus, actionText, lokasi?.NamaLokasi))
                    {
                        MessageBox.Show($"Lokasi berhasil di{actionText}!", "Sukses",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadLokasi();
                    }
                    else
                    {
                        MessageBox.Show("Gagal mengubah status lokasi.", "Error",
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

        private bool UpdateStatusWithActivityLog(ulong lokasiId, string newStatus, string actionText, string namaLokasi)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                string updateQuery = "UPDATE lokasis SET status = @status, updated_at = NOW() WHERE id = @id";

                int affected;

                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection, transaction))
                {
                    updateCmd.Parameters.AddWithValue("@status", newStatus);
                    updateCmd.Parameters.AddWithValue("@id", lokasiId);

                    affected = updateCmd.ExecuteNonQuery();

                    if (affected <= 0)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                ulong currentUserId = SessionManager.GetCurrentUserId();

                if (currentUserId > 0)
                {
                    string logQuery = @"
                        INSERT INTO activity_logs
                        (user_id, aktivitas, modul, referensi_id, ip_address, created_at)
                        VALUES
                        (@userId, @aktivitas, @modul, @referensiId, @ipAddress, NOW())";

                    using (MySqlCommand logCmd = new MySqlCommand(logQuery, connection, transaction))
                    {
                        string mesian = newStatus == "nonaktif" ? "nonaktif" : "aktif";
                        string activityDescription = $"Meng{actionText} lokasi '{namaLokasi}' (status menjadi {mesian})";

                        logCmd.Parameters.AddWithValue("@userId", currentUserId);
                        logCmd.Parameters.AddWithValue("@aktivitas", activityDescription);
                        logCmd.Parameters.AddWithValue("@modul", "Lokasi");
                        logCmd.Parameters.AddWithValue("@referensiId", lokasiId);
                        logCmd.Parameters.AddWithValue("@ipAddress", GetClientIpAddress());

                        int logResult = logCmd.ExecuteNonQuery();

                        if (logResult <= 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try { transaction.Rollback(); }
                    catch { }
                }

                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    connection.Dispose();
                }
            }
        }

        private void DeleteLokasi(ulong lokasiId)
        {
            var lokasi = _allLokasi.Find(l => l.Id == lokasiId);
            int jumlahAlat = lokasi?.JumlahAlat ?? 0;

            if (jumlahAlat > 0)
            {
                MessageBox.Show(
                    $"Lokasi '{lokasi?.NamaLokasi}' tidak dapat dihapus karena masih digunakan oleh {jumlahAlat} alat proyek.\n\nSilakan nonaktifkan lokasi atau pindahkan alat terlebih dahulu.",
                    "Lokasi Digunakan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus lokasi '{lokasi?.NamaLokasi}'?",
                "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (DeleteLokasiWithActivityLog(lokasiId, lokasi?.NamaLokasi))
                    {
                        MessageBox.Show("Lokasi berhasil dihapus!", "Sukses",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadLokasi();
                    }
                    else
                    {
                        MessageBox.Show("Lokasi tidak ditemukan atau gagal dihapus.", "Error",
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

        private bool DeleteLokasiWithActivityLog(ulong lokasiId, string namaLokasi)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                string deleteQuery = "DELETE FROM lokasis WHERE id = @id";

                int affected;

                using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, connection, transaction))
                {
                    deleteCmd.Parameters.AddWithValue("@id", lokasiId);

                    affected = deleteCmd.ExecuteNonQuery();

                    if (affected <= 0)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                ulong currentUserId = SessionManager.GetCurrentUserId();

                if (currentUserId > 0)
                {
                    string logQuery = @"
                        INSERT INTO activity_logs
                        (user_id, aktivitas, modul, referensi_id, ip_address, created_at)
                        VALUES
                        (@userId, @aktivitas, @modul, @referensiId, @ipAddress, NOW())";

                    using (MySqlCommand logCmd = new MySqlCommand(logQuery, connection, transaction))
                    {
                        string activityDescription = $"Menghapus lokasi '{namaLokasi}'";

                        logCmd.Parameters.AddWithValue("@userId", currentUserId);
                        logCmd.Parameters.AddWithValue("@aktivitas", activityDescription);
                        logCmd.Parameters.AddWithValue("@modul", "Lokasi");
                        logCmd.Parameters.AddWithValue("@referensiId", lokasiId);
                        logCmd.Parameters.AddWithValue("@ipAddress", GetClientIpAddress());

                        int logResult = logCmd.ExecuteNonQuery();

                        if (logResult <= 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try { transaction.Rollback(); }
                    catch { }
                }

                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    connection.Dispose();
                }
            }
        }

        private string GetClientIpAddress()
        {
            try
            {
                string hostName = System.Net.Dns.GetHostName();
                var addresses = System.Net.Dns.GetHostAddresses(hostName);

                foreach (var address in addresses)
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return address.ToString();
                    }
                }

                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }
}
