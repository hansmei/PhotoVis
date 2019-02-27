using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using BingMapsCredentials;
using PhotoVis.Util;
using PhotoVis.Views;

namespace PhotoVis
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void Licenses_Click(object sender, RoutedEventArgs e)
        {
            LicenseView licenseView = new LicenseView();
            licenseView.ShowDialog();
        }

        private void BingMap_Click(object sender, RoutedEventArgs e)
        {
            string pathToBingMaps = BingMapsCredentialsProvider.GetBingMapsCredentialsFullPath();
            CreateKeyWindow createKeyWindow = new CreateKeyWindow(pathToBingMaps);
            createKeyWindow.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UserManual_Click(object sender, RoutedEventArgs e)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string rootFolder = Path.GetDirectoryName(path);

            string pathToPdf = Path.Combine(rootFolder, "Manual.pdf");
            string usePath;
            if (File.Exists(pathToPdf))
            {
                usePath = pathToPdf;
            }
            else
            {
                usePath = Path.Combine(rootFolder, "Resources", "Manual.pdf");
            }

            Process.Start(usePath);
        }

        private void PoweredByButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"http://vavisjon.no");
        }
    }
}
