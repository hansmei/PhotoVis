using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Data;
using System.Drawing;
using System.IO;

using PhotoVis.Interfaces;
using PhotoVis.Data.DatabaseTables;
using Microsoft.Maps.MapControl.WPF;
using ClusterEngine;
using PhotoVis.Util;

namespace PhotoVis.Data
{
    public class ImageAtLocation : Entity, IImageAtLocation
    {
        private string _thumbBase64;

        public int ProjectId { get; set; }
        public DateTime TimeImageTaken { get; set; }
        public DateTime TimeIndexed { get; set; }

        public string ImagePath { get; set; }
        public BitmapSource Thumbnail { get; set; }
        public int Heading { get; set; }
        public int Rotation { get; set; }
        public string Title
        {
            get
            {
                return this.ID.ToString();
            }
        }

        public bool HasLocation
        {
            get
            {
                return this.Location != null;
            }
        }
        
        /// <summary>
        /// Default constructor for JSON serialization
        /// </summary>
        public ImageAtLocation()
        {
        }

        public ImageAtLocation(DataRow row)
        {
            this.ID = int.Parse(row[DImageAtLocation.Id].ToString());
            this.ProjectId = int.Parse(row[DImageAtLocation.ProjectId].ToString());
            this.ImagePath = row[DImageAtLocation.ImagePath].ToString();
            
            if (row[DImageAtLocation.Latitude].ToString() != "")
            {
                this.Location = new Location(
                        double.Parse(row[DImageAtLocation.Latitude].ToString(), App.RegionalCulture),
                        double.Parse(row[DImageAtLocation.Longitude].ToString(), App.RegionalCulture),
                        double.Parse(row[DImageAtLocation.Altitude].ToString(), App.RegionalCulture)
                        );
            }
            else
            {
                this.Location = null;
            }

            this.Heading = int.Parse(row[DImageAtLocation.Heading].ToString());
            if(row[DImageAtLocation.Thumbnail].ToString() != "")
            {
                this._thumbBase64 = row[DImageAtLocation.Thumbnail].ToString();
            }
            //this.Rotation = int.Parse(row[DImageAtLocation.Rotation].ToString());

            this.TimeImageTaken = DateTime.Parse(row[DImageAtLocation.TimeImageTaken].ToString(), App.RegionalCulture);
            this.TimeIndexed = DateTime.Parse(row[DImageAtLocation.TimeIndexed].ToString(), App.RegionalCulture);

            this.GetThumbnailImage();
        }

        public ImageAtLocation(int projectId, Location location, string path, string thumbnailBase64, DateTime imageTakenTime, double heading)
        {
            this.ProjectId = projectId;
            this.ImagePath = path;
            this._thumbBase64 = thumbnailBase64;
            this.Location = location;
            this.Heading = (int)heading;
            this.Rotation = 0;
            this.TimeImageTaken = imageTakenTime;

            this.GetThumbnailImage();
        }

        public void SetThumbnailImage(string base64string)
        {
            this._thumbBase64 = base64string;
            Image image = ImageHelper.Base64ToImage(this._thumbBase64);
            byte[] imgBytes = ImageHelper.BytesFromImage(image);
            BitmapSource source = ImageHelper.BitmapImageFromBuffer(imgBytes);
            this.Thumbnail = source;
        }

        private void GetThumbnailImage()
        {
            if(this._thumbBase64 != "" && this._thumbBase64 != null)
            {
                Image image = ImageHelper.Base64ToImage(this._thumbBase64);
                byte[] imgBytes = ImageHelper.BytesFromImage(image);
                BitmapSource source = ImageHelper.BitmapImageFromBuffer(imgBytes);
                this.Thumbnail = source;
            }
            //string thumb = ImageHelper.GetProjectThumbnailsFolder(this.ProjectId);
            //string thumbSource = Path.Combine(thumb, Path.GetFileNameWithoutExtension(this.ImagePath) + ".txt");
            
            //if (File.Exists(thumbSource) && !FileHelper.IsFileLocked(thumbSource))
            //{
            //    string base64string = File.ReadAllText(thumbSource);
            //    Image image = ImageHelper.Base64ToImage(base64string);
            //    byte[] imgBytes = ImageHelper.BytesFromImage(image);
            //    BitmapSource source = ImageHelper.BitmapImageFromBuffer(imgBytes);

            //    this.Thumbnail = source;
            //}
            //else
            //{
            //    // TODO: Fix this
            //    this.Thumbnail = null;
            //}
        }

        public int SaveToDatabase()
        {
            // Write to database
            Dictionary<string, object> row = new Dictionary<string, object>();

            row.Add(DImageAtLocation.ProjectId, this.ProjectId);
            row.Add(DImageAtLocation.ImagePath, this.ImagePath);
            
            if(this.Location != null)
            {
                row.Add(DImageAtLocation.Latitude, this.Location.Latitude.ToString(App.RegionalCulture));
                row.Add(DImageAtLocation.Longitude, this.Location.Longitude.ToString(App.RegionalCulture));
                row.Add(DImageAtLocation.Altitude, this.Location.Altitude.ToString(App.RegionalCulture));
            }
            row.Add(DImageAtLocation.Heading, this.Heading.ToString(App.RegionalCulture));
            row.Add(DImageAtLocation.Rotation, this.Rotation.ToString(App.RegionalCulture));

            if(this._thumbBase64 != null && this._thumbBase64 != "")
            {
                row.Add(DImageAtLocation.Thumbnail, this._thumbBase64);
            }

            row.Add(DImageAtLocation.TimeImageTaken, this.TimeImageTaken.ToString(App.RegionalCulture));
            row.Add(DImageAtLocation.TimeIndexed, DateTime.Now.ToString(App.RegionalCulture));

            int numAffected = App.DB.InsertValue(DTables.Images, row);
            return numAffected;
        }
    }
}
