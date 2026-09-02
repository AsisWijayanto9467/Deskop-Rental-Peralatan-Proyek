using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Pembayaran
{
    public partial class VerifikasiPembayaran : Form
    {
        private ulong _pembayaranId;

        public VerifikasiPembayaran(ulong pembayaranId)
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

        private void VerifikasiPembayaran_Load(object sender, EventArgs e)
        {
            LoadSummary();
        }

        private void LoadSummary()
        {
            try
            {
                string query = @"
                    SELECT pb.kode_pembayaran, pb.jumlah, pb.metode_pembayaran, pb.bukti_pembayaran,
                           ps.kode_penyewaan, ps.total AS total_sewa,
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

                string metode = row["metode_pembayaran"]?.ToString() ?? "cash";
                lblMetode.Text = FormatMetodePembayaran(metode);

                if (!string.IsNullOrEmpty(row["bukti_pembayaran"]?.ToString()))
                {
                    lblBukti.Text = row["bukti_pembayaran"]?.ToString() ?? "-";
                    btnLihatBukti.Visible = true;
                    btnLihatBukti.Tag = row["bukti_pembayaran"]?.ToString();
                }
                else
                {
                    lblBukti.Text = "Tidak ada bukti";
                    btnLihatBukti.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLihatBukti_Click(object sender, EventArgs e)
        {
            if (btnLihatBukti.Tag == null) return;

            string fileName = btnLihatBukti.Tag.ToString();
            try
            {
                string buktiPath = System.IO.Path.Combine(
                    Application.StartupPath,
                    "Resources", "BuktiPembayaran",
                    fileName);

                if (System.IO.File.Exists(buktiPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = buktiPath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show($"File bukti pembayaran tidak ditemukan:\n{buktiPath}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error membuka bukti pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            string catatan = txtCatatan.Text.Trim();

            DialogResult confirm = MessageBox.Show(
                "Apakah Anda yakin ingin memverifikasi pembayaran ini?\n\n" +
                "Status pembayaran akan berubah menjadi 'Diverifikasi' dan\n" +
                "status penyewaan akan berubah menjadi 'Dibayar'.",
                "Konfirmasi Verifikasi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    ulong verifikatorId = SessionManager.GetCurrentUserId();

                    // Mulai transaction
                    MySqlConnection connection = null;
                    MySqlTransaction transaction = null;

                    try
                    {
                        connection = DatabaseConnection.GetConnection();
                        connection.Open();
                        transaction = connection.BeginTransaction();

                        // 1. Update pembayarans
                        string updatePembayaranQuery = @"
                            UPDATE pembayarans
                            SET status = 'diverifikasi',
                                diverifikasi_oleh = @verifikator_id,
                                tanggal_verifikasi = NOW(),
                                catatan = @catatan,
                                updated_at = NOW()
                            WHERE id = @id";

                        MySqlParameter[] updateParams = new MySqlParameter[]
                        {
                            new MySqlParameter("@verifikator_id", verifikatorId == 0 ? (object)DBNull.Value : verifikatorId),
                            new MySqlParameter("@catatan", string.IsNullOrWhiteSpace(catatan) ? (object)DBNull.Value : catatan),
                            new MySqlParameter("@id", _pembayaranId)
                        };

                        using (MySqlCommand cmd = new MySqlCommand(updatePembayaranQuery, connection, transaction))
                        {
                            cmd.Parameters.AddRange(updateParams);
                            int result = cmd.ExecuteNonQuery();
                            if (result <= 0)
                            {
                                transaction.Rollback();
                                MessageBox.Show("Gagal memverifikasi pembayaran.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        // 2. Update penyewaans status ke dibayar
                        string getPenyewaanQuery = "SELECT penyewaan_id FROM pembayarans WHERE id = @id";
                        ulong penyewaanId = 0;
                        using (MySqlCommand cmd = new MySqlCommand(getPenyewaanQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", _pembayaranId);
                            object result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                penyewaanId = Convert.ToUInt64(result);
                            }
                        }

                        if (penyewaanId > 0)
                        {
                            string updatePenyewaanQuery = @"
                                UPDATE penyewaans
                                SET status = 'dibayar',
                                    updated_at = NOW()
                                WHERE id = @id";

                            MySqlParameter[] penyewaanParams = new MySqlParameter[]
                            {
                                new MySqlParameter("@id", penyewaanId)
                            };

                            using (MySqlCommand cmd = new MySqlCommand(updatePenyewaanQuery, connection, transaction))
                            {
                                cmd.Parameters.AddRange(penyewaanParams);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 3. Log activity
                        string aktivitas = $"Memverifikasi pembayaran '{lblKodePembayaran.Text}' untuk penyewaan '{lblKodePenyewaan.Text}' dari {lblCustomer.Text}";
                        if (!string.IsNullOrWhiteSpace(catatan))
                        {
                            aktivitas += $" | Catatan: {catatan}";
                        }

                        string logQuery = @"
                            INSERT INTO activity_logs
                            (user_id, aktivitas, modul, referensi_id, ip_address, created_at)
                            VALUES
                            (@user_id, @aktivitas, @modul, @referensi_id, @ip_address, NOW())";

                        MySqlParameter[] logParams = new MySqlParameter[]
                        {
                            new MySqlParameter("@user_id", verifikatorId == 0 ? (object)DBNull.Value : verifikatorId),
                            new MySqlParameter("@aktivitas", aktivitas),
                            new MySqlParameter("@modul", "Pembayaran"),
                            new MySqlParameter("@referensi_id", _pembayaranId),
                            new MySqlParameter("@ip_address", GetClientIpAddress())
                        };

                        using (MySqlCommand cmd = new MySqlCommand(logQuery, connection, transaction))
                        {
                            cmd.Parameters.AddRange(logParams);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        MessageBox.Show("Pembayaran berhasil diverifikasi!\nStatus penyewaan diubah menjadi 'Dibayar'.",
                            "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        if (transaction != null)
                        {
                            try { transaction.Rollback(); } catch { }
                        }
                        throw ex;
                    }
                    finally
                    {
                        if (connection != null && connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error memverifikasi pembayaran: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private string FormatMetodePembayaran(string metode)
        {
            switch (metode)
            {
                case "cash": return "Cash";
                case "transfer": return "Transfer Bank";
                case "qris": return "QRIS";
                default: return metode;
            }
        }

        private string GetClientIpAddress()
        {
            try
            {
                string hostName = System.Net.Dns.GetHostName();
                var addresses = System.Net.Dns.GetHostAddresses(hostName);

                foreach (var address in addresses)
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return address.ToString();
                    }
                }

                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }
}