using System.IO;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Models;
using WpfUI.Services;
using WpfUI.Stores;

namespace WpfUI.ViewModels
{
    public class StartupViewModel :ViewModelBase
    {
        #region Properties and Backing Fields
        private readonly ProjectFile _projectFile;
        public string? ProjectName { get; set; }
        public string? ProjectPath { get; set; }

        private string _selectedProject;

        private StartupService _startupService;

        #endregion

        public StartupViewModel(StartupService startupService)
        {
            _startupService = startupService;
            SetSelectedProject();
            SelectProjectCommand = new RelayCommand(SelectProject);
        }

        public StartupViewModel(NavigationBarViewModel navigationBarViewModel, ProjectFileStore projectFileStore, NavigationService<ProjectSettingsViewModel> projectSettingsNavigationService)
        {
            SetSelectedProject();

            //string filePath = string.Empty;
            //_projectFile = new ProjectFile(filePath) { FileName = ProjectName, FilePath = ProjectPath, };

            NavigationBarViewModel = navigationBarViewModel;

            OpenProjectCommand = new OpenProjectCommand(this, projectFileStore, projectSettingsNavigationService);

            SelectProjectCommand = new RelayCommand(SelectProject);
        }

       

        public ICommand OpenProjectCommand { get; }
        public ICommand SelectProjectCommand { get; }
        public NavigationBarViewModel NavigationBarViewModel { get; }

        public void SelectProject()
        {
            string rootPath = Path.GetDirectoryName(AppSettings.Default.ProjectDb);
            _startupService.SelectProject(rootPath);
            SetSelectedProject();            
        }

        public void SetSelectedProject()
        {
            _selectedProject = AppSettings.Default.ProjectDb; 
            //ProjectName = Path.GetFileNameWithoutExtension(_selectedProject);
            ProjectName = Path.GetFileName(_selectedProject);
            ProjectPath = Path.GetDirectoryName(_selectedProject);

        }
    }
}
