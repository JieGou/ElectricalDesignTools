using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using EDTLibrary.ProjectSettings;
using EDTLibrary.TypeTables;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WpfUI.Helpers;

namespace WpfUI.Services
{
    public class DataBaseService
    {

        public static bool IsProjectLoaded { get; set; }
        public static bool IsLibraryLoaded { get; set; }


        //DB Connections

        public static SQLiteConnector prjDb { get; set; }
        public static SQLiteConnector libDb { get; set; }




        public static void InitializeLibrary()
        {
            if (File.Exists(AppSettings.Default.LibraryDb)) {
                libDb = new SQLiteConnector(AppSettings.Default.LibraryDb);
                DbManager.SetLibraryDb(AppSettings.Default.LibraryDb);

                LoadLibraryTables();
                TypeManager.VoltageTypes = libDb.GetRecords<VoltageType>("VoltageTypes");
            }
        }
        public static void InitializeProject()
        {
            if (File.Exists(AppSettings.Default.ProjectDb)) {
                prjDb = new SQLiteConnector(AppSettings.Default.ProjectDb);
                DbManager.SetProjectDb(AppSettings.Default.ProjectDb);

                LoadProjectTables();
                LoadProjectSettings();
            }

        }

        // SELECT
        public static void SelectLibrary()
        {
            string selectedFile = FileSystemHelper.SelectFile();
            if (selectedFile != "") {
                AppSettings.Default.LibraryDb = selectedFile;
                AppSettings.Default.Save();

                InitializeLibrary();
            }
        }
        
        public static void SelectProject()
        {
            string selectedFile = FileSystemHelper.SelectFile();
            if (selectedFile != "") {
                AppSettings.Default.ProjectDb = selectedFile;
                AppSettings.Default.Save();

                InitializeProject();
            }
        }


        // LOAD

        public static void LoadLibraryTables()
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

        public static void LoadProjectTables()
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

                //TODO - Update to List Stores??
                ListManager.DteqList = prjDb.GetRecords<DteqModel>("DistributionEquipment");
                ListManager.LoadList = prjDb.GetRecords<LoadModel>("Loads");
                ListManager.CableList = prjDb.GetRecords<CableModel>("Cables");

                //ListManager.CreateMasterLoadList();

                IsProjectLoaded = true;
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
