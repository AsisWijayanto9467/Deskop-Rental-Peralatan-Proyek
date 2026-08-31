using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.Dashboard
{
    public partial class DashboardPetugas : System.Windows.Forms.UserControl
    {
        // ============================================
        // AKTIVITAS TERBARU
        // ============================================
        private readonly List<AktivitasItem> _aktivitas = new List<AktivitasItem>();

        private class AktivitasItem
        {
            public string Waktu { get; set; }
            public string Modul { get; set; }
            public string Deskripsi { get; set; }
            public string Tipe { get; set; }
        }

        public DashboardPetugas()
        {
            InitializeComponent();
            InitializeAktivitasGridView();
            LoadDashboard();
        }

        private void DashboardPetugas_Load(object sender, EventArgs e)
        {
            // Data sudah dimuat di constructor
        }

        // ============================================
        // LOAD SEMUA DATA DASHBOARD
        // ============================================
        private void LoadDashboard()
        {
            LoadStatsPenyewaan();
            LoadStatsPembayaran();
            LoadStatsPengembalian();
            LoadStatsDenda();
            LoadStatsAlat();
            LoadAktivitas();
        }

        // ============================================
        // STATISTIK PENYEWAAN
        // ============================================
        private void LoadStatsPenyewaan()
        {
            lblPengajuanBaruValue.Text = "0";
            lblRentalBerjalanValue.Text = "0";

            try
            {
                string query = @"
                    SELECT
                        (SELECT COUNT(*) FROM penyewaans WHERE status = 'pending') AS pengajuan_baru,
                        (SELECT COUNT(*) FROM penyewaans WHERE status = 'sedang_disewa') AS rental_berjalan";

                DataTable dt = DatabaseConnection.GetData(query);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    lblPengajuanBaruValue.Text = Convert.ToInt32(row["pengajuan_baru"]).ToString("N0");
                    lblRentalBerjalanValue.Text = Convert.ToInt32(row["rental_berjalan"]).ToString("N0");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error statistik penyewaan: {ex.Message}");
            }
        }

        // ============================================
        // STATISTIK PEMBAYARAN
        // ============================================
        private void LoadStatsPembayaran()
        {
            lblMenungguVerifikasiValue.Text = "0";
            lblPembayaranDiverifikasiValue.Text = "0";

            try
            {
                string query = @"
                    SELECT
                        (SELECT COUNT(*) FROM pembayarans WHERE status = 'pending') AS menunggu,
                        (SELECT COUNT(*) FROM pembayarans WHERE status = 'diverifikasi') AS diverifikasi";

                DataTable dt = DatabaseConnection.GetData(query);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    lblMenungguVerifikasiValue.Text = Convert.ToInt32(row["menunggu"]).ToString("N0");
                    lblPembayaranDiverifikasiValue.Text = Convert.ToInt32(row["diverifikasi"]).ToString("N0");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error statistik pembayaran: {ex.Message}");
            }
        }

        // ============================================
        // STATISTIK PENGEMBALIAN
        // ============================================
        private void LoadStatsPengembalian()
        {
            lblMenungguInspeksiValue.Text = "0";

            try
            {
                string query = @"
                    SELECT COUNT(*) FROM pengembalians WHERE status = 'menunggu_inspeksi'";

                object result = DatabaseConnection.ExecuteScalar(query);
                lblMenungguInspeksiValue.Text = (result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result)).ToString("N0");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error statistik pengembalian: {ex.Message}");
            }
        }

        // ============================================
        // STATISTIK DENDA
        // ============================================
        private void LoadStatsDenda()
        {
            lblDendaPendingValue.Text = "0";

            try
            {
                string query = @"
                    SELECT COUNT(*) FROM dendas WHERE status = 'pending'";

                object result = DatabaseConnection.ExecuteScalar(query);
                lblDendaPendingValue.Text = (result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result)).ToString("N0");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error statistik denda: {ex.Message}");
            }
        }

        // ============================================
        // STATISTIK ALAT PROYEK
        // ============================================
        private void LoadStatsAlat()
        {
            lblAlatDisewaValue.Text = "0";
            lblAlatTersediaValue.Text = "0";

            try
            {
                string query = @"
                    SELECT
                        (SELECT IFNULL(SUM(stok_tersedia), 0) FROM alat_proyeks WHERE status = 'tersedia') AS tersedia,
                        (SELECT IFNULL(SUM(stok), 0) - IFNULL(SUM(stok_tersedia), 0) FROM alat_proyeks WHERE status = 'disewa') AS disewa";

                DataTable dt = DatabaseConnection.GetData(query);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    lblAlatTersediaValue.Text = Convert.ToInt32(row["tersedia"]).ToString("N0");
                    lblAlatDisewaValue.Text = Convert.ToInt32(row["disewa"]).ToString("N0");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error statistik alat: {ex.Message}");
            }
        }

        // ============================================
        // AKTIVITAS TERBARU
        // ============================================
        private void InitializeAktivitasGridView()
        {
            dgvAktivitas.Columns.Clear();

            dgvAktivitas.Columns.Add("Waktu", "Waktu");
            dgvAktivitas.Columns.Add("Modul", "Modul");
            dgvAktivitas.Columns.Add("Deskripsi", "Deskripsi");

            dgvAktivitas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAktivitas.AllowUserToAddRows = false;
            dgvAktivitas.ReadOnly = true;
            dgvAktivitas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAktivitas.MultiSelect = false;

            dgvAktivitas.Columns["Waktu"].FillWeight = 20;
            dgvAktivitas.Columns["Modul"].FillWeight = 18;
            dgvAktivitas.Columns["Deskripsi"].FillWeight = 100;
            dgvAktivitas.Columns["Waktu"].MinimumWidth = 140;
            dgvAktivitas.Columns["Modul"].MinimumWidth = 110;
        }

        private void LoadAktivitas()
        {
            _aktivitas.Clear();

            try
            {
                // Pengajuan rental baru (penyewaans)
                string qPengajuan = @"
                    SELECT p.created_at AS waktu, 'Penyewaan' AS modul,
                           CONCAT('User mengajukan rental ', p.kode_penyewaan) AS deskripsi
                    FROM penyewaans p
                    WHERE p.status = 'pending'
                    ORDER BY p.created_at DESC LIMIT 25";

                DataTable dtPengajuan = DatabaseConnection.GetData(qPengajuan);
                foreach (DataRow row in dtPengajuan.Rows)
                {
                    _aktivitas.Add(new AktivitasItem
                    {
                        Waktu = row["waktu"] == DBNull.Value ? "-" : Convert.ToDateTime(row["waktu"]).ToString("dd/MM/yyyy HH:mm"),
                        Modul = row["modul"]?.ToString() ?? "-",
                        Deskripsi = row["deskripsi"]?.ToString() ?? "",
                        Tipe = "pengajuan"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aktivitas penyewaan: {ex.Message}");
            }

            try
            {
                // Upload pembayaran (pembayarans pending)
                string qPembayaranPending = @"
                    SELECT b.created_at AS waktu, 'Pembayaran' AS modul,
                           CONCAT('User mengupload pembayaran ', b.kode_pembayaran) AS deskripsi
                    FROM pembayarans b
                    WHERE b.status = 'pending'
                    ORDER BY b.created_at DESC LIMIT 25";

                DataTable dtPembayaran = DatabaseConnection.GetData(qPembayaranPending);
                foreach (DataRow row in dtPembayaran.Rows)
                {
                    _aktivitas.Add(new AktivitasItem
                    {
                        Waktu = row["waktu"] == DBNull.Value ? "-" : Convert.ToDateTime(row["waktu"]).ToString("dd/MM/yyyy HH:mm"),
                        Modul = row["modul"]?.ToString() ?? "-",
                        Deskripsi = row["deskripsi"]?.ToString() ?? "",
                        Tipe = "verifikasi"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aktivitas pembayaran: {ex.Message}");
            }

            try
            {
                // Pembayaran diverifikasi
                string qPembayaranVerified = @"
                    SELECT b.tanggal_verifikasi AS waktu, 'Pembayaran' AS modul,
                           CONCAT('Pembayaran ', b.kode_pembayaran, ' diverifikasi') AS deskripsi
                    FROM pembayarans b
                    WHERE b.status = 'diverifikasi' AND b.tanggal_verifikasi IS NOT NULL
                    ORDER BY b.tanggal_verifikasi DESC LIMIT 25";

                DataTable dtVerified = DatabaseConnection.GetData(qPembayaranVerified);
                foreach (DataRow row in dtVerified.Rows)
                {
                    _aktivitas.Add(new AktivitasItem
                    {
                        Waktu = row["waktu"] == DBNull.Value ? "-" : Convert.ToDateTime(row["waktu"]).ToString("dd/MM/yyyy HH:mm"),
                        Modul = row["modul"]?.ToString() ?? "-",
                        Deskripsi = row["deskripsi"]?.ToString() ?? "",
                        Tipe = "verifikasi"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aktivitas verifikasi: {ex.Message}");
            }

            try
            {
                // Pengajuan pengembalian (menunggu_inspeksi)
                string qPengembalian = @"
                    SELECT k.created_at AS waktu, 'Pengembalian' AS modul,
                           CONCAT('User mengajukan pengembalian untuk penyewaan #', k.penyewaan_id) AS deskripsi
                    FROM pengembalians k
                    WHERE k.status = 'menunggu_inspeksi'
                    ORDER BY k.created_at DESC LIMIT 25";

                DataTable dtPengembalian = DatabaseConnection.GetData(qPengembalian);
                foreach (DataRow row in dtPengembalian.Rows)
                {
                    _aktivitas.Add(new AktivitasItem
                    {
                        Waktu = row["waktu"] == DBNull.Value ? "-" : Convert.ToDateTime(row["waktu"]).ToString("dd/MM/yyyy HH:mm"),
                        Modul = row["modul"]?.ToString() ?? "-",
                        Deskripsi = row["deskripsi"]?.ToString() ?? "",
                        Tipe = "verifikasi"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aktivitas pengembalian: {ex.Message}");
            }

            try
            {
                // Pengembalian diterima/ditolak
                string qPengembalianSelesai = @"
                    SELECT k.updated_at AS waktu, 'Pengembalian' AS modul,
                           CONCAT('Pengembalian penyewaan #', k.penyewaan_id, ' ', 
                                  CASE WHEN k.status = 'diterima' THEN 'diterima' 
                                       WHEN k.status = 'ditolak' THEN 'ditolak' 
                                       ELSE k.status END) AS deskripsi
                    FROM pengembalians k
                    WHERE k.status IN ('diterima','ditolak')
                    ORDER BY k.updated_at DESC LIMIT 25";

                DataTable dtPengembalianSelesai = DatabaseConnection.GetData(qPengembalianSelesai);
                foreach (DataRow row in dtPengembalianSelesai.Rows)
                {
                    _aktivitas.Add(new AktivitasItem
                    {
                        Waktu = row["waktu"] == DBNull.Value ? "-" : Convert.ToDateTime(row["waktu"]).ToString("dd/MM/yyyy HH:mm"),
                        Modul = row["modul"]?.ToString() ?? "-",
                        Deskripsi = row["deskripsi"]?.ToString() ?? "",
                        Tipe = "sukses"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aktivitas pengembalian selesai: {ex.Message}");
            }

            try
            {
                // Denda dibuat
                string qDenda = @"
                    SELECT d.created_at AS waktu, 'Denda' AS modul,
                           CONCAT('Denda ', d.jenis_denda, ' dibuat sebesar ', d.jumlah) AS deskripsi
                    FROM dendas d
                    ORDER BY d.created_at DESC LIMIT 25";

                DataTable dtDenda = DatabaseConnection.GetData(qDenda);
                foreach (DataRow row in dtDenda.Rows)
                {
                    _aktivitas.Add(new AktivitasItem
                    {
                        Waktu = row["waktu"] == DBNull.Value ? "-" : Convert.ToDateTime(row["waktu"]).ToString("dd/MM/yyyy HH:mm"),
                        Modul = row["modul"]?.ToString() ?? "-",
                        Deskripsi = row["deskripsi"]?.ToString() ?? "",
                        Tipe = "denda"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aktivitas denda: {ex.Message}");
            }

            _aktivitas.Sort((a, b) => string.Compare(b.Waktu, a.Waktu, StringComparison.Ordinal));
            DisplayAktivitas();
        }

        private void DisplayAktivitas()
        {
            dgvAktivitas.Rows.Clear();

            if (_aktivitas.Count == 0)
            {
                dgvAktivitas.Rows.Add("-", "-", "Belum ada aktivitas tercatat.");
                return;
            }

            int count = Math.Min(_aktivitas.Count, 10);
            for (int i = 0; i < count; i++)
            {
                var item = _aktivitas[i];
                int rowIndex = dgvAktivitas.Rows.Add(item.Waktu, item.Modul, item.Deskripsi);

                string tipe = item.Tipe;
                if (tipe == "pengajuan")
                {
                    dgvAktivitas.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(52, 152, 219);
                }
                else if (tipe == "verifikasi")
                {
                    dgvAktivitas.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(230, 126, 34);
                }
                else if (tipe == "sukses")
                {
                    dgvAktivitas.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                }
                else if (tipe == "denda")
                {
                    dgvAktivitas.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                }
            }
        }

        // ============================================
        // EVENT HANDLERS
        // ============================================
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                LoadDashboard();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
