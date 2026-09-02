using App_Rental_Proyek.Config;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.PersiapanAlat
{
    public partial class DetailPersiapan : Form
    {
        private ulong _penyewaanId;

        public DetailPersiapan(ulong penyewaanId)
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
            this.Size = new Size(650, 550);
        }

        private void DetailPersiapan_Load(object sender, EventArgs e)
        {
            LoadDetail();
        }

        private void LoadDetail()
        {
            try
            {
                string query = @"
                    SELECT p.id, p.kode_penyewaan, p.tanggal_pengajuan,
                           p.tanggal_mulai, p.tanggal_selesai, p.total_hari,
                           p.subtotal, p.denda, p.total, p.status, p.catatan,
                           u.nama AS nama_customer, u.email AS email_customer,
                           u.no_telepon AS no_telepon_customer, u.alamat AS alamat_customer,
                           (SELECT COUNT(*) FROM detail_penyewaans dp WHERE dp.penyewaan_id = p.id) AS jumlah_alat
                    FROM penyewaans p
                    LEFT JOIN users u ON u.id = p.user_id
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

                lblKodePenyewaan.Text = row["kode_penyewaan"]?.ToString() ?? "-";
                lblStatus.Text = FormatStatusLabel(row["status"]?.ToString() ?? "dibayar");
                lblStatus.ForeColor = GetStatusColor(row["status"]?.ToString() ?? "dibayar");

                lblCustomer.Text = row["nama_customer"]?.ToString() ?? "-";
                lblEmail.Text = row["email_customer"]?.ToString() ?? "-";
                lblTelepon.Text = row["no_telepon_customer"]?.ToString() ?? "-";
                lblAlamat.Text = row["alamat_customer"]?.ToString() ?? "-";

                string tglMulai = row["tanggal_mulai"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_mulai"]).ToString("dd/MM/yyyy") : "-";
                string tglSelesai = row["tanggal_selesai"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_selesai"]).ToString("dd/MM/yyyy") : "-";

                lblTanggalMulai.Text = $"Tanggal Mulai: {tglMulai}";
                lblTanggalSelesai.Text = $"Tanggal Selesai: {tglSelesai}";
                lblTotalHari.Text = $"Total Hari: {row["total_hari"]} hari";
                lblJumlahAlat.Text = $"Jumlah Alat: {row["jumlah_alat"]} item";

                decimal total = row["total"] != DBNull.Value ? Convert.ToDecimal(row["total"]) : 0m;
                lblTotal.Text = $"Total Biaya: Rp {total.ToString("N0")}";

                string catatan = row["catatan"]?.ToString();
                lblCatatan.Text = string.IsNullOrWhiteSpace(catatan) ? "Catatan: -" : $"Catatan: {catatan}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat detail penyewaan: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "dibayar": return "✅ Siap Disiapkan";
                case "sedang_disewa": return "🚛 Sedang Disewa";
                default: return status;
            }
        }

        private Color GetStatusColor(string status)
        {
            switch (status)
            {
                case "dibayar": return Color.FromArgb(46, 204, 113);
                case "sedang_disewa": return Color.FromArgb(52, 152, 219);
                default: return Color.FromArgb(96, 110, 130);
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}