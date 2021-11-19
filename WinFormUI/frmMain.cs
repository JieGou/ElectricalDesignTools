﻿using System;
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
            UI.GetDataTables();




            UI.OpenChildForm(UI.frmEquipment, pnlChildForm);

        }

        

        private void btnEquipment_Click(object sender, EventArgs e) {
            UI.OpenChildForm(UI.frmEquipment, pnlChildForm);
        }
        private void btnCableSettings_Click(object sender, EventArgs e) {

        }
        private void btnDataTables_Click(object sender, EventArgs e) {
            UI.OpenChildForm(UI.frmDataTables, pnlChildForm);
        }


        private void btnSelectProjectDb_Click(object sender, EventArgs e) {
            UI.SelectProject();
        }
        private void btnSelectLibraryDb_Click(object sender, EventArgs e) {
            UI.SelectLibrary();
        }

        
    }
}
