// ============================================================
// الملف    : ReasonPromptForm.cs
// الغرض    : شاشة لإدخال وتوثيق سبب تعديل أو حذف الفاتورة
// ============================================================

using System;
using System.Windows.Forms;

namespace Sales.Forms
{
    public partial class ReasonPromptForm : Form
    {
        public string ReasonText => (txtReason.Text ?? string.Empty).Trim();

        public ReasonPromptForm(string title)
        {
            InitializeComponent();

            try
            {
                Text = title ?? string.Empty;
            }
            catch { }
        }
    }
}
