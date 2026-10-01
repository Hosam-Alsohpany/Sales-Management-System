using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using Sales.Models;
using Sales.Repositories;
using Sales.Services.BarcodePlatform;
using Sales.Utilities;

namespace Sales.Forms
{
    public partial class POSForm : Form
    {
        private string _draftRequestId;
        private readonly string _fullName;
        private readonly ProductRepository _productRepo = new ProductRepository();
        private readonly OrderRepository _orderRepo = new OrderRepository();
        private readonly OrderReadRepository _orderReadRepo = new OrderReadRepository();
        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private readonly PosHotkeysRepository _hotkeysRepo = new PosHotkeysRepository();

        private int? _selectedCustomerId;

        private readonly BindingList<SaleLine> _lines = new BindingList<SaleLine>();

        // Cache: barcode -> ProductUnitLookup (loaded once, refreshed on demand)
        private Dictionary<string, ProductUnitLookup> _productUnitByBarcode = new Dictionary<string, ProductUnitLookup>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<int, ProductUnitLookup> _productUnitByProductUnitId = new Dictionary<int, ProductUnitLookup>();
        private DateTime _cacheLoadedAtUtc;

        private BarcodeResolverEngine _barcodeEngine;

        private Dictionary<int, PosHotkeysRepository.HotkeyEntry> _hotkeysByKeyCode = new Dictionary<int, PosHotkeysRepository.HotkeyEntry>();

        private readonly Timer _scanTimer = new Timer();
        private bool _scanTimerHooked;

        private DateTime _lastScanTextChangedAtUtc;
        private int _fastScanChangeStreak;
        private DateTime _scannerModeUntilUtc;
        private DateTime _lastScannerSoftErrorAtUtc;

        private string _lastAutoScannedCode;
        private DateTime _lastAutoScanAtUtc;

        private readonly Timer _statusResetTimer = new Timer();
        private bool _statusTimerHooked;

        private DateTime _lastQtyAdjustAtUtc;

        public POSForm(string fullName)
        {
            _fullName = fullName;
            InitializeComponent();

            UiTheme.ApplyToForm(this);
            WireEvents();

            ReloadBarcodeCacheOrShowError();
            ReloadHotkeysOrShowError();

            try
            {
                _barcodeEngine = BarcodeResolverEngine.CreateDefault(new ProductRepositoryBarcodeDataStore(_productRepo));
            }
            catch
            {
                _barcodeEngine = null;
            }

            dgvLines.AutoGenerateColumns = false;
            dgvLines.DataSource = _lines;

            lblCashierValue.Text = _fullName;

            try
            {
                txtPaid.ReadOnly = true;
            }
            catch { }

            try
            {
                if (btnPay != null)
                    btnPay.Enabled = false;
            }
            catch { }

            txtPaid.Text = "0";
            ResetDraft();
            RefreshRecentOrders();
        }

        private void btnPickCustomer_Click(object sender, EventArgs e) => PickCustomer();

        private void lstRecentOrders_DoubleClick(object sender, EventArgs e) => ShowRecentOrderHint();

        private void PickCustomer()
        {
            using (var f = new CustomerPickerForm())
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;
                _selectedCustomerId = f.SelectedCustomerId;
                lblCustomerValue.Text = string.IsNullOrWhiteSpace(f.SelectedCustomerName) ? "—" : f.SelectedCustomerName;
                RefreshCustomerBalance();
                if (_selectedCustomerId.HasValue)
                    RefreshRecentOrdersForCustomer(_selectedCustomerId.Value);
                else
                    RefreshRecentOrders();
            }
            FocusScan();
        }

        private void RefreshCustomerBalance()
        {
            if (!_selectedCustomerId.HasValue || _selectedCustomerId.Value <= 0)
            {
                lblCustomerBalance.Text = "0";
                return;
            }
            try
            {
                decimal bal = _customerRepo.GetCustomerBalance(_selectedCustomerId.Value);
                lblCustomerBalance.Text = SalesNumberFormat.FormatPrice(bal);
            }
            catch
            {
                lblCustomerBalance.Text = "—";
            }
        }

        private void RefreshRecentOrders()
        {
            try
            {
                var list = _orderReadRepo.GetRecentOrders(5) ?? new List<OrderReadRepository.RecentOrderSummary>();
                lstRecentOrders.Items.Clear();
                foreach (var o in list)
                {
                    string cust = string.IsNullOrWhiteSpace(o.CustomerName) ? "" : " | " + o.CustomerName;
                    lstRecentOrders.Items.Add("#" + o.OrderId + " " + o.OrderDate.ToString("MM/dd HH:mm") +
                        " " + SalesNumberFormat.FormatPrice(o.Total) + cust);
                }
            }
            catch { }
        }

        private void RefreshRecentOrdersForCustomer(int customerId)
        {
            if (customerId <= 0) return;
            try
            {
                var list = _orderReadRepo.GetRecentOrdersForCustomer(customerId, 5) ?? new List<OrderReadRepository.RecentOrderSummary>();
                lstRecentOrders.Items.Clear();
                foreach (var o in list)
                {
                    lstRecentOrders.Items.Add("#" + o.OrderId + " " + o.OrderDate.ToString("MM/dd HH:mm") +
                        " " + SalesNumberFormat.FormatPrice(o.Total));
                }
            }
            catch { }
        }

