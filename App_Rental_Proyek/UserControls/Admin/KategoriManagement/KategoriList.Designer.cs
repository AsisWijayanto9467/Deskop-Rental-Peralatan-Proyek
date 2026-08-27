namespace App_Rental_Proyek.UserControls.Admin
{
    partial class KategoriList
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            btnTambah = new Guna.UI2.WinForms.Guna2Button();
            dgvKategori = new Guna.UI2.WinForms.Guna2DataGridView();
            lblTotal = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblHalaman = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnNext = new Guna.UI2.WinForms.Guna2Button();
            btnPrev = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)dgvKategori).BeginInit();
            SuspendLayout();
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.Location = new Point(20, 20);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(273, 39);
            guna2HtmlLabel1.TabIndex = 0;
            guna2HtmlLabel1.Text = "Manajemen Kategori";
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            guna2HtmlLabel2.Location = new Point(21, 64);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(412, 25);
            guna2HtmlLabel2.TabIndex = 1;
            guna2HtmlLabel2.Text = "Digunakan untuk mengelompokkan alat pada proyek.";
            // 
            // txtSearch
            // 
            txtSearch.BackColor = SystemColors.Control;
            txtSearch.BorderColor = Color.FromArgb(23, 59, 99);
            txtSearch.BorderRadius = 10;
            txtSearch.BorderThickness = 2;
            txtSearch.CustomizableEdges = customizableEdges1;
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
            txtSearch.Location = new Point(20, 106);
            txtSearch.Margin = new Padding(3, 4, 3, 4);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderForeColor = Color.Gray;
            txtSearch.PlaceholderText = "Cari nama kategori...";
            txtSearch.SelectedText = "";
            txtSearch.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtSearch.Size = new Size(520, 37);
            txtSearch.TabIndex = 2;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // btnTambah
            // 
            btnTambah.BackColor = Color.Transparent;
            btnTambah.BorderColor = Color.FromArgb(23, 59, 99);
            btnTambah.BorderRadius = 20;
            btnTambah.BorderThickness = 2;
            btnTambah.CustomizableEdges = customizableEdges3;
            btnTambah.DisabledState.BorderColor = Color.DarkGray;
            btnTambah.DisabledState.CustomBorderColor = Color.DarkGray;
            btnTambah.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnTambah.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnTambah.FillColor = Color.FromArgb(23, 59, 99);
            btnTambah.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnTambah.ForeColor = Color.White;
            btnTambah.ImageSize = new Size(22, 22);
            btnTambah.Location = new Point(1346, 102);
            btnTambah.Name = "btnTambah";
            btnTambah.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnTambah.Size = new Size(186, 44);
            btnTambah.TabIndex = 3;
            btnTambah.Text = "Tambah Kategori";
            btnTambah.Click += btnTambah_Click;
            // 
            // dgvKategori
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvKategori.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvKategori.BackgroundColor = Color.Silver;
            dgvKategori.BorderStyle = BorderStyle.FixedSingle;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(23, 59, 99);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvKategori.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvKategori.ColumnHeadersHeight = 40;
            dgvKategori.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvKategori.DefaultCellStyle = dataGridViewCellStyle3;
            dgvKategori.GridColor = Color.FromArgb(231, 229, 255);
            dgvKategori.Location = new Point(20, 155);
            dgvKategori.Name = "dgvKategori";
            dgvKategori.RowHeadersVisible = false;
            dgvKategori.RowHeadersWidth = 51;
            dgvKategori.Size = new Size(1512, 674);
            dgvKategori.TabIndex = 4;
            dgvKategori.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvKategori.ThemeStyle.BackColor = Color.Silver;
            dgvKategori.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(23, 59, 99);
            dgvKategori.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvKategori.ThemeStyle.HeaderStyle.Height = 40;
            dgvKategori.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvKategori.ThemeStyle.RowsStyle.Height = 29;
            dgvKategori.CellContentClick += dgvKategori_CellContentClick;
            // 
            // lblTotal
            // 
            lblTotal.BackColor = Color.Transparent;
            lblTotal.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotal.ForeColor = Color.FromArgb(23, 59, 99);
            lblTotal.Location = new Point(1350, 862);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(59, 25);
            lblTotal.TabIndex = 5;
            lblTotal.Text = "Total: 0";
            // 
            // lblHalaman
            // 
            lblHalaman.BackColor = Color.Transparent;
            lblHalaman.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblHalaman.ForeColor = Color.FromArgb(23, 59, 99);
            lblHalaman.Location = new Point(166, 862);
            lblHalaman.Name = "lblHalaman";
            lblHalaman.Size = new Size(130, 25);
            lblHalaman.TabIndex = 6;
            lblHalaman.Text = "Halaman 1 dari 1";
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.Transparent;
            btnNext.BorderColor = Color.FromArgb(23, 59, 99);
            btnNext.BorderRadius = 20;
            btnNext.BorderThickness = 2;
            btnNext.CustomizableEdges = customizableEdges5;
            btnNext.DisabledState.BorderColor = Color.DarkGray;
            btnNext.DisabledState.CustomBorderColor = Color.DarkGray;
            btnNext.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnNext.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnNext.FillColor = Color.FromArgb(23, 59, 99);
            btnNext.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNext.ForeColor = Color.White;
            btnNext.ImageAlign = HorizontalAlignment.Right;
            btnNext.ImageSize = new Size(22, 22);
            btnNext.Location = new Point(315, 855);
            btnNext.Name = "btnNext";
            btnNext.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnNext.Size = new Size(133, 44);
            btnNext.TabIndex = 7;
            btnNext.Text = "Next";
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.BackColor = Color.Transparent;
            btnPrev.BorderColor = Color.FromArgb(23, 59, 99);
            btnPrev.BorderRadius = 20;
            btnPrev.BorderThickness = 2;
            btnPrev.CustomizableEdges = customizableEdges7;
            btnPrev.DisabledState.BorderColor = Color.DarkGray;
            btnPrev.DisabledState.CustomBorderColor = Color.DarkGray;
            btnPrev.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnPrev.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnPrev.FillColor = Color.FromArgb(23, 59, 99);
            btnPrev.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrev.ForeColor = Color.White;
            btnPrev.ImageAlign = HorizontalAlignment.Left;
            btnPrev.ImageSize = new Size(22, 22);
            btnPrev.Location = new Point(20, 855);
            btnPrev.Name = "btnPrev";
            btnPrev.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnPrev.Size = new Size(133, 44);
            btnPrev.TabIndex = 8;
            btnPrev.Text = "Prev";
            btnPrev.Click += btnPrev_Click;
            // 
            // KategoriList
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(btnPrev);
            Controls.Add(btnNext);
            Controls.Add(lblHalaman);
            Controls.Add(lblTotal);
            Controls.Add(dgvKategori);
            Controls.Add(btnTambah);
            Controls.Add(txtSearch);
            Controls.Add(guna2HtmlLabel2);
            Controls.Add(guna2HtmlLabel1);
            Name = "KategoriList";
            Size = new Size(1555, 914);
            Load += KategoriList_Load;
            ((System.ComponentModel.ISupportInitialize)dgvKategori).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2Button btnTambah;
        private Guna.UI2.WinForms.Guna2DataGridView dgvKategori;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotal;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblHalaman;
        private Guna.UI2.WinForms.Guna2Button btnNext;
        private Guna.UI2.WinForms.Guna2Button btnPrev;
    }
}
