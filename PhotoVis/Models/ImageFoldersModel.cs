using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PhotoVis.Data.DatabaseTables;

namespace PhotoVis.Models
{
    public class ImageFoldersModel : ObservableObject
    {
        #region Fields

        private bool _includeSubfolders;
        private string _folderPath;

        #endregion // Fields

        #region Properties

        public bool IncludeSubfolders
        {
            get { return _includeSubfolders; }
            set
            {
                if (value != _includeSubfolders)
                {
                    _includeSubfolders = value;
                    OnPropertyChanged("IncludeSubfolders");
                }
            }
        }

        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                if (value != _folderPath)
                {
                    _folderPath = value;
                    OnPropertyChanged("FolderPath");
                }
            }
        }

        #endregion // Properties

        public ImageFoldersModel(string folderPath, bool includeSubfolders)
        {
            this.IncludeSubfolders = includeSubfolders;
            this.FolderPath = folderPath;
        }

        public ImageFoldersModel(DataRow row)
        {
            this.FolderPath = row[DFolders.FolderPath].ToString();
            this.IncludeSubfolders = bool.Parse(row[DFolders.UseSubfolders].ToString());
        }
    }
}
