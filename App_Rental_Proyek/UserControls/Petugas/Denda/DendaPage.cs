using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;

namespace App_Rental_Proyek.UserControls.Petugas.Denda
{
    public partial class DendaPage : System.Windows.Forms.UserControl
    {
        private class DendaViewItem
        {
            public DendaModel Denda { get; set; }
            public string KodePenyewaan { get; set; }
            public string NamaCustomer { get; set; }
            public string KodePembayaran { get; set; }
            public string StatusPembayaran { get; set; }
            public string BuktiPembayaran { get; set; }
            public string KodePengembalian { get; set; }
        }

        private List<DendaViewItem> _allData = new List<DendaViewItem>();
        private int _currentPage = 1;
        private const int PageSize = 20;
        private int _totalPages = 0;
        private string _currentSearch = "";
        private string _currentJenisDenda = "Semua";
        private string _currentStatusDenda = "Semua";

        public DendaPage()
        {
            InitializeComponent();
            InitializeGridView();
            InitializeComboBoxes();
            LoadData();
            CheckSessionAndSetupUI();
        }

        private void CheckSessionAndSetupUI()
        {
            if (!SessionManager.IsLoggedIn)
            {
                MessageBox.Show("Sesi tidak valid. Silakan login kembali.",
                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblWelcome.Text = $"Petugas: {SessionManager.CurrentUser?.Nama ?? "Unknown"}";
        }

        private void InitializeComboBoxes()
        {
            cbJenisDenda.Items.Clear();
            cbJenisDenda.Items.Add("Semua");
            cbJenisDenda.Items.Add("terlambat");
            cbJenisDenda.Items.Add("kerusakan");
            cbJenisDenda.Items.Add("kehilangan");
            cbJenisDenda.SelectedIndex = 0;

            cbStatusDenda.Items.Clear();
            cbStatusDenda.Items.Add("Semua");
            cbStatusDenda.Items.Add("pending");
            cbStatusDenda.Items.Add("dibayar");
            cbStatusDenda.Items.Add("ditangguhkan");
            cbStatusDenda.SelectedIndex = 0;
        }

        private void InitializeGridView()
        {
            guna2DataGridView1.Columns.Clear();
            guna2DataGridView1.AutoGenerateColumns = false;

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Visible = false
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "KodePenyewaan",
                HeaderText = "Kode Penyewaan",
                DataPropertyName = "KodePenyewaan",
                Width = 150
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NamaCustomer",
                HeaderText = "Customer",
                DataPropertyName = "NamaCustomer",
                Width = 180
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "JenisDenda",
                HeaderText = "Jenis Denda",
                DataPropertyName = "JenisDenda",
                Width = 120
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Jumlah",
                HeaderText = "Jumlah",
                DataPropertyName = "Jumlah",
                Width = 120
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                DataPropertyName = "Status",
                Width = 120
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StatusPembayaran",
                HeaderText = "Status Pembayaran",
                DataPropertyName = "StatusPembayaran",
                Width = 150
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CreatedAt",
                HeaderText = "Tanggal Dibuat",
                DataPropertyName = "CreatedAt",
                Width = 150
            });

            DataGridViewColumn colAction = new DataGridViewColumn
            {
                Name = "Action",
                HeaderText = "Aksi",
                CellTemplate = new DataGridViewTextBoxCell(),
                Width = 200
            };
            guna2DataGridView1.Columns.Add(colAction);

            guna2DataGridView1.CellPainting += Guna2DataGridView1_CellPainting;
            guna2DataGridView1.CellClick += Guna2DataGridView1_CellClick;
            guna2DataGridView1.CellFormatting += Guna2DataGridView1_CellFormatting;
        }

