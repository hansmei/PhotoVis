using System;
using System.Collections.Generic;
using System.Linq;
using IO = System.IO;
using Microsoft.Maps.MapControl.WPF;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

using PhotoVis.Data;
using PhotoVis.Helpers;

namespace PhotoVis.Util
{
    class AssignmentIndexer
    {

        public static void IndexImages(ProjectSingleFolder singleFolderToIndex)
        {
            List<ImageAtLocation> images = GetImagesList(singleFolderToIndex);
            foreach (ImageAtLocation img in images)
            {
                img.SaveToDatabase();
            }
        }

        public static List<ImageAtLocation> GetImagesList(ProjectSingleFolder singleFolderToIndex)
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
                }
            }

            return list;

        }
    }

}
