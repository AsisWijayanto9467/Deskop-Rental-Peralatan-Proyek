using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.Denda
{
    public partial class DendaDetail : Form
    {
        private readonly DendaViewItem _item;

        public DendaDetail(DendaViewItem item)
        {
            InitializeComponent();
            _item = item;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void DendaDetail_Load(object sender, EventArgs e)
        {
            LoadDetail();
        }

        private void LoadDetail()
        {
            try
            {
                if (_item?.Denda == null)
                {
                    MessageBox.Show("Data denda tidak ditemukan!", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                DendaModel denda = _item.Denda;
                string status = denda.Status;

                lblJudulKode.Text = $"Kode Sewa: {_item.KodeSewa}";
                lblStatusDetail.Text = FormatStatusLabel(status);
                lblStatusDetail.ForeColor = GetStatusColor(status);
                lblStatusDetail.BackColor = Color.Transparent;

                lblInfoJenis.Text = $"Jenis Denda: {FormatJenisLabel(denda.JenisDenda)}";
                lblInfoNominal.Text = $"Nominal Denda: Rp {denda.Jumlah.ToString("N0")}";
                lblInfoDibuat.Text = $"Dibuat: {(denda.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-")}";

                lblCustNama.Text = $"Nama: {_item.NamaCustomer}";

                LoadPenyewaanInfo(denda.PenyewaanId);

                lblAlasan.Text = string.IsNullOrWhiteSpace(denda.Alasan) ? "-" : denda.Alasan;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat detail denda: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPenyewaanInfo(ulong penyewaanId)
        {
            try
            {
                if (penyewaanId == 0) return;

                string query = @"
                    SELECT p.kode_penyewaan, p.tanggal_mulai, p.tanggal_selesai, p.total,
                           u.nama AS nama_customer, u.no_telepon,
                           (SELECT pg.tanggal_pengembalian FROM pengembalians pg
                            WHERE pg.penyewaan_id = p.id ORDER BY pg.id DESC LIMIT 1) AS tgl_pengembalian
                    FROM penyewaans p
                    LEFT JOIN users u ON u.id = p.user_id
                    WHERE p.id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", penyewaanId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                if (dt.Rows.Count == 0) return;

                DataRow row = dt.Rows[0];

                lblSewaMulai.Text = $"Tanggal Mulai: {FormatTanggal(row["tanggal_mulai"])}";
                lblSewaSelesai.Text = $"Tanggal Selesai: {FormatTanggal(row["tanggal_selesai"])}";
                lblSewaTotal.Text = "Total Biaya Sewa: Rp " +
                    (row["total"] != DBNull.Value ? Convert.ToDecimal(row["total"]).ToString("N0") : "0");

                lblSewaKembali.Text = "Tanggal Pengembalian: " + FormatTanggal(row["tgl_pengembalian"]);

                string cust = row["nama_customer"]?.ToString();
                if (!string.IsNullOrWhiteSpace(cust) && cust != "-")
                {
                    lblCustNama.Text = $"Nama: {cust}";
                }

                lblCustTelp.Text = "No. Telepon: " +
                    (string.IsNullOrWhiteSpace(row["no_telepon"]?.ToString())
                        ? "-"
                        : row["no_telepon"].ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error detail penyewaan denda: {ex.Message}");
            }
        }

        private string FormatTanggal(object value)
        {
            return value == null || value == DBNull.Value
                ? "-"
                : Convert.ToDateTime(value).ToString("dd/MM/yyyy");
        }

        private string FormatJenisLabel(string jenis)
        {
            switch (jenis)
            {
                case "terlambat": return "Keterlambatan";
                case "kerusakan": return "Kerusakan";
                case "kehilangan": return "Kehilangan";
                case "kekurangan": return "Kekurangan Komponen";
                default: return string.IsNullOrWhiteSpace(jenis) ? "-" : jenis;
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "pending": return "Belum Dibayar";
                case "dibayar": return "Dibayar";
                case "ditangguhkan": return "Ditangguhkan";
                default: return status;
            }
        }

        private Color GetStatusColor(string status)
        {
            switch (status)
            {
                case "pending": return Color.FromArgb(230, 126, 34);
                case "dibayar": return Color.FromArgb(46, 204, 113);
                case "ditangguhkan": return Color.FromArgb(52, 152, 219);
                default: return Color.FromArgb(23, 59, 99);
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}