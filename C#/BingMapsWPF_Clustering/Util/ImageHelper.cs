using System;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DImage = System.Drawing.Image;

namespace PhotoVis.Util
{
    class ImageHelper
    {
        public static string SnapshotUserControl(UserControl control)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)control.ActualHeight, (int)control.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(control);

            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (MemoryStream m = new MemoryStream())
            {
                pngImage.Save(m);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static string ImageToBase64(string sourceFile)
        {
            using (DImage image = DImage.FromFile(sourceFile))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        public static string ResizeBase64ImageString(string Base64String, int desiredWidth, int desiredHeight)
        {
            Base64String = Base64String.Replace("data:image/png;base64,", "");

            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(Base64String);

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                DImage image = DImage.FromStream(ms, true);

                var imag = ScaleImage(image, desiredWidth, desiredHeight);

                using (MemoryStream ms1 = new MemoryStream())
                {
                    //First Convert Image to byte[]
                    imag.Save(ms1, imag.RawFormat);
                    byte[] imageBytes1 = ms1.ToArray();

                    //Then Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes1);
                    return "data:image/png;base64," + base64String;
                }
            }
        }

        public static DImage ScaleImage(DImage image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
    }
}
