// ============================================================
// الملف    : IQuantityResolverStrategy.cs
// الغرض    : واجهة استراتيجية حساب كمية/وزن المنتج بناءً على ترميز الباركود
// ============================================================

using Sales.Models;

namespace Sales.Services.BarcodePlatform
{
    public class QuantityResolveResult
    {
        public decimal Qty { get; set; }
        public string Message { get; set; }
        public string Warning { get; set; }
    }

    public interface IQuantityResolverStrategy
    {
        QuantityResolveResult TryResolve(BarcodeParseResult parsed, ProductUnitLookup productUnit);
    }
}
