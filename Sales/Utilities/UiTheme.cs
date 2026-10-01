using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sales.Utilities
{
    internal static class UiTheme
    {
        public static readonly Font DefaultFont = new Font("Tahoma", 10F, FontStyle.Regular);
        public static readonly Font HeaderFont = new Font("Tahoma", 11F, FontStyle.Bold);

        public static readonly Color FormBackColor = Color.White;
        public static readonly Color PanelBackColor = Color.Snow;
        public static readonly Color GridAltRowColor = Color.FromArgb(245, 245, 245);
        public static readonly Color GridHeaderBackColor = Color.FromArgb(235, 235, 235);
        public static readonly Color GridHeaderForeColor = Color.Black;

        public static void ApplyToForm(Form form)
        {
            if (form == null) return;

            try
            {
                form.SuspendLayout();

                try { form.Font = DefaultFont; } catch { }
                try { form.BackColor = FormBackColor; } catch { }
                try { form.RightToLeft = RightToLeft.Yes; } catch { }
                try { form.RightToLeftLayout = true; } catch { }

                ApplyToControlsRecursive(form);
            }
            finally
            {
                try { form.ResumeLayout(true); } catch { }
            }
        }

        private static void ApplyToControlsRecursive(Control root)
        {
            if (root == null) return;

            foreach (Control c in root.Controls)
            {
                if (c == null) continue;

                try
                {
                    // Keep explicit fonts if the designer set them intentionally on a specific control.
                    // However, for most controls it is fine to inherit from the form.
                    if (c is GroupBox)
                    {
                        try { c.Font = HeaderFont; } catch { }
                    }

                    if (c is Panel || c is GroupBox)
                    {
                        try
                        {
                            if (c.BackColor == Color.Transparent)
                                c.BackColor = PanelBackColor;
                        }
                        catch { }
                    }

                    if (c is ToolStrip strip)
                    {
                        ApplyToolStripStyle(strip);
                    }

                    if (c is DataGridView dgv)
                    {
                        ApplyGridStyle(dgv);
                    }

                    if (c is TextBox tb)
                    {
                        try { tb.RightToLeft = RightToLeft.No; } catch { }
                        NumericTextBoxHelper.ApplyToTextBox(tb);
                    }
                    else if (c is MaskedTextBox mtb)
                    {
                        try { mtb.RightToLeft = RightToLeft.No; } catch { }
                        NumericTextBoxHelper.ApplyToMaskedTextBox(mtb);
                    }
                    else if (c is NumericUpDown nud)
                    {
                        try { nud.RightToLeft = RightToLeft.No; } catch { }
                    }

                    // RTL للنصوص العربية فقط — الأرقام تبقى LTR
                    try
                    {
                        if (!(c is DataGridView) && !(c is TextBoxBase) && !(c is NumericUpDown))
                        {
                            if (c.RightToLeft == RightToLeft.Inherit)
                                c.RightToLeft = RightToLeft.Yes;
                        }
                    }
                    catch { }
                }
                catch { }

                if (c.HasChildren)
                    ApplyToControlsRecursive(c);
            }
        }

        public static void ApplyGridStyle(DataGridView dgv)
        {
            if (dgv == null) return;

            try
            {
                dgv.SuspendLayout();

                try { dgv.EnableHeadersVisualStyles = false; } catch { }
                try { dgv.BackgroundColor = Color.White; } catch { }
                try { dgv.BorderStyle = BorderStyle.FixedSingle; } catch { }

                try { dgv.AllowUserToResizeRows = false; } catch { }
                try { dgv.RowHeadersVisible = false; } catch { }
                try { dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; } catch { }
                try { dgv.MultiSelect = false; } catch { }

                try { dgv.AlternatingRowsDefaultCellStyle.BackColor = GridAltRowColor; } catch { }

                try
                {
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = GridHeaderBackColor;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = GridHeaderForeColor;
                    dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgv.ColumnHeadersDefaultCellStyle.Font = HeaderFont;
                }
                catch { }

                try
                {
                    dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgv.DefaultCellStyle.Font = DefaultFont;
                }
                catch { }

                try { DataGridViewNumberFormatter.Apply(dgv); } catch { }

                try
                {
                    foreach (DataGridViewColumn col in dgv.Columns)
                    {
                        if (col == null) continue;
                        try { col.SortMode = DataGridViewColumnSortMode.Automatic; } catch { }
                    }
                }
                catch { }
            }
            finally
            {
                try { dgv.ResumeLayout(true); } catch { }
            }
        }

        public static void ApplyToolStripStyle(ToolStrip strip)
        {
            if (strip == null) return;

            try
            {
                strip.SuspendLayout();
                try { strip.Font = DefaultFont; } catch { }
                try { strip.GripStyle = ToolStripGripStyle.Hidden; } catch { }
                try { strip.RenderMode = ToolStripRenderMode.System; } catch { }
            }
            finally
            {
                try { strip.ResumeLayout(true); } catch { }
            }
        }
    }
}
