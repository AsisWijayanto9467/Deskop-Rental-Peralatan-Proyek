using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace App_Rental_Proyek
{
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Apakah Anda yakin ingin logout?",
                "Konfirmasi Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Login loginForm = new Login();
                loginForm.Show();
                this.Close();
            }
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {

        }

        private void btnUserManage_Click(object sender, EventArgs e)
        {

        }

        private void btnAlatProyek_Click(object sender, EventArgs e)
        {

        }

        private void btnCategory_Click(object sender, EventArgs e)
        {

        }

        private void btnLokasi_Click(object sender, EventArgs e)
        {

        }

        private void btnPenyewaan_Click(object sender, EventArgs e)
        {

        }

        private void btnPembayaran_Click(object sender, EventArgs e)
        {

        }

        private void btnPengembalian_Click(object sender, EventArgs e)
        {

        }

        private void btnActivityLog_Click(object sender, EventArgs e)
        {

        }

        private void btnDenda_Click(object sender, EventArgs e)
        {

        }
    }
}
