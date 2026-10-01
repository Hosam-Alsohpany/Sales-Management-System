using System;
using System.Data.SQLite;
using Sales.Models;
using Sales.Services;

namespace Sales.Utilities
{
    /// <summary>
    /// قراءة صف Products من SQLite إلى نموذج Product (بدون أعمدة legacy).
    /// </summary>
    public static class ProductRowMapper
    {
        public static Product FromReader(SQLiteDataReader dr, string labelColumn = "label")
        {
            if (dr == null) throw new ArgumentNullException(nameof(dr));

            long qtyScaled = 0;
            int scalePow = 0;
            try
            {
                if (HasColumn(dr, "qty_scaled") && dr["qty_scaled"] != DBNull.Value)
                    qtyScaled = Convert.ToInt64(dr["qty_scaled"]);
                else if (HasColumn(dr, "qty") && dr["qty"] != DBNull.Value)
                {
                    int legacyQty = Convert.ToInt32(dr["qty"]);
                    scalePow = HasColumn(dr, "qty_scale_pow10") && dr["qty_scale_pow10"] != DBNull.Value
                        ? ProductQuantityHelper.ClampScalePow10(Convert.ToInt32(dr["qty_scale_pow10"]))
                        : 0;
                    qtyScaled = ProductQuantityHelper.BaseQtyToScaled(legacyQty, scalePow);
                }
            }
            catch { }

            try
            {
                if (HasColumn(dr, "qty_scale_pow10") && dr["qty_scale_pow10"] != DBNull.Value)
                    scalePow = ProductQuantityHelper.ClampScalePow10(Convert.ToInt32(dr["qty_scale_pow10"]));
            }
            catch { }

            string productType = ProductTypeHelper.Physical;
            try
            {
                if (HasColumn(dr, "product_type") && dr["product_type"] != DBNull.Value)
                    productType = ProductTypeHelper.NormalizeProductType(dr["product_type"].ToString());
            }
            catch { }

            return new Product
            {
                Id = Convert.ToInt32(dr["id"]),
                Sku = HasColumn(dr, "sku") && dr["sku"] != DBNull.Value ? dr["sku"].ToString() : string.Empty,
                Label = dr[labelColumn].ToString(),
                QtyScaled = qtyScaled,
                QtyScalePow10 = scalePow,
                ProductType = productType,
                Price = HasColumn(dr, "price") && dr["price"] != DBNull.Value ? Convert.ToDecimal(dr["price"]) : 0m,
                CostPrice = HasColumn(dr, "cost_price") && dr["cost_price"] != DBNull.Value ? Convert.ToDecimal(dr["cost_price"]) : 0m,
                MinQty = HasColumn(dr, "min_qty") && dr["min_qty"] != DBNull.Value ? Convert.ToDecimal(dr["min_qty"]) : 0m,
                CategoryId = HasColumn(dr, "category_id") && dr["category_id"] != DBNull.Value ? (int?)Convert.ToInt32(dr["category_id"]) : null,
                CategoryName = HasColumn(dr, "category_name") && dr["category_name"] != DBNull.Value ? dr["category_name"].ToString() : string.Empty,
                Note = HasColumn(dr, "note") && dr["note"] != DBNull.Value ? dr["note"].ToString() : string.Empty,
                CreatedBy = HasColumn(dr, "created_by") && dr["created_by"] != DBNull.Value ? dr["created_by"].ToString() : string.Empty,
                ExpiryDate = HasColumn(dr, "expiry_date") && dr["expiry_date"] != DBNull.Value ? dr["expiry_date"].ToString() : string.Empty,
                CreatedAt = HasColumn(dr, "created_at") && dr["created_at"] != DBNull.Value ? dr["created_at"].ToString() : string.Empty,
                UpdatedAt = HasColumn(dr, "updated_at") && dr["updated_at"] != DBNull.Value ? dr["updated_at"].ToString() : string.Empty,
                ImagePath = HasColumn(dr, "image_path") && dr["image_path"] != DBNull.Value ? dr["image_path"].ToString() : string.Empty,
                DefaultBarcode = HasColumn(dr, "barcode") && dr["barcode"] != DBNull.Value ? dr["barcode"].ToString()
                    : HasColumn(dr, "default_barcode") && dr["default_barcode"] != DBNull.Value ? dr["default_barcode"].ToString() : string.Empty
            };
        }

        private static bool HasColumn(SQLiteDataReader dr, string column)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (string.Equals(dr.GetName(i), column, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
