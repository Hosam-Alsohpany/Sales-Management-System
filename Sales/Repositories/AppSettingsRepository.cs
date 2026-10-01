// ============================================================
// الملف    : AppSettingsRepository.cs
// الغرض    : مستودع حفظ وإدارة إعدادات النظام في قاعدة البيانات
// ============================================================

using System;

using System.Data.SQLite;

using Sales.Database;

using Sales.Services;



namespace Sales.Repositories

{

    public class AppSettingsRepository

    {

        public string GetValue(string key)

        {

            if (string.IsNullOrWhiteSpace(key)) return null;



            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))

            {

                con.Open();

                const string sql = "SELECT value FROM AppSettings WHERE key = @key LIMIT 1";

                using (var cmd = new SQLiteCommand(sql, con))

                {

                    cmd.Parameters.AddWithValue("@key", key.Trim());

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value) return null;

                    return result.ToString();

                }

            }

        }



        public void SetValue(string key, string value)

        {

            TransactionGuard.Run("AppSettings.Upsert", () =>
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException("key is required", "key");

                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    string k = key.Trim();
                    string v = value ?? string.Empty;

                    using (var insertIgnore = new SQLiteCommand("INSERT OR IGNORE INTO AppSettings(key, value) VALUES(@key, @value)", con))
                    {
                        insertIgnore.Parameters.AddWithValue("@key", k);
                        insertIgnore.Parameters.AddWithValue("@value", v);
                        insertIgnore.ExecuteNonQuery();
                    }

                    using (var update = new SQLiteCommand("UPDATE AppSettings SET value=@value WHERE key=@key", con))
                    {
                        update.Parameters.AddWithValue("@key", k);
                        update.Parameters.AddWithValue("@value", v);
                        update.ExecuteNonQuery();
                    }
                }
            });

        }

    }

}

