﻿using System;
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
        public enum LocationSourceType
        {
            Unknown,
            Manual,
            GPS
        }

        private string _thumbPathGuid;
        //private string _thumbBase64;

        public int ProjectId { get; set; }
        public DateTime TimeImageTaken { get; set; }
        public DateTime TimeIndexed { get; set; }

        public string ImagePath { get; set; }
        //public BitmapSource Thumbnail { get; set; }
        public string ThumbnailPath { get; set; }
        public int Heading { get; set; }
        public int Rotation { get; set; }
        public LocationSourceType LocationSource { get; set; }
        public string Creator { get; set; }

        public string Title
        {
            get
            {
                return this.Creator + " " + Path.GetFileName(this.ImagePath);
            }
        }

        public bool HasLocation
        {
            get
            {
                return this.Location != null;
            }
        }

        public bool Exists
        {
            get
            {
                return File.Exists(this.ImagePath);
            }
        }

        public string ThumbnailGuid
        {
            get
            {
                return _thumbPathGuid;
            }
            set
            {
                _thumbPathGuid = value;
            }
        }

        public string ThumbnailToolTip
        {
            get
            {
                return this.Creator + " " + this.TimeImageTaken.ToString(App.RegionalCulture);
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
                this._thumbPathGuid = row[DImageAtLocation.Thumbnail].ToString();
                string thumbPath = Path.Combine(ImageHelper.GetProjectThumbnailsFolder(this.ProjectId),
                    this._thumbPathGuid + ".png");
                this.ThumbnailPath = thumbPath;
            }
            //this.Rotation = int.Parse(row[DImageAtLocation.Rotation].ToString());
            this.LocationSource = (LocationSourceType)int.Parse(row[DImageAtLocation.LocationSource].ToString());
            this.Creator = row[DImageAtLocation.Creator].ToString();

            this.TimeImageTaken = DateTime.Parse(row[DImageAtLocation.TimeImageTaken].ToString(), App.RegionalCulture);
            this.TimeIndexed = DateTime.Parse(row[DImageAtLocation.TimeIndexed].ToString(), App.RegionalCulture);
        }

        public ImageAtLocation(int projectId, Location location, string path, string thumbnailBase64, LocationSourceType sourceType, string creator, DateTime imageTakenTime, double heading)
        {
            this.ProjectId = projectId;
            this.ImagePath = path;

            this._thumbPathGuid = thumbnailBase64;
            string thumbPath = Path.Combine(ImageHelper.GetProjectThumbnailsFolder(this.ProjectId),
                this._thumbPathGuid + ".png");
            this.ThumbnailPath = thumbPath;
            this.Location = location;
            this.Heading = (int)heading;
            this.Rotation = 0;
            this.LocationSource = sourceType;
            this.Creator = creator;
            this.TimeImageTaken = imageTakenTime;
        }
        
        public int SaveToDatabase()
        {
            // Write to database
            Dictionary<string, object> row = new Dictionary<string, object>();
            Dictionary<string, object> where = new Dictionary<string, object>();

            if (this.ID != 0)
            {
                where.Add(DImageAtLocation.Id, this.ID);
            }
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

            if(this._thumbPathGuid != null && this._thumbPathGuid != "")
            {
                row.Add(DImageAtLocation.Thumbnail, this._thumbPathGuid);
            }
            row.Add(DImageAtLocation.LocationSource, (int)this.LocationSource);
            row.Add(DImageAtLocation.Creator, this.Creator);
            row.Add(DImageAtLocation.TimeImageTaken, this.TimeImageTaken.ToString(App.RegionalCulture));
            row.Add(DImageAtLocation.TimeIndexed, DateTime.Now.ToString(App.RegionalCulture));
            int numAffected = 0;
            if (this.ID != 0)
            {
                numAffected = App.DB.UpdateValue(DTables.Images, where, row);
            }
            else
            {
                numAffected = App.DB.InsertValue(DTables.Images, row);
            }
            return numAffected;
        }
    }
}
