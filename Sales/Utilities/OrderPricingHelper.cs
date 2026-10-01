using System;

namespace Sales.Utilities
{
    /// <summary>
    /// حساب إجماليات الفاتورة مع الضريبة الافتراضية من الإعدادات.
    /// </summary>
    public static class OrderPricingHelper
    {
        public sealed class InvoiceTotals
        {
            public decimal Subtotal { get; set; }
            public decimal Discount { get; set; }
            public decimal TaxableAmount { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal GrandTotal { get; set; }
            public bool TaxEnabled { get; set; }
            public decimal TaxRatePercent { get; set; }
        }

        public static InvoiceTotals Compute(decimal linesSubtotal, decimal discount)
        {
            if (linesSubtotal < 0m) linesSubtotal = 0m;
            if (discount < 0m) discount = 0m;
            if (discount > linesSubtotal) discount = linesSubtotal;

            bool taxEnabled = AppSettingsManager.GetBool(AppSettingsManager.Keys.TaxEnabled, false);
            decimal rate = AppSettingsManager.GetDecimal(AppSettingsManager.Keys.DefaultTax, 0m);
            if (rate < 0m) rate = 0m;

            decimal taxable = linesSubtotal - discount;
            decimal tax = 0m;
            if (taxEnabled && rate > 0m)
                tax = Math.Round(taxable * rate / 100m, 2, MidpointRounding.AwayFromZero);

            return new InvoiceTotals
            {
                Subtotal = linesSubtotal,
                Discount = discount,
                TaxableAmount = taxable,
                TaxAmount = tax,
                GrandTotal = taxable + tax,
                TaxEnabled = taxEnabled,
                TaxRatePercent = rate
            };
        }

        public static string FormatTaxLine(InvoiceTotals t)
        {
            if (t == null || !t.TaxEnabled || t.TaxAmount <= 0m)
                return string.Empty;
            return "ضريبة (" + t.TaxRatePercent.ToString("0.##") + "%): " + SalesNumberFormat.FormatPrice(t.TaxAmount);
        }
    }
}
