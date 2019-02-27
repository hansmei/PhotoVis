using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using IO = System.IO;
using Microsoft.Maps.MapControl.WPF;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

using PhotoVis.Data;
using PhotoVis.Data.DatabaseTables;
using PhotoVis.Helpers;

namespace PhotoVis.Util
{
    class AssignmentIndexer
    {
        public static Tuple<int, int, int> LoadImagesFromDatabase(int projectId)
        {
            // First get an array of all existing paths for this project
            Dictionary<string, object> whereArray = new Dictionary<string, object>();
            whereArray.Add(DImageAtLocation.ProjectId, projectId);

            DataTable data = App.DB.GetValues(DTables.Images, whereArray, new string[] { "*" });
            HashSet<string> existingPaths = new HashSet<string>();

            App.MapVM.ImageLocations.Clear();
            foreach (DataRow row in data.Rows)
            {
                ImageAtLocation img = new ImageAtLocation(row);
                //if (!IO.File.Exists(img.ImagePath))
                //{
                //    // Delete the thumbnail
                //    string projectThumbFolder = ImageHelper.GetProjectThumbnailsFolder(projectId);
                //    string thumbPath = IO.Path.Combine(projectThumbFolder, img.ThumbnailGuid + ".png");
                //    if(IO.File.Exists(thumbPath))
                //    {
                //        IO.File.Delete(thumbPath);
                //    }

                //    // Delete this entry from the database
                //    Dictionary<string, object> where = new Dictionary<string, object>();
                //    where.Add(DImageAtLocation.Id, img.ID);

                //    int affected = App.DB.DeleteValue(DTables.Images, where);

                //    // Skip files that no longer exists
                //    continue;
                //}

                if (img.HasLocation)
                {
                    App.MapVM.ImageLocations.Add(img);
                }
                else
                {
                    App.MapVM.UnassignedImageLocations.Add(img);
                }
            }

            return new Tuple<int, int, int>(
                App.MapVM.ImageLocations.Count,
                App.MapVM.UnassignedImageLocations.Count,
                App.MapVM.ImageLocations.ErasedImages.Count + App.MapVM.UnassignedImageLocations.ErasedImages.Count
                );
        }

        public static HashSet<string> GetUniquePathNames()
        {
            HashSet<string> uniquePathNames = new HashSet<string>();
            int numImages = App.MapVM.ImageLocations.Count;
            for(int i = 0; i < numImages; i++)
            {
                ImageAtLocation img = App.MapVM.ImageLocations[i];
                uniquePathNames.Add(img.ImagePath);
            }
            int numUnassignedImages = App.MapVM.UnassignedImageLocations.Count;
            for (int i = 0; i < numUnassignedImages; i++)
            {
                ImageAtLocation img = App.MapVM.UnassignedImageLocations[i];
                uniquePathNames.Add(img.ImagePath);
            }
            return uniquePathNames;
        }

        public static List<ImageAtLocation> ExtractValidLocations(List<ImageAtLocation> extractFrom, out List<ImageAtLocation> unknownLocations)
        {
            List<ImageAtLocation> valid = new List<ImageAtLocation>();
            unknownLocations = new List<ImageAtLocation>();

            foreach (ImageAtLocation img in extractFrom)
            {
                if (img.HasLocation)
                {
                    valid.Add(img);
                }
                else
                {
                    unknownLocations.Add(img);
                }
            }
            return valid;
        }

        //public static async Task<ImageIndexerTransporter> IndexImagesAsync(ImageIndexerTransporter singleFolderToIndex)
        //{
        //    //create a task completion source
        //    //the type of the result value must be the same
        //    //as the type in the returning Task
        //    TaskCompletionSource<ImageIndexerTransporter> tcs = new TaskCompletionSource<ImageIndexerTransporter>();

        //    await Task.Run(() =>
        //    {
        //        List<ImageAtLocation> images = AssignmentIndexer.IndexImages(singleFolderToIndex);
        //        singleFolderToIndex.SetImages(images);
        //        tcs.SetResult(singleFolderToIndex);
        //    });

        //    return tcs.Task;
        //    //.ContinueWith(t =>
        //    //{

        //    //}
        //    //);
        //}

        //public static List<ImageAtLocation> IndexImagesAsync(ImageIndexerTransporter folderToIndex, IO.SearchOption searchMode)
        //{
        //    // First make sure a folder for thumbnails images exist for this project
        //    string projectThumbFolder = ImageHelper.GetProjectThumbnailsFolder(folderToIndex.ProjectId);
        //    if (!IO.Directory.Exists(projectThumbFolder))
        //        IO.Directory.CreateDirectory(projectThumbFolder);

        //    HashSet<string> unique = GetUniquePathNames();

        //    List<ImageAtLocation> images = IndexImagesInLocation(folderToIndex, searchMode);
        //    List<ImageAtLocation> newImages = new List<ImageAtLocation>();
        //    foreach (ImageAtLocation img in images)
        //    {
        //        if (!unique.Contains(img.ImagePath))
        //        {
        //            img.SaveToDatabase();
        //            newImages.Add(img);
        //        }
        //    }
        //    return newImages;
        //}

