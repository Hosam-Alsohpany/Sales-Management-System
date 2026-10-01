// ============================================================
// الملف    : OrderLineSnapshotService.cs
// الغرض    : خدمة أخذ لقطات أرشيفية لعناصر الفواتير وحفظها
// ============================================================

using System;
using System.Data.SQLite;
using Sales.Models;

namespace Sales.Services
{
    /// <summary>
    /// يلتقط لقطات بيانات بند الفاتورة وقت البيع (اسم، باركود، سعر).
    /// </summary>
    public static class OrderLineSnapshotService
    {
        /// <summary>
        /// يملأ حقول الـ snapshot الناقصة على OrderDetail قبل الحفظ.
        /// </summary>
        public static void EnrichSnapshots(OrderDetail item, SQLiteConnection con, SQLiteTransaction tran)
        {
            if (item == null) return;

            if (string.IsNullOrWhiteSpace(item.ProductNameSnapshot))
            {
                if (!string.IsNullOrWhiteSpace(item.ProductName))
                    item.ProductNameSnapshot = item.ProductName.Trim();
                else
                    item.ProductNameSnapshot = LookupProductLabel(con, tran, item.ProductId);
            }

            if (!item.SellPriceSnapshot.HasValue || item.SellPriceSnapshot.Value <= 0m)
                item.SellPriceSnapshot = item.Price;

            if (string.IsNullOrWhiteSpace(item.BarcodeSnapshot))
                item.BarcodeSnapshot = LookupDefaultBarcode(con, tran, item.ProductUnitId, item.ProductId);
        }

        private static string LookupProductLabel(SQLiteConnection con, SQLiteTransaction tran, int productId)
        {
            if (productId <= 0) return string.Empty;
            const string sql = "SELECT label FROM Products WHERE id=@id LIMIT 1";
            using (var cmd = new SQLiteCommand(sql, con, tran))
            {
                cmd.Parameters.AddWithValue("@id", productId);
                object v = cmd.ExecuteScalar();
                return v == null || v == DBNull.Value ? string.Empty : v.ToString();
            }
        }

        private static string LookupDefaultBarcode(SQLiteConnection con, SQLiteTransaction tran, int? productUnitId, int productId)
        {
            if (productUnitId.HasValue && productUnitId.Value > 0)
            {
                const string sqlUnit = @"SELECT barcode FROM ProductUnitBarcodes
                                         WHERE product_unit_id=@puid
                                         ORDER BY is_default DESC, id ASC LIMIT 1";
                using (var cmd = new SQLiteCommand(sqlUnit, con, tran))
                {
                    cmd.Parameters.AddWithValue("@puid", productUnitId.Value);
                    object v = cmd.ExecuteScalar();
                    if (v != null && v != DBNull.Value && !string.IsNullOrWhiteSpace(v.ToString()))
                        return v.ToString().Trim();
                }
            }

            if (productId <= 0) return string.Empty;

            const string sqlProduct = @"SELECT bub.barcode
                                        FROM ProductUnitBarcodes bub
                                        INNER JOIN ProductUnits pu ON pu.id = bub.product_unit_id
                                        WHERE pu.product_id = @pid
                                        ORDER BY bub.is_default DESC, bub.id ASC
                                        LIMIT 1";
            using (var cmd = new SQLiteCommand(sqlProduct, con, tran))
            {
                cmd.Parameters.AddWithValue("@pid", productId);
                object v = cmd.ExecuteScalar();
                return v == null || v == DBNull.Value ? string.Empty : v.ToString().Trim();
            }
        }
    }
}
