using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using PhotoVis.Models;
using PhotoVis.Util;
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
        private string _timeLastIndexed;
        private bool _unassignedVisible = false;
        private ImageAtLocationCollection _imageLocations = new ImageAtLocationCollection();
        private ImageAtLocationCollection _unassignedImageLocations = new ImageAtLocationCollection();

        private ICommand _startIndexCommand;

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

        public ICommand StartIndexCommand
        {
            get
            {
                if(_startIndexCommand == null)
                {
                    _startIndexCommand = new RelayCommand(
                        param => StartIndexingAsync(this._currentProject)
                    );
                }
                return _startIndexCommand;
            }
        }

        public bool UnassignedVisible
        {
            get
            {
                return _unassignedImageLocations.Count > 0;
            }
            set
            {
                if(_unassignedVisible != value)
                {
                    _unassignedVisible = value;
                    OnPropertyChanged("UnassignedVisible");
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

        public string TimeLastIndexed
        {
            get
            {
                return _currentProject.TimeLastIndexed.ToString();
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
            this._unassignedImageLocations = new ImageAtLocationCollection();

            // Set this to capture the change events
            this._unassignedImageLocations.CollectionChanged += _unassignedImageLocations_CollectionChanged;

            App.MapVM = this; // Need this as this is not set until after the constructor is complete

            // Create first load of content
            Tuple<int, int> data = AssignmentIndexer.LoadImagesFromDatabase(this._projectId);
            int numAvailableImages = data.Item1 + data.Item2;
            StatusText += string.Format("{0} Images loaded from database, where {1} are missing GPS information\r\n", numAvailableImages, data.Item2);

            if(numAvailableImages == 0)
            {
                //this.StartIndexBackgroundworker(model);
                //this.StartIndexingAsync(model);
                Task.Delay(4000).ContinueWith(
                    t => this.StartIndexingAsync(model)
                    );
            }
        }

        public void StartIndexBackgroundworker(ProjectModel model)
        {
            foreach (ImageFoldersModel folderModel in model.ProjectFolders)
            {
                // Get all folders subdirectories
                SearchOption method = folderModel.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                ImageIndexerTransporter ps = new ImageIndexerTransporter(model.ProjectId, folderModel.FolderPath);
                
                BackgroundWorker _worker = new BackgroundWorker();
                _worker.DoWork += (s, a) =>
                {
                    ImageIndexerTransporter p = (ImageIndexerTransporter)a.Argument;
                    List<ImageAtLocation> images = AssignmentIndexer.IndexImages(p, method);

                    p.SetImages(images);
                    a.Result = p;
                };
                _worker.RunWorkerCompleted += (s, a) =>
                {
                    ImageIndexerTransporter p = (ImageIndexerTransporter)a.Result;
                    StatusText += string.Format("Indexed folder {0}.\r\n", p.FolderPath);

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

        public async void StartIndexingAsync(ProjectModel model)
        {

            // Start a new background worker to index the folders in the project
            List<Task> tasks = new List<Task>();
            foreach (ImageFoldersModel folderModel in model.ProjectFolders)
            {
                // Get all folders subdirectories

                SearchOption method = folderModel.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                List<string> allDirectories = FileHelper.GetDirectories(folderModel.FolderPath, searchOption: method);

                // Start async tasks for each subdirectory to index its contents
                foreach (string folder in allDirectories)
                {
                    ImageIndexerTransporter ps = new ImageIndexerTransporter(model.ProjectId, folder);
                    tasks.Add(Task.Run(() =>
                    {
                        List<ImageAtLocation> images = AssignmentIndexer.IndexImages(ps, SearchOption.TopDirectoryOnly);
                        ps.SetImages(images);
                        return ps;
                    })
                    .ContinueWith((r) =>
                    {
                        ImageIndexerTransporter p = r.Result;
                        StatusText += string.Format("Indexed folder {0}.\r\n", p.FolderPath);

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
                    }
                    ));

                    //BackgroundWorker _worker = new BackgroundWorker();
                    //_worker.DoWork += (s, a) =>
                    //{
                    //    ImageIndexerTransporter p = (ImageIndexerTransporter)a.Argument;
                    //    List<ImageAtLocation> images = AssignmentIndexer.IndexImages(p);

                    //    p.SetImages(images);
                    //    a.Result = p;
                    //};
                    //_worker.RunWorkerCompleted += (s, a) =>
                    //{
                    //    ImageIndexerTransporter p = (ImageIndexerTransporter)a.Result;
                    //    StatusText += string.Format("Indexed folder {0}.\r\n", p.FolderPath);

                    //    App.MapVM.ImageLocations.SetProjectId(p.ProjectId);

                    //    // Extract valid and invalid locations
                    //    List<ImageAtLocation> unknownLocations = new List<ImageAtLocation>();
                    //    List<ImageAtLocation> validLocation = AssignmentIndexer.ExtractValidLocations(p.Images, out unknownLocations);
                    //    App.MapVM.ImageLocations.AddRange(validLocation);
                    //    App.MapVM.UnassignedImageLocations.AddRange(unknownLocations);

                    //    // Write to time last indexed
                    //    Dictionary<string, object> where = new Dictionary<string, object>();
                    //    where.Add(DAssignment.ProjectId, p.ProjectId);

                    //    Dictionary<string, object> update = new Dictionary<string, object>();
                    //    update.Add(DAssignment.TimeLastIndexed, DateTime.Now.ToString(App.RegionalCulture));
                    //    int numAffected = App.DB.UpdateValue(DTables.Assignments, where, update);

                    //};
                    //_worker.RunWorkerAsync(ps);
                }
            }

            await Task.WhenAll(tasks).ContinueWith(t =>
            {
                // write your code here
                StatusText += string.Format("Completed indexing all folders.\r\n");

            });
        }

        #region Events

        private void _unassignedImageLocations_CollectionChanged(object sender, EntityCollectionChangedEventArgs e)
        {
            this.UnassignedVisible = e.Entities.Count > 0;
        }


        #endregion
    }
}
