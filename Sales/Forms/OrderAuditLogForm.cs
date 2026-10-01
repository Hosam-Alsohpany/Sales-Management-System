// ============================================================
// الملف    : OrderAuditLogForm.cs
// الغرض    : شاشة تدقيق تتبع عمليات التعديل والحذف لفواتير المبيعات
// ============================================================

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sales.Models;
using Sales.Repositories;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class OrderAuditLogForm : Form
    {
        private readonly OrderAuditLogRepository _repo = new OrderAuditLogRepository();
        private BindingSource _binding = new BindingSource();

        public OrderAuditLogForm()
        {
            InitializeComponent();
            try { UiTheme.ApplyToForm(this); } catch { }
            cmbAction.Items.Add("");
            foreach (var a in OrderAuditLogRepository.GetKnownActions())
                cmbAction.Items.Add(a);
            if (cmbAction.Items.Count > 0) cmbAction.SelectedIndex = 0;
            dtpFrom.Value = DateTime.Today.AddDays(-7);
            dtpTo.Value = DateTime.Today;
            dgv.DataSource = _binding;
            ConfigureGrid();
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.ExportToPdf(dgv, "سجل تدقيق الفواتير", "order_audit_log.pdf", txtOrderId?.Text);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.PrintGrid(dgv, "سجل تدقيق الفواتير", txtOrderId?.Text);
        }

        private void OrderAuditLogForm_Load(object sender, EventArgs e)
        {
            if (!AdminFormAccess.EnsureAdminOnLoad(this, "سجل تدقيق الفواتير"))
                return;
            LoadData();
        }

        private void ConfigureGrid()
        {
            dgv.ReadOnly = true;
            dgv.AutoGenerateColumns = true;
            dgv.RightToLeft = RightToLeft.Yes;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
        }

        private void LoadData()
        {
            int? orderId = null;
            if (int.TryParse(txtOrderId.Text.Trim(), out int oid))
                orderId = oid;

            var filter = new OrderAuditLogFilter
            {
                FromDate = dtpFrom.Value.Date,
                ToDate = dtpTo.Value.Date,
                UserName = txtUser.Text,
                Action = cmbAction.SelectedItem?.ToString(),
                OrderId = orderId
            };

            _binding.DataSource = new BindingList<OrderAuditLogEntry>(_repo.Search(filter));
            ApplyHeaders();
        }

        private void ApplyHeaders()
        {
            try
            {
                if (dgv.Columns.Contains("Id")) dgv.Columns["Id"].HeaderText = "المعرف";
                if (dgv.Columns.Contains("OrderId")) dgv.Columns["OrderId"].HeaderText = "رقم الفاتورة";
                if (dgv.Columns.Contains("Action")) dgv.Columns["Action"].HeaderText = "العملية";
                if (dgv.Columns.Contains("UserName")) dgv.Columns["UserName"].HeaderText = "المستخدم";
                if (dgv.Columns.Contains("ActionDate")) dgv.Columns["ActionDate"].HeaderText = "التاريخ";
                if (dgv.Columns.Contains("Details")) dgv.Columns["Details"].HeaderText = "التفاصيل";
            }
            catch { }
        }

        private void btnSearch_Click(object sender, EventArgs e) => LoadData();

        private void btnExport_Click(object sender, EventArgs e)
        {
            try { DataGridViewExportHelper.ExportToCsv(dgv, "order_audit_log.csv"); }
            catch (Exception ex) { MessageHelper.ShowError("تصدير", ex.Message); }
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (_binding.Current is OrderAuditLogEntry entry)
                txtDetails.Text = entry.Details ?? string.Empty;
            else
                txtDetails.Text = string.Empty;
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
