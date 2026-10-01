using System;
using System.Windows.Forms;

namespace Sales.Utilities
{
    /// <summary>
    /// يحوّل أرقام لوحة المفاتيح/اللصق العربية إلى 0-9.
    /// </summary>
    internal sealed class EnglishDigitInputFilter : IMessageFilter
    {
        private const int WmChar = 0x0102;
        private const int WmUniChar = 0x0109;
        private const int WmPaste = 0x0302;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WmChar || m.Msg == WmUniChar)
            {
                char ch = (char)(m.WParam.ToInt32() & 0xFFFF);
                char? english = SalesNumberFormat.MapDigitCharToEnglish(ch);
                if (!english.HasValue) return false;

                m.WParam = (IntPtr)english.Value;
                return false;
            }

            if (m.Msg == WmPaste)
            {
                try
                {
                    var ctrl = Control.FromHandle(m.HWnd);
                    if (ctrl == null) return false;

                    ctrl.BeginInvoke(new Action(() =>
                    {
                        try { NormalizeControlText(ctrl); } catch { }
                    }));
                }
                catch { }
            }

            return false;
        }

        private static void NormalizeControlText(Control ctrl)
        {
            if (ctrl is TextBoxBase tb)
            {
                string n = SalesNumberFormat.ToEnglishDigits(tb.Text);
                if (!string.Equals(tb.Text, n, StringComparison.Ordinal))
                    tb.Text = n;
                try { tb.RightToLeft = RightToLeft.No; } catch { }
                return;
            }

            if (ctrl is DataGridView dgv && dgv.EditingControl is TextBox edit)
            {
                string n = SalesNumberFormat.ToEnglishDigits(edit.Text);
                if (!string.Equals(edit.Text, n, StringComparison.Ordinal))
                    edit.Text = n;
                try { edit.RightToLeft = RightToLeft.No; } catch { }
            }
        }
    }
}
