using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace streamdeck_foldereditor.Models
{
    internal class PageFolderLocations
    {
        public String FolderProfilePath { get; private set; }

        public String FolderLocation { get; private set; }

        public PageFolderLocations(string folderLocation, string folderProfilePath)
        {
            FolderLocation = folderLocation;
            FolderProfilePath = folderProfilePath;
        }
    }
}
