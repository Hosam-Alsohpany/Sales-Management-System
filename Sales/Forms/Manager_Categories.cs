using System;

using System.ComponentModel;

using System.Drawing;

using System.Windows.Forms;

using Sales.Models;

using Sales.Repositories;

using Sales.Services;

using Sales.Utilities;



namespace Sales.Forms

{

    public partial class Manager_Categories : Form

    {

        // النماذج

        private CategoryRepository _categoryRepo;

        private BindingList<Category> _categories;



        // عناصر الواجهة

        public Manager_Categories()

        {

            InitializeComponent();

            try { UiTheme.ApplyToForm(this); } catch { }
            DataGridViewDateTimeFormatter.Apply(dgvCategories);

            _categoryRepo = new CategoryRepository();

        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.ExportToPdf(dgvCategories, "الأصناف", "categories.pdf");
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.PrintGrid(dgvCategories, "الأصناف");
        }



        private void Manager_Categories_Load(object sender, EventArgs e)

        {
            if (!AdminFormAccess.EnsureAdminOnLoad(this, "الأصناف"))
                return;

            ConfigureGrid();

            LoadData();

        }



        private void ConfigureGrid()

        {

            if (dgvCategories == null) return;

            try

            {

                dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dgvCategories.AllowUserToAddRows = false;

                dgvCategories.AllowUserToDeleteRows = false;

                dgvCategories.AllowUserToOrderColumns = true;

                dgvCategories.MultiSelect = false;

                dgvCategories.ReadOnly = true;

                dgvCategories.RowHeadersVisible = false;

                dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                dgvCategories.RightToLeft = RightToLeft.Yes;

                try

                {

                    dgvCategories.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgvCategories.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgvCategories.ColumnHeadersDefaultCellStyle.Font = new Font(dgvCategories.Font.FontFamily, 10F, FontStyle.Bold);

                    dgvCategories.DefaultCellStyle.Font = new Font(dgvCategories.Font.FontFamily, 10F, FontStyle.Regular);

                }

                catch

                {

                }

            }

            catch

            {

            }

        }



        private void LoadData()

        {

            try

            {

                var list = _categoryRepo.GetAllCategories();

                _categories = new BindingList<Category>(list);

                dgvCategories.DataSource = null;

                dgvCategories.DataSource = _categories;



                if (dgvCategories.Columns["Id"] != null)

                    dgvCategories.Columns["Id"].Visible = false;



                if (dgvCategories.Columns["Name"] != null)

                    dgvCategories.Columns["Name"].HeaderText = "اسم الصنف";



                try

                {

                    foreach (DataGridViewColumn col in dgvCategories.Columns)

                    {

                        if (col != null)

                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    }

                }

                catch

                {

                }

            }

            catch (UnauthorizedAccessException)

            {

                MessageHelper.ShowUnauthorized("إضافة الأصناف");

            }

            catch (Exception ex)

            {

                MessageHelper.ShowError("خطأ أثناء تحميل الأصناف", ex.Message);

            }

        }



        private void DgvCategories_Click(object sender, EventArgs e)

        {

            if (dgvCategories.CurrentRow != null && !dgvCategories.CurrentRow.IsNewRow)

            {

                txtName.Text = dgvCategories.CurrentRow.Cells["Name"].Value.ToString();

            }

        }



        private void BtnAdd_Click(object sender, EventArgs e)

        {

            if (SessionManager.IsReadOnlyForCurrentUser)
            {
                MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text)) return;



            try

            {

                var cat = new Category { Name = txtName.Text };

                using (TransactionGuard.Enter("Categories.Insert"))
                {
                    _categoryRepo.AddCategory(cat);
                }

                MessageHelper.ShowSuccess("تمت الإضافة بنجاح");

                LoadData();

                txtName.Clear();

            }

            catch (Exception ex)

            {

                MessageHelper.ShowError("خطأ أثناء الإضافة", ex.Message);

            }

        }



        private void BtnUpdate_Click(object sender, EventArgs e)

        {

            if (SessionManager.IsReadOnlyForCurrentUser)
            {
                MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                return;
            }

            if (dgvCategories.CurrentRow == null) return;

            if (string.IsNullOrWhiteSpace(txtName.Text)) return;



            try

            {

                var cat = (Category)dgvCategories.CurrentRow.DataBoundItem;

                cat.Name = txtName.Text;

                using (TransactionGuard.Enter("Categories.Update"))
                {
                    _categoryRepo.UpdateCategory(cat);
                }

                MessageHelper.ShowSuccess("تم التعديل بنجاح");

                LoadData();

                txtName.Clear();

            }

            catch (Exception ex)

            {

                MessageHelper.ShowError("خطأ أثناء التعديل", ex.Message);

            }

        }



        private void BtnDelete_Click(object sender, EventArgs e)

        {

            if (dgvCategories.CurrentRow == null) return;

            if (SessionManager.IsReadOnlyForCurrentUser)
            {
                MessageHelper.ShowWarning("الحساب الحالي بوضع قراءة فقط");
                return;
            }

            if (!SessionManager.CanCurrentUserDelete)
            {
                MessageHelper.ShowUnauthorized("حذف الأصناف");
                return;
            }



            if (MessageHelper.AskForConfirmation("هل أنت متأكد من الحذف؟"))

            {

                try

                {

                    var cat = (Category)dgvCategories.CurrentRow.DataBoundItem;

                    using (TransactionGuard.Enter("Categories.Delete"))
                    {
                        _categoryRepo.DeleteCategory(cat.Id);
                    }

                    MessageHelper.ShowSuccess("تم الحذف بنجاح");

                    LoadData();

                    txtName.Clear();

                }

                catch (UnauthorizedAccessException)

                {

                    MessageHelper.ShowUnauthorized("حذف الأصناف");

                }

                catch (Exception ex)

                {

                    MessageHelper.ShowError("خطأ أثناء الحذف", ex.Message);

                }

            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();

        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewExportHelper.ExportToCsv(dgvCategories, "categories.csv");
            }
            catch
            {
            }
        }

    }

}
