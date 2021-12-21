using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Models;

namespace WpfUI.Stores
{
    public class ProjectFileStore
    {
        private ProjectFile? _selectedProject;
        public ProjectFile SelectedProject
        {
            get { return _selectedProject; }
            set { _selectedProject = value; }
        }
    }

    public class LibraryFileStore
    {
        private LibraryFile? _selectedProject;
        public LibraryFile SelectedProject
        {
            get { return _selectedProject; }
            set { _selectedProject = value; }
        }
    }


}
