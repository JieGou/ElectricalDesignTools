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
using EDTLibrary.ProjectSetting;

namespace WinFormCoreUI {
    public partial class frmSettings : Form {
        public frmSettings() {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e) {
            ExtensionMethods.DoubleBufferDataGridViews(this);

            Type prjStringSettings = typeof(StringSettings);

            lstStringSettings.Items.Clear();
            foreach (var prop in prjStringSettings.GetProperties()) {
                if (prop.PropertyType.Name == "String") {
                    lstStringSettings.Items.Add(prop.Name);
                }
                else if (prop.PropertyType.Name == "DataTable") {
                    lstTableSettings.Items.Add(prop.Name);
                }
            }
        }

        private void lstStringSettings_SelectedIndexChanged(object sender, EventArgs e) {
            if (lstStringSettings.SelectedItem != null) {
                txtStringSettingValue.Text = SettingManager.GetStringSetting(lstStringSettings.SelectedItem.ToString());
            }

        }

        private void btnSaveSetting_Click(object sender, EventArgs e) {
            StringSettings.InitializeSettings();
            if (lstStringSettings.SelectedItem!=null) {
                try {
                    SettingManager.SaveStringSetting(lstStringSettings.SelectedItem.ToString(), txtStringSettingValue.Text);
                    UI.SaveProjectSettings();
                }
            catch { };
            }
            
        }

        private void lstTableSettings_SelectedIndexChanged(object sender, EventArgs e) {
            if (lstTableSettings.SelectedItem != null) {
                dgvTableSettingValue.DataSource = SettingManager.GetTableSetting(lstTableSettings.SelectedItem.ToString());
            }
        }

        private void dgvTableSettingValue_DataSourceChanged(object sender, EventArgs e) {
            UI.ScaleDataGridView(dgvTableSettingValue);
        }
    }
}
