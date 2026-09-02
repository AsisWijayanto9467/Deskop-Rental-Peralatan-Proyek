using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Penyewaan
{
    public partial class TolakPenyewaan : Form
    {
        private ulong _penyewaanId;

        public TolakPenyewaan(ulong penyewaanId)
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

        private void TolakPenyewaan_Load(object sender, EventArgs e)
        {
            LoadSummary();
        }

        private void LoadSummary()
        {
            try
            {
                string query = @"
                    SELECT p.kode_penyewaan, p.tanggal_mulai, p.tanggal_selesai, p.total,
                           u.nama AS nama_customer
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

                lblKode.Text = row["kode_penyewaan"]?.ToString() ?? "-";
                lblCustomer.Text = row["nama_customer"]?.ToString() ?? "-";

                string tglMulai = row["tanggal_mulai"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_mulai"]).ToString("dd/MM/yyyy") : "-";
                string tglSelesai = row["tanggal_selesai"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_selesai"]).ToString("dd/MM/yyyy") : "-";

                lblPeriode.Text = $"{tglMulai} - {tglSelesai}";
                lblTotal.Text = "Rp " + (row["total"] != DBNull.Value
                    ? Convert.ToDecimal(row["total"]).ToString("N0") : "0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data penyewaan: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            string alasan = txtAlasan.Text.Trim();

            if (string.IsNullOrWhiteSpace(alasan))
            {
                MessageBox.Show("Alasan penolakan wajib diisi!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAlasan.Focus();
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Apakah Anda yakin ingin menolak pengajuan penyewaan ini?\n\n" +
                "Status akan berubah menjadi 'Ditolak'.",
                "Konfirmasi Penolakan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    ulong processedBy = SessionManager.GetCurrentUserId();

                    string query = @"
                        UPDATE penyewaans
                        SET status = 'ditolak',
                            alasan_penolakan = @alasan,
                            processed_by = @processed_by,
                            updated_at = NOW()
                        WHERE id = @id";

                    MySqlParameter[] parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@alasan", alasan),
                        new MySqlParameter("@processed_by", processedBy == 0 ? (object)DBNull.Value : processedBy),
                        new MySqlParameter("@id", _penyewaanId)
                    };

                    bool success = DatabaseConnection.ExecuteQuery(query, parameters) > 0;

                    if (success)
                    {
                        string aktivitas = $"Menolak pengajuan penyewaan '{lblKode.Text}' dari {lblCustomer.Text} | Alasan: {alasan}";
                        ActivityLogHelper.LogForSession(SessionManager.GetCurrentUserId(),
                            aktivitas, "Penyewaan", _penyewaanId);

                        MessageBox.Show("Pengajuan penyewaan berhasil ditolak.",
                            "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Gagal menolak penyewaan.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error menolak penyewaan: {ex.Message}", "Error",
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