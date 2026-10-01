using System;
using System.Windows.Forms;

namespace Sales.Utilities
{
    /// <summary>
    /// حماية شاشات الإعدادات الحساسة (مدير فقط).
    /// </summary>
    public static class AdminFormAccess
    {
        /// <summary>
        /// يُستدعى عند تحميل النموذج؛ يُغلق النموذج إذا لم يكن المستخدم مديراً.
        /// </summary>
        public static bool EnsureAdminOnLoad(Form form, string screenTitleArabic)
        {
            if (form == null) return false;
            if (SessionManager.IsAdmin) return true;

            try
            {
                MessageBox.Show(
                    "شاشة «" + (screenTitleArabic ?? "الإعدادات") + "» متاحة للمدير فقط.",
                    "صلاحيات",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch
            {
            }

            try
            {
                form.BeginInvoke(new Action(() =>
                {
                    try { form.Close(); } catch { }
                }));
            }
            catch
            {
            }

            return false;
        }
    }
}
