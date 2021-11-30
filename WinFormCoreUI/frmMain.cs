﻿using EDTLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormCoreUI {
    public partial class frmMain : Form {
        public frmMain() {
            InitializeComponent();
        }

        public bool Loaded { get; set; }

        private void frmMain_Load(object sender, EventArgs e) {

            this.WindowState = FormWindowState.Maximized;
            UI.LoadLibraryTables();
            UI.LoadProjectTables();
            StringSettings.InitializeSettings();
            if (UI.ProjectLoaded && UI.LibraryLoaded) {
                Loaded = true;


                //UI.OpenChildForm(UI.frmEquipment, pnlChildForm);
            }
        }

        private void btnSettings_Click(object sender, EventArgs e) {
            UI.OpenChildForm(UI.frmSettings, pnlChildForm);
        }

        private void btnEquipment_Click(object sender, EventArgs e) {
            UI.OpenChildForm(UI.frmEquipment, pnlChildForm);
        }

        private void btnSelectProject_Click(object sender, EventArgs e) {
            UI.SelectProject();
        }

        private void btnMainTest_Click(object sender, EventArgs e) {

        }

        private void btnSelectLibrary_Click(object sender, EventArgs e) {
            UI.SelectLibrary();
        }
    }
}
