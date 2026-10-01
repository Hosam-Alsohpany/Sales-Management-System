// ============================================================
// الملف    : PosHotkeysRepository.cs
// الغرض    : مستودع إعدادات أزرار الاختصارات لشاشة نقاط البيع
// ============================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Sales.Database;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Repositories
{
    public sealed class PosHotkeysRepository
    {
        public DataTable GetAllHotkeys()
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    const string sql = @"
                        SELECT
                            hk.key_code,
                            hk.product_unit_id,
                            p.label AS product_name,
                            u.name AS unit_name,
                            pu.factor,
                            hk.qty_delta,
                            hk.note,
                            hk.updated_at
                        FROM PosHotkeys hk
                        INNER JOIN ProductUnits pu ON pu.id = hk.product_unit_id
                        INNER JOIN Products p ON p.id = pu.product_id
                        INNER JOIN Units u ON u.id = pu.unit_id
                        ORDER BY hk.key_code ASC;";

                    using (var da = new SQLiteDataAdapter(sql, con))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetAllHotkeys", ex);
                throw;
            }

            return dt;
        }

        public Dictionary<int, HotkeyEntry> GetHotkeysDictionary()
        {
            var dict = new Dictionary<int, HotkeyEntry>();
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    const string sql = @"
                        SELECT key_code, product_unit_id, qty_delta, note
                        FROM PosHotkeys";

                    using (var cmd = new SQLiteCommand(sql, con))
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int keyCode = dr["key_code"] == DBNull.Value ? 0 : Convert.ToInt32(dr["key_code"]);
                            if (keyCode == 0) continue;

                            dict[keyCode] = new HotkeyEntry
                            {
                                KeyCode = keyCode,
                                ProductUnitId = dr["product_unit_id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["product_unit_id"]),
                                QtyDelta = dr["qty_delta"] == DBNull.Value ? 1m : Convert.ToDecimal(dr["qty_delta"]),
                                Note = dr["note"] == DBNull.Value ? string.Empty : dr["note"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetHotkeysDictionary", ex);
                throw;
            }

            return dict;
        }

        public void UpsertHotkey(int keyCode, int productUnitId, decimal qtyDelta, string note)
        {
            TransactionGuard.Run("PosHotkeys.Upsert", () =>
            {
                if (keyCode <= 0) throw new InvalidOperationException("KeyCode غير صحيح");
                if (productUnitId <= 0) throw new InvalidOperationException("ProductUnitId غير صحيح");
                if (qtyDelta <= 0) throw new InvalidOperationException("QtyDelta غير صحيحة");

                try
                {
                    using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                    {
                        con.Open();
                        using (var tran = con.BeginTransaction())
                        {
                            using (var insertIgnore = new SQLiteCommand(@"
                            INSERT OR IGNORE INTO PosHotkeys (key_code, product_unit_id, qty_delta, note, updated_at)
                            VALUES (@key, @puid, @qty, @note, CURRENT_TIMESTAMP);", con, tran))
                            {
                                insertIgnore.Parameters.AddWithValue("@key", keyCode);
                                insertIgnore.Parameters.AddWithValue("@puid", productUnitId);
                                insertIgnore.Parameters.AddWithValue("@qty", qtyDelta);
                                insertIgnore.Parameters.AddWithValue("@note", string.IsNullOrWhiteSpace(note) ? (object)DBNull.Value : note);
                                insertIgnore.ExecuteNonQuery();
                            }

                            using (var update = new SQLiteCommand(@"
                            UPDATE PosHotkeys
                            SET product_unit_id=@puid,
                                qty_delta=@qty,
                                note=@note,
                                updated_at=CURRENT_TIMESTAMP
                            WHERE key_code=@key;", con, tran))
                            {
                                update.Parameters.AddWithValue("@key", keyCode);
                                update.Parameters.AddWithValue("@puid", productUnitId);
                                update.Parameters.AddWithValue("@qty", qtyDelta);
                                update.Parameters.AddWithValue("@note", string.IsNullOrWhiteSpace(note) ? (object)DBNull.Value : note);
                                update.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error in UpsertHotkey", ex);
                    throw;
                }
            });
        }

        public void DeleteHotkey(int keyCode)
        {
            TransactionGuard.Run("PosHotkeys.Delete", () =>
            {
                if (keyCode <= 0) return;

                try
                {
                    using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                    {
                        con.Open();
                        using (var tran = con.BeginTransaction())
                        {
                            const string sql = "DELETE FROM PosHotkeys WHERE key_code=@key";
                            using (var cmd = new SQLiteCommand(sql, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@key", keyCode);
                                cmd.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error in DeleteHotkey", ex);
                    throw;
                }
            });
        }

        public sealed class HotkeyEntry
        {
            public int KeyCode { get; set; }
            public int ProductUnitId { get; set; }
            public decimal QtyDelta { get; set; }
            public string Note { get; set; }
        }
    }
}
