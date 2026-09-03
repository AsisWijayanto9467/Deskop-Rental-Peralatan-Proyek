using App_Rental_Proyek.Config;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Pengembalian
{
    public partial class DetailPengembalian : Form
    {
        private ulong _pengembalianId;

        public DetailPengembalian(ulong pengembalianId)
        {
            InitializeComponent();
            _pengembalianId = pengembalianId;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void DetailPengembalian_Load(object sender, EventArgs e)
        {
            LoadDetail();
        }

        private void LoadDetail()
        {
            try
            {
                string query = @"
                    SELECT pg.tanggal_pengembalian, pg.diterima_oleh, pg.kondisi_alat,
                           pg.terlambat_hari, pg.catatan, pg.status, pg.created_at,
                           p.kode_penyewaan,
                           u.nama AS nama_customer,
                           pu.nama AS nama_diterima
                    FROM pengembalians pg
                    LEFT JOIN penyewaans p ON p.id = pg.penyewaan_id
                    LEFT JOIN users u ON u.id = p.user_id
                    LEFT JOIN users pu ON pu.id = pg.diterima_oleh
                    WHERE pg.id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _pengembalianId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Data pengembalian tidak ditemukan!", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                DataRow row = dt.Rows[0];

                string status = row["status"]?.ToString() ?? "menunggu_inspeksi";

                lblJudulKode.Text = $"Kode Sewa: {(row["kode_penyewaan"]?.ToString() ?? "-")}";
                lblStatusDetail.Text = FormatStatusLabel(status);
                lblStatusDetail.ForeColor = GetStatusColor(status);
                lblStatusDetail.BackColor = Color.Transparent;

                string tgl = row["tanggal_pengembalian"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_pengembalian"]).ToString("dd/MM/yyyy") : "-";
                lblInfoTanggal.Text = $"Tanggal Pengembalian: {tgl}";

                int terlambat = row["terlambat_hari"] != DBNull.Value ? Convert.ToInt32(row["terlambat_hari"]) : 0;
                lblInfoTerlambat.Text = terlambat > 0
                    ? $"Terlambat: {terlambat} hari"
                    : "Terlambat: Tidak terlambat";

                if (terlambat > 0)
                {
                    lblInfoTerlambat.ForeColor = Color.FromArgb(231, 76, 60);
                }

                lblInfoDiterima.Text = $"Diterima Oleh: {(row["nama_diterima"]?.ToString() ?? "-")}";

                string createdAt = row["created_at"] != DBNull.Value
                    ? Convert.ToDateTime(row["created_at"]).ToString("dd/MM/yyyy HH:mm") : "-";
                lblInfoDibuat.Text = $"Dibuat: {createdAt}";

                lblCustNama.Text = $"Nama: {(row["nama_customer"]?.ToString() ?? "-")}";
                lblCustKode.Text = $"Kode Sewa: {(row["kode_penyewaan"]?.ToString() ?? "-")}";

                string kondisi = row["kondisi_alat"]?.ToString();
                lblKondisi.Text = string.IsNullOrWhiteSpace(kondisi) ? "-" : kondisi;

                string catatan = row["catatan"]?.ToString();
                lblCatatan.Text = string.IsNullOrWhiteSpace(catatan) ? "-" : catatan;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat detail pengembalian: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "menunggu_inspeksi": return "Menunggu Inspeksi";
                case "diterima": return "Diterima";
                case "perlu_perbaikan": return "Perlu Perbaikan";
                case "ditolak": return "Ditolak";
                default: return status;
            }
        }

        private Color GetStatusColor(string status)
        {
            switch (status)
            {
                case "menunggu_inspeksi": return Color.FromArgb(241, 196, 15);
                case "diterima": return Color.FromArgb(46, 204, 113);
                case "perlu_perbaikan": return Color.FromArgb(241, 196, 15);
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
