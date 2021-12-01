using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormCoreUI {
    public partial class frmDataTables : Form {
        public frmDataTables() {
            InitializeComponent();
        }
        private void frmDataTables_Load(object sender, EventArgs e) {
            ExtensionMethods.DoubleBufferDataGridViews(this);
            GetDataTables();
        }

        private void GetDataTables() {
            lstDataTables.Items.Clear();
            ArrayList tables = UI.libDb.GetListOfTablesNamesInDb();

            foreach (string item in tables) {
                lstDataTables.Items.Add(item);
            }
        }

        private void btnGetTables_Click(object sender, EventArgs e) {
            
        }

        private void lstDataTables_SelectedIndexChanged(object sender, EventArgs e) {
            DataTable dt = UI.libDb.GetDataTable(lstDataTables.SelectedItem.ToString());
            dgvDataTable.DataSource = dt;
        }

       
    }
}
