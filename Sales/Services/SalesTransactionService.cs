// ============================================================
// الملف    : SalesTransactionService.cs
// الغرض    : خدمة التحكم وتنفيذ فواتير المبيعات وحفظها
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using Sales.Database;
using Sales.Models;
using Sales.Repositories;
using Sales.Utilities;

namespace Sales.Services
{
    public sealed class SalesTransactionService
    {
        private static decimal GetBaseQtyDecimal(OrderDetail item)
        {
            if (item == null) return 0m;
            if (item.BaseQty.HasValue) return item.BaseQty.Value;
            if (item.QtyUnit.HasValue) return item.QtyUnit.Value;
            return 0m;
        }

        private static void ApplyPricingToOrder(Order order)
        {
            if (order == null) return;
            decimal subtotal = 0m;
            if (order.Details != null)
            {
                foreach (var d in order.Details)
                {
                    if (d != null)
                        subtotal += d.Total;
                }
            }

            var totals = OrderPricingHelper.Compute(subtotal, order.Discount);
            order.Discount = totals.Discount;
            order.TaxAmount = totals.TaxAmount;
            order.Total = totals.GrandTotal;
        }

        private static void InsertOrderDetailLine(
            SQLiteConnection con,
            SQLiteTransaction tran,
            long orderId,
            OrderDetail item,
            StockService.StockApplyResult apply)
        {
            OrderLineSnapshotService.EnrichSnapshots(item, con, tran);

            decimal baseQtyDec = GetBaseQtyDecimal(item);
            const string sqlDetail = @"
                INSERT INTO Order_Details (
                    id_order, id_product, price, total,
                    product_unit_id, unit_name_snapshot, factor_snapshot,
                    qty_unit, base_qty, cost_price,
                    qty_scaled_snapshot, qty_scale_pow10_snapshot,
                    product_name_snapshot, barcode_snapshot, sell_price_snapshot
                )
                VALUES (
                    @order, @product, @price, @total,
                    @puid, @unit_name, @factor,
                    @qty_unit, @base_qty, @cost_price,
                    @qty_scaled_snapshot, @qty_scale_pow10_snapshot,
                    @pname, @barcode, @sell_snap
                )";

            using (SQLiteCommand cmd = new SQLiteCommand(sqlDetail, con, tran))
            {
                cmd.Parameters.AddWithValue("@order", orderId);
                cmd.Parameters.AddWithValue("@product", item.ProductId);
                cmd.Parameters.AddWithValue("@price", item.SellPriceSnapshot ?? item.Price);
                cmd.Parameters.AddWithValue("@total", item.Total);
                cmd.Parameters.AddWithValue("@puid", item.ProductUnitId.HasValue ? (object)item.ProductUnitId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@unit_name", (object)(item.UnitNameSnapshot ?? string.Empty));
                cmd.Parameters.AddWithValue("@factor", item.FactorSnapshot.HasValue ? (object)item.FactorSnapshot.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@qty_unit", item.QtyUnit.HasValue ? (object)item.QtyUnit.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@base_qty", baseQtyDec);
                cmd.Parameters.AddWithValue("@cost_price", item.CostPrice.HasValue ? (object)item.CostPrice.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@qty_scaled_snapshot", (object)apply.AppliedDeltaScaledAbs);
                cmd.Parameters.AddWithValue("@qty_scale_pow10_snapshot", (object)apply.ScalePow10);
                cmd.Parameters.AddWithValue("@pname", (object)(item.ProductNameSnapshot ?? string.Empty));
                cmd.Parameters.AddWithValue("@barcode", string.IsNullOrWhiteSpace(item.BarcodeSnapshot) ? (object)DBNull.Value : item.BarcodeSnapshot);
                cmd.Parameters.AddWithValue("@sell_snap", item.SellPriceSnapshot.HasValue ? (object)item.SellPriceSnapshot.Value : (object)item.Price);
                cmd.ExecuteNonQuery();
            }
        }

        private static decimal ReadBaseQtyFromDetailRow(SQLiteDataReader dr)
        {
            if (dr["base_qty"] != DBNull.Value)
                return Convert.ToDecimal(dr["base_qty"], CultureInfo.InvariantCulture);
            if (HasColumn(dr, "qty") && dr["qty"] != DBNull.Value)
                return Convert.ToDecimal(dr["qty"], CultureInfo.InvariantCulture);
            return 0m;
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

        /// <summary>
        /// تطبيق حركة مخزون للبيع/الاسترجاع مع تخطي منتجات الخدمة (service).
        /// </summary>
        private static StockService.StockApplyResult ApplyOrderStockMovement(
            StockService.StockMovementFromBaseQtyContext ctx,
            SQLiteConnection con,
            SQLiteTransaction tran)
        {
            if (ProductTypeHelper.IsServiceProduct(con, tran, ctx.ProductId))
            {
                var preview = StockService.PreviewScaledFromBaseQty(ctx.ProductId, ctx.BaseQty, con, tran);
                if (ctx.Direction == StockService.MovementDirection.Decrease)
                    return preview;

                preview.AppliedDeltaScaled = 0;
                preview.AppliedDeltaScaledAbs = 0;
                return preview;
            }

            return StockService.ApplyStockMovementFromBaseQty(ctx, con, tran);
        }

        private static void InsertOrderAuditLog(SQLiteConnection con, SQLiteTransaction tran, int? orderId, string action, string userName, string details)
        {
            const string sql = @"INSERT INTO OrderAuditLog (order_id, action, user_name, action_date, details)
                                 VALUES (@oid, @action, @user, CURRENT_TIMESTAMP, @details)";
            using (var cmd = new SQLiteCommand(sql, con, tran))
            {
                cmd.Parameters.AddWithValue("@oid", orderId.HasValue ? (object)orderId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@action", action ?? string.Empty);
                cmd.Parameters.AddWithValue("@user", userName ?? string.Empty);
                cmd.Parameters.AddWithValue("@details", details ?? string.Empty);
                cmd.ExecuteNonQuery();
            }
        }

        private static string BuildOrderSnapshotForAudit(SQLiteConnection con, SQLiteTransaction tran, int orderId)
        {
            try
            {
                string header = string.Empty;
                const string sqlHead = "SELECT id, order_date, customer_id, total, note, created_by FROM Orders WHERE id=@id";
                using (var cmd = new SQLiteCommand(sqlHead, con, tran))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            header =
                                "OrderId=" + orderId + "; " +
                                "Date=" + (dr["order_date"] == DBNull.Value ? "" : dr["order_date"].ToString()) + "; " +
                                "CustomerId=" + (dr["customer_id"] == DBNull.Value ? "" : dr["customer_id"].ToString()) + "; " +
                                "Total=" + (dr["total"] == DBNull.Value ? "0" : dr["total"].ToString()) + "; " +
                                "Note=" + (dr["note"] == DBNull.Value ? "" : dr["note"].ToString()) + "; " +
                                "CreatedBy=" + (dr["created_by"] == DBNull.Value ? "" : dr["created_by"].ToString());
                        }
                    }
                }

                var items = new List<string>();
                const string sqlDetails = @"SELECT od.id_product, od.base_qty, od.qty_unit, od.product_name_snapshot, od.price, od.total, p.label
                                           FROM Order_Details od
                                           LEFT JOIN Products p ON p.id = od.id_product
                                           WHERE od.id_order=@id";
                using (var cmd = new SQLiteCommand(sqlDetails, con, tran))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string pId = dr["id_product"] == DBNull.Value ? "" : dr["id_product"].ToString();
                            string qty = dr["base_qty"] == DBNull.Value ? "0" : dr["base_qty"].ToString();
                            string price = dr["price"] == DBNull.Value ? "0" : dr["price"].ToString();
                            string total = dr["total"] == DBNull.Value ? "0" : dr["total"].ToString();
                            string label = dr["product_name_snapshot"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["product_name_snapshot"].ToString())
                                ? dr["product_name_snapshot"].ToString()
                                : (dr["label"] == DBNull.Value ? "" : dr["label"].ToString());
                            items.Add("{" + pId + "," + label + ",qty=" + qty + ",price=" + price + ",total=" + total + "}");
                        }
                    }
                }

