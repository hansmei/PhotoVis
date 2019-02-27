using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PhotoVis.Models;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using PhotoVis.Util;

namespace PhotoVis.ViewModel
{
    public class NewProjectViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private bool _projectIdAlreadyExists = false;
        private bool _canEditProjectId = true;
        private int _projectId;
        private string _projectName;
        private HashSet<int> _usedProjectIds;
        private ProjectModel _currentProject;
        private ICommand _addProjectFolderCommand;
        private ICommand _removeProjectFolderCommand;
        private ICommand _saveProjectCommand;
        private ICommand _openProjectCommand;
        private ICommand _deleteProjectCommand;

        #endregion

        #region Properties/Commands

        public string Name
        {
            get { return "New project"; }
        }

        public bool CanEditProjectId
        {
            get
            {
                return _canEditProjectId;
            }
            set
            {
                _canEditProjectId = value;
                OnPropertyChanged("CanEditProjectId");
            }
        }

        public int ProjectId
        {
            get
            {
                return _projectId;
            }
            set
            {
                if (this._usedProjectIds != null && this._usedProjectIds.Contains(value))
                {
                    this.ProjectIdAlreadyExists = true;
                }
                else
                {
                    this.ProjectIdAlreadyExists = false;
                }
                _projectId = value;
                OnPropertyChanged("ProjectId");
            }
        }

        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
                if (_currentProject == null && _projectId != 0)
                {
                    _currentProject = new ProjectModel(_projectId, _projectName);
                }
                _currentProject.ProjectName = _projectName;
                OnPropertyChanged("ProjectName");
            }
        }
        
        public ObservableCollection<ImageFoldersModel> IncludedPaths
        {
            get
            {
                if (_currentProject != null)
                    return _currentProject.ProjectFolders;
                return new ObservableCollection<ImageFoldersModel>();
            }
            set
            {
                if (_currentProject == null)
                {
                    _currentProject = new ProjectModel(_projectId, _projectName);
                }
                _currentProject.ProjectFolders = value;
                OnPropertyChanged("IncludedPaths");
            }
        }

        public bool ProjectIdAlreadyExists
        {
            get
            {
                return _projectIdAlreadyExists;
            }
            set
            {
                if(value != _projectIdAlreadyExists)
                {
                    this._projectIdAlreadyExists = value;
                    OnPropertyChanged("ProjectIdAlreadyExists");
                }
            }
        }

        public ICommand AddProjectFolderCommand
        {
            get
            {
                if (_addProjectFolderCommand == null)
                {
                    _addProjectFolderCommand = new RelayCommand(
                        param => AddFolder(),
                        param => (_projectName != null && _projectId != 0)
                        );
                }
                return _addProjectFolderCommand;
            }
        }

        public ICommand RemoveProjectFolderCommand
        {
            get
            {
                if (_removeProjectFolderCommand == null)
                {
                    _removeProjectFolderCommand = new RelayCommand(
                        param => RemoveFolder((string)param),
                        param => (_projectName != null)
                        );
                }
                return _removeProjectFolderCommand;
            }
        }

        public ICommand SaveProjectCommand
        {
            get
            {
                if (_saveProjectCommand == null)
                {
                    _saveProjectCommand = new RelayCommand(
                        param => SaveProject(),
                        param => (_currentProject != null && IncludedPaths.Count > 0 && !this._projectIdAlreadyExists)
                    );
                }
                return _saveProjectCommand;
            }
        }

        public ICommand OpenProjectCommand
        {
            get
            {
                if (_openProjectCommand == null)
                {
                    _openProjectCommand = new RelayCommand(
                        param => OpenProject(_currentProject),
                        param => (_currentProject != null && _currentProject.HasDatabase)
                    );
                }
                return _openProjectCommand;
            }
        }

        public ICommand DeleteProjectCommand
        {
            get
            {
                if (_deleteProjectCommand == null)
                {
                    _deleteProjectCommand = new RelayCommand(
                        param => DeleteProject(_currentProject),
                        param => (_currentProject != null && _currentProject.HasDatabase)
                    );
                }
                return _deleteProjectCommand;
            }
        }

        #endregion

        public NewProjectViewModel()
        {
            this.SetUsedProjectIds();
        }

        public NewProjectViewModel(ProjectModel model)
        {
            this._currentProject = model;
            this.ProjectId = model.ProjectId;
            this.ProjectName = model.ProjectName;

            this.IncludedPaths = model.ProjectFolders;
            this.CanEditProjectId = false;

            this.SetUsedProjectIds();
        }

        #region Methods

        private void SetUsedProjectIds()
        {
            List<ProjectModel> allProjects = ProjectModel.LoadAllProjects();
            _usedProjectIds = new HashSet<int>();
            foreach (ProjectModel model in allProjects)
            {
                _usedProjectIds.Add(model.ProjectId);
            }
        }

        private void SaveProject()
        {
            // Save the model in databse
            int numAffected = _currentProject.Save(this.CanEditProjectId); // Bool variable equals firstrun or edit mode
            if(numAffected > 0)
            {
                _currentProject.HasDatabase = true;
                this.CanEditProjectId = false;
            }
            OnPropertyChanged("AllProjects");
        }

        private void OpenProject(ProjectModel model)
        {
            App.VM.OpenMapView(model);
        }

        private void DeleteProject(ProjectModel model)
        {
            _currentProject.Delete();
            App.VM.OpenProjectsView();
        }

        private void AddFolder()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select folders to include";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = @"C:\dev\temp\";

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            //dlg.DefaultDirectory = currentDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = false;
            dlg.Multiselect = true;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folders = dlg.FileNames;

                ObservableCollection<ImageFoldersModel> tmp = IncludedPaths;
                foreach(string folder in folders)
                {
                    ImageFoldersModel model = new ImageFoldersModel(folder, true);
                    tmp.Add(model);
                }
                IncludedPaths = tmp;
            }

        }

        void RemoveFolder(string path)
        {
            ObservableCollection<ImageFoldersModel> tmp = IncludedPaths;
            for(int i  = tmp.Count - 1; i >= 0; i--)
            {
                if (tmp[i].FolderPath == path)
                {
                    tmp.RemoveAt(i);

                    // TODO: Delete images from the database

                }
            }
            IncludedPaths = tmp;
        }

        #endregion
    }
}
