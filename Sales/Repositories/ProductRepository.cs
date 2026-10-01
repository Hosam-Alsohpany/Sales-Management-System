// ============================================================
// الملف    : ProductRepository.cs
// الغرض    : مستودع لإدارة وتعديل المواد والمنتجات داخل المخزن
// ============================================================

// الملف    : ProductRepository.cs
// الغرض    : طبقة الوصول لبيانات المنتجات (CRUD) + فحص/تحديث المخزون + تسجيل حركات المخزون عند الإضافة/التعديل
// يتعامل مع: Forms/Manager_Products.cs + Forms/Add_Products.cs + Forms/Manager_Orders.cs + Repositories/OrderRepository.cs
// الجداول  : Products, Categories (JOIN), StockHistory
// ============================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using Sales.Database;
using Sales.Models;
using Sales.Services;
using Sales.Services.ProductDomains;
using Sales.Utilities;

namespace Sales.Repositories
{
    /// <summary>
    /// فئة مسؤولة عن جميع عمليات قاعدة البيانات المتعلقة بالمنتجات
    /// تتبع نمط Repository Pattern لفصل منطق الوصول للبيانات
    /// 
    /// التدفق العام:
    /// الواجهات (Manager_Products/Add_Products/Manager_Orders) → ProductRepository → SQLite → Product(Model)
    /// </summary>
    public class ProductRepository
    {
        public List<ProductUnitLookup> GetProductUnits(int productId)
        {
            var list = new List<ProductUnitLookup>();
            if (productId <= 0) return list;

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    const string sql = @"
                        SELECT
                            pu.id as product_unit_id,
                            pu.product_id,
                            p.label as product_name,
                            pu.unit_id,
                            u.name as unit_name,
                            pu.factor,
                            pu.sell_price,
                            pu.cost_price,
                            pu.display_order
                        FROM ProductUnits pu
                        INNER JOIN Products p ON p.id = pu.product_id
                        INNER JOIN Units u ON u.id = pu.unit_id
                        WHERE pu.product_id = @pid
                        ORDER BY pu.display_order ASC, pu.factor ASC, pu.id ASC;";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@pid", productId);
                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                list.Add(new ProductUnitLookup
                                {
                                    ProductId = Convert.ToInt32(dr["product_id"]),
                                    ProductUnitId = Convert.ToInt32(dr["product_unit_id"]),
                                    ProductName = dr["product_name"].ToString(),
                                    UnitName = dr["unit_name"].ToString(),
                                    Factor = dr["factor"] == DBNull.Value ? 1m : Convert.ToDecimal(dr["factor"]),
                                    SellPrice = dr["sell_price"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["sell_price"]),
                                    CostPrice = dr["cost_price"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["cost_price"]),
                                    DisplayOrder = dr["display_order"] == DBNull.Value ? 1 : Convert.ToInt32(dr["display_order"])
                                });
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetProductUnits for productId: {productId}", ex);
                throw;
            }

