using System;
using System.Collections.Generic;
using System.ComponentModel;
using PhotoVis.Models;
using PhotoVis.Util;
using PhotoVis.Interfaces;
using PhotoVis.Helpers;
using PhotoVis.Data;
using ClusterEngine;

using PhotoVis.Data.DatabaseTables;

namespace PhotoVis.ViewModel
{
    public class MapViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private int _projectId;
        private ProjectModel _currentProject;
        private string _statusText;
        private ImageAtLocationCollection _imageLocations = new ImageAtLocationCollection();
        private ImageAtLocationCollection _unassignedImageLocations = new ImageAtLocationCollection();

        #endregion

        #region Properties/Commands

        public string Name
        {
            get { return "Map display"; }
        }

        public int ProjectId
        {
            get { return _projectId; }
            set
            {
                if (value != _projectId)
                {
                    _projectId = value;
                    OnPropertyChanged("ProjectId");
                }
            }
        }

        public ImageAtLocationCollection ImageLocations
        {
            get
            {
                return _imageLocations;
            }
        }

        public ImageAtLocationCollection UnassignedImageLocations
        {
            get
            {
                return _unassignedImageLocations;
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                if (value != _statusText)
                {
                    _statusText = value;
                    OnPropertyChanged("StatusText");
                }
            }
        }

        public ProjectModel CurrentProject
        {
            get { return _currentProject; }
            set
            {
                if (value != _currentProject)
                {
                    _currentProject = value;
                    OnPropertyChanged("CurrentProject");
                }
            }
        }

        #endregion

        public MapViewModel(ProjectModel model)
        {
            this._projectId = model.ProjectId;
            this._currentProject = model;
            this._imageLocations = new ImageAtLocationCollection();

            App.MapVM = this; // Need this as this is not set until after the constructor is complete

            // Create first load of content
            AssignmentIndexer.LoadImagesFromDatabase(this._projectId);
            StatusText += "Images loaded from database.\r\n";
            
            // Start a new background worker to index the folders in the project
            foreach (ImageFoldersModel folderModel in model.ProjectFolders)
            {
                ImageIndexerTransporter ps = new ImageIndexerTransporter(model.ProjectId, folderModel);

                BackgroundWorker _worker = new BackgroundWorker();
                _worker.DoWork += (s, a) =>
                {
                    ImageIndexerTransporter p = (ImageIndexerTransporter)a.Argument;
                    List<ImageAtLocation> images = AssignmentIndexer.IndexImages(p);

                    p.SetImages(images);
                    a.Result = p;
                };
                _worker.RunWorkerCompleted += (s, a) =>
                {
                    ImageIndexerTransporter p = (ImageIndexerTransporter)a.Result;
                    StatusText += string.Format("Indexed folder {0} {1}.\r\n", 
                        p.FolderModel.FolderPath, 
                        (p.FolderModel.IncludeSubfolders ? "and all subfolders" : "not including subfolders"));
                    
                    App.MapVM.ImageLocations.SetProjectId(p.ProjectId);

                    // Extract valid and invalid locations
                    List<ImageAtLocation> unknownLocations = new List<ImageAtLocation>();
                    List<ImageAtLocation> validLocation = AssignmentIndexer.ExtractValidLocations(p.Images, out unknownLocations);
                    App.MapVM.ImageLocations.AddRange(validLocation);
                    App.MapVM.UnassignedImageLocations.AddRange(unknownLocations);

                    // Write to time last indexed
                    Dictionary<string, object> where = new Dictionary<string, object>();
                    where.Add(DAssignment.ProjectId, p.ProjectId);

                    Dictionary<string, object> update = new Dictionary<string, object>();
                    update.Add(DAssignment.TimeLastIndexed, DateTime.Now.ToString(App.RegionalCulture));
                    int numAffected = App.DB.UpdateValue(DTables.Assignments, where, update);

                };
                _worker.RunWorkerAsync(ps);
            }
        }

        #region Methods
        
        #endregion
    }
}
