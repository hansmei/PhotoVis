using System;
using System.Collections.Generic;
using System.Data;

using PhotoVis.Interfaces;
using PhotoVis.Data.DatabaseTables;
using Microsoft.Maps.MapControl.WPF;
using ClusterEngine;

namespace PhotoVis.Data
{
    public class ImageAtLocation : Entity, IImageAtLocation
    {
        public int AssignmentNumber { get; set; }
        public DateTime TimeImageTaken { get; set; }
        public DateTime TimeIndexed { get; set; }

        public string ImagePath { get; set; }
        public int Heading { get; set; }
        public int Rotation { get; set; }
        public string Title
        {
            get
            {
                return this.ID.ToString();
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
            this.AssignmentNumber = int.Parse(row[DImageAtLocation.AssignmentNumber].ToString());
            this.ImagePath = row[DImageAtLocation.ImagePath].ToString();

            this.Location = new Location(
                    double.Parse(row[DImageAtLocation.Latitude].ToString(), App.RegionalCulture),
                    double.Parse(row[DImageAtLocation.Longitude].ToString(), App.RegionalCulture)
                    );
            this.Heading = int.Parse(row[DImageAtLocation.Heading].ToString());
            //this.Rotation = int.Parse(row[DImageAtLocation.Rotation].ToString());

            this.TimeImageTaken = DateTime.Parse(row[DImageAtLocation.TimeImageTaken].ToString());
            this.TimeIndexed = DateTime.Parse(row[DImageAtLocation.TimeIndexed].ToString());
        }

        public ImageAtLocation(int assignmentNumber, Location location, string path, DateTime imageTakenTime, double heading)
        {
            this.AssignmentNumber = assignmentNumber;
            this.ImagePath = path;
            this.Location = location;
            this.Heading = (int)heading;
            this.Rotation = 0;
            this.TimeImageTaken = imageTakenTime;
        }

        public int SaveToDatabase()
        {
            // Write to database
            Dictionary<string, object> row = new Dictionary<string, object>();

            row.Add(DImageAtLocation.AssignmentNumber, this.AssignmentNumber);
            row.Add(DImageAtLocation.ImagePath, this.ImagePath);

            row.Add(DImageAtLocation.Latitude, this.Location.Latitude.ToString(App.RegionalCulture));
            row.Add(DImageAtLocation.Longitude, this.Location.Longitude.ToString(App.RegionalCulture));
            row.Add(DImageAtLocation.Heading, this.Heading.ToString(App.RegionalCulture));
            row.Add(DImageAtLocation.Rotation, this.Rotation.ToString(App.RegionalCulture));

            row.Add(DImageAtLocation.TimeImageTaken, this.TimeImageTaken.ToString());

            int numAffected = App.DB.InsertValue(DTables.Images, row);
            return numAffected;
        }
    }
}
