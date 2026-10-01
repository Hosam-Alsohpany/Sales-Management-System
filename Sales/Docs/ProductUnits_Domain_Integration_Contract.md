# ProductUnits Domain & Integration Final Contract

## 1) BaseQuantity Strategy

### القاعدة
- **جميع الحركات المخزنية تحفظ بالوحدة الأساسية فقط**
- أي ProductUnit تستخدم فقط للإدخال والعرض
- Stock ledger يعتمد دائمًا على BaseQty

### التطبيق (مطابق SQLite الفعلي — 2026)
```sql
-- Products: المصدر الوحيد للمخزون
-- qty_scaled + qty_scale_pow10 (حقيقة) | qty INTEGER (توافق قديم، يُزامَن تلقائياً)

-- StockHistory
CREATE TABLE StockHistory (
    id INTEGER PRIMARY KEY,
    product_id INTEGER NOT NULL,
    qty_change INTEGER NOT NULL,           -- توافق قديم
    qty_change_scaled INTEGER,             -- دلتا مقيّسة
    qty_scale_pow10 INTEGER DEFAULT 0,   -- منزلة العرض (مثلاً 3 = جرام من كيلو)
    reason TEXT,
    change_date TEXT,
    user_name TEXT
);

-- Order_Details
CREATE TABLE Order_Details (
    ...
    qty INTEGER,                -- توافق قديم فقط
    qty_unit REAL,
    base_qty REAL,              -- الكمية بالوحدة الأساسية (مصدر الحقيقة)
    qty_scaled_snapshot INTEGER,
    qty_scale_pow10_snapshot INTEGER,
    unit_name_snapshot TEXT,
    factor_snapshot REAL
    -- barcode_snapshot / sell_price_snapshot: مخطط مستقبلي
);
```

---

## 2) Invoice Snapshot Strategy

### القاعدة
- الفواتير تحفظ snapshot كامل للبيانات وقت المعاملة
- لا تعتمد فقط على product_unit_id

### الحقول المحفوظة
```sql
CREATE TABLE Order_Details (
    id INTEGER PRIMARY KEY,
    product_id INTEGER,
    product_unit_id INTEGER,           -- للرابط فقط
    unit_name_snapshot VARCHAR(50),     -- اسم الوحدة وقت البيع
    factor_snapshot DECIMAL(10,3),      -- المعامل وقت البيع
    barcode_snapshot VARCHAR(50),       -- الباركود وقت البيع
    sell_price_snapshot DECIMAL(10,3),  -- السعر وقت البيع
    base_qty DECIMAL(10,3),            -- الكمية بالوحدة الأساسية
    unit_qty DECIMAL(10,3),            -- الكمية بالوحدة المستخدمة
    ...
);
```

---

## 3) Decimal Policy

### القاعدة
- **UI يمنع الكسور حاليًا**
- **قاعدة البيانات والخدمات تستخدم decimal معماريًا**
- **المرونة للتوسع مستقبلاً**

### التطبيق
```csharp
// Domain Layer
public class ProductUnit
{
    public decimal PackSize { get; set; }  // decimal معماريًا
    public decimal Factor { get; set; }     // decimal معماريًا
}

// UI Layer
private void dgvProductUnits_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
{
    if (e.ColumnIndex == colPackSize.Index)
    {
        // UI يمنع الكسور حاليًا
        if (decimal.TryParse(e.FormattedValue.ToString(), out var value))
        {
            if (value % 1 != 0)  // يمنع الكسور
            {
                e.Cancel = true;
                _errorProvider.SetError(cell, "يجب أن يكون عددًا صحيحًا");
            }
        }
    }
}
```

---

## 4) Pricing Policy

### القاعدة
- **جميع الأسعار Manual بالكامل**
- **لا يوجد أي derived pricing أو auto pricing**
- **النظام فقط يحسب التحويلات والكميات وليس الأسعار**

