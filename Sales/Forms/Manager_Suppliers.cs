using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sales.Models;
using Sales.Repositories;
using System.Linq;
using Sales.Utilities;

namespace Sales.Forms
{
    /// <summary>
    /// نموذج إدارة الموردين - الواجهة الرئيسية لإدارة بيانات الموردين
    /// 
    /// الوظائف الرئيسية:
    /// - عرض قائمة الموردين مع البحث والتصفية
    /// - إضافة/تعديل/حذف الموردين
    /// - حفظ التغييرات دفعة واحدة
    /// 
    /// الصلاحيات:
    /// - Admin: جميع العمليات
    /// - User: عرض فقط (بحسب إعدادات النظام)
    /// </summary>
    public partial class Manager_Suppliers : Form
    {
        /// <summary>
        /// اسم المستخدم الحالي الذي فتح الفورم
        /// </summary>
        private string _UserName;
        
        /// <summary>
        /// المستودع المسؤول عن عمليات قاعدة البيانات للموردين
        /// </summary>
        private SupplierRepository _supplierRepo = new SupplierRepository();
        
        /// <summary>
        /// قائمة الموردين المربوطة بالجدول لدعم التعديل المباشر
        /// BindingList تدعم الإشعارات التلقائية عند التغيير
        /// </summary>
        private BindingList<Supplier> _suppliers;

        /// <summary>
        /// منشئ الفورم - يقوم بتهيئة المكونات وتعيين المستخدم الحالي
        /// </summary>
        /// <param name="UserName">اسم المستخدم الذي فتح الفورم</param>
        public Manager_Suppliers(string UserName)
        {
            InitializeComponent();
            _UserName = UserName;

            UiTheme.ApplyToForm(this);

            try
            {
                KeyPreview = true;
                KeyDown -= Manager_Suppliers_KeyDown;
                KeyDown += Manager_Suppliers_KeyDown;
            }
            catch
            {
            }

            DataGridViewDateTimeFormatter.Apply(dataGridView_Suppliers);

            if (dataGridView_Suppliers != null)
                dataGridView_Suppliers.DataError += dataGridView_Suppliers_DataError;

        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.ExportToPdf(dataGridView_Suppliers, "الموردين", "suppliers.pdf", txtSearch?.Text);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.PrintGrid(dataGridView_Suppliers, "الموردين", txtSearch?.Text);
        }