                return header + "\nDetails: " + string.Join("; ", items.ToArray());
            }
            catch
            {
                return "OrderId=" + orderId;
            }
        }

        private static bool IsUniqueConstraint(SQLiteException ex)
        {
            if (ex == null) return false;
            try
            {
                if (object.Equals(ex.ErrorCode, SQLiteErrorCode.Constraint) || object.Equals(ex.ErrorCode, (int)SQLiteErrorCode.Constraint))
                    return true;
            }
            catch
            {
            }

            var msg = ex.Message ?? string.Empty;
            return msg.IndexOf("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) >= 0
                   || msg.IndexOf("constraint failed", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static long GetCurrentStockScaled(int productId, SQLiteConnection con, SQLiteTransaction tran)
        {
            const string sql = "SELECT qty_scaled FROM Products WHERE id = @id";
            using (var cmd = new SQLiteCommand(sql, con, tran))
            {
                cmd.Parameters.AddWithValue("@id", productId);
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value) return 0;
                return Convert.ToInt64(result);
            }
        }

        private static List<(int ProductId, decimal QtyBase, string ProductName)> GetOrderDetailsForUpdateValidation(int orderId, SQLiteConnection con, SQLiteTransaction tran)
        {
            var details = new List<(int ProductId, decimal QtyBase, string ProductName)>();

            const string sql = @"
                SELECT od.id_product, od.base_qty, p.label
                FROM Order_Details od
                LEFT JOIN Products p ON p.id = od.id_product
                WHERE od.id_order = @oid";

            using (var cmd = new SQLiteCommand(sql, con, tran))
            {
                cmd.Parameters.AddWithValue("@oid", orderId);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        decimal qtyBaseDec = ReadBaseQtyFromDetailRow(dr);

                        details.Add((
                            ProductId: Convert.ToInt32(dr["id_product"]),
                            QtyBase: qtyBaseDec,
                            ProductName: dr["label"] == DBNull.Value ? string.Empty : dr["label"].ToString()
                        ));
                    }
                }
            }

            return details;
        }

        private static void ValidateStockForUpdate(SQLiteConnection con, SQLiteTransaction tran, List<OrderDetail> newDetails, int orderId)
        {
            if (newDetails == null)
                throw new InvalidOperationException("تفاصيل الفاتورة الجديدة غير موجودة");

            var oldDetails = GetOrderDetailsForUpdateValidation(orderId, con, tran);

            foreach (var newItem in newDetails)
            {
                if (newItem == null) continue;
                if (ProductTypeHelper.IsServiceProduct(con, tran, newItem.ProductId))
                    continue;

                decimal newBaseQtyDec = GetBaseQtyDecimal(newItem);
                if (newBaseQtyDec <= 0m) continue;

                var oldItem = oldDetails.FirstOrDefault(o => o.ProductId == newItem.ProductId);
                decimal oldBaseQtyDec = oldItem == default ? 0m : oldItem.QtyBase;

                var newApplyPreview = StockService.PreviewScaledFromBaseQty(newItem.ProductId, newBaseQtyDec, con, tran);
                var oldApplyPreview = StockService.PreviewScaledFromBaseQty(newItem.ProductId, oldBaseQtyDec, con, tran);

                long differenceScaled = newApplyPreview.AppliedDeltaScaled - oldApplyPreview.AppliedDeltaScaled;

                if (differenceScaled > 0)
                {
                    long currentStock = GetCurrentStockScaled(newItem.ProductId, con, tran);
                    if (currentStock < differenceScaled)
                    {
                        string name = !string.IsNullOrWhiteSpace(newItem.ProductName)
                            ? newItem.ProductName
                            : (oldItem == default ? newItem.ProductId.ToString() : oldItem.ProductName);

                        throw new InvalidOperationException(
                            "الكمية المطلوبة للمنتج '" + name + "' غير متوفرة.\n" +
                            "الكمية الإضافية المطلوبة (scaled): " + differenceScaled + "\n" +
                            "المتاح في المخزون (scaled): " + currentStock);
                    }
                }
            }
        }

        public void SaveSaleOrder(Order order, decimal paidAmount)
        {
            using (TransactionGuard.Enter("Sale"))
            using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (SQLiteTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        if (order != null && order.Id != 0)
                            throw new InvalidOperationException("Order already saved");

                        if (paidAmount < 0)
                            throw new InvalidOperationException("Invalid payment");

                        if (order == null)
                            throw new InvalidOperationException("بيانات الفاتورة غير مكتملة");

                        if (string.IsNullOrWhiteSpace(order.CreatedBy))
                            throw new InvalidOperationException("User required");

                        if (string.IsNullOrWhiteSpace(order.RequestId))
                            throw new InvalidOperationException("RequestId required");

                        if (paidAmount > order.Total)
                            throw new InvalidOperationException("Invalid payment");

                        if (order.Details == null)
                            throw new InvalidOperationException("بيانات الفاتورة غير مكتملة");

                        ApplyPricingToOrder(order);

                        decimal orderDiscount = order.Discount;
                        if (orderDiscount < 0m) orderDiscount = 0m;

                        string sqlOrder = @"
                            INSERT INTO Orders (request_id, order_date, customer_id, total, discount, tax_amount, note, created_by) 
                            VALUES (@rid, @date, @cust, @total, @discount, @tax, @note, @user);
                            SELECT last_insert_rowid();";

                        long orderId;
                        bool alreadyExists;
                        bool insertRetriedAfterCleanup = false;
                        while (true)
                        {
                            try
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(sqlOrder, con, tran))
                                {
                                    cmd.Parameters.AddWithValue("@rid", string.IsNullOrWhiteSpace(order.RequestId) ? (object)DBNull.Value : order.RequestId);
                                    cmd.Parameters.AddWithValue("@date", order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"));
                                    cmd.Parameters.AddWithValue("@cust", order.CustomerId ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@total", order.Total);
                                    cmd.Parameters.AddWithValue("@discount", orderDiscount);
                                    cmd.Parameters.AddWithValue("@tax", order.TaxAmount);
                                    cmd.Parameters.AddWithValue("@note", order.Note ?? "");
                                    cmd.Parameters.AddWithValue("@user", order.CreatedBy);
                                    orderId = (long)cmd.ExecuteScalar();
                                }

                                alreadyExists = false;
                                break;
                            }
                            catch (SQLiteException ex) when (IsUniqueConstraint(ex))
                            {
                                using (var cmd = new SQLiteCommand("SELECT id FROM Orders WHERE request_id=@rid LIMIT 1", con, tran))
                                {
                                    cmd.Parameters.AddWithValue("@rid", order.RequestId);
                                    object v = cmd.ExecuteScalar();
                                    if (v == null || v == DBNull.Value)
                                        throw;
                                    orderId = Convert.ToInt64(v);
                                }

                                using (var cmd = new SQLiteCommand("SELECT COUNT(1) FROM Order_Details WHERE id_order=@oid", con, tran))
                                {
                                    cmd.Parameters.AddWithValue("@oid", orderId);
                                    int cnt = Convert.ToInt32(cmd.ExecuteScalar());
                                    if (cnt <= 0)
                                    {
                                        if (insertRetriedAfterCleanup)
                                            throw new InvalidOperationException("Incomplete order for request_id");

                                        using (var del = new SQLiteCommand("DELETE FROM Orders WHERE id=@oid AND request_id=@rid", con, tran))
                                        {
                                            del.Parameters.AddWithValue("@oid", orderId);
                                            del.Parameters.AddWithValue("@rid", order.RequestId);
                                            del.ExecuteNonQuery();
                                        }

                                        insertRetriedAfterCleanup = true;
                                        continue;
                                    }
                                }

                                alreadyExists = true;
                                break;
                            }
                        }

                        if (order != null)
                            order.Id = (int)orderId;

                        if (alreadyExists)
                        {
                            tran.Commit();
                            return;
                        }

                        InsertOrderAuditLog(con, tran, (int)orderId, "CREATED", order.CreatedBy, "Paid=" + paidAmount);

                        foreach (var item in order.Details)
                        {
                            decimal baseQtyDec = GetBaseQtyDecimal(item);
                            if (baseQtyDec <= 0m)
                                throw new InvalidOperationException("BaseQty غير صحيحة");

                            StockService.StockApplyResult apply = ApplyOrderStockMovement(new StockService.StockMovementFromBaseQtyContext
                            {
                                ProductId = item.ProductId,
                                BaseQty = baseQtyDec,
                                Direction = StockService.MovementDirection.Decrease,
                                OperationType = StockService.StockOperationType.Sale,
                                ReferenceType = "Orders",
                                ReferenceId = orderId,
                                Reason = "بيع فاتورة #" + orderId,
                                UserName = order.CreatedBy
                            }, con, tran);

                            InsertOrderDetailLine(con, tran, orderId, item, apply);
                        }

                        if (paidAmount > 0)
                        {
                            new PaymentRepository().AddPayment((int)orderId, paidAmount, con, tran, order.CreatedBy);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public void UpdateSaleOrder(Order order, decimal paidAmount, string userName)
        {
            using (TransactionGuard.Enter("SaleUpdate"))
            using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (SQLiteTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        if (order == null || order.Details == null)
                            throw new InvalidOperationException("بيانات الفاتورة غير مكتملة");

                        ValidateStockForUpdate(con, tran, order.Details, order.Id);

                        ApplyPricingToOrder(order);

                        decimal orderDiscount = order.Discount;
                        if (orderDiscount < 0m) orderDiscount = 0m;

                        string sqlUpdate = @"
                            UPDATE Orders 
                            SET order_date=@date, customer_id=@cust, total=@total, discount=@discount, tax_amount=@tax, note=@note 
                            WHERE id=@id";

                        using (SQLiteCommand cmd = new SQLiteCommand(sqlUpdate, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@date", order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@cust", order.CustomerId ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@total", order.Total);
                            cmd.Parameters.AddWithValue("@discount", orderDiscount);
                            cmd.Parameters.AddWithValue("@tax", order.TaxAmount);
                            cmd.Parameters.AddWithValue("@note", order.Note ?? "");
                            cmd.Parameters.AddWithValue("@id", order.Id);
                            cmd.ExecuteNonQuery();
                        }

                        const string sqlGetOldDetails = "SELECT id_product, base_qty FROM Order_Details WHERE id_order=@order";
                        var oldDetails = new List<(int ProductId, decimal QtyBase)>();
                        using (SQLiteCommand cmd = new SQLiteCommand(sqlGetOldDetails, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@order", order.Id);
                            using (SQLiteDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    oldDetails.Add((Convert.ToInt32(dr["id_product"]), ReadBaseQtyFromDetailRow(dr)));
                                }
                            }
                        }

                        foreach (var item in oldDetails)
                        {
                            ApplyOrderStockMovement(new StockService.StockMovementFromBaseQtyContext
                            {
                                ProductId = item.ProductId,
                                BaseQty = item.QtyBase,
                                Direction = StockService.MovementDirection.Increase,
                                OperationType = StockService.StockOperationType.Adjustment,
                                ReferenceType = "Orders",
                                ReferenceId = order.Id,
                                Reason = "تعديل فاتورة - استرجاع",
                                UserName = userName
                            }, con, tran);
                        }

                        const string sqlDelete = "DELETE FROM Order_Details WHERE id_order=@order";
                        using (SQLiteCommand cmd = new SQLiteCommand(sqlDelete, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@order", order.Id);
                            cmd.ExecuteNonQuery();
                        }

                        foreach (var item in order.Details)
                        {
                            decimal baseQtyDec = GetBaseQtyDecimal(item);
                            if (baseQtyDec <= 0m)
                                throw new InvalidOperationException("BaseQty غير صحيحة");

                            StockService.StockApplyResult apply = ApplyOrderStockMovement(new StockService.StockMovementFromBaseQtyContext
                            {
                                ProductId = item.ProductId,
                                BaseQty = baseQtyDec,
                                Direction = StockService.MovementDirection.Decrease,
                                OperationType = StockService.StockOperationType.Sale,
                                ReferenceType = "Orders",
                                ReferenceId = order.Id,
                                Reason = "تعديل فاتورة - بيع",
                                UserName = userName
                            }, con, tran);

                            InsertOrderDetailLine(con, tran, order.Id, item, apply);
                        }

                        string snapshotAfter = BuildOrderSnapshotForAudit(con, tran, order.Id);
                        InsertOrderAuditLog(con, tran, order.Id, "EDITED", userName, snapshotAfter);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeleteSaleOrder(int orderId, string userName, string reason)
        {
            using (TransactionGuard.Enter("SaleDelete"))
            using (SQLiteConnection con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (SQLiteTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        string snapshot = BuildOrderSnapshotForAudit(con, tran, orderId);

                        const string sqlGetDetails = "SELECT id_product, base_qty FROM Order_Details WHERE id_order=@id";
                        using (var cmd = new SQLiteCommand(sqlGetDetails, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@id", orderId);
                            using (var dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    int productId = Convert.ToInt32(dr["id_product"]);
                                    decimal qtyBaseDec = ReadBaseQtyFromDetailRow(dr);

                                    ApplyOrderStockMovement(new StockService.StockMovementFromBaseQtyContext
                                    {
                                        ProductId = productId,
                                        BaseQty = qtyBaseDec,
                                        Direction = StockService.MovementDirection.Increase,
                                        OperationType = StockService.StockOperationType.Adjustment,
                                        ReferenceType = "Orders",
                                        ReferenceId = orderId,
                                        Reason = "حذف فاتورة #" + orderId,
                                        UserName = userName
                                    }, con, tran);
                                }
                            }
                        }

                        const string sqlDelete = "DELETE FROM Orders WHERE id=@id";
                        using (var cmd = new SQLiteCommand(sqlDelete, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@id", orderId);
                            int affected = cmd.ExecuteNonQuery();
                            if (affected == 0)
                                throw new InvalidOperationException("الفاتورة غير موجودة");
                        }

                        string details = "Reason=" + (reason ?? string.Empty) + "\n" + snapshot;
                        InsertOrderAuditLog(con, tran, orderId, "DELETED", userName, details);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public int SavePurchase(int? supplierId, DateTime purchaseDate, string note, string createdBy, List<PurchaseDetailInput> details)
        {
            if (details == null || details.Count == 0)
                throw new InvalidOperationException("لا توجد تفاصيل للفاتورة");

            using (TransactionGuard.Enter("Purchase"))
            using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        long purchaseId;

                        const string sqlPurchase = @"
                            INSERT INTO Purchases (supplier_id, purchase_date, note, created_by)
                            VALUES (@supplier_id, @purchase_date, @note, @created_by);
                            SELECT last_insert_rowid();";

                        using (var cmd = new SQLiteCommand(sqlPurchase, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@supplier_id", supplierId.HasValue ? (object)supplierId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@purchase_date", purchaseDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@note", (object)(note ?? string.Empty));
                            cmd.Parameters.AddWithValue("@created_by", (object)(createdBy ?? string.Empty));

                            purchaseId = (long)cmd.ExecuteScalar();
                        }

                        const string sqlDetail = @"
                            INSERT INTO PurchaseDetails (
                                purchase_id,
                                product_id,
                                product_unit_id,
                                unit_name_snapshot,
                                factor_snapshot,
                                qty,
                                base_qty,
                                cost_price,
                                expiry_date,
                                qty_scaled_snapshot,
                                qty_scale_pow10_snapshot
                            )
                            VALUES (
                                @purchase_id,
                                @product_id,
                                @product_unit_id,
                                @unit_name_snapshot,
                                @factor_snapshot,
                                @qty,
                                @base_qty,
                                @cost_price,
                                @expiry_date,
                                @qty_scaled_snapshot,
                                @qty_scale_pow10_snapshot
                            );";

                        foreach (var d in details)
                        {
                            if (d == null) continue;
                            if (d.ProductId <= 0) throw new InvalidOperationException("رقم المنتج غير صحيح");
                            if (d.QtyUnit <= 0) throw new InvalidOperationException("الكمية يجب أن تكون أكبر من صفر");
                            if (d.BaseQty <= 0) throw new InvalidOperationException("BaseQty غير صحيحة");
                            if (d.CostPrice < 0) throw new InvalidOperationException("سعر التكلفة غير صحيح");

                            StockService.StockApplyResult apply = ApplyOrderStockMovement(new StockService.StockMovementFromBaseQtyContext
                            {
                                ProductId = d.ProductId,
                                BaseQty = (decimal)d.BaseQty,
                                Direction = StockService.MovementDirection.Increase,
                                OperationType = StockService.StockOperationType.Purchase,
                                ReferenceType = "Purchases",
                                ReferenceId = purchaseId,
                                Reason = "شراء #" + purchaseId,
                                UserName = createdBy
                            }, con, tran);

                            using (var cmd = new SQLiteCommand(sqlDetail, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@purchase_id", purchaseId);
                                cmd.Parameters.AddWithValue("@product_id", d.ProductId);
                                cmd.Parameters.AddWithValue("@product_unit_id", d.ProductUnitId > 0 ? (object)d.ProductUnitId : DBNull.Value);
                                cmd.Parameters.AddWithValue("@unit_name_snapshot", (object)(d.UnitNameSnapshot ?? string.Empty));
                                cmd.Parameters.AddWithValue("@factor_snapshot", (object)d.FactorSnapshot);
                                cmd.Parameters.AddWithValue("@qty", (object)d.QtyUnit);
                                cmd.Parameters.AddWithValue("@base_qty", (object)d.BaseQty);
                                cmd.Parameters.AddWithValue("@cost_price", (object)d.CostPrice);
                                cmd.Parameters.AddWithValue("@expiry_date", string.IsNullOrWhiteSpace(d.ExpiryDate) ? (object)DBNull.Value : d.ExpiryDate);
                                cmd.Parameters.AddWithValue("@qty_scaled_snapshot", (object)apply.AppliedDeltaScaledAbs);
                                cmd.Parameters.AddWithValue("@qty_scale_pow10_snapshot", (object)apply.ScalePow10);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        return (int)purchaseId;
                    }
                    catch (Exception ex)
                    {
                        try { tran.Rollback(); }
                        catch (Exception rbEx)
                        {
                            Logger.LogError("SalesTransactionService.SavePurchase: Rollback failed", rbEx);
                        }
                        Logger.LogError("Error in SavePurchase", ex);
                        throw;
                    }
                }
            }
        }
    }
}
