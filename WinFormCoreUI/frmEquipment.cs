using EDTLibrary;
using EDTLibrary.Models;
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
    public partial class frmEquipment : Form {
        public frmEquipment() {
            InitializeComponent();
        }

        private void frmEquipment_Load(object sender, EventArgs e) {
            ExtensionMethods.DoubleBufferDataGridViews(this);
            if (UI.ProjectLoaded == true) {
                ShowDteq();
                FillDteqListBox();
            }
        }


    //---DATA ACCESS
        //TODO - move to DataAccess

        //Dteq
        private void GetDteq() {
            ListManager.dteqList = UI.prjDb.GetRecords<DistributionEquipmentModel>("DistributionEquipment");
            ListManager.CreateMasterLoadList();
            ListManager.AssignLoadsToDteq();
        }
        private void SaveDteq() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";
            foreach (var dteq in ListManager.dteqList) {
                update = UI.prjDb.UpdateRecord(dteq, "DistributionEquipment");
                if (update.Item1 == false) {
                    error = true;
                    message = update.Item2;
                }
            }
            if (error) {
                MessageBox.Show(message);
            }
        }

        //Loads
        private void GetLoads() {
            ListManager.loadList = UI.prjDb.GetRecords<LoadModel>("Loads");
        }
        private void SaveLoads() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";
            foreach (var load in ListManager.loadList) {
                update = UI.prjDb.UpdateRecord(load, "Loads");
                if (update.Item1 == false) {
                    error = true;
                    message = update.Item2;
                }
            }
            if (error) {
                MessageBox.Show(message);
            }
        }

        //Cables
        private void GetCables() {
            ListManager.cableList = UI.prjDb.GetRecords<CableModel>("Cables");
        }
        private void SaveCables() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";
            foreach (var cable in ListManager.cableList) {
                update = UI.prjDb.UpdateRecord(cable, "Cables");
                if (update.Item1 == false) {
                    error = true;
                    message = update.Item2;
                }
            }
            if (error) {
                MessageBox.Show(message);
            }
        }
        private void CreateCableList() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";

            //deletes all cables in Db
            UI.prjDb.DeleteAllRecords("Cables");
            ListManager.CreateCableList();
            //dgvEquipment.DataSource = ListManager.cableList;

            //creates a new Db Cable List
            List<string> properties = new List<string> { "Tag", "Category", "Source", "Destination" };
            foreach (var cable in ListManager.cableList) {
                update = UI.prjDb.InsertRecord(cable, "Cables");
                if (update.Item1 == false) {
                    error = true;
                    message = update.Item2;
                }
            }
            if (error) {
                MessageBox.Show(message);
            }
            ListManager.cableList.Clear();
            ListManager.cableList = UI.prjDb.GetRecords<CableModel>("Cables");
        }


    //---PRESENTERS (UI for this form)
        private void ShowDteq() {
            dgvEquipment.DataSource = ListManager.dteqList;

            //Red Highlight PercentLoaded and Size Cells
            double _dteqMaxLoadingPercentage;
            foreach (DataGridViewRow row in dgvEquipment.Rows) {
                if (double.TryParse(StringSettings.DteqMaxPercentLoaded, out _dteqMaxLoadingPercentage)) {
                    if (double.Parse(row.Cells["PercentLoaded"].Value.ToString()) > _dteqMaxLoadingPercentage) {
                        row.Cells["PercentLoaded"].Style.BackColor = Color.FromArgb(250, 150, 150);
                        row.Cells["Size"].Style.BackColor = Color.FromArgb(250, 150, 150);
                    }
                }
            }
        }
        private void ShowLoads() {
            dgvEquipment.DataSource = ListManager.loadList;
        }
        private void ShowCables() {
            dgvEquipment.DataSource = ListManager.cableList;
        }
        private void FillDteqListBox() {
            lstDteq.Items.Clear();
            foreach (var dteq in ListManager.dteqList) {
                lstDteq.Items.Add(dteq.Tag);
            }
            //lstDteq.DataSource = ListManager.dteqList;
            //lstDteq.DisplayMember = "Tag";
        }
        

        private void btnDistributionEquipment_Click(object sender, EventArgs e) {
            GetDteq();
            FillDteqListBox();
            ShowDteq();
            lblSelectedTag.Text = "";
            lblListName.Text = "Distribution Equipment";
        }

        private void btnSaveDteq_Click(object sender, EventArgs e) {
            SaveDteq();
        }

        private void btnDeleteDteq_Click(object sender, EventArgs e) {

        }

        private void btnLoadList_Click(object sender, EventArgs e) {
            SaveLoads();
            ShowLoads();
            lblSelectedTag.Text = "";
            lblListName.Text = "Loads";
        }

        private void btnAddLoad_Click(object sender, EventArgs e) {

        }

        private void btnDeleteLoad_Click(object sender, EventArgs e) {
            LoadModel load = dgvEquipment.SelectedRows[0].DataBoundItem as LoadModel;
            UI.prjDb.DeleteRecord("Loads", load.Id);
            ShowLoads();
        }

        private void btnCalculateLoads_Click(object sender, EventArgs e) {
            ListManager.CalculateLoads();
            SaveLoads();
            ShowLoads();
        }

        private void btnSaveLoads_Click(object sender, EventArgs e) {
            SaveLoads();
        }

        

        private void lstDteq_SelectedIndexChanged(object sender, EventArgs e) {
            string selectedEq = lstDteq.SelectedItem.ToString();
            DistributionEquipmentModel dteq;
            dteq = lstDteq.SelectedItem as DistributionEquipmentModel;
            dteq = ListManager.dteqList.FirstOrDefault(t => t.Tag == selectedEq);

            lblSelectedTag.Text = dteq.Tag;
            lblListName.Text = $"Assigned Loads";
            dgvEquipment.DataSource = dteq.AssignedLoads;
        }

        private void btnCreateCableList_Click(object sender, EventArgs e) {
            lblListName.Text = "CABLE LIST";
            CreateCableList();
            ShowCables();
        }
        private void btnCalculateCables_Click(object sender, EventArgs e) {
            ListManager.CalculateCableAmps();
            dgvEquipment.Refresh();
        }

    }
}
