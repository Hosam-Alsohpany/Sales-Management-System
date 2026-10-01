using System;
using System.Data;

namespace Sales.Utilities
{
    /// <summary>
    /// عرض بنود الفاتورة من الـ snapshots المحفوظة وقت البيع.
    /// </summary>
    public static class InvoiceLineFormatter
    {
        public static string GetProductName(DataRow row)
        {
            if (row == null) return string.Empty;
            if (row.Table != null && row.Table.Columns.Contains("product_label") && row["product_label"] != DBNull.Value)
                return row["product_label"].ToString();
            if (row.Table != null && row.Table.Columns.Contains("product_name_snapshot") && row["product_name_snapshot"] != DBNull.Value)
                return row["product_name_snapshot"].ToString();
            return string.Empty;
        }

        public static decimal GetUnitPrice(DataRow row)
        {
            if (row == null) return 0m;
            if (row.Table != null && row.Table.Columns.Contains("price") && row["price"] != DBNull.Value)
                return Convert.ToDecimal(row["price"]);
            return 0m;
        }

        public static decimal GetQuantity(DataRow row)
        {
            if (row == null) return 0m;
            if (row.Table != null && row.Table.Columns.Contains("qty") && row["qty"] != DBNull.Value)
                return Convert.ToDecimal(row["qty"]);
            return 0m;
        }
    }
}
