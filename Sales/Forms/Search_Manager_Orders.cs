using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using Sales.Database;
using Sales.Forms;
using Sales.Repositories;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class Search_Manager_Orders : Form
    {
        private string _userName;
        private  string _FullName;

        private readonly ToolTip _toolTip = new ToolTip();
        private readonly OrderRepository _orderRepo = new OrderRepository();

        
        public Search_Manager_Orders(string userName, string FullName)
        {
            InitializeComponent();
            _userName = userName;
            _FullName = FullName;

            UiTheme.ApplyToForm(this);
            try
            {
                KeyPreview = true;
                KeyDown -= Search_Manager_Orders_KeyDown;
                KeyDown += Search_Manager_Orders_KeyDown;
            }
            catch
            {
            }

            DataGridViewDateTimeFormatter.Apply(dataGridView_Search_Orders);

            try
            {
                if (dataGridView_Search_Orders != null)
                    dataGridView_Search_Orders.AutoGenerateColumns = false;
            }
            catch
            {
            }

            try
            {
                btnDeleteOrders.Click -= btnDeleteOrders_Click;
                btnDeleteOrders.Click += btnDeleteOrders_Click;
            }
            catch
            {
            }

            try
            {
                if (txtSearch != null)
                {
                    txtSearch.TextChanged -= txtSearch_TextChanged;
                    txtSearch.TextChanged += txtSearch_TextChanged;
                }
            }
            catch
            {
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplySearchFilter();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSearch != null)
                {
                    txtSearch.Text = string.Empty;
                    txtSearch.Focus();
                    txtSearch.SelectAll();
                }
            }
            catch
            {
            }

            ApplySearchFilter();
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            try { DataGridViewExportHelper.ExportToCsv(dataGridView_Search_Orders, "orders.csv"); }
            catch { }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            try { DataGridViewExportHelper.ExportToPdf(dataGridView_Search_Orders, "بحث الفواتير", "orders.pdf", txtSearch?.Text); }
            catch { }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try { DataGridViewExportHelper.PrintGrid(dataGridView_Search_Orders, "بحث الفواتير", txtSearch?.Text); }
            catch { }
        }

        private void ApplySearchFilter()
        {
            try
            {
                if (dataGridView_Search_Orders == null) return;
                if (dataGridView_Search_Orders.Rows == null) return;

                string searchText = (txtSearch != null ? txtSearch.Text : string.Empty) ?? string.Empty;
                searchText = searchText.Trim();

                foreach (DataGridViewRow row in dataGridView_Search_Orders.Rows)
                {
                    if (row == null) continue;
                    if (row.IsNewRow) continue;

                    if (string.IsNullOrEmpty(searchText))
                    {
                        row.Visible = true;
                        continue;
                    }

                    bool match = false;
                    try
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell == null) continue;
                            if (cell.Value == null) continue;

                            string cellText = Convert.ToString(cell.Value);
                            if (string.IsNullOrEmpty(cellText)) continue;

                            if (cellText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                match = true;
                                break;
                            }
                        }
                    }
                    catch
                    {
                        match = true;
                    }

                    row.Visible = match;
                }
            }
            catch
            {
                try
                {
                    foreach (DataGridViewRow row in dataGridView_Search_Orders.Rows)
                    {
                        if (row != null) row.Visible = true;
                    }
                }
                catch
                {
                }
            }
        }

        private void Search_Manager_Orders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null) return;

            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (txtSearch != null)
                    {
                        txtSearch.Focus();
                        txtSearch.SelectAll();
                    }
                    return;
                }

                if (e.KeyCode == Keys.F5)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    try { loadData_Orders_Search(); } catch { }
                    return;
                }

                if (e.KeyCode == Keys.Escape)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;

                    try
                    {
                        var q = (txtSearch != null ? txtSearch.Text : string.Empty) ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(q))
                        {
                            if (txtSearch != null)
                            {
                                txtSearch.Text = string.Empty;
                                txtSearch.Focus();
                            }
                            return;
                        }
                    }
                    catch { }

                    try { Close(); } catch { }
                    return;
                }
            }
            catch
            {
            }
        }

         
        private DataTable dt = new DataTable();

        // نفس فكرة Manager_Orders: نخزن الخصم داخل note بدون تعديل Schema
        private const string DiscountTagPrefix = "[خصم:";
        private const string DiscountTagSuffix = "]";


        private void loadData_Orders_Search()
        {
            try
            {
                dt.Clear();
                /// <summary>
                /// 📌 القاعدة الذهبية في SQL:
                ///إذا استخدمت دالة تجميع (COUNT, SUM, AVG …)
                ///فكل الأعمدة الأخرى في SELECT لازم تكون داخل GROUP BY
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
               /* string strQuery = @"SELECT o.id,o.order_date,o.created_by,o.note,
                            o.total,
                            c.name,
                           count(Order_Details.id_product) as ProductCount
                           FROM Orders o
                           inner join Order_Details ON o.id =  Order_Details.id_order
                            inner join Customers c  ON o.customer_id = c.id   
                             group by o.id,o.order_date,o.created_by,o.note,
                            o.total,
                            c.name";*/
                string strQuery = @"
                        SELECT 
                        o.id,
                        substr(o.order_date, 1, 19) AS order_date,

                        o.created_by,
                        o.note,
                        o.total,
                        IFNULL(o.discount, 0) AS discount,

                        -- لو اسم العميل NULL نعرض (نقدي / بيع مباشر)
                        COALESCE(c.name, 'نقدي / بيع مباشر') AS name,

                        COUNT(od.id_product) AS ProductCount

                        FROM Orders o

                        -- ربط تفاصيل الفاتورة (إجباري)
                        INNER JOIN Order_Details od 
                        ON o.id = od.id_order

                        -- ربط العملاء (اختياري)
                        LEFT JOIN Customers c  
                        ON o.customer_id = c.id

                        GROUP BY 
                        o.id,
                        o.order_date,
                        o.created_by,
                        o.note,
                        o.total,
                        o.discount,
                        c.name
                                ";
                // -- تنسيق التاريخ للعرض فقط--substr(o.order_date, 1, 19) AS order_date,





               
                using(SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString)){
                using(SQLiteDataAdapter adapter = new SQLiteDataAdapter(strQuery,con)){
                    dt.Clear();
                    adapter.Fill(dt);

                    // ── أعمدة محسوبة داخل الواجهة فقط (لا تتطلب تعديل قاعدة البيانات) ──
                    EnsureComputedColumns();
                    ApplyComputedValues();

                    // تحويل عمود order_date إلى DateTime
                 /*   foreach (DataRow row in dt.Rows)
                    {
                        row["order_date"] = DateTime.Parse(row["order_date"].ToString());
                    }*/

                  // الآن العمود order_date أصبح DateTime بالتالي تنسيق DefaultCellStyle.Format سيعمل

                    try { dataGridView_Search_Orders.AutoGenerateColumns = false; } catch { }
                    this.dataGridView_Search_Orders.DataSource = dt;

                }
                }
            }catch(Exception ex ){
                MessageBox.Show("حدث خطا اثناء ربط اكثر من جدول :\n"+ex.Message,
                   "خطاءاثناء الربط" ,MessageBoxButtons.OK,MessageBoxIcon.Error);
            
           }                                 
        }


        /* void SetDataSRC(){
       dt.Columns.Add("المعرف");
       dt.Columns.Add("تاريخ البيع");
       dt.Columns.Add("البائع");
       dt.Columns.Add("وصف الفاتورة");
       dt.Columns.Add("اجمالي السعر");
       dt.Columns.Add("اسم العميل");
       dt.Columns.Add("عدد المنتجات");
       dataGridView_Search_Orders.DataSource = dt;
       }*/
        //طريقه خاطئة 
        //الطريقه الصحيحه
        void FormatGrid()
        {// اختصار اسم الـ DataGridView في متغير
            // بدل ما نكتب الاسم الطويل كل مرة
            var dgv = dataGridView_Search_Orders;
            // تغيير عنوان العمود (Header) للعرض فقط
            // الاسم "OrderID" هو اسم العمود الحقيقي القادم من SQL
            if (dgv.Columns.Contains("id")) dgv.Columns["id"].HeaderText = "المعرف";
            // تغيير عنوان عمود تاريخ الطلب
            if (dgv.Columns.Contains("order_date")) dgv.Columns["order_date"].HeaderText = "تاريخ البيع";
            // تغيير عنوان عمود اسم المستخدم (البائع)
            if (dgv.Columns.Contains("created_by")) dgv.Columns["created_by"].HeaderText = "البائع";
            // تغيير عنوان عمود الملاحظات
            if (dgv.Columns.Contains("note")) dgv.Columns["note"].HeaderText = "وصف الفاتورة";
            // تغيير عنوان عمود إجمالي الفاتورة
            if (dgv.Columns.Contains("total")) dgv.Columns["total"].HeaderText = "اجمالي السعر";
            // تغيير عنوان عمود اسم العميل
            if (dgv.Columns.Contains("name")) dgv.Columns["name"].HeaderText = "اسم العميل";
            // تغيير عنوان عمود عدد المنتجات داخل الفاتورة
            if (dgv.Columns.Contains("ProductCount")) dgv.Columns["ProductCount"].HeaderText = "عدد المنتجات";

            // أعمدة محسوبة: الخصم + الصافي
            if (dgv.Columns.Contains("discount"))
                dgv.Columns["discount"].HeaderText = "الخصم";
            if (dgv.Columns.Contains("net_total"))
                dgv.Columns["net_total"].HeaderText = "الصافي";

            // تحديد عرض عمود المعرف ليكون ثابت (70 بكسل)
            if (dgv.Columns.Contains("id")) dgv.Columns["id"].Width = 70;
            if (dgv.Columns.Contains("order_date")) dgv.Columns["order_date"].Width = 300;
            // تنسيق عمود السعر:
            // N0 = رقم بدون كسور عشرية + فواصل آلاف
            if (dgv.Columns.Contains("total")) dgv.Columns["total"].DefaultCellStyle.Format = "N0";

            if (dgv.Columns.Contains("discount"))
                dgv.Columns["discount"].DefaultCellStyle.Format = "N0";
            if (dgv.Columns.Contains("net_total"))
                dgv.Columns["net_total"].DefaultCellStyle.Format = "N0";
            // تنسيق عرض التاريخ بشكل واضح (سنة/شهر/يوم)
           // dgv.Columns["order_date"].DefaultCellStyle.Format = "yyyy/MM/dd";
            //dgv.Columns["order_date"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            // جعل الجدول للعرض فقط (لا يمكن التعديل على الخلايا)
           // dgv.ReadOnly = true;
            // منع ظهور السطر الفارغ أسفل الجدول (سطر إضافة جديد)
            dgv.AllowUserToAddRows = false;
            // عند الضغط على أي خلية يتم تحديد الصف كامل
           // dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void Search_Manager_Orders_Load(object sender, EventArgs e)
        {
           
            loadData_Orders_Search();
            FormatGrid();
            txtUserName.Text = _userName;

            RefreshDeleteButtonState();
        }

        private void RefreshDeleteButtonState()
        {
            try
            {
                bool readOnly = AppSettingsManager.GetBool(AppSettingsManager.Keys.ReadOnlyForNormalUser, false);
                bool allowDelete = AppSettingsManager.GetBool(AppSettingsManager.Keys.AllowNormalUserDeleteOrders, false);

                bool canDelete = SessionManager.IsAdmin || (!readOnly && allowDelete);

                btnDeleteOrders.Enabled = canDelete;
                btnDeleteOrders.Cursor = canDelete ? Cursors.Hand : Cursors.No;

                string tip = SessionManager.IsAdmin
                    ? "حذف الفاتورة (يتطلب تأكيد)"
                    : (readOnly ? "الحساب الحالي بوضع قراءة فقط" : "الحذف متاح للمدير فقط");

                _toolTip.SetToolTip(btnDeleteOrders, tip);
            }
            catch
            {
            }
        }

        private bool EnsureCanDeleteOrdersAndShowMessage()
        {
            if (SessionManager.IsAdmin) return true;

            if (AppSettingsManager.GetBool(AppSettingsManager.Keys.ReadOnlyForNormalUser, false))
            {
                MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                return false;
            }

            if (!AppSettingsManager.GetBool(AppSettingsManager.Keys.AllowNormalUserDeleteOrders, false))
            {
                MessageHelper.ShowNoPermissionDeleteOrders();
                return false;
            }

            return true;
        }

        private void btnDeleteOrders_Click(object sender, EventArgs e)
        {
            try
            {
                if (!EnsureCanDeleteOrdersAndShowMessage()) return;

                if (dataGridView_Search_Orders.CurrentRow == null)
                {
                    MessageHelper.ShowWarning("حدد فاتورة أولاً");
                    return;
                }

                int orderId = Convert.ToInt32(dataGridView_Search_Orders.CurrentRow.Cells["id"].Value);

                if (!MessageHelper.AskForConfirmation("هل أنت متأكد من حذف الفاتورة رقم " + orderId + " ؟\nسيتم حذف التفاصيل والمدفوعات المرتبطة وإرجاع المخزون."))
                    return;

                string reason = PromptForReason("سبب الحذف");
                if (reason == null) return;
                if (string.IsNullOrWhiteSpace(reason))
                {
                    MessageHelper.ShowWarning("سبب الحذف مطلوب");
                    return;
                }

                _orderRepo.DeleteOrder(orderId, _userName, reason);
                MessageHelper.ShowSuccess("تم حذف الفاتورة بنجاح");

                loadData_Orders_Search();
                RefreshDeleteButtonState();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("تعذر حذف الفاتورة", ex.Message);
            }
        }

        private string PromptForReason(string title)
        {
            using (var frm = new ReasonPromptForm(title))
            {
                var result = frm.ShowDialog(this);
                if (result != DialogResult.OK) return null;
                return frm.ReasonText;
            }
        }

        private void EnsureComputedColumns()
        {
            if (!dt.Columns.Contains("discount"))
                dt.Columns.Add("discount", typeof(decimal));

            if (!dt.Columns.Contains("net_total"))
                dt.Columns.Add("net_total", typeof(decimal));
        }

        private void ApplyComputedValues()
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row == null) continue;

                decimal total = 0m;
                try
                {
                    if (row["total"] != DBNull.Value)
                        total = Convert.ToDecimal(row["total"]);
                }
                catch
                {
                    total = 0m;
                }

                string note = string.Empty;
                try
                {
                    note = row["note"] != DBNull.Value ? row["note"].ToString() : string.Empty;
                }
                catch
                {
                    note = string.Empty;
                }

                decimal discount = 0m;
                if (dt.Columns.Contains("discount") && row["discount"] != DBNull.Value)
                {
                    try { discount = Convert.ToDecimal(row["discount"]); }
                    catch { discount = 0m; }
                }
                if (discount <= 0m)
                    discount = TryExtractDiscount(note);
                if (discount < 0) discount = 0;

                row["discount"] = discount;
                row["net_total"] = total;

                // للعرض: نخفي وسم الخصم من عمود الوصف حتى تكون الفاتورة واضحة
                try
                {
                    row["note"] = StripDiscountTag(note);
                }
                catch
                {
                }
            }
        }

        private decimal TryExtractDiscount(string noteText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(noteText)) return 0;

                int start = noteText.IndexOf(DiscountTagPrefix, StringComparison.Ordinal);
                if (start < 0) return 0;
                start += DiscountTagPrefix.Length;

                int end = noteText.IndexOf(DiscountTagSuffix, start, StringComparison.Ordinal);
                if (end < 0) return 0;

                string number = noteText.Substring(start, end - start);
                if (decimal.TryParse(number.Trim().Replace(",", ""), out decimal d))
                    return d;
            }
            catch
            {
            }

            return 0;
        }

        private string StripDiscountTag(string noteText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(noteText)) return noteText;

                int start = noteText.IndexOf(DiscountTagPrefix, StringComparison.Ordinal);
                if (start < 0) return noteText;

                int end = noteText.IndexOf(DiscountTagSuffix, start, StringComparison.Ordinal);
                if (end < 0) return noteText;

                string before = noteText.Substring(0, start).TrimEnd();
                string after = noteText.Substring(end + DiscountTagSuffix.Length).TrimStart();
                return (before + " " + after).Trim();
            }
            catch
            {
                return noteText;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAddOrders_Click(object sender, EventArgs e)
        {
            string nameform="أضافه فاتورة جديده",namebtn="حفظ الفاتورة";
         
            var frm = new Manager_Orders(_FullName);
            frm.TextOrders(nameform, namebtn);
            frm.Mode = Manager_Orders.OrderMode.Add;

            frm.ShowDialog();
            loadData_Orders_Search();
        }

        private void btnEditOrders_Click(object sender, EventArgs e)
        {
             try
    {
        if (dataGridView_Search_Orders.CurrentRow == null)
        {
            MessageBox.Show("حدد فاتورة أولاً", "تنبيه");
            return;
        }

        // 1️⃣ تكوين نصوص العنوان والزر بدون استخدام $ (آمن لكل الإصدارات)
        // استخدم "+" للجمع بين النص والمتغير
        string nameForm = "تعديل الفاتورة:     " + dataGridView_Search_Orders.CurrentRow.Cells["id"].Value.ToString();
        string nameBtn = "تعديل الفاتورة المحددة";

        // 2️⃣ إنشاء فورم Manager_Orders جديد وتمرير اسم المستخدم
        var frm = new Manager_Orders(_FullName);
        frm.TextOrders(nameForm, nameBtn);

        // مهم جداً: تهيئة أعمدة جدول التفاصيل قبل تعبئته من هنا
        // لأن Manager_Orders_Load قد لا يكون اشتغل بعد
        frm.SetDataSRC();

        // تعيين وضعية التعديل
        frm.Mode = Manager_Orders.OrderMode.Edit;

        // 3️⃣ جلب بيانات الصف المحدد من DataGridView
        var row = dataGridView_Search_Orders.CurrentRow;

        int orderId = Convert.ToInt32(row.Cells["id"].Value);

        // 4️⃣ جلب بيانات الفاتورة من المستودع
        var readRepo = new OrderReadRepository();
        var header = readRepo.GetOrderHeaderForEdit(orderId);
        if (header == null)
        {
            MessageBox.Show("الفاتورة غير موجودة", "تنبيه");
            return;
        }

        frm.SetEditDataOrders(header.OrderDate, header.Note, header.CreatedBy,
            header.CustomerId, header.CustomerName, header.CustomerTel, header.Discount);
        frm.idOrder = orderId;

        frm.dt.Rows.Clear();
        var detailRows = readRepo.GetOrderDetailRowsForEdit(orderId);
        foreach (DataRow drDetails in detailRows.Rows)
        {
            DataRow r = frm.dt.NewRow();
            r["المعرف"] = Convert.ToInt32(drDetails["product_id"]);
            r["المنتج"] = drDetails["product_label"].ToString();
            r["تفاصيل"] = drDetails["product_note"] != DBNull.Value ? drDetails["product_note"].ToString() : string.Empty;
            r["الصنف"] = drDetails["category_name"] != DBNull.Value ? drDetails["category_name"].ToString() : string.Empty;
            r["الكمية"] = Convert.ToDecimal(drDetails["qty"]);
            r["السعر"] = Convert.ToDecimal(drDetails["price"]);
            r["المبلغ"] = Convert.ToDecimal(drDetails["total"]);
            frm.dt.Rows.Add(r);
        }

        frm.SetEditPaidAmount(header.PaidAmount);

        // 6️⃣ عرض الفورم كـ Dialog
        frm.ShowDialog();

        // 7️⃣ إعادة تحميل البيانات بعد التعديل
        loadData_Orders_Search();
    }
    catch (Exception ex)
    {
        MessageBox.Show(
            "حدث خطأ أثناء تعديل الفاتورة:\n" + ex.Message,
            "خطأ أثناء الربط",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        );
    }}
    }
}
