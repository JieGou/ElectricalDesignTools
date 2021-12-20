using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Helpers;
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

        #endregion

        public StartupViewModel()
        {
            SetSelectedProject();
            SelectProjectCommand = new RelayCommand(SelectProject);
        }

        public StartupViewModel(NavigationBarViewModel navigationBarViewModel, ProjectFileStore projectFileStore, NavigationService<ProjectSettingsViewModel> projectSettingsNavigationService)
        {
            SetSelectedProject();
            _projectFile = new ProjectFile() { Name = ProjectName, Path = ProjectPath, };

            NavigationBarViewModel = navigationBarViewModel;

            OpenProjectCommand = new OpenProjectCommand(this, projectFileStore, projectSettingsNavigationService);

            SelectProjectCommand = new RelayCommand(SelectProject);
        }

       

        public ICommand OpenProjectCommand { get; }
        public ICommand SelectProjectCommand { get; }
        public NavigationBarViewModel NavigationBarViewModel { get; }

        public void SelectProject()
        {
            DataBaseService.SelectProject();
            SetSelectedProject();            
        }

        public void SetSelectedProject()
        {
            _selectedProject = AppSettings.Default.ProjectDb; 
            ProjectName = Path.GetFileName(_selectedProject).Replace(".db", "");
            ProjectPath = Path.GetFullPath(_selectedProject).Replace(Path.GetFileName(_selectedProject), "");
            
        }
    }
}