        private void ShowRecentOrderHint()
        {
            MessageBox.Show(this, "لتعديل فاتورة سابقة استخدم شاشة إدارة الفواتير.", "آخر الفواتير",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void ShowPosHelp()
        {
            MessageBox.Show(
                "F1 مساعدة | F2 بحث | F3 عميل | F4 دفع/اعتماد\r\n" +
                "F5 خصم | F9 إلغاء السلة | Ctrl+F5 تحديث المنتجات\r\n" +
                "Del حذف سطر | F7 تقليل كمية | Ctrl+H اختصارات المنتجات",
                "اختصارات POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ClearSelectedCustomer()
        {
            _selectedCustomerId = null;
            lblCustomerValue.Text = "—";
            RefreshCustomerBalance();
        }

        private void WireEvents()
        {
            KeyPreview = true;
            KeyDown += POSForm_KeyDown;

            Deactivate += (s, e) => HideSuggestions();

            txtScan.KeyDown += txtScan_KeyDown;
            txtScan.TextChanged += txtScan_TextChanged;
            txtScan.LostFocus += (s, e) => HideSuggestions();
            // Paid is derived from Net (FULL PAYMENT ONLY)
            txtPaid.TextChanged += (s, e) => RecalcTotals();

            btnPost.Click += (s, e) => PostOrder();
            btnPay.Click += (s, e) => PostOrder();

            btnRemoveLine.Click += (s, e) => RemoveSelectedLine();
            btnClear.Click += (s, e) => ClearCart();

            try
            {
                if (btnDecLine != null)
                    btnDecLine.Click += (s, e) => DecrementSelectedLine();
            }
            catch { }

            FocusScan();

            if (!_scanTimerHooked)
            {
                _scanTimerHooked = true;
                _scanTimer.Interval = 120;
                _scanTimer.Tick += (s, e) =>
                {
                    _scanTimer.Stop();
                    ProcessScanBuffer(ScanCommitSource.ScannerAuto);
                };
            }

            if (!_statusTimerHooked)
            {
                _statusTimerHooked = true;
                _statusResetTimer.Interval = 1500;
                _statusResetTimer.Tick += (s, e) =>
                {
                    _statusResetTimer.Stop();
                    try
                    {
                        lblAlert.BackColor = Color.Silver;
                        lblAlert.ForeColor = Color.Black;
                    }
                    catch { }
                };
            }

            lstSuggestions.MouseDoubleClick += (s, e) => AcceptSuggestionFromList();
            lstSuggestions.KeyDown += lstSuggestions_KeyDown;
            lstSuggestions.LostFocus += (s, e) => HideSuggestions();

            // Hide suggestions if user clicks anywhere else
            splitMain.Panel1.MouseDown += (s, e) =>
            {
                if (!lstSuggestions.Bounds.Contains(e.Location))
                    HideSuggestions();
            };
            dgvLines.MouseDown += (s, e) => HideSuggestions();
        }

        private void POSForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null) return;

            if (e.Control && e.KeyCode == Keys.F)
            {
                e.SuppressKeyPress = true;
                try
                {
                    txtScan.Focus();
                    txtScan.SelectAll();
                }
                catch { }
                return;
            }

            if (e.Control && e.KeyCode == Keys.H)
            {
                e.SuppressKeyPress = true;
                using (var f = new PosHotkeysForm())
                {
                    f.ShowDialog(this);
                }
                ReloadHotkeysOrShowError();
                FocusScan();
                return;
            }

            if (e.Control && e.KeyCode == Keys.F5)
            {
                e.SuppressKeyPress = true;
                ReloadBarcodeCacheOrShowError();
                ReloadHotkeysOrShowError();
                FocusScan();
                return;
            }

            if (e.KeyCode == Keys.F1)
            {
                e.SuppressKeyPress = true;
                ShowPosHelp();
                FocusScan();
                return;
            }

            if (e.KeyCode == Keys.F2)
            {
                e.SuppressKeyPress = true;
                try { txtScan.Focus(); txtScan.SelectAll(); } catch { }
                return;
            }

            if (e.KeyCode == Keys.F3)
            {
                e.SuppressKeyPress = true;
                PickCustomer();
                return;
            }

            if (e.KeyCode == Keys.F4)
            {
                e.SuppressKeyPress = true;
                PostOrder();
                return;
            }

            if (e.KeyCode == Keys.F5)
            {
                e.SuppressKeyPress = true;
                try { txtDiscountTotal.Focus(); txtDiscountTotal.SelectAll(); } catch { }
                return;
            }

            if (e.KeyCode == Keys.F7)
            {
                e.SuppressKeyPress = true;
                DecrementSelectedLine();
                return;
            }

            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                int keyCode = (int)e.KeyCode;
                if (_hotkeysByKeyCode.TryGetValue(keyCode, out var hk) && hk != null)
                {
                    e.SuppressKeyPress = true;
                    ApplyHotkey(hk);
                    FocusScan();
                    return;
                }
            }

            if (e.KeyCode == Keys.F12)
            {
                e.SuppressKeyPress = true;
                PostOrder();
                return;
            }

            if (e.KeyCode == Keys.F9)
            {
                e.SuppressKeyPress = true;
                ClearCart();
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                e.SuppressKeyPress = true;
                RemoveSelectedLine();
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                ClearCart();
                return;
            }

            if (e.Control && e.KeyCode == Keys.L)
            {
                e.SuppressKeyPress = true;
                txtScan.Focus();
                return;
            }
        }

        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null) return;

