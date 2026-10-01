// ============================================================
// الملف    : UnitsRepository.cs
// الغرض    : مستودع إدارة الوحدات القياسية العامة (حبة، كرتون، كيلو)
// ============================================================

using System;
using System.Data;
using System.Data.SQLite;
using Sales.Database;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Repositories
{
    public class UnitsRepository
    {
        public DataTable GetAll()
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
                Logger.LogError("Error in UnitsRepository.GetAll", ex);
                throw;
            }

            return dt;
        }

        public bool ExistsByName(string name, int? excludeId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    const string sql = @"
                        SELECT 1
                        FROM Units
                        WHERE lower(trim(name)) = lower(trim(@name))
                          AND (@excludeId IS NULL OR id <> @excludeId)
                        LIMIT 1;";

                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@name", name.Trim());
                        if (excludeId.HasValue)
                            cmd.Parameters.AddWithValue("@excludeId", excludeId.Value);
                        else
                            cmd.Parameters.AddWithValue("@excludeId", DBNull.Value);

                        object o = cmd.ExecuteScalar();
                        return o != null && o != DBNull.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in UnitsRepository.ExistsByName", ex);
                throw;
            }
        }

        public int Add(string name)
        {
            return TransactionGuard.Run("Units.Insert", () =>
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("name");

                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            const string sql = @"
                            INSERT INTO Units(name)
                            VALUES(@name);
                            SELECT last_insert_rowid();";

                            using (var cmd = new SQLiteCommand(sql, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@name", name.Trim());
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
                                Logger.LogError("UnitsRepository.Add: Rollback failed", rbEx);
                            }
                            Logger.LogError("Error in UnitsRepository.Add", ex);
                            throw;
                        }
                    }
                }
            });
        }

        public void Update(int id, string name)
        {
            TransactionGuard.Run("Units.Update", () =>
            {
                if (id <= 0)
                    throw new ArgumentOutOfRangeException("id");
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("name");

                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            const string sql = "UPDATE Units SET name=@name WHERE id=@id";
                            using (var cmd = new SQLiteCommand(sql, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@name", name.Trim());
                                cmd.Parameters.AddWithValue("@id", id);
                                cmd.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            try { tran.Rollback(); }
                            catch (Exception rbEx)
                            {
                                Logger.LogError("UnitsRepository.Update: Rollback failed", rbEx);
                            }
                            Logger.LogError("Error in UnitsRepository.Update", ex);
                            throw;
                        }
                    }
                }
            });
        }

        public void Delete(int id)
        {
            TransactionGuard.Run("Units.Delete", () =>
            {
                if (id <= 0)
                    throw new ArgumentOutOfRangeException("id");

                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            const string sql = "DELETE FROM Units WHERE id=@id";
                            using (var cmd = new SQLiteCommand(sql, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@id", id);
                                cmd.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            try { tran.Rollback(); }
                            catch (Exception rbEx)
                            {
                                Logger.LogError("UnitsRepository.Delete: Rollback failed", rbEx);
                            }
                            Logger.LogError("Error in UnitsRepository.Delete", ex);
                            throw;
                        }
                    }
                }
            });
        }
    }
}
