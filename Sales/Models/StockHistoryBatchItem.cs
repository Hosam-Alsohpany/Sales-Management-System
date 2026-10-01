// ============================================================
// الملف    : StockHistoryBatchItem.cs
// الغرض    : نموذج عنصر فردي في حزمة حركات المخزون
// ============================================================

namespace Sales.Models
{
    public class StockHistoryBatchItem
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? QtyBefore { get; set; }
        public int? QtyAfter { get; set; }
        public int QtyChange { get; set; }
    }
}
