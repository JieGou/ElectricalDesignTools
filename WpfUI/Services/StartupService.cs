using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfUI.Helpers;
using WpfUI.ViewModels;

namespace WpfUI.Services
{
    public class StartupService
    {
        public StartupService(ListManager listManager)
        {
            _listManager = listManager;
        }

        ListManager _listManager;
        public bool IsProjectLoaded { get; set; }
        public bool IsLibraryLoaded { get; set; }


        //DB Connections

        public static SQLiteConnector prjDb { get; set; }
        public static SQLiteConnector libDb { get; set; }

        public void InitializeLibrary()
        {
            if (File.Exists(AppSettings.Default.LibraryDb)) {
                libDb = new SQLiteConnector(AppSettings.Default.LibraryDb);
                DaManager.SetLibraryDb(new SQLiteConnector(AppSettings.Default.LibraryDb));

                LoadLibraryDb();
                TypeManager.VoltageTypes = libDb.GetRecords<VoltageType>("VoltageTypes");
            }
        }

        public void InitializeProject(string projectFile)
        {
            if (File.Exists(projectFile)) {
                prjDb = new SQLiteConnector(projectFile);
                DaManager.SetProjectDb(new SQLiteConnector(projectFile));

                LoadProjectDb();
                LoadProjectSettings();
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
            }
            else {
                _listManager.GetProjectTablesAndAssigments();

                IsProjectLoaded = true;
                CommandManager.InvalidateRequerySuggested();  //Fires CanExecuteChanged in Relay Commands (ICommand);
            }
        }

       

        // SETTINGS

        public static void LoadProjectSettings()
        {
            SettingManager.LoadProjectSettings();
        }

        public static void SaveProjectSettings()
        {
            Type type = typeof(EdtSettings); // EdtSettings is a static class
            string propValue;
            try {
                foreach (var prop in type.GetProperties()) {
                    propValue = prop.GetValue(null).ToString(); //null for static class
                    prjDb.UpdateSetting(prop.Name, propValue);
                }
            }
            catch (Exception ex) {
                ErrorHelper.SqlErrorMessage(ex);
            }
        }

        
    }
}
