namespace App_Rental_Proyek.UserControls.Admin.Pembayaran
{
    partial class VerifikasiPembayaran
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblKode = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblInfoCust = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblInfoNominal = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblInfoMetode = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblInfoPenyewaan = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblSectionBukti = new Guna.UI2.WinForms.Guna2HtmlLabel();
            picBukti = new PictureBox();
            btnBukaBukti = new Guna.UI2.WinForms.Guna2Button();
            lblBuktiInfo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblCatatanLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtCatatan = new Guna.UI2.WinForms.Guna2TextBox();
            btnTerima = new Guna.UI2.WinForms.Guna2Button();
            btnTolak = new Guna.UI2.WinForms.Guna2Button();
            btnTutup = new Guna.UI2.WinForms.Guna2Button();
            guna2Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picBukti).BeginInit();
            SuspendLayout();
            // 
            // guna2Panel2
            // 
            guna2Panel2.BackColor = Color.FromArgb(23, 59, 99);
            guna2Panel2.Controls.Add(lblTitle);
            guna2Panel2.CustomizableEdges = customizableEdges1;
            guna2Panel2.Location = new Point(0, -3);
            guna2Panel2.Name = "guna2Panel2";
            guna2Panel2.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2Panel2.Size = new Size(720, 70);
            guna2Panel2.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(18, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(216, 39);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Verifikasi Pembayaran";
            // 
            // lblKode
            // 
            lblKode.BackColor = Color.Transparent;
            lblKode.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblKode.ForeColor = Color.FromArgb(23, 59, 99);
            lblKode.Location = new Point(18, 86);
            lblKode.Name = "lblKode";
            lblKode.Size = new Size(250, 39);
            lblKode.TabIndex = 1;
            lblKode.Text = "Kode Pembayaran";
            // 
            // lblStatus
            // 
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblStatus.Location = new Point(448, 92);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(180, 31);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Status: Menunggu";
            // 
            // lblInfoCust
            // 
            lblInfoCust.BackColor = Color.Transparent;
            lblInfoCust.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblInfoCust.ForeColor = Color.FromArgb(68, 88, 112);
            lblInfoCust.Location = new Point(18, 134);
            lblInfoCust.Name = "lblInfoCust";
            lblInfoCust.Size = new Size(450, 25);
            lblInfoCust.TabIndex = 3;
            lblInfoCust.Text = "Customer: -";
            // 
            // lblInfoNominal
            // 
            lblInfoNominal.BackColor = Color.Transparent;
            lblInfoNominal.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblInfoNominal.ForeColor = Color.FromArgb(23, 59, 99);
            lblInfoNominal.Location = new Point(18, 164);
            lblInfoNominal.Name = "lblInfoNominal";
            lblInfoNominal.Size = new Size(220, 33);
            lblInfoNominal.TabIndex = 4;
            lblInfoNominal.Text = "Nominal: Rp 0";
            // 
            // lblInfoMetode
            // 
            lblInfoMetode.BackColor = Color.Transparent;
            lblInfoMetode.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblInfoMetode.ForeColor = Color.FromArgb(68, 88, 112);
            lblInfoMetode.Location = new Point(448, 134);
            lblInfoMetode.Name = "lblInfoMetode";
            lblInfoMetode.Size = new Size(250, 25);
            lblInfoMetode.TabIndex = 5;
            lblInfoMetode.Text = "Metode: -";
            // 
            // lblInfoPenyewaan
            // 
            lblInfoPenyewaan.BackColor = Color.Transparent;
            lblInfoPenyewaan.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblInfoPenyewaan.ForeColor = Color.FromArgb(68, 88, 112);
            lblInfoPenyewaan.Location = new Point(448, 164);
            lblInfoPenyewaan.Name = "lblInfoPenyewaan";
            lblInfoPenyewaan.Size = new Size(250, 25);
            lblInfoPenyewaan.TabIndex = 6;
            lblInfoPenyewaan.Text = "Kode Sewa: -";
            // 
            // lblSectionBukti
            // 
            lblSectionBukti.BackColor = Color.Transparent;
            lblSectionBukti.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSectionBukti.ForeColor = Color.FromArgb(23, 59, 99);
            lblSectionBukti.Location = new Point(18, 216);
            lblSectionBukti.Name = "lblSectionBukti";
            lblSectionBukti.Size = new Size(160, 27);
            lblSectionBukti.TabIndex = 7;
            lblSectionBukti.Text = "Bukti Pembayaran";
            // 
            // picBukti
            // 
            picBukti.BackColor = Color.White;
            picBukti.BorderStyle = BorderStyle.FixedSingle;
            picBukti.Location = new Point(18, 244);
            picBukti.Name = "picBukti";
            picBukti.Size = new Size(340, 180);
            picBukti.SizeMode = PictureBoxSizeMode.Zoom;
            picBukti.TabIndex = 8;
            picBukti.TabStop = false;
            // 
            // btnBukaBukti
            // 
            btnBukaBukti.BorderRadius = 17;
            btnBukaBukti.CustomizableEdges = customizableEdges3;
            btnBukaBukti.DisabledState.BorderColor = Color.DarkGray;
            btnBukaBukti.DisabledState.CustomBorderColor = Color.DarkGray;
            btnBukaBukti.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnBukaBukti.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnBukaBukti.FillColor = Color.FromArgb(155, 89, 182);
            btnBukaBukti.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBukaBukti.ForeColor = Color.White;
            btnBukaBukti.Location = new Point(366, 262);
            btnBukaBukti.Name = "btnBukaBukti";
            btnBukaBukti.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnBukaBukti.Size = new Size(160, 44);
            btnBukaBukti.TabIndex = 9;
            btnBukaBukti.Text = "Buka Bukti";
            btnBukaBukti.Click += btnBukaBukti_Click;
            // 
            // lblBuktiInfo
            // 
            lblBuktiInfo.AutoSize = false;
            lblBuktiInfo.BackColor = Color.Transparent;
            lblBuktiInfo.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblBuktiInfo.ForeColor = Color.FromArgb(96, 110, 130);
            lblBuktiInfo.Location = new Point(366, 312);
            lblBuktiInfo.Name = "lblBuktiInfo";
            lblBuktiInfo.Size = new Size(336, 44);
            lblBuktiInfo.TabIndex = 10;
            lblBuktiInfo.Text = "-";
            // 
            // lblCatatanLabel
            // 
            lblCatatanLabel.BackColor = Color.Transparent;
            lblCatatanLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCatatanLabel.ForeColor = Color.FromArgb(23, 59, 99);
            lblCatatanLabel.Location = new Point(18, 444);
            lblCatatanLabel.Name = "lblCatatanLabel";
            lblCatatanLabel.Size = new Size(220, 25);
            lblCatatanLabel.TabIndex = 11;
            lblCatatanLabel.Text = "Catatan / Keterangan:";
            // 
            // txtCatatan
            // 
            txtCatatan.BorderColor = Color.FromArgb(23, 59, 99);
            txtCatatan.BorderRadius = 10;
            txtCatatan.CustomizableEdges = customizableEdges5;
            txtCatatan.DefaultText = "";
            txtCatatan.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtCatatan.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtCatatan.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtCatatan.FillColor = Color.White;
            txtCatatan.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtCatatan.Font = new Font("Segoe UI", 9.8F);
            txtCatatan.ForeColor = Color.FromArgb(23, 59, 99);
            txtCatatan.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtCatatan.Location = new Point(18, 472);
            txtCatatan.Multiline = true;
            txtCatatan.Name = "txtCatatan";
            txtCatatan.PlaceholderForeColor = Color.Gray;
            txtCatatan.PlaceholderText = "Contoh: Bukti transfer sesuai, pembayaran diterima.";
            txtCatatan.ScrollBars = ScrollBars.Vertical;
            txtCatatan.SelectedText = "";
            txtCatatan.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtCatatan.Size = new Size(684, 92);
            txtCatatan.TabIndex = 12;
            // 
            // btnTerima
            // 
            btnTerima.BorderRadius = 17;
            btnTerima.CustomizableEdges = customizableEdges7;
            btnTerima.DisabledState.BorderColor = Color.DarkGray;
            btnTerima.DisabledState.CustomBorderColor = Color.DarkGray;
            btnTerima.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnTerima.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnTerima.FillColor = Color.FromArgb(46, 204, 113);
            btnTerima.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnTerima.ForeColor = Color.White;
            btnTerima.Location = new Point(18, 580);
            btnTerima.Name = "btnTerima";
            btnTerima.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnTerima.Size = new Size(220, 44);
            btnTerima.TabIndex = 13;
            btnTerima.Text = "Verifikasi Lunas";
            btnTerima.Click += btnTerima_Click;
            // 
            // btnTolak
            // 
            btnTolak.BorderRadius = 17;
            btnTolak.CustomizableEdges = customizableEdges9;
            btnTolak.DisabledState.BorderColor = Color.DarkGray;
            btnTolak.DisabledState.CustomBorderColor = Color.DarkGray;
            btnTolak.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnTolak.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnTolak.FillColor = Color.FromArgb(231, 76, 60);
            btnTolak.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnTolak.ForeColor = Color.White;
            btnTolak.Location = new Point(250, 580);
            btnTolak.Name = "btnTolak";
            btnTolak.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btnTolak.Size = new Size(220, 44);
            btnTolak.TabIndex = 14;
            btnTolak.Text = "Tolak / Gagal";
            btnTolak.Click += btnTolak_Click;
            // 
            // btnTutup
            // 
            btnTutup.BackColor = Color.Transparent;
            btnTutup.BorderColor = Color.FromArgb(23, 59, 99);
            btnTutup.BorderRadius = 17;
            btnTutup.BorderThickness = 2;
            btnTutup.CustomizableEdges = customizableEdges11;
            btnTutup.DisabledState.BorderColor = Color.DarkGray;
            btnTutup.DisabledState.CustomBorderColor = Color.DarkGray;
            btnTutup.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnTutup.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnTutup.FillColor = Color.White;
            btnTutup.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnTutup.ForeColor = Color.FromArgb(23, 59, 99);
            btnTutup.Location = new Point(482, 580);
            btnTutup.Name = "btnTutup";
            btnTutup.ShadowDecoration.CustomizableEdges = customizableEdges12;
            btnTutup.Size = new Size(220, 44);
            btnTutup.TabIndex = 15;
            btnTutup.Text = "Batal";
            btnTutup.Click += btnTutup_Click;
            // 
            // VerifikasiPembayaran
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(720, 638);
            Controls.Add(guna2Panel2);
            Controls.Add(lblKode);
            Controls.Add(lblStatus);
            Controls.Add(lblInfoCust);
            Controls.Add(lblInfoNominal);
            Controls.Add(lblInfoMetode);
            Controls.Add(lblInfoPenyewaan);
            Controls.Add(lblSectionBukti);
            Controls.Add(picBukti);
            Controls.Add(btnBukaBukti);
            Controls.Add(lblBuktiInfo);
            Controls.Add(lblCatatanLabel);
            Controls.Add(txtCatatan);
            Controls.Add(btnTerima);
            Controls.Add(btnTolak);
            Controls.Add(btnTutup);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "VerifikasiPembayaran";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Verifikasi Pembayaran";
            Load += VerifikasiPembayaran_Load;
            guna2Panel2.ResumeLayout(false);
            guna2Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picBukti).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblKode;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblStatus;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblInfoCust;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblInfoNominal;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblInfoMetode;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblInfoPenyewaan;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSectionBukti;
        private PictureBox picBukti;
        private Guna.UI2.WinForms.Guna2Button btnBukaBukti;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblBuktiInfo;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCatatanLabel;
        private Guna.UI2.WinForms.Guna2TextBox txtCatatan;
        private Guna.UI2.WinForms.Guna2Button btnTerima;
        private Guna.UI2.WinForms.Guna2Button btnTolak;
        private Guna.UI2.WinForms.Guna2Button btnTutup;
    }
}