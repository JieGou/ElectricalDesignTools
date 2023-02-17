using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfUI.Helpers;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Home;

namespace WpfUI.Services
{
    [AddINotifyPropertyChangedInterface]
    public class StartupService
    {
        public string LibraryFile { get; set; }
        public string ProjectFileName { get; set; }
        public string ProjectFilePath { get; set; }
        public MainViewModel MainVm { get; set; }
        public ObservableCollection<PreviousProject> PreviousProjects { get; set; }


        Environment.SpecialFolder _appDataFolder = Environment.SpecialFolder.ApplicationData;
        private string _edtFolder = @"\Electrical Design Tools\";

        private string _libraryFile = "Edt Data Library.edl";
        private string _projectFile = "Edt Sample Project.edp";
        
        public StartupService(ListManager listManager, ObservableCollection<PreviousProject> previousProjects)
        {
            ListManager = listManager;
            PreviousProjects = previousProjects;


            //_libraryFile = Environment.GetFolderPath(_appDataFolder) + _edtFolder + _libraryFile;
            //_projectFile = Environment.GetFolderPath(_appDataFolder) + _edtFolder + _projectFile;
            //AppSettings.Default.LibraryDb = _libraryFile;
            //AppSettings.Default.ProjectDb = _projectFile;
            //AppSettings.Default.Save();

            _libraryFile = Path.Combine(Environment.CurrentDirectory, @"ContentFiles\", _libraryFile);
            _projectFile = Path.Combine(Environment.CurrentDirectory, @"ContentFiles\", _projectFile);

            if (AppSettings.Default.FirstStartup == true) {
                AppSettings.Default.FirstStartup = false;
                AppSettings.Default.LibraryDb = _libraryFile;
                AppSettings.Default.ProjectDb = _projectFile;
                AppSettings.Default.NewProjectFileTemplate = _projectFile;
                AppSettings.Default.Save();
            }

        }

        public ListManager ListManager { get; set; }

        public bool IsProjectLoaded { get; set; }
        public bool IsLibraryLoaded { get; set; }


        //DB Connections

        public static SQLiteConnector prjDb { get; set; }
        public static SQLiteConnector libDb { get; set; }

        public void InitializeLibrary()
        {
            _libraryFile = AppSettings.Default.LibraryDb;

            if (File.Exists(_libraryFile)) {
                libDb = new SQLiteConnector(_libraryFile);
                DaManager.SetLibraryDb(new SQLiteConnector(_libraryFile));

                LoadLibraryDb();
                TypeManager.VoltageTypes = libDb.GetRecords<VoltageType>("VoltageTypes");
                LibraryFile = _libraryFile;
                
            }
        }

        public void SetSelectedProject(string selectedProject)
        {
            ProjectFileName = string.Empty;
            ProjectFilePath = string.Empty;

            if (File.Exists(selectedProject)) {
                //ProjectName = Path.GetFileNameWithoutExtension(_selectedProject);
                AppSettings.Default.ProjectDb = selectedProject;
                AppSettings.Default.Save();
                ProjectFileName = Path.GetFileName(selectedProject);
                ProjectFilePath = Path.GetDirectoryName(selectedProject);
                UpdatePreviousProjectsList(selectedProject);
            }
        }

        private void UpdatePreviousProjectsList(string selectedProject)
        {
            var tempList = new ObservableCollection<PreviousProject>();

            foreach (var previousProject in PreviousProjects) {
                tempList.Add(previousProject);
                if (selectedProject == previousProject.Project) {
                    tempList.Remove(previousProject);
                }               
            }
            PreviousProjects.Clear();
            foreach (var item in tempList) {
                PreviousProjects.Add(item);
            }
            PreviousProjects.Insert(0, new PreviousProject(this, selectedProject));
        }


        public void InitializeProject(string projectFile)
        {
            ListManager = new ListManager();
            ScenarioManager.ListManager = ListManager;

            if (MainVm != null) {
                MainVm.ListManager = ListManager;
                MainVm.InitializeViewModels(); 
            }

            try {
                if (File.Exists(projectFile)) {
                    prjDb = new SQLiteConnector(projectFile);
                    DaManager.SetProjectDb(new SQLiteConnector(projectFile));

                    SettingsManager.LoadProjectSettings();
                    TagManager.LoadTagSettings();
                    LoadProjectDb();
                }
                else {
                    MessageBox.Show("Selected project not found. The file may have been moved or renamed",
                        "File Not Found", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Exclamation);
                    var project = PreviousProjects.FirstOrDefault(p => p.Project == projectFile);
                    PreviousProjects.Remove(project);
                }
            }
            catch (Exception ex) {

                IsProjectLoaded = true;
                NotificationHandler.ShowErrorMessage(ex);
            }
        }

        // SELECT
        public void SelectLibrary(string rootPath)
        {
            string selectedFile = FileSystemHelper.SelectFilePath(rootPath, "EDT files (*.edl)|*.edl");
            if (selectedFile != "") {
                AppSettings.Default.LibraryDb = selectedFile;
                AppSettings.Default.Save();

                InitializeLibrary();
            }
        }
        
        public void SelectProject(string rootPath)
        {
            string selectedFile = FileSystemHelper.SelectFilePath(rootPath, "EDT files (*.edp)|*.edp");
            if (selectedFile != "") {
                AppSettings.Default.ProjectDb = selectedFile;
                AppSettings.Default.Save();

                InitializeProject(selectedFile);
            }
        }
       
        // LOAD
        public void LoadLibraryDb()
        {
            string dbFilename = AppSettings.Default.LibraryDb;
            if (File.Exists(dbFilename)) {
                IsLibraryLoaded = DaManager.GetLibraryTables();
            }
            else {
                MessageBox.Show($"The selected Library file \n\n{dbFilename} cannot be found,it may have been moved or deleted. Please select another Library file.");
                IsLibraryLoaded = false;
                SelectLibrary(Environment.GetFolderPath(_appDataFolder) + _edtFolder + _libraryFile);
            }
        }

        public void LoadProjectDb()
        {
            string dbFilename = AppSettings.Default.ProjectDb;
            if (File.Exists(dbFilename) == false) {
                MessageBox.Show($"The selected Project file \n\n{dbFilename} cannot be found, it may have been moved or deleted. Please select another Project file.");
                IsProjectLoaded = false;
            }
            else if (IsLibraryLoaded == false) {
                MessageBox.Show($"The library file is not loaded.");
                IsProjectLoaded = false;
                SelectLibrary(Environment.GetFolderPath(_appDataFolder) + _edtFolder + _libraryFile);
            }
            else {
                ListManager.GetProjectTablesAndAssigments();

                IsProjectLoaded = true;
                OnProjectLoaded();
                CommandManager.InvalidateRequerySuggested();  //Fires CanExecuteChanged in Relay Commands (ICommand);
            }
        }

        public event EventHandler ProjectLoaded;
        public virtual void OnProjectLoaded()
        {
            if (ProjectLoaded != null) {
                ProjectLoaded(this, EventArgs.Empty);
            }
        }

    }
}
