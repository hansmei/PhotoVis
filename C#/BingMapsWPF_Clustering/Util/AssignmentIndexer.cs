using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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
        public static void LoadImagesFromDatabase(int projectId)
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
                if (img.HasLocation)
                {
                    App.MapVM.ImageLocations.Add(img);
                }
                else
                {
                    App.MapVM.UnassignedImageLocations.Add(img);
                }
            }
        }

        public static HashSet<string> GetUniquePathNames()
        {
            HashSet<string> uniquePathNames = new HashSet<string>();
            foreach(ImageAtLocation img in App.MapVM.ImageLocations)
            {
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


        public static List<ImageAtLocation> IndexImages(ImageIndexerTransporter singleFolderToIndex)
        {
            HashSet<string> unique = GetUniquePathNames();

            List<ImageAtLocation> images = GetImagesList(singleFolderToIndex);
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

        public static List<ImageAtLocation> GetImagesList(ImageIndexerTransporter singleFolderToIndex)
        {
            List<ImageAtLocation> list = new List<ImageAtLocation>();

            // Get all image files in directory and subdirectory
            if (IO.Directory.Exists(singleFolderToIndex.FolderModel.FolderPath))
            {
                IO.SearchOption method = singleFolderToIndex.FolderModel.IncludeSubfolders ? 
                    IO.SearchOption.AllDirectories : IO.SearchOption.TopDirectoryOnly;
                string[] images = IO.Directory.GetFiles(singleFolderToIndex.FolderModel.FolderPath, "*.jpg", method);
                
                foreach (string path in images)
                {
                    IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(path);

                    var gps = ImageMetadataReader.ReadMetadata(path)
                                    .OfType<GpsDirectory>()
                                    .FirstOrDefault();

                    //LocationCollection collection = new LocationCollection();

                    if (gps != null)
                    {

                        var location = gps.GetGeoLocation();

                        // TODO: Better handling of this
                        if (location == null || location.IsZero)
                            continue;

                        var thumb = ImageMetadataReader.ReadMetadata(path)
                                        .OfType<ExifThumbnailDirectory>()
                                        .FirstOrDefault();

                        // obtain a specific directory
                        var ifd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                        var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

                        if (subIfdDirectory == null)
                            continue;

                        // create a descriptor
                        //var descriptor = new ExifSubIfdDescriptor(subIfdDirectory);
                        //string res = descriptor.GetOrientationDescription();

                        // get tag description
                        DateTime imageTakenTime;
                        bool gotDateTime = ifd0Directory.TryGetDateTime(GpsDirectory.TagDateTime, out imageTakenTime);

                        float degrees = 0;
                        bool hasDegrees = gps.TryGetSingle(GpsDirectory.TagImgDirection, out degrees);

                        list.Add(
                            new ImageAtLocation(
                                singleFolderToIndex.ProjectId,
                                new Location(location.Latitude, location.Longitude),
                                path,
                                imageTakenTime,
                                degrees
                                )
                            );

                    }
                    else
                    {
                        // Image dont have image data, allow for addition still
                        var thumb = ImageMetadataReader.ReadMetadata(path)
                                        .OfType<ExifThumbnailDirectory>()
                                        .FirstOrDefault();

                        // obtain a specific directory
                        var ifd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                        var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

                        if (subIfdDirectory == null)
                            continue;
                        
                        // get tag description
                        DateTime imageTakenTime;
                        bool gotDateTime = ifd0Directory.TryGetDateTime(GpsDirectory.TagDateTime, out imageTakenTime);
                        
                        list.Add(
                            new ImageAtLocation(
                                singleFolderToIndex.ProjectId,
                                null,
                                path,
                                imageTakenTime,
                                0
                                )
                            );
                    }
                }
            }

            return list;

        }
    }

}
