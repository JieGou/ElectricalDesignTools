using EDTLibrary.Managers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Services;
using WpfUI.Stores;
using WpfUI.ViewModels.Home;
using WpfUI.Windows;

namespace WpfUI.ViewModels
{
    public class HomeViewModel :ViewModelBase
    {
        #region Properties and Backing Fields
        

        private string _selectedProject;
        private readonly MainViewModel _mainViewModel;
        private StartupService _startupService;
        public StartupService StartupService
        {
            get { return _startupService; }
            set { _startupService = value; }
        }

        private readonly ListManager _listManager;

        #endregion
        public ICommand NewProjectCommand { get; }
        public ICommand SelectProjectCommand { get; }
        public Window NewProjectWindow {get; set;}

        public ObservableCollection<PreviousProject> PreviousProjects { get; set; } = new ObservableCollection<PreviousProject>();

        public HomeViewModel(MainViewModel mainViewModel, StartupService startupService, ListManager listManager)
        {
            _mainViewModel = mainViewModel;
            _startupService = startupService;
            _listManager = listManager;
            _startupService.SetSelectedProject(AppSettings.Default.ProjectDb);
            PreviousProjects = _startupService.PreviousProjects;
            NewProjectCommand = new RelayCommand(NewProject);
            SelectProjectCommand = new RelayCommand(SelectProject);


        }

        private void NewProject()
        {
            
            NewProjectWindow = new NewProjectWindow();
            NewProjectViewModel newProjectVm = new NewProjectViewModel(
                _mainViewModel,
                new EDTLibrary.LibraryData.TypeManager(),
                new StartupService(_listManager, PreviousProjects),
                this);

            NewProjectWindow.DataContext = newProjectVm;
            NewProjectWindow.ShowDialog();

        }

        public void SelectProject()
        {
            string? rootPath = Path.GetDirectoryName(AppSettings.Default.ProjectDb);
            _startupService.SelectProject(rootPath); //Initializes project as well
            StartupService.SetSelectedProject(AppSettings.Default.ProjectDb);            
        }
    }
}
