// ============================================================
// الملف    : UserRepository.cs
// الغرض    : مستودع حسابات المستخدمين وصلاحيات الدخول الخاصة بهم
// ============================================================

// الملف    : UserRepository.cs
// الغرض    : إدارة المستخدمين (تسجيل الدخول/التحقق/CRUD) مع دعم ترقية كلمات المرور القديمة وصلاحيات الأدمن
// يتعامل مع: Forms/Login.cs + Forms/Signup.cs + Forms/Manager_Users.cs + Utilities/SessionManager.cs + Utilities/PasswordHasher.cs
// الجداول  : Users
// ============================================================

using System;
using System.Data;
using System.Data.SQLite;
using Sales.Database;
using Sales.Models;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Repositories
{
    public class UserRepository
    {
        /// <summary>
        /// GetAdminCount: حساب عدد المستخدمين الذين يمتلكون role=admin
        /// المدخلات : con اتصال مفتوح
        /// المخرجات : int عدد الأدمن
        /// التدفق   : UpdateUser/DeleteUser → GetAdminCount → SQLite(Users)
        /// الأخطاء  : أخطاء SQLite/تحويل نوع
        /// </summary>
        private int GetAdminCount(SQLiteConnection con)
        {
            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE lower(role) = 'admin'", con))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// IsAdminUser: التحقق هل مستخدم معين هو Admin
        /// المدخلات : con اتصال مفتوح، id معرف المستخدم
        /// المخرجات : bool
        /// التدفق   : UpdateUser/DeleteUser → IsAdminUser → SQLite(Users.role)
        /// </summary>
        private bool IsAdminUser(SQLiteConnection con, string id)
        {
            using (var cmd = new SQLiteCommand("SELECT role FROM Users WHERE id = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                var roleObj = cmd.ExecuteScalar();
                if (roleObj == null || roleObj == DBNull.Value) return false;
                return string.Equals(roleObj.ToString(), "admin", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// ColumnExists: التحقق من وجود عمود داخل جدول (يُستخدم لدعم قواعد قديمة تحتوي عمود pwd القديم)
        /// المدخلات : con اتصال مفتوح، table اسم الجدول، column اسم العمود
        /// المخرجات : bool
        /// التدفق   : ValidateUser → ColumnExists → SQLite(PRAGMA table_info)
        /// </summary>
        private bool ColumnExists(SQLiteConnection con, string table, string column)
        {
            string check = string.Format("PRAGMA table_info({0});", table);
            using (SQLiteCommand cmd = new SQLiteCommand(check, con))
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["name"].ToString().Equals(column, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Login: التحقق من بيانات الدخول (id/pwd) وإرجاع true/false
        /// المدخلات : id اسم المستخدم، pwd كلمة المرور
        /// المخرجات : bool
        /// التدفق   : Login Form → UserRepository.Login → ValidateUser → (User/null)
        /// الأخطاء  : أخطاء SQLite ستظهر كاستثناء للمستدعي
        /// </summary>
        public bool Login(string id, string pwd)
        {
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                return ValidateUser(id, pwd) != null;
            }
        }

        // جلب بيانات المستخدم بما فيها الصلاحية
        /// <summary>
        /// GetUser: جلب بيانات المستخدم الأساسية (بدون كلمة المرور) بما فيها role
        /// المدخلات : id
        /// المخرجات : User أو null
        /// التدفق   : UI/Session → GetUser → SQLite(Users)
        /// </summary>
        public User GetUser(string id)
        {
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                string sql = "SELECT id, full_name, role FROM Users WHERE id = @id";
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader["id"].ToString(),
                                FullName = reader["full_name"].ToString(),
                                Role = reader["role"] != DBNull.Value ? reader["role"].ToString() : "user"
                            };
                        }
                    }
                }
            }
            return null;
        }

        // التحقق من المستخدم وإرجاع بياناته
        /// <summary>
        /// ValidateUser: التحقق من المستخدم وإرجاع بياناته عند نجاح الدخول
        /// المدخلات : id اسم المستخدم، pwd كلمة المرور المدخلة
        /// المخرجات : User (عند النجاح) أو null (عند الفشل)
        /// التدفق   : Login → ValidateUser → (قراءة Users) → PasswordHasher.VerifyPassword
        /// ملاحظة   : يدعم ترقية كلمة المرور القديمة إذا وُجد عمود pwd بدون hash
        /// الأخطاء  : أخطاء SQLite/ترقية hash قد ترمي استثناء
        /// </summary>
        public User ValidateUser(string id, string pwd)
        {
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();

                // ── توافق خلفي: بعض القواعد القديمة كانت تحتوي عمود pwd نصي بدون hash ──
                bool hasLegacyPwd = ColumnExists(con, "Users", "pwd");
                string sql = hasLegacyPwd
                    ? "SELECT id, full_name, role, pwd_hash, pwd FROM Users WHERE id = @id"
                    : "SELECT id, full_name, role, pwd_hash FROM Users WHERE id = @id";

                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        string storedHash = reader["pwd_hash"] != DBNull.Value ? reader["pwd_hash"].ToString() : null;

                        // ── الحالة (1): لدينا hash حديث → نتحقق عبر PasswordHasher ──
                        if (!string.IsNullOrWhiteSpace(storedHash))
                        {
                            if (!PasswordHasher.VerifyPassword(storedHash, pwd))
                                return null;
                        }
                        // ── الحالة (2): لا يوجد hash ولكن يوجد عمود pwd قديم → تحقق ثم قم بالترقية إلى pwd_hash ──
                        else if (hasLegacyPwd)
                        {
                            string legacyPwd = reader["pwd"] != DBNull.Value ? reader["pwd"].ToString() : null;
                            if (!string.Equals(legacyPwd, pwd, StringComparison.Ordinal))
                                return null;

                            TransactionGuard.Run("Users.PasswordUpgrade", () =>
                            {
                                string newHash = PasswordHasher.HashPassword(pwd);
                                using (var up = new SQLiteCommand("UPDATE Users SET pwd_hash=@h WHERE id=@id", con))
                                {
                                    up.Parameters.AddWithValue("@h", newHash);
                                    up.Parameters.AddWithValue("@id", id);
                                    up.ExecuteNonQuery();
                                }
                            });
                        }
                        // ── الحالة (3): لا hash ولا legacy → فشل تحقق ──
                        else
                        {
                            return null;
                        }

                        return new User
                        {
                            Id = reader["id"].ToString(),
                            FullName = reader["full_name"].ToString(),
                            Role = reader["role"] != DBNull.Value ? reader["role"].ToString() : "user"
                        };
                    }
                }
            }
        }

        // التحقق من وجود المستخدم
        /// <summary>
        /// IsUserExists: التحقق من وجود مستخدم بمعرف محدد
        /// المدخلات : id
        /// المخرجات : bool
        /// التدفق   : Signup/Manager_Users → IsUserExists → SQLite(Users COUNT)
        /// </summary>
        public bool IsUserExists(string id)
        {
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM Users WHERE id = @id";
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // إضافة مستخدم جديد
        /// <summary>
        /// AddUser: إضافة مستخدم جديد (يتطلب Admin) مع تخزين كلمة المرور على شكل Hash
        /// المدخلات : user (Id, FullName, Pwd, Role)
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Users → AddUser → RequireAdmin → PasswordHasher.HashPassword → SQLite(Users INSERT)
        /// الأخطاء  : عدم صلاحية، تكرار id، أخطاء SQLite
        /// </summary>
        public void AddUser(User user)
        {
            TransactionGuard.Run("Users.Insert", () =>
            {
                SessionManager.RequireAdmin();
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    string sql = "INSERT INTO Users (id, pwd_hash, full_name, role) VALUES (@id, @pwd_hash, @name, @role)";
                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", user.Id);
                        cmd.Parameters.AddWithValue("@pwd_hash", PasswordHasher.HashPassword(user.Pwd ?? string.Empty));
                        cmd.Parameters.AddWithValue("@name", user.FullName);
                        // الافتراضي user إذا لم يتم تحديده
                        cmd.Parameters.AddWithValue("@role", string.IsNullOrEmpty(user.Role) ? "user" : user.Role);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }
        // جلب جميع المستخدمين
        /// <summary>
        /// GetAllUsers: جلب جميع المستخدمين (بدون كلمات مرور) لعرضهم في شاشة الإدارة
        /// المدخلات : لا يوجد
        /// المخرجات : List&lt;User&gt;
        /// التدفق   : Manager_Users → GetAllUsers → SQLite(Users)
        /// </summary>
        public System.Collections.Generic.List<User> GetAllUsers()
        {
            var list = new System.Collections.Generic.List<User>();
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                string sql = "SELECT id, full_name, role FROM Users ORDER BY full_name";
                using (var cmd = new SQLiteCommand(sql, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new User
                            {
                                Id = reader["id"].ToString(),
                                FullName = reader["full_name"].ToString(),
                                Role = reader["role"] != DBNull.Value ? reader["role"].ToString() : "user"
                            });
                        }
                    }
                }
            }
            return list;
        }

        // تعديل بيانات مستخدم
        /// <summary>
        /// UpdateUser: تعديل بيانات المستخدم (يتطلب Admin) مع حماية عدم إنزال آخر Admin
        /// المدخلات : user (Id, FullName, Pwd optional, Role)
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Users → UpdateUser → RequireAdmin → (حماية آخر Admin) → SQLite(Users UPDATE)
        /// الأخطاء  : عدم صلاحية، محاولة إنزال آخر Admin، أخطاء SQLite
        /// </summary>
        public void UpdateUser(User user)
        {
            TransactionGuard.Run("Users.Update", () =>
            {
                SessionManager.RequireAdmin();
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    // ── قاعدة أمان: لا تسمح بتحويل آخر admin إلى user حتى لا يفقد النظام صلاحية الإدارة ──
                    bool isCurrentlyAdmin = IsAdminUser(con, user.Id);
                    bool willBeAdmin = string.Equals(user.Role, "admin", StringComparison.OrdinalIgnoreCase);
                    if (isCurrentlyAdmin && !willBeAdmin && GetAdminCount(con) <= 1)
                        throw new InvalidOperationException("لا يمكن إزالة صلاحية آخر مدير. يجب أن يكون هناك مدير واحد على الأقل.");

                    if (string.IsNullOrWhiteSpace(user.Pwd))
                    {
                        string sql = "UPDATE Users SET full_name = @name, role = @role WHERE id = @id";
                        using (var cmd = new SQLiteCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@name", user.FullName);
                            cmd.Parameters.AddWithValue("@role", user.Role);
                            cmd.Parameters.AddWithValue("@id", user.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string sql = "UPDATE Users SET pwd_hash = @pwd_hash, full_name = @name, role = @role WHERE id = @id";
                        using (var cmd = new SQLiteCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@pwd_hash", PasswordHasher.HashPassword(user.Pwd));
                            cmd.Parameters.AddWithValue("@name", user.FullName);
                            cmd.Parameters.AddWithValue("@role", user.Role);
                            cmd.Parameters.AddWithValue("@id", user.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            });
        }

        // حذف مستخدم
        /// <summary>
        /// DeleteUser: حذف مستخدم (يتطلب Admin) مع منع حذف آخر Admin
        /// المدخلات : id
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Users → DeleteUser → RequireAdmin → (حماية آخر Admin) → SQLite(Users DELETE)
        /// الأخطاء  : عدم صلاحية، محاولة حذف آخر Admin
        /// </summary>
        public void DeleteUser(string id)
        {
            TransactionGuard.Run("Users.Delete", () =>
            {
                SessionManager.RequireAdminForDeleteIfEnabled();
                using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    // ── قاعدة أمان: لا تسمح بحذف آخر admin ──
                    if (IsAdminUser(con, id) && GetAdminCount(con) <= 1)
                        throw new InvalidOperationException("لا يمكن حذف آخر مدير. يجب أن يكون هناك مدير واحد على الأقل.");

                    string sql = "DELETE FROM Users WHERE id = @id";
                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  عدد الدوال    : 10                           ║
        // ║  الجدول        : Users                        ║
        // ║  يستدعيه       : Login/Signup/Manager_Users   ║
        // ║  يستدعي        : SessionManager + PasswordHasher + DatabaseInitializer.ConnectionString ║
        // ╚══════════════════════════════════════════════╝
    }
}
