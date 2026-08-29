using System;
using System.Windows.Forms;

namespace App_Rental_Proyek.UserControls.Admin.ActivityLog
{
    public partial class ActivityLogDetail : Form
    {
        private readonly ActivityLogViewItem _log;

        public ActivityLogDetail(ActivityLogViewItem log)
        {
            InitializeComponent();
            _log = log;
            InitializeForm();
            LoadDetails();
        }

        private void InitializeForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void LoadDetails()
        {
            if (_log == null) return;

            lbId.Text = "#" + _log.Id.ToString();
            lbWaktu.Text = _log.CreatedAtDisplay;
            lbUser.Text = _log.UserNama;
            lbRole.Text = _log.UserRole;
            lbAktivitas.Text = _log.Aktivitas;
            lbModul.Text = _log.Modul;
            lbReferensi.Text = _log.ReferensiId;
            lbIp.Text = _log.IpAddress;
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
