using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using Sales.Forms;
using Sales.Database;
using Sales.Utilities;
namespace Sales.Forms
{
    public partial class Customers_List : Form
    {
        public string SelectedCustomerID { get;private set; }
        public string SelectedCustomerName { get; private set; }
        public string SelectedCustomerTel { get; private set; }
        public Customers_List()
        {
            InitializeComponent();
            try { UiTheme.ApplyToForm(this); } catch { }
            DataGridViewDateTimeFormatter.Apply(dataGridView_Customer);
            try { dataGridView_Customer.AutoGenerateColumns = false; } catch { }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.ExportToPdf(dataGridView_Customer, "قائمة العملاء", "customers_list.pdf", txtSearch?.Text);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.PrintGrid(dataGridView_Customer, "قائمة العملاء", txtSearch?.Text);
        }

        private DataTable dt = new DataTable();
        private SQLiteDataAdapter adapter;   
     
        
        
        private void loadData_Customers_List()
        {
            try
            {
                dt.Clear(); // أنظف الجدول بالكامل

                using (SQLiteConnection con =
                       new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    string query = "SELECT id, name , tel FROM Customers";



                    /*لا تُنشئ Adapter داخل using إذا ستستخدمه لاحقًا
     أنت تستخدم adapter لاحقًا في زر الحفظ*/
                    // using( adapter = new SQLiteDataAdapter(query,con))//لأن using قد يتسبب في التخلص من الكائن مبكرًا.
                    adapter = new SQLiteDataAdapter(query, con);
                    adapter.Fill(dt);

                    //  dataGridView_Customer.AutoGenerateColumns = true;
                    try { dataGridView_Customer.AutoGenerateColumns = false; } catch { }
                    dataGridView_Customer.DataSource = dt;

                    // إخفاء عمود ID (الأكثر أمانًا)
                   if (dataGridView_Customer.Columns.Contains("ColId"))//معناها حرفيًاهل يوجد عمود في DataGridView اسمه ColId؟
                        dataGridView_Customer.Columns["ColId"].Visible = false;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                      this,
                      "حدث خطأ أثناء تحميل البيانات:\n" + ex.Message,
                      "خطأ",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error
                  );
            }
        }

        private void Customers_List_Load(object sender, EventArgs e)
        {
            loadData_Customers_List();
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
            try
            {
                DataGridViewExportHelper.ExportToCsv(dataGridView_Customer, "customers_list.csv");
            }
            catch
            {
            }
        }

        private void ApplySearchFilter()
        {
            try
            {
                if (dt == null) return;

                string text = string.Empty;
                try { text = (txtSearch != null ? txtSearch.Text : string.Empty) ?? string.Empty; } catch { text = string.Empty; }
                text = text.Trim().Replace("'", "''");

                if (string.IsNullOrWhiteSpace(text))
                {
                    dt.DefaultView.RowFilter = string.Empty;
                    return;
                }

                dt.DefaultView.RowFilter =
                    $"Convert(id, 'System.String') LIKE '%{text}%' OR " +
                    $"name LIKE '%{text}%' OR " +
                    $"tel LIKE '%{text}%'";
            }
            catch
            {
                try
                {
                    if (dt != null) dt.DefaultView.RowFilter = string.Empty;
                }
                catch
                {
                }
            }
        }

        private void dataGridView_Customer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // حماية: الصف غير صالح

            DataGridViewRow row = dataGridView_Customer.Rows[e.RowIndex];
            if (row == null) return;

            // Id
            if (row.Cells["ColId"] != null && row.Cells["ColId"].Value != null)
                SelectedCustomerID = row.Cells["ColId"].Value.ToString();
            else
                SelectedCustomerID = "";

            // Name
            if (row.Cells["ColName"] != null && row.Cells["ColName"].Value != null)
                SelectedCustomerName = row.Cells["ColName"].Value.ToString();
            else
                SelectedCustomerName = "";

            // Tel
            if (row.Cells["ColTel"] != null && row.Cells["ColTel"].Value != null)
                SelectedCustomerTel = row.Cells["ColTel"].Value.ToString();
            else
                SelectedCustomerTel = "";

            // التحقق أن العميل صالح
            if (string.IsNullOrEmpty(SelectedCustomerID))
            {
                MessageBox.Show(
                    this,
                    "البيانات المحددة غير صالحة",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // إرجاع النتيجة للفورم الأب وإغلاق الفورم
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        // سوف نتوقف من استخدام هذا الحدث والسبب لاج
        /*private void dataGridView_Customer_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           // if (e.RowIndex >= 0) {
            //    SelectedCustomerID = dataGridView_Customer.Rows[e.RowIndex].Cells["ColId"].Value.ToString();
            //    SelectedCustomerName = dataGridView_Customer.Rows[e.RowIndex].Cells["ColName"].Value.ToString();
              //  SelectedCustomerTel = dataGridView_Customer.Rows[e.RowIndex].Cells["ColTel"].Value.ToString();
                //this.DialogResult = DialogResult.OK;
                //this.Close(); // طريقه اكثر تحسينا

   // حماية: الضغط على الهيدر أو صف غير صالح
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView_Customer.Rows[e.RowIndex];

            // حماية إضافية: التأكد أن الصف غير فارغ
            if (row == null) return;

            // استخراج القيم بأمان (Null-Safe)
        //    SelectedCustomerID =
         //       row.Cells["ColId"]?.Value?.ToString() ?? string.Empty;

         //   SelectedCustomerName =
           //     row.Cells["ColName"]?.Value?.ToString() ?? string.Empty;

          //  SelectedCustomerTel =
           //    row.Cells["ColTel"]?.Value?.ToString() ?? string.Empty; // هنا مشكله بسبب الاصدار سوف نستخدم داله if

            if (row.Cells["ColId"] != null && row.Cells["ColId"].Value != null)
                SelectedCustomerID = row.Cells["ColId"].Value.ToString();
            else
                SelectedCustomerID = "";

            // Name
            if (row.Cells["ColName"] != null && row.Cells["ColName"].Value != null)
                SelectedCustomerName = row.Cells["ColName"].Value.ToString();
            else
                SelectedCustomerName = "";

            // Tel
            if (row.Cells["ColTel"] != null && row.Cells["ColTel"].Value != null)
                SelectedCustomerTel = row.Cells["ColTel"].Value.ToString();
            else
                SelectedCustomerTel = "";

            // في حال لم يتم اختيار عميل فعلي
            if (string.IsNullOrWhiteSpace(SelectedCustomerID))
            {
                MessageBox.Show(
                    this,
                    "البيانات المحددة غير صالحة",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // إرجاع النتيجة للفورم الأب
            this.DialogResult = DialogResult.OK;
            this.Close();            }*/
        }

      
    }

