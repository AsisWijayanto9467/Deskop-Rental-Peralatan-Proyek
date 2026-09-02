using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Penyewaan
{
    public partial class CatatanPenyewaan : Form
    {
        private ulong _penyewaanId;

        public CatatanPenyewaan(ulong penyewaanId)
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

        private void CatatanPenyewaan_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT kode_penyewaan, catatan FROM penyewaans WHERE id = @id";

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

                lblInfoKode.Text = $"Kode Sewa: {row["kode_penyewaan"]?.ToString() ?? "-"}";
                txtCatatan.Text = row["catatan"]?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data penyewaan: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            string catatan = txtCatatan.Text.Trim();

            try
            {
                ulong processedBy = SessionManager.GetCurrentUserId();

                string query = @"
                    UPDATE penyewaans
                    SET catatan = @catatan,
                        processed_by = @processed_by,
                        updated_at = NOW()
                    WHERE id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@catatan", (object)catatan ?? DBNull.Value),
                    new MySqlParameter("@processed_by", processedBy == 0 ? (object)DBNull.Value : processedBy),
                    new MySqlParameter("@id", _penyewaanId)
                };

                bool success = DatabaseConnection.ExecuteQuery(query, parameters) > 0;

                if (success)
                {
                    string aktivitas = $"Memberikan catatan pada penyewaan '{lblInfoKode.Text.Replace("Kode Sewa: ", "")}'";
                    if (!string.IsNullOrWhiteSpace(catatan))
                    {
                        aktivitas += $" | Catatan: {catatan}";
                    }

                    ActivityLogHelper.LogForSession(SessionManager.GetCurrentUserId(),
                        aktivitas, "Penyewaan", _penyewaanId);

                    MessageBox.Show("Catatan penyewaan berhasil diperbarui!",
                        "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gagal memperbarui catatan penyewaan.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memperbarui catatan: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}