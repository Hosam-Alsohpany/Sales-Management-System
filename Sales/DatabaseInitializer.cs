// ============================================================
// الملف    : DatabaseInitializer.cs
// الغرض    : تهيئة قاعدة بيانات SQLite (إنشاء الملف + إنشاء الجداول + تنفيذ الترقيات/المهاجرات)
// يتعامل مع: Program.cs (يستدعي Initialize) / Repositories (تستخدم ConnectionString)
// الجداول  : Categories, Users, Customers, Suppliers, Products, ProductDomains, Units, ProductUnits,
//             ProductUnitBarcodes, Purchases, Orders, Order_Details, Payments, StockHistory,
//             StockHistoryBatches, OrderAuditLog, AppSettings, BarcodeProfiles, PosHotkeys, ...
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using Sales.Utilities;
using Sales.Database.Migrations;
using Sales.Database.Triggers;

namespace Sales.Database
{
    public static class DatabaseInitializer
    {
        // ===================== مسار قاعدة البيانات =====================
        public static string DbPath = GetDatabasePath();

        // ===================== نص الاتصال (Connection String) =====================
        // إضافة foreign keys=true لتفعيل القيود بين الجداول (العلاقات)
        public static string ConnectionString = "Data Source=" + DbPath + ";Version=3;Foreign Keys=True;";

        /// <summary>
        /// GetDatabasePath: تحديد المسار القياسي لملف قاعدة البيانات داخل AppData
        /// المدخلات : لا يوجد
        /// المخرجات : string مسار ملف DataBase.db
        /// التدفق   : Program → DatabaseInitializer.GetDatabasePath → (إنشاء مجلد SalesApp إن لم يوجد)
        /// الأخطاء  : أخطاء صلاحيات/إنشاء مجلد/مسار غير صالح (قد تظهر لاحقاً عند فتح الاتصال)
        /// </summary>
        private static string GetDatabasePath()
        {
            // ── جلب مسار AppData الخاص بالمستخدم الحالي ─────────────────────────
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // ── إنشاء مجلد ثابت للتطبيق داخل AppData لتخزين قاعدة البيانات ─────
            string appFolder = Path.Combine(appDataPath, "SalesApp");

            // ── التأكد من وجود المجلد قبل استخدامه ─────────────────────────────
            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);

            // ── إرجاع المسار النهائي لملف قاعدة البيانات ──────────────────────
            return Path.Combine(appFolder, "DataBase.db");
        }

