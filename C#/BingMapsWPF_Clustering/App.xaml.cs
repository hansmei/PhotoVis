using System;
using System.Configuration;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Windows;

using PhotoVis.Util;
using PhotoVis.ViewModel;

namespace PhotoVis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public static string BingMapKey = ConfigurationManager.AppSettings.Get("BingMapsKey");
        //public static string CommonFilesRootPath = ConfigurationManager.AppSettings.Get("CommonFilesRootPath");
        public static readonly string AppDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string PhotoVisDataRoot = Path.Combine(AppDataRoot, "PhotoVis");

        public static DatabaseConnection DB;
        public static CultureInfo RegionalCulture;
        public static ApplicationViewModel VM;

        public App()
        {
            // Set invariant culture on main thread
            RegionalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // Make sure some special folders exist
            if (!Directory.Exists(PhotoVisDataRoot))
                Directory.CreateDirectory(PhotoVisDataRoot);

            // Create the database if it does not exist
            DatabaseInitializer init = new DatabaseInitializer();
            init.CreateDatabase();

            // Try to establish DB connection
            try
            {
                DB = new DatabaseConnection();
            }
            catch
            {
                System.Windows.MessageBox.Show("Fatal error, could not connect to database.");
                System.Windows.Application.Current.Shutdown();
            }

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            StartWindow app = new StartWindow();
            VM = new ApplicationViewModel();
            app.DataContext = VM;
            app.Show();
        }
    }
}
