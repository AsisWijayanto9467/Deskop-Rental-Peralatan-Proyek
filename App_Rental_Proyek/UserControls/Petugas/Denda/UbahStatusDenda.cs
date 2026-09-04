using System;
using System.Windows.Forms;
using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;

namespace App_Rental_Proyek.UserControls.Petugas.Denda
{
    public partial class UbahStatusDenda : Form
    {
        private DendaModel _denda;
        private string _kodePenyewaan;
        private string _namaCustomer;

        public UbahStatusDenda(DendaModel denda, string kodePenyewaan, string namaCustomer)
        {
            InitializeComponent();
            _denda = denda;
            _kodePenyewaan = kodePenyewaan;
            _namaCustomer = namaCustomer;

            LoadData();
        }

        private void LoadData()
        {
            lblKodePenyewaan.Text = _kodePenyewaan;
            lblNamaCustomer.Text = _namaCustomer;
            lblJenisDenda.Text = _denda.JenisDenda;
            lblJumlah.Text = $"Rp {_denda.Jumlah:N0}";
            lblStatusSaat.Text = _denda.Status;

            cbStatusBaru.Items.Clear();
            cbStatusBaru.Items.Add("dibayar");
            cbStatusBaru.Items.Add("ditangguhkan");
            cbStatusBaru.SelectedIndex = 0;
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            string statusBaru = cbStatusBaru.SelectedItem?.ToString() ?? "";
            string catatan = txtCatatan.Text.Trim();

            if (string.IsNullOrEmpty(statusBaru))
            {
                MessageBox.Show("Pilih status baru denda.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (statusBaru == _denda.Status)
            {
                MessageBox.Show("Status baru harus berbeda dengan status saat ini.", "Validasi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                MySqlConnection connection = null;
                MySqlTransaction transaction = null;

                try
                {
                    connection = DatabaseConnection.GetConnection();
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    string updateQuery = @"
                        UPDATE dendas
                        SET status = @statusBaru,
                            updated_at = NOW()
                        WHERE id = @dendaId";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@statusBaru", statusBaru);
                        cmd.Parameters.AddWithValue("@dendaId", _denda.Id);
                        cmd.ExecuteNonQuery();
                    }

                    ulong currentUserId = SessionManager.GetCurrentUserId();
                    if (currentUserId > 0)
                    {
                        string logQuery = @"
                            INSERT INTO activity_logs
                            (user_id, aktivitas, modul, referensi_id, ip_address, created_at)
                            VALUES (@userId, @aktivitas, @modul, @referensiId, @ipAddress, NOW())";

                        using (MySqlCommand logCmd = new MySqlCommand(logQuery, connection, transaction))
                        {
                            logCmd.Parameters.AddWithValue("@userId", currentUserId);
                            logCmd.Parameters.AddWithValue("@aktivitas",
                                $"Ubah status denda {_denda.JenisDenda} dari '{_denda.Status}' menjadi '{statusBaru}'");
                            logCmd.Parameters.AddWithValue("@modul", "Manajemen Denda");
                            logCmd.Parameters.AddWithValue("@referensiId", _denda.Id);
                            logCmd.Parameters.AddWithValue("@ipAddress", GetIpAddress());
                            logCmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();

                    MessageBox.Show($"Status denda berhasil diubah menjadi: {statusBaru}", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    throw;
                }
                finally
                {
                    if (connection?.State == System.Data.ConnectionState.Open) connection.Close();
                    connection?.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal mengubah status denda: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private string GetIpAddress()
        {
            try
            {
                string host = System.Net.Dns.GetHostName();
                System.Net.IPAddress[] addresses = System.Net.Dns.GetHostAddresses(host);

                foreach (System.Net.IPAddress ip in addresses)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }

                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
                this.DialogResult = DialogResult.Cancel;
            base.OnFormClosing(e);
        }
    }
}
