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
using System.Reflection;

namespace WinFormUI {
    public partial class frmMain: Form {
        SQLiteDapperConnector sqldc;

        public frmMain() {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e) {
            dgvMain.DoubleBuffered(true);
            this.WindowState = FormWindowState.Maximized;
            dgvMain.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            sqldc = new SQLiteDapperConnector(Properties.Settings.Default.ProjectDb);

            //RefreshDteq();
            ShowDteq();
            RefreshLoads();
            FillDteqListBox();
            //LM.CreateDteqDict();
            UpdateStatus();            
        }
        
        //Dteq Listbox
        private void FillDteqListBox() {
            foreach (var dteq in LM.dteqList) {
                lstDteq.Items.Add(dteq.Tag);
            }
        }

        //DB and Datagrid Refresh
        private void SaveLoads() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";
            foreach (var load in LM.loadList) {
                update = sqldc.UpdateRecord<LoadModel>(load, "Loads");
                if (update.Item1 == false) {
                    error = true;
                    message = update.Item2;
                }
            }
            if (error) {
                MessageBox.Show(message);
            }
        }
        private void RefreshLoads() {
            LM.loadList = sqldc.GetRecords<LoadModel>("Loads");
        }
        private void ShowLoads() {
            RefreshLoads();
            dgvMain.DataSource = LM.loadList;
        }
        private void SaveDteq() {
            Tuple<bool, string> update;
            bool error = false;
            string message = "";
            foreach (var dteq in LM.dteqList) {
                update = sqldc.UpdateRecord<DistributionEquipmentModel>(dteq, "DistributionEquipment");
                if (update.Item1 == false) {
                    error = true;
                    message = update.Item2;
                }
            }
            if (error) {
                MessageBox.Show(message);
            }
        }
        private void RefreshDteq() {
            LM.dteqList = sqldc.GetRecords<DistributionEquipmentModel>("DistributionEquipment");
        }
        private void ShowDteq() {
            RefreshDteq();
            dgvMain.DataSource = LM.dteqList;

        }

        ///Loads
        private void btnLoadList_Click(object sender, EventArgs e) {
            RefreshLoads();
            ShowLoads();
        }
        private void btnDeleteLoad_Click(object sender, EventArgs e) {
            LoadModel load = dgvMain.SelectedRows[0].DataBoundItem as LoadModel;
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
            //LM.CreateDteqDict();
            ShowDteq();
        }
        private void btnAssignLoads_Click(object sender, EventArgs e) {
            LM.AssignLoadsToDteq();
            SaveDteq();
            RefreshLoads();
            ShowDteq();
            LM.AssignLoadsToDteq();
        }
        private void btnSaveDteq_Click(object sender, EventArgs e) {
            SaveDteq();
        }

        //Misc
        private void btnSelectProjectDb_Click(object sender, EventArgs e) {
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
                    UpdateStatus();
                }
            }
        }
        private void UpdateStatus() {
            stsLabel1.Text = Path.GetFileName(Properties.Settings.Default.ProjectDb);
        }
        private void dgdMain_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            stsLabel3.Text = dgvMain.SelectedRows.Count.ToString();
        }

        private void lstDteq_SelectedIndexChanged(object sender, EventArgs e) {
            string selectedEq = lstDteq.SelectedItem.ToString();
            DistributionEquipmentModel dteq;
            dteq = LM.dteqList.FirstOrDefault(t => t.Tag == selectedEq);
            dgvMain.DataSource = dteq.AssignedLoads;
        }
    }






    public static class ExtensionMethods {
        public static void DoubleBuffered(this DataGridView dgv, bool setting) {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
}
