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
    public partial class frmSettings: Form {
        public frmSettings() {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e) {
            Type prjStringSettings = typeof(StringSettings);

            lstStringSettings.Items.Clear();
            foreach (var prop in prjStringSettings.GetProperties()) {
                if (prop.PropertyType.Name =="String") {
                    lstStringSettings.Items.Add(prop.Name);
                }
                else if(prop.PropertyType.Name == "DataTable") {
                    lstTableSettings.Items.Add(prop.Name);
                }
            }

            txtStringSettingValue.Text = "";

            //Styling dgvCablesInProject
            dgvCablesInProject.DataSource = EDTLibrary.StringSettings.CableSizesUsedInProject;
            dgvCablesInProject.Columns["Id"].Visible = false;
            dgvCablesInProject.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            dgvCablesInProject.Columns["Size"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCablesInProject.Columns["Size"].ReadOnly = true;
            dgvCablesInProject.Columns["UsedInProject"].HeaderText = "Use In Project";
            dgvCablesInProject.Columns["Size"].Width += 130;

            ScaleDataGridView(dgvCablesInProject);
        }
        
        private void AutoSizeDataGrid(DataGridView dgv) {

        }

        private void ScaleDataGridView(DataGridView dataGridView) {
            double totalWidth = 0;
            foreach (DataGridViewColumn col in dataGridView.Columns) {
                if (col.Visible==true) {
                    totalWidth += col.Width;
                }
            }
            double totalHeight = 0;
            foreach (DataGridViewRow row in dataGridView.Rows) {
                totalHeight += row.Height;
            }
            totalHeight += dataGridView.ColumnHeadersHeight;
            dataGridView.Width = (int)totalWidth  ;
            dataGridView.Height = (int)totalHeight ;
        }

        private void lstProperties_SelectedIndexChanged(object sender, EventArgs e) {
            txtStringSettingValue.Text = UI.GetSetting(lstStringSettings.SelectedItem.ToString());
            dgvTableSettingValue.DataSource = StringSettings.CableAmpsUsedInProject;
        }

        private void btnSaveProperty_Click(object sender, EventArgs e) {
            UI.SaveSetting(lstStringSettings.SelectedItem.ToString(), txtStringSettingValue.Text);
            UI.SaveProjectSettings();
        }

        private void dgvCablesInProject_CellContentClick(object sender, DataGridViewCellEventArgs e) {
        }

        private void dgvCablesInProject_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            StringSettings.InitializeSettings();
        }
    }
}
