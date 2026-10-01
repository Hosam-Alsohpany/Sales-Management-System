using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sales.Repositories;
using Sales.Utilities;

namespace Sales.Forms
{
    /// <summary>
    /// نموذج سجل المخزون - يعرض حركات المخزون لجميع المنتجات
    /// 
    /// الوظائف الرئيسية:
    /// - عرض جميع حركات المخزون
    /// - عرض تفاصيل الحركة (المنتج، الكمية، السبب، التاريخ، المستخدم)
    /// - تصفية البيانات حسب المنتج أو التاريخ
    /// </summary>
    public partial class Stock_History : Form
    {
        /// <summary>
        /// المستودع المسؤول عن عمليات سجل المخزون
        /// </summary>
        private StockHistoryRepository _repo = new StockHistoryRepository();

        private StockBatchRepository _batchRepo = new StockBatchRepository();

        /// <summary>
        /// منشئ الفورم - يقوم بتهيئة المكونات وتحميل البيانات
        /// </summary>
        public Stock_History()
        {
            InitializeComponent();
            try
            {
                if (tabBatches != null)
                    tabBatches.Parent = null;
            }
            catch { }
            UiTheme.ApplyToForm(this);
            DataGridViewDateTimeFormatter.Apply(dgvHistory);
            ConfigureGrid();
            ConfigureBatchesGrids();
            dgvBatches.SelectionChanged += dgvBatches_SelectionChanged;

            _ = LoadAllAsync();
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            var g = GetActiveExportGrid(out string t, out string f);
            DataGridViewExportHelper.ExportToPdf(g, t, f + ".pdf");
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            var g = GetActiveExportGrid(out string t, out _);
            DataGridViewExportHelper.PrintGrid(g, t);
        }