### التطبيق
```csharp
public class ProductUnit
{
    public decimal SellPrice { get; set; }    // سعر يدوي بالكامل
    public decimal CostPrice { get; set; }    // سعر يدوي بالكامل
}

// لا يوجد حساب تلقائي للأسعار
// السعر يُدخل يدويًا لكل وحدة
```

---

## 5) Barcode Strategy

### القاعدة
- **الباركود unique عالميًا على مستوى النظام بالكامل**
- **لا يسمح بتكرار الباركود حتى عبر منتجات مختلفة**

### التطبيق
```sql
CREATE UNIQUE INDEX idx_product_unit_barcodes_unique 
ON ProductUnitBarcodes(barcode);

-- Validation
CREATE TRIGGER tr_product_unit_barcodes_unique
BEFORE INSERT ON ProductUnitBarcodes
BEGIN
    SELECT CASE
        WHEN EXISTS (
            SELECT 1 FROM ProductUnitBarcodes 
            WHERE barcode = NEW.barcode
        ) THEN
            RAISE(ABORT, 'الباركود موجود مسبقًا')
    END;
END;
```

---

## 6) MaxDepth Strategy

### القاعدة
- **hierarchy limit = 3 فقط**
- **Base → Pack → Carton**
- **لا يسمح بأعماق أكبر**

### التطبيق
```csharp
public class ProductUnitHierarchyService
{
    private const int MAX_DEPTH = 3;
    
    public ProductUnitHierarchyValidationResult ValidateAndCompute(List<ProductUnit> units)
    {
        foreach (var unit in units)
        {
            var depth = CalculateDepth(unit, units);
            if (depth > MAX_DEPTH)
            {
                return new ProductUnitHierarchyValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> 
                    { 
                        $"الوحدة {unit.UnitName} تتجاوز الحد الأقصى للعمق ({MAX_DEPTH})" 
                    }
                };
            }
        }
        // ...
    }
}
```

---

## 7) Integration Map

### 7.1) POS Screen

| التأثير | التفاصيل |
|---------|----------|
| **الوحدة الحالية** | عرض اسم الوحدة المحددة |
| **الكميات** | إدخال بوحدة المستخدم، تحويل تلقائي للـBase |
| **الباركود** | قراءة الباركود، تحديد الوحدة تلقائيًا |
| **الإجمالي** | حساب السعر بناءً على سعر الوحدة المحددة |
| **الحفظ** | حفظ BaseQty + UnitQty + Factor |

### 7.2) Purchases Screen

| التأثير | التفاصيل |
|---------|----------|
| **اختيار الوحدة** | ComboBox بالوحدات المتاحة |
| **التحويل للمخزون** | تحويل تلقائي للـBase عند الحفظ |
| **التكلفة** | إدخال تكلفة الوحدة المحددة |
| **الحفظ** | حفظ BaseQty + UnitQty + Factor |

### 7.3) Returns Screen

| التأثير | التفاصيل |
|---------|----------|
| **المرتجعات** | عرض الوحدات الأصلية من الفاتورة |
| **التحويل** | تحويل تلقائي للـBase عند الحفظ |
| **التحقق** | التأكد من عدم تجاوز الكميات المرتجعة |

### 7.4) StockHistory

| التأثير | التفاصيل |
|---------|----------|
| **أعمدة جديدة** | unit_name, unit_qty, factor |
| **العرض** | إمكانية العرض بالوحدة المستخدمة أو الأساسية |
| **التلخيص** | دائمًا بالوحدة الأساسية |

### 7.5) Reports

| التأثير | التفاصيل |
|---------|----------|
| **عرض التقارير** | خيار العرض بأي وحدة |
| **الحسابات** | دائمًا بالوحدة الأساسية |
| **التحويل** | تحويل تلقائي للوحدة المطلوبة |

### 7.6) BarcodeResolver

| التأثير | التفاصيل |
|---------|----------|
| **النتيجة** | إرجاع ProductUnitLookup بالوحدة المحددة |
| **التحقق** | التأكد من تفرد الباركود |
| **الأولوية** | الباركود الافتراضي للوحدة أولاً |

