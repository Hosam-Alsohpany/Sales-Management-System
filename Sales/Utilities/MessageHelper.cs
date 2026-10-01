// ============================================================
// الملف    : MessageHelper.cs
// الغرض    : تسهيل وعرض رسائل التنبيه والخطأ والتأكيد باللغة العربية
// ============================================================

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sales.Utilities
{
    public static class MessageHelper
    {
        public static void ShowError(string message, string technicalDetails = null)
        {
            var msg = technicalDetails != null ? (message + "\n\nالتفاصيل التقنية: " + technicalDetails) : message;
            MessageBox.Show(msg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowWarning(string message)
        {
            MessageBox.Show(message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, "معلومة", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowSuccess(string message)
        {
            MessageBox.Show(message, "تم بنجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool AskForConfirmation(string message)
        {
            return MessageBox.Show(message, "تأكيد العملية", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public static void ShowInsufficientStock(string productName, int requested, int available)
        {
            ShowError(
                "الكمية المطلوبة للمنتج '" + productName + "' غير متوفرة في المخزون.",
                "الكمية المطلوبة: " + requested + "\n" +
                "المتاح: " + available + "\n\n" +
                "اقتراح: قلل الكمية المطلوبة أو قم بإضافة مخزون جديد."
            );
        }

        public static void ShowUnauthorized(string actionName = "هذه العملية")
        {
            ShowWarning("ليس لديك الصلاحية الكافية لـ" + actionName + ".\nيرجى الاتصال بمدير النظام.");
        }

        public static void ShowNoPermissionCreateOrders()
        {
            ShowWarning("ليس لديك صلاحية إنشاء فواتير جديدة.\nيرجى الاتصال بمدير النظام.");
        }

        public static void ShowNoPermissionEditOrders()
        {
            ShowWarning("ليس لديك صلاحية تعديل الفواتير المحفوظة.\nيرجى الاتصال بمدير النظام.");
        }

        public static void ShowNoPermissionDeleteOrders()
        {
            ShowWarning("ليس لديك صلاحية حذف الفواتير.\nيرجى الاتصال بمدير النظام.");
        }

        public static void ShowValidationErrors(List<string> errors)
        {
            if (errors == null || errors.Count == 0)
            {
                ShowError("لا يمكن إتمام العملية بسبب أخطاء في التحقق من البيانات");
                return;
            }

            var errorText = string.Join("\n• ", errors.ToArray());
            ShowError("لا يمكن إتمام العملية بسبب الأخطاء التالية:\n\n• " + errorText);
        }
    }
}