        private void ApplyBatchesGridArabicHeaders()
        {
            try
            {
                if (dgvBatches != null && dgvBatches.Columns != null)
                {
                    try
                    {
                        dgvBatches.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvBatches.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                    catch
                    {
                    }

                    if (dgvBatches.Columns.Contains("colBatchId")) dgvBatches.Columns["colBatchId"].HeaderText = "رقم العملية";
                    if (dgvBatches.Columns.Contains("created_at")) dgvBatches.Columns["created_at"].HeaderText = "التاريخ";
                    if (dgvBatches.Columns.Contains("created_by")) dgvBatches.Columns["created_by"].HeaderText = "المستخدم";
                    if (dgvBatches.Columns.Contains("reason")) dgvBatches.Columns["reason"].HeaderText = "السبب";
                    if (dgvBatches.Columns.Contains("reference")) dgvBatches.Columns["reference"].HeaderText = "المرجع";
                    if (dgvBatches.Columns.Contains("note")) dgvBatches.Columns["note"].HeaderText = "ملاحظات";
                    if (dgvBatches.Columns.Contains("items_count")) dgvBatches.Columns["items_count"].HeaderText = "عدد الأصناف";
                    if (dgvBatches.Columns.Contains("total_qty_change")) dgvBatches.Columns["total_qty_change"].HeaderText = "إجمالي التغير";

                    try
                    {
                        foreach (DataGridViewColumn c in dgvBatches.Columns)
                        {
                            if (c != null)
                                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                    catch
                    {
                    }
                }

                if (dgvBatchItems != null && dgvBatchItems.Columns != null)
                {
                    try
                    {
                        dgvBatchItems.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvBatchItems.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                    catch
                    {
                    }

                    if (dgvBatchItems.Columns.Contains("colBatchItemId")) dgvBatchItems.Columns["colBatchItemId"].HeaderText = "رقم السطر";
                    if (dgvBatchItems.Columns.Contains("batch_id")) dgvBatchItems.Columns["batch_id"].HeaderText = "رقم العملية";
                    if (dgvBatchItems.Columns.Contains("product_id")) dgvBatchItems.Columns["product_id"].HeaderText = "رقم المنتج";
                    if (dgvBatchItems.Columns.Contains("product_name")) dgvBatchItems.Columns["product_name"].HeaderText = "اسم المنتج";
                    if (dgvBatchItems.Columns.Contains("qty_before")) dgvBatchItems.Columns["qty_before"].HeaderText = "الكمية قبل";
                    if (dgvBatchItems.Columns.Contains("qty_after")) dgvBatchItems.Columns["qty_after"].HeaderText = "الكمية بعد";
                    if (dgvBatchItems.Columns.Contains("qty_change")) dgvBatchItems.Columns["qty_change"].HeaderText = "التغير";

                    try
                    {
                        foreach (DataGridViewColumn c in dgvBatchItems.Columns)
                        {
                            if (c != null)
                                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// حدث تحميل الفورم - يقوم بتحميل بيانات سجل المخزون
        /// </summary>
        private void Stock_History_Load(object sender, EventArgs e)
        {
            try
            {
                _ = LoadAllAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل بيانات سجل المخزون: {ex.Message}", 
                               "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// تهيئة أعمدة DataGridView بشكل احترافي
        /// </summary>
        private void ConfigureGrid()
        {
            try
            {
                if (dgvHistory == null) return;

                dgvHistory.AutoGenerateColumns = false;

                // تحسينات إضافية للجدول
                dgvHistory.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
                dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvHistory.MultiSelect = false;
                dgvHistory.RowHeadersVisible = false;

                try
                {
                    dgvHistory.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvHistory.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgvHistory.ColumnHeadersDefaultCellStyle.Font = new Font(dgvHistory.Font.FontFamily, 10F, FontStyle.Bold);
                    dgvHistory.DefaultCellStyle.Font = new Font(dgvHistory.Font.FontFamily, 10F, FontStyle.Regular);
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تهيئة الجدول: {ex.Message}", 
                               "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// تحميل بيانات سجل المخزون من قاعدة البيانات
        /// </summary>
        private void LoadData()
        {
            try
            {
                dgvHistory.DataSource = _repo.GetAllHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل البيانات: {ex.Message}", 
                               "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureBatchesGrids()
        {
            try
            {
                dgvBatches.AutoGenerateColumns = false;
                dgvBatches.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvBatches.MultiSelect = false;
                dgvBatches.RowHeadersVisible = false;

                try
                {
                    dgvBatches.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvBatches.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvBatches.ColumnHeadersDefaultCellStyle.Font = new Font(dgvBatches.Font.FontFamily, 10F, FontStyle.Bold);
                    dgvBatches.DefaultCellStyle.Font = new Font(dgvBatches.Font.FontFamily, 10F, FontStyle.Regular);
                }
                catch
                {
                }

                dgvBatchItems.AutoGenerateColumns = false;
                dgvBatchItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvBatchItems.MultiSelect = false;
                dgvBatchItems.RowHeadersVisible = false;

                try
                {
                    dgvBatchItems.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvBatchItems.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvBatchItems.ColumnHeadersDefaultCellStyle.Font = new Font(dgvBatchItems.Font.FontFamily, 10F, FontStyle.Bold);
                    dgvBatchItems.DefaultCellStyle.Font = new Font(dgvBatchItems.Font.FontFamily, 10F, FontStyle.Regular);
                }
                catch
                {
                }
            }
            catch
            {
                // تجاهل
            }
        }

        private async Task LoadAllAsync()
        {
            try
            {
                var history = await Task.Run(() => _repo.GetAllHistory());

                if (IsDisposed) return;

                BeginInvoke((Action)(() =>
                {
                    dgvHistory.DataSource = history;
                    if (dgvHistory.Columns.Contains("colQtyChange"))
                        dgvHistory.Columns["colQtyChange"].DataPropertyName = "QtyDisplay";
                }));
            }
            catch (Exception ex)
            {
                if (IsDisposed) return;
                BeginInvoke((Action)(() =>
                {
                    MessageBox.Show($"حدث خطأ أثناء تحميل البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private async void dgvBatches_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvBatches.CurrentRow == null)
                {
                    dgvBatchItems.DataSource = null;
                    return;
                }

                object idObj = dgvBatches.CurrentRow.Cells["colBatchId"].Value;
                if (idObj == null || idObj == DBNull.Value)
                {
                    dgvBatchItems.DataSource = null;
                    return;
                }

                long batchId;
                if (!long.TryParse(idObj.ToString(), out batchId))
                {
                    dgvBatchItems.DataSource = null;
                    return;
                }

                var dt = await Task.Run(() => _batchRepo.GetBatchItems(batchId));
                if (IsDisposed) return;
                BeginInvoke((Action)(() =>
                {
                    dgvBatchItems.DataSource = dt;
                    ApplyBatchesGridArabicHeaders();
                }));
            }
            catch
            {
                // لا نعرض MessageBox متكرر هنا لتجنب الإزعاج
            }
        }

        /// <summary>
        /// إغلاق الفورم الحالي
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private DataGridView GetActiveExportGrid(out string title, out string fileBase)
        {
            if (tabMain != null && tabMain.SelectedTab == tabHistory)
            {
                title = "سجل المخزون";
                fileBase = "stock_history";
                return dgvHistory;
            }
            if (tabMain != null && tabMain.SelectedTab == tabBatches)
            {
                title = "دفعات المخزون";
                fileBase = "stock_batches";
                return dgvBatches;
            }
            title = "تفاصيل دفعة المخزون";
            fileBase = "stock_batch_items";
            return dgvBatchItems;
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            try
            {
                var dgv = GetActiveExportGrid(out string title, out string fileBase);
                DataGridViewExportHelper.ExportToCsv(dgv, fileBase + ".csv");
            }
            catch
            {
            }
        }
    }
}
