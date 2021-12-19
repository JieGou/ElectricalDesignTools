using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WpfUI.HelpMethods;

namespace WpfUI.Services
{
    public class DataBaseService
    {

        public static bool ProjectLoaded { get; set; }
        public static bool LibraryLoaded { get; set; }


        //DB Connections

        public static SQLiteConnector prjDb { get; set; }
        public static SQLiteConnector libDb { get; set; }




        public static void Initialize()
        {
            prjDb = new SQLiteConnector(Settings.Default.ProjectDb);
            libDb = new SQLiteConnector(Settings.Default.LibraryDb);

            LoadLibraryTables();
            LoadProjectTables();
            LoadProjectSettings();
        }

        //Library
        public static void SelectLibrary()
        {
            string selectedFile = FileSystemHelper.SelectFile();
            if (selectedFile != "") {
                Settings.Default.LibraryDb = selectedFile;
                Settings.Default.Save();

                LoadLibraryTables();
            }
        }
        public static void LoadLibraryTables()
        {
            Type libTablesClass = typeof(LibraryTables); // MyClass is static class with static properties
            DataTable dt = new DataTable();

            string dbFilename = Settings.Default.LibraryDb;
            if (File.Exists(dbFilename)) {
                foreach (var prop in libTablesClass.GetProperties()) {
                    prop.SetValue(dt, libDb.GetDataTable(prop.Name));
                }
                LibraryLoaded = true;
            }
            else {
                MessageBox.Show($"The selected file \n\n{dbFilename} cannot be found. Please select another Library file.");
                LibraryLoaded = false;
            }
        }

        //Project
        public static void SelectProject()
        {
            string selectedFile = FileSystemHelper.SelectFile();
            if (selectedFile != "") {
                Settings.Default.ProjectDb = selectedFile;
                Settings.Default.Save();

                prjDb = new SQLiteConnector(Settings.Default.ProjectDb);
                LoadProjectSettings();
                LoadProjectTables();
            }
        }

        public static void LoadProjectTables()
        {
            string dbFilename = Settings.Default.ProjectDb;
            if (File.Exists(dbFilename) == false) {
                MessageBox.Show($"The selected file \n\n{dbFilename} cannot be found. Please select another project file.");
                ProjectLoaded = false;
               
            }
            else if (LibraryLoaded ==false) {
                MessageBox.Show($"The library file is not loaded.");
                ProjectLoaded = false;
            }
            else {

                //prjDb = new SQLiteConnector(dbFilename);

                //TODO - Update to List Stores??
                ListManager.dteqList = prjDb.GetRecords<DteqModel>("DistributionEquipment");
                ListManager.loadList = prjDb.GetRecords<LoadModel>("Loads");
                ListManager.cableList = prjDb.GetRecords<CableModel>("Cables");
                ListManager.CreateMasterLoadList();
                ListManager.AssignLoadsToDteq();

                ProjectLoaded = true;
            }
        }

        //Settings

        //TODO update load project settings to new style
        public static void LoadProjectSettings()
        {
            string dbFilename = Settings.Default.ProjectDb;
            if (File.Exists(dbFilename)) {
                //prjDb = new SQLiteConnector(dbFilename);

                DataTable settings = prjDb.GetDataTable("ProjectSettings");
                Type prjSettings = typeof(EDTLibrary.ProjectSettings.Settings);
                string propValue = "";
                for (int i = 0; i < settings.Rows.Count; i++) {
                    foreach (var prop in prjSettings.GetProperties()) {
                        if (settings.Rows[i]["Name"].ToString() == prop.Name && settings.Rows[i]["Type"].ToString() != "DataTable") {
                            prop.SetValue(propValue, settings.Rows[i]["Value"].ToString());
                            //MessageBox.Show(prop.Name + ": " + prop.GetValue(propValue).ToString());
                        }
                    }
                }

                //DataTables
                EDTLibrary.ProjectSettings.Settings.CableSizesUsedInProject = prjDb.GetDataTable("CablesUsedInProject");
            }
        }

        public static void SaveProjectSettings()
        {
            Type type = typeof(EDTLibrary.ProjectSettings.Settings); // ProjectSettings is a static class
            string propValue;
            foreach (var prop in type.GetProperties()) {
                propValue = prop.GetValue(null).ToString(); //null for static class
                prjDb.UpdateSetting(prop.Name, propValue);
            }
        }            
    }
}
