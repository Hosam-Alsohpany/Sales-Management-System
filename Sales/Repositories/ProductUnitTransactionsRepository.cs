// ============================================================
// الملف    : ProductUnitTransactionsRepository.cs
// الغرض    : مستودع عمليات وجرد وحدات المنتجات لتحديث مخزونها
// ============================================================

using System;
using System.Data.SQLite;
using Sales.Database;
using Sales.Utilities;

namespace Sales.Repositories
{
    public class ProductUnitTransactionsRepository
    {
        public bool HasAnyTransactions(int productId)
        {
            if (productId <= 0) return false;

            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    if (Exists(con, "SELECT 1 FROM Order_Details WHERE id_product=@pid LIMIT 1", productId))
                        return true;

                    if (Exists(con, "SELECT 1 FROM PurchaseDetails WHERE product_id=@pid LIMIT 1", productId))
                        return true;

                    if (Exists(con, "SELECT 1 FROM StockHistory WHERE product_id=@pid LIMIT 1", productId))
                        return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("ProductUnitTransactionsRepository.HasAnyTransactions failed", ex);
            }

            return false;
        }

        private bool Exists(SQLiteConnection con, string sql, int productId)
        {
            using (var cmd = new SQLiteCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@pid", productId);
                object v = cmd.ExecuteScalar();
                return v != null && v != DBNull.Value;
            }
        }
    }
}
