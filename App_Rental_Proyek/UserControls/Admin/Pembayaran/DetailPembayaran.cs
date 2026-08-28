using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.Pembayaran
{
    public partial class DetailPembayaran : Form
    {
        private ulong _pembayaranId;
        private string _buktiPath = "";

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
                    SELECT pm.kode_pembayaran, pm.penyewaan_id, pm.tanggal_pembayaran, pm.jumlah,
                           pm.metode_pembayaran, pm.bukti_pembayaran, pm.status,
                           pm.diverifikasi_oleh, pm.tanggal_verifikasi, pm.catatan,
                           p.kode_penyewaan, p.total AS total_sewa, p.status AS status_penyewaan,
                           u.nama AS nama_customer, u.email AS email_customer,
                           u.no_telepon AS no_telepon_customer, u.alamat AS alamat_customer,
                           pt.nama AS nama_verifikator
                    FROM pembayarans pm
                    LEFT JOIN penyewaans p ON p.id = pm.penyewaan_id
                    LEFT JOIN users u ON u.id = p.user_id
                    LEFT JOIN users pt ON pt.id = pm.diverifikasi_oleh
                    WHERE pm.id = @id";

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

                lblJudulKode.Text = kode;
                lblStatusDetail.Text = FormatStatusLabel(status);
                lblStatusDetail.ForeColor = GetStatusColor(status);
                lblStatusDetail.BackColor = Color.Transparent;

                string tglBayar = row["tanggal_pembayaran"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_pembayaran"]).ToString("dd/MM/yyyy") : "-";

                lblInfoTanggal.Text = $"Tanggal Bayar: {tglBayar}";
                lblInfoMetode.Text = "Metode: " + FormatMetodeLabel(row["metode_pembayaran"]?.ToString() ?? "");
                lblInfoNominal.Text = "Nominal: Rp " + (row["jumlah"] != DBNull.Value ? Convert.ToDecimal(row["jumlah"]).ToString("N0") : "0");
                lblInfoPenyewaan.Text = $"Kode Sewa: {(row["kode_penyewaan"]?.ToString() ?? "-")}";
                lblInfoTotal.Text = "Total Sewa: Rp " + (row["total_sewa"] != DBNull.Value ? Convert.ToDecimal(row["total_sewa"]).ToString("N0") : "0");
                lblInfoStatusSewa.Text = "Status Sewa: " + FormatStatusPenyewaan(row["status_penyewaan"]?.ToString() ?? "-");

                lblCustNama.Text = $"Nama: {(row["nama_customer"]?.ToString() ?? "-")}";
                lblCustEmail.Text = $"Email: {(row["email_customer"]?.ToString() ?? "-")}";
                lblCustTelepon.Text = $"Telepon: {(row["no_telepon_customer"]?.ToString() ?? "-")}";
                lblCustAlamat.Text = $"Alamat: {(row["alamat_customer"]?.ToString() ?? "-")}";

                string verifikator = row["diverifikasi_oleh"] != DBNull.Value ? (row["nama_verifikator"]?.ToString() ?? "-") : "-";
                lblVerifikator.Text = $"Verifikator: {verifikator}";

                string tglVerifikasi = row["tanggal_verifikasi"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_verifikasi"]).ToString("dd/MM/yyyy HH:mm") : "-";
                lblTanggalVerifikasi.Text = $"Tanggal Verifikasi: {tglVerifikasi}";

                string catatan = row["catatan"]?.ToString();
                lblCatatan.Text = string.IsNullOrWhiteSpace(catatan) ? "Catatan: -" : $"Catatan: {catatan}";

                _buktiPath = row["bukti_pembayaran"]?.ToString() ?? "";
                LoadBukti();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat detail pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBukti()
        {
            picBukti.Image?.Dispose();
            picBukti.Image = null;
            lblBuktiInfo.Text = "-";

            if (string.IsNullOrWhiteSpace(_buktiPath))
            {
                lblBuktiInfo.Text = "Tidak ada bukti pembayaran diunggah.";
                return;
            }

            string path = BuktiPembayaranHelper.ResolvePath(_buktiPath);
            if (!File.Exists(path))
            {
                lblBuktiInfo.Text = $"File bukti tidak ditemukan:\n{_buktiPath}";
                lblBuktiInfo.ForeColor = Color.FromArgb(231, 76, 60);
                return;
            }

            if (!BuktiPembayaranHelper.IsImage(path))
            {
                lblBuktiInfo.Text = $"Bukti tersedia (bukan gambar):\n{path}";
                return;
            }

            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    picBukti.Image = Image.FromStream(fs);
                }
                lblBuktiInfo.Text = $"Bukti: {Path.GetFileName(path)}";
                lblBuktiInfo.ForeColor = Color.FromArgb(96, 110, 130);
            }
            catch (Exception ex)
            {
                lblBuktiInfo.Text = $"Gagal memuat gambar bukti: {ex.Message}";
                lblBuktiInfo.ForeColor = Color.FromArgb(231, 76, 60);
            }
        }

        private void btnBukaBukti_Click(object sender, EventArgs e)
        {
            string path = BuktiPembayaranHelper.ResolvePath(_buktiPath);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                MessageBox.Show("File bukti pembayaran tidak ditemukan.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal membuka bukti pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "pending": return "Menunggu";
                case "diverifikasi": return "Lunas";
                case "ditolak": return "Gagal";
                default: return status;
            }
        }

        private string FormatStatusPenyewaan(string status)
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

        private string FormatMetodeLabel(string metode)
        {
            switch (metode)
            {
                case "cash": return "Tunai (Cash)";
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