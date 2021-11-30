using EDTLibrary;
using EDTLibrary.Models;
using LM = EDTLibrary.ListManager;
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

        public frmEquipment() {
            InitializeComponent();
        }
        private void frmEquipment_Load(object sender, EventArgs e) {

            dgvEquipment.DoubleBuffered(true);
            dgvEquipment.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblListName.Text = "";
            lblSelectedTag.Text = "";

            if (UI.ProjectLoaded == true) {
                ShowDteq();
                FillDteqListBox();
            }

            //Testing
            //dgvEquipment.DataSource = DataTables.CableTypes;
        }

        

        //Dteq Listbox
        private void FillDteqListBox() {
            lstDteq.Items.Clear();
            foreach (var dteq in LM.dteqList) {
                lstDteq.Items.Add(dteq.Tag);
            }
            //lstDteq.DataSource = LM.dteqList;
            //lstDteq.DisplayMember = "Tag";
        }

        //TODO - move generic methods to DataAccess
        //Loads
        private void GetLoads() {
            LM.loadList = UI.prjDb.GetRecords<LoadModel>("Loads");
        }
        private void ShowLoads() {
            dgvEquipment.DataSource = LM.loadList;

            
        }
        private void SaveLoads() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";
            foreach (var load in LM.loadList) {
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

        //Dteq
        private void GetDteq() {
            LM.dteqList = UI.prjDb.GetRecords<DistributionEquipmentModel>("DistributionEquipment");
            LM.CreateMasterLoadList();
            LM.AssignLoadsToDteq();
        }
        private void ShowDteq() {
            dgvEquipment.DataSource = LM.dteqList;

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
        private void SaveDteq() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";
            foreach (var dteq in LM.dteqList) {
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

        //Cables
        private void GetCables() {
            LM.cableList = UI.prjDb.GetRecords<CableModel>("Cables");
        }
        private void ShowCables() {
            dgvEquipment.DataSource = LM.cableList;
        }
        private void SaveCables() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";
            foreach (var cable in LM.cableList) {
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
            LM.CreateCableList();
            //dgvEquipment.DataSource = LM.cableList;

            //creates a new Db Cable List
            List<string> properties = new List<string> { "Tag", "Category", "Source", "Destination" };
            foreach (var cable in LM.cableList) {
                update = UI.prjDb.InsertRecord(cable, "Cables");
                if (update.Item1 == false) {
                    error = true;
                    message = update.Item2;
                }
            }
            if (error) {
                MessageBox.Show(message);
            }
            LM.cableList.Clear();
            LM.cableList = UI.prjDb.GetRecords<CableModel>("Cables");
            dgvEquipment.DataSource = LM.cableList;
        }

        //Buttons
        //Loads
        private void btnLoadList_Click(object sender, EventArgs e) {
            SaveLoads();
            //GetLoads();
            //LM.AssignLoadsToDteq();
            //LM.CreateMasterLoadList();
            ShowLoads();
            lblSelectedTag.Text = "";
            lblListName.Text = "Loads";

        }
        private void btnDeleteLoad_Click(object sender, EventArgs e) {
            LoadModel load = dgvEquipment.SelectedRows[0].DataBoundItem as LoadModel;
            UI.prjDb.DeleteRecord("Loads", load.Id);
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
            GetDteq();
            FillDteqListBox();
            ShowDteq();
            lblSelectedTag.Text = "";
            lblListName.Text = "Distribution Equipment";

            
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
            dteq = lstDteq.SelectedItem as DistributionEquipmentModel;
            dteq = LM.dteqList.FirstOrDefault(t => t.Tag == selectedEq);            

            lblSelectedTag.Text = dteq.Tag;
            lblListName.Text = $"Assigned Loads";
            dgvEquipment.DataSource = dteq.AssignedLoads;

            //testing
            listBox1.Items.Clear();
            foreach (var load in LM.masterLoadList) {
                listBox1.Items.Add(load.Tag);
            }
        }
        private void pnlHeader_MouseLeave(object sender, EventArgs e) {
            //pnlHeader.Visible = false;
        }
        private void pnlRight_MouseEnter(object sender, EventArgs e) {
            do {
                System.Threading.Thread.Sleep(1);
                pnlRight.Width += 10;
            } while (pnlRight.Width < 200);
        }
        private void pnlRight_MouseLeave(object sender, EventArgs e) {
            do {
                System.Threading.Thread.Sleep(1);
                pnlRight.Width -= 10;
            } while (pnlRight.Width > 20);

        }
        private void btnCreateCableList_Click_1(object sender, EventArgs e) {
            lblListName.Text = "CABLE LIST";
            CreateCableList();
            
        }

        private void btnAddDteq_Click(object sender, EventArgs e) {
            FillDteqListBox();
        }

        private void btnDeleteDteq_Click(object sender, EventArgs e) {
            
            
        }

        List<CablesUsedInProject> cablesUsedInProjects = new List<CablesUsedInProject>();
        public class CablesUsedInProject {
            public CablesUsedInProject() {
                UsedInProject = true;
            }
            public int Id { get; set; }
            public string Size { get; set; }
            public bool UsedInProject { get; set; }

            //public void CreateUsedCableList() {
            //    for (int i = 0; i < DataTables.CableSizes.Rows.Count; i++) {
            //        cablesUsedInProjects.Add(new CablesUsedInProject { Size = DataTables.CableSizes.Rows[i]["Size"].ToString() });
            //    }
            //    foreach (var line in cablesUsedInProjects) {
            //        UI.prjDb.InsertRecord(line, "CablesUsedInProject");
            //    }
            //}
            
        }

        private void button1_Click(object sender, EventArgs e) {
            LM.CalculateCableAmps();
            //dgvEquipment.Update();
            dgvEquipment.DataSource = null;
            dgvEquipment.DataSource = LM.cableList;
        }
    }
}
