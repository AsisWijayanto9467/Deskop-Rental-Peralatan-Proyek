using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.Pembayaran
{
    public partial class VerifikasiPembayaran : Form
    {
        private readonly PembayaranModel _model;
        private readonly ulong _currentUserId;

        public VerifikasiPembayaran(PembayaranModel model, ulong currentUserId)
        {
            InitializeComponent();
            _model = model;
            _currentUserId = currentUserId;
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
            lblKode.Text = _model.KodePembayaran;
            lblStatus.Text = "Status: " + FormatStatusLabel(_model.Status);
            lblStatus.ForeColor = GetStatusColor(_model.Status);
            lblStatus.BackColor = Color.Transparent;

            lblInfoCust.Text = $"Customer: {(string.IsNullOrWhiteSpace(_model.NamaCustomer) ? "-" : _model.NamaCustomer)}";
            lblInfoNominal.Text = "Nominal: Rp " + _model.Jumlah.ToString("N0");
            lblInfoMetode.Text = "Metode: " + FormatMetodeLabel(_model.MetodePembayaran);
            lblInfoPenyewaan.Text = $"Kode Sewa: {(string.IsNullOrWhiteSpace(_model.KodePenyewaan) ? "-" : _model.KodePenyewaan)}";

            LoadBukti();
        }

        private void LoadBukti()
        {
            picBukti.Image?.Dispose();
            picBukti.Image = null;
            lblBuktiInfo.Text = "-";

            string bukti = _model.BuktiPembayaran;
            if (string.IsNullOrWhiteSpace(bukti))
            {
                lblBuktiInfo.Text = "Tidak ada bukti pembayaran diunggah.";
                return;
            }

            string path = BuktiPembayaranHelper.ResolvePath(bukti);
            if (!File.Exists(path))
            {
                lblBuktiInfo.Text = $"File bukti tidak ditemukan:\n{bukti}";
                lblBuktiInfo.ForeColor = Color.FromArgb(231, 76, 60);
                return;
            }

            if (!BuktiPembayaranHelper.IsImage(path))
            {
                lblBuktiInfo.Text = $"Bukti tersedia (bukan gambar):\n{path}";
                return;
            }

            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    picBukti.Image = Image.FromStream(fs);
                }
                lblBuktiInfo.Text = $"Bukti: {Path.GetFileName(path)}";
                lblBuktiInfo.ForeColor = Color.FromArgb(96, 110, 130);
            }
            catch (Exception ex)
            {
                lblBuktiInfo.Text = $"Gagal memuat gambar bukti: {ex.Message}";
                lblBuktiInfo.ForeColor = Color.FromArgb(231, 76, 60);
            }
        }

        private bool SimpanVerifikasi(string newStatus, string catatan)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new MySqlCommand(@"
                            UPDATE pembayarans
                            SET status = @status,
                                diverifikasi_oleh = @verifikator,
                                tanggal_verifikasi = NOW(),
                                catatan = @catatan,
                                updated_at = NOW()
                            WHERE id = @id", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@status", newStatus);
                            cmd.Parameters.AddWithValue("@verifikator", _currentUserId == 0 ? (object)DBNull.Value : _currentUserId);
                            cmd.Parameters.AddWithValue("@catatan", string.IsNullOrWhiteSpace(catatan) ? (object)DBNull.Value : catatan.Trim());
                            cmd.Parameters.AddWithValue("@id", _model.Id);
                            cmd.ExecuteNonQuery();
                        }

                        if (newStatus == "diverifikasi")
                        {
                            using (var cmd = new MySqlCommand(@"
                                UPDATE penyewaans
                                SET status = 'dibayar',
                                    processed_by = @verifikator,
                                    updated_at = NOW()
                                WHERE id = @penyewaan_id AND status = 'menunggu_pembayaran'", conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@verifikator", _currentUserId == 0 ? (object)DBNull.Value : _currentUserId);
                                cmd.Parameters.AddWithValue("@penyewaan_id", _model.PenyewaanId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show($"Error menyimpan verifikasi: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }

        private void btnTerima_Click(object sender, EventArgs e)
        {
            DialogResult konfirmasi = MessageBox.Show(
                $"Verifikasi pembayaran '{_model.KodePembayaran}' sebesar Rp {_model.Jumlah:N0} sebagai LUNAS?",
                "Konfirmasi Verifikasi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (konfirmasi != DialogResult.Yes) return;

            if (SimpanVerifikasi("diverifikasi", txtCatatan.Text))
            {
                MessageBox.Show("Pembayaran berhasil diverifikasi sebagai LUNAS.",
                    "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Gagal memverifikasi pembayaran. Silakan coba lagi.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTolak_Click(object sender, EventArgs e)
        {
            DialogResult konfirmasi = MessageBox.Show(
                $"Tandai pembayaran '{_model.KodePembayaran}' sebesar Rp {_model.Jumlah:N0} sebagai GAGAL?",
                "Konfirmasi Penolakan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (konfirmasi != DialogResult.Yes) return;

            if (SimpanVerifikasi("ditolak", txtCatatan.Text))
            {
                MessageBox.Show("Pembayaran ditandai sebagai GAGAL.",
                    "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Gagal memperbarui pembayaran. Silakan coba lagi.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBukaBukti_Click(object sender, EventArgs e)
        {
            string path = BuktiPembayaranHelper.ResolvePath(_model.BuktiPembayaran);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                MessageBox.Show("File bukti pembayaran tidak ditemukan.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal membuka bukti pembayaran: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            base.OnFormClosing(e);
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "pending": return "Menunggu";
                case "diverifikasi": return "Lunas";
                case "ditolak": return "Gagal";
                default: return status;
            }
        }

        private string FormatMetodeLabel(string metode)
        {
            switch (metode)
            {
                case "cash": return "Tunai (Cash)";
                case "transfer": return "Transfer Bank";
                case "qris": return "QRIS";
                default: return metode;
            }
        }

        private Color GetStatusColor(string status)
        {
            switch (status)
            {
                case "pending": return Color.FromArgb(241, 196, 15);
                case "diverifikasi": return Color.FromArgb(46, 204, 113);
                case "ditolak": return Color.FromArgb(231, 76, 60);
                default: return Color.FromArgb(52, 152, 219);
            }
        }
    }
}