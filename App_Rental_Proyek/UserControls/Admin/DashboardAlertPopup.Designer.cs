namespace App_Rental_Proyek.UserControls.Admin
{
    partial class DashboardAlertPopup
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            guna2PanelHeader = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblJenis = new Guna.UI2.WinForms.Guna2HtmlLabel();
            cbJenis = new Guna.UI2.WinForms.Guna2ComboBox();
            lblInfo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            dgvPerhatian = new Guna.UI2.WinForms.Guna2DataGridView();
            btnTutup = new Guna.UI2.WinForms.Guna2Button();
            guna2PanelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPerhatian).BeginInit();
            SuspendLayout();
            // 
            // guna2PanelHeader
            // 
            guna2PanelHeader.BackColor = Color.FromArgb(23, 59, 99);
            guna2PanelHeader.Controls.Add(lblTitle);
            guna2PanelHeader.CustomizableEdges = customizableEdges1;
            guna2PanelHeader.FillColor = Color.FromArgb(23, 59, 99);
            guna2PanelHeader.Location = new Point(0, -3);
            guna2PanelHeader.Name = "guna2PanelHeader";
            guna2PanelHeader.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2PanelHeader.Size = new Size(820, 70);
            guna2PanelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(18, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(290, 39);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Transaksi Perlu Perhatian";
            // 
            // lblJenis
            // 
            lblJenis.BackColor = Color.Transparent;
            lblJenis.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblJenis.ForeColor = Color.FromArgb(96, 110, 130);
            lblJenis.Location = new Point(18, 88);
            lblJenis.Name = "lblJenis";
            lblJenis.Size = new Size(80, 25);
            lblJenis.TabIndex = 1;
            lblJenis.Text = "Tampilkan:";
            // 
            // cbJenis
            // 
            cbJenis.BackColor = Color.Transparent;
            cbJenis.BorderColor = Color.FromArgb(23, 59, 99);
            cbJenis.BorderRadius = 17;
            cbJenis.BorderThickness = 2;
            cbJenis.CustomizableEdges = customizableEdges3;
            cbJenis.DrawMode = DrawMode.OwnerDrawFixed;
            cbJenis.DropDownStyle = ComboBoxStyle.DropDownList;
            cbJenis.FocusedColor = Color.FromArgb(94, 148, 255);
            cbJenis.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cbJenis.Font = new Font("Segoe UI", 10F);
            cbJenis.ForeColor = Color.FromArgb(68, 88, 112);
            cbJenis.ItemHeight = 30;
            cbJenis.Items.AddRange(new object[] { "Semua", "Penyewaan Menunggu Proses", "Pembayaran Belum Diverifikasi" });
            cbJenis.Location = new Point(108, 82);
            cbJenis.Name = "cbJenis";
            cbJenis.ShadowDecoration.CustomizableEdges = customizableEdges4;
            cbJenis.Size = new Size(350, 36);
            cbJenis.TabIndex = 2;
            cbJenis.SelectedIndexChanged += cbJenis_SelectedIndexChanged;
            // 
            // lblInfo
            // 
            lblInfo.BackColor = Color.Transparent;
            lblInfo.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblInfo.ForeColor = Color.FromArgb(96, 110, 130);
            lblInfo.Location = new Point(18, 132);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(200, 24);
            lblInfo.TabIndex = 3;
            lblInfo.Text = "Memuat data...";
            // 
            // dgvPerhatian
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvPerhatian.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvPerhatian.BackgroundColor = Color.Silver;
            dgvPerhatian.BorderStyle = BorderStyle.FixedSingle;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(23, 59, 99);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvPerhatian.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvPerhatian.ColumnHeadersHeight = 36;
            dgvPerhatian.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvPerhatian.DefaultCellStyle = dataGridViewCellStyle3;
            dgvPerhatian.GridColor = Color.FromArgb(231, 229, 255);
            dgvPerhatian.Location = new Point(18, 162);
            dgvPerhatian.Name = "dgvPerhatian";
            dgvPerhatian.RowHeadersVisible = false;
            dgvPerhatian.RowHeadersWidth = 51;
            dgvPerhatian.Size = new Size(784, 320);
            dgvPerhatian.TabIndex = 4;
            dgvPerhatian.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvPerhatian.ThemeStyle.BackColor = Color.Silver;
            dgvPerhatian.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvPerhatian.ThemeStyle.HeaderStyle.Height = 36;
            dgvPerhatian.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvPerhatian.ThemeStyle.RowsStyle.Height = 30;
            // 
            // btnTutup
            // 
            btnTutup.BackColor = Color.Transparent;
            btnTutup.BorderRadius = 20;
            btnTutup.CustomizableEdges = customizableEdges5;
            btnTutup.DisabledState.BorderColor = Color.DarkGray;
            btnTutup.DisabledState.CustomBorderColor = Color.DarkGray;
            btnTutup.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnTutup.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnTutup.FillColor = Color.FromArgb(23, 59, 99);
            btnTutup.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnTutup.ForeColor = Color.White;
            btnTutup.Location = new Point(700, 496);
            btnTutup.Name = "btnTutup";
            btnTutup.ShadowDecoration.CustomizableEdges = customizableEdges5;
            btnTutup.Size = new Size(102, 40);
            btnTutup.TabIndex = 5;
            btnTutup.Text = "Tutup";
            btnTutup.Click += btnTutup_Click;
            // 
            // DashboardAlertPopup
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(820, 546);
            Controls.Add(btnTutup);
            Controls.Add(dgvPerhatian);
            Controls.Add(lblInfo);
            Controls.Add(cbJenis);
            Controls.Add(lblJenis);
            Controls.Add(guna2PanelHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DashboardAlertPopup";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Transaksi Perlu Perhatian";
            Load += DashboardAlertPopup_Load;
            guna2PanelHeader.ResumeLayout(false);
            guna2PanelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPerhatian).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2PanelHeader;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblJenis;
        private Guna.UI2.WinForms.Guna2ComboBox cbJenis;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblInfo;
        private Guna.UI2.WinForms.Guna2DataGridView dgvPerhatian;
        private Guna.UI2.WinForms.Guna2Button btnTutup;
    }
}