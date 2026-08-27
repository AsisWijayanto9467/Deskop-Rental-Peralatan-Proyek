namespace App_Rental_Proyek.UserControls.Admin.KategoriManagement
{
    partial class CreateKategori
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
            guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblNama = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtNama = new Guna.UI2.WinForms.Guna2TextBox();
            lblDeskripsi = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtDeskripsi = new Guna.UI2.WinForms.Guna2TextBox();
            lblStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            cbStatus = new Guna.UI2.WinForms.Guna2ComboBox();
            btnSimpan = new Guna.UI2.WinForms.Guna2Button();
            btnKembali = new Guna.UI2.WinForms.Guna2Button();
            guna2Panel2.SuspendLayout();
            SuspendLayout();
            // 
            // guna2Panel2
            // 
            guna2Panel2.BackColor = Color.FromArgb(23, 59, 99);
            guna2Panel2.Controls.Add(lblTitle);
            guna2Panel2.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Panel2.Location = new Point(0, -3);
            guna2Panel2.Name = "guna2Panel2";
            guna2Panel2.ShadowDecoration.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Panel2.Size = new Size(484, 70);
            guna2Panel2.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(18, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(191, 39);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Tambah Kategori";
            // 
            // lblNama
            // 
            lblNama.BackColor = Color.Transparent;
            lblNama.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblNama.Location = new Point(18, 90);
            lblNama.Name = "lblNama";
            lblNama.Size = new Size(129, 25);
            lblNama.TabIndex = 1;
            lblNama.Text = "Nama Kategori :";
            // 
            // txtNama
            // 
            txtNama.BackColor = SystemColors.Control;
            txtNama.BorderColor = Color.FromArgb(23, 59, 99);
            txtNama.BorderRadius = 10;
            txtNama.BorderThickness = 2;
            txtNama.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            txtNama.DefaultText = "";
            txtNama.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtNama.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtNama.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtNama.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtNama.FillColor = SystemColors.Control;
            txtNama.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtNama.Font = new Font("Segoe UI", 10F);
            txtNama.ForeColor = Color.FromArgb(23, 59, 99);
            txtNama.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtNama.Location = new Point(18, 122);
            txtNama.Margin = new Padding(3, 4, 3, 4);
            txtNama.Name = "txtNama";
            txtNama.PlaceholderForeColor = Color.Gray;
            txtNama.PlaceholderText = "Contoh: Mesin, Alat Beton, Alat Pemotong...";
            txtNama.SelectedText = "";
            txtNama.ShadowDecoration.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            txtNama.Size = new Size(447, 38);
            txtNama.TabIndex = 2;
            // 
            // lblDeskripsi
            // 
            lblDeskripsi.BackColor = Color.Transparent;
            lblDeskripsi.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDeskripsi.Location = new Point(18, 178);
            lblDeskripsi.Name = "lblDeskripsi";
            lblDeskripsi.Size = new Size(87, 25);
            lblDeskripsi.TabIndex = 3;
            lblDeskripsi.Text = "Deskripsi :";
            // 
            // txtDeskripsi
            // 
            txtDeskripsi.BackColor = SystemColors.Control;
            txtDeskripsi.BorderColor = Color.FromArgb(23, 59, 99);
            txtDeskripsi.BorderRadius = 10;
            txtDeskripsi.BorderThickness = 2;
            txtDeskripsi.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            txtDeskripsi.DefaultText = "";
            txtDeskripsi.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtDeskripsi.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtDeskripsi.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtDeskripsi.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtDeskripsi.FillColor = SystemColors.Control;
            txtDeskripsi.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtDeskripsi.Font = new Font("Segoe UI", 10F);
            txtDeskripsi.ForeColor = Color.FromArgb(23, 59, 99);
            txtDeskripsi.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtDeskripsi.Location = new Point(18, 210);
            txtDeskripsi.Margin = new Padding(3, 4, 3, 4);
            txtDeskripsi.Multiline = true;
            txtDeskripsi.Name = "txtDeskripsi";
            txtDeskripsi.PlaceholderForeColor = Color.Gray;
            txtDeskripsi.PlaceholderText = "Deskripsi singkat kategori (opsional)...";
            txtDeskripsi.SelectedText = "";
            txtDeskripsi.ShadowDecoration.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            txtDeskripsi.Size = new Size(447, 120);
            txtDeskripsi.TabIndex = 4;
            // 
            // lblStatus
            // 
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblStatus.Location = new Point(18, 348);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(59, 25);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Status :";
            // 
            // cbStatus
            // 
            cbStatus.BackColor = Color.Transparent;
            cbStatus.BorderColor = Color.FromArgb(23, 59, 99);
            cbStatus.BorderRadius = 17;
            cbStatus.BorderThickness = 2;
            cbStatus.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            cbStatus.DrawMode = DrawMode.OwnerDrawFixed;
            cbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cbStatus.FocusedColor = Color.FromArgb(94, 148, 255);
            cbStatus.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cbStatus.Font = new Font("Segoe UI", 10F);
            cbStatus.ForeColor = Color.FromArgb(68, 88, 112);
            cbStatus.ItemHeight = 30;
            cbStatus.Location = new Point(18, 380);
            cbStatus.Name = "cbStatus";
            cbStatus.ShadowDecoration.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            cbStatus.Size = new Size(447, 36);
            cbStatus.TabIndex = 6;
            // 
            // btnSimpan
            // 
            btnSimpan.BackColor = Color.Transparent;
            btnSimpan.BorderColor = Color.FromArgb(23, 59, 99);
            btnSimpan.BorderRadius = 17;
            btnSimpan.BorderThickness = 2;
            btnSimpan.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            btnSimpan.DisabledState.BorderColor = Color.DarkGray;
            btnSimpan.DisabledState.CustomBorderColor = Color.DarkGray;
            btnSimpan.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnSimpan.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnSimpan.FillColor = Color.FromArgb(23, 59, 99);
            btnSimpan.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSimpan.ForeColor = Color.White;
            btnSimpan.ImageSize = new Size(22, 22);
            btnSimpan.Location = new Point(18, 445);
            btnSimpan.Name = "btnSimpan";
            btnSimpan.ShadowDecoration.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            btnSimpan.Size = new Size(144, 38);
            btnSimpan.TabIndex = 7;
            btnSimpan.Text = "Simpan";
            btnSimpan.Click += btnSimpan_Click;
            // 
            // btnKembali
            // 
            btnKembali.BackColor = Color.Transparent;
            btnKembali.BorderColor = Color.FromArgb(23, 59, 99);
            btnKembali.BorderRadius = 17;
            btnKembali.BorderThickness = 2;
            btnKembali.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            btnKembali.DisabledState.BorderColor = Color.DarkGray;
            btnKembali.DisabledState.CustomBorderColor = Color.DarkGray;
            btnKembali.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnKembali.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnKembali.FillColor = Color.White;
            btnKembali.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnKembali.ForeColor = Color.FromArgb(23, 59, 99);
            btnKembali.ImageSize = new Size(22, 22);
            btnKembali.Location = new Point(180, 445);
            btnKembali.Name = "btnKembali";
            btnKembali.ShadowDecoration.CustomizableEdges = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            btnKembali.Size = new Size(144, 38);
            btnKembali.TabIndex = 8;
            btnKembali.Text = "Kembali";
            btnKembali.Click += btnKembali_Click;
            // 
            // CreateKategori
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 510);
            Controls.Add(guna2Panel2);
            Controls.Add(lblNama);
            Controls.Add(txtNama);
            Controls.Add(lblDeskripsi);
            Controls.Add(txtDeskripsi);
            Controls.Add(lblStatus);
            Controls.Add(cbStatus);
            Controls.Add(btnSimpan);
            Controls.Add(btnKembali);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateKategori";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Tambah Kategori";
            Load += CreateKategori_Load;
            guna2Panel2.ResumeLayout(false);
            guna2Panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNama;
        private Guna.UI2.WinForms.Guna2TextBox txtNama;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDeskripsi;
        private Guna.UI2.WinForms.Guna2TextBox txtDeskripsi;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblStatus;
        private Guna.UI2.WinForms.Guna2ComboBox cbStatus;
        private Guna.UI2.WinForms.Guna2Button btnSimpan;
        private Guna.UI2.WinForms.Guna2Button btnKembali;
    }
}
