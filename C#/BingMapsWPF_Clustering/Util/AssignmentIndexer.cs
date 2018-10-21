using System;
using System.Collections.Generic;
using System.Linq;
using IO = System.IO;

using Microsoft.Maps.MapControl.WPF;

using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

using PhotoVis.Data;

namespace PhotoVis.Util
{
    class AssignmentIndexer
    {

        public static void IndexImages()
        {
            List<ImageAtLocation> images = GetImagesList();

            foreach (ImageAtLocation img in images)
            {
                img.SaveToDatabase();
            }

        }

        public static List<ImageAtLocation> GetImagesList()
        {
            int assignmentNumber = 5141950;
            string parentDirectory = "N:\\514\\19\\5141950\\6 Bilder";
            //parentDirectory = "V:\\514\\70\\5147023\\6 Bilder";
            List<ImageAtLocation> list = new List<ImageAtLocation>();

            // Get all image files in directory and subdirectory
            if (IO.Directory.Exists(parentDirectory))
            {
                string[] images = IO.Directory.GetFiles(parentDirectory, "*.jpg", IO.SearchOption.AllDirectories);

                //string[] images = new string[]
                //{
                //    //"C:\\dev\\temp\\IMG\\PANO_20180127_142702_14.jpg",
                //    "C:\\dev\\temp\\IMG\\DSC_0482.JPG",
                //    "C:\\dev\\temp\\IMG\\DSC_0483.JPG",
                //    "C:\\dev\\temp\\IMG\\DSC_0484.JPG",
                //    "C:\\dev\\temp\\IMG\\DSC_0485.JPG",
                //    "C:\\dev\\temp\\IMG\\DSC_0486.JPG",
                //};

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
                                assignmentNumber,
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
