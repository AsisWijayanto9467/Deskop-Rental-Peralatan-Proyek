using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.Penyewaan
{
    public partial class CreatePenyewaan : Form
    {
        private readonly List<DetailPenyewaanModel> _daftarAlat = new List<DetailPenyewaanModel>();

        public CreatePenyewaan()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            dgvDaftarAlat.ReadOnly = true;
        }

        private void CreatePenyewaan_Load(object sender, EventArgs e)
        {
            SetupComboboxes();

            dtPengajuan.Value = DateTime.Today;
            dtMulai.Value = DateTime.Today;
            dtSelesai.Value = DateTime.Today.AddDays(1);

            UpdateSummary();
        }

        private void SetupComboboxes()
        {
            // Customer (role = user)
            try
            {
                DataTable dt = DatabaseConnection.GetData(
                    "SELECT id, nama, email FROM users WHERE role = 'user' ORDER BY nama ASC");
                cbCustomer.Items.Clear();
                cbCustomer.Tag = dt;
                foreach (DataRow row in dt.Rows)
                {
                    cbCustomer.Items.Add($"{row["nama"]} ({(row["email"]?.ToString() ?? "-")})");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat data customer: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Alat yang tersedia
            try
            {
                DataTable dt = DatabaseConnection.GetData(@"
                    SELECT a.id, a.kode_alat, a.nama_alat, a.harga_sewa_harian, a.stok_tersedia
                    FROM alat_proyeks a
                    WHERE a.status IN ('tersedia','disewa') AND a.stok_tersedia > 0
                    ORDER BY a.nama_alat ASC");
                cbAlat.Items.Clear();
                cbAlat.Tag = dt;
                foreach (DataRow row in dt.Rows)
                {
                    cbAlat.Items.Add($"{row["kode_alat"]} - {row["nama_alat"]}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat data alat: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataRow GetSelectedCustomerRow()
        {
            int idx = cbCustomer.SelectedIndex;
            if (idx < 0) return null;
            DataTable dt = cbCustomer.Tag as DataTable;
            if (dt == null || idx >= dt.Rows.Count) return null;
            return dt.Rows[idx];
        }

        private DataRow GetSelectedAlatRow()
        {
            int idx = cbAlat.SelectedIndex;
            if (idx < 0) return null;
            DataTable dt = cbAlat.Tag as DataTable;
            if (dt == null || idx >= dt.Rows.Count) return null;
            return dt.Rows[idx];
        }

        private void btnTambahAlat_Click(object sender, EventArgs e)
        {
            DataRow alatRow = GetSelectedAlatRow();
            if (alatRow == null)
            {
                MessageBox.Show("Silakan pilih alat terlebih dahulu!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ulong alatId = Convert.ToUInt64(alatRow["id"]);
            string kodeAlat = alatRow["kode_alat"]?.ToString() ?? "";
            string namaAlat = alatRow["nama_alat"]?.ToString() ?? "";
            decimal harga = alatRow["harga_sewa_harian"] != DBNull.Value ? Convert.ToDecimal(alatRow["harga_sewa_harian"]) : 0m;
            int stokTersedia = alatRow["stok_tersedia"] != DBNull.Value ? Convert.ToInt32(alatRow["stok_tersedia"]) : 0;

            if (!int.TryParse(txtJumlah.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Jumlah harus berupa angka yang valid dan lebih dari 0!", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtJumlah.Focus();
                return;
            }

            var existing = _daftarAlat.Find(d => d.AlatId == alatId);
            int totalQty = qty + (existing?.Jumlah ?? 0);

            if (totalQty > stokTersedia)
            {
                MessageBox.Show($"Stok tersedia '{namaAlat}' hanya {stokTersedia}.", "Validasi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (existing != null)
            {
                existing.Jumlah = totalQty;
                existing.Subtotal = decimal.Parse(existing.Jumlah.ToString()) * existing.HargaSewa;
            }
            else
            {
                _daftarAlat.Add(new DetailPenyewaanModel
                {
                    AlatId = alatId,
                    KodeAlat = kodeAlat,
                    NamaAlat = namaAlat,
                    Jumlah = qty,
                    HargaSewa = harga,
                    Subtotal = (decimal)qty * harga,
                    KondisiSebelum = "baik"
                });
            }

            RefreshGrid();
            UpdateSummary();

            txtJumlah.Text = "1";
        }

        private void RefreshGrid()
        {
            dgvDaftarAlat.Rows.Clear();

            foreach (var item in _daftarAlat)
            {
                dgvDaftarAlat.Rows.Add(
                    item.AlatId,
                    $"{item.KodeAlat} - {item.NamaAlat}",
                    "Rp " + item.HargaSewa.ToString("N0"),
                    item.Jumlah,
                    "Rp " + item.Subtotal.ToString("N0")
                );
            }
        }

        private void UpdateSummary()
        {
            int totalHari = HitungTotalHari();
            decimal subtotal = 0m;
            foreach (var item in _daftarAlat)
            {
                subtotal += item.Subtotal;
            }

            lblTotalHari.Text = $"Total Hari: {totalHari}";
            lblSubtotal.Text = "Subtotal: Rp " + subtotal.ToString("N0");
            lblTotal.Text = "Total: Rp " + subtotal.ToString("N0");
        }

        private int HitungTotalHari()
        {
            DateTime mulai = dtMulai.Value.Date;
            DateTime selesai = dtSelesai.Value.Date;

            if (selesai < mulai) return 0;

            int hari = (selesai - mulai).Days + 1;
            return hari > 0 ? hari : 1;
        }

        private void dtMulai_ValueChanged(object sender, EventArgs e)
        {
            UpdateSummary();
        }

        private void dtSelesai_ValueChanged(object sender, EventArgs e)
        {
            UpdateSummary();
        }

        private string GenerateKodePenyewaan()
        {
            try
            {
                string prefix = "PEN" + DateTime.Now.ToString("yyyyMMdd");
                string query = "SELECT COUNT(*) FROM penyewaans WHERE kode_penyewaan LIKE @prefix";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@prefix", prefix + "%")
                };
                object count = DatabaseConnection.ExecuteScalar(query, parameters);
                int next = Convert.ToInt32(count) + 1;
                return $"{prefix}{next:D4}";
            }
            catch
            {
                return "PEN" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private bool CreatePenyewaanInDatabase(PenyewaanModel penyewaan, List<DetailPenyewaanModel> items)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new MySqlCommand(@"
                            INSERT INTO penyewaans (kode_penyewaan, user_id, tanggal_pengajuan,
                                tanggal_mulai, tanggal_selesai, total_hari, subtotal, denda, total,
                                status, catatan, created_at, updated_at)
                            VALUES (@kode, @user_id, @tglPengajuan, @tglMulai, @tglSelesai,
                                @totalHari, @subtotal, @denda, @total, @status, @catatan, NOW(), NOW())", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@kode", penyewaan.KodePenyewaan);
                            cmd.Parameters.AddWithValue("@user_id", penyewaan.UserId);
                            cmd.Parameters.AddWithValue("@tglPengajuan", penyewaan.TanggalPengajuan);
                            cmd.Parameters.AddWithValue("@tglMulai", penyewaan.TanggalMulai);
                            cmd.Parameters.AddWithValue("@tglSelesai", penyewaan.TanggalSelesai);
                            cmd.Parameters.AddWithValue("@totalHari", penyewaan.TotalHari);
                            cmd.Parameters.AddWithValue("@subtotal", penyewaan.Subtotal);
                            cmd.Parameters.AddWithValue("@denda", penyewaan.Denda);
                            cmd.Parameters.AddWithValue("@total", penyewaan.Total);
                            cmd.Parameters.AddWithValue("@status", penyewaan.Status);
                            cmd.Parameters.AddWithValue("@catatan", (object)penyewaan.Catatan ?? DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }

                        ulong penyewaanId = 0;
                        using (var cmd = new MySqlCommand("SELECT LAST_INSERT_ID()", conn, trans))
                        {
                            penyewaanId = Convert.ToUInt64(cmd.ExecuteScalar());
                        }

                        foreach (var item in items)
                        {
                            using (var cmd = new MySqlCommand(@"
                                INSERT INTO detail_penyewaans (penyewaan_id, alat_id, jumlah,
                                    harga_sewa, subtotal, kondisi_sebelum, created_at, updated_at)
                                VALUES (@penyewaan_id, @alat_id, @jumlah, @harga, @subtotal,
                                    @kondisi, NOW(), NOW())", conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@penyewaan_id", penyewaanId);
                                cmd.Parameters.AddWithValue("@alat_id", item.AlatId);
                                cmd.Parameters.AddWithValue("@jumlah", item.Jumlah);
                                cmd.Parameters.AddWithValue("@harga", item.HargaSewa);
                                cmd.Parameters.AddWithValue("@subtotal", item.Subtotal);
                                cmd.Parameters.AddWithValue("@kondisi", (object)item.KondisiSebelum ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show($"Error menyimpan penyewaan: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow custRow = GetSelectedCustomerRow();
                if (custRow == null)
                {
                    MessageBox.Show("Silakan pilih customer terlebih dahulu!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbCustomer.Focus();
                    return;
                }

                if (_daftarAlat.Count == 0)
                {
                    MessageBox.Show("Minimal pilih satu alat untuk disewa!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DateTime mulai = dtMulai.Value.Date;
                DateTime selesai = dtSelesai.Value.Date;

                if (selesai < mulai)
                {
                    MessageBox.Show("Tanggal selesai tidak boleh sebelum tanggal mulai!", "Validasi Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int totalHari = HitungTotalHari();

                decimal subtotal = 0m;
                foreach (var item in _daftarAlat)
                {
                    subtotal += item.Subtotal;
                }

                var penyewaan = new PenyewaanModel
                {
                    KodePenyewaan = GenerateKodePenyewaan(),
                    UserId = Convert.ToUInt64(custRow["id"]),
                    TanggalPengajuan = dtPengajuan.Value.Date,
                    TanggalMulai = mulai,
                    TanggalSelesai = selesai,
                    TotalHari = totalHari,
                    Subtotal = subtotal,
                    Denda = 0m,
                    Total = subtotal,
                    Status = "pending",
                    Catatan = string.IsNullOrWhiteSpace(txtCatatan.Text) ? null : txtCatatan.Text.Trim()
                };

                if (CreatePenyewaanInDatabase(penyewaan, _daftarAlat))
                {
                    MessageBox.Show($"Penyewaan '{penyewaan.KodePenyewaan}' berhasil dibuat dengan status 'Menunggu'!",
                        "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gagal membuat penyewaan. Silakan coba lagi.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi kesalahan: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
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
    }
}