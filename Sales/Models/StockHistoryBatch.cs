// ============================================================
// الملف    : StockHistoryBatch.cs
// الغرض    : نموذج حزمة حركات المخزون لتحديث الجرد في معاملة موحدة
// ============================================================

using System;
using System.Collections.Generic;

namespace Sales.Models
{
    public class StockHistoryBatch
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string Reason { get; set; }
        public string Reference { get; set; }
        public string Note { get; set; }
        public List<StockHistoryBatchItem> Items { get; set; } = new List<StockHistoryBatchItem>();
    }
}
