using System.Configuration;
using System.Threading;
using System.Globalization;
using System.Windows;

using PhotoVis.Util;

namespace PhotoVis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public static string BingMapKey = ConfigurationManager.AppSettings.Get("BingMapsKey");
        public static string CommonFilesRootPath = ConfigurationManager.AppSettings.Get("CommonFilesRootPath");

        public static DatabaseConnection DB;
        public static CultureInfo RegionalCulture;

        public App()
        {
            // Set invariant culture on main thread
            RegionalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

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
    }
}
