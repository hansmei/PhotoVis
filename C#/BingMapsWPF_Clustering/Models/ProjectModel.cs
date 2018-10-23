using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using PhotoVis.Util;
using PhotoVis.Data.DatabaseTables;

namespace PhotoVis.Models
{
    public class ProjectModel : ObservableObject
    {
        #region Fields

        private int _projectId;
        private string _projectName;
        private ObservableCollection<ImageFoldersModel> _folders = new ObservableCollection<ImageFoldersModel>();
        private bool _hasBeenIndexed = false;
        private bool _hasDatabase = false;

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
        }

        public ProjectModel(DataRow row)
        {
            this.ProjectId = int.Parse(row[DAssignment.ProjectId].ToString());
            this.ProjectName = row[DAssignment.ProjectName].ToString();
            this.HasDatabase = true;
            
            //this.TimeCreated = DateTime.Parse(row[DAssignment.TimeCreated].ToString());
            //this.TimeLastIndexed = DateTime.Parse(row[DAssignment.TimeLastIndexed].ToString());
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
            return projects;
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
                row.Add(DAssignment.TimeCreated, DateTime.Now.ToString());
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
                row.Add(DAssignment.TimeCreated, DateTime.Now.ToString());
                App.DB.UpdateValue(DTables.Assignments, where, row);
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
    }
}