### 7.7) Inventory Valuation

| التأثير | التفاصيل |
|---------|----------|
| **التقييم** | دائمًا بالوحدة الأساسية |
| **التكلفة** | متوسط التكلفة بالوحدة الأساسية |
| **العرض** | إمكانية العرض بأي وحدة للعرض فقط |

---

## 8) DTOs & Contracts النهائية

### 8.1) SaleLine
```csharp
public class SaleLine
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int ProductUnitId { get; set; }
    public string UnitNameSnapshot { get; set; }
    public decimal FactorSnapshot { get; set; }
    public string BarcodeSnapshot { get; set; }
    public decimal SellPriceSnapshot { get; set; }
    public decimal BaseQty { get; set; }        // دائمًا بالوحدة الأساسية
    public decimal UnitQty { get; set; }        // الكمية بالوحدة المستخدمة
    public decimal Total { get; set; }
}
```

### 8.2) PurchaseLine
```csharp
public class PurchaseLine
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int ProductUnitId { get; set; }
    public string UnitNameSnapshot { get; set; }
    public decimal FactorSnapshot { get; set; }
    public decimal CostPriceSnapshot { get; set; }
    public decimal BaseQty { get; set; }        // دائمًا بالوحدة الأساسية
    public decimal UnitQty { get; set; }        // الكمية بالوحدة المستخدمة
    public decimal Total { get; set; }
}
```

### 8.3) StockTransaction
```csharp
public class StockTransaction
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int ProductUnitId { get; set; }
    public string UnitNameSnapshot { get; set; }
    public decimal FactorSnapshot { get; set; }
    public decimal BaseQty { get; set; }        // دائمًا بالوحدة الأساسية
    public decimal UnitQty { get; set; }        // الكمية بالوحدة المستخدمة
    public string TransactionType { get; set; }  // IN/OUT/ADJUST
    public string Reason { get; set; }
    public DateTime TransactionDate { get; set; }
    public string CreatedBy { get; set; }
}
```

### 8.4) ProductUnitLookup
```csharp
public class ProductUnitLookup
{
    public int ProductId { get; set; }
    public int ProductUnitId { get; set; }
    public string ProductName { get; set; }
    public string UnitName { get; set; }
    public decimal Factor { get; set; }
    public decimal SellPrice { get; set; }
    public decimal CostPrice { get; set; }
    public string Barcode { get; set; }
    public bool IsDefaultBarcode { get; set; }
    public string BarcodeType { get; set; }
    public bool IsBaseUnit { get; set; }
}
```

### 8.5) BarcodeLookupResult
```csharp
public class BarcodeLookupResult
{
    public bool IsFound { get; set; }
    public int ProductId { get; set; }
    public int ProductUnitId { get; set; }
    public string ProductName { get; set; }
    public string UnitName { get; set; }
    public decimal Factor { get; set; }
    public decimal SellPrice { get; set; }
    public decimal Qty { get; set; }           // الكمية بالباركود (لـ WEIGHT_EAN13)
    public string BarcodeType { get; set; }
    public string Message { get; set; }
}
```

---

## 9) Migration Strategy

### 9.1) المرحلة 1: إضافة الأعمدة الجديدة
```sql
-- إضافة الأعمدة الجديدة لـProductUnits
ALTER TABLE ProductUnits ADD COLUMN parent_product_unit_id INTEGER;
ALTER TABLE ProductUnits ADD COLUMN pack_size DECIMAL(10,3);

-- إضافة الأعمدة الجديدة للجداول الحركية
ALTER TABLE Order_Details ADD COLUMN product_unit_id INTEGER;
ALTER TABLE Order_Details ADD COLUMN unit_name_snapshot VARCHAR(50);
ALTER TABLE Order_Details ADD COLUMN factor_snapshot DECIMAL(10,3);
ALTER TABLE Order_Details ADD COLUMN barcode_snapshot VARCHAR(50);
ALTER TABLE Order_Details ADD COLUMN sell_price_snapshot DECIMAL(10,3);
ALTER TABLE Order_Details ADD COLUMN base_qty DECIMAL(10,3);
ALTER TABLE Order_Details ADD COLUMN unit_qty DECIMAL(10,3);

-- نفس التعديلات لـPurchaseDetails و StockHistory
```

