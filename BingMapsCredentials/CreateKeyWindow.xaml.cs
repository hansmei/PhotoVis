using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BingMapsCredentials
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CreateKeyWindow : Window
    {

        private string _key = "";
        public string Key
        {
            get
            {
                return _key;
            }
        }

        private string credentialSavePath;

        public CreateKeyWindow(string credentialSavePath)
        {
            InitializeComponent();

            this.credentialSavePath = credentialSavePath;
            if (File.Exists(this.credentialSavePath))
            {
                this._key = File.ReadAllText(this.credentialSavePath);
                this.BingMapsKey.Text = this.Key;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string useKey = this.BingMapsKey.Text;
            File.WriteAllText(this.credentialSavePath, useKey);
            this._key = useKey;

            MessageBox.Show("Bing maps key updated successfully!\nIn order for the map to update, restart the application.");
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
