// ============================================================
// الملف    : StockService.cs
// الغرض    : إدارة ومعالجة حركات المخزون والمواد المتبقية
// ============================================================

using System;
using System.Diagnostics;
using System.Data.SQLite;
using Sales.Database;
using Sales.Utilities;

namespace Sales.Services
{
    public static class StockService
    {
        public enum MovementDirection
        {
            Increase = 1,
            Decrease = 2
        }

        public enum StockOperationType
        {
            Sale = 1,
            Purchase = 2,
            Adjustment = 3,
            Transfer = 4,
            Return = 5
        }

        public sealed class StockApplyResult
        {
            public int ProductId { get; set; }
            public int ScalePow10 { get; set; }
            public long AppliedDeltaScaled { get; set; }
            public long AppliedDeltaScaledAbs { get; set; }
            public long NewQtyScaled { get; set; }
        }

        public sealed class StockMovementFromBaseQtyContext
        {
            public int ProductId { get; set; }
            public decimal BaseQty { get; set; }
            public MovementDirection Direction { get; set; }
            public StockOperationType OperationType { get; set; }
            public string ReferenceType { get; set; }
            public long? ReferenceId { get; set; }
            public string Reason { get; set; }
            public string UserName { get; set; }
        }

        public sealed class StockMovementContext
        {
            public int ProductId { get; set; }
            public long DeltaScaled { get; set; }
            public StockOperationType OperationType { get; set; }
            public string ReferenceType { get; set; }
            public long? ReferenceId { get; set; }
            public string Reason { get; set; }
            public string UserName { get; set; }
        }

        private static void EnsureCalledFromSalesTransactionService(string api)
        {
            // Hard guardrail: StockService must be invoked only via SalesTransactionService
            var st = new StackTrace();
            bool ok = false;
            for (int i = 0; i < st.FrameCount; i++)
            {
                var m = st.GetFrame(i)?.GetMethod();
                var t = m?.DeclaringType;
                if (t == null) continue;
                if (t.FullName == "Sales.Services.SalesTransactionService")
                {
                    ok = true;
                    break;
                }
            }

            if (ok) return;

#if DEBUG
            try
            {
                Logger.LogError("StockService violation: " + api + " called outside SalesTransactionService", new InvalidOperationException(st.ToString()));
            }
            catch
            {
            }
#endif
            throw new InvalidOperationException("StockService violation: must be called via SalesTransactionService");
        }

        private static long ConvertBaseQtyToScaled(int productId, decimal baseQty, SQLiteConnection con = null, SQLiteTransaction tran = null)
        {
            if (productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));

            if (con == null)
            {
                using (var c = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    c.Open();
                    using (var t = c.BeginTransaction())
                    {
                        long v = ConvertBaseQtyToScaled(productId, baseQty, c, t);
                        t.Commit();
                        return v;
                    }
                }
            }

            int pow10 = 0;
            using (var cmd = new SQLiteCommand("SELECT qty_scale_pow10 FROM Products WHERE id=@id", con, tran))
            {
                cmd.Parameters.AddWithValue("@id", productId);
                object sv = cmd.ExecuteScalar();
                pow10 = sv == null || sv == DBNull.Value ? 0 : Convert.ToInt32(sv);
            }

            if (pow10 < 0) pow10 = 0;
            if (pow10 > 6) pow10 = 6;

            decimal scale = 1m;
            for (int i = 0; i < pow10; i++) scale *= 10m;

            decimal scaledDec = baseQty * scale;
            try
            {
                return Convert.ToInt64(Math.Round(scaledDec, 0, MidpointRounding.AwayFromZero));
            }
            catch
            {
                throw new InvalidOperationException("Invalid quantity");
            }
        }

