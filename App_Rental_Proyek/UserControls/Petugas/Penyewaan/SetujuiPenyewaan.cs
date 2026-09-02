using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Penyewaan
{
    public partial class SetujuiPenyewaan : Form
    {
        private ulong _penyewaanId;

        public SetujuiPenyewaan(ulong penyewaanId)
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

        private void SetujuiPenyewaan_Load(object sender, EventArgs e)
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

        private void Persetujuan(ulong id, string status, string aktivitas, string? catatan = null)
        {
            try
            {
                ulong processedBy = SessionManager.GetCurrentUserId();

                string catQuery;
                MySqlParameter[] parameters;

                if (catatan != null)
                {
                    catQuery = @"
                        UPDATE penyewaans
                        SET status = @status,
                            catatan = @catatan,
                            processed_by = @processed_by,
                            updated_at = NOW()
                        WHERE id = @id";

                    parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@status", status),
                        new MySqlParameter("@catatan", (object)catatan ?? DBNull.Value),
                        new MySqlParameter("@processed_by", processedBy == 0 ? (object)DBNull.Value : processedBy),
                        new MySqlParameter("@id", id)
                    };
                }
                else
                {
                    catQuery = @"
                        UPDATE penyewaans
                        SET status = @status,
                            processed_by = @processed_by,
                            updated_at = NOW()
                        WHERE id = @id";

                    parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@status", status),
                        new MySqlParameter("@processed_by", processedBy == 0 ? (object)DBNull.Value : processedBy),
                        new MySqlParameter("@id", id)
                    };
                }

                bool success = DatabaseConnection.ExecuteQuery(catQuery, parameters) > 0;

                if (success)
                {
                    ActivityLogHelper.LogForSession(SessionManager.GetCurrentUserId(),
                        aktivitas, "Penyewaan", id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error menyetujui penyewaan: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            string catatan = txtCatatan.Text.Trim();

            DialogResult confirm = MessageBox.Show(
                "Apakah Anda yakin ingin menyetujui pengajuan penyewaan ini?\n\n" +
                "Status akan berubah menjadi 'Disetujui'.",
                "Konfirmasi Persetujuan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                string kode = lblKode.Text;
                string customer = lblCustomer.Text;

                string aktivitas = $"Menyetujui pengajuan penyewaan '{kode}' dari {customer}";
                if (!string.IsNullOrWhiteSpace(catatan))
                {
                    aktivitas += $" | Catatan: {catatan}";
                }

                Persetujuan(_penyewaanId, "disetujui", aktivitas, catatan);

                MessageBox.Show("Pengajuan penyewaan berhasil disetujui!",
                    "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}