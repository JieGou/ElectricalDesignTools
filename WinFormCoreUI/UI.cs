using EDTLibrary;
using EDTLibrary.Models;
using EDTLibrary.DataAccess;
using LM = EDTLibrary.ListManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;

namespace WinFormCoreUI {
    public static class UI {
        public static bool ProjectLoaded { get; set; }
        public static bool LibraryLoaded { get; set; }


        //DB Connections and Db Files
        public static string ProjectFile { get; set; }
        public static string LibraryFile { get; set; }

        public static SQLiteConnector prjDb { get; set; }
        public static SQLiteConnector libDb { get; set; }

        //public static string ProjectFile = Properties.Settings.Default.ProjectDb;
        //public static string LibraryFile = Properties.Settings.Default.LibraryDb;
        //public static SQLiteConnector prjDb = new SQLiteConnector(ProjectFile);
        //public static SQLiteConnector libDb = new SQLiteConnector(LibraryFile);


        

        

        public static void Initialize() {
            ProjectFile = Properties.Settings.Default.ProjectDb;
            //LibraryFile = Properties.Settings.Default.LibraryDb;

#if (DEBUG)
            LibraryFile = Properties.Settings.Default.LibraryDb;
            //LibraryFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\ContentFiles\\EDTDataLibrary.db";
#else
            LibraryFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\ContentFiles\\EDTDataLibrary.db";
            LibraryFile = Properties.Settings.Default.LibraryDb;
#endif
            prjDb = new SQLiteConnector(ProjectFile);
            libDb = new SQLiteConnector(LibraryFile);
        }



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
                    ProjectFile = filePath;
                    Properties.Settings.Default.Save();
                    LoadProjectTables();
                    LoadProjectSettings();
                }
            }

        }
        public static void LoadProjectTables() {
            string dbFilename = ProjectFile;
            if (File.Exists(dbFilename)) {
                prjDb = new SQLiteConnector(dbFilename);

                LM.dteqList = prjDb.GetRecords<DteqModel>("DistributionEquipment");
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

            //Settings
            public static void LoadProjectSettings() {
                DataTable settings = UI.prjDb.GetDataTable("ProjectSettings");
                Type prjSettings = typeof(EDTLibrary.ProjectSettings.Settings);
                string propValue = "";
                for (int i = 0; i < settings.Rows.Count; i++) {
                    foreach (var prop in prjSettings.GetProperties()) {
                        if (settings.Rows[i]["Name"].ToString() == prop.Name) {
                            prop.SetValue(propValue, settings.Rows[i]["Value"].ToString());
                            //MessageBox.Show(prop.Name + ": " + prop.GetValue(propValue).ToString());
                        }
                    }
                }
            EDTLibrary.ProjectSettings.Settings.CableSizesUsedInProject = UI.prjDb.GetDataTable("CablesUsedInProject");
            }
            public static void SaveProjectSettings() {
                Type type = typeof(EDTLibrary.ProjectSettings.Settings); // ProjectSettings is a static class
                string propValue;
                foreach (var prop in type.GetProperties()) {
                    propValue = prop.GetValue(null).ToString(); //null for static class
                    UI.prjDb.UpdateSetting(prop.Name, propValue);
                }
            }

            public static string GetStringSetting(string settingName) {
                string settingValue = "";
                Type type = typeof(EDTLibrary.ProjectSettings.Settings); // MyClass is static class with static properties
                foreach (var prop in type.GetProperties()) {
                    if (settingName == prop.Name) {
                        settingValue = prop.GetValue(null).ToString();
                    }
                }
                return settingValue;
            }

            public static void SaveStringSetting(string settingName, string settingValue) {
                Type type = typeof(EDTLibrary.ProjectSettings.Settings); // MyClass is static class with static properties
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
                    LibraryFile = filePath;
                    Properties.Settings.Default.Save();
                }
            }
            LoadLibraryTables();
        }
        public static void LoadLibraryTables() {
            Type dataTablesClass = typeof(LibraryTables); // MyClass is static class with static properties
            DataTable dt = new DataTable();

            string dbFilename = LibraryFile;
            if (File.Exists(dbFilename)) {
                foreach (var prop in dataTablesClass.GetProperties()) {
                    prop.SetValue(dt, UI.libDb.GetDataTable(prop.Name));
                }
                LibraryLoaded = true;
            }
            else {
                MessageBox.Show($"The selected file \n\n{dbFilename} cannot be found. Please select another Library file.");
                LibraryLoaded = false;
            }
        }
                
        public static void ScaleDataGridView(DataGridView dataGridView) {
            double totalWidth = 0;
            foreach (DataGridViewColumn col in dataGridView.Columns) {
                if (col.Visible == true) {
                    totalWidth += col.Width;
                }
            }
            double totalHeight = 0;
            foreach (DataGridViewRow row in dataGridView.Rows) {
                totalHeight += row.Height;
            }
            totalHeight += dataGridView.ColumnHeadersHeight;
            dataGridView.Width = (int)totalWidth + 15;
            dataGridView.Height = (int)totalHeight + 15;


            dataGridView.Columns["Id"].Visible = false;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        }


        //Forms
        public static Form activeForm;
        public static Form frmEquipment = new frmEquipment();
        public static Form frmSettings = new frmSettings();
        public static Form frmDataTables = new frmDataTables();
        public static Form frmLoads = new frmLoads();
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
