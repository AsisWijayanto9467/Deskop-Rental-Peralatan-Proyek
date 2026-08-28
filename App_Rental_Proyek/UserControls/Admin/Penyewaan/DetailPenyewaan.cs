using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.Penyewaan
{
    public partial class DetailPenyewaan : Form
    {
        private ulong _penyewaanId;

        public DetailPenyewaan(ulong penyewaanId)
        {
            InitializeComponent();
            _penyewaanId = penyewaanId;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void DetailPenyewaan_Load(object sender, EventArgs e)
        {
            LoadHeader();
            LoadDetailItems();
        }

        private void LoadHeader()
        {
            try
            {
                string query = @"
                    SELECT p.kode_penyewaan, p.tanggal_pengajuan, p.tanggal_mulai, p.tanggal_selesai,
                           p.total_hari, p.subtotal, p.denda, p.total, p.status, p.catatan,
                           u.nama AS nama_customer, u.email AS email_customer,
                           u.no_telepon AS no_telepon_customer, u.alamat AS alamat_customer,
                           pu.nama AS nama_petugas
                    FROM penyewaans p
                    LEFT JOIN users u ON u.id = p.user_id
                    LEFT JOIN users pu ON pu.id = p.processed_by
                    WHERE p.id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _penyewaanId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Data penyewaan tidak ditemukan!", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                DataRow row = dt.Rows[0];

                string kode = row["kode_penyewaan"]?.ToString() ?? "-";
                string status = row["status"]?.ToString() ?? "pending";

                lblJudulKode.Text = kode;
                lblStatusDetail.Text = FormatStatusLabel(status);
                lblStatusDetail.ForeColor = GetStatusColor(status);
                lblStatusDetail.BackColor = Color.Transparent;

                string tglPengajuan = row["tanggal_pengajuan"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_pengajuan"]).ToString("dd/MM/yyyy") : "-";
                string tglMulai = row["tanggal_mulai"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_mulai"]).ToString("dd/MM/yyyy") : "-";
                string tglSelesai = row["tanggal_selesai"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_selesai"]).ToString("dd/MM/yyyy") : "-";

                lblInfoPengajuan.Text = $"Tanggal Pengajuan: {tglPengajuan}";
                lblInfoPeriode.Text = $"Periode Sewa: {tglMulai} - {tglSelesai}";
                lblInfoHari.Text = $"Total Hari: {(row["total_hari"] != DBNull.Value ? row["total_hari"].ToString() : "0")}";
                lblInfoPetugas.Text = $"Petugas: {(row["nama_petugas"]?.ToString() ?? "-")}";

                lblCustNama.Text = $"Nama: {(row["nama_customer"]?.ToString() ?? "-")}";
                lblCustEmail.Text = $"Email: {(row["email_customer"]?.ToString() ?? "-")}";
                lblCustTelepon.Text = $"Telepon: {(row["no_telepon_customer"]?.ToString() ?? "-")}";
                lblCustAlamat.Text = $"Alamat: {(row["alamat_customer"]?.ToString() ?? "-")}";

                decimal subtotal = row["subtotal"] != DBNull.Value ? Convert.ToDecimal(row["subtotal"]) : 0m;
                decimal denda = row["denda"] != DBNull.Value ? Convert.ToDecimal(row["denda"]) : 0m;
                decimal total = row["total"] != DBNull.Value ? Convert.ToDecimal(row["total"]) : 0m;

                lblSubtotal.Text = "Subtotal: Rp " + subtotal.ToString("N0");
                lblDenda.Text = "Denda: Rp " + denda.ToString("N0");
                lblTotal.Text = "Total: Rp " + total.ToString("N0");

                string catatan = row["catatan"]?.ToString();
                lblCatatan.Text = string.IsNullOrWhiteSpace(catatan) ? "Catatan: -" : $"Catatan: {catatan}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat detail penyewaan: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDetailItems()
        {
            InitializeGridView();

            try
            {
                string query = @"
                    SELECT dp.jumlah, dp.harga_sewa, dp.subtotal, dp.kondisi_sebelum,
                           a.kode_alat, a.nama_alat
                    FROM detail_penyewaans dp
                    LEFT JOIN alat_proyeks a ON a.id = dp.alat_id
                    WHERE dp.penyewaan_id = @id
                    ORDER BY a.nama_alat ASC";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _penyewaanId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                guna2DataGridView1.Rows.Clear();

                if (dt.Rows.Count == 0)
                {
                    guna2DataGridView1.Rows.Add("-", "-", "-", "-");
                    return;
                }

                foreach (DataRow row in dt.Rows)
                {
                    string namaAlat = row["nama_alat"]?.ToString() ?? "-";
                    string kodeAlat = row["kode_alat"]?.ToString() ?? "-";

                    guna2DataGridView1.Rows.Add(
                        $"{kodeAlat} - {namaAlat}",
                        row["jumlah"] != DBNull.Value ? row["jumlah"].ToString() : "0",
                        "Rp " + (row["harga_sewa"] != DBNull.Value ? Convert.ToDecimal(row["harga_sewa"]).ToString("N0") : "0"),
                        "Rp " + (row["subtotal"] != DBNull.Value ? Convert.ToDecimal(row["subtotal"]).ToString("N0") : "0")
                    );
                }
            }
            catch (Exception ex)
            {
                guna2DataGridView1.Rows.Clear();
                guna2DataGridView1.Rows.Add("-", "-", "-", "Tabel detail_penyewaans belum tersedia");
                System.Diagnostics.Debug.WriteLine($"Error load detail item: {ex.Message}");
            }
        }

        private void InitializeGridView()
        {
            guna2DataGridView1.Columns.Clear();

            guna2DataGridView1.Columns.Add("Alat", "Alat");
            guna2DataGridView1.Columns.Add("Jumlah", "Jumlah");
            guna2DataGridView1.Columns.Add("Harga", "Harga Sewa");
            guna2DataGridView1.Columns.Add("SubtotalItem", "Subtotal");

            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;

            guna2DataGridView1.Columns["Alat"].MinimumWidth = 300;
            guna2DataGridView1.Columns["Jumlah"].Width = 90;
            guna2DataGridView1.Columns["Harga"].Width = 130;
            guna2DataGridView1.Columns["SubtotalItem"].Width = 130;

            guna2DataGridView1.Columns["Jumlah"].FillWeight = 25;
            guna2DataGridView1.Columns["Harga"].FillWeight = 35;
            guna2DataGridView1.Columns["SubtotalItem"].FillWeight = 35;
            guna2DataGridView1.Columns["Alat"].FillWeight = 100;
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

        private Color GetStatusColor(string status)
        {
            switch (status)
            {
                case "pending": return Color.FromArgb(241, 196, 15);
                case "disetujui": return Color.FromArgb(52, 152, 219);
                case "menunggu_pembayaran": return Color.FromArgb(230, 126, 34);
                case "dibayar": return Color.FromArgb(155, 89, 182);
                case "sedang_disewa": return Color.FromArgb(241, 196, 15);
                case "selesai": return Color.FromArgb(46, 204, 113);
                case "ditolak": return Color.FromArgb(231, 76, 60);
                case "dibatalkan": return Color.FromArgb(127, 140, 141);
                default: return Color.FromArgb(52, 152, 219);
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}