using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Sales.Utilities;

namespace Sales.Database.Migrations
{
    /// <summary>
    /// يطبّق ترحيلات SchemaVersion بالترتيب.
    /// </summary>
    public static class MigrationRunner
    {
        public static void EnsureSchemaVersionTable(SQLiteConnection con)
        {
            SqlExecutor.Execute(@"CREATE TABLE IF NOT EXISTS SchemaVersion (
                version INTEGER PRIMARY KEY,
                description TEXT,
                applied_at TEXT DEFAULT CURRENT_TIMESTAMP
            );", con);
        }

        public static int GetCurrentVersion(SQLiteConnection con)
        {
            EnsureSchemaVersionTable(con);
            using (var cmd = new SQLiteCommand("SELECT IFNULL(MAX(version), 0) FROM SchemaVersion", con))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void RunPending(SQLiteConnection con, IEnumerable<IMigration> migrations)
        {
            if (con == null) throw new ArgumentNullException(nameof(con));
            EnsureSchemaVersionTable(con);
            int current = GetCurrentVersion(con);

            foreach (var m in migrations.OrderBy(x => x.Version))
            {
                if (m.Version <= current) continue;
                try
                {
                    m.Apply(con);
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO SchemaVersion(version, description) VALUES (@v, @d)", con))
                    {
                        cmd.Parameters.AddWithValue("@v", m.Version);
                        cmd.Parameters.AddWithValue("@d", m.Description ?? string.Empty);
                        cmd.ExecuteNonQuery();
                    }
                    current = m.Version;
                }
                catch (Exception ex)
                {
                    try { Logger.LogError("Migration " + m.Version + " failed: " + m.Description, ex); } catch { }
                    throw;
                }
            }
        }

        public static IEnumerable<IMigration> GetAllMigrations()
        {
            yield return new Migration_001_ErrorLogTable();
            yield return new Migration_002_OrderTaxAmount();
            yield return new Migration_003_LegacyColumnsBatch1();
        }
    }
}
