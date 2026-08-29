namespace App_Rental_Proyek.UserControls.Admin.ActivityLog
{
    partial class ActivityLogList
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            btnRefresh = new Guna.UI2.WinForms.Guna2Button();
            lbTotalLog = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lbPetunjukHalaman = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnNext = new Guna.UI2.WinForms.Guna2Button();
            btnPrev = new Guna.UI2.WinForms.Guna2Button();
            guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            cbRole = new Guna.UI2.WinForms.Guna2ComboBox();
            cbModul = new Guna.UI2.WinForms.Guna2ComboBox();
            txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            guna2DataGridView1 = new Guna.UI2.WinForms.Guna2DataGridView();
            ((System.ComponentModel.ISupportInitialize)guna2DataGridView1).BeginInit();
            SuspendLayout();
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.Transparent;
            btnRefresh.BorderColor = Color.FromArgb(23, 59, 99);
            btnRefresh.BorderRadius = 20;
            btnRefresh.BorderThickness = 2;
            btnRefresh.CustomizableEdges = customizableEdges1;
            btnRefresh.DisabledState.BorderColor = Color.DarkGray;
            btnRefresh.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRefresh.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRefresh.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRefresh.FillColor = Color.White;
            btnRefresh.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRefresh.ForeColor = Color.FromArgb(23, 59, 99);
            btnRefresh.ImageSize = new Size(22, 22);
            btnRefresh.Location = new Point(1353, 48);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnRefresh.Size = new Size(186, 44);
            btnRefresh.TabIndex = 19;
            btnRefresh.Text = "Refresh";
            btnRefresh.Click += btnRefresh_Click;
            // 
            // lbTotalLog
            // 
            lbTotalLog.BackColor = Color.Transparent;
            lbTotalLog.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbTotalLog.ForeColor = Color.FromArgb(23, 59, 99);
            lbTotalLog.Location = new Point(1390, 828);
            lbTotalLog.Name = "lbTotalLog";
            lbTotalLog.Size = new Size(148, 25);
            lbTotalLog.TabIndex = 18;
            lbTotalLog.Text = "Total: 0 log";
            lbTotalLog.Click += lbTotalLog_Click;
            // 
            // lbPetunjukHalaman
            // 
            lbPetunjukHalaman.BackColor = Color.Transparent;
            lbPetunjukHalaman.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbPetunjukHalaman.ForeColor = Color.FromArgb(23, 59, 99);
            lbPetunjukHalaman.Location = new Point(163, 828);
            lbPetunjukHalaman.Name = "lbPetunjukHalaman";
            lbPetunjukHalaman.Size = new Size(132, 25);
            lbPetunjukHalaman.TabIndex = 17;
            lbPetunjukHalaman.Text = "Halaman 1 dari 1";
            lbPetunjukHalaman.Click += lbPetunjukHalaman_Click;
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.Transparent;
            btnNext.BorderColor = Color.FromArgb(23, 59, 99);
            btnNext.BorderRadius = 20;
            btnNext.BorderThickness = 2;
            btnNext.CustomizableEdges = customizableEdges3;
            btnNext.DisabledState.BorderColor = Color.DarkGray;
            btnNext.DisabledState.CustomBorderColor = Color.DarkGray;
            btnNext.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnNext.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnNext.FillColor = Color.White;
            btnNext.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNext.ForeColor = Color.FromArgb(23, 59, 99);
            btnNext.ImageAlign = HorizontalAlignment.Right;
            btnNext.ImageSize = new Size(22, 22);
            btnNext.Location = new Point(315, 819);
            btnNext.Name = "btnNext";
            btnNext.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnNext.Size = new Size(133, 44);
            btnNext.TabIndex = 16;
            btnNext.Text = "Next";
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.BackColor = Color.Transparent;
            btnPrev.BorderColor = Color.FromArgb(23, 59, 99);
            btnPrev.BorderRadius = 20;
            btnPrev.BorderThickness = 2;
            btnPrev.CustomizableEdges = customizableEdges5;
            btnPrev.DisabledState.BorderColor = Color.DarkGray;
            btnPrev.DisabledState.CustomBorderColor = Color.DarkGray;
            btnPrev.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnPrev.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnPrev.FillColor = Color.White;
            btnPrev.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrev.ForeColor = Color.FromArgb(23, 59, 99);
            btnPrev.ImageAlign = HorizontalAlignment.Left;
            btnPrev.ImageSize = new Size(22, 22);
            btnPrev.Location = new Point(15, 819);
            btnPrev.Name = "btnPrev";
            btnPrev.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnPrev.Size = new Size(133, 44);
            btnPrev.TabIndex = 15;
            btnPrev.Text = "Prev";
            btnPrev.Click += btnPrev_Click;
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            guna2HtmlLabel2.Location = new Point(16, 67);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(318, 25);
            guna2HtmlLabel2.TabIndex = 14;
            guna2HtmlLabel2.Text = "Riwayat aktivitas pengguna (audit trail)";
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.Location = new Point(16, 22);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(160, 39);
            guna2HtmlLabel1.TabIndex = 13;
            guna2HtmlLabel1.Text = "Activity Log";
            // 
            // cbRole
            // 
            cbRole.BackColor = Color.Transparent;
            cbRole.BorderColor = Color.FromArgb(23, 59, 99);
            cbRole.BorderRadius = 17;
            cbRole.BorderThickness = 2;
            cbRole.CustomizableEdges = customizableEdges7;
            cbRole.DrawMode = DrawMode.OwnerDrawFixed;
            cbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRole.FocusedColor = Color.FromArgb(94, 148, 255);
            cbRole.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cbRole.Font = new Font("Segoe UI", 10F);
            cbRole.ForeColor = Color.FromArgb(68, 88, 112);
            cbRole.ItemHeight = 30;
            cbRole.Location = new Point(1381, 114);
            cbRole.Name = "cbRole";
            cbRole.ShadowDecoration.CustomizableEdges = customizableEdges8;
            cbRole.Size = new Size(158, 36);
            cbRole.TabIndex = 22;
            cbRole.SelectedIndexChanged += cbRole_SelectedIndexChanged;
            // 
            // cbModul
            // 
            cbModul.BackColor = Color.Transparent;
            cbModul.BorderColor = Color.FromArgb(23, 59, 99);
            cbModul.BorderRadius = 17;
            cbModul.BorderThickness = 2;
            cbModul.CustomizableEdges = customizableEdges9;
            cbModul.DrawMode = DrawMode.OwnerDrawFixed;
            cbModul.DropDownStyle = ComboBoxStyle.DropDownList;
            cbModul.FocusedColor = Color.FromArgb(94, 148, 255);
            cbModul.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cbModul.Font = new Font("Segoe UI", 10F);
            cbModul.ForeColor = Color.FromArgb(68, 88, 112);
            cbModul.ItemHeight = 30;
            cbModul.Location = new Point(1218, 114);
            cbModul.Name = "cbModul";
            cbModul.ShadowDecoration.CustomizableEdges = customizableEdges10;
            cbModul.Size = new Size(157, 36);
            cbModul.TabIndex = 21;
            cbModul.SelectedIndexChanged += cbModul_SelectedIndexChanged;
            // 
            // txtSearch
            // 
            txtSearch.BackColor = SystemColors.Control;
            txtSearch.BorderColor = Color.FromArgb(23, 59, 99);
            txtSearch.BorderRadius = 10;
            txtSearch.BorderThickness = 2;
            txtSearch.CustomizableEdges = customizableEdges11;
            txtSearch.DefaultText = "";
            txtSearch.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtSearch.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtSearch.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtSearch.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtSearch.FillColor = SystemColors.Control;
            txtSearch.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearch.Font = new Font("Segoe UI", 9F);
            txtSearch.ForeColor = Color.FromArgb(23, 59, 99);
            txtSearch.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearch.Location = new Point(16, 113);
            txtSearch.Margin = new Padding(3, 4, 3, 4);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderForeColor = Color.Gray;
            txtSearch.PlaceholderText = "Cari user, aktivitas, modul, referensi, IP...";
            txtSearch.SelectedText = "";
            txtSearch.ShadowDecoration.CustomizableEdges = customizableEdges12;
            txtSearch.Size = new Size(1196, 37);
            txtSearch.TabIndex = 20;
            txtSearch.TextChanged += txtSearch_TextChanged;
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
            guna2DataGridView1.Location = new Point(16, 157);
            guna2DataGridView1.Name = "guna2DataGridView1";
            guna2DataGridView1.RowHeadersVisible = false;
            guna2DataGridView1.RowHeadersWidth = 51;
            guna2DataGridView1.Size = new Size(1523, 640);
            guna2DataGridView1.TabIndex = 23;
            guna2DataGridView1.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            guna2DataGridView1.ThemeStyle.BackColor = Color.Silver;
            guna2DataGridView1.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            guna2DataGridView1.ThemeStyle.HeaderStyle.Height = 40;
            guna2DataGridView1.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            guna2DataGridView1.ThemeStyle.RowsStyle.Height = 29;
            guna2DataGridView1.CellContentClick += guna2DataGridView1_CellContentClick_1;
            // 
            // ActivityLogList
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(guna2DataGridView1);
            Controls.Add(btnRefresh);
            Controls.Add(lbTotalLog);
            Controls.Add(lbPetunjukHalaman);
            Controls.Add(btnNext);
            Controls.Add(btnPrev);
            Controls.Add(guna2HtmlLabel2);
            Controls.Add(guna2HtmlLabel1);
            Controls.Add(cbRole);
            Controls.Add(cbModul);
            Controls.Add(txtSearch);
            Name = "ActivityLogList";
            Size = new Size(1555, 914);
            Load += ActivityLogList_Load;
            ((System.ComponentModel.ISupportInitialize)guna2DataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button btnRefresh;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbTotalLog;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbPetunjukHalaman;
        private Guna.UI2.WinForms.Guna2Button btnNext;
        private Guna.UI2.WinForms.Guna2Button btnPrev;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2ComboBox cbRole;
        private Guna.UI2.WinForms.Guna2ComboBox cbModul;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2DataGridView guna2DataGridView1;
    }
}
