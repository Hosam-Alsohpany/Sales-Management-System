using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sales.Repositories;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class UnitsForm : Form
    {
        #region Fields
        private readonly UnitsRepository _repo = new UnitsRepository();
        private DataTable _dt;
        private readonly BindingSource _bs = new BindingSource();
        private int? _selectedId;
        private string _selectedName;
        private bool _suppressSelectionChanged;

        private Timer _searchDebounceTimer;
        private string _lastSearchNormalized;
        private Label _emptyStateLabel;

        private const string ColSearchKey = "__search_key";
        private const string ColRank = "__rank";
        #endregion

        public UnitsForm()
        {
            InitializeComponent();
        }

        private void UnitsForm_Load(object sender, EventArgs e)
        {
            if (!AdminFormAccess.EnsureAdminOnLoad(this, "الوحدات"))
                return;

            ConfigureGrid();
            EnsureSearchDebounce();
            EnsureEmptyStateOverlay();
            SetModeNew();
            _ = ReloadAsync();
            try { txtUnitName.Focus(); } catch { }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.ExportToPdf(dgvUnits, "الوحدات", "units.pdf", txtSearch?.Text);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataGridViewExportHelper.PrintGrid(dgvUnits, "الوحدات", txtSearch?.Text);
        }

        private void ConfigureGrid()
        {
            if (dgvUnits == null) return;

            try
            {
                dgvUnits.AutoGenerateColumns = false;
                dgvUnits.DataSource = _bs;

                dgvUnits.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvUnits.AllowUserToOrderColumns = true;

                try
                {
                    dgvUnits.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvUnits.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgvUnits.ColumnHeadersDefaultCellStyle.Font = new Font(dgvUnits.Font.FontFamily, 10F, FontStyle.Bold);
                    dgvUnits.DefaultCellStyle.Font = new Font(dgvUnits.Font.FontFamily, 10F, FontStyle.Regular);
                }
                catch
                {
                }
            }
            catch
            {
            }
        }

        private void EnsureEmptyStateOverlay()
        {
            try
            {
                if (dgvUnits == null) return;
                if (_emptyStateLabel != null) return;

                _emptyStateLabel = lblEmptyState;
                if (_emptyStateLabel == null) return;

                try
                {
                    _emptyStateLabel.Font = new Font(dgvUnits.Font.FontFamily, dgvUnits.Font.Size + 2, FontStyle.Bold);
                }
                catch
                {
                }

                dgvUnits.SizeChanged += (s, e) => CenterEmptyStateOverlay();
                CenterEmptyStateOverlay();
            }
            catch
            {
            }
        }

        private void CenterEmptyStateOverlay()
        {
            try
            {
                if (dgvUnits == null || _emptyStateLabel == null) return;

                int x = (dgvUnits.ClientSize.Width - _emptyStateLabel.Width) / 2;
                int y = (dgvUnits.ClientSize.Height - _emptyStateLabel.Height) / 2;
                if (x < 0) x = 0;
                if (y < 0) y = 0;
                _emptyStateLabel.Location = new Point(x, y);
            }
            catch
            {
            }
        }

        private void UpdateEmptyStateOverlay()
        {
            try
            {
                if (_emptyStateLabel == null) return;

                int visibleCount = 0;
                try { visibleCount = _dt?.DefaultView?.Count ?? 0; } catch { visibleCount = 0; }

                _emptyStateLabel.Visible = visibleCount <= 0;
                if (_emptyStateLabel.Visible)
                    CenterEmptyStateOverlay();
            }
            catch
            {
            }
        }

        private enum StatusType
        {
            Info = 0,
            Success = 1,
            Warning = 2,
            Error = 3
        }

        private void SetStatus(string text, StatusType type, int autoClearMs)
        {
            try
            {
                if (lblStatus == null) return;
                lblStatus.Text = string.IsNullOrWhiteSpace(text) ? "جاهز" : text;

                switch (type)
                {
                    case StatusType.Success:
                        lblStatus.ForeColor = Color.DarkGreen;
                        break;
                    case StatusType.Warning:
                        lblStatus.ForeColor = Color.DarkGoldenrod;
                        break;
                    case StatusType.Error:
                        lblStatus.ForeColor = Color.Firebrick;
                        break;
                    default:
                        lblStatus.ForeColor = Color.Black;
                        break;
                }
            }
            catch
            {
            }
        }

        private void SetStatus(string text, bool isError, int autoClearMs)
        {
            SetStatus(text, isError ? StatusType.Error : StatusType.Info, autoClearMs);
        }

        private void ShowError(string userMessage, Exception ex, string logContext)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(logContext))
                    Logger.LogError(nameof(UnitsForm), logContext, ex);
                else
                    Logger.LogError(nameof(UnitsForm), "Unhandled", ex);
            }
            catch
            {
            }

            SetStatus(userMessage, StatusType.Error, autoClearMs: 0);
        }

        private void SetModeNew()
        {
            _selectedId = null;
            _selectedName = null;

            try { txtUnitName.Clear(); } catch { }

            try { btnAdd.Enabled = true; } catch { }
            try { btnUpdate.Enabled = false; } catch { }
            try { btnDelete.Enabled = false; } catch { }

            SetStatus("جاهز", StatusType.Info, autoClearMs: 0);
        }

        private void SetModeEdit(int id, string name)
        {
            _selectedId = id;
            _selectedName = name;

            try
            {
                txtUnitName.Text = name ?? string.Empty;
                txtUnitName.SelectAll();
            }
            catch
            {
            }

            try { btnAdd.Enabled = false; } catch { }
            try { btnUpdate.Enabled = true; } catch { }
            try { btnDelete.Enabled = true; } catch { }

            SetStatus("وضع التعديل", StatusType.Info, autoClearMs: 0);
        }

        private string GetEnteredName()
        {
            try { return (txtUnitName.Text ?? string.Empty).Trim(); } catch { return string.Empty; }
        }

        private bool ValidateNameForSave(string name, bool isUpdate)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                SetStatus("اسم الوحدة مطلوب", StatusType.Warning, autoClearMs: 0);
                try { txtUnitName.Focus(); } catch { }
                return false;
            }

            try
            {
                bool exists = _repo.ExistsByName(name, excludeId: isUpdate ? _selectedId : null);
                if (exists)
                {
                    SetStatus("اسم الوحدة موجود بالفعل", StatusType.Warning, autoClearMs: 0);
                    try { txtUnitName.Focus(); txtUnitName.SelectAll(); } catch { }
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowError("حدث خطأ أثناء التحقق من الاسم", ex, "UnitsForm.ValidateNameForSave");
                return false;
            }

            return true;
        }

        private static string EscapeRowFilterLikeValue(string value)
        {
            if (value == null) return string.Empty;
            return value
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("*", "[*]")
                .Replace("'", "''");
        }

        private static string NormalizeArabic(string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                    return string.Empty;

                string s = input.Trim();
                s = s.Replace('ة', 'ه');
                s = s.Replace('أ', 'ا').Replace('إ', 'ا').Replace('آ', 'ا');

                string formD = s.Normalize(System.Text.NormalizationForm.FormD);
                var sb = new System.Text.StringBuilder(formD.Length);
                for (int i = 0; i < formD.Length; i++)
                {
                    char c = formD[i];
                    var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                    if (cat == UnicodeCategory.NonSpacingMark)
                        continue;
                    sb.Append(c);
                }

                return sb.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLowerInvariant();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(nameof(UnitsForm), "NormalizeArabic", "Normalize failed", ex);
                try { return (input ?? string.Empty).Trim().ToLowerInvariant(); } catch { return string.Empty; }
            }
        }

        private void EnsureSearchColumns()
        {
            if (_dt == null) return;

            try
            {
                if (!_dt.Columns.Contains(ColSearchKey))
                    _dt.Columns.Add(ColSearchKey, typeof(string));
                if (!_dt.Columns.Contains(ColRank))
                    _dt.Columns.Add(ColRank, typeof(int));

                foreach (DataRow r in _dt.Rows)
                {
                    if (r == null) continue;
                    if (r.RowState == DataRowState.Deleted) continue;

                    string name = string.Empty;
                    try { name = r["name"] == DBNull.Value ? string.Empty : (r["name"]?.ToString() ?? string.Empty); } catch { name = string.Empty; }
                    r[ColSearchKey] = NormalizeArabic(name);
                    r[ColRank] = 0;
                }
            }
            catch (Exception ex)
            {
                ShowError("فشل تجهيز البحث", ex, "UnitsForm.EnsureSearchColumns");
            }
        }

        private void ApplySmartSearchNow()
        {
            try
            {
                if (_dt == null) return;

                string q = string.Empty;
                try { q = (txtSearch?.Text ?? string.Empty); } catch { q = string.Empty; }

                string qNorm = NormalizeArabic(q);
                if (string.Equals(qNorm, _lastSearchNormalized, StringComparison.Ordinal))
                {
                    UpdateEmptyStateOverlay();
                    return;
                }
                _lastSearchNormalized = qNorm;

                var view = _dt.DefaultView;

                if (string.IsNullOrWhiteSpace(qNorm))
                {
                    foreach (DataRow r in _dt.Rows)
                    {
                        if (r == null) continue;
                        if (r.RowState == DataRowState.Deleted) continue;
                        try { r[ColRank] = 0; } catch { }
                    }

                    view.RowFilter = string.Empty;
                    try { view.Sort = "name ASC"; } catch { }
                    UpdateEmptyStateOverlay();
                    return;
                }

                foreach (DataRow r in _dt.Rows)
                {
                    if (r == null) continue;
                    if (r.RowState == DataRowState.Deleted) continue;

                    string key = string.Empty;
                    try { key = r[ColSearchKey] == DBNull.Value ? string.Empty : (r[ColSearchKey]?.ToString() ?? string.Empty); } catch { key = string.Empty; }

                    int rank;
                    if (!string.IsNullOrEmpty(key) && key.StartsWith(qNorm, StringComparison.Ordinal))
                        rank = 0;
                    else if (!string.IsNullOrEmpty(key) && key.IndexOf(qNorm, StringComparison.Ordinal) >= 0)
                        rank = 1;
                    else
                        rank = 2;

                    try { r[ColRank] = rank; } catch { }
                }

                view.RowFilter = string.Format("{0} < 2", ColRank);
                try { view.Sort = string.Format("{0} ASC, name ASC", ColRank); } catch { }
                UpdateEmptyStateOverlay();
            }
            catch (Exception ex)
            {
                ShowError("فشل تطبيق البحث", ex, "UnitsForm.ApplySmartSearchNow");
            }
        }

        private void EnsureSearchDebounce()
        {
            if (_searchDebounceTimer != null) return;

            _searchDebounceTimer = new Timer();
            _searchDebounceTimer.Interval = 300;
            _searchDebounceTimer.Tick += (s, e) =>
            {
                try { _searchDebounceTimer.Stop(); } catch { }
                ApplySmartSearchNow();
            };
        }

        private void ConfigureGridColumns()
        {
            if (dgvUnits == null) return;

            try
            {
                foreach (DataGridViewColumn col in dgvUnits.Columns)
                {
                    try { col.SortMode = DataGridViewColumnSortMode.Automatic; } catch { }
                }

                if (dgvUnits.Columns.Contains("id"))
                {
                    dgvUnits.Columns["id"].Visible = false;
                }

                if (dgvUnits.Columns.Contains(ColSearchKey))
                {
                    dgvUnits.Columns[ColSearchKey].Visible = false;
                }

                if (dgvUnits.Columns.Contains(ColRank))
                {
                    dgvUnits.Columns[ColRank].Visible = false;
                }

                if (dgvUnits.Columns.Contains("name"))
                {
                    dgvUnits.Columns["name"].HeaderText = "اسم الوحدة";
                }

                try
                {
                    foreach (DataGridViewColumn col in dgvUnits.Columns)
                    {
                        if (col != null)
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }
                catch
                {
                }
            }
            catch
            {
            }
        }

        private int FindRowIndexById(int id)
        {
            try
            {
                if (dgvUnits == null) return -1;
                for (int i = 0; i < dgvUnits.Rows.Count; i++)
                {
                    var row = dgvUnits.Rows[i];
                    if (row == null || row.IsNewRow) continue;
                    object idObj = null;
                    try { idObj = row.Cells["id"].Value; } catch { idObj = null; }
                    if (idObj == null || idObj == DBNull.Value) continue;
                    if (int.TryParse(idObj.ToString(), out int rid) && rid == id)
                        return i;
                }
            }
            catch
            {
            }

            return -1;
        }

        private void SelectRowById(int id)
        {
            if (dgvUnits == null) return;

            int idx = FindRowIndexById(id);
            if (idx < 0) return;

            try
            {
                _suppressSelectionChanged = true;
                dgvUnits.ClearSelection();
                dgvUnits.Rows[idx].Selected = true;
                try
                {
                    if (dgvUnits.Columns.Contains("name"))
                        dgvUnits.CurrentCell = dgvUnits.Rows[idx].Cells["name"];
                    else
                        dgvUnits.CurrentCell = dgvUnits.Rows[idx].Cells[0];
                }
                catch
                {
                }

                try { dgvUnits.FirstDisplayedScrollingRowIndex = idx; } catch { }
            }
            catch
            {
            }
            finally
            {
                _suppressSelectionChanged = false;
            }
        }

        private void FocusNameBox(bool selectAll)
        {
            try
            {
                txtUnitName.Focus();
                if (selectAll)
                    txtUnitName.SelectAll();
            }
            catch
            {
            }
        }

        private async void HighlightRowById(int id, Color highlightColor, int durationMs)
        {
            if (dgvUnits == null) return;

            int idx = FindRowIndexById(id);
            if (idx < 0) return;

            DataGridViewRow row = null;
            try { row = dgvUnits.Rows[idx]; } catch { row = null; }
            if (row == null) return;

            Color oldBack = Color.Empty;
            try { oldBack = row.DefaultCellStyle.BackColor; } catch { oldBack = Color.Empty; }

            try { row.DefaultCellStyle.BackColor = highlightColor; } catch { }

            try { await Task.Delay(durationMs); } catch { }

            try
            {
                if (row.Index >= 0 && row.Index < dgvUnits.Rows.Count)
                    dgvUnits.Rows[row.Index].DefaultCellStyle.BackColor = oldBack;
            }
            catch
            {
            }
        }

        private async Task ReloadAsync(int? preserveSelectedId = null)
        {
            SetStatus("تحميل...", StatusType.Info, autoClearMs: 0);
            try
            {
                var dt = await Task.Run(() => _repo.GetAll());
                _dt = dt;

                try { _dt.CaseSensitive = false; } catch { }
                try { _dt.DefaultView.Sort = "name ASC"; } catch { }

                EnsureSearchColumns();

                _bs.DataSource = _dt.DefaultView;
                ApplySmartSearchNow();
                ConfigureGridColumns();

                if (preserveSelectedId.HasValue)
                    SelectRowById(preserveSelectedId.Value);

                SetStatus("تم التحميل", StatusType.Success, autoClearMs: 0);
                UpdateEmptyStateOverlay();
            }
            catch (Exception ex)
            {
                ShowError("فشل التحميل", ex, "UnitsForm.ReloadAsync");
                UpdateEmptyStateOverlay();
            }
        }

        private async void btnReload_Click(object sender, EventArgs e)
        {
            await ReloadAsync(preserveSelectedId: _selectedId);
            SetModeNew();
            FocusNameBox(selectAll: false);
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

            try { ApplySmartSearchNow(); } catch { }
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewExportHelper.ExportToCsv(dgvUnits, "units.csv");
            }
            catch
            {
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            SetModeNew();
            FocusNameBox(selectAll: false);
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string name = GetEnteredName();
            if (!ValidateNameForSave(name, isUpdate: false)) return;

            try
            {
                int newId = await Task.Run(() =>
                {
                    using (TransactionGuard.Enter("Units.Insert"))
                    {
                        return _repo.Add(name);
                    }
                });

                try
                {
                    if (_dt != null)
                    {
                        var row = _dt.NewRow();
                        row["id"] = newId;
                        row["name"] = name;
                        if (_dt.Columns.Contains(ColSearchKey))
                            row[ColSearchKey] = NormalizeArabic(name);
                        if (_dt.Columns.Contains(ColRank))
                            row[ColRank] = 0;
                        _dt.Rows.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("UnitsForm.btnAdd_Click local insert failed", ex);
                }

                SetStatus("تمت الإضافة", StatusType.Success, autoClearMs: 0);
                ApplySmartSearchNow();
                SelectRowById(newId);
                SetModeEdit(newId, name);
                FocusNameBox(selectAll: true);

                HighlightRowById(newId, Color.FromArgb(210, 255, 235), 1000);
                UpdateEmptyStateOverlay();
            }
            catch (Exception ex)
            {
                ShowError("فشل الإضافة", ex, "UnitsForm.btnAdd_Click");
                FocusNameBox(selectAll: true);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!_selectedId.HasValue)
            {
                SetStatus("اختر وحدة للتعديل", StatusType.Warning, autoClearMs: 0);
                return;
            }

            string name = GetEnteredName();
            if (!ValidateNameForSave(name, isUpdate: true)) return;

            try
            {
                int id = _selectedId.Value;
                await Task.Run(() =>
                {
                    using (TransactionGuard.Enter("Units.Update"))
                    {
                        _repo.Update(id, name);
                    }
                });

                try
                {
                    if (_dt != null)
                    {
                        foreach (DataRow r in _dt.Rows)
                        {
                            if (r == null) continue;
                            if (r.RowState == DataRowState.Deleted) continue;
                            if (r["id"] == DBNull.Value) continue;
                            if (Convert.ToInt32(r["id"]) == id)
                            {
                                r["name"] = name;
                                try { r[ColSearchKey] = NormalizeArabic(name); } catch { }
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("UnitsForm.btnUpdate_Click local update failed", ex);
                }

                SetStatus("تم التعديل", StatusType.Success, autoClearMs: 0);
                ApplySmartSearchNow();
                SelectRowById(id);
                SetModeEdit(id, name);
                FocusNameBox(selectAll: true);

                HighlightRowById(id, Color.FromArgb(225, 240, 255), 1000);
            }
            catch (Exception ex)
            {
                ShowError("فشل التعديل", ex, "UnitsForm.btnUpdate_Click");
                FocusNameBox(selectAll: true);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!_selectedId.HasValue)
            {
                SetStatus("اختر وحدة للحذف", StatusType.Warning, autoClearMs: 0);
                return;
            }

            var confirm = MessageBox.Show(
                "تأكيد حذف الوحدة؟\n" + (_selectedName ?? string.Empty),
                "تأكيد",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                int id = _selectedId.Value;
                int currentIndex = -1;
                try { currentIndex = dgvUnits?.CurrentRow?.Index ?? -1; } catch { currentIndex = -1; }

                await Task.Run(() =>
                {
                    using (TransactionGuard.Enter("Units.Delete"))
                    {
                        _repo.Delete(id);
                    }
                });

                try
                {
                    if (_dt != null)
                    {
                        for (int i = _dt.Rows.Count - 1; i >= 0; i--)
                        {
                            var r = _dt.Rows[i];
                            if (r == null) continue;
                            if (r.RowState == DataRowState.Deleted) continue;
                            if (r["id"] == DBNull.Value) continue;
                            if (Convert.ToInt32(r["id"]) == id)
                            {
                                _dt.Rows.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("UnitsForm.btnDelete_Click local remove failed", ex);
                }

                SetStatus("تم الحذف", StatusType.Success, autoClearMs: 0);
                ApplySmartSearchNow();

                int targetIndex = currentIndex;
                if (targetIndex >= dgvUnits.Rows.Count)
                    targetIndex = dgvUnits.Rows.Count - 1;

                if (targetIndex >= 0 && dgvUnits.Rows.Count > 0)
                {
                    try
                    {
                        _suppressSelectionChanged = true;
                        dgvUnits.ClearSelection();
                        dgvUnits.Rows[targetIndex].Selected = true;
                        try
                        {
                            if (dgvUnits.Columns.Contains("name"))
                                dgvUnits.CurrentCell = dgvUnits.Rows[targetIndex].Cells["name"];
                            else
                                dgvUnits.CurrentCell = dgvUnits.Rows[targetIndex].Cells[0];
                        }
                        catch
                        {
                        }
                        try { dgvUnits.FirstDisplayedScrollingRowIndex = targetIndex; } catch { }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _suppressSelectionChanged = false;
                    }

                    try
                    {
                        object idObj = null;
                        try { idObj = dgvUnits.Rows[targetIndex].Cells["id"].Value; } catch { idObj = null; }
                        if (idObj != null && idObj != DBNull.Value && int.TryParse(idObj.ToString(), out int nextId) && nextId > 0)
                            HighlightRowById(nextId, Color.FromArgb(255, 250, 220), 700);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    SetModeNew();
                }

                FocusNameBox(selectAll: false);
                UpdateEmptyStateOverlay();
            }
            catch (SQLiteException ex) when (ex.ErrorCode == SQLiteErrorCode.Constraint && (ex.Message ?? string.Empty).IndexOf("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ShowError("لا يمكن حذف هذه الوحدة لأنها مستخدمة في منتجات", ex, "UnitsForm.btnDelete_Click FK");
                FocusNameBox(selectAll: false);
            }
            catch (Exception ex)
            {
                ShowError("فشل الحذف", ex, "UnitsForm.btnDelete_Click");
                FocusNameBox(selectAll: false);
            }
        }

        private void dgvUnits_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelectionChanged) return;

            try
            {
                if (dgvUnits == null || dgvUnits.CurrentRow == null)
                {
                    SetModeNew();
                    return;
                }

                if (dgvUnits.CurrentRow.IsNewRow)
                {
                    SetModeNew();
                    return;
                }

                object idObj = null;
                object nameObj = null;
                try { idObj = dgvUnits.CurrentRow.Cells["id"].Value; } catch { idObj = null; }
                try { nameObj = dgvUnits.CurrentRow.Cells["name"].Value; } catch { nameObj = null; }

                if (idObj == null || idObj == DBNull.Value)
                {
                    SetModeNew();
                    return;
                }

                if (!int.TryParse(idObj.ToString(), out int id) || id <= 0)
                {
                    SetModeNew();
                    return;
                }

                string name = nameObj == null || nameObj == DBNull.Value ? string.Empty : nameObj.ToString();
                SetModeEdit(id, name);
            }
            catch
            {
                // keep UX stable
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (_searchDebounceTimer == null)
                    EnsureSearchDebounce();

                _searchDebounceTimer.Stop();
                _searchDebounceTimer.Start();
            }
            catch
            {
                ApplySmartSearchNow();
            }
        }
    }
}
