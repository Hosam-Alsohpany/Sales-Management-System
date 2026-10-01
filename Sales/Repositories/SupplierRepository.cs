// ============================================================
// الملف    : SupplierRepository.cs
// الغرض    : مستودع لإدارة وإضافة حسابات وبيانات الموردين
// ============================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Sales.Database;
using Sales.Models;
using Sales.Services;
using Sales.Utilities;
using System.Linq;

namespace Sales.Repositories
{
    /// <summary>
    /// فئة مسؤولة عن جميع عمليات قاعدة البيانات المتعلقة بالموردين
    /// تتبع نمط Repository Pattern لفصل منطق الوصول للبيانات
    /// </summary>
    public class SupplierRepository
    {
        /// <summary>
        /// جلب جميع الموردين من قاعدة البيانات
        /// </summary>
        /// <returns>قائمة بجميع الموردين</returns>
        public List<Supplier> GetAllSuppliers()
        {
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    string sql = "SELECT * FROM Suppliers ORDER BY name";
                    using (var cmd = new SQLiteCommand(sql, con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        var suppliers = new List<Supplier>();
                        while (reader.Read())
                        {
                            suppliers.Add(new Supplier
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString(),
                                Phone = reader["phone"].ToString(),
                                Address = reader["address"].ToString(),
                                Details = reader["details"] != DBNull.Value ? reader["details"].ToString() : ""
                            });
                        }
                        return suppliers;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetAllSuppliers", ex);
                throw;
            }
        }

        /// <summary>
        /// جلب مورد محدد بالمعرف
        /// </summary>
        /// <param name="id">معرف المورد المطلوب</param>
        /// <returns>كائن المورد إذا وجد، أو null إذا لم يوجد</returns>
        public Supplier GetSupplierById(int id)
        {
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    string sql = "SELECT * FROM Suppliers WHERE id = @id";
                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Supplier
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Name = reader["name"].ToString(),
                                    Phone = reader["phone"].ToString(),
                                    Address = reader["address"].ToString(),
                                    Details = reader["details"] != DBNull.Value ? reader["details"].ToString() : ""
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetSupplierById for id: {id}", ex);
                throw;
            }
            return null;
        }

        /// <summary>
        /// إضافة مورد جديد إلى قاعدة البيانات
        /// </summary>
        /// <param name="supplier">كائن المورد المراد إضافته</param>
        /// <exception cref="ArgumentNullException">إذا كان المورد null</exception>
        public void AddSupplier(Supplier supplier)
        {
            TransactionGuard.Run("Suppliers.Insert", () =>
            {
                if (supplier == null)
                    throw new ArgumentNullException(nameof(supplier), "المورد لا يمكن أن يكون فارغاً");

                try
                {
                    using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                    {
                        con.Open();
                        string sql = "INSERT INTO Suppliers (name, phone, address, details) VALUES (@name, @phone, @address, @details)";
                        using (var cmd = new SQLiteCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@name", supplier.Name);
                            cmd.Parameters.AddWithValue("@phone", supplier.Phone ?? "");
                            cmd.Parameters.AddWithValue("@address", supplier.Address ?? "");
                            cmd.Parameters.AddWithValue("@details", supplier.Details ?? "");
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error in AddSupplier for supplier: {supplier.Name}", ex);
                    throw;
                }
            });
        }

        /// <summary>
        /// تحديث بيانات مورد موجود
        /// </summary>
        /// <param name="supplier">كائن المورد المحدث</param>
        /// <exception cref="ArgumentNullException">إذا كان المورد null</exception>
        public void UpdateSupplier(Supplier supplier)
        {
            TransactionGuard.Run("Suppliers.Update", () =>
            {
                if (supplier == null)
                    throw new ArgumentNullException(nameof(supplier), "المورد لا يمكن أن يكون فارغاً");

                try
                {
                    using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                    {
                        con.Open();
                        string sql = "UPDATE Suppliers SET name = @name, phone = @phone, address = @address, details = @details WHERE id = @id";
                        using (var cmd = new SQLiteCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@name", supplier.Name);
                            cmd.Parameters.AddWithValue("@phone", supplier.Phone ?? "");
                            cmd.Parameters.AddWithValue("@address", supplier.Address ?? "");
                            cmd.Parameters.AddWithValue("@details", supplier.Details ?? "");
                            cmd.Parameters.AddWithValue("@id", supplier.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error in UpdateSupplier for supplier id: {supplier.Id}", ex);
                    throw;
                }
            });
        }

        /// <summary>
        /// حذف مورد من قاعدة البيانات
        /// </summary>
        /// <param name="id">معرف المورد المراد حذفه</param>
        /// <exception cref="UnauthorizedAccessException">إذا لم يكن المستخدم مديراً</exception>
        public void DeleteSupplier(int id)
        {
            TransactionGuard.Run("Suppliers.Delete", () =>
            {
                SessionManager.RequireAdminForDeleteIfEnabled();

                try
                {
                    using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                    {
                        con.Open();
                        string sql = "DELETE FROM Suppliers WHERE id = @id";
                        using (var cmd = new SQLiteCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error in DeleteSupplier for id: {id}", ex);
                    throw;
                }
            });
        }

        /// <summary>
        /// جلب جميع الموردين كـ DataTable (للتوافق مع الكود القديم)
        /// </summary>
        /// <returns>DataTable بجميع الموردين</returns>
        public DataTable GetAllSuppliersDataTable()
        {
            try
            {
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    string sql = "SELECT * FROM Suppliers ORDER BY name";
                    using (var cmd = new SQLiteDataAdapter(sql, con))
                    {
                        DataTable dt = new DataTable();
                        cmd.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetAllSuppliersDataTable", ex);
                throw;
            }
        }
    }
}
