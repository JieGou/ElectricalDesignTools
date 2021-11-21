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
        }

        private void lstProperties_SelectedIndexChanged(object sender, EventArgs e) {
            txtPropertyValue.Text = UI.GetSetting(lstProperties.SelectedItem.ToString());
        }

        private void btnSaveProperty_Click(object sender, EventArgs e) {
            UI.SaveProperty(lstProperties.SelectedItem.ToString(), txtPropertyValue.Text);
            UI.SaveProjectSettings();
        }
    }
}
