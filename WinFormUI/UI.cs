using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLiteLibrary;

namespace WinFormUI {
    public static class UI {

        //Forms
        public static Form activeForm;
        public static Form frmEquipment = new frmEquipment();
        public static Form frmDataTables = new frmDataTables();

        //DB Connections
        public static SQLiteDapperConnector prjDb = new SQLiteDapperConnector(Properties.Settings.Default.ProjectDb);
        public static SQLiteDapperConnector libDb = new SQLiteDapperConnector(Properties.Settings.Default.LibraryDb);


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
                }
            }
        }
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
