using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WpfUI.Helpers;

namespace WpfUI.Services
{
    [AddINotifyPropertyChangedInterface]
    public class StartupService
    {
        public string LibraryFile { get; set; }
        public string ProjectFileName { get; set; }
        public string ProjectFilePath { get; set; }

        private string _libraryFile = "Edt Data Library.edl";
        private string _defaultLibrarypath = Path.Combine(Environment.CurrentDirectory, @"ContentFiles\");

        public StartupService(ListManager listManager)
        {
            _listManager = listManager;
            _libraryFile = Path.Combine(Environment.CurrentDirectory, @"ContentFiles\", _libraryFile);
            if (AppSettings.Default.FirstStartup==true) {
                AppSettings.Default.FirstStartup = false;
                AppSettings.Default.LibraryDb = _libraryFile;
                AppSettings.Default.Save();
            }
        }

        ListManager _listManager;
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
            }
        }

        public void InitializeProject(string projectFile)
        {

            try {
                if (File.Exists(projectFile)) {
                    prjDb = new SQLiteConnector(projectFile);
                    DaManager.SetProjectDb(new SQLiteConnector(projectFile));

                    LoadProjectSettings();
                    LoadProjectDb();
                    //LoadProjectSettings();
                }
            }
            catch (Exception ex) {

                ErrorHelper.ShowErrorMessage(ex);
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
                SelectLibrary(_defaultLibrarypath);
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
                SelectLibrary(_defaultLibrarypath);
            }
            else {
                _listManager.GetProjectTablesAndAssigments();

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


        // SETTINGS

        public static void LoadProjectSettings()
        {
            SettingsManager.LoadProjectSettings();
        }
    }
}
