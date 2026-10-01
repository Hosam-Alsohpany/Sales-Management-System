// ============================================================
// الملف    : ProductUnitsRepository.cs
// الغرض    : مستودع لإدارة الوحدات المرتبطة بالمنتجات وأسعارها
// ============================================================

using System;
using System.Data;
using System.Data.SQLite;
using Sales.Database;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Repositories
{
    public class ProductUnitsRepository
    {
        public DataTable GetAllUnits()
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    const string sql = "SELECT id, name FROM Units ORDER BY name";
                    using (var da = new SQLiteDataAdapter(sql, con))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetAllUnits", ex);
                throw;
            }
            return dt;
        }

        public DataTable GetProductUnits(int productId)
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    const string sql = @"
                        SELECT 
                            pu.id,
                            pu.product_id,
                            pu.unit_id,
                            u.name AS unit_name,
                            pu.factor,
                            pu.sell_price,
                            pu.cost_price,
                            pu.display_order,
                            pu.parent_product_unit_id,
                            pu.pack_size,
                            (
                                SELECT u2.name
                                FROM ProductUnits ppar
                                INNER JOIN Units u2 ON u2.id = ppar.unit_id
                                WHERE ppar.id = pu.parent_product_unit_id
                                LIMIT 1
                            ) AS parent_unit_name,
                            (
                                SELECT b.barcode
                                FROM ProductUnitBarcodes b
                                WHERE b.product_unit_id = pu.id
                                ORDER BY b.is_default DESC, b.id DESC
                                LIMIT 1
                            ) AS default_barcode,
                            (
                                SELECT b.barcode_type
                                FROM ProductUnitBarcodes b
                                WHERE b.product_unit_id = pu.id
                                ORDER BY b.is_default DESC, b.id DESC
                                LIMIT 1
                            ) AS default_barcode_type,
                            CASE WHEN pu.factor = 1 THEN 1 ELSE 0 END AS is_base_unit,
                            CASE WHEN pu.parent_product_unit_id IS NULL THEN 0 ELSE 1 END AS hierarchy_level
                        FROM ProductUnits pu
                        INNER JOIN Units u ON u.id = pu.unit_id
                        WHERE pu.product_id = @pid
                        ORDER BY pu.display_order, u.name";

                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@pid", productId);
                        using (var da = new SQLiteDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetProductUnits", ex);
                throw;
            }
            return dt;
        }

        public int AddProductUnit(int productId, int unitId, decimal factor, decimal sellPrice, decimal costPrice, int displayOrder, int? parentProductUnitId = null, decimal? packSize = null)
        {
            return TransactionGuard.Run("ProductUnits.Insert", () =>
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            const string sql = @"
                            INSERT INTO ProductUnits (product_id, unit_id, factor, sell_price, cost_price, display_order, parent_product_unit_id, pack_size)
                            VALUES (@pid, @uid, @factor, @sell, @cost, @disp, @parent, @pack);
                            SELECT last_insert_rowid();";

                            using (var cmd = new SQLiteCommand(sql, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@pid", productId);
                                cmd.Parameters.AddWithValue("@uid", unitId);
                                cmd.Parameters.AddWithValue("@factor", factor);
                                cmd.Parameters.AddWithValue("@sell", sellPrice);
                                cmd.Parameters.AddWithValue("@cost", costPrice);
                                cmd.Parameters.AddWithValue("@disp", displayOrder);
                                cmd.Parameters.AddWithValue("@parent", (object)parentProductUnitId ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@pack", (object)packSize ?? DBNull.Value);

                                int id = Convert.ToInt32((long)cmd.ExecuteScalar());
                                tran.Commit();
                                return id;
                            }
                        }
                        catch (Exception ex)
                        {
                            try { tran.Rollback(); }
                            catch (Exception rbEx)
                            {
                                Logger.LogError("ProductUnitsRepository.AddProductUnit: Rollback failed", rbEx);
                            }
                            Logger.LogError("Error in AddProductUnit", ex);
                            throw;
                        }
                    }
                }
            });
        }

        public void UpdateProductUnit(int productUnitId, int unitId, decimal factor, decimal sellPrice, decimal costPrice, int displayOrder, int? parentProductUnitId = null, decimal? packSize = null)
        {
            TransactionGuard.Run("ProductUnits.Update", () =>
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            const string sql = @"
                            UPDATE ProductUnits
                            SET unit_id=@uid, factor=@factor, sell_price=@sell, cost_price=@cost, display_order=@disp,
                                parent_product_unit_id=@parent, pack_size=@pack
                            WHERE id=@id";

                            using (var cmd = new SQLiteCommand(sql, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@uid", unitId);
                                cmd.Parameters.AddWithValue("@factor", factor);
                                cmd.Parameters.AddWithValue("@sell", sellPrice);
                                cmd.Parameters.AddWithValue("@cost", costPrice);
                                cmd.Parameters.AddWithValue("@disp", displayOrder);
                                cmd.Parameters.AddWithValue("@parent", (object)parentProductUnitId ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@pack", (object)packSize ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@id", productUnitId);
                                int affected = cmd.ExecuteNonQuery();
                                if (affected == 0)
                                    throw new InvalidOperationException("السجل غير موجود");
                            }

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            try { tran.Rollback(); }
                            catch (Exception rbEx)
                            {
                                Logger.LogError("ProductUnitsRepository.UpdateProductUnit: Rollback failed", rbEx);
                            }
                            Logger.LogError("Error in UpdateProductUnit", ex);
                            throw;
                        }
                    }
                }
            });
        }

        public void DeleteProductUnit(int productUnitId)
        {
            TransactionGuard.Run("ProductUnits.Delete", () =>
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            const string sql = "DELETE FROM ProductUnits WHERE id=@id";
                            using (var cmd = new SQLiteCommand(sql, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@id", productUnitId);
                                int affected = cmd.ExecuteNonQuery();
                                if (affected == 0)
                                    throw new InvalidOperationException("السجل غير موجود");
                            }
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            try { tran.Rollback(); }
                            catch (Exception rbEx)
                            {
                                Logger.LogError("ProductUnitsRepository.DeleteProductUnit: Rollback failed", rbEx);
                            }
                            Logger.LogError("Error in DeleteProductUnit", ex);
                            throw;
                        }
                    }
                }
            });
        }

        public DataTable GetProductUnitBarcodes(int productUnitId)
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    string sql = @"
                        SELECT id, product_unit_id, barcode_type, barcode, is_default
                        FROM ProductUnitBarcodes
                        WHERE product_unit_id = @puid
                        ORDER BY is_default DESC, id DESC";

                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@puid", productUnitId);
                        using (var da = new SQLiteDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetProductUnitBarcodes", ex);
                throw;
            }
            return dt;
        }

        public int AddProductUnitBarcode(int productUnitId, string barcode, bool isDefault)
        {
            return TransactionGuard.Run("ProductUnitBarcodes.Insert", () =>
            {
                if (string.IsNullOrWhiteSpace(barcode))
                    throw new InvalidOperationException("الباركود فارغ");

                barcode = barcode.Trim();

                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            using (var cmdCheck = new SQLiteCommand("SELECT COUNT(*) FROM ProductUnitBarcodes WHERE barcode_type=@t AND barcode=@b", con, tran))
                            {
                                cmdCheck.Parameters.AddWithValue("@t", "NORMAL");
                                cmdCheck.Parameters.AddWithValue("@b", barcode);
                                int exists = Convert.ToInt32(cmdCheck.ExecuteScalar());
                                if (exists > 0)
                                    throw new InvalidOperationException("الباركود مستخدم مسبقاً: " + barcode);
                            }

                            const string sql = @"
                            INSERT INTO ProductUnitBarcodes (product_unit_id, barcode_type, barcode, is_default)
                            VALUES (@puid, @t, @barcode, @is_default);
                            SELECT last_insert_rowid();";

                            using (var cmd = new SQLiteCommand(sql, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@puid", productUnitId);
                                cmd.Parameters.AddWithValue("@t", "NORMAL");
                                cmd.Parameters.AddWithValue("@barcode", barcode);
                                cmd.Parameters.AddWithValue("@is_default", isDefault ? 1 : 0);
                                int id = Convert.ToInt32((long)cmd.ExecuteScalar());
                                tran.Commit();
                                return id;
                            }
                        }
                        catch (Exception ex)
                        {
                            try { tran.Rollback(); }
                            catch (Exception rbEx)
                            {
                                Logger.LogError("ProductUnitsRepository.AddProductUnitBarcode: Rollback failed", rbEx);
                            }
                            Logger.LogError("Error in AddProductUnitBarcode", ex);
                            throw;
                        }
                    }
                }
            });
        }

        public void SetDefaultBarcode(int barcodeId, int productUnitId)
        {
            TransactionGuard.Run("ProductUnitBarcodes.UpdateDefault", () =>
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            const string sqlClear = "UPDATE ProductUnitBarcodes SET is_default=0 WHERE product_unit_id=@puid";
                            using (var cmd = new SQLiteCommand(sqlClear, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@puid", productUnitId);
                                cmd.ExecuteNonQuery();
                            }

                            const string sqlSet = "UPDATE ProductUnitBarcodes SET is_default=1 WHERE id=@id";
                            using (var cmd = new SQLiteCommand(sqlSet, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@id", barcodeId);
                                int affected = cmd.ExecuteNonQuery();
                                if (affected == 0)
                                    throw new InvalidOperationException("الباركود غير موجود");
                            }

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            try { tran.Rollback(); }
                            catch (Exception rbEx)
                            {
                                Logger.LogError("ProductUnitsRepository.SetDefaultBarcode: Rollback failed", rbEx);
                            }
                            Logger.LogError("Error in SetDefaultBarcode", ex);
                            throw;
                        }
                    }
                }
            });
        }

        public void DeleteBarcode(int barcodeId)
        {
            TransactionGuard.Run("ProductUnitBarcodes.Delete", () =>
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            const string sql = "DELETE FROM ProductUnitBarcodes WHERE id=@id";
                            using (var cmd = new SQLiteCommand(sql, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@id", barcodeId);
                                int affected = cmd.ExecuteNonQuery();
                                if (affected == 0)
                                    throw new InvalidOperationException("الباركود غير موجود");
                            }
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            try { tran.Rollback(); }
                            catch (Exception rbEx)
                            {
                                Logger.LogError("ProductUnitsRepository.DeleteBarcode: Rollback failed", rbEx);
                            }
                            Logger.LogError("Error in DeleteBarcode", ex);
                            throw;
                        }
                    }
                }
            });
        }
    }
}
