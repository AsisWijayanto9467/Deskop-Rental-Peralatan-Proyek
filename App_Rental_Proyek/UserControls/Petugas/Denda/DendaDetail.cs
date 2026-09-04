using System;
using System.Drawing;
using System.Windows.Forms;
using App_Rental_Proyek.Model;

namespace App_Rental_Proyek.UserControls.Petugas.Denda
{
    public partial class DendaDetail : Form
    {
        private DendaModel _denda;
        private string _kodePenyewaan;
        private string _namaCustomer;
        private string _kodePembayaran;
        private string _statusPembayaran;
        private string _buktiPembayaran;
        private string _kodePengembalian;

        public DendaDetail(DendaModel denda, string kodePenyewaan, string namaCustomer,
            string kodePembayaran, string statusPembayaran, string buktiPembayaran, string kodePengembalian)
        {
            InitializeComponent();
            _denda = denda;
            _kodePenyewaan = kodePenyewaan;
            _namaCustomer = namaCustomer;
            _kodePembayaran = kodePembayaran;
            _statusPembayaran = statusPembayaran;
            _buktiPembayaran = buktiPembayaran;
            _kodePengembalian = kodePengembalian;

            LoadDetail();
        }

        private void LoadDetail()
        {
            lblKodePenyewaan.Text = _kodePenyewaan;
            lblNamaCustomer.Text = _namaCustomer;
            lblJenisDenda.Text = _denda.JenisDenda;
            lblJumlah.Text = $"Rp {_denda.Jumlah:N0}";
            lblStatus.Text = _denda.Status;
            lblAlasan.Text = _denda.Alasan;
            lblKodePengembalian.Text = _kodePengembalian;
            lblKodePembayaran.Text = _kodePembayaran;
            lblStatusPembayaran.Text = _statusPembayaran;
            lblTanggalDibuat.Text = _denda.CreatedAt?.ToString("dd MMMM yyyy HH:mm") ?? "-";
            lblTanggalUpdate.Text = _denda.UpdatedAt?.ToString("dd MMMM yyyy HH:mm") ?? "-";

            SetStatusColor(lblStatus, _denda.Status);
            SetStatusColor(lblStatusPembayaran, _statusPembayaran);

            if (string.IsNullOrEmpty(_buktiPembayaran))
            {
                lblBuktiInfo.Text = "Belum ada bukti pembayaran";
                lblBuktiInfo.ForeColor = Color.Gray;
            }
            else
            {
                lblBuktiInfo.Text = _buktiPembayaran;
                lblBuktiInfo.ForeColor = Color.FromArgb(23, 59, 99);
            }
        }

        private void SetStatusColor(Guna.UI2.WinForms.Guna2HtmlLabel label, string status)
        {
            switch (status.ToLower())
            {
                case "pending":
                    label.ForeColor = Color.FromArgb(230, 126, 34);
                    break;
                case "dibayar":
                case "diverifikasi":
                    label.ForeColor = Color.FromArgb(46, 204, 113);
                    break;
                case "ditangguhkan":
                    label.ForeColor = Color.FromArgb(52, 152, 219);
                    break;
                case "ditolak":
                    label.ForeColor = Color.FromArgb(231, 76, 60);
                    break;
                default:
                    label.ForeColor = Color.Gray;
                    break;
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
                this.DialogResult = DialogResult.Cancel;
            base.OnFormClosing(e);
        }
    }
}
