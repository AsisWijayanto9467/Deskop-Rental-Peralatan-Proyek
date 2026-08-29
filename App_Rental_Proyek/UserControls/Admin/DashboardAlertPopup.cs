using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin
{
    public partial class DashboardAlertPopup : Form
    {
        private class AlertViewItem
        {
            public string Jenis { get; set; }
            public string Kode { get; set; }
            public string Referensi { get; set; }
            public string Nama { get; set; }
            public DateTime Tanggal { get; set; }
            public decimal Nilai { get; set; }
        }

        private readonly List<AlertViewItem> _items = new List<AlertViewItem>();

        public DashboardAlertPopup()
        {
            InitializeComponent();
            InitializeGridView();
        }

        private void DashboardAlertPopup_Load(object sender, EventArgs e)
        {
            cbJenis.SelectedIndex = 0;
        }

        // ============================================
        // INISIALISASI TABEL
        // ============================================
        private void InitializeGridView()
        {
            dgvPerhatian.Columns.Clear();

            dgvPerhatian.Columns.Add("Jenis", "Jenis");
            dgvPerhatian.Columns.Add("Kode", "Kode");
            dgvPerhatian.Columns.Add("Referensi", "Referensi");
            dgvPerhatian.Columns.Add("Nama", "Nama");
            dgvPerhatian.Columns.Add("Tanggal", "Tanggal");
            dgvPerhatian.Columns.Add("Nilai", "Nilai");

            dgvPerhatian.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPerhatian.AllowUserToAddRows = false;
            dgvPerhatian.ReadOnly = true;
            dgvPerhatian.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPerhatian.MultiSelect = false;

            dgvPerhatian.Columns["Kode"].MinimumWidth = 130;
            dgvPerhatian.Columns["Referensi"].MinimumWidth = 130;
            dgvPerhatian.Columns["Nama"].MinimumWidth = 140;
            dgvPerhatian.Columns["Jenis"].FillWeight = 20;
            dgvPerhatian.Columns["Kode"].FillWeight = 25;
            dgvPerhatian.Columns["Referensi"].FillWeight = 25;
            dgvPerhatian.Columns["Nama"].FillWeight = 40;
            dgvPerhatian.Columns["Tanggal"].FillWeight = 25;
            dgvPerhatian.Columns["Nilai"].FillWeight = 30;
        }

        // ============================================
        // LOAD DATA
        // ============================================
        private void LoadData()
        {
            _items.Clear();

            // 1. Penyewaan menunggu proses
            try
            {
                string qSewa = @"
                    SELECT p.kode_penyewaan, p.tanggal_pengajuan, p.total, u.nama
                    FROM penyewaans p
                    LEFT JOIN users u ON u.id = p.user_id
                    WHERE p.status = 'pending'
                    ORDER BY p.created_at ASC";

                DataTable dtSewa = DatabaseConnection.GetData(qSewa);

                foreach (DataRow row in dtSewa.Rows)
                {
                    _items.Add(new AlertViewItem
                    {
                        Jenis = "Penyewaan",
                        Kode = row["kode_penyewaan"]?.ToString() ?? "-",
                        Referensi = "-",
                        Nama = row["nama"]?.ToString() ?? "-",
                        Tanggal = row["tanggal_pengajuan"] != DBNull.Value
                            ? Convert.ToDateTime(row["tanggal_pengajuan"])
                            : DateTime.MinValue,
                        Nilai = row["total"] != DBNull.Value ? Convert.ToDecimal(row["total"]) : 0m
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error load penyewaan pending: {ex.Message}");
            }

            // 2. Pembayaran belum diverifikasi
            try
            {
                string qPmb = @"
                    SELECT pm.kode_pembayaran, p.kode_penyewaan, pm.tanggal_pembayaran, pm.jumlah, u.nama
                    FROM pembayarans pm
                    LEFT JOIN penyewaans p ON p.id = pm.penyewaan_id
                    LEFT JOIN users u ON u.id = p.user_id
                    WHERE pm.status = 'pending'
                    ORDER BY pm.created_at ASC";

                DataTable dtPmb = DatabaseConnection.GetData(qPmb);

                foreach (DataRow row in dtPmb.Rows)
                {
                    _items.Add(new AlertViewItem
                    {
                        Jenis = "Pembayaran",
                        Kode = row["kode_pembayaran"]?.ToString() ?? "-",
                        Referensi = row["kode_penyewaan"]?.ToString() ?? "-",
                        Nama = row["nama"]?.ToString() ?? "-",
                        Tanggal = row["tanggal_pembayaran"] != DBNull.Value
                            ? Convert.ToDateTime(row["tanggal_pembayaran"])
                            : DateTime.MinValue,
                        Nilai = row["jumlah"] != DBNull.Value ? Convert.ToDecimal(row["jumlah"]) : 0m
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error load pembayaran pending: {ex.Message}");
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = new List<AlertViewItem>(_items);

            int index = cbJenis.SelectedIndex;
            if (index == 1)
            {
                filtered = filtered.FindAll(i => i.Jenis == "Penyewaan");
            }
            else if (index == 2)
            {
                filtered = filtered.FindAll(i => i.Jenis == "Pembayaran");
            }

            DisplayItems(filtered);
        }

        private void DisplayItems(List<AlertViewItem> list)
        {
            dgvPerhatian.Rows.Clear();

            if (list == null || list.Count == 0)
            {
                lblInfo.Text = "Tidak ada transaksi yang memerlukan perhatian.";
                dgvPerhatian.Rows.Add("-", "-", "-", "-", "-", "-");
                return;
            }

            lblInfo.Text = $"{list.Count} transaksi memerlukan perhatian.";

            foreach (var item in list)
            {
                int rowIndex = dgvPerhatian.Rows.Add(
                    item.Jenis,
                    item.Kode,
                    item.Referensi,
                    item.Nama,
                    item.Tanggal != DateTime.MinValue ? item.Tanggal.ToString("dd/MM/yyyy") : "-",
                    "Rp " + item.Nilai.ToString("N0")
                );

                if (item.Jenis == "Penyewaan")
                {
                    dgvPerhatian.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(230, 126, 34);
                }
                else
                {
                    dgvPerhatian.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                }
            }
        }

        // ============================================
        // EVENT HANDLERS
        // ============================================
        private void cbJenis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_items.Count == 0 && cbJenis.SelectedIndex == 0)
            {
                LoadData();
            }
            else
            {
                ApplyFilter();
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}