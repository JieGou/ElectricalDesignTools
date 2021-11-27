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
            Type prjSettings = typeof(ProjectSettings);

            lstProperties.Items.Clear();
            foreach (var prop in prjSettings.GetProperties()) {
                lstProperties.Items.Add(prop.Name);
            }

            txtPropertyValue.Text = "";

            //Styling dgvCablesInProject
            dgvCablesInProject.DataSource = EDTLibrary.ProjectSettings.CableSizesUsedInProject;
            dgvCablesInProject.Columns["Id"].Visible = false;
            dgvCablesInProject.Columns["Size"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCablesInProject.Columns["Size"].ReadOnly = true;
            dgvCablesInProject.Columns["UsedInProject"].HeaderText = "Use In Project";
            dgvCablesInProject.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvCablesInProject.Columns["Size"].Width += 130;
            ScaleDataGridView(dgvCablesInProject);
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
            txtPropertyValue.Text = UI.GetSetting(lstProperties.SelectedItem.ToString());
        }

        private void btnSaveProperty_Click(object sender, EventArgs e) {
            UI.SaveSetting(lstProperties.SelectedItem.ToString(), txtPropertyValue.Text);
            UI.SaveProjectSettings();
        }

        private void dgvCablesInProject_CellContentClick(object sender, DataGridViewCellEventArgs e) {
        }

        private void dgvCablesInProject_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            ProjectSettings.CreateCableAmpsUsedInProject();
        }
    }
}