        public static StockApplyResult PreviewScaledFromBaseQty(int productId, decimal baseQty, SQLiteConnection con = null, SQLiteTransaction tran = null)
        {
            EnsureCalledFromSalesTransactionService(nameof(PreviewScaledFromBaseQty));
            if (productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
            if (baseQty < 0m) throw new InvalidOperationException("Invalid quantity");

            if (con == null)
            {
                using (var c = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    c.Open();
                    using (var t = c.BeginTransaction())
                    {
                        StockApplyResult r = PreviewScaledFromBaseQty(productId, baseQty, c, t);
                        t.Commit();
                        return r;
                    }
                }
            }

            int pow10;
            using (var cmd = new SQLiteCommand("SELECT qty_scale_pow10 FROM Products WHERE id=@id", con, tran))
            {
                cmd.Parameters.AddWithValue("@id", productId);
                object sv = cmd.ExecuteScalar();
                pow10 = sv == null || sv == DBNull.Value ? 0 : Convert.ToInt32(sv);
            }

            if (pow10 < 0) pow10 = 0;
            if (pow10 > 6) pow10 = 6;

            long deltaScaled = ConvertBaseQtyToScaled(productId, baseQty, con, tran);
            if (deltaScaled < 0) deltaScaled = 0;

            return new StockApplyResult
            {
                ProductId = productId,
                ScalePow10 = pow10,
                AppliedDeltaScaled = deltaScaled,
                AppliedDeltaScaledAbs = Math.Abs(deltaScaled),
                NewQtyScaled = 0
            };
        }

        public static StockApplyResult ApplyStockMovementFromBaseQty(StockMovementFromBaseQtyContext ctx, SQLiteConnection con = null, SQLiteTransaction tran = null)
        {
            EnsureCalledFromSalesTransactionService(nameof(ApplyStockMovementFromBaseQty));
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (ctx.ProductId <= 0) throw new ArgumentOutOfRangeException(nameof(ctx.ProductId));
            if (ctx.BaseQty <= 0m) throw new InvalidOperationException("BaseQty must be > 0");
            if (string.IsNullOrWhiteSpace(ctx.UserName)) throw new InvalidOperationException("UserName is required for stock operations");

            if (con == null)
            {
                using (var c = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    c.Open();
                    using (var t = c.BeginTransaction())
                    {
                        StockApplyResult r = ApplyStockMovementFromBaseQty(ctx, c, t);
                        t.Commit();
                        return r;
                    }
                }
            }

            long deltaAbs = ConvertBaseQtyToScaled(ctx.ProductId, ctx.BaseQty, con, tran);
            if (deltaAbs <= 0) throw new InvalidOperationException("BaseQty بعد التحويل أصبحت غير صالحة");

            long signedDelta = ctx.Direction == MovementDirection.Decrease ? -deltaAbs : deltaAbs;

            StockApplyResult applied = ApplyStockMovementCore(new StockMovementContext
            {
                ProductId = ctx.ProductId,
                DeltaScaled = signedDelta,
                OperationType = ctx.OperationType,
                ReferenceType = ctx.ReferenceType,
                ReferenceId = ctx.ReferenceId,
                Reason = ctx.Reason,
                UserName = ctx.UserName
            }, con, tran);

            return applied;
        }

        public static StockApplyResult ApplyStockMovement(StockMovementContext ctx, SQLiteConnection con = null, SQLiteTransaction tran = null)
        {
            // Hard rule: deltaScaled API is forbidden. Only baseQty API allowed via SalesTransactionService.
            throw new InvalidOperationException("Forbidden API: use ApplyStockMovementFromBaseQty via SalesTransactionService");
        }

        private static StockApplyResult ApplyStockMovementCore(StockMovementContext ctx, SQLiteConnection con = null, SQLiteTransaction tran = null)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (ctx.ProductId <= 0) throw new ArgumentOutOfRangeException(nameof(ctx.ProductId));
            if (ctx.DeltaScaled == 0)
            {
                return new StockApplyResult
                {
                    ProductId = ctx.ProductId,
                    ScalePow10 = 0,
                    AppliedDeltaScaled = 0,
                    AppliedDeltaScaledAbs = 0,
                    NewQtyScaled = 0
                };
            }
            if (string.IsNullOrWhiteSpace(ctx.UserName)) throw new InvalidOperationException("UserName is required for stock operations");

            if (con == null)
            {
                using (var c = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                {
                    c.Open();
                    using (var t = c.BeginTransaction())
                    {
                        StockApplyResult r = ApplyStockMovement(ctx, c, t);
                        t.Commit();
                        return r;
                    }
                }

                // Unreachable
            }

            const string sqlUpdateNonNegative = @"
UPDATE Products
SET qty_scaled = qty_scaled + @delta
WHERE id = @id
  AND qty_scaled + @delta >= 0;";

            using (var cmd = new SQLiteCommand(sqlUpdateNonNegative, con, tran))
            {
                cmd.Parameters.AddWithValue("@delta", ctx.DeltaScaled);
                cmd.Parameters.AddWithValue("@id", ctx.ProductId);
                int affected = cmd.ExecuteNonQuery();
                if (affected == 0)
                    throw new InvalidOperationException("Insufficient stock");
            }

            long newQtyScaled;
            using (var cmd = new SQLiteCommand("SELECT qty_scaled FROM Products WHERE id=@id", con, tran))
            {
                cmd.Parameters.AddWithValue("@id", ctx.ProductId);
                object v = cmd.ExecuteScalar();
                if (v == null || v == DBNull.Value)
                    throw new InvalidOperationException("المنتج غير موجود: " + ctx.ProductId);
                newQtyScaled = Convert.ToInt64(v);
            }

            int scalePow10 = 0;
            using (var cmdScale = new SQLiteCommand("SELECT qty_scale_pow10 FROM Products WHERE id=@id", con, tran))
            {
                cmdScale.Parameters.AddWithValue("@id", ctx.ProductId);
                object sv = cmdScale.ExecuteScalar();
                scalePow10 = sv == null || sv == DBNull.Value ? 0 : ProductQuantityHelper.ClampScalePow10(Convert.ToInt32(sv));
            }

            long deltaScaledLog = ctx.DeltaScaled;
            if (deltaScaledLog > int.MaxValue) deltaScaledLog = int.MaxValue;
            if (deltaScaledLog < int.MinValue) deltaScaledLog = int.MinValue;

            int deltaLegacy = ProductQuantityHelper.LegacyQtyFromScaled(deltaScaledLog, scalePow10);

            string reason = string.IsNullOrWhiteSpace(ctx.Reason)
                ? ctx.OperationType.ToString()
                : ctx.Reason;

            const string sqlLog = @"INSERT INTO StockHistory (product_id, qty_change, qty_change_scaled, qty_scale_pow10, reason, change_date, user_name)
VALUES (@pid, @qty, @qty_scaled, @pow, @reason, CURRENT_TIMESTAMP, @user)";
            using (var cmd = new SQLiteCommand(sqlLog, con, tran))
            {
                cmd.Parameters.AddWithValue("@pid", ctx.ProductId);
                cmd.Parameters.AddWithValue("@qty", deltaLegacy);
                cmd.Parameters.AddWithValue("@qty_scaled", (int)deltaScaledLog);
                cmd.Parameters.AddWithValue("@pow", scalePow10);
                cmd.Parameters.AddWithValue("@reason", (object)reason ?? string.Empty);
                cmd.Parameters.AddWithValue("@user", (object)ctx.UserName ?? string.Empty);
                cmd.ExecuteNonQuery();
            }

            // Legacy qty column sync removed — Products.qty no longer exists after batch1 migration.
            return new StockApplyResult
            {
                ProductId = ctx.ProductId,
                ScalePow10 = scalePow10,
                AppliedDeltaScaled = ctx.DeltaScaled,
                AppliedDeltaScaledAbs = Math.Abs(ctx.DeltaScaled),
                NewQtyScaled = newQtyScaled
            };
        }

        public static void SetQty(int productId, int newQty, int? expectedOldQty, string reason, string userName, SQLiteConnection con = null, SQLiteTransaction tran = null)
        {
            throw new InvalidOperationException("Forbidden API: stock mutations must go through SalesTransactionService");
        }

        public static void Decrease(int productId, int qty, string reason, string userName, SQLiteConnection con = null, SQLiteTransaction tran = null)
        {
            throw new InvalidOperationException("Forbidden API: stock mutations must go through SalesTransactionService");
        }

        public static void Increase(int productId, int qty, string reason, string userName, SQLiteConnection con = null, SQLiteTransaction tran = null)
        {
            throw new InvalidOperationException("Forbidden API: stock mutations must go through SalesTransactionService");
        }
    }
}
