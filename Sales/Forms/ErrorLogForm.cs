// ============================================================
// الملف    : ErrorLogForm.cs
// الغرض    : شاشة عرض وتصفح الأخطاء التقنية الموثقة في النظام
// ============================================================

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sales.Repositories;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class ErrorLogForm : Form
    {
        private readonly ErrorLogRepository _repo = new ErrorLogRepository();
        private BindingSource _binding = new BindingSource();

        public ErrorLogForm()
        {
            InitializeComponent();
            try { UiTheme.ApplyToForm(this); } catch { }
            dgv.DataSource = _binding;
            dgv.ReadOnly = true;
            dgv.AutoGenerateColumns = true;
            dgv.RightToLeft = RightToLeft.Yes;
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.ExportToPdf(dgv, "سجل الأخطاء", "error_log.pdf");
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.PrintGrid(dgv, "سجل الأخطاء");
        }

        private void ErrorLogForm_Load(object sender, EventArgs e)
        {
            if (!AdminFormAccess.EnsureAdminOnLoad(this, "سجل الأخطاء"))
                return;
            LoadData();
        }

        private void LoadData()
        {
            _binding.DataSource = new BindingList<Models.ErrorLogEntry>(_repo.GetRecent(500));
            try
            {
                if (dgv.Columns.Contains("Message")) dgv.Columns["Message"].Width = 280;
                if (dgv.Columns.Contains("Exception")) dgv.Columns["Exception"].Width = 320;
            }
            catch { }
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadData();

        private void btnExport_Click(object sender, EventArgs e)
        {
            try { DataGridViewExportHelper.ExportToCsv(dgv, "error_log.csv"); }
            catch (Exception ex) { MessageHelper.ShowError("تصدير", ex.Message); }
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
