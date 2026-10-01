using System;
using System.Data;
using System.Data.SQLite;
using Sales.Database;
using Sales.Models;
using Sales.Utilities;

namespace Sales.Repositories
{
    public class StockHistoryRepository
    {
        // تسجيل حركة مخزون جديدة
        public void AddLog(StockHistory log)
        {
            throw new InvalidOperationException("Use StockService only");
        }

        // جلب سجل حركات صنف معين
        public DataTable GetHistoryByProduct(int productId)
        {
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                string sql = "SELECT * FROM StockHistory WHERE product_id = @pid ORDER BY change_date DESC";
                using (var cmd = new SQLiteDataAdapter(sql, con))
                {
                    cmd.SelectCommand.Parameters.AddWithValue("@pid", productId);
                    DataTable dt = new DataTable();
                    cmd.Fill(dt);
                    return dt;
                }
            }
        }

        // جلب كامل السجل مع اسم المنتج
        public DataTable GetAllHistory()
        {
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                string sql = @"
                    SELECT 
                        sh.id AS Id,
                        p.label AS Product,
                        sh.qty_change AS QtyChange,
                        sh.qty_change_scaled AS QtyChangeScaled,
                        COALESCE(sh.qty_scale_pow10, p.qty_scale_pow10, 0) AS QtyScalePow10,
                        sh.reason AS Reason,
                        sh.change_date AS ChangeDate,
                        sh.user_name AS UserName
                    FROM StockHistory sh
                    JOIN Products p ON sh.product_id = p.id
                    ORDER BY sh.change_date DESC";

                using (var da = new SQLiteDataAdapter(sql, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    EnrichDisplayQuantities(dt);
                    return dt;
                }
            }
        }

        private static void EnrichDisplayQuantities(DataTable dt)
        {
            if (dt == null) return;
            if (!dt.Columns.Contains("QtyDisplay"))
                dt.Columns.Add("QtyDisplay", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                if (row == null) continue;
                long scaled = 0;
                int pow = 0;
                try
                {
                    if (dt.Columns.Contains("QtyChangeScaled") && row["QtyChangeScaled"] != DBNull.Value)
                        scaled = Convert.ToInt64(row["QtyChangeScaled"]);
                    else if (row["QtyChange"] != DBNull.Value)
                        scaled = Convert.ToInt64(row["QtyChange"]);
                }
                catch { }

                try
                {
                    if (dt.Columns.Contains("QtyScalePow10") && row["QtyScalePow10"] != DBNull.Value)
                        pow = ProductQuantityHelper.ClampScalePow10(Convert.ToInt32(row["QtyScalePow10"]));
                }
                catch { }

                row["QtyDisplay"] = ProductQuantityHelper.FormatScaledChange(scaled, pow);
            }
        }
    }
}
