using System;
using System.Collections.Generic;

namespace Sales.Models
{
    // ============================================
    // نموذج بيانات الفاتورة (Order Model)
    // ============================================
    public class Order
    {
        public int Id { get; set; }
        public string RequestId { get; set; }
        public DateTime OrderDate { get; set; }
        public int? CustomerId { get; set; }   // رقم العميل (يمكن أن يكون فارغاً)
        public string Note { get; set; }
        /// <summary>قيمة الخصم على الفاتورة (يُخزَّن في Orders.discount).</summary>
        public decimal Discount { get; set; }
        /// <summary>قيمة الضريبة المحسوبة على الفاتورة.</summary>
        public decimal TaxAmount { get; set; }
        /// <summary>الصافي بعد الخصم والضريبة (يُخزَّن في Orders.total).</summary>
        public decimal Total { get; set; }
        public string CreatedBy { get; set; }

        // قائمة بتفاصيل الفاتورة (المنتجات المباعة)
        public List<OrderDetail> Details { get; set; } = new List<OrderDetail>();
    }

    // ============================================
    // نموذج تفاصيل الفاتورة (Order Detail Model)
    // ============================================
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } // للاستخدام في العرض فقط
        // Qty: الكمية بوحدة المخزون الأساسية (BaseQty) للحفاظ على التوافق مع الجداول القديمة
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }

        // ===== Unit-based sales (new) =====
        public int? ProductUnitId { get; set; }
        public string UnitNameSnapshot { get; set; }
        public decimal? FactorSnapshot { get; set; }

        // qty_unit: الكمية كما أدخلها المستخدم (بوحدة البيع المختارة)
        public decimal? QtyUnit { get; set; }
        // base_qty: الكمية بعد التحويل للوحدة الأساسية (QtyUnit * Factor)
        public decimal? BaseQty { get; set; }
        public decimal? CostPrice { get; set; }

        public string ProductNameSnapshot { get; set; }
        public string BarcodeSnapshot { get; set; }
        public decimal? SellPriceSnapshot { get; set; }
    }
}
