// ============================================================
// الملف    : BarcodeProfilesRepository.cs
// الغرض    : مستودع إدارة وحفظ ملفات تكوين قراءة الباركود
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Sales.Database;

namespace Sales.Services.BarcodePlatform
{
    public class BarcodeProfilesRepository
    {
        public List<BarcodeProfileRow> GetAllProfiles()
        {
            var list = new List<BarcodeProfileRow>();

            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    const string sql = @"SELECT code, name, is_enabled, priority, config_json
                                        FROM BarcodeProfiles
                                        ORDER BY priority ASC, id ASC;";

                    using (var cmd = new SQLiteCommand(sql, con))
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new BarcodeProfileRow
                            {
                                Code = dr["code"].ToString(),
                                Name = dr["name"].ToString(),
                                IsEnabled = Convert.ToInt32(dr["is_enabled"]) == 1,
                                Priority = dr["priority"] == DBNull.Value ? 100 : Convert.ToInt32(dr["priority"]),
                                ConfigJson = dr["config_json"].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
            }

            return list;
        }

        public List<BarcodeProfileRow> GetEnabledProfiles()
        {
            var list = new List<BarcodeProfileRow>();

            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    const string sql = @"SELECT code, name, is_enabled, priority, config_json
                                        FROM BarcodeProfiles
                                        WHERE is_enabled = 1
                                        ORDER BY priority ASC, id ASC;";

                    using (var cmd = new SQLiteCommand(sql, con))
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new BarcodeProfileRow
                            {
                                Code = dr["code"].ToString(),
                                Name = dr["name"].ToString(),
                                IsEnabled = Convert.ToInt32(dr["is_enabled"]) == 1,
                                Priority = dr["priority"] == DBNull.Value ? 100 : Convert.ToInt32(dr["priority"]),
                                ConfigJson = dr["config_json"].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                // If DB is old or table missing, engine will fallback to defaults.
            }

            return list;
        }

        public void UpsertProfile(BarcodeProfileRow row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            string code = (row.Code ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Profile code is required", "row");

            string name = (row.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Profile name is required", "row");

            string cfg = row.ConfigJson ?? "{}";
            int enabled = row.IsEnabled ? 1 : 0;
            int priority = row.Priority;

            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();

                using (var insertIgnore = new SQLiteCommand(@"INSERT OR IGNORE INTO BarcodeProfiles(code, name, is_enabled, priority, config_json)
                                                           VALUES(@code, @name, @is_enabled, @priority, @cfg);", con))
                {
                    insertIgnore.Parameters.AddWithValue("@code", code);
                    insertIgnore.Parameters.AddWithValue("@name", name);
                    insertIgnore.Parameters.AddWithValue("@is_enabled", enabled);
                    insertIgnore.Parameters.AddWithValue("@priority", priority);
                    insertIgnore.Parameters.AddWithValue("@cfg", cfg);
                    insertIgnore.ExecuteNonQuery();
                }

                using (var update = new SQLiteCommand(@"UPDATE BarcodeProfiles
                                                     SET name=@name,
                                                         is_enabled=@is_enabled,
                                                         priority=@priority,
                                                         config_json=@cfg,
                                                         updated_at=CURRENT_TIMESTAMP
                                                     WHERE code=@code;", con))
                {
                    update.Parameters.AddWithValue("@code", code);
                    update.Parameters.AddWithValue("@name", name);
                    update.Parameters.AddWithValue("@is_enabled", enabled);
                    update.Parameters.AddWithValue("@priority", priority);
                    update.Parameters.AddWithValue("@cfg", cfg);
                    update.ExecuteNonQuery();
                }
            }
        }
    }
}
