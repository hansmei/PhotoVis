using System;
using System.Windows.Controls;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace PhotoVis.Controls
{
    public class ImageButton
    {
        public static ImageSource GetImage(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(ImageProperty);
        }

        public static void SetImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.RegisterAttached("Image", typeof(ImageSource), typeof(ImageButton), new UIPropertyMetadata((ImageSource)null));

        public static ImageSource GetStretch(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(StretchProperty);
        }

        public static void SetStretch(DependencyObject obj, string value)
        {
            obj.SetValue(StretchProperty, value);
        }

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.RegisterAttached("Stretch", typeof(string), typeof(ImageButton), new UIPropertyMetadata((string)null));
    }
}
