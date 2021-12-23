using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.Models
{
    public class ProjectFile
    {
        public ProjectFile(string? fullPath)
        {
            FullPath = fullPath;
            FileName = Path.GetFileName(fullPath);
            FilePath = Path.GetFullPath(fullPath);
        }

        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? FullPath { get; set; }
        public bool IsLoaded { get; set; } = false;

    }
}