namespace App_Rental_Proyek.UserControls.Petugas.Laporan
{
    partial class DownloadLaporanForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblJenis = new Guna.UI2.WinForms.Guna2HtmlLabel();
            cbJenisLaporan = new Guna.UI2.WinForms.Guna2ComboBox();
            lblPeriode = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblFormat = new Guna.UI2.WinForms.Guna2HtmlLabel();
            cbFormat = new Guna.UI2.WinForms.Guna2ComboBox();
            lblDari = new Guna.UI2.WinForms.Guna2HtmlLabel();
            dtpDari = new Guna.UI2.WinForms.Guna2DateTimePicker();
            lblSampai = new Guna.UI2.WinForms.Guna2HtmlLabel();
            dtpSampai = new Guna.UI2.WinForms.Guna2DateTimePicker();
            btnBatal = new Guna.UI2.WinForms.Guna2Button();
            btnDownload = new Guna.UI2.WinForms.Guna2Button();
            guna2Panel1.SuspendLayout();
            SuspendLayout();
            // 
            // guna2Panel1
            // 
            guna2Panel1.BackColor = Color.FromArgb(23, 59, 99);
            guna2Panel1.Controls.Add(lblTitle);
            guna2Panel1.CustomizableEdges = customizableEdges1;
            guna2Panel1.Location = new Point(0, -4);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2Panel1.Size = new Size(520, 70);
            guna2Panel1.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(18, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(210, 39);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Download Laporan";
            // 
            // lblJenis
            // 
            lblJenis.BackColor = Color.Transparent;
            lblJenis.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblJenis.ForeColor = Color.FromArgb(96, 110, 130);
            lblJenis.Location = new Point(24, 95);
            lblJenis.Name = "lblJenis";
            lblJenis.Size = new Size(100, 25);
            lblJenis.TabIndex = 1;
            lblJenis.Text = "Jenis Laporan";
            // 
            // cbJenisLaporan
            // 
            cbJenisLaporan.BackColor = Color.Transparent;
            cbJenisLaporan.BorderColor = Color.FromArgb(23, 59, 99);
            cbJenisLaporan.BorderRadius = 17;
            cbJenisLaporan.BorderThickness = 2;
            cbJenisLaporan.CustomizableEdges = customizableEdges3;
            cbJenisLaporan.DrawMode = DrawMode.OwnerDrawFixed;
            cbJenisLaporan.DropDownStyle = ComboBoxStyle.DropDownList;
            cbJenisLaporan.FocusedColor = Color.FromArgb(94, 148, 255);
            cbJenisLaporan.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cbJenisLaporan.Font = new Font("Segoe UI", 10F);
            cbJenisLaporan.ForeColor = Color.FromArgb(68, 88, 112);
            cbJenisLaporan.ItemHeight = 30;
            cbJenisLaporan.Location = new Point(24, 125);
            cbJenisLaporan.Name = "cbJenisLaporan";
            cbJenisLaporan.ShadowDecoration.CustomizableEdges = customizableEdges4;
            cbJenisLaporan.Size = new Size(460, 36);
            cbJenisLaporan.TabIndex = 2;
            // 
            // lblPeriode
            // 
            lblPeriode.BackColor = Color.Transparent;
            lblPeriode.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPeriode.ForeColor = Color.FromArgb(96, 110, 130);
            lblPeriode.Location = new Point(24, 280);
            lblPeriode.Name = "lblPeriode";
            lblPeriode.Size = new Size(120, 25);
            lblPeriode.TabIndex = 4;
            lblPeriode.Text = "Periode Tanggal";
            // 
            // lblFormat
            // 
            lblFormat.BackColor = Color.Transparent;
            lblFormat.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblFormat.ForeColor = Color.FromArgb(96, 110, 130);
            lblFormat.Location = new Point(24, 180);
            lblFormat.Name = "lblFormat";
            lblFormat.Size = new Size(100, 25);
            lblFormat.TabIndex = 3;
            lblFormat.Text = "Format File";
            // 
            // cbFormat
            // 
            cbFormat.BackColor = Color.Transparent;
            cbFormat.BorderColor = Color.FromArgb(23, 59, 99);
            cbFormat.BorderRadius = 17;
            cbFormat.BorderThickness = 2;
            cbFormat.CustomizableEdges = customizableEdges3;
            cbFormat.DrawMode = DrawMode.OwnerDrawFixed;
            cbFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            cbFormat.FocusedColor = Color.FromArgb(94, 148, 255);
            cbFormat.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cbFormat.Font = new Font("Segoe UI", 10F);
            cbFormat.ForeColor = Color.FromArgb(68, 88, 112);
            cbFormat.ItemHeight = 30;
            cbFormat.Location = new Point(24, 210);
            cbFormat.Name = "cbFormat";
            cbFormat.ShadowDecoration.CustomizableEdges = customizableEdges4;
            cbFormat.Size = new Size(460, 36);
            cbFormat.TabIndex = 4;
            // 
            // lblDari
            // 
            lblDari.BackColor = Color.Transparent;
            lblDari.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDari.ForeColor = Color.FromArgb(96, 110, 130);
            lblDari.Location = new Point(24, 325);
            lblDari.Name = "lblDari";
            lblDari.Size = new Size(40, 25);
            lblDari.TabIndex = 6;
            lblDari.Text = "Dari";
            // 
            // dtpDari
            // 
            dtpDari.BorderColor = Color.FromArgb(23, 59, 99);
            dtpDari.BorderRadius = 10;
            dtpDari.BorderThickness = 2;
            dtpDari.Checked = true;
            dtpDari.CustomizableEdges = customizableEdges3;
            dtpDari.FillColor = Color.White;
            dtpDari.Font = new Font("Segoe UI", 10F);
            dtpDari.ForeColor = Color.FromArgb(68, 88, 112);
            dtpDari.Format = DateTimePickerFormat.Short;
            dtpDari.Location = new Point(80, 318);
            dtpDari.MaxDate = new DateTime(2100, 12, 31, 0, 0, 0, 0);
            dtpDari.MinDate = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            dtpDari.Name = "dtpDari";
            dtpDari.ShadowDecoration.CustomizableEdges = customizableEdges4;
            dtpDari.Size = new Size(180, 40);
            dtpDari.TabIndex = 7;
            dtpDari.Value = new DateTime(2026, 1, 1, 0, 0, 0, 0);
            // 
            // lblSampai
            // 
            lblSampai.BackColor = Color.Transparent;
            lblSampai.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSampai.ForeColor = Color.FromArgb(96, 110, 130);
            lblSampai.Location = new Point(280, 325);
            lblSampai.Name = "lblSampai";
            lblSampai.Size = new Size(60, 25);
            lblSampai.TabIndex = 8;
            lblSampai.Text = "Sampai";
            // 
            // dtpSampai
            // 
            dtpSampai.BorderColor = Color.FromArgb(23, 59, 99);
            dtpSampai.BorderRadius = 10;
            dtpSampai.BorderThickness = 2;
            dtpSampai.Checked = true;
            dtpSampai.CustomizableEdges = customizableEdges3;
            dtpSampai.FillColor = Color.White;
            dtpSampai.Font = new Font("Segoe UI", 10F);
            dtpSampai.ForeColor = Color.FromArgb(68, 88, 112);
            dtpSampai.Format = DateTimePickerFormat.Short;
            dtpSampai.Location = new Point(340, 318);
            dtpSampai.MaxDate = new DateTime(2100, 12, 31, 0, 0, 0, 0);
            dtpSampai.MinDate = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            dtpSampai.Name = "dtpSampai";
            dtpSampai.ShadowDecoration.CustomizableEdges = customizableEdges4;
            dtpSampai.Size = new Size(144, 40);
            dtpSampai.TabIndex = 9;
            dtpSampai.Value = new DateTime(2026, 12, 31, 0, 0, 0, 0);
            // 
            // btnBatal
            // 
            btnBatal.BackColor = Color.Transparent;
            btnBatal.BorderColor = Color.FromArgb(23, 59, 99);
            btnBatal.BorderRadius = 17;
            btnBatal.BorderThickness = 2;
            btnBatal.CustomizableEdges = customizableEdges3;
            btnBatal.DisabledState.BorderColor = Color.DarkGray;
            btnBatal.DisabledState.CustomBorderColor = Color.DarkGray;
            btnBatal.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnBatal.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnBatal.FillColor = Color.White;
            btnBatal.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBatal.ForeColor = Color.FromArgb(23, 59, 99);
            btnBatal.ImageSize = new Size(22, 22);
            btnBatal.Location = new Point(120, 400);
            btnBatal.Name = "btnBatal";
            btnBatal.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnBatal.Size = new Size(130, 46);
            btnBatal.TabIndex = 10;
            btnBatal.Text = "Batal";
            btnBatal.Click += btnBatal_Click;
            // 
            // btnDownload
            // 
            btnDownload.BackColor = Color.Transparent;
            btnDownload.BorderColor = Color.FromArgb(23, 59, 99);
            btnDownload.BorderRadius = 17;
            btnDownload.BorderThickness = 2;
            btnDownload.CustomizableEdges = customizableEdges3;
            btnDownload.DisabledState.BorderColor = Color.DarkGray;
            btnDownload.DisabledState.CustomBorderColor = Color.DarkGray;
            btnDownload.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnDownload.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnDownload.FillColor = Color.FromArgb(23, 59, 99);
            btnDownload.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDownload.ForeColor = Color.White;
            btnDownload.ImageSize = new Size(22, 22);
            btnDownload.Location = new Point(270, 400);
            btnDownload.Name = "btnDownload";
            btnDownload.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnDownload.Size = new Size(130, 46);
            btnDownload.TabIndex = 11;
            btnDownload.Text = "Download";
            btnDownload.Click += btnDownload_Click;
            // 
            // DownloadLaporanForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 480);
            Controls.Add(guna2Panel1);
            Controls.Add(lblJenis);
            Controls.Add(cbJenisLaporan);
            Controls.Add(lblFormat);
            Controls.Add(cbFormat);
            Controls.Add(lblPeriode);
            Controls.Add(lblDari);
            Controls.Add(dtpDari);
            Controls.Add(lblSampai);
            Controls.Add(dtpSampai);
            Controls.Add(btnBatal);
            Controls.Add(btnDownload);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DownloadLaporanForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Download Laporan";
            guna2Panel1.ResumeLayout(false);
            guna2Panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblJenis;
        private Guna.UI2.WinForms.Guna2ComboBox cbJenisLaporan;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPeriode;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblFormat;
        private Guna.UI2.WinForms.Guna2ComboBox cbFormat;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDari;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpDari;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSampai;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpSampai;
        private Guna.UI2.WinForms.Guna2Button btnBatal;
        private Guna.UI2.WinForms.Guna2Button btnDownload;
    }
}
