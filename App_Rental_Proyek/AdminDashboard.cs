using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using App_Rental_Proyek.UserControls.Admin; // Tambahkan using untuk UserList

namespace App_Rental_Proyek
{
    public partial class AdminDashboard : Form
    {
        private Control currentControl = null; // Gunakan Control sebagai tipe dasar
        private Guna.UI2.WinForms.Guna2Button currentButton = null;

        private readonly Color DefaultColor = Color.FromArgb(23, 59, 99);
        private readonly Color ActiveColor = Color.FromArgb(0, 123, 255);
        private readonly Color HoverColor = Color.FromArgb(30, 80, 130);

        public AdminDashboard()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            SetAllButtonsDefault();
            ShowDashboard();
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            // Inisialisasi tambahan jika diperlukan
        }

        // ============================================
        // METHOD UNTUK MENAMPILKAN USER CONTROL
        // ============================================
        private void ShowControl(Control control)
        {
            if (currentControl != null)
            {
                PanelKonten.Controls.Remove(currentControl);
                currentControl.Dispose();
                currentControl = null;
            }

            currentControl = control;
            control.Dock = DockStyle.Fill;
            PanelKonten.Controls.Add(control);
            control.BringToFront();
        }

        // ============================================
        // METHOD UNTUK MENGATUR TAMPILAN BUTTON
        // ============================================
        private void SetActiveButton(Guna.UI2.WinForms.Guna2Button activeButton)
        {
            if (currentButton != null && currentButton != activeButton)
            {
                SetButtonDefault(currentButton);
            }

            currentButton = activeButton;
            activeButton.FillColor = ActiveColor;
            activeButton.ForeColor = Color.White;
            activeButton.HoverState.FillColor = Color.FromArgb(0, 140, 255);
        }

        private void SetButtonDefault(Guna.UI2.WinForms.Guna2Button button)
        {
            button.FillColor = DefaultColor;
            button.ForeColor = Color.White;
            button.HoverState.FillColor = HoverColor;
        }

        private void SetAllButtonsDefault()
        {
            Guna.UI2.WinForms.Guna2Button[] buttons = {
                btnDashboard,
                btnUserManage,
                btnAlatProyek,
                btnCategory,
                btnLokasi,
                btnPenyewaan,
                btnPembayaran,
                btnPengembalian,
                btnActivityLog,
                btnDenda
            };

            foreach (Guna.UI2.WinForms.Guna2Button btn in buttons)
            {
                SetButtonDefault(btn);
            }
        }

        // ============================================
        // METHOD UNTUK MENAMPILKAN HALAMAN
        // ============================================
        private void ShowDashboard()
        {
            ShowControl(new UserControls.Admin.Dashboard1());
            SetActiveButton(btnDashboard);
        }

        private void ShowUserManagement()
        {
            ShowControl(new UserControls.Admin.UserList()); // Memanggil UserList
            SetActiveButton(btnUserManage);
        }

        private void ShowAlatProyek()
        {
            ShowControl(new UserControls.Admin.AlatProyekList());
            SetActiveButton(btnAlatProyek);
        }

        private void ShowCategory()
        {
            ShowControl(new UserControls.Admin.KategoriList());
            SetActiveButton(btnCategory);
        }

        private void ShowLokasi()
        {
            ShowControl(new UserControls.Admin.LokasiList());
            SetActiveButton(btnLokasi);
        }

        private void ShowPenyewaan()
        {
            // Ganti dengan UserControl Penyewaan yang sesuai
            // ShowControl(new UserControls.Penyewaan());
            SetActiveButton(btnPenyewaan);
        }

        private void ShowPembayaran()
        {
            // Ganti dengan UserControl Pembayaran yang sesuai
            // ShowControl(new UserControls.Pembayaran());
            SetActiveButton(btnPembayaran);
        }

        private void ShowPengembalian()
        {
            // Ganti dengan UserControl Pengembalian yang sesuai
            // ShowControl(new UserControls.Pengembalian());
            SetActiveButton(btnPengembalian);
        }

        private void ShowActivityLog()
        {
            // Ganti dengan UserControl ActivityLog yang sesuai
            // ShowControl(new UserControls.ActivityLog());
            SetActiveButton(btnActivityLog);
        }

        private void ShowDenda()
        {
            // Ganti dengan UserControl Denda yang sesuai
            // ShowControl(new UserControls.Denda());
            SetActiveButton(btnDenda);
        }

        // ============================================
        // EVENT HANDLERS UNTUK BUTTON NAVIGASI
        // ============================================
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void btnUserManage_Click(object sender, EventArgs e)
        {
            ShowUserManagement();
        }

        private void btnAlatProyek_Click(object sender, EventArgs e)
        {
            ShowAlatProyek();
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            ShowCategory();
        }

        private void btnLokasi_Click(object sender, EventArgs e)
        {
            ShowLokasi();
        }

        private void btnPenyewaan_Click(object sender, EventArgs e)
        {
            ShowPenyewaan();
        }

        private void btnPembayaran_Click(object sender, EventArgs e)
        {
            ShowPembayaran();
        }

        private void btnPengembalian_Click(object sender, EventArgs e)
        {
            ShowPengembalian();
        }

        private void btnActivityLog_Click(object sender, EventArgs e)
        {
            ShowActivityLog();
        }

        private void btnDenda_Click(object sender, EventArgs e)
        {
            ShowDenda();
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

        private void AdminDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(
                    "Apakah Anda yakin ingin keluar dari aplikasi?",
                    "Konfirmasi Keluar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Event handler untuk label (kosongkan jika tidak digunakan)
        }
    }
}