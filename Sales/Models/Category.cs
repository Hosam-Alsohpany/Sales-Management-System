using System;

namespace Sales.Models
{
    // ============================================
    // نموذج بيانات الصنف (Category Model)
    // يمثل صف واحد من جدول Categories
    // ============================================
    public class Category
    {
        // رقم الصنف (المفتاح الأساسي)
        public int Id { get; set; }// رقم الصنف (مثل الرقم التسلسلي على العلبة)

        // اسم الصنف (مثل: مشروبات، إلكترونيات)
        public string Name { get; set; }

        // دالة لإرجاع اسم الصنف عند عرضه في ComboBox
        public override string ToString()
        {
            return Name;
        }
    }
}
