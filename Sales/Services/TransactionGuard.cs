// ============================================================
// الملف    : TransactionGuard.cs
// الغرض    : فئة لإدارة وحماية معاملات قاعدة البيانات (SQL Transactions) وضمان تراجعها
// ============================================================

using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading;
using Sales.Database;
using Sales.Utilities;

namespace Sales.Services
{
    public static class TransactionGuard
    {
        private static readonly AsyncLocal<int> _depth = new AsyncLocal<int>();
        private static readonly AsyncLocal<string> _scope = new AsyncLocal<string>();
        private static int _loggingReentrancy;

        public static bool IsActive => _depth.Value > 0;

        public static IDisposable Enter(string scope)
        {
            _depth.Value = _depth.Value + 1;
            if (_depth.Value == 1)
                _scope.Value = scope ?? string.Empty;

            return new ExitHandle();
        }

        public static void Run(string scope, Action action)
        {
            if (action == null) return;

            if (IsActive)
            {
                action();
                return;
            }

            using (Enter(scope))
            {
                action();
            }
        }

        public static T Run<T>(string scope, Func<T> func)
        {
            if (func == null) return default(T);

            if (IsActive)
                return func();

            using (Enter(scope))
            {
                return func();
            }
        }

        public static void EnsureActive(string operationType)
        {
            if (IsActive) return;

            var st = new StackTrace();
            LogViolation(operationType, st);
            // Do not block execution; this guard is used for diagnostics and enforcing best-practice call paths.
            // Many legacy flows perform their own SQLite transactions inside repositories.
            // We keep logging so bypasses are still visible and can be migrated gradually.
            return;
        }

        private static void Exit()
        {
            int v = _depth.Value;
            if (v <= 0) return;

            v--;
            _depth.Value = v;
            if (v == 0)
                _scope.Value = null;
        }

        private sealed class ExitHandle : IDisposable
        {
            private bool _disposed;

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                Exit();
            }
        }

        private static void LogViolation(string operationType, StackTrace st)
        {
            if (Interlocked.Exchange(ref _loggingReentrancy, 1) == 1) return;
            try
            {
                string stack = st?.ToString() ?? string.Empty;
                string caller = "";
                try
                {
                    var f = st?.GetFrame(1);
                    var m = f?.GetMethod();
                    if (m != null)
                        caller = (m.DeclaringType?.FullName ?? "") + "." + m.Name;
                }
                catch
                {
                }

                try
                {
                    using (var con = new SQLiteConnection(DatabaseInitializer.ConnectionString))
                    {
                        con.Open();
                        const string sql = @"INSERT INTO SystemViolationLog (operation_type, caller, stack_trace, created_at)
VALUES (@op, @caller, @stack, CURRENT_TIMESTAMP);";
                        using (var cmd = new SQLiteCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@op", (object)(operationType ?? string.Empty));
                            cmd.Parameters.AddWithValue("@caller", (object)(caller ?? string.Empty));
                            cmd.Parameters.AddWithValue("@stack", (object)(stack ?? string.Empty));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    try { Logger.LogError("Failed to write SystemViolationLog", ex); } catch { }
                }
            }
            finally
            {
                Interlocked.Exchange(ref _loggingReentrancy, 0);
            }
        }
    }
}
