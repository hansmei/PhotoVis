using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotoVis.Models;

namespace PhotoVis.Helpers
{
    public class ProjectSingleFolder
    {
        public int ProjectId;
        public ImageFoldersModel FolderModel;

        public ProjectSingleFolder (int projectId, ImageFoldersModel folder)
        {
            this.ProjectId = projectId;
            this.FolderModel = folder;
        }

    }
}