            if (lstSuggestions.Visible)
            {
                if (e.KeyCode == Keys.Down)
                {
                    e.SuppressKeyPress = true;
                    if (lstSuggestions.Items.Count > 0)
                        lstSuggestions.SelectedIndex = Math.Min(lstSuggestions.Items.Count - 1, Math.Max(0, lstSuggestions.SelectedIndex + 1));
                    return;
                }
                if (e.KeyCode == Keys.Up)
                {
                    e.SuppressKeyPress = true;
                    if (lstSuggestions.Items.Count > 0)
                        lstSuggestions.SelectedIndex = Math.Max(0, lstSuggestions.SelectedIndex - 1);
                    return;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    AcceptSuggestionFromList();
                    return;
                }
                if (e.KeyCode == Keys.Tab)
                {
                    e.SuppressKeyPress = true;
                    AcceptSuggestionFromList();
                    return;
                }
                if (e.KeyCode == Keys.Escape)
                {
                    e.SuppressKeyPress = true;
                    HideSuggestions();
                    return;
                }
            }

            if (e.KeyCode != Keys.Enter && e.KeyCode != Keys.Tab) return;
            e.SuppressKeyPress = true;

            _scanTimer.Stop();
            ProcessScanBuffer(ScanCommitSource.UserCommit);
        }

        private void txtScan_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!txtScan.Focused) return;

                // Hide suggestions immediately on new input (UX requirement)
                if (lstSuggestions.Visible)
                    HideSuggestions();

                var now = DateTime.UtcNow;
                if (_lastScanTextChangedAtUtc != default(DateTime))
                {
                    var deltaMs = (now - _lastScanTextChangedAtUtc).TotalMilliseconds;
                    if (deltaMs <= 35)
                        _fastScanChangeStreak++;
                    else
                        _fastScanChangeStreak = 0;

                    if (_fastScanChangeStreak >= 4)
                        _scannerModeUntilUtc = now.AddSeconds(2);
                }
                _lastScanTextChangedAtUtc = now;

                _scanTimer.Stop();
                _scanTimer.Start();
            }
            catch { }
        }

        private void ProcessScanBuffer(ScanCommitSource source)
        {
            string raw = (txtScan.Text ?? string.Empty);
            if (string.IsNullOrWhiteSpace(raw))
            {
                FocusScan();
                return;
            }

            txtScan.Clear();

            string normalized = raw.Replace("\r\n", "\n").Replace("\r", "\n");

            // Many scanners send a single line, some send multi-lines, and some devices may inject tabs/spaces.
            // We normalize by splitting on \n first, then splitting each line by whitespace.
            var parts = new System.Collections.Generic.List<string>();
            foreach (var line in normalized.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line == null) continue;
                string ln = line.Trim();
                if (ln.Length == 0) continue;

                var tokens = ln.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                foreach (var t in tokens)
                {
                    string code = NormalizeScanCode(t);
                    if (string.IsNullOrWhiteSpace(code)) continue;
                    parts.Add(code);
                }
            }

            if (parts.Count == 0)
            {
                BeepError();
                FocusScan();
                return;
            }

            bool scannerMode = IsScannerMode(parts);

            int ok = 0;
            int fail = 0;
            string firstFailedCode = null;

            bool scannerSoftErrorShown = false;

            foreach (var code in parts)
            {
                if (ShouldIgnoreDuplicateAutoScan(code, scannerMode, source))
                    continue;

                BarcodeResolveResult resolved = null;
                try
                {
                    if (_barcodeEngine != null)
                    {
                        resolved = _barcodeEngine.Resolve(new BarcodeResolveRequest
                        {
                            RawCode = code,
                            UserName = _fullName,
                            Terminal = Environment.MachineName
                        });
                    }
                }
                catch
                {
                    resolved = null;
                }

                if (resolved != null && resolved.Status == BarcodeResolveStatus.Blocked)
                {
                    fail++;
                    if (firstFailedCode == null)
                        firstFailedCode = code;

                    if (scannerMode)
                    {
                        if (!scannerSoftErrorShown)
                        {
                            scannerSoftErrorShown = true;
                            ShowSoftErrorScanner(string.IsNullOrWhiteSpace(resolved.Message) ? "باركود غير صالح" : resolved.Message);
                        }
                    }

                    continue;
                }

                ProductUnitLookup pu = null;
                decimal qty = 1m;

                if (resolved != null && resolved.Status == BarcodeResolveStatus.Resolved && resolved.Item != null)
                {
                    qty = resolved.Item.Qty <= 0m ? 1m : resolved.Item.Qty;
                    if (!_productUnitByProductUnitId.TryGetValue(resolved.Item.ProductUnitId, out pu))
                        pu = null;
                }
                else
                {
                    if (!_productUnitByBarcode.TryGetValue(code, out pu))
                        pu = null;
                }

                if (pu == null)
                {
                    fail++;

                    if (firstFailedCode == null)
                        firstFailedCode = code;

                    // Per-code independent handling:
                    // - Scanner/multi-scan: don't show dropdown, only gentle status.
                    // - Manual: show dropdown for first failed code AFTER adding all successful codes.
                    if (scannerMode)
                    {
                        if (!scannerSoftErrorShown)
                        {
                            scannerSoftErrorShown = true;
                            ShowSoftErrorScanner("لم يتم العثور على المنتج");
                        }
                    }

                    continue;
                }

                AddOrIncrementLine(pu, qty);
                ok++;
            }

            if (ok > 0)
            {
                RecalcTotals();
                BeepOk();
                if (fail > 0)
                    ShowInlineAlert($"تمت إضافة {ok} - فشل {fail}", AlertType.Warning);
            }
            else
            {
                if (scannerMode)
                {
                    BeepError();
                }
            }

            // Manual mode: after processing ALL codes, show dropdown only for the first failed code.
            if (!scannerMode && !string.IsNullOrWhiteSpace(firstFailedCode))
            {
                ShowSoftErrorManual("لم يتم العثور على المنتج — اختر من القائمة أو استمر بالمسح");
                ShowSuggestionsForInput(firstFailedCode);
                BeepError();
            }

            FocusScan();
        }

        private void DecrementSelectedLine()
        {
            if (_lines.Count == 0)
            {
                ShowInlineAlert("لا توجد سطور", AlertType.Error);
                BeepError();
                return;
            }

            if (dgvLines.CurrentRow == null)
            {
                ShowInlineAlert("اختر سطر أولاً", AlertType.Warning);
                BeepError();
                return;
            }

            var now = DateTime.UtcNow;
            if (_lastQtyAdjustAtUtc != default(DateTime) && (now - _lastQtyAdjustAtUtc).TotalMilliseconds < 80)
                return;
            _lastQtyAdjustAtUtc = now;

            var line = dgvLines.CurrentRow.DataBoundItem as SaleLine;
            if (line == null)
            {
                ShowInlineAlert("سطر غير صالح", AlertType.Error);
                BeepError();
                return;
            }

            int idx = _lines.IndexOf(line);
            if (idx < 0)
                return;

            decimal newQty = line.Qty - 1m;
            if (newQty <= 0m)
            {
                _lines.RemoveAt(idx);
                RecalcTotals();
                ShowInlineAlert("تم حذف السطر", AlertType.Warning);
                BeepOk();
                FocusScan();
                return;
            }

            line.Qty = newQty;
            line.Recalc();
            _lines.ResetItem(idx);
            RecalcTotals();

            try
            {
                dgvLines.ClearSelection();
                if (idx < dgvLines.Rows.Count)
                    dgvLines.Rows[idx].Selected = true;
            }
            catch { }

            ShowInlineAlert("تم إنقاص الكمية", AlertType.Warning);
            BeepOk();
            FocusScan();
        }

        private enum ScanCommitSource
        {
            ScannerAuto = 0,
            UserCommit = 1
        }

        private bool ShouldIgnoreDuplicateAutoScan(string code, bool scannerMode, ScanCommitSource source)
        {
            if (source != ScanCommitSource.ScannerAuto)
                return false;

            if (!scannerMode)
                return false;

            if (string.IsNullOrWhiteSpace(code))
                return false;

            // Adaptive window:
            // - Tied to the current scan timer interval (scanner typically commits via timer).
            // - Clamped to avoid being too strict or too lenient.
            int windowMs;
            try
            {
                windowMs = Math.Max(150, Math.Min(400, _scanTimer.Interval * 2));
            }
            catch
            {
                windowMs = 240;
            }

            var now = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(_lastAutoScannedCode)
                && string.Equals(code, _lastAutoScannedCode, StringComparison.OrdinalIgnoreCase)
                && _lastAutoScanAtUtc != default(DateTime)
                && (now - _lastAutoScanAtUtc).TotalMilliseconds <= windowMs)
            {
                return true;
            }

            _lastAutoScannedCode = code;
            _lastAutoScanAtUtc = now;
            return false;
        }

        private static string NormalizeScanCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return string.Empty;

            // Trim common non-printable/control chars that some scanners append.
            string s = code.Trim();
            s = s.Trim('\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\u0007', '\u0008', '\t', '\n', '\r');
            return s.Trim();
        }

        private void lstSuggestions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null) return;
            if (!lstSuggestions.Visible) return;

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AcceptSuggestionFromList();
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                HideSuggestions();
                txtScan.Focus();
                return;
            }
        }

        private void AcceptSuggestionFromList()
        {
            try
            {
                if (!lstSuggestions.Visible) return;
                var item = lstSuggestions.SelectedItem as SuggestionItem;
                if (item == null || item.ProductUnit == null)
                {
                    HideSuggestions();
                    return;
                }

                HideSuggestions();

                try { txtScan.Clear(); } catch { }

                AddOrIncrementLine(item.ProductUnit, 1m);
                RecalcTotals();
                ShowInlineAlert("تمت إضافة المنتج", AlertType.Success);
                BeepOk();
            }
            catch
            {
                HideSuggestions();
            }
            finally
            {
                FocusScan();
            }
        }

        private void ShowSuggestionsForInput(string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    HideSuggestions();
                    return;
                }

                string q = input.Trim();
                bool looksNumeric = q.All(char.IsDigit);

                // Manual failure improvement:
                // allow 1-digit numeric queries (partial barcode/SKU) to show suggestions,
                // while keeping the original minimum length for non-numeric.
                if (q.Length < 2 && !looksNumeric)
                {
                    HideSuggestions();
                    return;
                }

                var items = BuildSuggestions(q, max: 5);
                if (items.Count == 0)
                {
                    HideSuggestions();
                    return;
                }

                PositionSuggestionsUnderScan();

                lstSuggestions.BeginUpdate();
                try
                {
                    lstSuggestions.Items.Clear();
                    foreach (var it in items)
                        lstSuggestions.Items.Add(it);
                    lstSuggestions.SelectedIndex = 0;
                    lstSuggestions.Visible = true;
                    lstSuggestions.BringToFront();
                }
                finally
                {
                    lstSuggestions.EndUpdate();
                }
            }
            catch
            {
                HideSuggestions();
            }
        }

        private void HideSuggestions()
        {
            try
            {
                lstSuggestions.Visible = false;
                lstSuggestions.Items.Clear();
            }
            catch { }
        }

        private void PositionSuggestionsUnderScan()
        {
            try
            {
                // Place the dropdown directly under txtScan
                var parent = splitMain.Panel1;
                var scanScreen = txtScan.Parent.PointToScreen(txtScan.Location);
                var scanClient = parent.PointToClient(scanScreen);

                int x = scanClient.X;
                int y = scanClient.Y + txtScan.Height;

                int width = txtScan.Width;
                int height = 140;

                lstSuggestions.SetBounds(x, y, width, height);
            }
            catch { }
        }

        private bool IsScannerMode(System.Collections.Generic.List<string> parts)
        {
            // Multiple codes at once usually means scanner/paste.
            if (parts != null && parts.Count > 1) return true;
            return DateTime.UtcNow <= _scannerModeUntilUtc;
        }

        private void ShowSoftErrorManual(string msg)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(msg)) return;
                lblAlert.Text = msg;
                lblAlert.BackColor = Color.Firebrick;
                lblAlert.ForeColor = Color.White;
                _statusResetTimer.Stop();
                _statusResetTimer.Start();
            }
            catch { }
        }

        private void ShowSoftErrorScanner(string msg)
        {
            // Cooldown to avoid repeated spam in scanner mode
            var now = DateTime.UtcNow;
            if (_lastScannerSoftErrorAtUtc != default(DateTime))
            {
                if ((now - _lastScannerSoftErrorAtUtc).TotalMilliseconds < 700)
                    return;
            }
            _lastScannerSoftErrorAtUtc = now;
            ShowSoftErrorManual(msg);
        }

        private System.Collections.Generic.List<SuggestionItem> BuildSuggestions(string q, int max)
        {
            var list = new System.Collections.Generic.List<SuggestionItem>();
            if (max <= 0) return list;
            if (string.IsNullOrWhiteSpace(q)) return list;

            q = q.Trim();

            bool looksNumeric = q.All(char.IsDigit);

            // Ensure we don't create too many candidates.
            int barcodeScanLimit = 4000;
            int nameScanLimit = 2500;
            int seen = 0;

#if DEBUG
            bool diagEnabled = true;
            int diagPrefixSeenInScan = 0;
            int diagContainsCollected = 0;
            bool diagPass2Merged = false;
            int diagEnumerated = 0;

            int? diagDbPrefixCount = null;
            try
            {
                if (diagEnabled)
                {
                    string cs = global::Sales.Database.DatabaseInitializer.ConnectionString;
                    using (var con = new global::System.Data.SQLite.SQLiteConnection(cs))
                    {
                        con.Open();
                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT COUNT(*) FROM ProductUnitBarcodes WHERE barcode LIKE @p";
                            cmd.Parameters.AddWithValue("@p", q + "%");
                            object v = cmd.ExecuteScalar();
                            if (v != null && v != DBNull.Value)
                                diagDbPrefixCount = Convert.ToInt32(v);
                        }
                    }
                }
            }
            catch
            {
                // Diagnostics must never impact runtime behavior.
                diagDbPrefixCount = null;
            }
