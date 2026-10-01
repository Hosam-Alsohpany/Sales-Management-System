// ============================================================
// الملف    : StockHistory.cs
// الغرض    : نموذج حركة المخزون لتسجيل عمليات الصرف والتوريد وتاريخها
// ============================================================

using System;

namespace Sales.Models
{
    public class StockHistory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int QtyChange { get; set; } // + or -
        public string Reason { get; set; } // sale, purchase, adjustment
        public DateTime ChangeDate { get; set; }
        public string UserName { get; set; }
    }
}
