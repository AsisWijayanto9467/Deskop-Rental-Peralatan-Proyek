using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace App_Rental_Proyek
{
    public partial class PetugasDashboard : Form
    {
        private UserControl currentControl = null;
        private Guna.UI2.WinForms.Guna2Button currentButton = null;

        private readonly Color DefaultColor = Color.FromArgb(23, 59, 99);
        private readonly Color ActiveColor = Color.FromArgb(0, 123, 255);
        private readonly Color HoverColor = Color.FromArgb(30, 80, 130);

        public PetugasDashboard()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            SetAllButtonsDefault();
            ShowDashboard();
        }

        private void PetugasDashboard_Load(object sender, EventArgs e)
        {
            // Inisialisasi tambahan jika diperlukan
        }

        // ============================================
        // METHOD UNTUK MENAMPILKAN USER CONTROL
        // ============================================
        private void ShowControl(UserControl control)
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
                btnPenyewaan,
                btnPembayaran,
                btnPersiapanAlat,
                btnPengembalian,
                btnDenda,
                btnLaporan,
                btnLokasi,
                btnAlatProyek
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
        }

        private void ShowPenyewaan()
        {
        }

        private void ShowPembayaran()
        {
        }

        private void ShowPersiapanAlat()
        {
        }

        private void ShowPengembalian()
        {
        }

        private void ShowDenda()
        {
        }

        private void ShowLaporan()
        {
        }

        private void ShowLokasi()
        {
        }

        private void ShowAlatProyek()
        {
        }

        // ============================================
        // EVENT HANDLERS UNTUK BUTTON NAVIGASI
        // ============================================
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void btnPenyewaan_Click(object sender, EventArgs e)
        {
            ShowPenyewaan();
        }

        private void btnPembayaran_Click(object sender, EventArgs e)
        {
            ShowPembayaran();
        }

        private void btnPersiapanAlat_Click(object sender, EventArgs e)
        {
            ShowPersiapanAlat();
        }

        private void btnPengembalian_Click(object sender, EventArgs e)
        {
            ShowPengembalian();
        }

        private void btnDenda_Click(object sender, EventArgs e)
        {
            ShowDenda();
        }

        private void btnLaporan_Click(object sender, EventArgs e)
        {
            ShowLaporan();
        }

        private void btnLokasi_Click(object sender, EventArgs e)
        {
            ShowLokasi();
        }

        private void btnAlatProyek_Click(object sender, EventArgs e)
        {
            ShowAlatProyek();
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

        private void PetugasDashboard_FormClosing(object sender, FormClosingEventArgs e)
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
    }
}