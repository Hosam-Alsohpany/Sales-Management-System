using System;
using System.Globalization;
using System.Windows.Forms;

namespace Sales.Utilities
{
    /// <summary>
    /// تهيئة عالمية: ثقافة أرقام إنجليزية + ربط كل عناصر الواجهة.
    /// </summary>
    public static class SalesUiBootstrap
    {
        private const string WiredMarker = "__SalesUiWired__";
        private static bool _messageFilterAdded;

        public static void Initialize()
        {
            SalesNumberFormat.ApplyApplicationCulture();

            if (!_messageFilterAdded)
            {
                Application.AddMessageFilter(new EnglishDigitInputFilter());
                _messageFilterAdded = true;
            }
        }

        public static void WireForm(Form form)
        {
            if (form == null) return;

            string tag = form.Tag as string ?? string.Empty;
            if (tag.IndexOf(WiredMarker, StringComparison.Ordinal) >= 0) return;

            form.Tag = tag + WiredMarker;

            form.Load += Form_Load;
            form.Activated += Form_Activated;
            form.ControlAdded += Form_ControlAdded;
            form.Shown += Form_Shown;

            try { ApplyToControlTree(form); } catch { }
        }

        private static void Form_Load(object sender, EventArgs e)
        {
            if (sender is Form form)
                ApplyToControlTree(form);
        }

        private static void Form_Shown(object sender, EventArgs e)
        {
            if (sender is Form form)
                ApplyToControlTree(form);
        }

        private static void Form_Activated(object sender, EventArgs e)
        {
            SalesNumberFormat.ApplyToCurrentThread();
        }

        private static void Form_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e?.Control == null) return;
            try { ApplyToControlTree(e.Control); } catch { }
        }

        public static void ApplyToControlTree(Control root)
        {
            if (root == null) return;

            SalesNumberFormat.ApplyToCurrentThread();
            ApplyToControl(root);

            foreach (Control child in root.Controls)
                ApplyToControlTree(child);
        }

        private static void ApplyToControl(Control c)
        {
            if (c == null) return;

            if (c is DataGridView dgv)
            {
                try { DataGridViewNumberFormatter.Apply(dgv); } catch { }
                try { UiTheme.ApplyGridStyle(dgv); } catch { }
                return;
            }

            if (c is TextBox tb)
            {
                try { tb.RightToLeft = RightToLeft.No; } catch { }
                try { NumericTextBoxHelper.ApplyToTextBox(tb); } catch { }
            }
            else if (c is MaskedTextBox mtb)
            {
                try { NumericTextBoxHelper.ApplyToMaskedTextBox(mtb); } catch { }
            }
            else if (c is DateTimePicker dtp)
            {
                try
                {
                    dtp.RightToLeft = RightToLeft.No;
                    dtp.Format = DateTimePickerFormat.Custom;
                    if (string.IsNullOrWhiteSpace(dtp.CustomFormat))
                        dtp.CustomFormat = "dd/MM/yyyy";
                }
                catch { }
            }
            else if (c is ToolStrip strip)
            {
                try
                {
                    foreach (ToolStripItem item in strip.Items)
                        ApplyToToolStripItem(item);
                }
                catch { }
            }

            try { NormalizeDisplayedText(c); } catch { }
        }

        private static void ApplyToToolStripItem(ToolStripItem item)
        {
            if (item == null) return;

            if (item is ToolStripTextBox tsb)
            {
                try { NumericTextBoxHelper.ApplyToTextBox(tsb.TextBox); } catch { }
            }

            try
            {
                string text = item.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    string normalized = SalesNumberFormat.ToEnglishDigits(text);
                    if (!string.Equals(text, normalized, StringComparison.Ordinal))
                        item.Text = normalized;
                }
            }
            catch { }

            if (item is ToolStripDropDownItem dropDown)
            {
                foreach (ToolStripItem child in dropDown.DropDownItems)
                    ApplyToToolStripItem(child);
            }
        }

        private static void NormalizeDisplayedText(Control c)
        {
            if (c == null) return;
            if (c is TextBoxBase) return;

            string text = c.Text;
            if (string.IsNullOrEmpty(text)) return;

            string normalized = SalesNumberFormat.ToEnglishDigits(text);
            if (SalesNumberFormat.LooksLikeNumericText(normalized))
                normalized = SalesNumberFormat.ForDisplay(normalized);

            if (!string.Equals(text, normalized, StringComparison.Ordinal))
                c.Text = normalized;
        }
    }
}
