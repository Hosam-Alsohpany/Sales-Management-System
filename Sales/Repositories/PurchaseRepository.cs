// ============================================================
// الملف    : PurchaseRepository.cs
// الغرض    : مستودع تسجيل وعرض وإدارة فواتير المشتريات من الموردين
// ============================================================

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Sales.Database;
using Sales.Models;
using Sales.Services;
using Sales.Utilities;

namespace Sales.Repositories
{
    public class PurchaseRepository
    {
        private readonly SalesTransactionService _tx = new SalesTransactionService();

        public int SavePurchase(int? supplierId, DateTime purchaseDate, string note, string createdBy, List<PurchaseDetailInput> details)
        {
            return _tx.SavePurchase(supplierId, purchaseDate, note, createdBy, details);
        }
    }
}