            return list;
        }

        public ProductUnitLookup GetProductUnitByBarcodeType(string barcodeType, string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcodeType) || string.IsNullOrWhiteSpace(barcode))
                return null;

            barcodeType = barcodeType.Trim();
            barcode = barcode.Trim();

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    const string sql = @"
                        SELECT
                            bub.barcode,
                            bub.is_default,
                            pu.id as product_unit_id,
                            pu.product_id,
                            p.label as product_name,
                            u.name as unit_name,
                            pu.factor,
                            pu.sell_price,
                            pu.cost_price,
                            pu.display_order
                        FROM ProductUnitBarcodes bub
                        INNER JOIN ProductUnits pu ON pu.id = bub.product_unit_id
                        INNER JOIN Products p ON p.id = pu.product_id
                        INNER JOIN Units u ON u.id = pu.unit_id
                        WHERE bub.barcode_type = @t AND bub.barcode = @barcode
                        ORDER BY bub.is_default DESC, bub.id ASC
                        LIMIT 2;";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@t", barcodeType);
                        cmd.Parameters.AddWithValue("@barcode", barcode);

                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            ProductUnitLookup first = null;
                            int count = 0;

                            while (dr.Read())
                            {
                                count++;
                                if (count == 1)
                                {
                                    first = new ProductUnitLookup
                                    {
                                        Barcode = dr["barcode"].ToString(),
                                        IsDefaultBarcode = Convert.ToInt32(dr["is_default"]) == 1,
                                        ProductUnitId = Convert.ToInt32(dr["product_unit_id"]),
                                        ProductId = Convert.ToInt32(dr["product_id"]),
                                        ProductName = dr["product_name"].ToString(),
                                        UnitName = dr["unit_name"].ToString(),
                                        Factor = dr["factor"] == DBNull.Value ? 1m : Convert.ToDecimal(dr["factor"]),
                                        SellPrice = dr["sell_price"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["sell_price"]),
                                        CostPrice = dr["cost_price"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["cost_price"]),
                                        DisplayOrder = dr["display_order"] == DBNull.Value ? 1 : Convert.ToInt32(dr["display_order"])
                                    };
                                }
                            }

                            if (count == 0)
                                return null;

                            if (count > 1)
                                Logger.LogError($"Duplicated barcode detected in DB (GetProductUnitByBarcodeType): {barcodeType}:{barcode}");

                            return first;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetProductUnitByBarcodeType for barcode: {barcodeType}:{barcode}", ex);
                throw;
            }
        }

        public ProductUnitLookup GetProductUnitByPlu(string plu)
        {
            return GetProductUnitByBarcodeType("PLU", plu);
        }

        public ProductUnitLookup GetProductUnitByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return null;

            barcode = barcode.Trim();

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    const string sql = @"
                        SELECT
                            bub.barcode,
                            bub.is_default,
                            pu.id as product_unit_id,
                            pu.product_id,
                            p.label as product_name,
                            u.name as unit_name,
                            pu.factor,
                            pu.sell_price,
                            pu.cost_price,
                            pu.display_order
                        FROM ProductUnitBarcodes bub
                        INNER JOIN ProductUnits pu ON pu.id = bub.product_unit_id
                        INNER JOIN Products p ON p.id = pu.product_id
                        INNER JOIN Units u ON u.id = pu.unit_id
                        WHERE bub.barcode_type = 'NORMAL' AND bub.barcode = @barcode
                        ORDER BY bub.is_default DESC, bub.id ASC
                        LIMIT 2;";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@barcode", barcode);

                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            ProductUnitLookup first = null;
                            int count = 0;

                            while (dr.Read())
                            {
                                count++;
                                if (count == 1)
                                {
                                    first = new ProductUnitLookup
                                    {
                                        Barcode = dr["barcode"].ToString(),
                                        IsDefaultBarcode = Convert.ToInt32(dr["is_default"]) == 1,
                                        ProductUnitId = Convert.ToInt32(dr["product_unit_id"]),
                                        ProductId = Convert.ToInt32(dr["product_id"]),
                                        ProductName = dr["product_name"].ToString(),
                                        UnitName = dr["unit_name"].ToString(),
                                        Factor = dr["factor"] == DBNull.Value ? 1m : Convert.ToDecimal(dr["factor"]),
                                        SellPrice = dr["sell_price"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["sell_price"]),
                                        CostPrice = dr["cost_price"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["cost_price"]),
                                        DisplayOrder = dr["display_order"] == DBNull.Value ? 1 : Convert.ToInt32(dr["display_order"])
                                    };
                                }
                            }

                            if (count == 0)
                                return null;

                            if (count > 1)
                                Logger.LogError($"Duplicated barcode detected in DB (GetProductUnitByBarcode): {barcode}");

                            return first;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetProductUnitByBarcode for barcode: {barcode}", ex);
                throw;
            }
        }

        public Dictionary<string, ProductUnitLookup> GetAllProductUnitsByBarcodeDictionary()
        {
            var dict = new Dictionary<string, ProductUnitLookup>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    const string sql = @"
                        SELECT
                            bub.barcode,
                            bub.is_default,
                            pu.id as product_unit_id,
                            pu.product_id,
                            p.label as product_name,
                            u.name as unit_name,
                            pu.factor,
                            pu.sell_price,
                            pu.cost_price,
                            pu.display_order
                        FROM ProductUnitBarcodes bub
                        INNER JOIN ProductUnits pu ON pu.id = bub.product_unit_id
                        INNER JOIN Products p ON p.id = pu.product_id
                        INNER JOIN Units u ON u.id = pu.unit_id
                        WHERE bub.barcode_type = 'NORMAL'
                        ORDER BY bub.barcode, bub.is_default DESC, bub.id ASC;";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string barcode = dr["barcode"].ToString();
                            if (string.IsNullOrWhiteSpace(barcode))
                                continue;

                            if (dict.ContainsKey(barcode))
                            {
                                Logger.LogError($"Duplicated barcode detected in cache load (skipped): {barcode}");
                                continue;
                            }

                            dict[barcode] = new ProductUnitLookup
                            {
                                Barcode = barcode,
                                IsDefaultBarcode = Convert.ToInt32(dr["is_default"]) == 1,
                                ProductUnitId = Convert.ToInt32(dr["product_unit_id"]),
                                ProductId = Convert.ToInt32(dr["product_id"]),
                                ProductName = dr["product_name"].ToString(),
                                UnitName = dr["unit_name"].ToString(),
                                Factor = dr["factor"] == DBNull.Value ? 1m : Convert.ToDecimal(dr["factor"]),
                                SellPrice = dr["sell_price"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["sell_price"]),
                                CostPrice = dr["cost_price"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["cost_price"]),
                                DisplayOrder = dr["display_order"] == DBNull.Value ? 1 : Convert.ToInt32(dr["display_order"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetAllProductUnitsByBarcodeDictionary", ex);
                throw;
            }

            return dict;
        }

        /// <summary>
        /// GetProductByBarcode: جلب منتج عبر باركود (بدلاً من الاعتماد على ProductId)
        /// يعالج حالات: باركود غير موجود (return null) / باركود مكرر (throw InvalidOperationException)
        /// </summary>
        public Product GetProductByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return null;

            barcode = barcode.Trim();

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    // نرتب بحيث لو هناك أكثر من صف (لا يجب بسبب UNIQUE) نقرأهم ونرمي خطأ واضح
                    string sql = @"
                        SELECT
                            p.*, c.name as category_name,
                            bub.barcode as barcode,
                            bub.is_default as is_default
                        FROM ProductUnitBarcodes bub
                        INNER JOIN ProductUnits pu ON pu.id = bub.product_unit_id
                        INNER JOIN Products p ON p.id = pu.product_id
                        LEFT JOIN Categories c ON p.category_id = c.id
                        WHERE bub.barcode = @barcode
                        ORDER BY bub.is_default DESC, bub.id ASC
                        LIMIT 2;";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@barcode", barcode);

                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            Product first = null;
                            int count = 0;

                            while (dr.Read())
                            {
                                count++;

                                if (count == 1)
                                {
                                    first = ProductRowMapper.FromReader(dr);
                                    first.DefaultBarcode = dr["barcode"].ToString();
                                }
                            }

                            if (count == 0)
                                return null;

                            if (count > 1)
                                Logger.LogError($"Duplicated barcode detected in DB (GetProductByBarcode): {barcode}");

                            return first;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetProductByBarcode for barcode: {barcode}", ex);
                throw;
            }
        }

        /// <summary>
        /// GetAllProductsByBarcodeDictionary: تحميل كل الباركودات مع منتجاتها في Dictionary للاستخدام في POS بدون DB call لكل scan
        /// ملاحظة: في حال وجود تعارض (نفس الباركود أكثر من مرة) يتم رمي استثناء لتجنب بيع خاطئ.
        /// </summary>
        public Dictionary<string, Product> GetAllProductsByBarcodeDictionary()
        {
            var dict = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();

                    string sql = @"
                        SELECT bub.barcode, bub.is_default, p.*, c.name as category_name
                        FROM ProductUnitBarcodes bub
                        INNER JOIN ProductUnits pu ON pu.id = bub.product_unit_id
                        INNER JOIN Products p ON p.id = pu.product_id
                        LEFT JOIN Categories c ON p.category_id = c.id
                        ORDER BY bub.barcode, bub.is_default DESC, bub.id ASC;";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string barcode = dr["barcode"].ToString();
                            if (string.IsNullOrWhiteSpace(barcode))
                                continue;

                            if (dict.ContainsKey(barcode))
                            {
                                Logger.LogError($"Duplicated barcode detected in cache load (skipped): {barcode}");
                                continue;
                            }

                            dict[barcode] = ProductRowMapper.FromReader(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetAllProductsByBarcodeDictionary", ex);
                throw;
            }

            return dict;
        }

        /// <summary>
        /// جلب جميع المنتجات من قاعدة البيانات مع معلومات الفئات
        /// المدخلات : لا يوجد
        /// المخرجات : List&lt;Product&gt; قائمة المنتجات (قد تحتوي CategoryId واسم الصنف من خلال الاستعلام)
        /// التدفق   : UI → GetAllProducts → SQLite(Products LEFT JOIN Categories) → List<Product>
        /// الأخطاء  : أخطاء اتصال/SQL/تحويل أنواع، ويتم تسجيلها عبر Logger ثم إعادة رمي الاستثناء
        /// </summary>
        /// <returns>قائمة بجميع المنتجات مرتبة حسب الاسم</returns>
        public List<Product> GetAllProducts() // المهمة: أعطيك سلة كل المنتجات
        {
            List<Product> products = new List<Product>(); // السلة الفارغة
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                // ↑ أنشأنا اتصالاً بالقاعدة. using = أغلقه تلقائياً بعد الانتهاء
                {
                    con.Open(); // افتح الاتصال

                    // الأمر SQL: "هات كل المنتجات مع اسم الصنف، ورتبهم أبجدياً"
                    string sql = @"
                SELECT 
                    p.*, 
                    c.name as category_name,
                    (
                        SELECT bub.barcode
                        FROM ProductUnitBarcodes bub
                        INNER JOIN ProductUnits pu ON pu.id = bub.product_unit_id
                        WHERE pu.product_id = p.id
                        ORDER BY bub.is_default DESC, bub.id ASC
                        LIMIT 1
                    ) as default_barcode
                FROM Products p 
                LEFT JOIN Categories c ON p.category_id = c.id
                ORDER BY p.label";
                    // p.*         = كل أعمدة المنتج
                    // LEFT JOIN   = "ولو له صنف، هات اسم الصنف أيضاً"
                    // ORDER BY    = "رتبهم حسب الاسم"

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    // ↑ جهزنا "الرسول" الذي سيحمل الأمر لقاعدة البيانات
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    // ↑ "القارئ": نفذ الأمر وابدأ قراءة النتائج سطراً سطراً
                    {
                        while (dr.Read()) // كرر: طالما يوجد سطر تالي في النتائج
                        {
                            var product = ProductRowMapper.FromReader(dr);
                            if (string.IsNullOrWhiteSpace(product.CategoryName))
                                product.CategoryName = "—";
                            product.DefaultBarcode = dr["default_barcode"] == DBNull.Value ? "" : dr["default_barcode"].ToString();
                            products.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex) // لو حدث خطأ
            {
                Logger.LogError("Error in GetAllProducts", ex); // سجله
                throw; // وارميه للشاشة
            }
            return products; // سلم السلة
        }


        /// <summary>
        /// جلب منتج محدد بالمعرف
        /// المدخلات : id رقم المنتج
        /// المخرجات : Product أو null
        /// التدفق   : Manager_Orders(txtID) → GetProductById → SQLite(Products) → تعبئة بيانات المنتج في الشاشة
        /// الأخطاء  : أخطاء اتصال/SQL/تحويل أنواع، ويتم تسجيلها عبر Logger ثم إعادة رمي الاستثناء
        /// </summary>
        /// <param name="id">معرف المنتج المطلوب</param>
        /// <returns>كائن المنتج إذا وجد، أو null إذا لم يوجد</returns>
        public Product GetProductById(int id) // أعطني منتجاً واحداً برقمه
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    con.Open();
                    string sql = @"
                        SELECT 
                            p.*, 
                            c.name as category_name,
                            (
                                SELECT bub.barcode
                                FROM ProductUnitBarcodes bub
                                INNER JOIN ProductUnits pu ON pu.id = bub.product_unit_id
                                WHERE pu.product_id = p.id
                                ORDER BY bub.is_default DESC, bub.id ASC
                                LIMIT 1
                            ) as default_barcode
                        FROM Products p
                        LEFT JOIN Categories c ON p.category_id = c.id
                        WHERE p.id = @id";
                    // ↑ "هات المنتج الذي رقمه = الرقم الذي سأعطيك إياه"

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        // ↑ @id هو "مكان فارغ" في الأمر، نملأه بالقيمة الحقيقية (id)
                        // هذا يحمينا من الاختراق (SQL Injection)

                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                var product = ProductRowMapper.FromReader(dr);
                                if (string.IsNullOrWhiteSpace(product.CategoryName))
                                    product.CategoryName = "—";
                                product.DefaultBarcode = dr["default_barcode"] == DBNull.Value ? "" : dr["default_barcode"].ToString();
                                return product;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetProductById for id: {id}", ex);
                throw;
            }
            return null; // لا، يدي فارغة
        }

        /// <summary>
        /// المهمة: CheckStock(هل يكفي المخزون؟)
        /// التحقق من توفر كمية كافية في المخزون
        /// المدخلات : productId رقم المنتج، qtyRequired الكمية المطلوبة
        /// المخرجات : true إذا كانت الكمية متاحة، false خلاف ذلك
        /// التدفق   : Order/Manager_Orders → CheckStock → GetProductById → مقارنة Qty
        /// الأخطاء  : أي خطأ أثناء القراءة يتم تسجيله وإرجاع false كقيمة آمنة
        /// </summary>
        /// <param name="productId">معرف المنتج</param>
        /// <param name="qtyRequired">الكمية المطلوبة</param>
        /// <returns>true إذا كانت الكمية متوفرة، false خلاف ذلك</returns>
        public bool CheckStockScaled(int productId, decimal baseQtyRequired)
        {
            try
            {
                var p = GetProductById(productId);
                if (p == null) return false;
                long requiredScaled = ProductQuantityHelper.BaseQtyToScaled(baseQtyRequired, p.QtyScalePow10);
                return p.QtyScaled >= requiredScaled;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in CheckStockScaled for productId: {productId}", ex);
                return false;
            }
        }

        /// <summary>
        /// خصم كمية المنتج من المخزون بعد البيع
        /// تستخدم داخل معاملة لضمان التناسق
        /// المدخلات : productId رقم المنتج، qtySold الكمية المباعة، con اتصال مفتوح، tran معاملة نشطة
        /// المخرجات : لا يوجد
        /// التدفق   : OrderRepository.SaveOrder/UpdateOrder → ProductRepository.DecreaseStock → SQLite(Products.qty)
        /// الأخطاء  : أخطاء SQLite داخل المعاملة (يتم تسجيلها ثم رميها لعمل Rollback من المستدعي)
        /// </summary>
        /// <param name="productId">معرف المنتج</param>
        /// <param name="qtySold">الكمية المباعة</param>
        /// <param name="con">اتصال قاعدة البيانات المفتوح</param>
        /// <param name="tran">المعاملة الحالية</param>
        public void DecreaseStock(int productId, int qtySold, SQLiteConnection con, SQLiteTransaction tran)
        {// ↑ void = نفذ ولا ترجع بشيء
         // ↑ con, tran = الاتصال والصفقة يأتيان من الخارج (من OrderRepository)
            try
            {
                throw new InvalidOperationException("Forbidden: stock mutation must go through SalesTransactionService");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in DecreaseStock for productId: {productId}, qty: {qtySold}", ex);
                throw;
            }
        }

        /// <summary>
        /// إضافة منتج جديد إلى قاعدة البيانات
        /// تقوم أيضاً بتسجيل الحركة في سجل المخزون إذا كانت الكمية موجودة
        /// المدخلات : p كائن المنتج
        /// المخرجات : لا يوجد (يتم إنشاء id تلقائياً في DB)
        /// التدفق   : Add_Products/Manager_Products → AddProduct → SQLite(Products) → SQLite(StockHistory)
        /// الأخطاء  : أي خطأ يؤدي لإلغاء المعاملة (Rollback) عبر SQLiteTransaction
        /// </summary>
        /// <param name="p">كائن المنتج المراد إضافته</param>
        /// <returns>معرف المنتج الجديد</returns>
        /// <exception cref="ArgumentNullException">إذا كان المنتج null</exception>
        public void AddProduct(Product p) // أعطيني علبة مملوءة بالبيانات وسأحفظها
        {
            TransactionGuard.Run("Products.Insert", () =>
            {
                AddProduct(p, null);
            });
        }

        public int AddProduct(Product p, string defaultBarcode)
        {
            return TransactionGuard.Run("Products.Insert", () =>
            {
                if (p == null)
                    throw new ArgumentNullException(nameof(p), "المنتج لا يمكن أن يكون فارغاً"); // لا تعطيني علبة فارغة!

                if (string.IsNullOrWhiteSpace(p.CreatedBy))
                    throw new InvalidOperationException("User required");

                try
                {
                    using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                    {
                        con.Open();
                        using (SQLiteTransaction tran = con.BeginTransaction())
                        {
                            string productType = ProductTypeHelper.NormalizeProductType(p.ProductType);
                            int scalePow = ProductTypeHelper.DefaultScalePow10ForType(productType);

                            string sql = @"INSERT INTO Products (sku, label, qty_scaled, qty_scale_pow10, product_type, price, cost_price, min_qty, category_id, note, created_by, expiry_date, created_at, updated_at, image_path) 
                                           VALUES (@sku, @label, 0, @pow, @ptype, @price, @cost, @min, @cat, @note, @user, @expiry, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @imagePath);
                                           SELECT last_insert_rowid();";

                            long newId;
                            using (SQLiteCommand cmd = new SQLiteCommand(sql, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@sku", string.IsNullOrWhiteSpace(p.Sku) ? (object)DBNull.Value : p.Sku.Trim());
                                cmd.Parameters.AddWithValue("@label", p.Label);
                                cmd.Parameters.AddWithValue("@price", p.Price);
                                cmd.Parameters.AddWithValue("@cost", p.CostPrice);
                                cmd.Parameters.AddWithValue("@min", p.MinQty);
                                cmd.Parameters.AddWithValue("@cat", p.CategoryId ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@note", p.Note ?? "");
                                cmd.Parameters.AddWithValue("@user", p.CreatedBy ?? "");
                                cmd.Parameters.AddWithValue("@pow", scalePow);
                                cmd.Parameters.AddWithValue("@ptype", productType);
                                cmd.Parameters.AddWithValue("@expiry", string.IsNullOrWhiteSpace(p.ExpiryDate) ? (object)DBNull.Value : p.ExpiryDate);
                                cmd.Parameters.AddWithValue("@imagePath", string.IsNullOrWhiteSpace(p.ImagePath) ? (object)DBNull.Value : p.ImagePath);

                                newId = (long)cmd.ExecuteScalar();
                            }

                            // Enforce new barcode model: ProductUnitBarcodes is the only source.
                            // Product must be sellable immediately:
                            // - Always create a default ProductUnit (so POS can find by name/suggestions even with no barcode).
                            // - If a barcode is provided, create a default ProductUnitBarcode.

                            // Resolve default unit.
                            string defaultUnitName = null;
                            try { defaultUnitName = AppSettingsManager.GetString(AppSettingsManager.Keys.DefaultUnit, ""); } catch { defaultUnitName = null; }
                            if (string.IsNullOrWhiteSpace(defaultUnitName))
                                defaultUnitName = "حبة";

                            int unitId;
                            using (var cmdUnit = new SQLiteCommand("SELECT id FROM Units WHERE name=@n LIMIT 1", con, tran))
                            {
                                cmdUnit.Parameters.AddWithValue("@n", defaultUnitName.Trim());
                                object idObj = cmdUnit.ExecuteScalar();
                                if (idObj != null && idObj != DBNull.Value)
                                {
                                    unitId = Convert.ToInt32(idObj);
                                }
                                else
                                {
                                    using (var cmdInsUnit = new SQLiteCommand("INSERT INTO Units (name) VALUES (@n); SELECT last_insert_rowid();", con, tran))
                                    {
                                        cmdInsUnit.Parameters.AddWithValue("@n", defaultUnitName.Trim());
                                        unitId = Convert.ToInt32((long)cmdInsUnit.ExecuteScalar());
                                    }
                                }
                            }

                            // Create default ProductUnit (factor=1).
                            int productUnitId;
                            const string sqlPU = @"INSERT INTO ProductUnits (product_id, unit_id, factor, sell_price, cost_price, display_order)
                                               VALUES (@pid, @uid, 1, @sell, @cost, 1);
                                               SELECT last_insert_rowid();";
                            using (var cmdPu = new SQLiteCommand(sqlPU, con, tran))
                            {
                                cmdPu.Parameters.AddWithValue("@pid", newId);
                                cmdPu.Parameters.AddWithValue("@uid", unitId);
                                cmdPu.Parameters.AddWithValue("@sell", p.Price);
                                cmdPu.Parameters.AddWithValue("@cost", p.CostPrice);
                                productUnitId = Convert.ToInt32((long)cmdPu.ExecuteScalar());
                            }

                            if (!string.IsNullOrWhiteSpace(defaultBarcode))
                            {
                                string barcodeClean = defaultBarcode.Trim();

                                // Validate barcode uniqueness early to return a clear error.
                                using (var check = new SQLiteCommand("SELECT COUNT(*) FROM ProductUnitBarcodes WHERE barcode=@b", con, tran))
                                {
                                    check.Parameters.AddWithValue("@b", barcodeClean);
                                    int exists = Convert.ToInt32(check.ExecuteScalar());
                                    if (exists > 0)
                                        throw new InvalidOperationException("الباركود مستخدم مسبقاً: " + barcodeClean);
                                }

                                // Create default ProductUnitBarcode.
                                const string sqlB = @"INSERT INTO ProductUnitBarcodes (product_unit_id, barcode, is_default)
                                                  VALUES (@puid, @b, 1);";
                                using (var cmdB = new SQLiteCommand(sqlB, con, tran))
                                {
                                    cmdB.Parameters.AddWithValue("@puid", productUnitId);
                                    cmdB.Parameters.AddWithValue("@b", barcodeClean);
                                    cmdB.ExecuteNonQuery();
                                }
                            }

                            if (p.Qty > 0)
                                throw new InvalidOperationException("Forbidden: initial stock must be applied via SalesTransactionService");

                            try
                            {
                                var domainRepo = new ProductDomainsRepository();
                                var domainType = ProductTypeHelper.ToDomainType(productType);
                                string cfg = ProductDomainConfigSerializer.CreateTemplateJson(domainType);
                                domainRepo.UpsertDomain((int)newId, domainType, cfg);
                            }
                            catch (Exception domainEx)
                            {
                                Logger.LogError("AddProduct: ProductDomains upsert failed for product " + newId, domainEx);
                            }

                            tran.Commit();
                            return (int)newId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error in AddProduct for product: {p.Label}", ex);
                    throw;
                }
            });
        }


        /// <summary>
        /// تحديث بيانات منتج موجود
        /// تتطلب صلاحيات المدير
        /// تقوم بتسجيل أي تغيير في الكمية في سجل المخزون
        /// المدخلات : p كائن المنتج بعد التعديل
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Products → UpdateProduct → SQLite(Products) → SQLite(StockHistory عند تغير qty)
        /// الأخطاء  : عدم صلاحية (RequireAdmin) أو أخطاء SQLite داخل المعاملة
        /// </summary>
        /// <param name="p">كائن المنتج المحدث</param>
        /// <exception cref="ArgumentNullException">إذا كان المنتج null</exception>
        /// <exception cref="UnauthorizedAccessException">إذا لم يكن المستخدم مديراً</exception>
        public void UpdateProduct(Product p, int? expectedOldQty = null)
        {
            TransactionGuard.Run("Products.Update", () =>
            {
                if (p == null)
                    throw new ArgumentNullException(nameof(p), "المنتج لا يمكن أن يكون فارغاً");

                SessionManager.RequireAdmin();

                if (string.IsNullOrWhiteSpace(SessionManager.CurrentUsername))
                    throw new InvalidOperationException("User required");

                try
                {
                    using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                    {
                        con.Open();

                        using (SQLiteTransaction tran = con.BeginTransaction())
                        {
                            string productType = ProductTypeHelper.NormalizeProductType(p.ProductType);
                            int pow = p.QtyScalePow10;
                            if (pow < 0) pow = 0;
                            if (pow > 6) pow = 6;
                            if (string.IsNullOrWhiteSpace(p.ProductType) || ProductTypeHelper.IsWeighted(productType))
                            {
                                if (ProductTypeHelper.IsWeighted(productType) && pow == 0)
                                    pow = ProductTypeHelper.DefaultScalePow10ForType(productType);
                            }

                            string sql = @"UPDATE Products SET 
                                           sku=@sku, label=@label, product_type=@ptype, price=@price, cost_price=@cost, min_qty=@min, expiry_date=@expiry, category_id=@cat, note=@note, updated_at=CURRENT_TIMESTAMP, image_path=@imagePath,
                                           qty_scaled=@qty_scaled, qty_scale_pow10=@qty_scale_pow10
                                           WHERE id=@id";
                            using (SQLiteCommand cmd = new SQLiteCommand(sql, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@sku", string.IsNullOrWhiteSpace(p.Sku) ? (object)DBNull.Value : p.Sku.Trim());
                                cmd.Parameters.AddWithValue("@label", p.Label);
                                cmd.Parameters.AddWithValue("@ptype", productType);
                                cmd.Parameters.AddWithValue("@price", p.Price);
                                cmd.Parameters.AddWithValue("@cost", p.CostPrice);
                                cmd.Parameters.AddWithValue("@min", p.MinQty);
                                cmd.Parameters.AddWithValue("@expiry", string.IsNullOrWhiteSpace(p.ExpiryDate) ? (object)DBNull.Value : p.ExpiryDate);
                                cmd.Parameters.AddWithValue("@cat", p.CategoryId ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@note", p.Note ?? "");
                                cmd.Parameters.AddWithValue("@imagePath", string.IsNullOrWhiteSpace(p.ImagePath) ? (object)DBNull.Value : p.ImagePath);

                                long scaled = p.QtyScaled;
                                if (scaled <= 0 && p.Qty > 0)
                                    scaled = ProductQuantityHelper.BaseQtyToScaled(p.Qty, pow);

                                cmd.Parameters.AddWithValue("@qty_scaled", scaled);
                                cmd.Parameters.AddWithValue("@qty_scale_pow10", pow);
                                p.QtyScalePow10 = pow;
                                p.QtyScaled = scaled;
                                cmd.Parameters.AddWithValue("@id", p.Id);
                                cmd.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error in UpdateProduct for product id: {p.Id}", ex);
                    throw;
                }
            });
        }

        /// <summary>
        /// حذف منتج من قاعدة البيانات
        /// تتطلب صلاحيات المدير
        /// المدخلات : id رقم المنتج
        /// المخرجات : لا يوجد
        /// التدفق   : Manager_Products → DeleteProduct → SQLite(Products)
        /// الأخطاء  : عدم صلاحية (RequireAdmin) أو أخطاء SQLite أثناء الحذف
        /// </summary>
        /// <param name="id">معرف المنتج المراد حذفه</param>
        /// <exception cref="UnauthorizedAccessException">إذا لم يكن المستخدم مديراً</exception>
        public void DeleteProduct(int id)
        {
            TransactionGuard.Run("Products.Delete", () =>
            {
                SessionManager.RequireAdminForDeleteIfEnabled();

                try
                {
                    using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                    {
                        con.Open();
                        string sql = "DELETE FROM Products WHERE id = @id";
                        using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error in DeleteProduct for id: {id}", ex);
                    throw;
                }
            });
        }

        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  عدد الدوال    : 6                            ║
        // ║  الجداول       : Products، Categories، StockHistory ║
        // ║  يستدعيه       : Manager_Products.cs، Add_Products.cs، Manager_Orders.cs ║
        // ║  يستدعي        : DatabaseInitializer.ConnectionString + SessionManager/Logger ║
        // ╚══════════════════════════════════════════════╝
    }
}
