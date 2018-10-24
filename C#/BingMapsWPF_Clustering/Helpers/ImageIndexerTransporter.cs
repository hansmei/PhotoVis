using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotoVis.Models;
using PhotoVis.Data;

namespace PhotoVis.Helpers
{
    public class ImageIndexerTransporter
    {
        public int ProjectId;
        public ImageFoldersModel FolderModel;
        public List<ImageAtLocation> Images;

        public ImageIndexerTransporter (int projectId, ImageFoldersModel folder)
        {
            this.ProjectId = projectId;
            this.FolderModel = folder;
        }

        public void SetImages(List<ImageAtLocation> images)
        {
            this.Images = images;
        }

    }
}