        private void Guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "Jumlah")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal jumlah))
                {
                    e.Value = $"Rp {jumlah:N0}";
                    e.FormattingApplied = true;
                }
            }
            else if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "Status")
            {
                if (e.Value != null)
                {
                    string status = e.Value.ToString();
                    DataGridViewCell cell = guna2DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    switch (status.ToLower())
                    {
                        case "pending":
                            cell.Style.BackColor = Color.FromArgb(230, 126, 34);
                            cell.Style.ForeColor = Color.White;
                            break;
                        case "dibayar":
                            cell.Style.BackColor = Color.FromArgb(46, 204, 113);
                            cell.Style.ForeColor = Color.White;
                            break;
                        case "ditangguhkan":
                            cell.Style.BackColor = Color.FromArgb(52, 152, 219);
                            cell.Style.ForeColor = Color.White;
                            break;
                    }
                }
            }
            else if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "StatusPembayaran")
            {
                if (e.Value != null)
                {
                    string status = e.Value.ToString();
                    DataGridViewCell cell = guna2DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    switch (status.ToLower())
                    {
                        case "pending":
                            cell.Style.BackColor = Color.FromArgb(230, 126, 34);
                            cell.Style.ForeColor = Color.White;
                            break;
                        case "diverifikasi":
                            cell.Style.BackColor = Color.FromArgb(46, 204, 113);
                            cell.Style.ForeColor = Color.White;
                            break;
                        case "ditolak":
                            cell.Style.BackColor = Color.FromArgb(231, 76, 60);
                            cell.Style.ForeColor = Color.White;
                            break;
                        default:
                            cell.Style.BackColor = Color.LightGray;
                            cell.Style.ForeColor = Color.Black;
                            break;
                    }
                }
            }
        }

        private void Guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                e.PaintBackground(e.CellBounds, true);

                int buttonWidth = (e.CellBounds.Width - 12) / 2;
                int buttonHeight = e.CellBounds.Height - 6;
                int buttonY = e.CellBounds.Y + 3;

                Rectangle detailRect = new Rectangle(e.CellBounds.X + 3, buttonY, buttonWidth, buttonHeight);
                Rectangle actionRect = new Rectangle(e.CellBounds.X + buttonWidth + 9, buttonY, buttonWidth, buttonHeight);

                using (Brush detailBrush = new SolidBrush(Color.FromArgb(23, 59, 99)))
                using (Brush actionBrush = new SolidBrush(Color.FromArgb(46, 204, 113)))
                {
                    e.Graphics.FillRectangle(detailBrush, detailRect);
                    e.Graphics.FillRectangle(actionBrush, actionRect);

                    using (StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    })
                    {
                        e.Graphics.DrawString("Detail", guna2DataGridView1.Font, Brushes.White, detailRect, sf);
                        e.Graphics.DrawString("Aksi", guna2DataGridView1.Font, Brushes.White, actionRect, sf);
                    }
                }

                e.Handled = true;
            }
        }

        private void Guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "Action")
            {
                ulong dendaId = Convert.ToUInt64(guna2DataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
                DendaViewItem item = _allData.FirstOrDefault(d => d.Denda.Id == dendaId);

                if (item == null) return;

                Rectangle cellBounds = guna2DataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                Point clickPoint = guna2DataGridView1.PointToClient(Cursor.Position);
                int relativeX = clickPoint.X - cellBounds.X;

                int buttonWidth = (cellBounds.Width - 12) / 2;

                if (relativeX < buttonWidth + 3)
                {
                    ShowDetail(item);
                }
                else
                {
                    ShowActionMenu(item);
                }
            }
        }

        private void ShowDetail(DendaViewItem item)
        {
            using (var form = new DendaDetail(item.Denda, item.KodePenyewaan, item.NamaCustomer,
                item.KodePembayaran, item.StatusPembayaran, item.BuktiPembayaran, item.KodePengembalian))
            {
                form.ShowDialog();
            }
        }

        private void ShowActionMenu(DendaViewItem item)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.BackColor = Color.White;
            menu.Font = new Font("Segoe UI", 10F);

            if (item.Denda.Status == "pending" && !string.IsNullOrEmpty(item.BuktiPembayaran) && item.StatusPembayaran == "pending")
            {
                ToolStripMenuItem verifikasiItem = new ToolStripMenuItem("Verifikasi Pembayaran");
                verifikasiItem.Click += (s, e) => ShowVerifikasiPembayaran(item);
                menu.Items.Add(verifikasiItem);
            }

            if (item.Denda.Status == "pending")
            {
                ToolStripMenuItem ubahStatusItem = new ToolStripMenuItem("Ubah Status Denda");
                ubahStatusItem.Click += (s, e) => ShowUbahStatus(item);
                menu.Items.Add(ubahStatusItem);
            }

            if (menu.Items.Count == 0)
            {
                MessageBox.Show("Tidak ada aksi yang tersedia untuk denda ini.", "Informasi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Point screenPoint = guna2DataGridView1.PointToScreen(
                guna2DataGridView1.GetCellDisplayRectangle(
                    guna2DataGridView1.Columns["Action"].Index,
                    guna2DataGridView1.CurrentCell.RowIndex, false).Location);

            menu.Show(screenPoint);
        }

        private void ShowVerifikasiPembayaran(DendaViewItem item)
        {
            using (var form = new VerifikasiPembayaran(item.Denda, item.KodePenyewaan, item.NamaCustomer,
                item.KodePembayaran, item.BuktiPembayaran))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void ShowUbahStatus(DendaViewItem item)
        {
            using (var form = new UbahStatusDenda(item.Denda, item.KodePenyewaan, item.NamaCustomer))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void LoadData()
        {
            try
            {
                string query = @"
                    SELECT 
                        d.id,
                        d.penyewaan_id,
                        d.pengembalian_id,
                        d.jenis_denda,
                        d.jumlah,
                        d.alasan,
                        d.status,
                        d.created_at,
                        d.updated_at,
                        p.kode_penyewaan,
                        u.nama AS nama_customer,
                        pb.kode_pembayaran,
                        pb.status AS status_pembayaran,
                        pb.bukti_pembayaran,
                        pg.id AS pengembalian_id_ref
                    FROM dendas d
                    INNER JOIN penyewaans p ON d.penyewaan_id = p.id
                    INNER JOIN users u ON p.user_id = u.id
                    LEFT JOIN pembayarans pb ON d.id = pb.denda_id
                    LEFT JOIN pengembalians pg ON d.pengembalian_id = pg.id
                    WHERE 1=1";

                List<MySqlParameter> parameters = new List<MySqlParameter>();

                if (!string.IsNullOrWhiteSpace(_currentSearch))
                {
                    query += " AND (p.kode_penyewaan LIKE @search OR u.nama LIKE @search OR d.alasan LIKE @search)";
                    parameters.Add(new MySqlParameter("@search", $"%{_currentSearch}%"));
                }

                if (_currentJenisDenda != "Semua")
                {
                    query += " AND d.jenis_denda = @jenisDenda";
                    parameters.Add(new MySqlParameter("@jenisDenda", _currentJenisDenda));
                }

                if (_currentStatusDenda != "Semua")
                {
                    query += " AND d.status = @statusDenda";
                    parameters.Add(new MySqlParameter("@statusDenda", _currentStatusDenda));
                }

                query += " ORDER BY d.created_at DESC";

                DataTable dt = DatabaseConnection.GetData(query, parameters.ToArray());

                _allData.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    var item = new DendaViewItem
                    {
                        Denda = new DendaModel
                        {
                            Id = Convert.ToUInt64(row["id"]),
                            PenyewaanId = Convert.ToUInt64(row["penyewaan_id"]),
                            PengembalianId = row["pengembalian_id"] != DBNull.Value ? Convert.ToUInt64(row["pengembalian_id"]) : (ulong?)null,
                            JenisDenda = row["jenis_denda"].ToString(),
                            Jumlah = Convert.ToDecimal(row["jumlah"]),
                            Alasan = row["alasan"].ToString(),
                            Status = row["status"].ToString(),
                            CreatedAt = row["created_at"] != DBNull.Value ? Convert.ToDateTime(row["created_at"]) : (DateTime?)null,
                            UpdatedAt = row["updated_at"] != DBNull.Value ? Convert.ToDateTime(row["updated_at"]) : (DateTime?)null
                        },
                        KodePenyewaan = row["kode_penyewaan"].ToString(),
                        NamaCustomer = row["nama_customer"].ToString(),
                        KodePembayaran = row["kode_pembayaran"] != DBNull.Value ? row["kode_pembayaran"].ToString() : "-",
                        StatusPembayaran = row["status_pembayaran"] != DBNull.Value ? row["status_pembayaran"].ToString() : "-",
                        BuktiPembayaran = row["bukti_pembayaran"] != DBNull.Value ? row["bukti_pembayaran"].ToString() : "",
                        KodePengembalian = row["pengembalian_id_ref"] != DBNull.Value ? $"PGM-{row["pengembalian_id_ref"]}" : "-"
                    };

                    _allData.Add(item);
                }

                ApplyPagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat data denda: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyPagination()
        {
            _totalPages = (int)Math.Ceiling((double)_allData.Count / PageSize);

            if (_totalPages == 0) _totalPages = 1;
            if (_currentPage > _totalPages) _currentPage = _totalPages;

            var pagedData = _allData
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(item => new
                {
                    Id = item.Denda.Id,
                    KodePenyewaan = item.KodePenyewaan,
                    NamaCustomer = item.NamaCustomer,
                    JenisDenda = item.Denda.JenisDenda,
                    Jumlah = item.Denda.Jumlah,
                    Status = item.Denda.Status,
                    StatusPembayaran = item.StatusPembayaran,
                    CreatedAt = item.Denda.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-"
                })
                .ToList();

            guna2DataGridView1.DataSource = pagedData;

            lbPetunjukHalaman.Text = $"Halaman {_currentPage} dari {_totalPages}";
            lbTotalDenda.Text = $"Total: {_allData.Count} denda";

            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _currentSearch = txtSearch.Text.Trim();
            _currentPage = 1;
            LoadData();
        }

        private void cbJenisDenda_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentJenisDenda = cbJenisDenda.SelectedItem?.ToString() ?? "Semua";
            _currentPage = 1;
            LoadData();
        }

        private void cbStatusDenda_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentStatusDenda = cbStatusDenda.SelectedItem?.ToString() ?? "Semua";
            _currentPage = 1;
            LoadData();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyPagination();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                ApplyPagination();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cbJenisDenda.SelectedIndex = 0;
            cbStatusDenda.SelectedIndex = 0;
            _currentPage = 1;
            LoadData();

            ActivityLogHelper.LogForSession(
                SessionManager.GetCurrentUserId(),
                "Refresh data denda",
                "Manajemen Denda",
                null
            );
        }
    }
}
