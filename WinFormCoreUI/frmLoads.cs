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
using EDTLibrary.Models;

namespace WinFormCoreUI {
    public partial class frmLoads : Form {
        public frmLoads() {
            InitializeComponent();
        }

        bool loadsLoaded = false;
        private void frmLoads_Load(object sender, EventArgs e) {
            string[] loadTypes = Enum.GetNames(typeof(LoadTypes));
            cbxLoadTypeToAdd.Items.AddRange(loadTypes);
            cbxLoadTypeToAdd.SelectedItem = loadTypes[0];
            dgvMain.DataSource = ListManager.loadList;
            HideFooter();
            loadsLoaded = true;
            ExtensionMethods.DoubleBufferDataGridViews(this);

            //foreach (DataGridViewColumn col in dgvMain.Columns) {
            //    col.HeaderCell.Style.Padding = new Padding(5, 0, 0, 0);
            //}
        }

        bool editor = false;
        private void btnToggleEditor_Click(object sender, EventArgs e) {
            if (editor) {
                HideFooter();
            }
            else {
                ShowFooter();
            }
        }
        private void ShowFooter() {
            pnlFooter.Height = 400;
            dgvEditor.Visible = true;
            editor = true;
        }
        private void HideFooter() {
            pnlFooter.Height = 50;
            dgvEditor.Visible = true;
            editor = false;
        }


        private void AddLoad(string loadType) {

            List<LoadModel> editLoad = new List<LoadModel>();
            editLoad.Add(new LoadModel {
                Type = loadType,
                Size = 5,
                //Unit = "HP",
                LoadFactor = 0.8,
                Voltage = double.Parse(EDTLibrary.ProjectSettings.EdtSettings.VoltageDefaultLV)
            });
            dgvEditor.DataSource = editLoad;
        }

        private void EditLoad(LoadModel load) {

            List<LoadModel> editLoad = new List<LoadModel>();
            editLoad.Add(load);
            dgvEditor.DataSource = editLoad;
        }


        private void cbxLoadTypeToAdd_SelectedIndexChanged(object sender, EventArgs e) {
            ShowFooter();
            if (cbxLoadTypeToAdd.SelectedItem != null) {
                AddLoad(cbxLoadTypeToAdd.SelectedItem.ToString());
            }
        }

        private void btnLoadList_Click(object sender, EventArgs e) {
        }
               

        private void dgvMain_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (loadsLoaded) {
                LoadModel load = dgvMain.SelectedRows[0].DataBoundItem as LoadModel;
                EditLoad(load);
            }
            //foreach (DataGridViewColumn col in dgvMain.Columns) {
            //    col.HeaderCell.Style.Padding = new Padding(15, 0, 0, 0);
            //}
        }

        private void dgvEditor_Validating(object sender, CancelEventArgs e) {

        }

        private void dgvMain_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            ShowFooter();
        }
    }
}