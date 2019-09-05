using System;
using System.Configuration;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Windows;

using PhotoVis.Util;
using PhotoVis.ViewModel;
using SpikeAccountManager;

namespace PhotoVis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string BingMapKey = ConfigurationManager.AppSettings.Get("BingMapsKey");
        //public static string CommonFilesRootPath = ConfigurationManager.AppSettings.Get("CommonFilesRootPath");
        public static readonly string AppDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string PhotoVisDataRoot = Path.Combine(AppDataRoot, "PhotoVis");
        public static readonly string ProjctsDataRoot = Path.Combine(PhotoVisDataRoot, "Projects");

        public static DatabaseConnection DB;
        public static CultureInfo RegionalCulture;
        public static ApplicationViewModel VM;
        public static MapViewModel MapVM;

        public App()
        {
            // Set invariant culture on main thread
            RegionalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = RegionalCulture;

            // Make sure some special folders exist
            if (!Directory.Exists(PhotoVisDataRoot))
                Directory.CreateDirectory(PhotoVisDataRoot);
            if (!Directory.Exists(ProjctsDataRoot))
                Directory.CreateDirectory(ProjctsDataRoot);

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
                MessageBox.Show("Fatal error, could not connect to database.");
                Application.Current.Shutdown();
            }

        }
        
        private void LoginStart(object sender, StartupEventArgs e)
        {
            //Disable shutdown when the dialog closes
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Fetch the user account
            SpikeAccountManager.AccountWindow window = new SpikeAccountManager.AccountWindow();
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                this.ApplicationStart(sender, e);
            }
            else if(result.HasValue && !result.Value)
            {
                Current.Shutdown(-1);
            }
            else
            {
                MessageBox.Show("Unable to establish a valid login.", "Error", MessageBoxButton.OK);
                Current.Shutdown(-1);
            }
        }

        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            StartWindow app = new StartWindow();
            //Re-enable normal shutdown mode.
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow = app;
            VM = new ApplicationViewModel();
            app.DataContext = VM;
            app.Show();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            // Fetch the user account
            SpikeAccountManager.AccountWindow window = new SpikeAccountManager.AccountWindow();

            try
            {
                bool? result = window.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    // Happy result, do not terminate the application
                    VM.User = window.LoginResponse.Resource;
                }
                else
                {
                    Application.Current.Shutdown(-1);
                }
            }
            catch(SpikeException err)
            {
                MessageBox.Show(err.Message + "\r\nAborting application");
                Application.Current.Shutdown(-1);
            }
            catch (AggregateException ae)
            {
                MessageBox.Show("Fatal error. Aborting application");
                Application.Current.Shutdown(-1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nAborting application");
                Application.Current.Shutdown(-1);
            }

        }
    }
}
