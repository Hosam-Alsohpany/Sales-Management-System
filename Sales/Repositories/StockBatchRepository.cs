// ============================================================
// الملف    : StockBatchRepository.cs
// الغرض    : مستودع إدارة دفعات المخزون وتواريخ الصلاحية والكميات
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
    public sealed class StockBatchRepository
    {
        public sealed class StockBatchSession : IDisposable
        {
            private readonly SQLiteConnection _con;
            private readonly SQLiteTransaction _tran;
            private bool _completed;

            public long BatchId { get; private set; }

            internal StockBatchSession(SQLiteConnection con, SQLiteTransaction tran, long batchId)
            {
                _con = con;
                _tran = tran;
                BatchId = batchId;
            }

            public void AddBatchItem(int productId, int qtyBefore, int qtyAfter)
            {
                TransactionGuard.Run("StockHistoryBatchItems.Insert", () =>
                {
                    int qtyChange = qtyAfter - qtyBefore;

                    const string sql = @"
                    INSERT INTO StockHistoryBatchItems (batch_id, product_id, qty_before, qty_after, qty_change)
                    VALUES (@bid, @pid, @before, @after, @change);";

                    using (var cmd = new SQLiteCommand(sql, _con, _tran))
                    {
                        cmd.Parameters.AddWithValue("@bid", BatchId);
                        cmd.Parameters.AddWithValue("@pid", productId);
                        cmd.Parameters.AddWithValue("@before", qtyBefore);
                        cmd.Parameters.AddWithValue("@after", qtyAfter);
                        cmd.Parameters.AddWithValue("@change", qtyChange);
                        cmd.ExecuteNonQuery();
                    }
                });
            }

            public void CommitBatch()
            {
                if (_completed) return;
                _tran.Commit();
                _completed = true;
            }

            public void Dispose()
            {
                if (!_completed)
                {
                    try { _tran.Rollback(); }
                    catch (Exception ex)
                    {
                        Logger.LogError("StockBatchSession.Dispose: Rollback failed", ex);
                    }
                }

                try { _tran.Dispose(); }
                catch (Exception ex)
                {
                    Logger.LogError("StockBatchSession.Dispose: Transaction dispose failed", ex);
                }

                try { _con.Dispose(); }
                catch (Exception ex)
                {
                    Logger.LogError("StockBatchSession.Dispose: Connection dispose failed", ex);
                }
            }
        }

        public DataTable GetBatches(string fromDate = null, string toDate = null)
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    string sql = @"
                        SELECT
                            b.id,
                            b.created_at,
                            b.created_by,
                            b.reason,
                            b.reference,
                            b.note,
                            (SELECT COUNT(*) FROM StockHistoryBatchItems i WHERE i.batch_id = b.id) AS items_count,
                            (SELECT IFNULL(SUM(i.qty_change), 0) FROM StockHistoryBatchItems i WHERE i.batch_id = b.id) AS total_qty_change
                        FROM StockHistoryBatches b
                        WHERE ( @from IS NULL OR b.created_at >= @from )
                          AND ( @to   IS NULL OR b.created_at <= @to )
                        ORDER BY b.id DESC
                        LIMIT 2000;";

                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@from", string.IsNullOrWhiteSpace(fromDate) ? (object)DBNull.Value : fromDate);
                        cmd.Parameters.AddWithValue("@to", string.IsNullOrWhiteSpace(toDate) ? (object)DBNull.Value : toDate);
                        using (var da = new SQLiteDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetBatches", ex);
                throw;
            }

            return dt;
        }

        public DataTable GetBatchItems(long batchId)
        {
            var dt = new DataTable();
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    const string sql = @"
                        SELECT
                            i.id,
                            i.batch_id,
                            i.product_id,
                            p.label AS product_name,
                            i.qty_before,
                            i.qty_after,
                            i.qty_change
                        FROM StockHistoryBatchItems i
                        INNER JOIN Products p ON p.id = i.product_id
                        WHERE i.batch_id = @bid
                        ORDER BY i.id DESC
                        LIMIT 5000;";

                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@bid", batchId);
                        using (var da = new SQLiteDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetBatchItems", ex);
                throw;
            }

            return dt;
        }

        public StockBatchSession BeginStockBatch(string reason, string reference, string userName, string note)
        {
            return TransactionGuard.Run("StockHistoryBatches.Insert", () =>
            {
                try
                {
                    var con = new SQLiteConnection(DatabaseInitializer.ConnectionString);
                    con.Open();

                    var tran = con.BeginTransaction();

                    const string sql = @"
                    INSERT INTO StockHistoryBatches (created_by, reason, reference, note)
                    VALUES (@user, @reason, @ref, @note);
                    SELECT last_insert_rowid();";

                    long batchId;
                    using (var cmd = new SQLiteCommand(sql, con, tran))
                    {
                        cmd.Parameters.AddWithValue("@user", (object)(userName ?? string.Empty));
                        cmd.Parameters.AddWithValue("@reason", (object)(reason ?? string.Empty));
                        cmd.Parameters.AddWithValue("@ref", string.IsNullOrWhiteSpace(reference) ? (object)DBNull.Value : reference);
                        cmd.Parameters.AddWithValue("@note", string.IsNullOrWhiteSpace(note) ? (object)DBNull.Value : note);

                        batchId = (long)cmd.ExecuteScalar();
                    }

                    return new StockBatchSession(con, tran, batchId);
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error in BeginStockBatch", ex);
                    throw;
                }
            });
        }

        public List<long> CreateBatchWithItems(string reason, string reference, string userName, string note, List<Tuple<int, int, int>> items)
        {
            var ids = new List<long>();
            using (var session = BeginStockBatch(reason, reference, userName, note))
            {
                if (items != null)
                {
                    foreach (var it in items)
                    {
                        if (it == null) continue;
                        session.AddBatchItem(it.Item1, it.Item2, it.Item3);
                    }
                }

                session.CommitBatch();
                ids.Add(session.BatchId);
            }
            return ids;
        }
    }
}
