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
        private static ListManager _listManager;
        public StartupService(ListManager listManager)
        {
            _listManager = listManager;
        }

        public bool IsProjectLoaded { get; set; }
        public bool IsLibraryLoaded { get; set; }


        //DB Connections

        public static SQLiteConnector prjDb { get; set; }
        public static SQLiteConnector libDb { get; set; }




        public void InitializeLibrary()
        {
            if (File.Exists(AppSettings.Default.LibraryDb)) {
                libDb = new SQLiteConnector(AppSettings.Default.LibraryDb);
                DbManager.SetLibraryDb(AppSettings.Default.LibraryDb);

                LoadLibraryTables();
                TypeManager.VoltageTypes = libDb.GetRecords<VoltageType>("VoltageTypes");
            }
        }

        public void InitializeProject()
        {
            if (File.Exists(AppSettings.Default.ProjectDb)) {
                prjDb = new SQLiteConnector(AppSettings.Default.ProjectDb);
                DbManager.SetProjectDb(AppSettings.Default.ProjectDb);

                LoadProjectTables();
                LoadProjectSettings();
            }
        }

        // SELECT
        public void SelectLibrary(string rootPath)
        {
            string selectedFile = FileSystemHelper.SelectFilePath(rootPath, "EDT files (*.edl)|*.edl|All files (*.*)|*.*");
            if (selectedFile != "") {
                AppSettings.Default.LibraryDb = selectedFile;
                AppSettings.Default.Save();

                InitializeLibrary();
            }
        }
        
        public void SelectProject(string rootPath)
        {
            string selectedFile = FileSystemHelper.SelectFilePath(rootPath, "EDT files (*.edp)|*.edp|All files (*.*)|*.*");
            if (selectedFile != "") {
                AppSettings.Default.ProjectDb = selectedFile;
                AppSettings.Default.Save();

                InitializeProject();
            }
        }


        // LOAD

        public void LoadLibraryTables()
        {
            string dbFilename = AppSettings.Default.LibraryDb;
            if (File.Exists(dbFilename)) {
                IsLibraryLoaded = DbManager.GetLibraryTables();
            }
            else {
                MessageBox.Show($"The selected Library file \n\n{dbFilename} cannot be found,it may have been moved or deleted. Please select another Library file.");
                IsLibraryLoaded = false;
            }
        }

        public void LoadProjectTables()
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

                GlobalConfig.GettingRecords = true;

                    _listManager.GetAreas();
                    _listManager.GetDteq();
                    _listManager.GetLoads();
                    _listManager.AssignLoadsToAllDteq();

                    //Cables
                    _listManager.GetCables();
                    _listManager.AssignCables();

                GlobalConfig.GettingRecords = false;


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
            Type type = typeof(EdtSettings); // ProjectSettings is a static class
            string propValue;
            foreach (var prop in type.GetProperties()) {
                propValue = prop.GetValue(null).ToString(); //null for static class
                prjDb.UpdateSetting(prop.Name, propValue);
            }
        }            
    }
}
