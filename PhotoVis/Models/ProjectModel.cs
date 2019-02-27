using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using PhotoVis.Util;
using PhotoVis.Data;
using PhotoVis.Data.DatabaseTables;

namespace PhotoVis.Models
{
    public class ProjectModel : ObservableObject
    {
        #region Fields

        private int _projectId;
        private string _projectName;
        private string _thumbnailBase64 = "";
        private DateTime _timeCreated;
        private DateTime _timeLastIndexed;
        private ObservableCollection<ImageFoldersModel> _folders = new ObservableCollection<ImageFoldersModel>();
        private bool _hasBeenIndexed = false;
        private bool _hasDatabase = false;
        private BitmapSource _thumbnail;

        #endregion // Fields

        #region Properties

        public int ProjectId
        {
            get { return _projectId; }
            set
            {
                if (value != _projectId)
                {
                    _projectId = value;
                    OnPropertyChanged("ProjectId");
                    OnPropertyChanged("ProjectDisplayName");
                }
            }
        }

        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                if (value != _projectName)
                {
                    _projectName = value;
                    OnPropertyChanged("ProjectName");
                    OnPropertyChanged("ProjectDisplayName");
                }
            }
        }

        public string ProjectDisplayName
        {
            get
            {
                return _projectId + " - " + _projectName;
            }
        }

        public BitmapSource Thumbnail
        {
            get { return _thumbnail; }
            set
            {
                if (value != _thumbnail)
                {
                    _thumbnail = value;
                    OnPropertyChanged("Thumbnail");
                }
            }
        }
        
        public DateTime TimeCreated
        {
            get { return _timeCreated; }
            set
            {
                if (value != _timeCreated)
                {
                    _timeCreated = value;
                    OnPropertyChanged("TimeCreated");
                }
            }
        }

        public DateTime TimeLastIndexed
        {
            get { return _timeLastIndexed; }
            set
            {
                if (value != _timeLastIndexed)
                {
                    _timeLastIndexed = value;
                    OnPropertyChanged("TimeLastIndexed");
                }
            }
        }

        public ObservableCollection<ImageFoldersModel> ProjectFolders
        {
            get { return _folders; }
            set
            {
                _folders = value;
                OnPropertyChanged("ProjectFolders");
            }
        }

        public bool HasBeenIndexed
        {
            get
            {
                return _hasBeenIndexed;
            }
            set
            {
                if (_hasBeenIndexed != value)
                {
                    _hasBeenIndexed = value;
                    OnPropertyChanged("HasBeenIndexed");
                }
            }
        }

        public bool HasDatabase
        {
            get
            {
                return _hasDatabase;
            }
            set
            {
                if (_hasDatabase != value)
                {
                    _hasDatabase = value;
                    OnPropertyChanged("HasDatabase");
                }
            }
        }

        #endregion // Properties

        public ProjectModel(int id, string name)
        {
            this._projectId = id;
            this._projectName = name;
            this.SetThumbnailSource();
        }

        public ProjectModel(DataRow row)
        {
            this.ProjectId = int.Parse(row[DAssignment.ProjectId].ToString());
            this.ProjectName = row[DAssignment.ProjectName].ToString();
            this.HasDatabase = true;

            try
            {
                this._thumbnailBase64 = row[DAssignment.Thumbnail].ToString();
                this.SetThumbnailSource();
            } catch { }

            try
            {
                this.TimeCreated = DateTime.Parse(row[DAssignment.TimeCreated].ToString(), App.RegionalCulture);
                this.TimeLastIndexed = DateTime.Parse(row[DAssignment.TimeLastIndexed].ToString(), App.RegionalCulture);
            }
            catch { }
        }

        public void SetThumbnailSource()
        {
            string path = ImageHelper.GetProjectThumbnailBase64Path(this.ProjectId);
            //if (this._thumbnailBase64 != "")
            //{
            //    Image image = ImageHelper.Base64ToImage(this._thumbnailBase64);
            //    byte[] imgBytes = ImageHelper.BytesFromImage(image);
            //    BitmapSource source = ImageHelper.BitmapImageFromBuffer(imgBytes);

            //    this.Thumbnail = source;
            //}
            //else 
            if (File.Exists(path))
            {
                string base64string = File.ReadAllText(path);
                this._thumbnailBase64 = base64string;

                Image image = ImageHelper.Base64ToImage(this._thumbnailBase64);
                byte[] imgBytes = ImageHelper.BytesFromImage(image);
                BitmapSource source = ImageHelper.BitmapImageFromBuffer(imgBytes);

                this.Thumbnail = source;
            }
            else
            {
                // TODO: Fix this
                this.Thumbnail = null;
            }
            //string path = ImageHelper.GetProjectThumbnailBase64Path(this.ProjectId);
            //if (File.Exists(path))
            //{
            //    string base64string = File.ReadAllText(path);

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

        public static List<ProjectModel> LoadAllProjects()
        {
            DataTable project = App.DB.GetValues(DTables.Assignments, null, new string[] { "*" });
            List<ProjectModel> projects = new List<ProjectModel>();
            foreach(DataRow row in project.Rows)
            {
                ProjectModel mod = LoadProject(int.Parse(row[DAssignment.ProjectId].ToString()));
                projects.Add(mod);
            }

            List<ProjectModel> ordered = projects.OrderByDescending(s => s.TimeCreated).ToList();
            return ordered;
        }

        public static ProjectModel LoadProject(int id)
        {
            Dictionary<string, object> whereProject = new Dictionary<string, object>();
            whereProject.Add(DAssignment.ProjectId, id);
            DataTable data = App.DB.GetValues(DTables.Folders, whereProject, new string[] { "*" });
            ObservableCollection<ImageFoldersModel> folders = new ObservableCollection<ImageFoldersModel>();
            foreach (DataRow row in data.Rows)
            {
                ImageFoldersModel folder = new ImageFoldersModel(row);
                folders.Add(folder);
            }
            Dictionary<string, object> where = new Dictionary<string, object>();
            where.Add(DAssignment.ProjectId, id);
            DataTable project = App.DB.GetValues(DTables.Assignments, where, new string[] { "*" });

            DataRow p = project.Rows[0];

            ProjectModel model = new ProjectModel(p);
            model.ProjectFolders = folders;
            return model;
        }

        public int Save(bool firstTimeCreation)
        {
            int numAffected = 0;
            if (firstTimeCreation)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                row.Add(DAssignment.ProjectId, this.ProjectId);
                row.Add(DAssignment.ProjectName, this.ProjectName);
                row.Add(DAssignment.Latitude, 0.0);
                row.Add(DAssignment.Longitude, 0.0);
                row.Add(DAssignment.TimeCreated, DateTime.Now.ToString(App.RegionalCulture));
                numAffected = App.DB.InsertValue(DTables.Assignments, row);
            }
            else
            {
                Dictionary<string, object> where = new Dictionary<string, object>();
                where.Add(DAssignment.ProjectId, this.ProjectId);

                Dictionary<string, object> row = new Dictionary<string, object>();
                row.Add(DAssignment.ProjectName, this.ProjectName);
                row.Add(DAssignment.Latitude, 0);
                row.Add(DAssignment.Longitude, 0);
                //row.Add(DImageAtLocation.Latitude, this.Location.Latitude.ToString(App.RegionalCulture));
                //row.Add(DImageAtLocation.Longitude, this.Location.Longitude.ToString(App.RegionalCulture));
                row.Add(DAssignment.TimeCreated, DateTime.Now.ToString(App.RegionalCulture));
                numAffected = App.DB.UpdateValue(DTables.Assignments, where, row);
            }

            // Then remove all folders
            Dictionary<string, object> whereDelete = new Dictionary<string, object>();
            whereDelete.Add(DAssignment.ProjectId, this.ProjectId);
            App.DB.DeleteValue(DTables.Folders, whereDelete);

            // Then add all folders
            foreach(ImageFoldersModel folder in this.ProjectFolders)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                row.Add(DFolders.ProjectId, this.ProjectId);
                row.Add(DFolders.FolderPath, folder.FolderPath);
                row.Add(DFolders.UseSubfolders, folder.IncludeSubfolders);

                App.DB.InsertValue(DTables.Folders, row);
            }

            return numAffected;
        }

        public int Delete()
        {

            //First prompt the user about deletion
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this project?",
                "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.No)
            {
                return 0;
            }

            // First dispose of the current project
            if (App.MapVM != null)
            {
                App.MapVM.CurrentProject = null;
                App.MapVM = null;
                App.MapVM = new ViewModel.MapViewModel();
                GC.Collect();
            }

            // First get an array of all existing paths for this project
            Dictionary<string, object> whereArray = new Dictionary<string, object>();
            whereArray.Add(DImageAtLocation.ProjectId, this.ProjectId);

            DataTable data = App.DB.GetValues(DTables.Images, whereArray, new string[] { "*" });
            
            // Remove all image thumbnail files
            foreach (DataRow row in data.Rows)
            {
                ImageAtLocation img = new ImageAtLocation(row);
                // Delete the thumbnail
                string projectThumbFolder = ImageHelper.GetProjectThumbnailsFolder(this.ProjectId);
                string thumbPath = Path.Combine(projectThumbFolder, img.ThumbnailGuid + ".png");
                if (File.Exists(thumbPath))
                {
                    File.Delete(thumbPath);
                }
            }
            data.Dispose();

            // Remove the directory for the project
            string projectDirectory = ImageHelper.GetProjectThumbnailsFolder(this.ProjectId);
            if (Directory.Exists(projectDirectory))
            {
                Directory.Delete(projectDirectory, true);
            }

            // Delete the thumb base 64 string
            string projectThumbTxt = ImageHelper.GetProjectThumbnailBase64Path(this.ProjectId);
            if (File.Exists(projectThumbTxt))
            {
                File.Delete(projectThumbTxt);
            }

            // Remove all images from the database
            Dictionary<string, object> whereDeleteImages = new Dictionary<string, object>();
            whereDeleteImages.Add(DImageAtLocation.ProjectId, this.ProjectId);
            int numAffectedImages = App.DB.DeleteValue(DTables.Images, whereDeleteImages);

            // Remove all folders from the database
            Dictionary<string, object> whereDeleteFolders = new Dictionary<string, object>();
            whereDeleteFolders.Add(DImageAtLocation.ProjectId, this.ProjectId);
            int numAffectedFolders = App.DB.DeleteValue(DTables.Folders, whereDeleteFolders);

            // Remove the project from the database
            Dictionary<string, object> whereDeleteProject = new Dictionary<string, object>();
            whereDeleteProject.Add(DAssignment.ProjectId, this.ProjectId);
            int numAffectedProjects = App.DB.DeleteValue(DTables.Assignments, whereDeleteProject);
            
            string thumbnailPath = ImageHelper.GetProjectThumbnailBase64Path(this.ProjectId);
            if (File.Exists(thumbnailPath))
            {
                File.Delete(thumbnailPath);
            }
            
            return numAffectedProjects;
        }
    }
}
