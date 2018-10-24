using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Maps.MapControl.WPF;
using System.IO;
using DImage = System.Drawing.Image;

using PhotoVis.Data.DatabaseTables;

namespace PhotoVis.Util
{
    class ImageHelper
    {

        public static string GetProjectThumbnailPath(int projectId)
        {
            string imagePath = Path.Combine(App.ProjctsDataRoot, "Thumb_" + projectId + ".png");
            return imagePath;
        }

        public static string SnapshotMap(int projectId, Map control)
        {
            int size;
            if (control.ActualHeight > control.ActualWidth)
                size = (int)control.ActualWidth;
            else
                size = (int)control.ActualHeight;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(control);

            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (MemoryStream m = new MemoryStream())
            {
                pngImage.Save(m);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                string resized = ResizeBase64ImageString(base64String, 200, 200);

                DImage img = Base64ToImage(resized);
                string path = GetProjectThumbnailPath(projectId);

                // Make sure that file can be written and is not open
                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    if (!IsFileLocked(fileInfo))
                    {
                        img.Save(path);
                    }
                }
                else
                {
                    img.Save(path);
                }
                img.Dispose();

                return base64String;
            }
        }

        protected static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        //public static string ImageToBase64(string sourceFile)
        //{
        //    using (DImage image = DImage.FromFile(sourceFile))
        //    {
        //        using (MemoryStream m = new MemoryStream())
        //        {
        //            image.Save(m, image.RawFormat);
        //            byte[] imageBytes = m.ToArray();

        //            // Convert byte[] to Base64 String
        //            string base64String = Convert.ToBase64String(imageBytes);
        //            return base64String;
        //        }
        //    }
        //}

        public static DImage Base64ToImage(string base64string)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64string);

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                DImage image = DImage.FromStream(ms, true);
                return image;
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
                    imag.Save(ms1, image.RawFormat);
                    byte[] imageBytes1 = ms1.ToArray();

                    //Then Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes1);
                    //return "data:image/png;base64," + base64String;
                    return base64String;
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
