using App_Rental_Proyek.Config;
using App_Rental_Proyek.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.AlatProyek
{
    public partial class RiwayatAlatProyek : Form
    {
        private ulong _alatId;
        private string _namaAlat;

        public RiwayatAlatProyek(ulong alatId, string namaAlat)
        {
            InitializeComponent();
            _alatId = alatId;
            _namaAlat = namaAlat;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void RiwayatAlatProyek_Load(object sender, EventArgs e)
        {
            LoadAlatInfo();
            LoadRiwayat();
        }

        private void LoadAlatInfo()
        {
            try
            {
                string query = @"
                    SELECT a.id, a.kode_alat, a.nama_alat, a.stok, a.stok_tersedia, a.kondisi, a.status
                    FROM alat_proyeks a
                    WHERE a.id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _alatId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string kode = row["kode_alat"]?.ToString() ?? "-";
                    string nama = row["nama_alat"]?.ToString() ?? _namaAlat;
                    int stok = row["stok"] != DBNull.Value ? Convert.ToInt32(row["stok"]) : 0;
                    int tersedia = row["stok_tersedia"] != DBNull.Value ? Convert.ToInt32(row["stok_tersedia"]) : 0;
                    string kondisi = FormatKondisi(row["kondisi"]?.ToString() ?? "baik");
                    string status = FormatStatus(row["status"]?.ToString() ?? "tersedia");

                    lblInfo.Text = $"{kode} - {nama}";
                    lblStok.Text = $"Stok: {stok} (tersedia {tersedia})";
                    lblKondisi.Text = $"Kondisi: {kondisi}";
                    lblStatus.Text = $"Status: {status}";
                }
            }
            catch (Exception ex)
            {
                lblInfo.Text = _namaAlat;
                System.Diagnostics.Debug.WriteLine($"Error load info alat: {ex.Message}");
            }
        }

        private void LoadRiwayat()
        {
            InitializeGridView();

            try
            {
                string query = @"
                    SELECT p.kode_penyewaan, p.status AS status_penyewaan, p.tanggal_mulai, p.tanggal_selesai,
                           dp.jumlah, dp.harga_sewa, dp.subtotal, u.nama AS nama_user
                    FROM detail_penyewaans dp
                    INNER JOIN penyewaans p ON p.id = dp.penyewaan_id
                    LEFT JOIN users u ON u.id = p.user_id
                    WHERE dp.alat_id = @alatId
                    ORDER BY p.tanggal_mulai DESC";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@alatId", _alatId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                guna2DataGridView1.Rows.Clear();
                if (dt.Rows.Count == 0)
                {
                    guna2DataGridView1.Rows.Add("-", "-", "-", "-", "-", "Belum ada riwayat penyewaan");
                    return;
                }

                foreach (DataRow row in dt.Rows)
                {
                    string tglMulai = row["tanggal_mulai"] != DBNull.Value
                        ? Convert.ToDateTime(row["tanggal_mulai"]).ToString("dd/MM/yyyy") : "-";
                    string tglSelesai = row["tanggal_selesai"] != DBNull.Value
                        ? Convert.ToDateTime(row["tanggal_selesai"]).ToString("dd/MM/yyyy") : "-";

                    guna2DataGridView1.Rows.Add(
                        row["kode_penyewaan"]?.ToString() ?? "-",
                        tglMulai,
                        tglSelesai,
                        row["nama_user"]?.ToString() ?? "-",
                        row["jumlah"] != DBNull.Value ? row["jumlah"].ToString() : "0",
                        row["status_penyewaan"]?.ToString() ?? "-"
                    );
                }
            }
            catch (Exception ex)
            {
                guna2DataGridView1.Rows.Clear();
                guna2DataGridView1.Rows.Add("-", "-", "-", "-", "-", "Tabel penyewaan belum tersedia");
                System.Diagnostics.Debug.WriteLine($"Error load riwayat: {ex.Message}");
            }
        }

        private void InitializeGridView()
        {
            guna2DataGridView1.Columns.Clear();

            guna2DataGridView1.Columns.Add("Kode", "Kode Sewa");
            guna2DataGridView1.Columns.Add("Mulai", "Tgl Mulai");
            guna2DataGridView1.Columns.Add("Selesai", "Tgl Selesai");
            guna2DataGridView1.Columns.Add("User", "Penyewa");
            guna2DataGridView1.Columns.Add("Jumlah", "Jumlah");
            guna2DataGridView1.Columns.Add("Status", "Status");

            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;
        }

        private string FormatKondisi(string kondisi)
        {
            switch (kondisi)
            {
                case "rusak_ringan": return "Rusak Ringan";
                case "rusak_berat": return "Rusak Berat";
                default: return "Baik";
            }
        }

        private string FormatStatus(string status)
        {
            switch (status)
            {
                case "disewa": return "Disewa";
                case "maintenance": return "Maintenance";
                case "tidak_aktif": return "Tidak Aktif";
                default: return "Tersedia";
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