### 9.2) المرحلة 2: ترحيل البيانات الحالية
```sql
-- تعبئة parent_product_unit_id و pack_size
UPDATE ProductUnits SET 
    parent_product_unit_id = NULL,
    pack_size = NULL
WHERE factor = 1;  -- الوحدات الأساسية

-- ترحيل البيانات الحالية للفواتير
UPDATE Order_Details SET
    product_unit_id = (SELECT id FROM ProductUnits WHERE product_id = Order_Details.product_id LIMIT 1),
    unit_name_snapshot = (SELECT u.unit_name FROM Units u JOIN ProductUnits pu ON u.id = pu.unit_id WHERE pu.product_id = Order_Details.product_id LIMIT 1),
    factor_snapshot = 1,
    barcode_snapshot = (SELECT barcode FROM ProductUnitBarcodes WHERE product_unit_id = product_unit_id AND is_default = 1),
    sell_price_snapshot = Order_Details.price,
    base_qty = Order_Details.qty,
    unit_qty = Order_Details.qty;
```

### 9.3) المرحلة 3: تحديث Triggers والـIndexes
```sql
-- إضافة الـIndexes الجديدة
CREATE INDEX idx_product_units_parent ON ProductUnits(parent_product_unit_id);
CREATE INDEX idx_product_units_product_parent ON ProductUnits(product_id, parent_product_unit_id);

-- تحديث الـTriggers للـBaseQty
CREATE TRIGGER tr_order_details_base_qty
BEFORE INSERT ON Order_Details
BEGIN
    UPDATE Order_Details SET 
        base_qty = unit_qty * factor_snapshot
    WHERE id = NEW.id;
END;
```

### 9.4) المرحلة 4: التحقق والاختبار
- تشغيل self-test harness
- التحقق من سلامة البيانات
- اختبار التحويلات
- اختبار الفواتير القديمة

---

## 10) التنفيذ النهائي

### 10.1) ترتيب التنفيذ
1. **Database Migration** - المراحل الأربع
2. **Domain Services** - ProductUnitHierarchyService
3. **Repositories** - تحديث الـRepositories الحالية
4. **DTOs** - تعريف الـDTOs الجديدة
5. **UI Forms** - ProductUnitsForm بالتصميم الجديد
6. **Integration** - تحديث الشاشات الأخرى
7. **Testing** - End-to-end tests

### 10.2) النقاط الحرجة
- **Backup كامل للبيانات قبل البدء**
- **اختبار الترحيل على نسخة تجريبية**
- **التأكد من عدم كسر الـPOS الحالي**
- **اختبار الفواتير القديمة بعد الترحيل**

### 10.3) Rollback Plan
- **Database backup restore**
- **Revert code changes**
- **Verify POS functionality**

---

## خلاصة

هذه الوثيقة تحدد:
- **BaseQuantity Strategy** - جميع الحركات بالوحدة الأساسية
- **Invoice Snapshot** - حفظ كامل البيانات وقت المعاملة
- **Decimal Policy** - UI يمنع الكسور، DB تستخدم decimal
- **Pricing Policy** - أسعار يدوية بالكامل
- **Barcode Strategy** - unique عالميًا
- **MaxDepth** - 3 مستويات كحد أقصى
- **Integration Map** - تأثير النظام على كل الشاشات
- **DTOs** - تعريفات النهائية
- **Migration** - استراتيجية آمنة للترحيل

**بعد اعتماد هذه الوثيقة، يمكن البدء في التنفيذ النهائي للنظام.**
