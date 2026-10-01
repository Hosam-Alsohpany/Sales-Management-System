using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sales.Forms
{
    /// <summary>
    /// نافذة "حول البرنامج" - تعرض معلومات أساسية عن نظام إدارة المبيعات
    /// تحتوي على معلومات الإصدار والشركة المطور وحقوق النشر
    /// </summary>
    public partial class AboutBox : Form
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// منشئ النافذة - يقوم بتهيئة المكونات وتعيين الخصائص الأساسية
        /// </summary>
        public AboutBox()
        {
            InitializeComponent();
            pictureBoxLogo.Image = SystemIcons.Information.ToBitmap();
            this.CancelButton = btnOK;
            this.AcceptButton = btnOK;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
