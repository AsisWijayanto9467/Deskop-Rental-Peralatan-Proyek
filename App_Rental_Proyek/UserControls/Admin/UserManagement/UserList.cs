using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using App_Rental_Proyek.UserControl.Admin;
using App_Rental_Proyek.Helper;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin
{
    public partial class UserList : System.Windows.Forms.UserControl
    {
        private List<UserModel> _allUsers = new List<UserModel>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 0;
        private string _currentSearch = "";
        private string _currentStatus = "Semua";
        private string _currentRole = "Semua";

        // ❌ HAPUS property ini - tidak perlu lagi
        // [Browsable(false)]
        // [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        // public UserModel CurrentLoggedInUser { get; set; }

        public UserList()
        {
            InitializeComponent();
            InitializeGridView();
            InitializeComboBoxes();
            LoadUsers();

            // ✅ Cek session di constructor
            CheckSessionAndSetupUI();
        }

        private void UserList_Load(object sender, EventArgs e)
        {
            // ✅ Gunakan SessionManager, bukan dari ParentForm.Tag
            CheckSessionAndSetupUI();
        }

        // ✅ Method untuk mengecek session dan setup UI
        private void CheckSessionAndSetupUI()
        {
            if (!SessionManager.IsLoggedIn)
            {
                System.Diagnostics.Debug.WriteLine("Warning: UserList - Tidak ada user login!");
                btnTambah.Enabled = false;
                lbTotalUser.Text = "⚠️ Session tidak ditemukan";

                // Tampilkan peringatan sekali saja
                MessageBox.Show("Session user tidak ditemukan. Aktivitas tidak akan dicatat ke log.",
                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                btnTambah.Enabled = true;

                // Tampilkan user yang login di status bar (opsional)
                string currentUser = SessionManager.CurrentUser?.Nama ?? "Unknown";
                lbTotalUser.Text = $"User: {currentUser} | Total: {_allUsers?.Count ?? 0} users";
            }
        }

        // ============================================
        // INISIALISASI - UPDATED GridView
        // ============================================
        private void InitializeGridView()
        {
            guna2DataGridView1.Columns.Clear();

            // Add columns
            guna2DataGridView1.Columns.Add("Id", "ID");
            guna2DataGridView1.Columns.Add("Nama", "Nama");
            guna2DataGridView1.Columns.Add("Username", "Username");
            guna2DataGridView1.Columns.Add("Email", "Email");
            guna2DataGridView1.Columns.Add("NoTelepon", "No. Telepon");
            guna2DataGridView1.Columns.Add("Role", "Role");
            guna2DataGridView1.Columns.Add("Status", "Status");
            guna2DataGridView1.Columns.Add("CreatedAt", "Tanggal Dibuat");

            // Add single Action column with both buttons
            DataGridViewColumn colAction = new DataGridViewColumn();
            colAction.Name = "Action";
            colAction.HeaderText = "Aksi";
            colAction.CellTemplate = new DataGridViewTextBoxCell();
            colAction.Width = 150;
            colAction.MinimumWidth = 150;
            guna2DataGridView1.Columns.Add(colAction);

            // Set column properties
            guna2DataGridView1.Columns["Id"].Visible = false;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;

            // Set column widths
            guna2DataGridView1.Columns["Nama"].MinimumWidth = 150;
            guna2DataGridView1.Columns["Username"].MinimumWidth = 100;
            guna2DataGridView1.Columns["Email"].MinimumWidth = 150;
            guna2DataGridView1.Columns["Action"].Width = 150;

            // Add CellPainting event for custom button rendering
            guna2DataGridView1.CellPainting += Guna2DataGridView1_CellPainting;

            // IMPORTANT: Use CellClick instead of CellContentClick for custom painted cells
            guna2DataGridView1.CellClick += Guna2DataGridView1_CellClick;

            // Remove any existing CellContentClick handlers to avoid conflicts
            guna2DataGridView1.CellContentClick -= guna2DataGridView1_CellContentClick_1;
        }

        private void InitializeComboBoxes()
        {
            // Setup cbRole
            cbRole.Items.Clear();
            cbRole.Items.Add("Semua");
            cbRole.Items.Add("admin");
            cbRole.Items.Add("petugas");
            cbRole.Items.Add("user");
            cbRole.SelectedIndex = 0;

            // Setup cbStatus
            cbStatus.Items.Clear();
            cbStatus.Items.Add("Semua");
            cbStatus.Items.Add("aktif");
            cbStatus.Items.Add("nonaktif");
            cbStatus.SelectedIndex = 0;
        }

        // ============================================
        // DATABASE OPERATIONS
        // ============================================
        private List<UserModel> GetAllUsersFromDatabase()
        {
            var users = new List<UserModel>();

            try
            {
                string query = @"
                    SELECT id, nama, username, email, password, no_telepon, alamat, role, status, created_at, updated_at
                    FROM users
                    ORDER BY created_at DESC";

                DataTable dt = DatabaseConnection.GetData(query);

                foreach (DataRow row in dt.Rows)
                {
                    users.Add(new UserModel
                    {
                        Id = Convert.ToUInt64(row["id"]),
                        Nama = row["nama"]?.ToString() ?? "",
                        Username = row["username"]?.ToString() ?? "",
                        Email = row["email"]?.ToString() ?? "",
                        Password = row["password"]?.ToString() ?? "",
                        NoTelepon = row["no_telepon"]?.ToString(),
                        Alamat = row["alamat"]?.ToString(),
                        Role = row["role"]?.ToString() ?? "user",
                        Status = row["status"]?.ToString() ?? "aktif",
                        CreatedAt = row["created_at"] as DateTime?,
                        UpdatedAt = row["updated_at"] as DateTime?
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to database: {ex.Message}\n\nPlease check your connection string and make sure the database is running.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return users;
        }

        // ⚠️ Method ini TIDAK DIGUNAKAN - bisa dihapus atau dikomentari
        // private bool InsertUserToDatabase(UserModel user) { ... }

        // ⚠️ Method ini TIDAK DIGUNAKAN - bisa dihapus atau dikomentari
        // private bool UpdateUserInDatabase(UserModel user) { ... }

        // ============================================
        // DELETE USER WITH ACTIVITY LOG - COMBINED METHOD
        // ============================================
        private bool DeleteUserWithActivityLog(ulong userId, UserModel userToDelete)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                // Buka koneksi dan mulai transaction
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                // 1. Hapus user dari tabel users
                string deleteQuery = "DELETE FROM users WHERE id = @id";

                using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, connection, transaction))
                {
                    deleteCmd.Parameters.AddWithValue("@id", userId);
                    int deleteResult = deleteCmd.ExecuteNonQuery();

                    if (deleteResult <= 0)
                    {
                        // Rollback jika gagal menghapus user
                        transaction.Rollback();
                        return false;
                    }
                }

                // ✅ Gunakan SessionManager untuk mendapatkan user ID
                ulong currentUserId = SessionManager.GetCurrentUserId();

                // ✅ Jika user tidak login, skip log
                if (currentUserId == 0)
                {
                    transaction.Commit();

                    MessageBox.Show("User berhasil dihapus.\n\n" +
                        "⚠️ Peringatan: Aktivitas tidak dicatat ke log karena session tidak ditemukan.\n" +
                        "Admin yang melakukan penghapusan tidak teridentifikasi.",
                        "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }

                // 2. Catat aktivitas penghapusan ke activity_logs
                string logQuery = @"
                    INSERT INTO activity_logs 
                    (user_id, aktivitas, modul, referensi_id, ip_address, created_at) 
                    VALUES 
                    (@userId, @aktivitas, @modul, @referensiId, @ipAddress, NOW())";

                using (MySqlCommand logCmd = new MySqlCommand(logQuery, connection, transaction))
                {
                    string ipAddress = GetClientIpAddress();
                    string activityDescription = $"Menghapus user '{userToDelete.Nama}' (username: {userToDelete.Username})";

                    logCmd.Parameters.AddWithValue("@userId", currentUserId);
                    logCmd.Parameters.AddWithValue("@aktivitas", activityDescription);
                    logCmd.Parameters.AddWithValue("@modul", "User Management");
                    logCmd.Parameters.AddWithValue("@referensiId", userId);
                    logCmd.Parameters.AddWithValue("@ipAddress", ipAddress);

                    int logResult = logCmd.ExecuteNonQuery();

                    if (logResult <= 0)
                    {
                        // Rollback jika gagal mencatat log
                        transaction.Rollback();
                        return false;
                    }
                }

                // Commit transaction jika semua berhasil
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // Rollback jika terjadi error
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch { /* Ignore rollback error */ }
                }

                MessageBox.Show($"Error deleting user: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                // Tutup connection
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

        // ============================================
        // HELPER: Get Client IP Address
        // ============================================
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

        // ============================================
        // LOAD DATA
        // ============================================
        private void LoadUsers()
        {
            try
            {
                _allUsers = GetAllUsersFromDatabase();

                if (_allUsers == null)
                {
                    _allUsers = new List<UserModel>();
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                _allUsers = new List<UserModel>();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (_allUsers == null)
            {
                _allUsers = new List<UserModel>();
            }

            var filteredUsers = new List<UserModel>(_allUsers);

            // Filter by search text
            if (!string.IsNullOrEmpty(_currentSearch))
            {
                filteredUsers = filteredUsers.FindAll(u =>
                    (u.Nama?.ToLower().Contains(_currentSearch.ToLower()) ?? false) ||
                    (u.Username?.ToLower().Contains(_currentSearch.ToLower()) ?? false) ||
                    (u.Email?.ToLower().Contains(_currentSearch.ToLower()) ?? false)
                );
            }

            // Filter by role
            if (_currentRole != "Semua")
            {
                filteredUsers = filteredUsers.FindAll(u => u.Role == _currentRole);
            }

            // Filter by status
            if (_currentStatus != "Semua")
            {
                filteredUsers = filteredUsers.FindAll(u => u.Status == _currentStatus);
            }

            // Calculate total pages
            _totalPages = (int)Math.Ceiling((double)filteredUsers.Count / PageSize);
            if (_totalPages == 0) _totalPages = 1;

            // Adjust current page if needed
            if (_currentPage > _totalPages)
                _currentPage = _totalPages;

            // Get current page data
            var pageUsers = filteredUsers
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Display data
            DisplayUsers(pageUsers);

            // Update UI
            UpdatePaginationInfo(filteredUsers.Count);
        }

        // ============================================
        // DISPLAY USERS - UPDATED
        // ============================================
        private void DisplayUsers(List<UserModel> users)
        {
            guna2DataGridView1.Rows.Clear();

            if (users == null || users.Count == 0)
            {
                UpdatePaginationInfo(0);
                return;
            }

            foreach (var user in users)
            {
                int rowIndex = guna2DataGridView1.Rows.Add(
                    user.Id,
                    user.Nama,
                    user.Username,
                    user.Email,
                    user.NoTelepon ?? "-",
                    user.Role,
                    user.Status,
                    user.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                    "" // Empty string, will be drawn by CellPainting
                );

                // Style the row based on status
                if (user.Status == "nonaktif")
                {
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                    guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.Font =
                        new Font(guna2DataGridView1.Font, FontStyle.Strikeout);
                }
            }
        }

        // ============================================
        // CELL PAINTING - Custom button rendering
        // ============================================
        private void Guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Only paint for Action column
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                e.PaintBackground(e.CellBounds, true);

                // Calculate button bounds
                int halfWidth = e.CellBounds.Width / 2;
                int buttonHeight = e.CellBounds.Height - 6;
                int buttonY = e.CellBounds.Y + 3;

                // Edit button (left side)
                Rectangle editRect = new Rectangle(e.CellBounds.X + 2, buttonY, halfWidth - 4, buttonHeight);

                // Delete button (right side)
                Rectangle deleteRect = new Rectangle(e.CellBounds.X + halfWidth + 2, buttonY, halfWidth - 4, buttonHeight);

                // Draw Edit button
                using (Brush editBrush = new SolidBrush(Color.FromArgb(52, 152, 219)))
                using (Pen borderPen = new Pen(Color.FromArgb(200, 200, 200), 1))
                {
                    // Edit button
                    e.Graphics.FillRectangle(editBrush, editRect);
                    e.Graphics.DrawRectangle(borderPen, editRect);

                    // Edit text
                    using (Font buttonFont = new Font("Microsoft Sans Serif", 8, FontStyle.Bold))
                    using (Brush textBrush = new SolidBrush(Color.White))
                    {
                        StringFormat sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        e.Graphics.DrawString("Edit", buttonFont, textBrush, editRect, sf);
                    }

                    // Delete button
                    using (Brush deleteBrush = new SolidBrush(Color.FromArgb(231, 76, 60)))
                    {
                        e.Graphics.FillRectangle(deleteBrush, deleteRect);
                        e.Graphics.DrawRectangle(borderPen, deleteRect);

                        // Delete text
                        using (Font buttonFont = new Font("Microsoft Sans Serif", 8, FontStyle.Bold))
                        using (Brush textBrush = new SolidBrush(Color.White))
                        {
                            StringFormat sf = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            };
                            e.Graphics.DrawString("Hapus", buttonFont, textBrush, deleteRect, sf);
                        }
                    }
                }

                e.Handled = true;
            }
        }

        private void UpdatePaginationInfo(int totalFiltered)
        {
            // ✅ Tampilkan info user yang login jika ada
            if (SessionManager.IsLoggedIn)
            {
                string currentUser = SessionManager.CurrentUser?.Nama ?? "Unknown";
                lbTotalUser.Text = $"User: {currentUser} | Total: {totalFiltered} users";
            }
            else
            {
                lbTotalUser.Text = $"⚠️ Session tidak ditemukan | Total: {totalFiltered} users";
            }

            lbPetunjukHalaman.Text = $"Halaman {_currentPage} dari {_totalPages}";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        // ============================================
        // EVENT HANDLERS
        // ============================================
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _currentSearch = txtSearch.Text;
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

        private void cbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbRole.SelectedItem != null)
            {
                _currentRole = cbRole.SelectedItem.ToString();
                _currentPage = 1;
                ApplyFilters();
            }
        }

        // ============================================
        // CRUD OPERATIONS
        // ============================================

        private void btnTambah_Click(object sender, EventArgs e)
        {
            ShowAddUserForm();
        }

        // ============================================
        // CELL CLICK - FIXED to handle combined action column
        // ============================================
        private void Guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Check if the click is on a valid row and column
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // Check if clicked on Action column
                    if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
                    {
                        // Get the User ID from the row
                        var idCell = guna2DataGridView1.Rows[e.RowIndex].Cells["Id"];
                        if (idCell.Value == null || idCell.Value == DBNull.Value)
                        {
                            return;
                        }

                        var userId = Convert.ToUInt64(idCell.Value);

                        // Get the click position relative to the cell
                        Rectangle cellRect = guna2DataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                        Point clickPoint = guna2DataGridView1.PointToClient(Control.MousePosition);

                        // Calculate click position within the cell
                        int clickX = clickPoint.X - cellRect.X;
                        int cellWidth = guna2DataGridView1.Columns[e.ColumnIndex].Width;

                        // Determine which button was clicked based on X position
                        if (clickX < cellWidth / 2)
                        {
                            // Left side - Edit button
                            ShowEditUserForm(userId);
                        }
                        else
                        {
                            // Right side - Delete button
                            DeleteUser(userId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================
        // OLD EVENT HANDLER - Keep empty to avoid designer errors
        // ============================================
        private void guna2DataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // This method is kept empty to satisfy the designer.
            // Actual functionality is handled by Guna2DataGridView1_CellClick.
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
        // FORM METHODS
        // ============================================

        private void ShowAddUserForm()
        {
            using (var createUserForm = new CreateUser())
            {
                // ✅ Kirim user dari session (opsional, untuk fallback)
                if (SessionManager.IsLoggedIn)
                {
                    createUserForm.Tag = SessionManager.CurrentUser;
                }

                var result = createUserForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        // ============================================
        // SHOW EDIT USER FORM - FIXED with proper disposal
        // ============================================
        private void ShowEditUserForm(ulong userId)
        {
            // ✅ Cek user mencoba mengedit diri sendiri
            if (SessionManager.IsLoggedIn && SessionManager.CurrentUser.Id == userId)
            {
                DialogResult result = MessageBox.Show(
                    "Anda akan mengedit akun Anda sendiri.\n\n" +
                    "Perubahan pada role atau status dapat mempengaruhi akses Anda.\n" +
                    "Lanjutkan?",
                    "Konfirmasi Edit Akun Sendiri",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            using (var editUserForm = new App_Rental_Proyek.UserControls.Admin.UserManagement.EditUser(userId))
            {
                try
                {
                    // Check if initialization succeeded
                    if (!editUserForm.InitializationSucceeded)
                    {
                        string errorMsg = editUserForm.InitializationErrorMessage ??
                                         "Gagal memuat data user. Silakan coba lagi.";

                        MessageBox.Show(errorMsg, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Show the form as a dialog
                    DialogResult result = editUserForm.ShowDialog();

                    // Reload users if the form was closed with OK
                    if (result == DialogResult.OK)
                    {
                        LoadUsers();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error membuka form Edit User: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ============================================
        // DELETE USER - Combined with Activity Log
        // ============================================
        private void DeleteUser(ulong userId)
        {
            if (_allUsers == null) return;

            var user = _allUsers.Find(u => u.Id == userId);
            if (user != null)
            {
                // ✅ Cek user mencoba menghapus dirinya sendiri
                if (SessionManager.IsLoggedIn && SessionManager.CurrentUser.Id == userId)
                {
                    MessageBox.Show("Anda tidak dapat menghapus akun Anda sendiri!",
                        "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Apakah Anda yakin ingin menghapus user '{user.Nama}'?\n\n" +
                    "Aktivitas ini akan dicatat dalam log sistem.",
                    "Konfirmasi Hapus",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Panggil method yang menggabungkan delete dan log
                        if (DeleteUserWithActivityLog(userId, user))
                        {
                            MessageBox.Show("User berhasil dihapus!",
                                "Sukses",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            LoadUsers();
                        }
                        else
                        {
                            MessageBox.Show("Gagal menghapus user!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
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

        private void lbTotalUser_Click(object sender, EventArgs e)
        {
            // Optional
        }

        private void lbPetunjukHalaman_Click(object sender, EventArgs e)
        {
            // Optional
        }
    }
}