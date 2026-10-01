using System;
using System.Data.SQLite;
using System.Globalization;

namespace Sales.Utilities
{
    /// <summary>
    /// تحويلات الكمية بين الوحدة الأساسية (base_qty) والتمثيل المقيّس (qty_scaled).
    /// المصدر الوحيد للحقيقة: qty_scaled + qty_scale_pow10؛ qty (INTEGER) للتوافق فقط.
    /// </summary>
    public static class ProductQuantityHelper
    {
        public static int ClampScalePow10(int pow10)
        {
            if (pow10 < 0) return 0;
            if (pow10 > 6) return 6;
            return pow10;
        }

        public static long ScaleDivisor(int pow10)
        {
            pow10 = ClampScalePow10(pow10);
            long div = 1;
            for (int i = 0; i < pow10; i++)
            {
                if (div > long.MaxValue / 10L) return long.MaxValue;
                div *= 10L;
            }
            return div <= 0 ? 1 : div;
        }

        public static decimal ScaledToDisplay(long qtyScaled, int pow10)
        {
            long div = ScaleDivisor(pow10);
            if (div <= 0) return qtyScaled;
            return qtyScaled / (decimal)div;
        }

        public static long BaseQtyToScaled(decimal baseQty, int pow10)
        {
            pow10 = ClampScalePow10(pow10);
            decimal scale = ScaleDivisor(pow10);
            return Convert.ToInt64(Math.Round(baseQty * scale, 0, MidpointRounding.AwayFromZero));
        }

        public static int LegacyQtyFromBaseQty(decimal baseQty, int pow10)
        {
            if (baseQty <= 0m) return 0;
            pow10 = ClampScalePow10(pow10);
            if (pow10 <= 0)
                return Convert.ToInt32(Math.Round(baseQty, 0, MidpointRounding.AwayFromZero));

            long scaled = BaseQtyToScaled(baseQty, pow10);
            long div = ScaleDivisor(pow10);
            long legacy = div <= 0 ? scaled : scaled / div;
            if (legacy < 0) legacy = 0;
            if (legacy > int.MaxValue) legacy = int.MaxValue;
            return (int)legacy;
        }

        public static int LegacyQtyFromScaled(long qtyScaled, int pow10)
        {
            pow10 = ClampScalePow10(pow10);
            if (pow10 <= 0)
            {
                if (qtyScaled < 0) return 0;
                if (qtyScaled > int.MaxValue) return int.MaxValue;
                return (int)qtyScaled;
            }

            long div = ScaleDivisor(pow10);
            long legacy = div <= 0 ? qtyScaled : qtyScaled / div;
            if (legacy < 0) legacy = 0;
            if (legacy > int.MaxValue) legacy = int.MaxValue;
            return (int)legacy;
        }

        public static string FormatScaledChange(long qtyChangeScaled, int pow10)
        {
            decimal display = ScaledToDisplay(Math.Abs(qtyChangeScaled), pow10);
            string sign = qtyChangeScaled < 0 ? "-" : (qtyChangeScaled > 0 ? "+" : "");
            return sign + SalesNumberFormat.FormatQuantity(display);
        }

        public static int GetScalePow10(SQLiteConnection con, SQLiteTransaction tran, int productId)
        {
            if (con == null) throw new ArgumentNullException(nameof(con));
            using (var cmd = new SQLiteCommand("SELECT qty_scale_pow10 FROM Products WHERE id=@id", con, tran))
            {
                cmd.Parameters.AddWithValue("@id", productId);
                object sv = cmd.ExecuteScalar();
                return sv == null || sv == DBNull.Value ? 0 : ClampScalePow10(Convert.ToInt32(sv));
            }
        }
    }
}
