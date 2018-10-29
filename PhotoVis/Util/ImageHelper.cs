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
        //public static string GetProjectThumbnailsFolder(int projectId)
        //{
        //    string thumbFolder = Path.Combine(App.ProjctsDataRoot, projectId.ToString());
        //    return thumbFolder;
        //}

        //public static string GetProjectThumbnailPath(int projectId)
        //{
        //    string imagePath = Path.Combine(App.ProjctsDataRoot, "Thumb_" + projectId + ".png");
        //    return imagePath;
        //}

        public static string GetProjectThumbnailBase64Path(int projectId)
        {
            string imagePath = Path.Combine(App.ProjctsDataRoot, "Thumb_" + projectId + ".txt");
            return imagePath;
        }

        public static DImage PathToThumbnail(string path, int width, int height)
        {
            DImage img = DImage.FromFile(path);
            var newImage = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(img, 0, 0, width, height);

            return newImage;
        }

        public static string SnapshotMap(int projectId, Map control)
        {
            //int size;
            //if (control.ActualHeight > control.ActualWidth)
            //    size = (int)control.ActualWidth;
            //else
            //    size = (int)control.ActualHeight;
            System.Windows.Point relativePoint = control.TransformToAncestor(Application.Current.MainWindow)
                          .Transform(new System.Windows.Point(0, 0));

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)control.ActualWidth, (int)control.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(control);

            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (MemoryStream m = new MemoryStream())
            {
                pngImage.Save(m);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);

                int height = (int)control.ActualHeight;
                int offset = (int)relativePoint.X;
                int imageCenter = (((int)control.ActualWidth - offset) / 2) + offset;

                Rectangle r = new Rectangle(imageCenter - (height/2), 0, height, height);

                DImage image = Base64ToImage(base64String);
                DImage resized = CropImage(image, r);

                string base64resize = ImageToBase64(resized);

                // Write to database
                Dictionary<string, object> where = new Dictionary<string, object>();
                where.Add(DAssignment.ProjectId, projectId);

                Dictionary<string, object> row = new Dictionary<string, object>();
                row.Add(DAssignment.Thumbnail, base64resize);
                int numAffected = App.DB.UpdateValue(DTables.Assignments, where, row);

                string base64path = GetProjectThumbnailBase64Path(projectId);
                WriteBase64ToFile(base64path, base64resize);

                ////DImage img = Base64ToImage(resized);
                //string path = GetProjectThumbnailPath(projectId);

                //// Make sure that file can be written and is not open
                //if (File.Exists(path))
                //{
                //    if (!FileHelper.IsFileLocked(path))
                //    {
                //        resized.Save(path);
                //    }
                //}
                //else
                //{
                //    resized.Save(path);
                //}
                //resized.Dispose();

                return base64String;
            }
        }

        public static void WriteBase64ToFile(string path, string base64string)
        {
            // Make sure that file can be written and is not open
            if (File.Exists(path) && !FileHelper.IsFileLocked(path))
            {
                File.WriteAllText(path, base64string);
            }
            else
            {
                File.WriteAllText(path, base64string);
            }
        }

        public static byte[] BytesFromImage(Image image)
        {
            byte[] imageBytes;
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, ImageFormat.Png);
                imageBytes = m.ToArray();
            }
            return imageBytes;
        }

        public static BitmapImage BitmapImageFromBuffer(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        public static string ImageToBase64(DImage image)
        {
            byte[] imageBytes = BytesFromImage(image);
            // Convert byte[] to Base64 String
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
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

        public static DImage CropImage(DImage img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

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