#endif

            // 1) Barcode matching (Exact / StartsWith / Contains)
            if (_productUnitByBarcode != null)
            {
                // Pass 1: prefix only (Exact / StartsWith)
                // Pass 2: fallback Contains only if prefix results are less than max
                var prefixMatches = new System.Collections.Generic.List<SuggestionItem>();
                var containsMatches = new System.Collections.Generic.List<SuggestionItem>();
                int containsCap = Math.Max(25, max * 40);

                foreach (var kv in _productUnitByBarcode)
                {
                    if (seen++ > barcodeScanLimit) break;
#if DEBUG
                    diagEnumerated = seen;
#endif
                    string barcode = kv.Key;
                    var pu = kv.Value;
                    if (pu == null) continue;
                    if (string.IsNullOrWhiteSpace(barcode)) continue;

                    // Pass 1: Exact / StartsWith only
                    if (string.Equals(barcode, q, StringComparison.OrdinalIgnoreCase))
                    {
                        prefixMatches.Add(new SuggestionItem(pu, barcode, 0, isBarcode: true));
                        continue;
                    }
                    if (barcode.StartsWith(q, StringComparison.OrdinalIgnoreCase))
                    {
                        prefixMatches.Add(new SuggestionItem(pu, barcode, 1, isBarcode: true));
                        continue;
                    }

                    // Pass 2 candidate: Contains (collected but not merged unless needed)
                    if (prefixMatches.Count < max && containsMatches.Count < containsCap
                        && barcode.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        containsMatches.Add(new SuggestionItem(pu, barcode, 2, isBarcode: true));
                    }
                }

#if DEBUG
                if (diagEnabled)
                {
                    diagPrefixSeenInScan = prefixMatches.Count;
                    diagContainsCollected = containsMatches.Count;
                }
#endif

                list.AddRange(prefixMatches);
                if (list.Count < max && containsMatches.Count > 0)
                {
#if DEBUG
                    if (diagEnabled)
                        diagPass2Merged = true;
#endif
                    list.AddRange(containsMatches);
                }
            }

