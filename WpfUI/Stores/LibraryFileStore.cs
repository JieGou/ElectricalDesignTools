using WpfUI.Models;

namespace WpfUI.Stores
{
    public class LibraryFileStore
    {
        private ProjectFile? _selectedLibrary;
        public ProjectFile SelectedLibrary
        {
            get { return _selectedLibrary; }
            set { _selectedLibrary = value; }
        }
    }
}
