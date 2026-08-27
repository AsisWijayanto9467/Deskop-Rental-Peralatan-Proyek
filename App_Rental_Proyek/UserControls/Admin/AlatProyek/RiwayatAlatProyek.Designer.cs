namespace App_Rental_Proyek.UserControls.Admin.AlatProyek
{
    partial class RiwayatAlatProyek
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblInfo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2DataGridView1 = new Guna.UI2.WinForms.Guna2DataGridView();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblKondisi = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblStok = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnTutup = new Guna.UI2.WinForms.Guna2Button();
            guna2Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)guna2DataGridView1).BeginInit();
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
            guna2Panel2.Size = new Size(794, 70);
            guna2Panel2.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(18, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(151, 39);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Riwayat Alat";
            // 
            // lblInfo
            // 
            lblInfo.BackColor = Color.Transparent;
            lblInfo.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblInfo.ForeColor = Color.FromArgb(23, 59, 99);
            lblInfo.Location = new Point(18, 86);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(200, 25);
            lblInfo.TabIndex = 1;
            lblInfo.Text = "Detail Alat";
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.ForeColor = Color.FromArgb(23, 59, 99);
            guna2HtmlLabel1.Location = new Point(18, 120);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(100, 25);
            guna2HtmlLabel1.TabIndex = 2;
            guna2HtmlLabel1.Text = "Riwayat Sewa";
            // 
            // lblKondisi
            // 
            lblKondisi.BackColor = Color.Transparent;
            lblKondisi.Font = new Font("Segoe UI", 9.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblKondisi.ForeColor = Color.FromArgb(68, 88, 112);
            lblKondisi.Location = new Point(496, 86);
            lblKondisi.Name = "lblKondisi";
            lblKondisi.Size = new Size(120, 24);
            lblKondisi.TabIndex = 3;
            lblKondisi.Text = "Kondisi: -";
            // 
            // lblStok
            // 
            lblStok.BackColor = Color.Transparent;
            lblStok.Font = new Font("Segoe UI", 9.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblStok.ForeColor = Color.FromArgb(68, 88, 112);
            lblStok.Location = new Point(618, 86);
            lblStok.Name = "lblStok";
            lblStok.Size = new Size(120, 24);
            lblStok.TabIndex = 4;
            lblStok.Text = "Stok: -";
            // 
            // lblStatus
            // 
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Segoe UI", 9.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblStatus.ForeColor = Color.FromArgb(68, 88, 112);
            lblStatus.Location = new Point(496, 116);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(120, 24);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Status: -";
            // 
            // guna2DataGridView1
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            guna2DataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            guna2DataGridView1.BackgroundColor = Color.Silver;
            guna2DataGridView1.BorderStyle = BorderStyle.FixedSingle;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            guna2DataGridView1.ColumnHeadersHeight = 40;
            guna2DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            guna2DataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            guna2DataGridView1.GridColor = Color.FromArgb(231, 229, 255);
            guna2DataGridView1.Location = new Point(18, 152);
            guna2DataGridView1.Name = "guna2DataGridView1";
            guna2DataGridView1.RowHeadersVisible = false;
            guna2DataGridView1.RowHeadersWidth = 51;
            guna2DataGridView1.Size = new Size(758, 428);
            guna2DataGridView1.TabIndex = 23;
            guna2DataGridView1.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            guna2DataGridView1.ThemeStyle.BackColor = Color.Silver;
            guna2DataGridView1.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            guna2DataGridView1.ThemeStyle.HeaderStyle.Height = 40;
            guna2DataGridView1.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            guna2DataGridView1.ThemeStyle.RowsStyle.Height = 28;
            // 
            // btnTutup
            // 
            btnTutup.BackColor = Color.Transparent;
            btnTutup.BorderColor = Color.FromArgb(23, 59, 99);
            btnTutup.BorderRadius = 17;
            btnTutup.BorderThickness = 2;
            btnTutup.CustomizableEdges = customizableEdges3;
            btnTutup.DisabledState.BorderColor = Color.DarkGray;
            btnTutup.DisabledState.CustomBorderColor = Color.DarkGray;
            btnTutup.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnTutup.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnTutup.FillColor = Color.FromArgb(23, 59, 99);
            btnTutup.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnTutup.ForeColor = Color.White;
            btnTutup.ImageSize = new Size(22, 22);
            btnTutup.Location = new Point(326, 600);
            btnTutup.Name = "btnTutup";
            btnTutup.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnTutup.Size = new Size(144, 40);
            btnTutup.TabIndex = 24;
            btnTutup.Text = "Tutup";
            btnTutup.Click += btnTutup_Click;
            // 
            // RiwayatAlatProyek
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(794, 656);
            Controls.Add(guna2Panel2);
            Controls.Add(lblInfo);
            Controls.Add(guna2HtmlLabel1);
            Controls.Add(lblKondisi);
            Controls.Add(lblStok);
            Controls.Add(lblStatus);
            Controls.Add(guna2DataGridView1);
            Controls.Add(btnTutup);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RiwayatAlatProyek";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Riwayat Alat";
            Load += RiwayatAlatProyek_Load;
            guna2Panel2.ResumeLayout(false);
            guna2Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)guna2DataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblInfo;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblKondisi;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblStok;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblStatus;
        private Guna.UI2.WinForms.Guna2DataGridView guna2DataGridView1;
        private Guna.UI2.WinForms.Guna2Button btnTutup;
    }
}