#if DEBUG
            if (diagEnabled)
            {
                if (diagDbPrefixCount.HasValue)
                {
                    if (diagDbPrefixCount.Value > 0 && diagPrefixSeenInScan == 0)
                    {
                        global::System.Diagnostics.Debug.WriteLine(
                            $"[POS][BuildSuggestions][PrefixHidden] q='{q}' max={max} dbPrefix={diagDbPrefixCount.Value} pass1PrefixSeen=0 enumerated={diagEnumerated}/{barcodeScanLimit} pass2Merged={diagPass2Merged} containsCollected={diagContainsCollected}");
                    }
                    else if (diagPass2Merged && diagDbPrefixCount.Value > 0 && diagPrefixSeenInScan < max)
                    {
                        global::System.Diagnostics.Debug.WriteLine(
                            $"[POS][BuildSuggestions][Pass2WhilePrefixExists] q='{q}' max={max} dbPrefix={diagDbPrefixCount.Value} pass1PrefixSeen={diagPrefixSeenInScan} enumerated={diagEnumerated}/{barcodeScanLimit} containsCollected={diagContainsCollected}");
                    }
                }
            }
#endif

            // 2) Name matching (StartsWith / Contains)
            if (!looksNumeric && _productUnitByProductUnitId != null)
            {
                int nameSeen = 0;
                foreach (var pu in _productUnitByProductUnitId.Values)
                {
                    if (nameSeen++ > nameScanLimit) break;
                    if (pu == null) continue;
                    string name = pu.ProductName ?? string.Empty;
                    int rank = RankName(name, q);
                    if (rank < 0) continue;

                    list.Add(new SuggestionItem(pu, name, rank, isBarcode: false));
                }
            }

            // De-dupe by ProductUnitId keeping best rank.
            var best = list
                .GroupBy(x => x.ProductUnit.ProductUnitId)
                .Select(g => g.OrderBy(x => x.Rank)
                              .ThenByDescending(x => x.IsDefaultBarcode)
                              .ThenByDescending(x => x.UsageScore)
                              .ThenBy(x => x.MatchTextLength)
                              .First())
                .OrderBy(x => x.Rank)
                .ThenByDescending(x => x.IsDefaultBarcode)
                .ThenByDescending(x => x.UsageScore)
                .ThenBy(x => x.MatchTextLength)
                .ThenBy(x => x.DisplayTextLength)
                .Take(max)
                .ToList();

            return best;
        }

        private static int RankBarcode(string barcode, string q)
        {
            if (string.IsNullOrWhiteSpace(barcode) || string.IsNullOrWhiteSpace(q)) return -1;

            if (string.Equals(barcode, q, StringComparison.OrdinalIgnoreCase)) return 0;
            if (barcode.StartsWith(q, StringComparison.OrdinalIgnoreCase)) return 1;

            // For very short numeric queries, 'Contains' creates too many noisy matches.
            // Keep support for partial barcode/SKU via StartsWith.
            if (q.Length < 3 && q.All(char.IsDigit)) return -1;

            if (barcode.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) return 2;
            return -1;
        }

        private static int RankName(string name, string q)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(q)) return -1;

            if (name.StartsWith(q, StringComparison.OrdinalIgnoreCase)) return 1;
            if (name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) return 2;
            return -1;
        }

        private sealed class SuggestionItem
        {
            public ProductUnitLookup ProductUnit { get; private set; }
            public int Rank { get; private set; }
            public int MatchTextLength { get; private set; }
            public int DisplayTextLength { get; private set; }
            public bool IsDefaultBarcode { get; private set; }
            public int UsageScore { get; private set; }

            private readonly string _display;

            public SuggestionItem(ProductUnitLookup pu, string matchText, int rank, bool isBarcode)
            {
                ProductUnit = pu;
                Rank = rank;
                IsDefaultBarcode = pu != null && pu.IsDefaultBarcode;
                UsageScore = 0; // reserved for future usage-based ranking

                string pn = (pu.ProductName ?? string.Empty).Trim();
                string un = (pu.UnitName ?? string.Empty).Trim();
                string bc = (pu.Barcode ?? string.Empty).Trim();

                string match = (matchText ?? string.Empty).Trim();
                MatchTextLength = match.Length;

                // Display: Product (Unit) - Barcode
                _display = pn + " (" + un + ")" + (string.IsNullOrWhiteSpace(bc) ? string.Empty : " - " + bc);
                DisplayTextLength = _display.Length;
            }

            public override string ToString()
            {
                return _display;
            }
        }

        private void ReloadBarcodeCacheOrShowError()
        {
            try
            {
                _productUnitByBarcode = _productRepo.GetAllProductUnitsByBarcodeDictionary();
                _cacheLoadedAtUtc = DateTime.UtcNow;

                RebuildProductUnitIdCache();

                int count = _productUnitByBarcode == null ? 0 : _productUnitByBarcode.Count;
                ShowInlineAlert($"تم تحديث الباركودات ({count})", AlertType.Success);
            }
            catch (Exception ex)
            {
                ShowInlineAlert("فشل تحميل الباركودات: " + ex.Message, AlertType.Error);
                BeepError();
            }
        }

        private void RebuildProductUnitIdCache()
        {
            var dict = new Dictionary<int, ProductUnitLookup>();
            if (_productUnitByBarcode != null)
            {
                foreach (var kv in _productUnitByBarcode)
                {
                    var pu = kv.Value;
                    if (pu == null) continue;
                    if (pu.ProductUnitId <= 0) continue;
                    if (!dict.ContainsKey(pu.ProductUnitId))
                        dict[pu.ProductUnitId] = pu;
                }
            }

            _productUnitByProductUnitId = dict;
        }

        private void ReloadHotkeysOrShowError()
        {
            try
            {
                _hotkeysByKeyCode = _hotkeysRepo.GetHotkeysDictionary();
            }
            catch (Exception ex)
            {
                ShowInlineAlert("فشل تحميل اختصارات POS: " + ex.Message, AlertType.Error);
            }
        }

        private void ApplyHotkey(PosHotkeysRepository.HotkeyEntry hk)
        {
            if (hk == null) return;
            if (hk.ProductUnitId <= 0)
            {
                BeepError();
                return;
            }

            if (!_productUnitByProductUnitId.TryGetValue(hk.ProductUnitId, out var pu) || pu == null)
            {
                ShowInlineAlert("الوحدة غير موجودة في كاش الباركود (اضغط F5)", AlertType.Warning);
                BeepError();
                return;
            }

            decimal qty = hk.QtyDelta <= 0 ? 1m : hk.QtyDelta;
            AddOrIncrementLine(pu, qty);
            RecalcTotals();
            BeepOk();
        }

        private void AddOrIncrementLine(ProductUnitLookup pu, decimal qtyDelta)
        {
            if (pu == null) return;

            var existing = _lines.FirstOrDefault(x => x.ProductUnitId == pu.ProductUnitId);
            if (existing != null)
            {
                existing.Qty += qtyDelta;
                existing.Recalc();

                // Force grid refresh for this row.
                int idx = _lines.IndexOf(existing);
                if (idx >= 0)
                {
                    _lines.ResetItem(idx);
                    dgvLines.ClearSelection();
                    if (idx < dgvLines.Rows.Count)
                        dgvLines.Rows[idx].Selected = true;
                }

                ShowInlineAlert("تمت زيادة الكمية", AlertType.Success);
                return;
            }

            var line = new SaleLine
            {
                ProductId = pu.ProductId,
                ProductUnitId = pu.ProductUnitId,
                ScanCode = pu.Barcode,
                ProductName = pu.ProductName,
                UnitName = pu.UnitName,
                Factor = pu.Factor,
                Qty = qtyDelta,
                Price = pu.SellPrice,
                CostPrice = pu.CostPrice,
                Discount = 0m
            };
            line.Recalc();
            _lines.Add(line);

            int newIdx = _lines.Count - 1;
            dgvLines.ClearSelection();
            if (newIdx >= 0 && newIdx < dgvLines.Rows.Count)
                dgvLines.Rows[newIdx].Selected = true;

            ShowInlineAlert("تمت إضافة المنتج", AlertType.Success);
        }

        private void RemoveSelectedLine()
        {
            if (_lines.Count == 0) return;
            if (dgvLines.CurrentRow == null) return;

            var line = dgvLines.CurrentRow.DataBoundItem as SaleLine;
            if (line == null) return;

            int idx = _lines.IndexOf(line);
            if (idx >= 0)
            {
                _lines.RemoveAt(idx);
                RecalcTotals();
                ShowInlineAlert("تم حذف السطر", AlertType.Warning);
                BeepOk();
            }

            FocusScan();
        }

        private void ClearCart()
        {
            if (_lines.Count == 0)
            {
                FocusScan();
                return;
            }

            if (!MessageHelper.AskForConfirmation("مسح السلة بالكامل؟"))
            {
                FocusScan();
                return;
            }

            _lines.Clear();
            ResetDraft();
            _draftRequestId = null;
            ClearSelectedCustomer();
            ShowInlineAlert("تم مسح السلة", AlertType.Warning);
            FocusScan();
        }

        private void ResetDraft()
        {
            lblStatusValue.Text = "DRAFT";
            txtSubtotal.Text = "0";
            txtDiscountTotal.Text = "0";
            txtNetTotal.Text = "0";
            txtChange.Text = "0";
            txtPaid.Text = "0";
            ClearSelectedCustomer();
            RefreshRecentOrders();

            FocusScan();
        }

        private void FocusScan()
        {
            try
            {
                txtScan.Focus();
                txtScan.SelectAll();
            }
            catch { }
        }

        private void RecalcTotals()
        {
            decimal subtotal = 0m;
            foreach (var l in _lines)
            {
                if (l == null) continue;
                l.Recalc();
                subtotal += l.LineTotal;
            }

            txtSubtotal.Text = SalesNumberFormat.FormatPrice(subtotal);

            decimal discount = 0m;
            SalesNumberFormat.TryParsePrice((txtDiscountTotal.Text ?? "0"), out discount);
            if (discount < 0) discount = 0;
            if (discount > subtotal) discount = subtotal;

            decimal net = subtotal - discount;
            var pricing = OrderPricingHelper.Compute(subtotal, discount);
            string taxLine = OrderPricingHelper.FormatTaxLine(pricing);
            txtNetTotal.Text = SalesNumberFormat.FormatPrice(pricing.GrandTotal);
            if (!string.IsNullOrEmpty(taxLine))
            {
                try
                {
                    if (lblAlert != null && lblAlert.Text != null && lblAlert.Text.IndexOf("ضريبة", StringComparison.Ordinal) < 0)
                        lblAlert.Text = taxLine;
                }
                catch { }
            }

            net = pricing.GrandTotal;

            // FULL PAYMENT ONLY: paid must equal net.
            try
            {
                txtPaid.Text = SalesNumberFormat.FormatPrice(net);
            }
            catch { }

            txtChange.Text = "0";
        }

        private void PostOrder()
        {
            if (!EnsureCanPost())
            {
                BeepError();
                FocusScan();
                return;
            }

            try
            {
                RecalcTotals();

                if (string.IsNullOrWhiteSpace(_draftRequestId))
                    _draftRequestId = Guid.NewGuid().ToString();

                decimal net = 0m;
                SalesNumberFormat.TryParsePrice((txtNetTotal.Text ?? "0"), out net);

                decimal paid = 0m;
                SalesNumberFormat.TryParsePrice((txtPaid.Text ?? "0"), out paid);

                if (paid != net)
                {
                    ShowInlineAlert("يجب أن يكون المدفوع = الصافي", AlertType.Error);
                    BeepError();
                    return;
                }

                decimal discountPos = 0m;
                SalesNumberFormat.TryParsePrice((txtDiscountTotal.Text ?? "0"), out discountPos);
                if (discountPos < 0m) discountPos = 0m;

                decimal linesSub = _lines.Where(l => l != null).Sum(l => l.LineTotal);
                var pricing = OrderPricingHelper.Compute(linesSub, discountPos);

                var details = _lines.Select(l => new OrderDetail
                {
                    ProductId = l.ProductId,
                    ProductName = l.ProductName,
                    ProductNameSnapshot = l.ProductName,
                    Price = l.Price,
                    SellPriceSnapshot = l.Price,
                    Total = l.LineTotal,
                    BarcodeSnapshot = l.ScanCode,
                    ProductUnitId = l.ProductUnitId,
                    UnitNameSnapshot = l.UnitName,
                    FactorSnapshot = l.Factor,
                    QtyUnit = l.Qty,
                    BaseQty = l.BaseQty,
                    CostPrice = l.CostPrice
                }).ToList();

                var printLines = _lines.ToList();

                var order = new Order
                {
                    Id = 0,
                    RequestId = _draftRequestId,
                    OrderDate = DateTime.Now,
                    CustomerId = _selectedCustomerId,
                    Note = BuildPosInvoiceNote(pricing),
                    Discount = pricing.Discount,
                    TaxAmount = pricing.TaxAmount,
                    Total = pricing.GrandTotal,
                    CreatedBy = _fullName,
                    Details = details
                };

                _orderRepo.SaveOrder(order, paidAmount: pricing.GrandTotal);

                lblStatusValue.Text = "POSTED";
                ShowInlineAlert("تم حفظ الفاتورة", AlertType.Success);
                BeepOk();

                try
                {
                    string store = AppSettingsManager.GetString(AppSettingsManager.Keys.StoreName, "Sales");
                    PosInvoicePrinter.PrintOrder(order.Id, order, printLines, store);
                }
                catch { }

                _lines.Clear();
                ResetDraft();
                _draftRequestId = null;
                RefreshRecentOrders();
            }
            catch (Exception ex)
            {
                ShowInlineAlert("فشل الحفظ: " + ex.Message, AlertType.Error);
                BeepError();
            }
            finally
            {
                FocusScan();
            }
        }

        private bool EnsureCanPost()
        {
            if (_lines.Count == 0)
            {
                ShowInlineAlert("لا توجد سطور", AlertType.Error);
                return false;
            }

            foreach (var l in _lines)
            {
                if (l == null) continue;
                if (l.ProductId <= 0)
                {
                    ShowInlineAlert("سطر منتج غير صالح", AlertType.Error);
                    return false;
                }
                if (l.Qty <= 0)
                {
                    ShowInlineAlert("الكمية يجب أن تكون أكبر من صفر", AlertType.Error);
                    return false;
                }
                if (l.Price <= 0)
                {
                    ShowInlineAlert("السعر غير صالح", AlertType.Error);
                    return false;
                }
            }

            return true;
        }

        private enum AlertType { Success, Warning, Error }

        private void ShowInlineAlert(string msg, AlertType type)
        {
            if (string.IsNullOrWhiteSpace(msg)) return;

            lblAlert.Text = msg;
            switch (type)
            {
                case AlertType.Success:
                    lblAlert.BackColor = Color.DarkSeaGreen;
                    lblAlert.ForeColor = Color.Black;
                    break;
                case AlertType.Warning:
                    lblAlert.BackColor = Color.Gold;
                    lblAlert.ForeColor = Color.Black;
                    break;
                default:
                    lblAlert.BackColor = Color.Firebrick;
                    lblAlert.ForeColor = Color.White;
                    break;
            }
        }

        private static void BeepOk()
        {
            try { SystemSounds.Asterisk.Play(); } catch { }
        }

        private static void BeepError()
        {
            try { SystemSounds.Hand.Play(); } catch { }
        }

        private static string BuildPosInvoiceNote(OrderPricingHelper.InvoiceTotals pricing)
        {
            string note = "POS";
            string taxLine = OrderPricingHelper.FormatTaxLine(pricing);
            if (!string.IsNullOrWhiteSpace(taxLine))
                note += " | " + taxLine;
            return note;
        }

    }
}
