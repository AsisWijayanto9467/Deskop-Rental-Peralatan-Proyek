using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Pembayaran
{
    public partial class TolakPembayaran : Form
    {
        private ulong _pembayaranId;

        public TolakPembayaran(ulong pembayaranId)
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

        private void TolakPembayaran_Load(object sender, EventArgs e)
        {
            LoadSummary();
        }

        private void LoadSummary()
        {
            try
            {
                string query = @"
                    SELECT pb.kode_pembayaran, pb.jumlah, pb.metode_pembayaran,
                           ps.kode_penyewaan,
                           u.nama AS nama_customer
                    FROM pembayarans pb
                    LEFT JOIN penyewaans ps ON ps.id = pb.penyewaan_id
                    LEFT JOIN users u ON u.id = ps.user_id
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

                lblKodePembayaran.Text = row["kode_pembayaran"]?.ToString() ?? "-";
                lblKodePenyewaan.Text = row["kode_penyewaan"]?.ToString() ?? "-";
                lblCustomer.Text = row["nama_customer"]?.ToString() ?? "-";

                decimal jumlah = row["jumlah"] != DBNull.Value ? Convert.ToDecimal(row["jumlah"]) : 0m;
                lblJumlah.Text = "Rp " + jumlah.ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            string catatan = txtCatatan.Text.Trim();

            if (string.IsNullOrWhiteSpace(catatan))
            {
                MessageBox.Show("Catatan penolakan wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCatatan.Focus();
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Apakah Anda yakin ingin menolak pembayaran ini?\n\n" +
                "Status pembayaran akan berubah menjadi 'Ditolak'.",
                "Konfirmasi Penolakan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    ulong verifikatorId = SessionManager.GetCurrentUserId();

                    string updateQuery = @"
                        UPDATE pembayarans
                        SET status = 'ditolak',
                            diverifikasi_oleh = @verifikator_id,
                            tanggal_verifikasi = NOW(),
                            catatan = @catatan,
                            updated_at = NOW()
                        WHERE id = @id";

                    MySqlParameter[] parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@verifikator_id", verifikatorId == 0 ? (object)DBNull.Value : verifikatorId),
                        new MySqlParameter("@catatan", catatan),
                        new MySqlParameter("@id", _pembayaranId)
                    };

                    bool success = DatabaseConnection.ExecuteQuery(updateQuery, parameters) > 0;

                    if (success)
                    {
                        string aktivitas = $"Menolak pembayaran '{lblKodePembayaran.Text}' untuk penyewaan '{lblKodePenyewaan.Text}' dari {lblCustomer.Text} | Alasan: {catatan}";
                        ActivityLogHelper.LogForSession(SessionManager.GetCurrentUserId(),
                            aktivitas, "Pembayaran", _pembayaranId);

                        MessageBox.Show("Pembayaran berhasil ditolak.",
                            "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Gagal menolak pembayaran.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error menolak pembayaran: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}