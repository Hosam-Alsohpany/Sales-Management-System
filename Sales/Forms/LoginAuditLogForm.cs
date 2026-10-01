// ============================================================
// الملف    : LoginAuditLogForm.cs
// الغرض    : شاشة مراقبة وتتبع محاولات وسجلات دخول المستخدمين
// ============================================================

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sales.Models;
using Sales.Repositories;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class LoginAuditLogForm : Form
    {
        private readonly LoginAuditLogRepository _repo = new LoginAuditLogRepository();
        private BindingSource _binding = new BindingSource();

        public LoginAuditLogForm()
        {
            InitializeComponent();
            try { UiTheme.ApplyToForm(this); } catch { }
            dtpFrom.Value = DateTime.Today.AddDays(-7);
            dtpTo.Value = DateTime.Today;
            dgv.DataSource = _binding;
            dgv.ReadOnly = true;
            dgv.AutoGenerateColumns = true;
            dgv.RightToLeft = RightToLeft.Yes;
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.ExportToPdf(dgv, "سجل تسجيل الدخول", "login_audit.pdf", txtUser?.Text);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.PrintGrid(dgv, "سجل تسجيل الدخول", txtUser?.Text);
        }

        private void LoginAuditLogForm_Load(object sender, EventArgs e)
        {
            if (!AdminFormAccess.EnsureAdminOnLoad(this, "سجل تسجيل الدخول"))
                return;
            LoadData();
        }

        private void LoadData()
        {
            bool? successOnly = null;
            if (cmbSuccess.SelectedIndex == 1) successOnly = true;
            else if (cmbSuccess.SelectedIndex == 2) successOnly = false;

            _binding.DataSource = new BindingList<LoginAuditLogEntry>(_repo.Search(
                dtpFrom.Value.Date,
                dtpTo.Value.Date,
                txtUser.Text,
                successOnly));
            ApplyHeaders();
        }

        private void ApplyHeaders()
        {
            try
            {
                if (dgv.Columns.Contains("Id")) dgv.Columns["Id"].HeaderText = "المعرف";
                if (dgv.Columns.Contains("UserName")) dgv.Columns["UserName"].HeaderText = "المستخدم";
                if (dgv.Columns.Contains("LoginTime")) dgv.Columns["LoginTime"].HeaderText = "دخول";
                if (dgv.Columns.Contains("LogoutTime")) dgv.Columns["LogoutTime"].HeaderText = "خروج";
                if (dgv.Columns.Contains("IpAddress")) dgv.Columns["IpAddress"].HeaderText = "الجهاز";
                if (dgv.Columns.Contains("Success")) dgv.Columns["Success"].HeaderText = "نجاح";
                if (dgv.Columns.Contains("FailureReason")) dgv.Columns["FailureReason"].HeaderText = "سبب الفشل";
            }
            catch { }
        }

        private void btnSearch_Click(object sender, EventArgs e) => LoadData();

        private void btnExport_Click(object sender, EventArgs e)
        {
            try { DataGridViewExportHelper.ExportToCsv(dgv, "login_audit.csv"); }
            catch (Exception ex) { MessageHelper.ShowError("تصدير", ex.Message); }
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
