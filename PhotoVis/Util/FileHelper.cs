using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maps.MapControl.WPF;

namespace PhotoVis.Util
{
    class FileHelper
    {
        public static Location ReadCoordinateFile(string originalImagePath)
        {
            string coordinateFilePath = GetCoordinateFilePathFromImagePaht(originalImagePath);
            if (File.Exists(coordinateFilePath))
            {
                string[] lines = File.ReadAllLines(coordinateFilePath);
                double latitude = double.Parse(lines[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                double longitude = double.Parse(lines[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                Location location = new Location(latitude, longitude);
                return location;
            }
            return null;
        }

        public static void CreateCoordinateFile(string originalImagePath, Location location)
        {
            string coordinateFileContent = "";
            coordinateFileContent += location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "\r\n";
            coordinateFileContent += location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            string newFilePath = GetCoordinateFilePathFromImagePaht(originalImagePath);
            if (!File.Exists(newFilePath))
            {
                File.WriteAllText(newFilePath, coordinateFileContent);
            }
        }

        public static string GetCoordinateFilePathFromImagePaht(string originalImagePath)
        {
            string newFilePath = Path.Combine(
                Path.GetDirectoryName(originalImagePath),
                Path.GetFileNameWithoutExtension(originalImagePath) + ".koo"
                );
            return newFilePath;
        }

        public static List<string> GetDirectories(string path, string searchPattern = "*",
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (searchOption == SearchOption.TopDirectoryOnly)
                return Directory.GetDirectories(path, searchPattern).ToList();

            var directories = new List<string>(GetDirectories(path, searchPattern));

            for (var i = 0; i < directories.Count; i++)
                directories.AddRange(GetDirectories(directories[i], searchPattern));

            return directories;
        }

        private static List<string> GetDirectories(string path, string searchPattern)
        {
            try
            {
                return Directory.GetDirectories(path, searchPattern).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
        }

        public static bool IsFileLocked(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            return IsFileLocked(fileInfo);
        }

        public static bool IsFileLocked(FileInfo file)
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

    }
}