        /// <summary>
        /// IsFirstRun: التحقق هل النظام يعمل لأول مرة (بناءً على وجود مستخدمين)
        /// المدخلات : لا يوجد
        /// المخرجات : bool (true إذا لا يوجد أي سجل في جدول Users)
        /// التدفق   : Login/Program → DatabaseInitializer.IsFirstRun → SQLite(Users) → bool
        /// الأخطاء  : فشل الاتصال/عدم وجود جدول Users بعد (في هذه الحالة نعتبرها أول تشغيل)
        /// </summary>
        public static bool IsFirstRun()
        {
            try
            {
                // ── فتح اتصال بقاعدة البيانات ثم تنفيذ استعلام COUNT على Users ──
                using (var con = new SQLiteConnection(ConnectionString))
                {
                    con.Open();

                    // ── الاستعلام: كم مستخدم موجود؟ ────────────────────────────
                    using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Users", con))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count == 0;
                    }
                }
            }
            catch
            {
                // ── في حال أي مشكلة (قاعدة غير موجودة/جدول غير موجود/اتصال فشل)
                // نعتبرها أول تشغيل حتى يتم إنشاء القاعدة عبر Initialize().
                return true;
            }
        }

        public static bool IsInitialized()
        {
            try
            {
                using (var con = new SQLiteConnection(ConnectionString))
                {
                    con.Open();

                    CreateAppSettingsTable(con);

                    using (var cmd = new SQLiteCommand("SELECT value FROM AppSettings WHERE key='IsInitialized' LIMIT 1", con))
                    {
                        object v = cmd.ExecuteScalar();
                        if (v == null || v == DBNull.Value)
                            return false;

                        string s = v.ToString();
                        return string.Equals(s, "1", StringComparison.Ordinal) ||
                               string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(s, "yes", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // ===================== نقطة البداية =====================
        /// <summary>
        /// Initialize: تهيئة قاعدة البيانات (إنشاء ملف DB + إنشاء الجداول + تنفيذ الترقيات)
        /// المدخلات : لا يوجد
        /// المخرجات : لا يوجد
        /// التدفق   : Program → DatabaseInitializer.Initialize → SQLite(Create Tables + Migration)
        /// الأخطاء  : فشل إنشاء ملف DB، فشل فتح الاتصال، أخطاء SQL أثناء CREATE/ALTER
        /// </summary>
        public static void Initialize()
        {
            try
            {
                bool isNewDatabase = false;

                // 1️⃣ إنشاء ملف القاعدة إن لم يكن موجوداً
                if (!File.Exists(DbPath))
                {
                    SQLiteConnection.CreateFile(DbPath);
                    isNewDatabase = true;
                }

                // ── فتح اتصال واحد فقط وإنشاء كل الجداول داخل نفس الاتصال ──────
                using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
                {
                    con.Open();

                    try
                    {
                        Execute("PRAGMA foreign_keys = ON;", con);
                    }
                    catch
                    {
                    }

                    try
                    {
                        // Improve concurrency and reduce locking issues (safe fallback if not supported)
                        Execute("PRAGMA journal_mode = WAL;", con);
                    }
                    catch
                    {
                    }

                    if (!isNewDatabase)
                    {
                        // Verify that critical FKs exist (legacy DBs may have been created without them)
                        try
                        {
                            bool okOrderDetailsFk = false;
                            using (var cmd = new SQLiteCommand("PRAGMA foreign_key_list('Order_Details');", con))
                            using (var r = cmd.ExecuteReader())
                            {
                                while (r.Read()) { okOrderDetailsFk = true; break; }
                            }

                            bool okPaymentsFk = false;
                            using (var cmd = new SQLiteCommand("PRAGMA foreign_key_list('Payments');", con))
                            using (var r = cmd.ExecuteReader())
                            {
                                while (r.Read()) { okPaymentsFk = true; break; }
                            }

                            if (!okOrderDetailsFk || !okPaymentsFk)
                            {
                                MessageBox.Show(
                                    "تحذير: قاعدة البيانات الحالية قديمة أو بدون قيود Foreign Keys لبعض الجداول.\n" +
                                    "قد يؤدي هذا إلى بيانات orphan عند الحذف.\n" +
                                    "يُفضّل ترحيل/إعادة إنشاء قاعدة البيانات لضمان سلامة العلاقات.",
                                    "Database Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }
                        }
                        catch
                        {
                        }
                    }

                    // Remove legacy tables (kept only for backward compatibility)
                    try
                    {
                        Execute("DROP TABLE IF EXISTS " + "Product" + "Barcodes" + ";", con);
                        Execute("DROP TABLE IF EXISTS " + "Set" + "tings" + ";", con);
                    }
                    catch
                    {
                    }

                    // 2️⃣ إنشاء الجداول (بالترتيب الصحيح لتجنب مشاكل العلاقات)

                    // ── Categories: جدول مرجعي للأصناف (يُستخدم في Products.category_id) ──
                    CreateCategoriesTable(con); // [جديد] الأصناف

                    // ── Users: حسابات الدخول والصلاحيات ─────────────────────────
                    CreateUsersTable(con);      // المستخدمين

                    // ── AppSettings: إعدادات النظام القابلة للتخصيص ─────────────
                    CreateAppSettingsTable(con);

                    // ── Customers: عملاء النظام (يُستخدم في Orders.customer_id) ──
                    CreateCustomersTable(con);  // العملاء

                    // ── Suppliers: موردين النظام (يُستخدم لاحقاً في المشتريات/الفواتير) ──
                    CreateSuppliersTable(con);

                    // ── Products: المنتجات + ربط اختياري بالصنف (category_id) ─────
                    CreateProductsTable(con);   // المنتجات (يعتمد على الأصناف)

                    CreateProductDomainsTable(con);

                    CreateUnitsTable(con);

                    CreateProductUnitsTable(con);

                    CreateProductUnitBarcodesTable(con);

                    CreatePurchasesTables(con);

                    // ── Orders: رأس فاتورة البيع (يرتبط بالعميل) ────────────────
                    CreateOrdersTable(con);     // الطلبات (يعتمد على العملاء)

                    // ── Order_Details: بنود الفاتورة (ترتبط بـ Orders و Products) ─
                    CreateOrderDetailsTable(con); // التفاصيل (يعتمد على الطلبات والمنتجات)

                    // ── Payments: دفعات على الفواتير (ترتبط بالطلب) ──────────────
                    CreatePaymentsTable(con);     // [جديد] المدفوعات

                    // ── StockHistory: سجل حركات المخزون (يرتبط بالمنتج) ─────────
                    CreateStockHistoryTable(con); // [جديد] سجل المخزون

                    CreateOrderAuditLogTable(con);

                    // 3️⃣ تحديثات مستقبلية (Migration)

                    // ── UpgradeDatabase: إضافة أعمدة/ترقيات للنسخ القديمة بدون حذف بيانات ──
                    UpgradeDatabase(con);
                }

                if (isNewDatabase)
                {
                    MessageBox.Show("تم إنشاء قاعدة البيانات الجديدة وهيكلتها بنجاح!", "قاعدة البيانات");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تهيئة قاعدة البيانات:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===================== 1. جدول الأصناف (Categories) [جديد] =====================
        /// <summary>
        /// CreateCategoriesTable: إنشاء جدول الأصناف وإضافة صنف افتراضي
        /// المدخلات : SQLiteConnection اتصال مفتوح
        /// المخرجات : لا يوجد
        /// التدفق   : Initialize → CreateCategoriesTable → SQLite(Categories)
        /// الأخطاء  : أخطاء SQL (صلاحيات/ملف DB تالف/صيغة SQL غير صحيحة)
        /// </summary>
        private static void CreateCategoriesTable(SQLiteConnection con)
        {
            // هذا الجدول سيظهر في الواجهة كـ ComboBox لاختيار نوع المنتج

            // ── إنشاء جدول Categories (مرجعي) ──────────────────────────────────
            string sql = @"
            CREATE TABLE IF NOT EXISTS Categories (
                id INTEGER PRIMARY KEY AUTOINCREMENT,   -- رقم الصنف
                name TEXT NOT NULL UNIQUE               -- اسم الصنف (مثل: مشروبات، مأكولات)
            );";
            Execute(sql, con);

            // إضافة صنف افتراضي "عام"

            // ── INSERT OR IGNORE: لو الصنف موجود لا تكرر الإدخال ───────────────
            string initData = "INSERT OR IGNORE INTO Categories (id, name) VALUES (1, 'عام');";
            Execute(initData, con);
        }

        /// <summary>
        /// CreateUsersTable: إنشاء جدول المستخدمين (الحسابات والصلاحيات)
        /// المدخلات : SQLiteConnection اتصال مفتوح
        /// المخرجات : لا يوجد
        /// التدفق   : Initialize → CreateUsersTable → SQLite(Users)
        /// الأخطاء  : أخطاء SQL أثناء CREATE TABLE
        /// </summary>
        private static void CreateUsersTable(SQLiteConnection con)
        {
            // ── جدول Users: مفتاحه الأساسي id = اسم المستخدم (TEXT) ────────────
            string sql = @"
            CREATE TABLE IF NOT EXISTS Users (
                id TEXT PRIMARY KEY,        -- اسم المستخدم
                pwd_hash TEXT NOT NULL,              -- كلمة المرور (Hash)
                full_name TEXT NOT NULL DEFAULT '',  -- الاسم الكامل
                role TEXT NOT NULL DEFAULT 'user',   -- [جديد] الصلاحية (admin, user)
                CONSTRAINT ck_Users_Role CHECK (role IN ('admin','user'))
            );";
            Execute(sql, con);
        }

        private static void CreateProductDomainsTable(SQLiteConnection con)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS ProductDomains (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                product_id INTEGER NOT NULL UNIQUE,
                domain_type TEXT NOT NULL,
                config_json TEXT NOT NULL DEFAULT '{""version"":1}',
                last_validated_at TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT,
                FOREIGN KEY(product_id) REFERENCES Products(id) ON DELETE CASCADE,
                CONSTRAINT ck_ProductDomains_DomainType CHECK (domain_type IN ('GENERAL','WEIGHTED','FASHION','INTERNAL'))
            );";

            Execute(sql, con);
            try { Execute("CREATE INDEX IF NOT EXISTS idx_ProductDomains_ProductId ON ProductDomains(product_id);", con); } catch { }
        }

        private static void CreateAppSettingsTable(SQLiteConnection con)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS AppSettings (
                key TEXT PRIMARY KEY,
                value TEXT NOT NULL DEFAULT ''
            );";
            Execute(sql, con);
        }

        /// <summary>
        /// CreateCustomersTable: إنشاء جدول العملاء (Customers)
        /// المدخلات : SQLiteConnection اتصال مفتوح
        /// المخرجات : لا يوجد
        /// التدفق   : Initialize → CreateCustomersTable → SQLite(Customers)
        /// الأخطاء  : أخطاء SQL أثناء CREATE TABLE
        /// </summary>
        private static void CreateCustomersTable(SQLiteConnection con)
        {
            // ── جدول Customers: بيانات العميل الأساسية المستخدمة في الفواتير ──
            string sql = @"
            CREATE TABLE IF NOT EXISTS Customers (
                id INTEGER PRIMARY KEY AUTOINCREMENT,  -- رقم العميل
                name TEXT NOT NULL,                    -- اسم العميل
                tel TEXT,                              -- الهاتف
                details TEXT,                          -- تفاصيل العميل
                address TEXT                           -- عنوان العميل
            );";
            Execute(sql, con);
        }

        private static void CreateSuppliersTable(SQLiteConnection con)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS Suppliers (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                phone TEXT,
                address TEXT,
                details TEXT
            );";
            Execute(sql, con);
        }

        // ===================== تعديل جدول المنتجات =====================
        /// <summary>
        /// CreateProductsTable: إنشاء جدول المنتجات وربطه اختيارياً بجدول الأصناف
        /// المدخلات : SQLiteConnection اتصال مفتوح
        /// المخرجات : لا يوجد
        /// التدفق   : Initialize → CreateProductsTable → SQLite(Products + FK Categories)
        /// الأخطاء  : أخطاء SQL أثناء CREATE TABLE أو تعريف الـ FK
        /// </summary>
        private static void CreateProductsTable(SQLiteConnection con)
        {
            // أضفنا category_id لربط المنتج بالصنف

            // ── FK: category_id → Categories(id)
            // ON DELETE SET NULL: إذا حذفنا صنفاً لا نحذف المنتج، فقط نصفر الربط ──
            string sql = @"
            CREATE TABLE IF NOT EXISTS Products (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                sku TEXT,
                label TEXT NOT NULL,
                qty_scaled INTEGER NOT NULL DEFAULT 0,
                qty_scale_pow10 INTEGER NOT NULL DEFAULT 0,
                product_type TEXT NOT NULL DEFAULT 'physical',
                price REAL NOT NULL DEFAULT 0,
                cost_price REAL NOT NULL DEFAULT 0,
                min_qty REAL NOT NULL DEFAULT 0,
                category_id INTEGER,
                note TEXT,
                created_by TEXT,
                expiry_date TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT,
                image_path TEXT,
                FOREIGN KEY(category_id) REFERENCES Categories(id) ON DELETE SET NULL,
                CONSTRAINT ck_Products_ProductType CHECK (product_type IN ('physical','weighted','service')),
                CONSTRAINT ck_Products_QtyScalePow10 CHECK (qty_scale_pow10 >= 0 AND qty_scale_pow10 <= 6)
            );";
            Execute(sql, con);
        }


        /// <summary>
        /// CreateOrdersTable: إنشاء جدول رأس الفاتورة (Orders) وربطه بالعميل
        /// المدخلات : SQLiteConnection اتصال مفتوح
        /// المخرجات : لا يوجد
        /// التدفق   : Initialize → CreateOrdersTable → SQLite(Orders + FK Customers)
        /// الأخطاء  : أخطاء SQL أثناء CREATE TABLE
        /// </summary>
        private static void CreateOrdersTable(SQLiteConnection con)
        {
            // ── Orders: رأس الفاتورة (بيع) ────────────────────────────────────
            // FK: customer_id → Customers(id)
            // ON DELETE SET NULL: حذف العميل لا يحذف فواتيره، فقط يفصل الربط ──
            string sql = @"
            CREATE TABLE IF NOT EXISTS Orders (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                request_id TEXT,
                order_date TEXT NOT NULL,
                customer_id INTEGER,
                total REAL NOT NULL DEFAULT 0,
                discount REAL NOT NULL DEFAULT 0,
                tax_amount REAL NOT NULL DEFAULT 0,
                note TEXT,
                created_by TEXT,
                FOREIGN KEY(customer_id) REFERENCES Customers(id) ON DELETE SET NULL
            );";
            Execute(sql, con);

            Execute("CREATE UNIQUE INDEX IF NOT EXISTS idx_orders_request_id_unique ON Orders(request_id);", con);
            try { Execute("CREATE INDEX IF NOT EXISTS idx_Orders_OrderDate ON Orders(order_date);", con); } catch { }
        }

        /// <summary>
        /// CreateOrderDetailsTable: إنشاء جدول بنود الفاتورة وربطه بالفاتورة والمنتج
        /// المدخلات : SQLiteConnection اتصال مفتوح
        /// المخرجات : لا يوجد
        /// التدفق   : Initialize → CreateOrderDetailsTable → SQLite(Order_Details + FK Orders/Products)
        /// الأخطاء  : أخطاء SQL أثناء CREATE TABLE أو تعريف القيود
        /// </summary>
        private static void CreateOrderDetailsTable(SQLiteConnection con)
        {
            // أضفنا قيود Foreign Keys لضمان عدم وجود تفاصيل لطلب محذوف أو منتج محذوف

            // ── FK(id_order) → Orders(id) ON DELETE CASCADE
            // إذا حُذفت الفاتورة نحذف تلقائياً تفاصيلها (أمن بيانات) ─────────────
            // ── FK(id_product) → Products(id) ON DELETE RESTRICT
            // يمنع حذف منتج مرتبط بتفاصيل فواتير (حتى لا نفقد تاريخ المبيعات) ───
            string sql = @"
            CREATE TABLE IF NOT EXISTS Order_Details (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                id_order INTEGER NOT NULL,
                id_product INTEGER NOT NULL,
                price REAL NOT NULL,
                total REAL NOT NULL,
                product_unit_id INTEGER,
                unit_name_snapshot TEXT,
                factor_snapshot REAL,
                qty_unit REAL,
                base_qty REAL NOT NULL DEFAULT 0,
                cost_price REAL,
                qty_scaled_snapshot INTEGER,
                qty_scale_pow10_snapshot INTEGER,
                product_name_snapshot TEXT,
                barcode_snapshot TEXT,
                sell_price_snapshot REAL,
                FOREIGN KEY(id_order) REFERENCES Orders(id) ON DELETE CASCADE,
                FOREIGN KEY(id_product) REFERENCES Products(id) ON DELETE RESTRICT
            );";
            Execute(sql, con);
        }

        private static void CreateUnitsTable(SQLiteConnection con)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS Units (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE
            );";
            Execute(sql, con);
        }

        private static void CreateProductUnitsTable(SQLiteConnection con)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS ProductUnits (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                product_id INTEGER NOT NULL,
                unit_id INTEGER NOT NULL,
                factor REAL NOT NULL,
                sell_price REAL NOT NULL DEFAULT 0,
                cost_price REAL NOT NULL DEFAULT 0,
                display_order INTEGER NOT NULL DEFAULT 1,
                FOREIGN KEY(product_id) REFERENCES Products(id) ON DELETE CASCADE,
                FOREIGN KEY(unit_id) REFERENCES Units(id) ON DELETE RESTRICT,
                CONSTRAINT ck_ProductUnits_Factor_Positive CHECK (factor > 0)
            );";
            Execute(sql, con);

            Execute("CREATE UNIQUE INDEX IF NOT EXISTS idx_ProductUnits_Product_Unit_unique ON ProductUnits(product_id, unit_id);", con);
            Execute("CREATE INDEX IF NOT EXISTS idx_ProductUnits_ProductId ON ProductUnits(product_id);", con);

            // Enforce: a product must have factor=1 before inserting other factors
            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_ProductUnits_RequireBaseBeforeOther_Insert
                BEFORE INSERT ON ProductUnits
                FOR EACH ROW
                WHEN NEW.factor <> 1
                BEGIN
                    SELECT CASE
                        WHEN (SELECT COUNT(1) FROM ProductUnits pu WHERE pu.product_id = NEW.product_id AND pu.factor = 1) = 0
                        THEN RAISE(ABORT, 'Product must have base unit (factor=1) before adding other units')
                    END;
                END;", con);

            // Enforce: do not delete last base unit (factor=1)
            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_ProductUnits_PreventDeleteLastBase
                BEFORE DELETE ON ProductUnits
                FOR EACH ROW
                WHEN OLD.factor = 1
                BEGIN
                    SELECT CASE
                        WHEN (SELECT COUNT(1) FROM ProductUnits pu WHERE pu.product_id = OLD.product_id AND pu.factor = 1) <= 1
                        THEN RAISE(ABORT, 'Cannot delete last base unit (factor=1) for product')
                    END;
                END;", con);

            // Enforce: prevent updating base unit away from factor=1 if it is the last base
            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_ProductUnits_PreventUpdateLastBase
                BEFORE UPDATE OF factor, product_id ON ProductUnits
                FOR EACH ROW
                WHEN OLD.factor = 1 AND NEW.factor <> 1
                BEGIN
                    SELECT CASE
                        WHEN (SELECT COUNT(1) FROM ProductUnits pu WHERE pu.product_id = OLD.product_id AND pu.factor = 1 AND pu.id <> OLD.id) = 0
                        THEN RAISE(ABORT, 'Cannot change factor=1 for the last base unit of product')
                    END;
                END;", con);
        }

        private static void CreateSystemViolationLogTable(SQLiteConnection con)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS SystemViolationLog (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                operation_type TEXT,
                caller TEXT,
                stack_trace TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP
            );";
            Execute(sql, con);
            Execute("CREATE INDEX IF NOT EXISTS idx_SystemViolationLog_created_at ON SystemViolationLog(created_at);", con);
        }

        private static void CreateProductUnitBarcodesTable(SQLiteConnection con)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS ProductUnitBarcodes (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                product_unit_id INTEGER NOT NULL,
                barcode_type TEXT NOT NULL DEFAULT 'NORMAL',
                barcode TEXT NOT NULL,
                is_default INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY(product_unit_id) REFERENCES ProductUnits(id) ON DELETE CASCADE
            );";
            Execute(sql, con);

            Execute("CREATE UNIQUE INDEX IF NOT EXISTS idx_ProductUnitBarcodes_BarcodeTypeBarcode_unique ON ProductUnitBarcodes(barcode_type, barcode);", con);
            Execute("CREATE INDEX IF NOT EXISTS idx_pub_barcode ON ProductUnitBarcodes(barcode);", con);
            Execute("CREATE INDEX IF NOT EXISTS idx_pub_barcode_type ON ProductUnitBarcodes(barcode_type);", con);
            Execute("CREATE INDEX IF NOT EXISTS idx_ProductUnitBarcodes_ProductUnitId ON ProductUnitBarcodes(product_unit_id);", con);

            // One default barcode per product unit (via triggers for compatibility)
            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_ProductUnitBarcodes_OneDefault_Insert
                BEFORE INSERT ON ProductUnitBarcodes
                FOR EACH ROW
                WHEN NEW.is_default = 1
                BEGIN
                    UPDATE ProductUnitBarcodes
                    SET is_default = 0
                    WHERE product_unit_id = NEW.product_unit_id;
                END;", con);

            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_ProductUnitBarcodes_OneDefault_Update
                BEFORE UPDATE OF is_default, product_unit_id ON ProductUnitBarcodes
                FOR EACH ROW
                WHEN NEW.is_default = 1
                BEGIN
                    UPDATE ProductUnitBarcodes
                    SET is_default = 0
                    WHERE product_unit_id = NEW.product_unit_id AND id <> NEW.id;
                END;", con);
        }

        private static void CreatePurchasesTables(SQLiteConnection con)
        {
            string sqlPurchases = @"
            CREATE TABLE IF NOT EXISTS Purchases (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                supplier_id INTEGER,
                purchase_date TEXT NOT NULL,
                note TEXT,
                created_by TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY(supplier_id) REFERENCES Suppliers(id) ON DELETE SET NULL
            );";
            Execute(sqlPurchases, con);

            string sqlDetails = @"
            CREATE TABLE IF NOT EXISTS PurchaseDetails (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                purchase_id INTEGER NOT NULL,
                product_id INTEGER NOT NULL,
                product_unit_id INTEGER,
                unit_name_snapshot TEXT,
                factor_snapshot REAL,
                qty REAL NOT NULL,
                base_qty REAL NOT NULL,
                cost_price REAL NOT NULL DEFAULT 0,
                sell_price REAL NOT NULL DEFAULT 0,
                expiry_date TEXT,
                note TEXT,
                qty_scaled_snapshot INTEGER,
                qty_scale_pow10_snapshot INTEGER,
                FOREIGN KEY(purchase_id) REFERENCES Purchases(id) ON DELETE CASCADE,
                FOREIGN KEY(product_id) REFERENCES Products(id) ON DELETE RESTRICT,
                FOREIGN KEY(product_unit_id) REFERENCES ProductUnits(id) ON DELETE SET NULL
            );";
            Execute(sqlDetails, con);

            Execute("CREATE INDEX IF NOT EXISTS idx_PurchaseDetails_PurchaseId ON PurchaseDetails(purchase_id);", con);
            Execute("CREATE INDEX IF NOT EXISTS idx_PurchaseDetails_ProductId ON PurchaseDetails(product_id);", con);
        }

        // ===================== [جديد] جدول المدفوعات (Payments) =====================
        /// <summary>
        /// CreatePaymentsTable: إنشاء جدول دفعات الفواتير وربطه بالطلبات
        /// المدخلات : SQLiteConnection اتصال مفتوح
        /// المخرجات : لا يوجد
        /// التدفق   : Initialize → CreatePaymentsTable → SQLite(Payments + FK Orders)
        /// الأخطاء  : أخطاء SQL أثناء CREATE TABLE
        /// </summary>
        private static void CreatePaymentsTable(SQLiteConnection con)
        {
            // ── Payments: كل سجل يمثل دفعة على فاتورة (Orders) ─────────────────
            // FK(order_id) → Orders(id) ON DELETE CASCADE
            // إذا حُذفت الفاتورة تُحذف الدفعات التابعة لها تلقائياً ─────────────
            string sql = @"
            CREATE TABLE IF NOT EXISTS Payments (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                order_id INTEGER NOT NULL,
                amount REAL NOT NULL,
                payment_date TEXT DEFAULT CURRENT_TIMESTAMP,
                user_name TEXT,
                FOREIGN KEY(order_id) REFERENCES Orders(id) ON DELETE CASCADE
            );";
            Execute(sql, con);

            // Enforce: SUM(Payments.amount) must not exceed Orders.total (DB-level invariant)
            PaymentTriggers.EnsureCreated(con);
        }

        private static void CreatePaymentOverpaymentTriggers(SQLiteConnection con)
        {
            PaymentTriggers.EnsureCreated(con);
        }

        // ===================== [جديد] جدول سجل المخزون (StockHistory) =====================
        /// <summary>
        /// CreateStockHistoryTable: إنشاء جدول سجل حركة المخزون وربطه بالمنتج
        /// المدخلات : SQLiteConnection اتصال مفتوح
        /// المخرجات : لا يوجد
        /// التدفق   : Initialize → CreateStockHistoryTable → SQLite(StockHistory + FK Products)
        /// الأخطاء  : أخطاء SQL أثناء CREATE TABLE
        /// </summary>
        private static void CreateStockHistoryTable(SQLiteConnection con)
        {
            // ── StockHistory: تتبع الزيادة/النقصان في المخزون (audit trail) ─────
            // FK(product_id) → Products(id) ON DELETE CASCADE
            // إذا حذفنا المنتج (نادر/إداري) نحذف سجل حركته معه ─────────────────
            string sql = @"
            CREATE TABLE IF NOT EXISTS StockHistory (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                product_id INTEGER NOT NULL,
                qty_change INTEGER NOT NULL,
                qty_change_scaled INTEGER,
                qty_scale_pow10 INTEGER NOT NULL DEFAULT 0,
                reason TEXT,
                change_date TEXT DEFAULT CURRENT_TIMESTAMP,
                user_name TEXT,
                FOREIGN KEY(product_id) REFERENCES Products(id) ON DELETE CASCADE
            );";
            Execute(sql, con);
        }

        // ===================== دوال مساعدة =====================
        /// <summary>
        /// Execute: تنفيذ أمر SQL لا يُرجع نتائج (CREATE/ALTER/INSERT/UPDATE/DELETE)
        /// المدخلات : sql نص الأمر، con اتصال مفتوح
        /// المخرجات : لا يوجد
        /// التدفق   : CreateXxxTable/UpgradeDatabase → Execute → SQLite
        /// الأخطاء  : أخطاء SQLSyntax/قيود FK/ملف DB مقفل
        /// </summary>
        private static void Execute(string sql, SQLiteConnection con)
        {
            // ── SQLiteCommand: يمثل أمر SQL سيتم تنفيذه على الاتصال الحالي ─────
            using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// UpgradeDatabase: ترقية قاعدة البيانات للنسخ القديمة بإضافة أعمدة مفقودة
        /// المدخلات : SQLiteConnection اتصال مفتوح
        /// المخرجات : لا يوجد
        /// التدفق   : Initialize → UpgradeDatabase → AddColumnIfNotExists → SQLite(PRAGMA/ALTER)
        /// الأخطاء  : فشل ALTER TABLE، أو جدول غير موجود، أو قاعدة بيانات للقراءة فقط
        /// </summary>
        private static void UpgradeDatabase(SQLiteConnection con)
        {
            // هنا نضع التعديلات المستقبلية للمستخدمين القدامى
            // مثلاً: إضافة عمود الفئة إذا لم يكن موجوداً

            // Remove legacy tables (kept only for backward compatibility)
            try
            {
                Execute("DROP TABLE IF EXISTS " + "Product" + "Barcodes" + ";", con);
                Execute("DROP TABLE IF EXISTS " + "Set" + "tings" + ";", con);
            }
            catch
            {
            }

            // ── إضافة category_id للمنتجات إن كانت قاعدة قديمة لا تملكه ─────────
            AddColumnIfNotExists(con, "Products", "category_id", "INTEGER REFERENCES Categories(id)");

            AddColumnIfNotExists(con, "Orders", "request_id", "TEXT");
            AddColumnIfNotExists(con, "Orders", "discount", "REAL NOT NULL DEFAULT 0");
            try
            {
                Execute("CREATE UNIQUE INDEX IF NOT EXISTS idx_orders_request_id_unique ON Orders(request_id);", con);
            }
            catch
            {
            }

            try { MigrateOrdersDiscountFromNotes(con); } catch { }

            try { CreatePaymentOverpaymentTriggers(con); } catch { }

            AddColumnIfNotExists(con, "StockHistory", "qty_change_scaled", "INTEGER");
            AddColumnIfNotExists(con, "StockHistory", "qty_scale_pow10", "INTEGER NOT NULL DEFAULT 0");
            try
            {
                Execute(@"UPDATE StockHistory
                         SET qty_change_scaled = qty_change,
                             qty_scale_pow10 = COALESCE((
                                 SELECT qty_scale_pow10 FROM Products p WHERE p.id = StockHistory.product_id
                             ), 0)
                         WHERE qty_change_scaled IS NULL;", con);
            }
            catch { }

            try { CreateProductUnitPriceSyncTriggers(con); } catch { }

            try { DropLegacyWarehouses(con); } catch { }

            // [إصلاح هام] إضافة عمود التاريخ إذا لم يكن موجوداً

            // ── دعم created_at للمنتجات ─────────────────
            AddColumnIfNotExists(con, "Products", "created_at", "TEXT DEFAULT CURRENT_TIMESTAMP");
            AddColumnIfNotExists(con, "Products", "sku", "TEXT");
            AddColumnIfNotExists(con, "Order_Details", "qty_scaled_snapshot", "INTEGER");
            AddColumnIfNotExists(con, "Order_Details", "qty_scale_pow10_snapshot", "INTEGER");
            AddColumnIfNotExists(con, "Order_Details", "product_name_snapshot", "TEXT");
            AddColumnIfNotExists(con, "Order_Details", "barcode_snapshot", "TEXT");
            AddColumnIfNotExists(con, "Order_Details", "sell_price_snapshot", "REAL");

            AddColumnIfNotExists(con, "PurchaseDetails", "qty_scaled_snapshot", "INTEGER");
            AddColumnIfNotExists(con, "PurchaseDetails", "qty_scale_pow10_snapshot", "INTEGER");

            AddColumnIfNotExists(con, "Products", "cost_price", "REAL NOT NULL DEFAULT 0");
            AddColumnIfNotExists(con, "Products", "min_qty", "REAL NOT NULL DEFAULT 0");
            AddColumnIfNotExists(con, "Products", "expiry_date", "TEXT");
            AddColumnIfNotExists(con, "Products", "created_at", "TEXT DEFAULT CURRENT_TIMESTAMP");
            AddColumnIfNotExists(con, "Products", "updated_at", "TEXT");

            AddColumnIfNotExists(con, "Products", "qty_scaled", "INTEGER NOT NULL DEFAULT 0");
            AddColumnIfNotExists(con, "Products", "qty_scale_pow10", "INTEGER NOT NULL DEFAULT 0");
            AddColumnIfNotExists(con, "Products", "product_type", "TEXT NOT NULL DEFAULT 'physical'");

            // Ensure SystemViolationLog exists on upgraded databases
            try
            {
                CreateSystemViolationLogTable(con);
            }
            catch { }

            try
            {
                Execute(@"UPDATE Products
                         SET qty_scale_pow10 = 0
                         WHERE qty_scale_pow10 IS NULL;", con);

                Execute(@"UPDATE Products
                         SET qty_scale_pow10 = 3
                         WHERE product_type = 'weighted'
                           AND (qty_scale_pow10 IS NULL OR qty_scale_pow10 = 0);", con);

                Execute(@"UPDATE Products
                         SET qty_scaled = qty
                         WHERE qty_scaled IS NULL;", con);

                Execute(@"UPDATE Products
                         SET qty_scaled = qty
                         WHERE qty_scaled = 0 AND qty <> 0;", con);

                Execute(@"UPDATE Products
                         SET product_type = 'physical'
                         WHERE product_type IS NULL OR TRIM(product_type) = '';", con);
            }
            catch
            {
            }

            // Defaults: physical => 0, weighted => 3 (unless explicitly provided)
            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_Products_DefaultScale_OnInsert
                AFTER INSERT ON Products
                FOR EACH ROW
                WHEN NEW.product_type = 'weighted' AND (NEW.qty_scale_pow10 IS NULL OR NEW.qty_scale_pow10 = 0)
                BEGIN
                    UPDATE Products
                    SET qty_scale_pow10 = 3
                    WHERE id = NEW.id;
                END;", con);

            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_Products_DefaultScale_OnProductTypeUpdate
                AFTER UPDATE OF product_type ON Products
                FOR EACH ROW
                WHEN NEW.product_type = 'weighted' AND (NEW.qty_scale_pow10 IS NULL OR NEW.qty_scale_pow10 = 0)
                BEGIN
                    UPDATE Products
                    SET qty_scale_pow10 = 3
                    WHERE id = NEW.id;
                END;", con);

            // Backfill: created_at/updated_at للأصناف القديمة
            try
            {
                // إذا كان عندنا product_date قديم (yyyy-MM-dd) نحوله لـ created_at بشكل ثابت
                Execute(@"UPDATE Products
                         SET created_at = COALESCE(created_at, product_date, CURRENT_TIMESTAMP)
                         WHERE created_at IS NULL OR created_at = '';", con);

                Execute(@"UPDATE Products
                         SET updated_at = COALESCE(updated_at, created_at, CURRENT_TIMESTAMP)
                         WHERE updated_at IS NULL OR updated_at = '';", con);
            }
            catch
            {
                // تجاهل
            }

            // [جديد] تفاصيل العميل

            // ── دعم عمود details للعملاء (ملاحظات) ─────────────────────────────
            AddColumnIfNotExists(con, "Customers", "details", "TEXT");

            // [جديد] عنوان العميل

            // ── دعم عمود address للعملاء ──────────────────────────────────────
            AddColumnIfNotExists(con, "Customers", "address", "TEXT");

            // إضافة الصلاحيات للمستخدمين

            // ── دعم عمود role للمستخدمين (admin/user) ─────────────────────────
            AddColumnIfNotExists(con, "Users", "role", "TEXT DEFAULT 'user'");

            // دعم ترقية كلمات المرور القديمة (إن كان العمود القديم موجوداً)

            // ── دعم عمود pwd_hash (قد يكون مفقوداً في قواعد قديمة) ─────────────
            AddColumnIfNotExists(con, "Users", "pwd_hash", "TEXT");

            // ===================== Indexes (Performance) =====================
            // هذه الفهارس مبنية على استعلامات فعلية في Repositories:
            // - CustomerRepository.GetAllCustomers: يعتمد على Orders.customer_id و Payments.order_id + ORDER BY Customers.name
            // - ProductRepository.GetAllProducts: ORDER BY Products.label
            // - عمليات الفواتير: تعتمد كثيراً على Order_Details.id_order
            try { Execute("CREATE INDEX IF NOT EXISTS idx_Customers_Name ON Customers(name);", con); } catch { }
            try { Execute("CREATE INDEX IF NOT EXISTS idx_Orders_CustomerId ON Orders(customer_id);", con); } catch { }
            try { Execute("CREATE INDEX IF NOT EXISTS idx_Payments_OrderId ON Payments(order_id);", con); } catch { }
            try { Execute("CREATE INDEX IF NOT EXISTS idx_OrderDetails_OrderId ON Order_Details(id_order);", con); } catch { }
            try { Execute("CREATE INDEX IF NOT EXISTS idx_Products_Label ON Products(label);", con); } catch { }

            try
            {
                Execute(@"
                CREATE TABLE IF NOT EXISTS BarcodeProfiles (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    code TEXT NOT NULL,
                    name TEXT NOT NULL,
                    is_enabled INTEGER NOT NULL DEFAULT 1,
                    priority INTEGER NOT NULL DEFAULT 100,
                    config_json TEXT NOT NULL DEFAULT '{}',
                    updated_at TEXT DEFAULT CURRENT_TIMESTAMP
                );", con);
                Execute("CREATE UNIQUE INDEX IF NOT EXISTS idx_BarcodeProfiles_Code_unique ON BarcodeProfiles(code);", con);
                Execute("CREATE INDEX IF NOT EXISTS idx_BarcodeProfiles_IsEnabled ON BarcodeProfiles(is_enabled);", con);
                Execute("CREATE INDEX IF NOT EXISTS idx_BarcodeProfiles_Priority ON BarcodeProfiles(priority);", con);

                Execute(@"
                INSERT OR IGNORE INTO BarcodeProfiles(code, name, is_enabled, priority, config_json)
                VALUES
                    ('GENERAL', 'General Barcode Profile', 1, 100, '{""type"":""general""}');", con);

                Execute(@"
                INSERT OR IGNORE INTO BarcodeProfiles(code, name, is_enabled, priority, config_json)
                VALUES
                    ('WEIGHT_EAN13', 'Weight Barcode Profile (EAN-13)', 1, 10, '{""type"":""weight_ean13"",""prefix"":""21"",""pluDigits"":5,""priceDigits"":5,""priceDecimals"":2}');", con);
            }
            catch
            {
            }

            CreateAppSettingsTable(con);

            CreateOrderAuditLogTable(con);

            CreateUnitsTable(con);
            CreateProductUnitsTable(con);
            CreateProductUnitBarcodesTable(con);
            CreatePurchasesTables(con);

            // Barcode identity upgrade: add barcode_type and enforce uniqueness per (barcode_type, barcode)
            try
            {
                AddColumnIfNotExists(con, "ProductUnitBarcodes", "barcode_type", "TEXT NOT NULL DEFAULT 'NORMAL'");
                Execute("UPDATE ProductUnitBarcodes SET barcode_type='NORMAL' WHERE barcode_type IS NULL OR TRIM(barcode_type)='';", con);

                try { Execute("DROP INDEX IF EXISTS idx_ProductUnitBarcodes_Barcode_unique;", con); } catch { }
                Execute("CREATE UNIQUE INDEX IF NOT EXISTS idx_ProductUnitBarcodes_BarcodeTypeBarcode_unique ON ProductUnitBarcodes(barcode_type, barcode);", con);
                Execute("CREATE INDEX IF NOT EXISTS idx_pub_barcode_type ON ProductUnitBarcodes(barcode_type);", con);
            }
            catch
            {
            }

            try
            {
                AddColumnIfNotExists(con, "ProductUnits", "parent_product_unit_id", "INTEGER");
                AddColumnIfNotExists(con, "ProductUnits", "pack_size", "REAL");

                try { Execute("CREATE INDEX IF NOT EXISTS idx_ProductUnits_ProductId_Parent ON ProductUnits(product_id, parent_product_unit_id);", con); } catch { }
                try { Execute("CREATE INDEX IF NOT EXISTS idx_ProductUnits_ParentId ON ProductUnits(parent_product_unit_id);", con); } catch { }

                try { Execute("UPDATE ProductUnits SET pack_size = NULL WHERE pack_size IS NOT NULL AND pack_size <= 0;", con); } catch { }
            }
            catch
            {
            }

            AddColumnIfNotExists(con, "Order_Details", "product_unit_id", "INTEGER");
            AddColumnIfNotExists(con, "Order_Details", "unit_name_snapshot", "TEXT");
            AddColumnIfNotExists(con, "Order_Details", "factor_snapshot", "REAL");
            AddColumnIfNotExists(con, "Order_Details", "qty_unit", "REAL");
            AddColumnIfNotExists(con, "Order_Details", "base_qty", "REAL");
            AddColumnIfNotExists(con, "Order_Details", "cost_price", "REAL");

            Execute(@"
            CREATE TABLE IF NOT EXISTS StockHistoryBatches (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                created_by TEXT,
                reason TEXT,
                reference TEXT,
                note TEXT
            );", con);

            Execute(@"
            CREATE TABLE IF NOT EXISTS StockHistoryBatchItems (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                batch_id INTEGER NOT NULL,
                product_id INTEGER NOT NULL,
                qty_before INTEGER,
                qty_after INTEGER,
                qty_change INTEGER,
                FOREIGN KEY(batch_id) REFERENCES StockHistoryBatches(id) ON DELETE CASCADE,
                FOREIGN KEY(product_id) REFERENCES Products(id) ON DELETE RESTRICT
            );", con);

            Execute("CREATE INDEX IF NOT EXISTS idx_StockHistoryBatches_CreatedAt ON StockHistoryBatches(created_at);", con);
            Execute("CREATE INDEX IF NOT EXISTS idx_StockHistoryBatches_Reason ON StockHistoryBatches(reason);", con);
            Execute("CREATE INDEX IF NOT EXISTS idx_StockHistoryBatchItems_BatchId ON StockHistoryBatchItems(batch_id);", con);
            Execute("CREATE INDEX IF NOT EXISTS idx_StockHistoryBatchItems_ProductId ON StockHistoryBatchItems(product_id);", con);

            Execute(@"
            CREATE TABLE IF NOT EXISTS PosHotkeys (
                key_code INTEGER PRIMARY KEY,
                product_unit_id INTEGER NOT NULL,
                qty_delta REAL NOT NULL DEFAULT 1,
                note TEXT,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY(product_unit_id) REFERENCES ProductUnits(id) ON DELETE RESTRICT
            );", con);

            Execute("CREATE INDEX IF NOT EXISTS idx_PosHotkeys_ProductUnitId ON PosHotkeys(product_unit_id);", con);

            Execute(@"
            CREATE TABLE IF NOT EXISTS BarcodeMissLog (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                barcode TEXT NOT NULL,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                user_name TEXT,
                terminal TEXT,
                resolved_product_unit_id INTEGER,
                resolved_at TEXT,
                note TEXT,
                FOREIGN KEY(resolved_product_unit_id) REFERENCES ProductUnits(id) ON DELETE SET NULL
            );", con);

            Execute("CREATE INDEX IF NOT EXISTS idx_BarcodeMissLog_CreatedAt ON BarcodeMissLog(created_at);", con);
            Execute("CREATE INDEX IF NOT EXISTS idx_BarcodeMissLog_Barcode ON BarcodeMissLog(barcode);", con);

            Execute(@"
            CREATE TABLE IF NOT EXISTS ProductReplacements (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                old_product_id INTEGER NOT NULL,
                new_product_id INTEGER NOT NULL,
                reason TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                created_by TEXT,
                FOREIGN KEY(old_product_id) REFERENCES Products(id) ON DELETE RESTRICT,
                FOREIGN KEY(new_product_id) REFERENCES Products(id) ON DELETE RESTRICT
            );", con);

            Execute("CREATE UNIQUE INDEX IF NOT EXISTS idx_ProductReplacements_Old_unique ON ProductReplacements(old_product_id);", con);
            Execute("CREATE INDEX IF NOT EXISTS idx_ProductReplacements_New ON ProductReplacements(new_product_id);", con);

            Execute(@"
            CREATE TABLE IF NOT EXISTS BarcodeCacheVersion (
                id INTEGER PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL DEFAULT 1,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP
            );", con);

            try
            {
                Execute("INSERT OR IGNORE INTO BarcodeCacheVersion (id, version) VALUES (1, 1);", con);
            }
            catch
            {
            }

            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_ProductUnitBarcodes_BumpCacheVersion_Insert
                AFTER INSERT ON ProductUnitBarcodes
                FOR EACH ROW
                BEGIN
                    UPDATE BarcodeCacheVersion
                    SET version = version + 1,
                        updated_at = CURRENT_TIMESTAMP
                    WHERE id = 1;
                END;", con);

            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_ProductUnitBarcodes_BumpCacheVersion_Update
                AFTER UPDATE ON ProductUnitBarcodes
                FOR EACH ROW
                BEGIN
                    UPDATE BarcodeCacheVersion
                    SET version = version + 1,
                        updated_at = CURRENT_TIMESTAMP
                    WHERE id = 1;
                END;", con);

            Execute(@"CREATE TRIGGER IF NOT EXISTS trg_ProductUnitBarcodes_BumpCacheVersion_Delete
                AFTER DELETE ON ProductUnitBarcodes
                FOR EACH ROW
                BEGIN
                    UPDATE BarcodeCacheVersion
                    SET version = version + 1,
                        updated_at = CURRENT_TIMESTAMP
                    WHERE id = 1;
                END;", con);

            try { DropOrphanedFeatureTables(con); } catch { }

            try { CreateLoginAuditLogTable(con); } catch { }
            try { CreateLoginAttemptsTable(con); } catch { }

            try { UpgradeOrderAuditLogForeignKey(con); } catch { }

            DatabaseBootstrap.RunPostSchemaMigrations(con);
        }

        private static void CreateOrderAuditLogTable(SQLiteConnection con)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS OrderAuditLog (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                order_id INTEGER,
                action TEXT NOT NULL,
                user_name TEXT,
                action_date TEXT DEFAULT CURRENT_TIMESTAMP,
                details TEXT,
                FOREIGN KEY(order_id) REFERENCES Orders(id) ON DELETE SET NULL
            );";
            Execute(sql, con);
            try { Execute("CREATE INDEX IF NOT EXISTS idx_OrderAuditLog_OrderId ON OrderAuditLog(order_id);", con); } catch { }
            try { Execute("CREATE INDEX IF NOT EXISTS idx_OrderAuditLog_ActionDate ON OrderAuditLog(action_date);", con); } catch { }
        }

        private static void CreateLoginAuditLogTable(SQLiteConnection con)
        {
            Execute(@"CREATE TABLE IF NOT EXISTS LoginAuditLog (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_name TEXT,
                login_time DATETIME,
                logout_time DATETIME,
                ip_address TEXT,
                success INTEGER,
                failure_reason TEXT
            );", con);
            try { Execute("CREATE INDEX IF NOT EXISTS idx_LoginAuditLog_User ON LoginAuditLog(user_name);", con); } catch { }
        }

        private static void CreateLoginAttemptsTable(SQLiteConnection con)
        {
            Execute(@"CREATE TABLE IF NOT EXISTS LoginAttempts (
                user_name TEXT PRIMARY KEY,
                failed_count INTEGER NOT NULL DEFAULT 0,
                locked_until TEXT
            );", con);
        }

        /// <summary>
        /// إعادة بناء OrderAuditLog لإضافة FK ON DELETE SET NULL على القواعد القديمة.
        /// </summary>
        private static void UpgradeOrderAuditLogForeignKey(SQLiteConnection con)
        {
            if (!TableExists(con, "OrderAuditLog")) return;

            using (var cmd = new SQLiteCommand("SELECT sql FROM sqlite_master WHERE type='table' AND name='OrderAuditLog'", con))
            {
                object sqlObj = cmd.ExecuteScalar();
                string ddl = sqlObj?.ToString() ?? string.Empty;
                if (ddl.IndexOf("REFERENCES Orders", StringComparison.OrdinalIgnoreCase) >= 0)
                    return;
            }

            Execute(@"CREATE TABLE IF NOT EXISTS OrderAuditLog_new (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                order_id INTEGER,
                action TEXT NOT NULL,
                user_name TEXT,
                action_date TEXT DEFAULT CURRENT_TIMESTAMP,
                details TEXT,
                FOREIGN KEY(order_id) REFERENCES Orders(id) ON DELETE SET NULL
            );", con);

            Execute(@"INSERT INTO OrderAuditLog_new (id, order_id, action, user_name, action_date, details)
                      SELECT id, order_id, action, user_name, action_date, details FROM OrderAuditLog;", con);
            Execute("DROP TABLE OrderAuditLog;", con);
            Execute("ALTER TABLE OrderAuditLog_new RENAME TO OrderAuditLog;", con);
            try { Execute("CREATE INDEX IF NOT EXISTS idx_OrderAuditLog_OrderId ON OrderAuditLog(order_id);", con); } catch { }
        }

        private static bool TableExists(SQLiteConnection con, string table)
        {
            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@t", con))
            {
                cmd.Parameters.AddWithValue("@t", table);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// AddColumnIfNotExists: إضافة عمود لجدول إذا لم يكن موجوداً (Migration Helper)
        /// المدخلات : con اتصال مفتوح، table اسم الجدول، column اسم العمود، type تعريف العمود
        /// المخرجات : لا يوجد
        /// التدفق   : UpgradeDatabase → AddColumnIfNotExists → SQLite(PRAGMA table_info + ALTER TABLE)
        /// الأخطاء  : جدول غير موجود، أو ALTER TABLE فشل، أو اسم عمود غير صالح
        /// </summary>
        private static void AddColumnIfNotExists(SQLiteConnection con, string table, string column, string type)
        {
            // ── PRAGMA table_info: قراءة وصف أعمدة الجدول (بدون تعديل) ─────────
            string check = $"PRAGMA table_info({table});";
            using (SQLiteCommand cmd = new SQLiteCommand(check, con))
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // ── إذا العمود موجود نخرج بدون أي ALTER (لتفادي Exception) ──
                    if (reader["name"].ToString().Equals(column, StringComparison.OrdinalIgnoreCase))
                        return;
                }
            }

            // ── إضافة العمود بشكل ديناميكي حسب نوعه المطلوب ────────────────────
            string alter = $"ALTER TABLE {table} ADD COLUMN {column} {type};";
            Execute(alter, con);
        }

        /// <summary>
        /// AddUniqueIndexIfNotExists: إنشاء فهرس فريد على عمود (اختياري حسب الحاجة)
        /// المدخلات : con اتصال مفتوح، table اسم الجدول، column اسم العمود
        /// المخرجات : لا يوجد
        /// التدفق   : (عند الحاجة) → AddUniqueIndexIfNotExists → SQLite(CREATE UNIQUE INDEX)
        /// الأخطاء  : فشل إنشاء الفهرس بسبب بيانات مكررة موجودة بالفعل
        /// </summary>
        private static void AddUniqueIndexIfNotExists(SQLiteConnection con, string table, string column)
        {
            // تم تبسيطها ودمجها حسب الحاجة
            string indexName = $"idx_{table}_{column}_unique";
            string sql = $"CREATE UNIQUE INDEX IF NOT EXISTS {indexName} ON {table}({column});";
            Execute(sql, con);
        }

        private static void MigrateOrdersDiscountFromNotes(SQLiteConnection con)
        {
            var pending = new List<Tuple<int, decimal, string>>();
            using (var cmd = new SQLiteCommand("SELECT id, note, discount FROM Orders", con))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["id"]);
                    string note = reader["note"] == DBNull.Value ? string.Empty : reader["note"].ToString();
                    decimal currentDiscount = 0m;
                    try
                    {
                        if (reader["discount"] != DBNull.Value)
                            currentDiscount = Convert.ToDecimal(reader["discount"]);
                    }
                    catch { }

                    decimal extracted = OrderDiscountHelper.ExtractDiscount(note);
                    if (extracted <= 0m) continue;
                    if (currentDiscount > 0m) continue;

                    string cleanNote = OrderDiscountHelper.RemoveDiscount(note);
                    pending.Add(Tuple.Create(id, extracted, cleanNote));
                }
            }

            foreach (var row in pending)
            {
                using (var upd = new SQLiteCommand("UPDATE Orders SET discount=@d, note=@n WHERE id=@id", con))
                {
                    upd.Parameters.AddWithValue("@d", row.Item2);
                    upd.Parameters.AddWithValue("@n", row.Item3 ?? string.Empty);
                    upd.Parameters.AddWithValue("@id", row.Item1);
                    upd.ExecuteNonQuery();
                }
            }
        }

        private static void CreateProductUnitPriceSyncTriggers(SQLiteConnection con)
        {
            PriceSyncTriggers.EnsureCreated(con);
        }

        private static void DropLegacyWarehouses(SQLiteConnection con)
        {
            Execute("DROP TABLE IF EXISTS Warehouses;", con);
            try
            {
                using (var cmd = new SQLiteCommand("DELETE FROM AppSettings WHERE key='DefaultWarehouseId'", con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private static void DropOrphanedFeatureTables(SQLiteConnection con)
        {
            Execute("DROP TABLE IF EXISTS BarcodeMissLog;", con);
            Execute("DROP TABLE IF EXISTS ProductReplacements;", con);
        }

        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  عدد الدوال    : 12                           ║
        // ║  الجداول       : Categories، Users، Customers، Products، Orders، Order_Details، Payments، StockHistory ║
        // ║  يستدعيه       : Program.cs (Initialize)، Login/Startup (IsFirstRun) ║
        // ║  يستدعي        : System.Data.SQLite (SQLiteConnection/SQLiteCommand/SQLiteDataReader) ║
        // ╚══════════════════════════════════════════════╝
    }
}
