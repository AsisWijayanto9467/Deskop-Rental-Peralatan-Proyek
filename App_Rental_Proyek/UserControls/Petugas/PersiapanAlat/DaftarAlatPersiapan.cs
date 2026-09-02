using App_Rental_Proyek.Config;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Petugas.PersiapanAlat
{
    public partial class DaftarAlatPersiapan : Form
    {
        private ulong _penyewaanId;
        private string _kodePenyewaan;
        private string _namaCustomer;

        public DaftarAlatPersiapan(ulong penyewaanId)
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
            this.Size = new Size(900, 600);
        }

        private void DaftarAlatPersiapan_Load(object sender, EventArgs e)
        {
            LoadHeader();
            LoadAlatList();
        }

        private void LoadHeader()
        {
            try
            {
                string query = @"
                    SELECT p.kode_penyewaan, u.nama AS nama_customer
                    FROM penyewaans p
                    LEFT JOIN users u ON u.id = p.user_id
                    WHERE p.id = @id";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _penyewaanId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    _kodePenyewaan = dt.Rows[0]["kode_penyewaan"]?.ToString() ?? "-";
                    _namaCustomer = dt.Rows[0]["nama_customer"]?.ToString() ?? "-";

                    lblKodePenyewaan.Text = _kodePenyewaan;
                    lblCustomer.Text = _namaCustomer;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error load header: {ex.Message}");
            }
        }

        private void InitializeGridView()
        {
            guna2DataGridView1.Columns.Clear();

            guna2DataGridView1.Columns.Add("Id", "ID");
            guna2DataGridView1.Columns.Add("KodeAlat", "Kode Alat");
            guna2DataGridView1.Columns.Add("NamaAlat", "Nama Alat");
            guna2DataGridView1.Columns.Add("Jumlah", "Jumlah");
            guna2DataGridView1.Columns.Add("Kondisi", "Kondisi");
            guna2DataGridView1.Columns.Add("StokTersedia", "Stok Tersedia");
            guna2DataGridView1.Columns.Add("StatusAlat", "Status Alat");
            guna2DataGridView1.Columns.Add("Tersedia", "Tersedia?");

            guna2DataGridView1.Columns["Id"].Visible = false;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;

            guna2DataGridView1.Columns["KodeAlat"].MinimumWidth = 100;
            guna2DataGridView1.Columns["NamaAlat"].MinimumWidth = 200;
            guna2DataGridView1.Columns["Jumlah"].Width = 80;
            guna2DataGridView1.Columns["Kondisi"].Width = 120;
            guna2DataGridView1.Columns["StokTersedia"].Width = 100;
            guna2DataGridView1.Columns["StatusAlat"].Width = 120;
            guna2DataGridView1.Columns["Tersedia"].Width = 80;
        }

        private void LoadAlatList()
        {
            InitializeGridView();

            try
            {
                string query = @"
                    SELECT dp.id, dp.jumlah, dp.kondisi_sebelum,
                           a.id AS alat_id, a.kode_alat, a.nama_alat,
                           a.stok, a.stok_tersedia, a.kondisi, a.status
                    FROM detail_penyewaans dp
                    LEFT JOIN alat_proyeks a ON a.id = dp.alat_id
                    WHERE dp.penyewaan_id = @id
                    ORDER BY a.nama_alat ASC";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@id", _penyewaanId)
                };

                DataTable dt = DatabaseConnection.GetData(query, parameters);

                guna2DataGridView1.Rows.Clear();

                if (dt.Rows.Count == 0)
                {
                    guna2DataGridView1.Rows.Add("-", "-", "-", "-", "-", "-", "-", "-");
                    return;
                }

                int allAvailable = 0;
                int totalItems = dt.Rows.Count;

                foreach (DataRow row in dt.Rows)
                {
                    string kodeAlat = row["kode_alat"]?.ToString() ?? "-";
                    string namaAlat = row["nama_alat"]?.ToString() ?? "-";
                    int jumlah = row["jumlah"] != DBNull.Value ? Convert.ToInt32(row["jumlah"]) : 1;
                    string kondisi = row["kondisi"]?.ToString() ?? "baik";
                    int stok = row["stok"] != DBNull.Value ? Convert.ToInt32(row["stok"]) : 0;
                    int stokTersedia = row["stok_tersedia"] != DBNull.Value ? Convert.ToInt32(row["stok_tersedia"]) : 0;
                    string statusAlat = row["status"]?.ToString() ?? "tersedia";

                    bool tersedia = stokTersedia >= jumlah;
                    if (tersedia) allAvailable++;

                    int rowIndex = guna2DataGridView1.Rows.Add(
                        row["id"],
                        kodeAlat,
                        namaAlat,
                        jumlah,
                        FormatKondisi(kondisi),
                        stokTersedia.ToString(),
                        FormatStatusAlat(statusAlat),
                        tersedia ? "✅ Ya" : "❌ Tidak"
                    );

                    if (!tersedia)
                    {
                        guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                        guna2DataGridView1.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 243);
                    }
                }

                lblSummary.Text = $"Total: {totalItems} jenis alat | Tersedia: {allAvailable} | Kurang: {totalItems - allAvailable}";
                lblSummary.ForeColor = allAvailable == totalItems ? Color.FromArgb(46, 204, 113) : Color.FromArgb(231, 76, 60);
            }
            catch (Exception ex)
            {
                guna2DataGridView1.Rows.Clear();
                guna2DataGridView1.Rows.Add("-", "-", "-", "-", "-", "-", "-", "Error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Error load alat: {ex.Message}");
            }
        }

        private string FormatKondisi(string kondisi)
        {
            switch (kondisi)
            {
                case "baik": return "✅ Baik";
                case "rusak_ringan": return "⚠️ Rusak Ringan";
                case "rusak_berat": return "❌ Rusak Berat";
                default: return kondisi;
            }
        }

        private string FormatStatusAlat(string status)
        {
            switch (status)
            {
                case "tersedia": return "✅ Tersedia";
                case "disewa": return "🚛 Disewa";
                case "maintenance": return "🔧 Maintenance";
                case "tidak_aktif": return "🚫 Tidak Aktif";
                default: return status;
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}