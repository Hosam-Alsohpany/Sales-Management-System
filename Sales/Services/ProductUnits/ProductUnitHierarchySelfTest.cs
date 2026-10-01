// ============================================================
// الملف    : ProductUnitHierarchySelfTest.cs
// الغرض    : اختبارات كشف الحلقات الدائرية المتكررة غير الصحيحة في هرمية الوحدات
// ============================================================

using System;
using System.Collections.Generic;

namespace Sales.Services.ProductUnits
{
    public static class ProductUnitHierarchySelfTest
    {
        public static int RunAllOrThrow()
        {
            int passed = 0;
            Test_LinearChain_ComputesFactors(ref passed);
            Test_Cycle_Detected(ref passed);
            Test_Orphan_Detected(ref passed);
            Test_InvalidParent_Detected(ref passed);
            Test_DuplicateUnit_Detected(ref passed);
            Test_FactorRecompute_AfterPackChange(ref passed);
            Test_RootReplacement_Blocked_WhenTransactions(ref passed);
            return passed;
        }

        private static void Test_LinearChain_ComputesFactors(ref int passed)
        {
            var svc = new ProductUnitHierarchyService();
            var nodes = new List<ProductUnitHierarchyNode>
            {
                new ProductUnitHierarchyNode { ProductUnitId = 1, ProductId = 10, UnitId = 100, ParentProductUnitId = null, PackSize = null },
                new ProductUnitHierarchyNode { ProductUnitId = 2, ProductId = 10, UnitId = 101, ParentProductUnitId = 1, PackSize = 2m },
                new ProductUnitHierarchyNode { ProductUnitId = 3, ProductId = 10, UnitId = 102, ParentProductUnitId = 2, PackSize = 24m }
            };

            var r = svc.ValidateAndCompute(10, nodes, 1, false, ProductUnitHierarchyServiceOptions.Default());
            AssertTrue(r.IsValid, "LinearChain should be valid");
            AssertEq(r.ComputedFactorsByProductUnitId[1], 1m, "Root factor");
            AssertEq(r.ComputedFactorsByProductUnitId[2], 2m, "B factor");
            AssertEq(r.ComputedFactorsByProductUnitId[3], 48m, "C factor");
            passed++;
        }

        private static void Test_Cycle_Detected(ref int passed)
        {
            var svc = new ProductUnitHierarchyService();
            var nodes = new List<ProductUnitHierarchyNode>
            {
                new ProductUnitHierarchyNode { ProductUnitId = 1, ProductId = 10, UnitId = 100, ParentProductUnitId = 2, PackSize = 2m },
                new ProductUnitHierarchyNode { ProductUnitId = 2, ProductId = 10, UnitId = 101, ParentProductUnitId = 1, PackSize = 2m }
            };

            var r = svc.ValidateAndCompute(10, nodes, null, false, ProductUnitHierarchyServiceOptions.Default());
            AssertTrue(!r.IsValid, "Cycle should be invalid");
            passed++;
        }

        private static void Test_Orphan_Detected(ref int passed)
        {
            var svc = new ProductUnitHierarchyService();
            var nodes = new List<ProductUnitHierarchyNode>
            {
                new ProductUnitHierarchyNode { ProductUnitId = 1, ProductId = 10, UnitId = 100, ParentProductUnitId = null, PackSize = null },
                new ProductUnitHierarchyNode { ProductUnitId = 2, ProductId = 10, UnitId = 101, ParentProductUnitId = 999, PackSize = 2m }
            };

            var r = svc.ValidateAndCompute(10, nodes, 1, false, ProductUnitHierarchyServiceOptions.Default());
            AssertTrue(!r.IsValid, "Orphan should be invalid");
            passed++;
        }

        private static void Test_InvalidParent_Detected(ref int passed)
        {
            var svc = new ProductUnitHierarchyService();
            var nodes = new List<ProductUnitHierarchyNode>
            {
                new ProductUnitHierarchyNode { ProductUnitId = 1, ProductId = 10, UnitId = 100, ParentProductUnitId = null, PackSize = null },
                new ProductUnitHierarchyNode { ProductUnitId = 2, ProductId = 10, UnitId = 101, ParentProductUnitId = 0, PackSize = 2m }
            };

            var r = svc.ValidateAndCompute(10, nodes, 1, false, ProductUnitHierarchyServiceOptions.Default());
            AssertTrue(!r.IsValid, "Invalid parent should be invalid");
            passed++;
        }

        private static void Test_DuplicateUnit_Detected(ref int passed)
        {
            var svc = new ProductUnitHierarchyService();
            var nodes = new List<ProductUnitHierarchyNode>
            {
                new ProductUnitHierarchyNode { ProductUnitId = 1, ProductId = 10, UnitId = 100, ParentProductUnitId = null, PackSize = null },
                new ProductUnitHierarchyNode { ProductUnitId = 2, ProductId = 10, UnitId = 100, ParentProductUnitId = 1, PackSize = 2m }
            };

            var r = svc.ValidateAndCompute(10, nodes, 1, false, ProductUnitHierarchyServiceOptions.Default());
            AssertTrue(!r.IsValid, "Duplicate unit should be invalid");
            passed++;
        }

        private static void Test_FactorRecompute_AfterPackChange(ref int passed)
        {
            var svc = new ProductUnitHierarchyService();
            var nodes = new List<ProductUnitHierarchyNode>
            {
                new ProductUnitHierarchyNode { ProductUnitId = 1, ProductId = 10, UnitId = 100, ParentProductUnitId = null, PackSize = null },
                new ProductUnitHierarchyNode { ProductUnitId = 2, ProductId = 10, UnitId = 101, ParentProductUnitId = 1, PackSize = 3m },
                new ProductUnitHierarchyNode { ProductUnitId = 3, ProductId = 10, UnitId = 102, ParentProductUnitId = 2, PackSize = 10m }
            };

            var r = svc.ValidateAndCompute(10, nodes, 1, false, ProductUnitHierarchyServiceOptions.Default());
            AssertTrue(r.IsValid, "Recompute should be valid");
            AssertEq(r.ComputedFactorsByProductUnitId[2], 3m, "B factor");
            AssertEq(r.ComputedFactorsByProductUnitId[3], 30m, "C factor");
            passed++;
        }

        private static void Test_RootReplacement_Blocked_WhenTransactions(ref int passed)
        {
            var svc = new ProductUnitHierarchyService();
            var nodes = new List<ProductUnitHierarchyNode>
            {
                new ProductUnitHierarchyNode { ProductUnitId = 1, ProductId = 10, UnitId = 100, ParentProductUnitId = 2, PackSize = 2m },
                new ProductUnitHierarchyNode { ProductUnitId = 2, ProductId = 10, UnitId = 101, ParentProductUnitId = null, PackSize = null }
            };

            var r = svc.ValidateAndCompute(10, nodes, 1, true, ProductUnitHierarchyServiceOptions.Default());
            AssertTrue(!r.IsValid, "Root replacement should be blocked when transactions exist");
            passed++;
        }

        private static void AssertTrue(bool v, string message)
        {
            if (!v)
                throw new InvalidOperationException(message);
        }

        private static void AssertEq(decimal actual, decimal expected, string message)
        {
            if (actual != expected)
                throw new InvalidOperationException(message + " Expected=" + expected + " Actual=" + actual);
        }
    }
}
