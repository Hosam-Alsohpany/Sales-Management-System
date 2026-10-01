// ============================================================
// الملف    : Logger.cs
// الغرض    : مسجل بسيط لكتابة رسائل الأخطاء/المعلومات إلى ملف نصي داخل AppData (للدعم الفني والتتبع)
// يتعامل مع: أي جزء من المشروع يستدعي Logger.LogError/LogInfo (مثل Repositories عند التقاط الاستثناءات)
// المسار   : %AppData%\SalesApp\Logs\log_yyyy-MM-dd.txt
// ============================================================

using System;
using System.IO;
using Sales.Repositories;

namespace Sales.Utilities
{
    public static class Logger
    {
        private static readonly ErrorLogRepository _errorDb = new ErrorLogRepository();
        /// <summary>
        /// LogFilePath: بناء مسار ملف السجل لليوم الحالي، مع ضمان وجود مجلد Logs
        /// المدخلات : لا يوجد
        /// المخرجات : string مسار الملف (يتغير يومياً)
        /// التدفق   : LogError/LogInfo → LogFilePath → Environment(AppData) → Directory/CreateDirectory
        /// الأخطاء  : قد تفشل عملية الإنشاء/الكتابة بسبب صلاحيات النظام (يتم تجاهلها داخل try/catch)
        /// </summary>
        private static string LogFilePath
        {
            get
            {
                // ── الحصول على AppData الخاص بالمستخدم الحالي ──
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                // ── تجهيز مجلد ثابت للسجلات داخل مجلد التطبيق ──
                string logFolder = Path.Combine(appData, "SalesApp", "Logs");
                // ── إنشاء المجلد إذا لم يكن موجوداً ──
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);
                // ── ملف يومي: لكل يوم ملف مستقل لتسهيل تتبع المشاكل حسب التاريخ ──
                return Path.Combine(logFolder, string.Format("log_{0}.txt", DateTime.Now.ToString("yyyy-MM-dd")));
            }
        }

        /// <summary>
        /// LogError: كتابة رسالة خطأ إلى ملف السجل مع توقيت، وإرفاق الاستثناء إن وُجد
        /// المدخلات : message رسالة الخطأ، ex استثناء اختياري
        /// المخرجات : لا يوجد
        /// التدفق   : أي كود catch → Logger.LogError → File.AppendAllText
        /// ملاحظة   : أي خطأ أثناء التسجيل يتم تجاهله حتى لا يتسبب بتعطيل البرنامج
        /// </summary>
        public static void LogError(string message, Exception ex = null)
        {
            try
            {
                // ── تنسيق سطر السجل مع التاريخ/الوقت ──
                string logMessage = string.Format("[{0}] ERROR: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);
                if (ex != null)
                    // ── إضافة تفاصيل الاستثناء (StackTrace + Message) ──
                    logMessage += "\r\n" + ex + "\r\n";

                // ── AppendAllText: إضافة للسجل بدون حذف القديم ──
                File.AppendAllText(LogFilePath, logMessage + "\r\n---\r\n");
                try { _errorDb.Insert("ERROR", null, message, ex?.ToString()); } catch { }
            }
            catch
            {
                // نتجاهل أي خطأ أثناء التسجيل حتى لا ينكسر التطبيق بسبب مشكلة في الكتابة
            }
        }

        public static void LogError(Exception ex)
        {
            LogError("Unhandled error", ex);
        }

        /// <summary>
        /// LogInfo: كتابة رسالة معلوماتية (INFO) إلى ملف السجل
        /// المدخلات : message
        /// المخرجات : لا يوجد
        /// التدفق   : أي مكان بالمشروع → Logger.LogInfo → File.AppendAllText
        /// </summary>
        public static void LogInfo(string message)
        {
            try
            {
                string logMessage = string.Format("[{0}] INFO: {1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);
                File.AppendAllText(LogFilePath, logMessage);
            }
            catch
            {
            }
        }

        public static void LogWarning(string message, Exception ex = null)
        {
            try
            {
                string logMessage = string.Format("[{0}] WARN: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);
                if (ex != null)
                    logMessage += "\r\n" + ex + "\r\n";

                File.AppendAllText(LogFilePath, logMessage + "\r\n---\r\n");
            }
            catch
            {
            }
        }

        public static void LogInfo(string formName, string actionName, string message)
        {
            LogInfo(string.Format("[{0}] [{1}] {2}", formName ?? string.Empty, actionName ?? string.Empty, message ?? string.Empty));
        }

        public static void LogWarning(string formName, string actionName, string message, Exception ex = null)
        {
            LogWarning(string.Format("[{0}] [{1}] {2}", formName ?? string.Empty, actionName ?? string.Empty, message ?? string.Empty), ex);
        }

        public static void LogError(string formName, string actionName, Exception ex)
        {
            LogError(string.Format("[{0}] [{1}] Error", formName ?? string.Empty, actionName ?? string.Empty), ex);
        }

        // ╔══════════════════════════════════════════════╗
        // ║  ملخص الملف                                  ║
        // ║  عدد الدوال    : 2 (+ خاصية مسار)             ║
        // ║  ناتج العمل    : ملفات نصية يومية داخل Logs   ║
        // ║  يستخدمه       : Repositories/Forms عند تسجيل الأخطاء ║
        // ╚══════════════════════════════════════════════╝
    }
}
