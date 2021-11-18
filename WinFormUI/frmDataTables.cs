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
using SQLiteLibrary;

namespace WinFormUI {
    public partial class frmDataTables: Form {

        public frmDataTables() {
            InitializeComponent();
        }

        private void frmDataTables_Load(object sender, EventArgs e) {
            dgvDataTable.DoubleBuffered(true);

        }

        private void lstDataTables_SelectedIndexChanged(object sender, EventArgs e) {
            DataTable dt = UI.libDb.GetDataTable(lstDataTables.SelectedItem.ToString());
            dgvDataTable.DataSource = dt;
        }

        private void btnGetTables_Click(object sender, EventArgs e) {
            lstDataTables.Items.Clear();
            ArrayList tables = UI.libDb.GetDbTables();

            foreach (string item in tables) {
                lstDataTables.Items.Add(item);
            }
        }
    }


}
