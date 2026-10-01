// ============================================================
// الملف    : Manager_Customers.cs
// الغرض    : شاشة إدارة العملاء (عرض/بحث/إضافة/تعديل/حذف) مع حفظ التغييرات دفعة واحدة
// يتعامل مع: Repositories/CustomerRepository.cs (CRUD + حساب Balance) + Models/Customer.cs
// الجداول  : Customers, Orders, Payments (الرصيد محسوب عبر CustomerRepository.GetAllCustomers)
// ملاحظة   : عمود Balance للعرض فقط لأنه محسوب (إجمالي الطلبات - إجمالي المدفوعات)
// ============================================================

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
    /// نموذج إدارة العملاء - الواجهة الرئيسية لإدارة بيانات العملاء
    /// 
    /// الوظائف الرئيسية:
    /// - عرض قائمة العملاء مع البحث والتصفية
    /// - إضافة/تعديل/حذف العملاء
    /// - حفظ التغييرات دفعة واحدة
    /// 
    /// الصلاحيات:
    /// - Admin: جميع العمليات
    /// - User: عرض فقط (بحسب إعدادات النظام)
    /// </summary>
    public partial class Manager_Customers : Form
    {
        /// <summary>
        /// اسم المستخدم الحالي الذي فتح الفورم
        /// </summary>
        private string _UserName;
        
        /// <summary>
        /// المستودع المسؤول عن عمليات قاعدة البيانات للعملاء
        /// </summary>
        private CustomerRepository _customerRepo = new CustomerRepository();
        
        /// <summary>
        /// قائمة العملاء المربوطة بالجدول لدعم التعديل المباشر
        /// BindingList تدعم الإشعارات التلقائية عند التغيير
        /// </summary>
        private BindingList<Customer> _customers;

        /// <summary>
        /// منشئ الفورم - يقوم بتهيئة المكونات وتعيين المستخدم الحالي
        /// </summary>
        /// <param name="UserName">اسم المستخدم الذي فتح الفورم</param>
        public Manager_Customers(string UserName)
        {
            InitializeComponent();
            _UserName = UserName;

            UiTheme.ApplyToForm(this);

            try
            {
                KeyPreview = true;
                KeyDown -= Manager_Customers_KeyDown;
                KeyDown += Manager_Customers_KeyDown;
            }
            catch
            {
            }

            DataGridViewDateTimeFormatter.Apply(dataGridView_Customer);

            if (dataGridView_Customer != null)
                dataGridView_Customer.DataError += dataGridView_Customer_DataError;

        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.ExportToPdf(dataGridView_Customer, "العملاء", "customers.pdf", txtSearch?.Text);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.PrintGrid(dataGridView_Customer, "العملاء", txtSearch?.Text);
        }

        private void Manager_Customers_KeyDown(object sender, KeyEventArgs e)
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
        private void Manager_Customers_Load(object sender, EventArgs e)
        {
            try
            {
                loadData_Customers();
                txtUserName.Text = _UserName;
                
                // تحسينات وتنسيق الجدول
                FormatDataGridColumns();

                if (SessionManager.IsReadOnlyForCurrentUser)
                {
                    dataGridView_Customer.ReadOnly = true;
                    dataGridView_Customer.AllowUserToAddRows = false;
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("خطأ أثناء تحميل الفورم", ex.Message);
                Logger.LogError("Manager_Customers_Load failed", ex);
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
                dataGridView_Customer.AutoGenerateColumns = false;
                dataGridView_Customer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView_Customer.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView_Customer.MultiSelect = false;
                dataGridView_Customer.ReadOnly = false;
                dataGridView_Customer.AllowUserToAddRows = true;
                dataGridView_Customer.AllowUserToDeleteRows = false;
                dataGridView_Customer.RowHeadersVisible = false;
                dataGridView_Customer.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

                // إخفاء عمود المعرف لأنه غير مفيد للمستخدم النهائي
                if (dataGridView_Customer.Columns["ColId"] != null)
                    dataGridView_Customer.Columns["ColId"].Visible = false;

                // تنسيق عمود حساب العميل (الرصيد) - محسوب من قاعدة البيانات
                if (dataGridView_Customer.Columns["ColBalance"] != null)
                {
                    // ملاحظة: هذا العمود مربوط بـ Balance (حساب العميل) وهو محسوب من قاعدة البيانات
                    // لذلك نجعله للعرض فقط حتى لا يحدث خطأ تحويل أثناء الكتابة
                    dataGridView_Customer.Columns["ColBalance"].ReadOnly = true;
                    dataGridView_Customer.Columns["ColBalance"].DefaultCellStyle.Format = "N0";
                    dataGridView_Customer.Columns["ColBalance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // تنسيق عمود الهاتف
                if (dataGridView_Customer.Columns["ColTel"] != null)
                {
                    dataGridView_Customer.Columns["ColTel"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // تنسيق عمود الاسم
                if (dataGridView_Customer.Columns["ColName"] != null)
                {
                    dataGridView_Customer.Columns["ColName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error formatting DataGrid columns", ex);
            }
        }

        private void dataGridView_Customer_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                e.ThrowException = false;

                // نتجنب إزعاج المستخدم برسائل متكررة أثناء الكتابة
                // لكن لو احتجنا يمكن تفعيل رسالة عامة
                // MessageBox.Show("قيمة غير صحيحة في الحقل.");
            }
            catch
            {
                // تجاهل
            }
        }

        /// <summary>
        /// تحميل بيانات العملاء من قاعدة البيانات وعرضها في الجدول
        /// يتم استدعاؤها عند فتح الفورم وعند تحديث البيانات
        /// </summary>
        private void loadData_Customers()
        {
            try
            {
                // جلب البيانات من المستودع (Repository Pattern)
                var list = _customerRepo.GetAllCustomers();
                
                // تحويلها إلى BindingList لربطها بالجدول ودعم التعديل المباشر
                _customers = new BindingList<Customer>(list);
                dataGridView_Customer.DataSource = _customers;

                // التأكد من إخفاء عمود المعرف
                if (dataGridView_Customer.Columns["ColId"] != null)
                    dataGridView_Customer.Columns["ColId"].Visible = false;
            }
            catch (System.Data.SQLite.SQLiteException sqlEx)
            {
                MessageHelper.ShowError("خطأ في قاعدة البيانات أثناء تحميل العملاء", sqlEx.Message);
                Logger.LogError("Database error in loadData_Customers", sqlEx);
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("حدث خطأ غير متوقع أثناء تحميل البيانات", ex.Message);
                Logger.LogError("Unexpected error in loadData_Customers", ex);
            }
        }

        /// <summary>
        /// حفظ جميع التغييرات على العملاء في قاعدة البيانات
        /// يقوم بمعالجة الإضافات والتعديلات دفعة واحدة
        /// </summary>
        private void btnSaveCustomer_Click(object sender, EventArgs e)
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
                    dataGridView_Customer.EndEdit();
                    dataGridView_Customer.CommitEdit(DataGridViewDataErrorContexts.Commit);

                    var cm = BindingContext[dataGridView_Customer.DataSource] as CurrencyManager;
                    if (cm != null)
                        cm.EndCurrentEdit();
                }
                catch
                {
                    // تجاهل
                }

                // التحقق من وجود بيانات للحفظ
                if (_customers == null || _customers.Count == 0)
                {
                    MessageHelper.ShowWarning("لا توجد بيانات لحفظها");
                    return;
                }

                int addedCount = 0, updatedCount = 0;
                
                // المرور على جميع العملاء في القائمة
                foreach (var customer in _customers)
                {
                    // التحقق من صحة البيانات الأساسية
                    if (string.IsNullOrWhiteSpace(customer.Name)) 
                        continue;

                    if (customer.Id == 0)
                    {
                        // جديد -> إضافة
                        _customerRepo.AddCustomer(customer);
                        addedCount++;
                    }
                    else
                    {
                        // موجود -> تعديل
                        _customerRepo.UpdateCustomer(customer);
                        updatedCount++;
                    }
                }

                // عرض رسالة نجاح تفصيلية
                string message = $"تم حفظ التغييرات بنجاح:\n• عملاء جدد: {addedCount}\n• عملاء معدلين: {updatedCount}";
                MessageHelper.ShowSuccess(message);
                
                // إعادة تحميل البيانات لضمان المزامنة (خاصة لتحديث الـ IDs للعناصر الجديدة)
                loadData_Customers();
            }
            catch (System.Data.SQLite.SQLiteException sqlEx)
            {
                MessageHelper.ShowError("خطأ في قاعدة البيانات أثناء الحفظ", sqlEx.Message);
                Logger.LogError("Database error in btnSaveCustomer_Click", sqlEx);
            }
            catch (InvalidOperationException opEx)
            {
                // أخطاء منطقية (مثل بيانات مكررة)
                MessageHelper.ShowWarning(opEx.Message);
                Logger.LogError("Validation error in btnSaveCustomer_Click", opEx);
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("حدث خطأ غير متوقع أثناء حفظ المعاملات", ex.Message);
                Logger.LogError("Unexpected error in btnSaveCustomer_Click", ex);
            }
        }

        /// <summary>
        /// حذف العميل المحدد من قاعدة البيانات ومن الجدول
        /// </summary>
        private void btnDeleteCustomer_Click(object sender, EventArgs e)
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
                    MessageHelper.ShowUnauthorized("حذف العملاء");
                    return;
                }

                // التحقق من وجود صف محدد
                if (dataGridView_Customer.CurrentRow == null)
                {
                    MessageHelper.ShowWarning("يرجى تحديد عميل أولاً");
                    return;
                }

                // الحصول على الكائن المربوط بالصف
                var customer = dataGridView_Customer.CurrentRow.DataBoundItem as Customer;
                if (customer == null) return;

                // التأكيد قبل الحذف
                if (!MessageHelper.AskForConfirmation($"هل أنت متأكد من حذف العميل: {customer.Name}؟"))
                    return;

                // التحقق من وجود فواتير مرتبطة بالعميل (إذا كان هناك علاقة)
                // هذا يمكن إضافته لاحقاً عند وجود فواتير مرتبطة بالعملاء

                // الحذف من قاعدة البيانات
                if (customer.Id != 0)
                {
                    _customerRepo.DeleteCustomer(customer.Id);
                }

                // الحذف من القائمة في الشاشة
                _customers.Remove(customer);

                MessageHelper.ShowSuccess("تم حذف العميل بنجاح");
            }
            catch (System.Data.SQLite.SQLiteException sqlEx)
            {
                // قد يحدث هذا إذا كان العميل مرتبطاً بسجلات أخرى
                MessageHelper.ShowError("لا يمكن حذف العميل", sqlEx.Message);
                Logger.LogError("Database error in btnDeleteCustomer_Click", sqlEx);
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("حدث خطأ غير متوقع أثناء الحذف", ex.Message);
                Logger.LogError("Unexpected error in btnDeleteCustomer_Click", ex);
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
            // إعادة ربط المصدر الأساسي إن كان قد تغير لأي سبب
            if (_customers != null)
                dataGridView_Customer.DataSource = _customers;

            ApplySearchFilter();
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewExportHelper.ExportToCsv(dataGridView_Customer, "customers.csv");
            }
            catch
            {
            }
        }

        /// <summary>
        /// تطبيق فلتر البحث على البيانات المعروضة في الجدول
        /// يقوم بالبحث في اسم العميل ورقم الهاتف
        /// </summary>
        private void ApplySearchFilter()
        {
            try
            {
                // التحقق من صحة البيانات الأساسية
                if (_customers == null) return;
                if (dataGridView_Customer == null) return;
                if (dataGridView_Customer.Rows == null) return;

                // قد يحدث TextChanged قبل اكتمال الربط
                if (dataGridView_Customer.DataSource == null) return;

                string searchText = (txtSearch.Text ?? string.Empty).Trim();

                // نُبقي الـDataSource ثابت (BindingList) ونفلتر عبر إخفاء الصفوف
                foreach (DataGridViewRow row in dataGridView_Customer.Rows)
                {
                    if (row == null) continue;
                    if (row.IsNewRow) continue;

                    var item = row.DataBoundItem as Customer;
                    if (item == null)
                    {
                        row.Visible = true;
                        continue;
                    }

                    // إذا كان نص البحث فارغاً، أظهر كل الصفوف
                    if (string.IsNullOrEmpty(searchText))
                    {
                        row.Visible = true;
                        continue;
                    }

                    // البحث في الاسم والهاتف والبريد الإلكتروني
                    string name = item.Name ?? string.Empty;
                    string tel = item.Tel ?? string.Empty;

                    bool match =
                        name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        tel.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;

                    row.Visible = match;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in ApplySearchFilter", ex);
                // في حالة الخطأ، أظهر كل البيانات
                foreach (DataGridViewRow row in dataGridView_Customer.Rows)
                {
                    if (row != null) row.Visible = true;
                }
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  الدور          : إدارة العملاء (UI)          ║
        // ║  يعتمد على       : CustomerRepository         ║
        // ║  أهم التدفقات   : تحميل/بحث/حفظ/حذف           ║
        // ║  الجداول        : Customers + (Orders/Payments لحساب Balance) ║
        // ╚══════════════════════════════════════════════╝
    }
}
