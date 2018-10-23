using System;
using System.Collections.Generic;
using System.ComponentModel;
using PhotoVis.Models;
using PhotoVis.Util;
using PhotoVis.Interfaces;
using PhotoVis.Helpers;
using PhotoVis.Views;
using ClusterEngine;

namespace PhotoVis.ViewModel
{
    public class MapViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private int _projectId;
        private ProjectModel _currentProject;
        private string _statusText;

        #endregion

        public IView View { get; set; }

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

            EntityCollection clusterPins = new EntityCollection();

            // Start a new background worker to index the folders in the project
            foreach (ImageFoldersModel folderModel in model.ProjectFolders)
            {
                ProjectSingleFolder ps = new ProjectSingleFolder(model.ProjectId, folderModel);

                BackgroundWorker _worker = new BackgroundWorker();
                _worker.DoWork += (s, a) =>
                {
                    ProjectSingleFolder p = (ProjectSingleFolder)a.Argument;
                    AssignmentIndexer.IndexImages(p);
                    a.Result = p;
                };
                _worker.RunWorkerCompleted += (s, a) =>
                {
                    ProjectSingleFolder p = (ProjectSingleFolder)a.Result;
                    StatusText += string.Format("Indexed folder {0} {1}.\r\n", 
                        p.FolderModel.FolderPath, 
                        (p.FolderModel.IncludeSubfolders ? "and all subfolders" : "not including subfolders"));
                    //this.ShowAllDataPointClustered();

                    //MapView v = this.View as MapView;
                    //v.LoadDatabaseImages();
                };
                _worker.RunWorkerAsync(ps);
            }
        }

        #region Methods
        
        #endregion
    }
}
