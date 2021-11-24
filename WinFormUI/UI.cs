using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EDTLibrary;
using EDTLibrary.Models;
using LM = EDTLibrary.ListManager;
using SQLiteLibrary;

namespace WinFormUI {
    public static class UI {

        public static bool ProjectLoaded { get; set; }
        public static bool LibraryLoaded { get; set; }


        //Forms
        public static Form activeForm;
        public static Form frmEquipment = new frmEquipment();
        public static Form frmSettings = new frmSettings();
        public static Form frmDataTables = new frmDataTables();

        //DB Connections
        public static SQLiteDapperConnector prjDb = new SQLiteDapperConnector(Properties.Settings.Default.ProjectDb);
        public static SQLiteDapperConnector libDb = new SQLiteDapperConnector(Properties.Settings.Default.LibraryDb);

        //Project
        public static void SelectProject() {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    Properties.Settings.Default.ProjectDb = filePath;
                    Properties.Settings.Default.Save();
                    LoadProjectTables();
                    LoadProjectSettings();
                }
            }
            
        }
        public static void LoadProjectTables() {
            string dbFilename = Properties.Settings.Default.ProjectDb;
            if (File.Exists(dbFilename)) {
                prjDb = new SQLiteDapperConnector(dbFilename);

                LM.dteqList = prjDb.GetRecords<DistributionEquipmentModel>("DistributionEquipment");
                LM.CreateDteqDict();
                LM.AssignLoadsToDteq();
                LM.loadList = prjDb.GetRecords<LoadModel>("Loads");
                LM.CreateMasterLoadList();
                LM.cableList = prjDb.GetRecords<CableModel>("Cables");

                LoadProjectSettings();


                ProjectLoaded = true;
            }
            else {
                MessageBox.Show($"The selected file \n\n{dbFilename} cannot be found. Please select another project file.");
                ProjectLoaded = false;
            }
        }
        public static void LoadProjectSettings() {
            DataTable settings = UI.prjDb.GetDataTable("ProjectSettings");
            Type prjSettings = typeof(ProjectSettings);
            string propValue = "";
            for (int i = 0; i < settings.Rows.Count; i++) {
                foreach (var prop in prjSettings.GetProperties()) {
                    if (settings.Rows[i]["Name"].ToString() == prop.Name ) {
                        prop.SetValue(propValue, settings.Rows[i]["Value"].ToString());
                        //MessageBox.Show(prop.Name + ": " + prop.GetValue(propValue).ToString());
                    }
                }
            }
            ProjectSettings.CablesUsedInProject = UI.prjDb.GetDataTable("CablesUsedInProject");
        }
        public static void SaveProjectSettings() {
            Type type = typeof(ProjectSettings); // ProjectSettings is a static class
            string propValue;
            foreach (var prop in type.GetProperties()) {
                propValue = prop.GetValue(null).ToString(); //null for static class
                UI.prjDb.UpdateSetting(prop.Name, propValue);
            }

        }
        public static string GetSetting(string settingName) {
            string settingValue="";
            Type type = typeof(ProjectSettings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (settingName == prop.Name) {
                    settingValue = prop.GetValue(null).ToString();
                }
            }
            return settingValue;
        }
        public static void SaveSetting(string settingName, string settingValue) {
            Type type = typeof(ProjectSettings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (settingName == prop.Name) {
                    prop.SetValue(settingValue, settingValue);
                }
            }
        }

        //Library
        public static void SelectLibrary() {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    Properties.Settings.Default.LibraryDb = filePath;
                    Properties.Settings.Default.Save();
                }
            }
            LoadLibraryTables();
        }
        public static void LoadLibraryTables() {
            Type type = typeof(DataTables); // MyClass is static class with static properties
            DataTable dt = new DataTable();

            string dbFilename = Properties.Settings.Default.ProjectDb;
            if (File.Exists(dbFilename)) {
                foreach (var prop in type.GetProperties()) {
                prop.SetValue(dt, UI.libDb.GetDataTable(prop.Name));
                }
                LibraryLoaded = true;
            }
            else {
                MessageBox.Show($"The selected file \n\n{dbFilename} cannot be found. Please select another project file.");
                LibraryLoaded = false;
            }
        }

        public static void OpenChildForm(Form childForm, Panel pnlChildForm) {
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pnlChildForm.Controls.Add(childForm);
            pnlChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            childForm.Activate();
        }
    }
}
