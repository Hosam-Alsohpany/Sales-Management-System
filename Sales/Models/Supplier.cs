using System;

namespace Sales.Models
{
    /// <summary>
    /// نموذج يمثل مورد في النظام
    /// يحتوي على معلومات المورد الأساسية وبيانات الاتصال
    /// </summary>
    public class Supplier
    {
        /// <summary>
        /// معرف المورد الفريد (Primary Key)
        /// يتم توليده تلقائياً من قاعدة البيانات
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// اسم المورد (مطلوب)
        /// يظهر في القوائم والبحث والفواتير
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// رقم هاتف المورد
        /// يستخدم للتواصل المباشر
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// عنوان المورد
        /// يستخدم للمراسلات والتوصيل
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// تفاصيل المورد
        /// ملاحظات أو معلومات إضافية (اختياري)
        /// </summary>
        public string Details { get; set; }
    }
}
