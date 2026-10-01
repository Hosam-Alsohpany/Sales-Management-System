using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sales.Forms
{
    /// <summary>
    /// نافذة "من نحن" - تعرض معلومات عن فريق التطوير ووسائل التواصل
    /// تحتوي على معلومات الشركة والفريق المطور وطرق التواصل
    /// </summary>
    public partial class AboutUsForm : Form
    {
        /// <summary>
        /// منشئ النافذة - يقوم بتهيئة المكونات وتعيين الخصائص الأساسية
        /// </summary>
        public AboutUsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// معالج حدث النقر على زر الإغلاق
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
