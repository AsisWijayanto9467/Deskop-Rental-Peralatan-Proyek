using App_Rental_Proyek.Config;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Pembayaran
{
    public partial class DetailPembayaran : Form
    {
        private ulong _pembayaranId;

        public DetailPembayaran(ulong pembayaranId)
        {
            InitializeComponent();
            _pembayaranId = pembayaranId;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(600, 700);
        }

        private void DetailPembayaran_Load(object sender, EventArgs e)
        {
            LoadDetail();
        }

        private void LoadDetail()
        {
            try
            {
                string query = @"
                    SELECT pb.id, pb.penyewaan_id, pb.kode_pembayaran, pb.tanggal_pembayaran,
                           pb.jumlah, pb.metode_pembayaran, pb.bukti_pembayaran, pb.status,
                           pb.diverifikasi_oleh, pb.tanggal_verifikasi, pb.catatan,
                           pb.created_at, pb.updated_at,
                           ps.kode_penyewaan, ps.total AS total_sewa, ps.status AS status_penyewaan,
                           u.nama AS nama_customer, u.email AS email_customer,
                           u.no_telepon AS no_telepon_customer, u.alamat AS alamat_customer,
                           v.nama AS nama_verifikator
                    FROM pembayarans pb
                    LEFT JOIN penyewaans ps ON ps.id = pb.penyewaan_id
                    LEFT JOIN users u ON u.id = ps.user_id
                    LEFT JOIN users v ON v.id = pb.diverifikasi_oleh
                    WHERE pb.id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _pembayaranId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Data pembayaran tidak ditemukan!", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                DataRow row = dt.Rows[0];

                string kode = row["kode_pembayaran"]?.ToString() ?? "-";
                string status = row["status"]?.ToString() ?? "pending";
                string metode = row["metode_pembayaran"]?.ToString() ?? "cash";

                lblKodePembayaran.Text = kode;
                lblStatus.Text = FormatStatusLabel(status);
                lblStatus.ForeColor = GetStatusColor(status);
                lblStatus.BackColor = Color.Transparent;

                lblKodePenyewaan.Text = row["kode_penyewaan"]?.ToString() ?? "-";
                lblTotalSewa.Text = "Total Sewa: Rp " + (row["total_sewa"] != DBNull.Value ? Convert.ToDecimal(row["total_sewa"]).ToString("N0") : "0");
                lblStatusPenyewaan.Text = "Status Penyewaan: " + FormatStatusPenyewaan(row["status_penyewaan"]?.ToString() ?? "");

                lblCustomer.Text = row["nama_customer"]?.ToString() ?? "-";
                lblEmail.Text = row["email_customer"]?.ToString() ?? "-";
                lblTelepon.Text = row["no_telepon_customer"]?.ToString() ?? "-";
                lblAlamat.Text = row["alamat_customer"]?.ToString() ?? "-";

                string tglBayar = row["tanggal_pembayaran"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_pembayaran"]).ToString("dd/MM/yyyy") : "-";
                lblTanggalBayar.Text = $"Tanggal Pembayaran: {tglBayar}";

                decimal jumlah = row["jumlah"] != DBNull.Value ? Convert.ToDecimal(row["jumlah"]) : 0m;
                lblJumlah.Text = $"Jumlah: Rp {jumlah.ToString("N0")}";

                lblMetode.Text = $"Metode: {FormatMetodePembayaran(metode)}";

                if (!string.IsNullOrEmpty(row["bukti_pembayaran"]?.ToString()))
                {
                    lblBukti.Text = $"Bukti: {row["bukti_pembayaran"]}";
                    btnLihatBukti.Visible = true;
                    btnLihatBukti.Tag = row["bukti_pembayaran"]?.ToString();
                }
                else
                {
                    lblBukti.Text = "Bukti: Tidak ada";
                    btnLihatBukti.Visible = false;
                }

                string tglVerif = row["tanggal_verifikasi"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_verifikasi"]).ToString("dd/MM/yyyy HH:mm") : "-";
                lblVerifikator.Text = $"Diverifikasi Oleh: {row["nama_verifikator"]?.ToString() ?? "-"}";
                lblTanggalVerif.Text = $"Tanggal Verifikasi: {tglVerif}";

                string catatan = row["catatan"]?.ToString();
                lblCatatan.Text = string.IsNullOrWhiteSpace(catatan) ? "Catatan: -" : $"Catatan: {catatan}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat detail pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLihatBukti_Click(object sender, EventArgs e)
        {
            if (btnLihatBukti.Tag == null) return;

            string fileName = btnLihatBukti.Tag.ToString();
            try
            {
                string buktiPath = System.IO.Path.Combine(
                    Application.StartupPath,
                    "Resources", "BuktiPembayaran",
                    fileName);

                if (System.IO.File.Exists(buktiPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = buktiPath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show($"File bukti pembayaran tidak ditemukan:\n{buktiPath}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error membuka bukti pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "pending": return "⏳ Pending";
                case "diverifikasi": return "✅ Diverifikasi";
                case "ditolak": return "❌ Ditolak";
                default: return status;
            }
        }

        private string FormatStatusPenyewaan(string status)
        {
            switch (status)
            {
                case "pending": return "Menunggu Konfirmasi";
                case "disetujui": return "Disetujui";
                case "menunggu_pembayaran": return "Menunggu Pembayaran";
                case "dibayar": return "Dibayar";
                case "sedang_disewa": return "Sedang Disewa";
                case "selesai": return "Selesai";
                case "ditolak": return "Ditolak";
                case "dibatalkan": return "Dibatalkan";
                default: return status;
            }
        }

        private string FormatMetodePembayaran(string metode)
        {
            switch (metode)
            {
                case "cash": return "Cash";
                case "transfer": return "Transfer Bank";
                case "qris": return "QRIS";
                default: return metode;
            }
        }

        private Color GetStatusColor(string status)
        {
            switch (status)
            {
                case "pending": return Color.FromArgb(241, 196, 15);
                case "diverifikasi": return Color.FromArgb(46, 204, 113);
                case "ditolak": return Color.FromArgb(231, 76, 60);
                default: return Color.FromArgb(52, 152, 219);
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}