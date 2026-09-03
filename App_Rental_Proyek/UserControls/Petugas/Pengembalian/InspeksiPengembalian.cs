using App_Rental_Proyek.Config;
using App_Rental_Proyek.Helper;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Pengembalian
{
    public partial class InspeksiPengembalian : Form
    {
        private ulong _pengembalianId;
        private ulong _penyewaanId;
        private decimal _totalSewa;
        private int _terlambatHari;
        private decimal _dendaPerHari = 10000m;

        public InspeksiPengembalian(ulong pengembalianId)
        {
            InitializeComponent();
            _pengembalianId = pengembalianId;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InspeksiPengembalian_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = @"
                    SELECT pg.id, pg.penyewaan_id, pg.tanggal_pengembalian,
                           pg.kondisi_alat, pg.foto, pg.terlambat_hari,
                           pg.catatan, pg.status, pg.created_at,
                           p.kode_penyewaan, p.tanggal_selesai, p.total AS total_sewa,
                           u.nama AS nama_customer
                    FROM pengembalians pg
                    LEFT JOIN penyewaans p ON p.id = pg.penyewaan_id
                    LEFT JOIN users u ON u.id = p.user_id
                    WHERE pg.id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _pengembalianId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Data pengembalian tidak ditemukan!", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                DataRow row = dt.Rows[0];

                _penyewaanId = row["penyewaan_id"] != DBNull.Value ? Convert.ToUInt64(row["penyewaan_id"]) : 0;
                _totalSewa = row["total_sewa"] != DBNull.Value ? Convert.ToDecimal(row["total_sewa"]) : 0m;
                _terlambatHari = row["terlambat_hari"] != DBNull.Value ? Convert.ToInt32(row["terlambat_hari"]) : 0;

                lblKode.Text = row["kode_penyewaan"]?.ToString() ?? "-";
                lblCustomer.Text = row["nama_customer"]?.ToString() ?? "-";

                string tglMulai = "";
                string tglSelesai = "";
                DataTable dtSewa = DatabaseConnection.GetData(
                    "SELECT tanggal_mulai, tanggal_selesai FROM penyewaans WHERE id = @id",
                    new MySqlParameter[] { new MySqlParameter("@id", _penyewaanId) });

                if (dtSewa.Rows.Count > 0)
                {
                    tglMulai = dtSewa.Rows[0]["tanggal_mulai"] != DBNull.Value
                        ? Convert.ToDateTime(dtSewa.Rows[0]["tanggal_mulai"]).ToString("dd/MM/yyyy") : "-";
                    tglSelesai = dtSewa.Rows[0]["tanggal_selesai"] != DBNull.Value
                        ? Convert.ToDateTime(dtSewa.Rows[0]["tanggal_selesai"]).ToString("dd/MM/yyyy") : "-";
                }

                lblPeriode.Text = $"{tglMulai} - {tglSelesai}";

                string tglKembali = row["tanggal_pengembalian"] != DBNull.Value
                    ? Convert.ToDateTime(row["tanggal_pengembalian"]).ToString("dd/MM/yyyy") : "-";
                lblTglKembali.Text = tglKembali;

                if (_terlambatHari > 0)
                {
                    lblTerlambat.Text = $"{_terlambatHari} hari";
                    lblTerlambat.ForeColor = Color.FromArgb(231, 76, 60);
                }
                else
                {
                    lblTerlambat.Text = "Tepat Waktu";
                    lblTerlambat.ForeColor = Color.FromArgb(46, 204, 113);
                }

                string kondisi = row["kondisi_alat"]?.ToString();
                lblKondisiUser.Text = string.IsNullOrWhiteSpace(kondisi) ? "-" : kondisi;

                string catatan = row["catatan"]?.ToString();
                lblCatatanUser.Text = string.IsNullOrWhiteSpace(catatan) ? "-" : catatan;

                LoadFoto(row["foto"]?.ToString());
                HitungDenda();

                string status = row["status"]?.ToString() ?? "menunggu_inspeksi";
                if (status != "menunggu_inspeksi")
                {
                    btnTerima.Enabled = false;
                    btnTolak.Enabled = false;
                    txtCatatanInspeksi.Enabled = false;

                    if (status == "diterima")
                    {
                        lblDenda.Text = "Pengembalian ini sudah diinspeksi dan diterima.";
                        lblDenda.ForeColor = Color.FromArgb(46, 204, 113);
                    }
                    else if (status == "ditolak")
                    {
                        lblDenda.Text = "Pengembalian ini sudah diinspeksi dan ditolak.";
                        lblDenda.ForeColor = Color.FromArgb(231, 76, 60);
                    }
                    else
                    {
                        lblDenda.Text = $"Status: {FormatStatusLabel(status)}";
                        lblDenda.ForeColor = Color.FromArgb(241, 196, 15);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data pengembalian: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadFoto(string? foto)
        {
            if (string.IsNullOrEmpty(foto))
            {
                picFoto.Image = null;
                return;
            }

            try
            {
                string fotoPath = System.IO.Path.Combine(
                    Application.StartupPath, "Resources", "FotoPengembalian", foto);

                if (!System.IO.File.Exists(fotoPath))
                {
                    fotoPath = System.IO.Path.Combine("D:\\Cross_Storage\\Sistem_Proyek", foto);
                }

                if (System.IO.File.Exists(fotoPath))
                {
                    byte[] imageBytes = System.IO.File.ReadAllBytes(fotoPath);
                    using (var ms = new System.IO.MemoryStream(imageBytes))
                    {
                        picFoto.Image = Image.FromStream(ms).GetThumbnailImage(220, 160, null, IntPtr.Zero);
                    }
                }
                else
                {
                    picFoto.Image = null;
                }
            }
            catch
            {
                picFoto.Image = null;
            }
        }

        private void HitungDenda()
        {
            if (_terlambatHari > 0)
            {
                decimal jumlahDenda = _terlambatHari * _dendaPerHari;
                lblDenda.Text = $"Keterlambatan: {_terlambatHari} hari\n" +
                                $"Denda per hari: Rp {_dendaPerHari:N0}\n" +
                                $"Total denda: Rp {jumlahDenda:N0}";
                lblDenda.ForeColor = Color.FromArgb(231, 76, 60);
            }
            else
            {
                lblDenda.Text = "Tidak ada denda (tepat waktu)";
                lblDenda.ForeColor = Color.FromArgb(46, 204, 113);
            }
        }

        private string FormatStatusLabel(string status)
        {
            switch (status)
            {
                case "menunggu_inspeksi": return "Menunggu Inspeksi";
                case "diterima": return "Diterima";
                case "perlu_perbaikan": return "Perlu Perbaikan";
                case "ditolak": return "Ditolak";
                default: return status;
            }
        }

        // ============================================
        // LOG ACTIVITY
        // ============================================
        private void LogActivity(string aktivitas, string modul, ulong? referensiId = null)
        {
            ActivityLogHelper.LogForSession(SessionManager.GetCurrentUserId(), aktivitas, modul, referensiId);
        }

        // ============================================
        // TERIMA PENGEMBALIAN
        // ============================================
        private void btnTerima_Click(object sender, EventArgs e)
        {
            string catatan = txtCatatanInspeksi.Text.Trim();

            DialogResult confirm = MessageBox.Show(
                "Apakah Anda yakin ingin menerima pengembalian ini?\n\n" +
                (_terlambatHari > 0
                    ? $"Pengembalian terlambat {_terlambatHari} hari. Denda akan dibuat secara otomatis.\n"
                    : "Pengembalian tepat waktu. Tidak ada denda.\n") +
                "Status penyewaan akan berubah menjadi 'Selesai'.",
                "Konfirmasi Penerimaan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    ulong petugasId = SessionManager.GetCurrentUserId();

                    // 1. Update pengembalian
                    string updatePengembalian = @"
                        UPDATE pengembalians
                        SET status = 'diterima',
                            diterima_oleh = @diterima_oleh,
                            terlambat_hari = @terlambat_hari,
                            catatan = @catatan,
                            updated_at = NOW()
                        WHERE id = @id";

                    MySqlParameter[] paramsPengembalian = new MySqlParameter[]
                    {
                        new MySqlParameter("@diterima_oleh", petugasId == 0 ? (object)DBNull.Value : petugasId),
                        new MySqlParameter("@terlambat_hari", _terlambatHari),
                        new MySqlParameter("@catatan", (object)catatan ?? DBNull.Value),
                        new MySqlParameter("@id", _pengembalianId)
                    };

                    DatabaseConnection.ExecuteQuery(updatePengembalian, paramsPengembalian);

                    // 2. Update penyewaan -> selesai
                    string updatePenyewaan = @"
                        UPDATE penyewaans
                        SET status = 'selesai',
                            updated_at = NOW()
                        WHERE id = @id";

                    DatabaseConnection.ExecuteQuery(updatePenyewaan,
                        new MySqlParameter[] { new MySqlParameter("@id", _penyewaanId) });

                    // 3. Update stok alat_proyeks -> tambah stok_tersedia
                    UpdateStokAlat(_penyewaanId, 1);

                    // 4. Buat denda jika terlambat
                    if (_terlambatHari > 0)
                    {
                        decimal jumlahDenda = _terlambatHari * _dendaPerHari;
                        string insertDenda = @"
                            INSERT INTO denda
                            (penyewaan_id, pengembalian_id, jenis_denda, jumlah, alasan, status, created_at, updated_at)
                            VALUES
                            (@penyewaan_id, @pengembalian_id, 'terlambat', @jumlah, @alasan, 'pending', NOW(), NOW())";

                        string alasan = $"Keterlambatan pengembalian {_terlambatHari} hari (Rp {_dendaPerHari:N0}/hari)";

                        DatabaseConnection.ExecuteQuery(insertDenda, new MySqlParameter[]
                        {
                            new MySqlParameter("@penyewaan_id", _penyewaanId),
                            new MySqlParameter("@pengembalian_id", _pengembalianId),
                            new MySqlParameter("@jumlah", jumlahDenda),
                            new MySqlParameter("@alasan", alasan)
                        });

                        // Update denda pada penyewaan
                        DatabaseConnection.ExecuteQuery(
                            "UPDATE penyewaans SET denda = @denda, total = subtotal + @denda WHERE id = @id",
                            new MySqlParameter[]
                            {
                                new MySqlParameter("@denda", jumlahDenda),
                                new MySqlParameter("@id", _penyewaanId)
                            });
                    }

                    // 5. Activity Log
                    string kode = lblKode.Text;
                    string customer = lblCustomer.Text;
                    string aktivitas = $"Menerima pengembalian '{kode}' dari {customer}";
                    if (_terlambatHari > 0)
                    {
                        decimal dendaTotal = _terlambatHari * _dendaPerHari;
                        aktivitas += $" | Terlambat {_terlambatHari} hari, denda Rp {dendaTotal:N0}";
                    }
                    if (!string.IsNullOrWhiteSpace(catatan))
                    {
                        aktivitas += $" | Catatan: {catatan}";
                    }

                    LogActivity(aktivitas, "Pengembalian", _pengembalianId);

                    MessageBox.Show("Pengembalian berhasil diterima!",
                        "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error menerima pengembalian: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ============================================
        // TOLAK PENGEMBALIAN
        // ============================================
        private void btnTolak_Click(object sender, EventArgs e)
        {
            string catatan = txtCatatanInspeksi.Text.Trim();

            if (string.IsNullOrWhiteSpace(catatan))
            {
                MessageBox.Show("Catatan inspeksi wajib diisi saat menolak pengembalian.",
                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCatatanInspeksi.Focus();
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Apakah Anda yakin ingin menolak pengembalian ini?\n\n" +
                "Status akan berubah menjadi 'Ditolak'.",
                "Konfirmasi Penolakan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    ulong petugasId = SessionManager.GetCurrentUserId();

                    string updatePengembalian = @"
                        UPDATE pengembalians
                        SET status = 'ditolak',
                            diterima_oleh = @diterima_oleh,
                            terlambat_hari = @terlambat_hari,
                            catatan = @catatan,
                            updated_at = NOW()
                        WHERE id = @id";

                    DatabaseConnection.ExecuteQuery(updatePengembalian, new MySqlParameter[]
                    {
                        new MySqlParameter("@diterima_oleh", petugasId == 0 ? (object)DBNull.Value : petugasId),
                        new MySqlParameter("@terlambat_hari", _terlambatHari),
                        new MySqlParameter("@catatan", catatan),
                        new MySqlParameter("@id", _pengembalianId)
                    });

                    string kode = lblKode.Text;
                    string customer = lblCustomer.Text;
                    string aktivitas = $"Menolak pengembalian '{kode}' dari {customer} | Alasan: {catatan}";

                    LogActivity(aktivitas, "Pengembalian", _pengembalianId);

                    MessageBox.Show("Pengembalian ditolak.",
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error menolak pengembalian: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ============================================
        // UPDATE STOK ALAT
        // ============================================
        private void UpdateStokAlat(ulong penyewaanId, int tambah)
        {
            try
            {
                string query = "SELECT alat_id, jumlah FROM detail_penyewaans WHERE penyewaan_id = @penyewaan_id";
                DataTable dt = DatabaseConnection.GetData(query,
                    new MySqlParameter[] { new MySqlParameter("@penyewaan_id", penyewaanId) });

                foreach (DataRow row in dt.Rows)
                {
                    ulong alatId = Convert.ToUInt64(row["alat_id"]);
                    int jumlah = row["jumlah"] != DBNull.Value ? Convert.ToInt32(row["jumlah"]) : 1;

                    DatabaseConnection.ExecuteQuery(
                        "UPDATE alat_proyeks SET stok_tersedia = stok_tersedia + @jumlah WHERE id = @id",
                        new MySqlParameter[]
                        {
                            new MySqlParameter("@jumlah", jumlah * tambah),
                            new MySqlParameter("@id", alatId)
                        });

                    DatabaseConnection.ExecuteQuery(
                        "UPDATE alat_proyeks SET status = 'tersedia' WHERE id = @id AND status = 'disewa'",
                        new MySqlParameter[] { new MySqlParameter("@id", alatId) });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error update stok alat: {ex.Message}");
            }
        }
    }
}
