using EDTLibrary;
using EDTLibrary.Models;
using LM = EDTLibrary.ListManager;
using SQLiteLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormUI {
    public partial class frmEquipment: Form {
        SQLiteDapperConnector sqldc;

        public frmEquipment() {
            InitializeComponent();
        }
        private void frmEquipment_Load(object sender, EventArgs e) {

            dgvEquipment.DoubleBuffered(true);
            dgvEquipment.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblListName.Text = "";
            lblSelectedTag.Text = "";
            if (OpenDb()) {
                //RefreshDteq();
                ShowDteq();
                GetLoads();
                FillDteqListBox();
                //LM.CreateDteqDict();
            }


            dgvEquipment.DataSource = DataTables.Breakers;

        }

        private bool OpenDb() {
            string dbFilename = Properties.Settings.Default.ProjectDb;
            if (File.Exists(dbFilename)) {
                sqldc = new SQLiteDapperConnector(dbFilename);
                return true;
            }
            else {
                MessageBox.Show($"The selected file \n\n{dbFilename} cannot be found. Please select another project file.");
                return false;
            }

        }
        //Dteq Listbox
        private void FillDteqListBox() {
            foreach (var dteq in LM.dteqList) {
                lstDteq.Items.Add(dteq.Tag);
            }
        }



        //Loads
        private void GetLoads() {
            LM.loadList = sqldc.GetRecords<LoadModel>("Loads");
        }
        private void ShowLoads() {
            GetLoads();
            dgvEquipment.DataSource = LM.loadList;
        }
        private void SaveLoads() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";
            foreach (var load in LM.loadList) {
                update = sqldc.UpdateRecord(load, "Loads");
                if (update.Item1 == false) {
                    error = true;
                    message = update.Item2;
                }
            }
            if (error) {
                MessageBox.Show(message);
            }
        }

        //Dteq
        private void GetDteq() {
            LM.dteqList = sqldc.GetRecords<DistributionEquipmentModel>("DistributionEquipment");
            LM.AssignLoadsToDteq();
        }
        private void ShowDteq() {
            GetDteq();
            dgvEquipment.DataSource = LM.dteqList;

        }
        private void SaveDteq() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";
            foreach (var dteq in LM.dteqList) {
                update = sqldc.UpdateRecord(dteq, "DistributionEquipment");
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
        private void SaveCables() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";
            foreach (var cable in LM.cableList) {
                update = sqldc.UpdateRecord(cable, "Cables");
                if (update.Item1 == false) {
                    error = true;
                    message = update.Item2;
                }
            }
            if (error) {
                MessageBox.Show(message);
            }
        }
        private void RefreshCables() {
            LM.cableList = sqldc.GetRecords<CableModel>("Cables");
        }
        private void ShowCables() {
            RefreshCables();
            dgvEquipment.DataSource = LM.cableList;
        }
        private void CreateCableList() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";

            sqldc.DeleteAllRecords("Cables");
            LM.CreateCableList();
            dgvEquipment.DataSource = LM.cableList;

            List<string> properties = new List<string> { "Tag", "Category", "Source", "Destination" };
            foreach (var cable in LM.cableList) {
                update = sqldc.InsertRecord(cable, "Cables");
                if (update.Item1 == false) {
                    error = true;
                    message = update.Item2;
                }
            }
            if (error) {
                MessageBox.Show(message);
            }
            LM.cableList.Clear();
            LM.cableList = sqldc.GetRecords<CableModel>("Cables");
            dgvEquipment.DataSource = LM.cableList;
        }

        //Buttons
        //Loads
        private void btnLoadList_Click(object sender, EventArgs e) {
            GetLoads();
            ShowLoads();
            lblSelectedTag.Text = "";
            lblListName.Text = "Loads";

        }
        private void btnDeleteLoad_Click(object sender, EventArgs e) {
            LoadModel load = dgvEquipment.SelectedRows[0].DataBoundItem as LoadModel;
            sqldc.DeleteRecord("Loads", load.Id);
            ShowLoads();
        }
        private void btnCalculateLoads_Click(object sender, EventArgs e) {
            LM.CalculateLoads();
            SaveLoads();
            ShowLoads();
        }
        private void btnSaveLoads_Click(object sender, EventArgs e) {
            SaveLoads();
        }

        //Dteq
        private void btnDteqList_Click(object sender, EventArgs e) {
            ShowDteq();
            lblSelectedTag.Text = "";
            lblListName.Text = "Distribution Equipment";
        }
        private void btnAssignLoads_Click(object sender, EventArgs e) {
            ShowDteq();
            LM.AssignLoadsToDteq();
        }
        private void btnSaveDteq_Click(object sender, EventArgs e) {
            SaveDteq();
        }

        //Cables
        private void btnCables_Click(object sender, EventArgs e) {
            ShowCables();
        }
        private void btnSaveCables_Click(object sender, EventArgs e) {
            SaveCables();
        }
        private void btnCreateCableList_Click(object sender, EventArgs e) {
            
        }



        //Misc
        private void UpdateStatus() {

        }
        private void lstDteq_SelectedIndexChanged(object sender, EventArgs e) {
            string selectedEq = lstDteq.SelectedItem.ToString();
            DistributionEquipmentModel dteq;
            dteq = LM.dteqList.FirstOrDefault(t => t.Tag == selectedEq);            
            dgvEquipment.DataSource = dteq.AssignedLoads;

            lblSelectedTag.Text = selectedEq;
            lblListName.Text = $"Assigned Loads";
        }
        private void pnlHeader_MouseLeave(object sender, EventArgs e) {
            //pnlHeader.Visible = false;
        }
        private void pnlRight_MouseEnter(object sender, EventArgs e) {
            pnlRight.Width = 200;
        }
        private void pnlRight_MouseLeave(object sender, EventArgs e) {
            pnlRight.Width = 20;

        }
        private void btnCreateCableList_Click_1(object sender, EventArgs e) {
            CreateCableList();
        }
    }
}
