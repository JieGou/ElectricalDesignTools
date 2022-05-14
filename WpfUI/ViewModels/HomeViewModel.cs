using EDTLibrary;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Services;
using WpfUI.Stores;
using WpfUI.Windows;

namespace WpfUI.ViewModels
{
    public class HomeViewModel :ViewModelBase
    {
        #region Properties and Backing Fields
        public string? ProjectName { get; set; }
        public string? ProjectPath { get; set; }


        private string _selectedProject;

        private StartupService _startupService;
        private readonly ListManager _listManager;

        #endregion
        public ICommand NewProjectCommand { get; }
        public ICommand SelectProjectCommand { get; }
        public HomeViewModel(StartupService startupService, ListManager listManager)
        {
            _startupService = startupService;
            _listManager = listManager;
            SetSelectedProject(AppSettings.Default.ProjectDb);
            NewProjectCommand = new RelayCommand(NewProject);
            SelectProjectCommand = new RelayCommand(SelectProject);
        }

        private void NewProject()
        {
            Window newProjectWindow = new NewProjectWindow();

            NewProjectViewModel newProjectVm = new NewProjectViewModel(
                new EDTLibrary.LibraryData.TypeTables.TypeManager(),
                new StartupService(_listManager),
                this);

            newProjectWindow.DataContext = newProjectVm;
            newProjectWindow.Show();

        }



        public void SelectProject()
        {
            string? rootPath = Path.GetDirectoryName(AppSettings.Default.ProjectDb);
            _startupService.SelectProject(rootPath);
            SetSelectedProject(AppSettings.Default.ProjectDb);            
        }

        public void SetSelectedProject(string selectedProject)
        {
            //ProjectName = Path.GetFileNameWithoutExtension(_selectedProject);
            AppSettings.Default.ProjectDb = selectedProject;
            AppSettings.Default.Save();
            ProjectName = Path.GetFileName(selectedProject);
            ProjectPath = Path.GetDirectoryName(selectedProject);


        }
    }
}
