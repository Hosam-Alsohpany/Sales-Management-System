using System;
using System.Windows.Forms;
using Sales.Database;
using Sales.Services.ProductUnits;
using Sales.Utilities;

namespace Sales
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
#if DEBUG
            try
            {
                string run = Environment.GetEnvironmentVariable("SALES_RUN_SELFTESTS");
                if (!string.IsNullOrWhiteSpace(run) && run.Trim() == "1")
                {
                    ProductUnitHierarchySelfTest.RunAllOrThrow();
                    return;
                }
            }
            catch
            {
                throw;
            }
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            SalesUiBootstrap.Initialize();

            // تهيئة قاعدة بيانات SQLite
            DatabaseInitializer.Initialize();

            if (!DatabaseInitializer.IsInitialized())
            {
                using (var setup = new FirstTimeSetup())
                {
                    SalesUiBootstrap.WireForm(setup);
                    if (setup.ShowDialog() != DialogResult.OK)
                        return;
                }
            }

            using (Login login = new Login())
            {
                SalesUiBootstrap.WireForm(login);
                if (login.ShowDialog() == DialogResult.OK)
                {
                    var mainForm = new main(login.LoggedUserName, login.FullName, login.Role);
                    SalesUiBootstrap.WireForm(mainForm);
                    Application.Run(mainForm);
                }
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try { Logger.LogError("UI thread exception", e.Exception); } catch { }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is Exception ex)
                    Logger.LogError("Unhandled domain exception", ex);
            }
            catch { }
        }
    }
}
