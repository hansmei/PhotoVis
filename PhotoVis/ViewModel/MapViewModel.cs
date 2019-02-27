using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Configuration;
using Microsoft.Maps.MapControl.WPF;
using Color = System.Drawing.Color;
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
        private string _overlayLoadMessage = "";
        private double _lowerAgeValue = 0;
        private double _upperAgeValue = 100;
        private bool _unassignedVisible = false;
        private bool _showLoadingMessage = false;
        private ImageAtLocationCollection _imageLocations = new ImageAtLocationCollection();
        private ImageAtLocationCollection _filteredImageLocations = new ImageAtLocationCollection();
        private ImageAtLocationCollection _unassignedImageLocations = new ImageAtLocationCollection();
        private ImageAtLocationCollection _filteredUnassignedImageLocations = new ImageAtLocationCollection();

        private ICommand _startIndexCommand;

        #endregion

        #region Properties/Commands

        public string Name
        {
            get { return "Map display"; }
        }

        private readonly ApplicationIdCredentialsProvider bingMapsCredentials; // = new ApplicationIdCredentialsProvider(ConfigurationManager.AppSettings.Get("BingMapsKey"));
        public ApplicationIdCredentialsProvider BingMapKey
        {
            get
            {
                return bingMapsCredentials;
            }
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
                if (_unassignedVisible != value)
                {
                    _unassignedVisible = value;
                    OnPropertyChanged("UnassignedVisible");
                }
            }
        }

        public string OverlayLoadMessage
        {
            get
            {
                return _overlayLoadMessage;
            }
            set
            {
                if(_overlayLoadMessage != value)
                {
                    _overlayLoadMessage = value;
                    if (_overlayLoadMessage == "")
                        this.ShowLoadingText = false;
                    else
                        this.ShowLoadingText = true;
                    OnPropertyChanged("OverlayLoadMessage");
                }
            }
        }


        public bool ShowLoadingText
        {
            get
            {
                return _showLoadingMessage;
            }
            set
            {
                if (_showLoadingMessage != value)
                {
                    _showLoadingMessage = value;
                    OnPropertyChanged("ShowLoadingText");
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

        public ImageAtLocationCollection FilterdImageLocations
        {
            get
            {
                return _filteredImageLocations;
            }
        }

        public ImageAtLocationCollection UnassignedImageLocations
        {
            get
            {
                return _unassignedImageLocations;
            }
        }

        public ImageAtLocationCollection FilterdUnassignedImageLocations
        {
            get
            {
                return _filteredUnassignedImageLocations;
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

        public double LowerAgeValue
        {
            get
            {
                return _lowerAgeValue;
            }
            set
            {
                if(value != _lowerAgeValue)
                {
                    _lowerAgeValue = value;
                    OnPropertyChanged("LowerAgeValue");
                }
            }
        }

        public double UpperAgeValue
        {
            get
            {
                return _upperAgeValue;
            }
            set
            {
                if (value != _upperAgeValue)
                {
                    _upperAgeValue = value;
                    OnPropertyChanged("UpperAgeValue");
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

        public MapViewModel()
        {
            this._projectId = -1;
            this._currentProject = new ProjectModel(-1, "Empty");
            this._imageLocations = new ImageAtLocationCollection();
            this._unassignedImageLocations = new ImageAtLocationCollection();

            App.MapVM = this;
        }

        public MapViewModel(ProjectModel model)
        {

            BingMapsCredentialsProvider bingMapsCredentials = new BingMapsCredentialsProvider();
            this.bingMapsCredentials = new ApplicationIdCredentialsProvider(bingMapsCredentials.Key);

            this._projectId = model.ProjectId;
            this._currentProject = model;
            this._imageLocations = new ImageAtLocationCollection();
            this._unassignedImageLocations = new ImageAtLocationCollection();

            // Set this to capture the change events
            this._filteredUnassignedImageLocations.CollectionChanged += _unassignedImageLocations_CollectionChanged;

            App.MapVM = this; // Need this as this is not set until after the constructor is complete

            // Create first load of content
            Tuple<int, int, int> data = AssignmentIndexer.LoadImagesFromDatabase(this._projectId);
            int numAvailableImages = data.Item1 + data.Item2;
            StatusText += string.Format("{0} Images loaded from database, where {1} are missing GPS information.\r\n",
                numAvailableImages, data.Item2);

            // Print error text that some images could not be found or is erased
            if(data.Item3 > 0)
            {
                StatusText += string.Format("{0} Images could no longer be found. They are either moved or erased.\r\n", data.Item3);
            }

            this.ApplyImageFilters();

            if(numAvailableImages == 0)
            {
                //this.StartIndexBackgroundworker(model);
                //this.StartIndexingAsync(model);
                Task.Delay(800).ContinueWith(
                    t => this.StartIndexBackgroundworker(model)
                    );
            }
        }

        public void StartIndexBackgroundworker(ProjectModel model)
        {
            StatusText += string.Format("Started indexing all folders\r\n");
            this.OverlayLoadMessage = "Please wait\r\nLoading images from the specified folders.";

            foreach (ImageFoldersModel folderModel in model.ProjectFolders)
            {
                // Get all folders subdirectories
                SearchOption method = folderModel.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                ImageIndexerTransporter ps = new ImageIndexerTransporter(model.ProjectId, folderModel.FolderPath);
                
                BackgroundWorker _worker = new BackgroundWorker();
                _worker.DoWork += (s, a) =>
                {
                    ImageIndexerTransporter p = (ImageIndexerTransporter)a.Argument;
                    
                    List<string> allDirectories = FileHelper.GetDirectories(ps.FolderPath, searchOption: method);
                    allDirectories.Add(ps.FolderPath); // Make sure we add the current folder to the query

                    // Start async tasks for each subdirectory to index its contents
                    List<ImageAtLocation> images = new List<ImageAtLocation>();
                    foreach (string folder in allDirectories)
                    {
                        ImageIndexerTransporter transport = new ImageIndexerTransporter(p.ProjectId, folder);
                        List<ImageAtLocation> tmp = AssignmentIndexer.IndexImages(transport, SearchOption.TopDirectoryOnly);
                        images.AddRange(tmp);
                        StatusText += string.Format("Indexed folder {0}.\r\n", folder);
                    }

                    p.SetImages(images);
                    a.Result = p;
                };
                _worker.RunWorkerCompleted += (s, a) =>
                {
                    ImageIndexerTransporter p = (ImageIndexerTransporter)a.Result;

                    // Write to time last indexed
                    Dictionary<string, object> where = new Dictionary<string, object>();
                    where.Add(DAssignment.ProjectId, p.ProjectId);

                    Dictionary<string, object> update = new Dictionary<string, object>();
                    update.Add(DAssignment.TimeLastIndexed, DateTime.Now.ToString(App.RegionalCulture));
                    int numAffected = App.DB.UpdateValue(DTables.Assignments, where, update);

                    StatusText += string.Format("Completed all subfolders of {0}.\r\n", p.FolderPath);
                    StatusText += string.Format("To get an overview of all images without GPS coordinates, re-open this project.");

                    App.MapVM.ImageLocations.SetProjectId(p.ProjectId);

                    // Extract valid and invalid locations
                    List<ImageAtLocation> unknownLocations = new List<ImageAtLocation>();
                    List<ImageAtLocation> validLocation = AssignmentIndexer.ExtractValidLocations(p.Images, out unknownLocations);
                    App.MapVM.ImageLocations.AddRange(validLocation);
                    //App.MapVM.UnassignedImageLocations.AddRange(unknownLocations);

                    this.ApplyImageFilters();
                    this.OverlayLoadMessage = "";
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
                allDirectories.Add(folderModel.FolderPath); // Make sure we add the current folder to the query

                // Start async tasks for each subdirectory to index its contents
                foreach (string folder in allDirectories)
                {
                    //ImageIndexerTransporter ps = new ImageIndexerTransporter(model.ProjectId, folder);

                    //ImageIndexerTransporter modified = await this.AsyncIndexFolder(ps).ConfigureAwait(false);
                    //int numAdded = await this.AsyncModifyCollections(modified).ConfigureAwait(false);


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
                        this.ApplyImageFilters();

                        // Write to time last indexed
                        Dictionary<string, object> where = new Dictionary<string, object>();
                        where.Add(DAssignment.ProjectId, p.ProjectId);

                        Dictionary<string, object> update = new Dictionary<string, object>();
                        update.Add(DAssignment.TimeLastIndexed, DateTime.Now.ToString(App.RegionalCulture));
                        int numAffected = App.DB.UpdateValue(DTables.Assignments, where, update);
                    }//, System.Threading.CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default
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
                //App.MapVM.ImageLocations.TriggerCollectionChanged(true);
                this.ApplyImageFilters();
                App.MapVM.FilterdImageLocations.TriggerCollectionChanged(true);
                App.MapVM.UnassignedImageLocations.TriggerCollectionChanged(true);
            }).ConfigureAwait(false);
        }

        public void ApplyImageFilters()
        {
            IntervalToAgeFilter.SetIntervalToAgeFilter();

            DateTime startTime = IntervalToAgeFilter.ValueToDateTime(LowerAgeValue);
            DateTime endTime = IntervalToAgeFilter.ValueToDateTime(UpperAgeValue);

            List<ImageAtLocation> list =
                _imageLocations.Where(s => s.TimeImageTaken >= startTime && s.TimeImageTaken <= endTime).Select(s => s).ToList();
            this._filteredImageLocations.Clear();
            this._filteredImageLocations.AddRange(list);

            List<ImageAtLocation> listUnassigned =
                _unassignedImageLocations.Where(s => s.TimeImageTaken >= startTime && s.TimeImageTaken <= endTime).Select(s => s).ToList();
            this._filteredUnassignedImageLocations.Clear();
            this._filteredUnassignedImageLocations.AddRange(listUnassigned);
        }


        public string GetPushpinColors(ImageAtLocation img, out string strokeHex)
        {
            double percentAlongTotalRange = IntervalToAgeFilter.ImageToParameter(img);
            Color fillColor = ColorHelper.GetBlendedColor(percentAlongTotalRange);
            //Color fillColor = HSL2RGB(0.8, percentAlongTotalRange, 0.7, 0.6);
            //Color fillColor = RainBowColor(percentAlongTotalRange * 255, 255);
            //string fillHex = fillColor.ToString();
            string fillHex = "#" + fillColor.Name.ToString();
            Color strokeColor = ColorHelper.GetBlendedDarkerColor(percentAlongTotalRange);
            //Color strokeColor = HSL2RGB(0.8, percentAlongTotalRange, 0.7, 0.2);
            //Color strokeColor = RainBowColor(percentAlongTotalRange * 255, 255, 0);
            //strokeHex = strokeColor.ToString();
            strokeHex = "#" + strokeColor.Name.ToString();

            //Color t = HSL2RGB(percentAlongTotalRange, 0.5, 0.5);

            return fillHex;
        }
        

        #region Events

        private void _unassignedImageLocations_CollectionChanged(object sender, EntityCollectionChangedEventArgs e)
        {
            this.UnassignedVisible = e.Entities.Count > 0;
        }


        #endregion
    }
}