        public static List<ImageAtLocation> IndexImages(ImageIndexerTransporter folderToIndex, IO.SearchOption searchMode)
        {
            // First make sure a folder for thumbnails images exist for this project
            string projectThumbFolder = ImageHelper.GetProjectThumbnailsFolder(folderToIndex.ProjectId);
            if (!IO.Directory.Exists(projectThumbFolder))
                IO.Directory.CreateDirectory(projectThumbFolder);

            HashSet<string> unique = GetUniquePathNames();

            List<ImageAtLocation> images = IndexImagesInLocation(folderToIndex, searchMode);
            List<ImageAtLocation> newImages = new List<ImageAtLocation>();
            foreach (ImageAtLocation img in images)
            {
                if (!unique.Contains(img.ImagePath))
                {
                    img.SaveToDatabase();
                    newImages.Add(img);
                }
            }
            return newImages;
        }
        
        private static List<ImageAtLocation> IndexImagesInLocation(ImageIndexerTransporter singleFolderToIndex, IO.SearchOption searchMode)
        {
            List<ImageAtLocation> list = new List<ImageAtLocation>();

            // Get all image files in directory and subdirectory
            if (IO.Directory.Exists(singleFolderToIndex.FolderPath))
            {
                string[] images = IO.Directory.GetFiles(singleFolderToIndex.FolderPath, "*.jpg", searchMode);
                
                foreach (string path in images)
                {
                    ImageAtLocation img = ProcessImage(path, singleFolderToIndex.ProjectId);
                    if (img != null)
                        list.Add(img);
                }
            }
            return list;
        }

        private static ImageAtLocation ProcessImage(string path, int projectId)
        {
            IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(path);
            var gps = ImageMetadataReader.ReadMetadata(path)
                            .OfType<GpsDirectory>()
                            .FirstOrDefault();

            // Define some variables
            Location useLocation = null;
            float degrees = 0;
            ImageAtLocation.LocationSourceType locationType = ImageAtLocation.LocationSourceType.Unknown;

            // Try to fetch the location
            if (gps != null)
            {
                GeoLocation location = gps.GetGeoLocation();
            
                // Test if the location is valid
                if (location == null || location.IsZero)
                {
                    useLocation = FileHelper.ReadCoordinateFile(path);
                    locationType = ImageAtLocation.LocationSourceType.Manual;
                }
                else
                {
                    useLocation = new Location(location.Latitude, location.Longitude);
                    bool hasDegrees = gps.TryGetSingle(GpsDirectory.TagImgDirection, out degrees);
                    locationType = ImageAtLocation.LocationSourceType.GPS;
                }
            }
            else
            {
                useLocation = FileHelper.ReadCoordinateFile(path);
            }
            
            // obtain a specific directory
            var ifd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
            var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

            // get tag description
            DateTime imageTakenTime;
            bool gotDateTime = false;
            if (ifd0Directory != null)
            {
                gotDateTime = ifd0Directory.TryGetDateTime(GpsDirectory.TagDateTime, out imageTakenTime);
            }
            else
            {
                imageTakenTime = IO.File.GetCreationTime(path);
            }

            if (useLocation != null)
            { 
                //if (subIfdDirectory == null)
                //    return null;

                // create a descriptor
                //var descriptor = new ExifSubIfdDescriptor(subIfdDirectory);
                //string res = descriptor.GetOrientationDescription();
                
                return new ImageAtLocation(
                        projectId,
                        useLocation,
                        path,
                        null,
                        locationType,
                        GetFileCreator(path),
                        imageTakenTime,
                        degrees
                        );

            }
            else
            {
                // Create a thumbnail 
                string thumbBase64 = ExtractThumbnailBase64(path);
                string resized = null;
                string thumbId = "";
                if (thumbBase64 != null)
                {
                    resized = ImageHelper.ResizeBase64ImageString(thumbBase64, 200, 200);
                    Guid guid = Guid.NewGuid();
                    thumbId = guid.ToString();
                    SaveThumbnailImage(projectId, thumbId, resized);
                }

                return new ImageAtLocation(
                        projectId,
                        null,
                        path,
                        thumbId,
                        locationType,
                        GetFileCreator(path),
                        imageTakenTime,
                        0
                        );
            }
        }

        private static string GetFileCreator(string path)
        {
            try
            {
                string user = IO.File.GetAccessControl(path).GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
                return user;
            }
            catch
            {
                return "";
            }
        }

        protected static string ExtractThumbnailBase64(string path)
        {
            try
            {
                Image thumbnailImage = ExifThumbReader.ReadThumb(path);
                string base64thumb = ImageHelper.ImageToBase64(thumbnailImage);
                return base64thumb;
            }
            catch
            {
                Image thumb = ImageHelper.PathToThumbnail(path, 200, 200);
                string baseThumb = ImageHelper.ImageToBase64(thumb);
                return baseThumb;
            }
        }

        protected static void SaveThumbnailImage(int projectId, string guid, string base64thumb)
        {
            try
            {
                string projectThumbFolder = ImageHelper.GetProjectThumbnailsFolder(projectId);
                string thumbPath = IO.Path.Combine(projectThumbFolder, guid + ".png");

                // Convert Base64 String to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64thumb);
                using (IO.MemoryStream ms = new IO.MemoryStream(imageBytes))
                {
                    // Convert byte[] to Image
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    Image image = Image.FromStream(ms, true);

                    image.Save(thumbPath);
                    image.Dispose();
                }
            }
            catch
            {

            }
        }
    }

}
