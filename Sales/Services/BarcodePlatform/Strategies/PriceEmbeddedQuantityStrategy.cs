using System.Globalization;
using Sales.Models;

namespace Sales.Services.BarcodePlatform.Strategies
{
    public class PriceEmbeddedQuantityStrategy : IQuantityResolverStrategy
    {
        public QuantityResolveResult TryResolve(BarcodeParseResult parsed, ProductUnitLookup productUnit)
        {
            if (parsed == null || parsed.WeightEan13 == null)
                return new QuantityResolveResult { Qty = 0m, Message = "باركود غير صالح" };

            if (productUnit == null || productUnit.ProductUnitId <= 0)
                return new QuantityResolveResult { Qty = 0m, Message = "لم يتم العثور على الصنف" };

            if (productUnit.SellPrice <= 0m)
                return new QuantityResolveResult { Qty = 0m, Message = "سعر الصنف غير صالح" };

            int decimals = parsed.WeightEan13.PriceDecimals < 0 ? 0 : parsed.WeightEan13.PriceDecimals;
            decimal price = ParseFixedPoint(parsed.WeightEan13.PriceRaw, decimals);
            if (price <= 0m)
                return new QuantityResolveResult { Qty = 0m, Message = "قيمة السعر غير صالحة" };

            decimal qty = price / productUnit.SellPrice;
            if (qty <= 0m)
                return new QuantityResolveResult { Qty = 0m, Message = "قيمة الكمية غير صالحة" };

            qty = decimal.Round(qty, 3, System.MidpointRounding.AwayFromZero);
            return new QuantityResolveResult { Qty = qty };
        }

        private static decimal ParseFixedPoint(string digits, int decimals)
        {
            if (string.IsNullOrWhiteSpace(digits)) return 0m;
            if (!int.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v)) return 0m;
            if (decimals <= 0) return v;

            decimal scale = 1m;
            for (int i = 0; i < decimals; i++) scale *= 10m;
            return v / scale;
        }
    }
}
