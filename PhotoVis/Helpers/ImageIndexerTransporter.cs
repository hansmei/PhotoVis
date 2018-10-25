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
        public string FolderPath;
        public List<ImageAtLocation> Images;

        public ImageIndexerTransporter (int projectId, string folder)
        {
            this.ProjectId = projectId;
            this.FolderPath = folder;
        }

        public void SetImages(List<ImageAtLocation> images)
        {
            this.Images = images;
        }

    }
}
