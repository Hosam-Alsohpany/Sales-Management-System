// ============================================================
// الملف    : BarcodeSetupForm.cs
// الغرض    : شاشة تهيئة باركود الموازين وفك ترميز البيانات الموزونة
// ============================================================

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sales.Repositories;
using Sales.Services.BarcodePlatform;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class BarcodeSetupForm : Form
    {
        private readonly BarcodeProfilesRepository _repo = new BarcodeProfilesRepository();
        private readonly ProductRepository _productRepo = new ProductRepository();

        private readonly BindingList<BarcodeProfileRow> _rows = new BindingList<BarcodeProfileRow>();

        public BarcodeSetupForm()
        {
            InitializeComponent();
            UiTheme.ApplyToForm(this);
            if (!AdminFormAccess.EnsureAdminOnLoad(this, "إعدادات الباركود"))
                return;
            EnsureGridInitialized();
            AdjustLayout();
            LoadProfiles();
        }

        private void EnsureGridInitialized()
        {
            if (IsDesignTime()) return;
            if (dgv == null) return;

            try
            {
                dgv.SuspendLayout();

                dgv.AutoGenerateColumns = false;
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = false;
                dgv.MultiSelect = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                if (dgv.Columns == null || dgv.Columns.Count == 0)
                {
                    var colCode = new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Code",
                        HeaderText = "Code",
                        Width = 130,
                        ReadOnly = true
                    };

                    var colName = new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Name",
                        HeaderText = "Name",
                        Width = 220
                    };

                    var colEnabled = new DataGridViewCheckBoxColumn
                    {
                        DataPropertyName = "IsEnabled",
                        HeaderText = "Enabled",
                        Width = 90
                    };

                    var colPriority = new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Priority",
                        HeaderText = "Priority",
                        Width = 90
                    };

                    var colCfg = new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "ConfigJson",
                        HeaderText = "Config JSON",
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    };

                    dgv.Columns.AddRange(new DataGridViewColumn[] { colCode, colName, colEnabled, colPriority, colCfg });
                }

                dgv.DataSource = _rows;
            }
            catch
            {
            }
            finally
            {
                try { dgv.ResumeLayout(true); } catch { }
            }
        }

        private static bool IsDesignTime()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        private void AdjustLayout()
        {
            if (IsDesignTime()) return;

            try
            {
                foreach (Control c in Controls)
                {
                    if (c is GroupBox g)
                    {
                        g.Width = ClientSize.Width - 40;

                        foreach (Control inner in g.Controls)
                        {
                            if (inner is Label lbl)
                                lbl.Width = g.Width - 40;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadProfiles();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveProfiles();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            TestBarcode();
        }

        private void BarcodeSetupForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void LoadProfiles()
        {
            _rows.Clear();

            var list = _repo.GetAllProfiles();
            if (list == null || list.Count == 0)
            {
                MessageHelper.ShowWarning("لا توجد Profiles في قاعدة البيانات. تأكد من تشغيل البرنامج بعد التحديث.");
                return;
            }

            foreach (var r in list)
                _rows.Add(r);
        }

        private void SaveProfiles()
        {
            try
            {
                foreach (var r in _rows.ToList())
                {
                    if (r == null) continue;
                    _repo.UpsertProfile(r);
                }

                MessageHelper.ShowInfo("تم الحفظ");
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("فشل الحفظ: " + ex.Message);
            }
        }

        private void TestBarcode()
        {
            string code = (txtTestCode.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                lblTestResult.Text = "";
                return;
            }

            try
            {
                var engine = BarcodeResolverEngine.CreateDefault(new ProductRepositoryBarcodeDataStore(_productRepo));
                var r = engine.Resolve(new BarcodeResolveRequest
                {
                    RawCode = code,
                    UserName = string.Empty,
                    Terminal = Environment.MachineName
                });

                if (r == null)
                {
                    lblTestResult.Text = "No result";
                    return;
                }

                if (r.Status == BarcodeResolveStatus.Resolved && r.Item != null)
                {
                    lblTestResult.Text = "Resolved" + Environment.NewLine +
                                         "Profile: " + (r.Item.ProfileCode ?? string.Empty) + Environment.NewLine +
                                         "ProductUnitId: " + r.Item.ProductUnitId + Environment.NewLine +
                                         "Qty: " + r.Item.Qty;
                    return;
                }

                lblTestResult.Text = r.Status + (string.IsNullOrWhiteSpace(r.Message) ? string.Empty : (Environment.NewLine + r.Message));
            }
            catch (Exception ex)
            {
                lblTestResult.Text = "Error: " + ex.Message;
            }
        }
    }
}