        private void Manager_Suppliers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null) return;

            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    e.SuppressKeyPress = true;
                    if (txtSearch != null)
                    {
                        txtSearch.Focus();
                        txtSearch.SelectAll();
                    }
                    return;
                }

                if (e.KeyCode == Keys.Escape)
                {
                    e.SuppressKeyPress = true;
                    if (txtSearch != null)
                        txtSearch.Text = string.Empty;
                    return;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// حدث تحميل الفورم - يقوم بتحميل البيانات الأولية وتنسيق الجدول
        /// </summary>
        private void Manager_Suppliers_Load(object sender, EventArgs e)
        {
            try
            {
                loadData_Suppliers();
                txtUserName.Text = _UserName;
                
                // تحسينات وتنسيق الجدول
                FormatDataGridColumns();

                if (SessionManager.IsReadOnlyForCurrentUser)
                {
                    dataGridView_Suppliers.ReadOnly = true;
                    dataGridView_Suppliers.AllowUserToAddRows = false;
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("خطأ أثناء تحميل الفورم", ex.Message);
                Logger.LogError("Manager_Suppliers_Load failed", ex);
            }
        }

        private void dataGridView_Suppliers_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                e.ThrowException = false;
            }
            catch
            {
                // تجاهل
            }
        }

        /// <summary>
        /// تنسيق أعمدة DataGridView بشكل احترافي
        /// </summary>
        private void FormatDataGridColumns()
        {
            try
            {
                // إعدادات عامة للجدول
                dataGridView_Suppliers.AutoGenerateColumns = false;
                dataGridView_Suppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView_Suppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView_Suppliers.MultiSelect = false;
                dataGridView_Suppliers.ReadOnly = false;
                dataGridView_Suppliers.AllowUserToAddRows = true;
                dataGridView_Suppliers.AllowUserToDeleteRows = false;
                dataGridView_Suppliers.RowHeadersVisible = false;
                dataGridView_Suppliers.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

                // إخفاء عمود المعرف لأنه غير مفيد للمستخدم النهائي
                if (dataGridView_Suppliers.Columns["ColId"] != null)
                    dataGridView_Suppliers.Columns["ColId"].Visible = false;

                // تنسيق عمود اسم المورد
                if (dataGridView_Suppliers.Columns["ColName"] != null)
                {
                    dataGridView_Suppliers.Columns["ColName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                // تنسيق عمود الهاتف
                if (dataGridView_Suppliers.Columns["ColPhone"] != null)
                {
                    dataGridView_Suppliers.Columns["ColPhone"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // تنسيق عمود العنوان
                if (dataGridView_Suppliers.Columns["ColAddress"] != null)
                {
                    dataGridView_Suppliers.Columns["ColAddress"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                // تنسيق عمود التفاصيل
                if (dataGridView_Suppliers.Columns["ColDetails"] != null)
                {
                    dataGridView_Suppliers.Columns["ColDetails"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error formatting DataGrid columns", ex);
            }
        }

        /// <summary>
        /// تحميل بيانات الموردين من قاعدة البيانات وعرضها في الجدول
        /// يتم استدعاؤها عند فتح الفورم وعند تحديث البيانات
        /// </summary>
        private void loadData_Suppliers()
        {
            try
            {
                // جلب البيانات من المستودع (Repository Pattern)
                var list = _supplierRepo.GetAllSuppliers();
                
                // تحويلها إلى BindingList لربطها بالجدول ودعم التعديل المباشر
                _suppliers = new BindingList<Supplier>(list);
                dataGridView_Suppliers.DataSource = _suppliers;

                // التأكد من إخفاء عمود المعرف
                if (dataGridView_Suppliers.Columns["ColId"] != null)
                    dataGridView_Suppliers.Columns["ColId"].Visible = false;
            }
            catch (System.Data.SQLite.SQLiteException sqlEx)
            {
                MessageHelper.ShowError("خطأ في قاعدة البيانات أثناء تحميل الموردين", sqlEx.Message);
                Logger.LogError("Database error in loadData_Suppliers", sqlEx);
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("حدث خطأ غير متوقع أثناء تحميل البيانات", ex.Message);
                Logger.LogError("Unexpected error in loadData_Suppliers", ex);
            }
        }

        /// <summary>
        /// حفظ جميع التغييرات على الموردين في قاعدة البيانات
        /// يقوم بمعالجة الإضافات والتعديلات دفعة واحدة
        /// </summary>
        private void btnSaveSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                if (SessionManager.IsReadOnlyForCurrentUser)
                {
                    MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                    return;
                }

                // تأكيد تثبيت آخر تعديل داخل الجدول قبل الحفظ
                try
                {
                    dataGridView_Suppliers.EndEdit();
                    dataGridView_Suppliers.CommitEdit(DataGridViewDataErrorContexts.Commit);

                    var cm = BindingContext[dataGridView_Suppliers.DataSource] as CurrencyManager;
                    if (cm != null)
                        cm.EndCurrentEdit();
                }
                catch
                {
                    // تجاهل
                }

                // التحقق من وجود بيانات للحفظ
                var listToSave = dataGridView_Suppliers.DataSource as BindingList<Supplier>;
                if (listToSave == null || listToSave.Count == 0)
                {
                    MessageHelper.ShowWarning("لا توجد بيانات لحفظها");
                    return;
                }

                int addedCount = 0, updatedCount = 0;
                
                // المرور على جميع الموردين في القائمة
                foreach (var supplier in listToSave)
                {
                    // التحقق من صحة البيانات الأساسية
                    if (string.IsNullOrWhiteSpace(supplier.Name)) 
                        continue;

                    if (supplier.Id == 0)
                    {
                        // جديد -> إضافة
                        _supplierRepo.AddSupplier(supplier);
                        addedCount++;
                    }
                    else
                    {
                        // موجود -> تعديل
                        _supplierRepo.UpdateSupplier(supplier);
                        updatedCount++;
                    }
                }

                // عرض رسالة نجاح تفصيلية
                string message = $"تم حفظ التغييرات بنجاح:\n• موردين جدد: {addedCount}\n• موردين معدلين: {updatedCount}";
                MessageHelper.ShowSuccess(message);
                
                // إعادة تحميل البيانات لضمان المزامنة (خاصة لتحديث الـ IDs للعناصر الجديدة)
                loadData_Suppliers();
            }
            catch (System.Data.SQLite.SQLiteException sqlEx)
            {
                MessageHelper.ShowError("خطأ في قاعدة البيانات أثناء الحفظ", sqlEx.Message);
                Logger.LogError("Database error in btnSaveSupplier_Click", sqlEx);
            }
            catch (InvalidOperationException opEx)
            {
                // أخطاء منطقية (مثل بيانات مكررة)
                MessageHelper.ShowWarning(opEx.Message);
                Logger.LogError("Validation error in btnSaveSupplier_Click", opEx);
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("حدث خطأ غير متوقع أثناء حفظ المعاملات", ex.Message);
                Logger.LogError("Unexpected error in btnSaveSupplier_Click", ex);
            }
        }

        /// <summary>
        /// حذف المورد المحدد من قاعدة البيانات ومن الجدول
        /// </summary>
        private void btnDeleteSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                if (SessionManager.IsReadOnlyForCurrentUser)
                {
                    MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                    return;
                }

                if (!SessionManager.CanCurrentUserDelete)
                {
                    MessageHelper.ShowUnauthorized("حذف الموردين");
                    return;
                }

                // التحقق من وجود صف محدد
                if (dataGridView_Suppliers.CurrentRow == null)
                {
                    MessageHelper.ShowWarning("يرجى تحديد مورد أولاً");
                    return;
                }

                // الحصول على الكائن المربوط بالصف
                var supplier = dataGridView_Suppliers.CurrentRow.DataBoundItem as Supplier;
                if (supplier == null) return;

                // التأكيد قبل الحذف
                if (!MessageHelper.AskForConfirmation($"هل أنت متأكد من حذف المورد: {supplier.Name}؟"))
                    return;

                // التحقق من وجود عمليات مرتبطة بالمورد (إذا كان هناك علاقة)
                // هذا يمكن إضافته لاحقاً عند وجود عمليات مرتبطة بالموردين

                // الحذف من قاعدة البيانات
                if (supplier.Id != 0)
                {
                    _supplierRepo.DeleteSupplier(supplier.Id);
                }

                // الحذف من القائمة في الشاشة
                _suppliers.Remove(supplier);

                MessageHelper.ShowSuccess("تم حذف المورد بنجاح");
            }
            catch (System.Data.SQLite.SQLiteException sqlEx)
            {
                // قد يحدث هذا إذا كان المورد مرتبطاً بسجلات أخرى
                MessageHelper.ShowError("لا يمكن حذف المورد", sqlEx.Message);
                Logger.LogError("Database error in btnDeleteSupplier_Click", sqlEx);
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("حدث خطأ غير متوقع أثناء الحذف", ex.Message);
                Logger.LogError("Unexpected error in btnDeleteSupplier_Click", ex);
            }
        }

        /// <summary>
        /// إغلاق الفورم الحالي
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// حدث تغيير نص البحث - يقوم بتصفية البيانات بشكل فوري
        /// </summary>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplySearchFilter();
        }

        /// <summary>
        /// مسح نص البحث وإعادة عرض جميع البيانات
        /// </summary>
        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;

            ApplySearchFilter();
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewExportHelper.ExportToCsv(dataGridView_Suppliers, "suppliers.csv");
            }
            catch
            {
            }
        }

        /// <summary>
        /// تطبيق فلتر البحث على البيانات المعروضة في الجدول
        /// يقوم بالبحث في اسم المورد والهاتف والعنوان والتفاصيل
        /// </summary>
        private void ApplySearchFilter()
        {
            try
            {
                // التحقق من صحة البيانات الأساسية
                if (_suppliers == null) return;
                if (dataGridView_Suppliers == null) return;
                if (dataGridView_Suppliers.Rows == null) return;

                // قد يحدث TextChanged قبل اكتمال الربط
                if (dataGridView_Suppliers.DataSource == null) return;

                string searchText = (txtSearch.Text ?? string.Empty).Trim();

                foreach (DataGridViewRow row in dataGridView_Suppliers.Rows)
                {
                    if (row == null) continue;
                    if (row.IsNewRow) continue;

                    var item = row.DataBoundItem as Supplier;
                    if (item == null)
                    {
                        row.Visible = true;
                        continue;
                    }

                    if (string.IsNullOrEmpty(searchText))
                    {
                        row.Visible = true;
                        continue;
                    }

                    string name = item.Name ?? string.Empty;
                    string phone = item.Phone ?? string.Empty;
                    string address = item.Address ?? string.Empty;
                    string details = item.Details ?? string.Empty;

                    bool match =
                        name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        phone.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        address.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        details.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;

                    row.Visible = match;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in ApplySearchFilter", ex);

                // في حالة الخطأ، أظهر كل البيانات
                foreach (DataGridViewRow row in dataGridView_Suppliers.Rows)
                {
                    if (row != null) row.Visible = true;
                }
            }
        }
    }
}
