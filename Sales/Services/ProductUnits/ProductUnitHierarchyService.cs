// ============================================================
// الملف    : ProductUnitHierarchyService.cs
// الغرض    : خدمة إدارة وحساب هرمية الوحدات والنسب التحويلية بينها لتحديث المخزون والأسعار
// ============================================================

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sales.Services.ProductUnits
{
    public class ProductUnitHierarchyService
    {
        public ProductUnitHierarchyValidationResult ValidateAndCompute(
            int productId,
            IList<ProductUnitHierarchyNode> nodes,
            int? originalRootProductUnitId,
            bool hasAnyTransactions,
            ProductUnitHierarchyServiceOptions options)
        {
            var result = new ProductUnitHierarchyValidationResult();

            if (productId <= 0)
            {
                result.AddError("INVALID_PRODUCT", "ProductId غير صحيح", null, null);
                return result;
            }

            if (nodes == null)
            {
                result.AddError("NULL_NODES", "لا توجد وحدات", null, null);
                return result;
            }

            if (options == null)
                options = ProductUnitHierarchyServiceOptions.Default();

            var byId = new Dictionary<int, ProductUnitHierarchyNode>();
            var unitKeySet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var n in nodes)
            {
                if (n == null) continue;

                if (n.ProductId != productId)
                {
                    result.AddError("MIXED_PRODUCT", "يوجد صف لوحدة من منتج مختلف", n.ProductUnitId > 0 ? (int?)n.ProductUnitId : null, n.UnitId > 0 ? (int?)n.UnitId : null);
                    continue;
                }

                if (n.ProductUnitId <= 0)
                {
                    result.AddError("INVALID_PRODUCT_UNIT_ID", "ProductUnitId غير صحيح", null, n.UnitId > 0 ? (int?)n.UnitId : null);
                    continue;
                }

                if (n.UnitId <= 0)
                {
                    result.AddError("INVALID_UNIT_ID", "UnitId غير صحيح", n.ProductUnitId, null);
                    continue;
                }

                if (byId.ContainsKey(n.ProductUnitId))
                {
                    result.AddError("DUPLICATE_PRODUCT_UNIT_ID", "تكرار ProductUnitId داخل البيانات", n.ProductUnitId, n.UnitId);
                    continue;
                }

                byId[n.ProductUnitId] = n;

                string unitKey = productId.ToString(CultureInfo.InvariantCulture) + ":" + n.UnitId.ToString(CultureInfo.InvariantCulture);
                if (!unitKeySet.Add(unitKey))
                    result.AddError("DUPLICATE_UNIT", "لا يمكن تكرار نفس الوحدة داخل نفس المنتج", n.ProductUnitId, n.UnitId);

                if (n.ParentProductUnitId.HasValue && n.ParentProductUnitId.Value == n.ProductUnitId)
                    result.AddError("SELF_PARENT", "لا يمكن أن تكون الوحدة أب لنفسها", n.ProductUnitId, n.UnitId);
            }

            if (result.Errors.Count > 0)
                return result;

            int rootCount = 0;
            int rootId = 0;

            foreach (var kv in byId)
            {
                var n = kv.Value;
                if (!n.ParentProductUnitId.HasValue)
                {
                    rootCount++;
                    rootId = n.ProductUnitId;
                }
            }

            if (rootCount != 1)
            {
                result.AddError("ROOT_COUNT", "يجب أن يوجد Root واحد فقط (parent=NULL)", null, null);
                return result;
            }

            if (hasAnyTransactions && originalRootProductUnitId.HasValue && originalRootProductUnitId.Value > 0 && originalRootProductUnitId.Value != rootId)
            {
                result.AddError("ROOT_CHANGE_BLOCKED", "لا يمكن تغيير Root بعد وجود حركات", rootId, null);
                return result;
            }

            foreach (var kv in byId)
            {
                var n = kv.Value;
                if (!n.ParentProductUnitId.HasValue) continue;

                int parentId = n.ParentProductUnitId.Value;
                if (!byId.ContainsKey(parentId))
                {
                    result.AddError("ORPHAN", "Parent غير موجود داخل نفس المنتج", n.ProductUnitId, n.UnitId);
                    continue;
                }

                var p = byId[parentId];
                if (p.ProductId != productId)
                    result.AddError("PARENT_OTHER_PRODUCT", "Parent من منتج مختلف", n.ProductUnitId, n.UnitId);

                if (!n.PackSize.HasValue || n.PackSize.Value <= 0m)
                    result.AddError("PACK_SIZE_INVALID", "PackSize يجب أن يكون أكبر من صفر", n.ProductUnitId, n.UnitId);
            }

            if (result.Errors.Count > 0)
                return result;

            var visiting = new HashSet<int>();
            var visited = new HashSet<int>();

            foreach (var kv in byId)
            {
                int id = kv.Key;
                if (!visited.Contains(id))
                {
                    int depth = 0;
                    if (!DfsDetectCycles(id, byId, visiting, visited, result, ref depth, options.MaxDepth))
                        return result;
                }
            }

            if (result.Errors.Count > 0)
                return result;

            var memo = new Dictionary<int, decimal>();
            foreach (var kv in byId)
            {
                int id = kv.Key;
                int depth = 0;
                decimal f;
                if (!TryComputeFactor(id, byId, memo, options, ref depth, out f, result))
                    return result;

                result.ComputedFactorsByProductUnitId[id] = f;
                result.AffectedProductUnitIds.Add(id);
            }

            return result;
        }

        private bool DfsDetectCycles(
            int id,
            Dictionary<int, ProductUnitHierarchyNode> byId,
            HashSet<int> visiting,
            HashSet<int> visited,
            ProductUnitHierarchyValidationResult result,
            ref int depth,
            int maxDepth)
        {
            if (depth > maxDepth)
            {
                result.AddError("DEPTH_LIMIT", "تداخل هرمي عميق جدًا", id, null);
                return false;
            }

            if (visiting.Contains(id))
            {
                result.AddError("CYCLE", "يوجد Cycle في hierarchy", id, null);
                return false;
            }

            if (visited.Contains(id))
                return true;

            visiting.Add(id);

            var n = byId[id];
            if (n.ParentProductUnitId.HasValue)
            {
                int parentId = n.ParentProductUnitId.Value;
                depth++;
                bool ok = DfsDetectCycles(parentId, byId, visiting, visited, result, ref depth, maxDepth);
                depth--;
                if (!ok) return false;
            }

            visiting.Remove(id);
            visited.Add(id);
            return true;
        }

        private bool TryComputeFactor(
            int id,
            Dictionary<int, ProductUnitHierarchyNode> byId,
            Dictionary<int, decimal> memo,
            ProductUnitHierarchyServiceOptions options,
            ref int depth,
            out decimal factor,
            ProductUnitHierarchyValidationResult result)
        {
            factor = 0m;

            if (depth > options.MaxDepth)
            {
                result.AddError("DEPTH_LIMIT", "تداخل هرمي عميق جدًا", id, null);
                return false;
            }

            decimal cached;
            if (memo.TryGetValue(id, out cached))
            {
                factor = cached;
                return true;
            }

            var n = byId[id];
            if (!n.ParentProductUnitId.HasValue)
            {
                factor = 1m;
                memo[id] = factor;
                return true;
            }

            int parentId = n.ParentProductUnitId.Value;
            decimal pack = n.PackSize.HasValue ? n.PackSize.Value : 0m;

            decimal parentFactor;
            depth++;
            bool okParent = TryComputeFactor(parentId, byId, memo, options, ref depth, out parentFactor, result);
            depth--;

            if (!okParent) return false;

            if (pack <= 0m)
            {
                result.AddError("PACK_SIZE_INVALID", "PackSize يجب أن يكون أكبر من صفر", id, n.UnitId);
                return false;
            }

            factor = parentFactor * pack;
            if (factor <= 0m)
            {
                result.AddError("FACTOR_INVALID", "Factor الناتج غير صحيح", id, n.UnitId);
                return false;
            }

            if (factor > options.MaxFactor)
            {
                result.AddError("FACTOR_OVERFLOW", "Factor الناتج كبير جدًا", id, n.UnitId);
                return false;
            }

            memo[id] = factor;
            return true;
        }
    }
}
