// ============================================================
// الملف    : CategoryRepository.cs
// الغرض    : مستودع إدارة أصناف المنتجات وعملياتها (CRUD)
// ============================================================

// الملف    : CategoryRepository.cs
// الغرض    : إدارة أصناف المنتجات (CRUD) مع حماية عمليات التعديل/الحذف بصلاحيات الأدمن والتحقق من الارتباط بالمنتجات
// يتعامل مع: Forms/Manager_Categories.cs (شاشة إدارة الأصناف) + Forms/Add_Products.cs (ComboBox الأصناف)
// الجداول  : Categories, Products (للتحقق من الارتباط قبل الحذف)
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Sales.Models;
using Sales.Database;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Repositories
{
    // ============================================
    // مخزن بيانات الأصناف (Category Repository)
    // المسؤول عن جميع عمليات قاعدة البيانات الخاصة بالأصناف
    // ============================================
    public class CategoryRepository
    {
        // دالة لجلب جميع الأصناف (لملء الـ ComboBox)
        /// <summary>
        /// GetAllCategories: جلب جميع الأصناف مرتبة بالاسم (لا يتطلب صلاحيات)
        /// المدخلات : لا يوجد
        /// المخرجات : List&lt;Category&gt; قائمة الأصناف
        /// التدفق   : Add_Products/Manager_Categories → GetAllCategories → SQLite(Categories) → Category(Model)
        /// الأخطاء  : أخطاء اتصال/SQL/تحويل أنواع
        /// </summary>
        public List<Category> GetAllCategories()
        {
            List<Category> list = new List<Category>();

            using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                // ── استعلام: كل الأصناف مرتبة أبجدياً ──
                string sql = "SELECT * FROM Categories ORDER BY name";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                using (SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new Category
                        {
                            Id = Convert.ToInt32(dr["id"]),
                            Name = dr["name"].ToString()
                        });
                    }
                }
            }
            return list;
        }



        // دالة لإضافة صنف جديد
        /// <summary>
        /// AddCategory: إضافة صنف جديد (يتطلب Admin)
        /// المدخلات : category كائن الصنف (Name)
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Categories → AddCategory → SessionManager.RequireAdmin → SQLite(Categories INSERT)
        /// الأخطاء  : عدم صلاحية، أو تكرار اسم (UNIQUE) أو أخطاء SQLite
        /// </summary>
        public void AddCategory(Category category)
        {
            TransactionGuard.Run("Categories.Insert", () =>
            {
                // ── حماية: السماح بالإضافة فقط للمسؤول ──
                SessionManager.RequireAdmin();
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    string sql = "INSERT INTO Categories (name) VALUES (@name)";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@name", category.Name);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        // دالة لتعديل صنف
        /// <summary>
        /// UpdateCategory: تعديل اسم صنف موجود (يتطلب Admin)
        /// المدخلات : category (Id + Name)
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Categories → UpdateCategory → SessionManager.RequireAdmin → SQLite(Categories UPDATE)
        /// الأخطاء  : عدم صلاحية، أو تكرار اسم، أو أخطاء SQLite
        /// </summary>
        public void UpdateCategory(Category category)
        {
            TransactionGuard.Run("Categories.Update", () =>
            {
                // ── حماية: السماح بالتعديل فقط للمسؤول ──
                SessionManager.RequireAdmin();
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    string sql = "UPDATE Categories SET name=@name WHERE id=@id";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@name", category.Name);
                        cmd.Parameters.AddWithValue("@id", category.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        // دالة لحذف صنف
        /// <summary>
        /// DeleteCategory: حذف صنف (يتطلب Admin) مع منع الحذف إذا كان هناك منتجات مرتبطة
        /// المدخلات : id رقم الصنف
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Categories → DeleteCategory → RequireAdmin → (Check Products) → SQLite(Categories DELETE)
        /// الأخطاء  : عدم صلاحية، أو وجود منتجات مرتبطة (InvalidOperationException)، أو أخطاء SQLite
        /// </summary>
        public void DeleteCategory(int id)
        {
            TransactionGuard.Run("Categories.Delete", () =>
            {
                // ── حماية: السماح بالحذف فقط للمسؤول ──
                SessionManager.RequireAdminForDeleteIfEnabled();
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    // التحقق من وجود منتجات مرتبطة
                    // الهدف: منع كسر مرجعية Products.category_id أو فقدان تصنيف منتجات موجودة
                    string checkQuery = "SELECT COUNT(*) FROM Products WHERE category_id = @id";
                    using (var checkCmd = new SQLiteCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                            throw new InvalidOperationException(string.Format("لا يمكن حذف هذا الصنف لأنه مرتبط بـ {0} منتج", count));
                    }

                    string sql = "DELETE FROM Categories WHERE id=@id";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  عدد الدوال    : 4                            ║
        // ║  الجداول       : Categories، Products         ║
        // ║  يستدعيه       : Manager_Categories.cs، Add_Products.cs ║
        // ║  يستدعي        : DatabaseInitializer.ConnectionString + SessionManager ║
        // ╚══════════════════════════════════════════════╝
    }
}
