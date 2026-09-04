using System;
using System.Drawing;
using System.Windows.Forms;
using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;

namespace App_Rental_Proyek.UserControls.Petugas.Denda
{
    public partial class VerifikasiPembayaran : Form
    {
        private DendaModel _denda;
        private string _kodePenyewaan;
        private string _namaCustomer;
        private string _kodePembayaran;
        private string _buktiPembayaran;

        public VerifikasiPembayaran(DendaModel denda, string kodePenyewaan, string namaCustomer,
            string kodePembayaran, string buktiPembayaran)
        {
            InitializeComponent();
            _denda = denda;
            _kodePenyewaan = kodePenyewaan;
            _namaCustomer = namaCustomer;
            _kodePembayaran = kodePembayaran;
            _buktiPembayaran = buktiPembayaran;

            LoadData();
        }

        private void LoadData()
        {
            lblKodePenyewaan.Text = _kodePenyewaan;
            lblNamaCustomer.Text = _namaCustomer;
            lblKodePembayaran.Text = _kodePembayaran;
            lblJumlahDenda.Text = $"Rp {_denda.Jumlah:N0}";
            lblJenisDenda.Text = _denda.JenisDenda;
            lblBuktiPembayaran.Text = _buktiPembayaran;

            cbStatus.Items.Clear();
            cbStatus.Items.Add("diverifikasi");
            cbStatus.Items.Add("ditolak");
            cbStatus.SelectedIndex = 0;
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            string status = cbStatus.SelectedItem?.ToString() ?? "diverifikasi";
            string catatan = txtCatatan.Text.Trim();

            if (string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Pilih status verifikasi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    string updatePembayaranQuery = @"
                        UPDATE pembayarans
                        SET status = @status,
                            diverifikasi_oleh = @diverifikasiOleh,
                            tanggal_verifikasi = NOW(),
                            catatan = @catatan,
                            updated_at = NOW()
                        WHERE kode_pembayaran = @kodePembayaran";

                    using (MySqlCommand cmd = new MySqlCommand(updatePembayaranQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@diverifikasiOleh", SessionManager.GetCurrentUserId());
                        cmd.Parameters.AddWithValue("@catatan", catatan);
                        cmd.Parameters.AddWithValue("@kodePembayaran", _kodePembayaran);
                        cmd.ExecuteNonQuery();
                    }

                    if (status == "diverifikasi")
                    {
                        string updateDendaQuery = @"
                            UPDATE dendas
                            SET status = 'dibayar',
                                updated_at = NOW()
                            WHERE id = @dendaId";

                        using (MySqlCommand cmd = new MySqlCommand(updateDendaQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@dendaId", _denda.Id);
                            cmd.ExecuteNonQuery();
                        }
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
                                $"Verifikasi pembayaran denda {_denda.JenisDenda} senilai Rp {_denda.Jumlah:N0} - Status: {status}");
                            logCmd.Parameters.AddWithValue("@modul", "Manajemen Denda");
                            logCmd.Parameters.AddWithValue("@referensiId", _denda.Id);
                            logCmd.Parameters.AddWithValue("@ipAddress", GetIpAddress());
                            logCmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();

                    MessageBox.Show($"Pembayaran berhasil diverifikasi dengan status: {status}", "Sukses",
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
                MessageBox.Show($"Gagal memverifikasi pembayaran: {ex.Message}", "Error",
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
