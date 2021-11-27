using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EDTLibrary;

namespace WinFormUI {
    public partial class frmMain: Form {
        public frmMain() {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            UI.LoadLibraryTables();
            UI.LoadProjectTables();
            ProjectSettings.InitializeSettings();

            UI.OpenChildForm(UI.frmEquipment, pnlChildForm);
        }

        

        private void btnEquipment_Click(object sender, EventArgs e) {
            UI.OpenChildForm(UI.frmEquipment, pnlChildForm);
        }
        private void btnDataTables_Click(object sender, EventArgs e) {
            UI.OpenChildForm(UI.frmDataTables, pnlChildForm);
        }

        private void btnSelectLibraryDb_Click(object sender, EventArgs e) {
            UI.SelectLibrary();
        }

        private void btnSelectProject_Click(object sender, EventArgs e) {
            UI.SelectProject();

        }

        private void btnSettings_Click(object sender, EventArgs e) {
            UI.OpenChildForm(UI.frmSettings, pnlChildForm);
        }
    }
}
