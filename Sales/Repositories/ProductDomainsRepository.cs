// ============================================================
// الملف    : ProductDomainsRepository.cs
// الغرض    : مستودع تصنيف وإدارة مجالات المنتجات والباركود
// ============================================================

using System;
using System.Data.SQLite;
using Sales.Database;
using Sales.Services;
using Sales.Services.ProductDomains;
using Sales.Utilities;

namespace Sales.Repositories
{
    public class ProductDomainRow
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string DomainType { get; set; }
        public string ConfigJson { get; set; }
        public string LastValidatedAt { get; set; }
    }

    public class ProductDomainsRepository : IProductDomainsReadStore
    {
        public ProductDomainRow GetDomainOrNull(int productId)
        {
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    const string sql = @"SELECT id, product_id, domain_type, config_json, last_validated_at FROM ProductDomains WHERE product_id=@pid LIMIT 1";
                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@pid", productId);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) return null;

                            return new ProductDomainRow
                            {
                                Id = Convert.ToInt32(r["id"]),
                                ProductId = Convert.ToInt32(r["product_id"]),
                                DomainType = Convert.ToString(r["domain_type"]),
                                ConfigJson = Convert.ToString(r["config_json"]),
                                LastValidatedAt = r["last_validated_at"] == DBNull.Value ? null : Convert.ToString(r["last_validated_at"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("ProductDomainsRepository.GetDomainOrNull failed", ex);
                return null;
            }
        }

        ProductDomainSnapshot IProductDomainsReadStore.GetDomainOrNull(int productId)
        {
            var row = GetDomainOrNull(productId);
            if (row == null) return null;

            ProductDomainType domain;
            if (!Enum.TryParse((row.DomainType ?? string.Empty).Trim(), true, out domain))
            {
                Logger.LogInfo("ProductDomains: Unknown domain_type='" + (row.DomainType ?? string.Empty) + "' for product_id=" + productId + ". Fallback to GENERAL.");
                domain = ProductDomainType.GENERAL;
            }

            return new ProductDomainSnapshot
            {
                DomainType = domain,
                ConfigJson = row.ConfigJson
            };
        }

        public ProductDomainSnapshot GetDomainSnapshotOrNull(int productId)
        {
            return ((IProductDomainsReadStore)this).GetDomainOrNull(productId);
        }

        public ProductDomainRow EnsureDomainExists(int productId)
        {
            return TransactionGuard.Run("ProductDomains.Ensure", () =>
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            var existing = GetDomainOrNullTx(con, tran, productId);
                            if (existing != null)
                            {
                                tran.Commit();
                                return existing;
                            }

                            string domainType = ProductDomainType.GENERAL.ToString();
                            string cfg = ProductDomainConfigSerializer.CreateTemplateJson(ProductDomainType.GENERAL);

                            const string ins = @"INSERT INTO ProductDomains (product_id, domain_type, config_json, created_at) VALUES (@pid, @t, @cfg, CURRENT_TIMESTAMP); SELECT last_insert_rowid();";
                            using (var cmd = new SQLiteCommand(ins, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@pid", productId);
                                cmd.Parameters.AddWithValue("@t", domainType);
                                cmd.Parameters.AddWithValue("@cfg", cfg);
                                int id = Convert.ToInt32((long)cmd.ExecuteScalar());

                                tran.Commit();
                                return new ProductDomainRow
                                {
                                    Id = id,
                                    ProductId = productId,
                                    DomainType = domainType,
                                    ConfigJson = cfg,
                                    LastValidatedAt = null
                                };
                            }
                        }
                        catch (Exception ex)
                        {
                            try { tran.Rollback(); } catch { }
                            Logger.LogError("ProductDomainsRepository.EnsureDomainExists failed", ex);
                            throw;
                        }
                    }
                }
            });
        }

        public void UpsertDomain(int productId, ProductDomainType domainType, string configJson)
        {
            TransactionGuard.Run("ProductDomains.Upsert", () =>
            {
                string normalized;
                string err;
                if (!ProductDomainConfigSerializer.TryValidateAndNormalize(domainType, configJson, out normalized, out err))
                    throw new InvalidOperationException(err ?? "config_json غير صالح");

                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            var existing = GetDomainOrNullTx(con, tran, productId);
                            if (existing == null)
                            {
                                const string ins = @"INSERT INTO ProductDomains (product_id, domain_type, config_json, last_validated_at, created_at) VALUES (@pid, @t, @cfg, NULL, CURRENT_TIMESTAMP);";
                                using (var cmd = new SQLiteCommand(ins, con, tran))
                                {
                                    cmd.Parameters.AddWithValue("@pid", productId);
                                    cmd.Parameters.AddWithValue("@t", domainType.ToString());
                                    cmd.Parameters.AddWithValue("@cfg", normalized);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                const string upd = @"UPDATE ProductDomains SET domain_type=@t, config_json=@cfg, updated_at=CURRENT_TIMESTAMP, last_validated_at=CURRENT_TIMESTAMP WHERE product_id=@pid";
                                using (var cmd = new SQLiteCommand(upd, con, tran))
                                {
                                    cmd.Parameters.AddWithValue("@pid", productId);
                                    cmd.Parameters.AddWithValue("@t", domainType.ToString());
                                    cmd.Parameters.AddWithValue("@cfg", normalized);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            tran.Commit();
                        }
                        catch
                        {
                            try { tran.Rollback(); } catch { }
                            throw;
                        }
                    }
                }
            });
        }

        private static ProductDomainRow GetDomainOrNullTx(SQLiteConnection con, SQLiteTransaction tran, int productId)
        {
            const string sql = @"SELECT id, product_id, domain_type, config_json, last_validated_at FROM ProductDomains WHERE product_id=@pid LIMIT 1";
            using (var cmd = new SQLiteCommand(sql, con, tran))
            {
                cmd.Parameters.AddWithValue("@pid", productId);
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;

                    return new ProductDomainRow
                    {
                        Id = Convert.ToInt32(r["id"]),
                        ProductId = Convert.ToInt32(r["product_id"]),
                        DomainType = Convert.ToString(r["domain_type"]),
                        ConfigJson = Convert.ToString(r["config_json"]),
                        LastValidatedAt = r["last_validated_at"] == DBNull.Value ? null : Convert.ToString(r["last_validated_at"])
                    };
                }
            }
        }
    }
}
