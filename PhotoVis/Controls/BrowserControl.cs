using System;
using System.Windows.Controls;
using System.Text;
using System.IO;
using System.Windows;

namespace PhotoVis.Controls
{
    public class BrowserControl : Control
    {
        private static BrowserControl currentBrowser;

        public static readonly DependencyProperty URLproperty
            = DependencyProperty.Register(
                "URL",
                typeof(string),
                typeof(BrowserControl),
                new PropertyMetadata(string.Empty, OnURLPropertyChanged),
                OnValidateURLCallBack);

        private static bool OnValidateURLCallBack(object value)
        {
            Uri uri = null;
            var url = Convert.ToString(value);
            if (!string.IsNullOrEmpty(url))
            {
                return Uri.TryCreate(Convert.ToString(value), UriKind.Absolute, out uri);
            }
            return true;
        }

        private static void OnURLPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var browserControl = sender as BrowserControl;
            currentBrowser = browserControl;
            if (browserControl != null)
            {
                Uri uri = null;
                var url = Convert.ToString(args.NewValue);
                browserControl.ApplyTemplate();
                var template = browserControl.Template;
                if (template != null)
                {
                    var internalBrowser =
                        browserControl.Template.FindName("_InternalBrowser", browserControl) as WebBrowser;
                    if (internalBrowser != null)
                    {
                        if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out uri))
                        {
                            internalBrowser.Navigate(uri);
                        }
                        else if (string.IsNullOrEmpty(url))
                        {
                            internalBrowser.NavigateToStream(new MemoryStream(Encoding.ASCII.GetBytes(string.Empty)));
                        }
                    }
                }
            }
        }

        public string URL
        {
            get { return Convert.ToString(GetValue(URLproperty)); }
            set { SetValue(URLproperty, value); }
        }

        public string ModifiedURL
        {
            get
            {
                var template = currentBrowser.Template;
                if (template != null)
                {
                    var internalBrowser =
                        currentBrowser.Template.FindName("_InternalBrowser", currentBrowser) as WebBrowser;
                    if (internalBrowser != null)
                    {
                        Uri uri = internalBrowser.Source;
                        return uri.ToString();
                    }
                }
                return URL;
            }
        }
    }
}
