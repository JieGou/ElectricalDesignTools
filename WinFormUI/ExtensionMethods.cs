using System;
using System.Windows.Forms;
using System.Reflection;

namespace WinFormUI {
    //DoubleBuffering
    public static class ExtensionMethods {
        public static void DoubleBuffered(this DataGridView dgv, bool setting) {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static void EnumerateChildren(this Control root) {
            foreach (Control control in root.Controls) {
                MessageBox.Show($"Control {control.Name} - Parent {root.Name}");
                if (control.Controls != null) {
                    EnumerateChildren(control);
                }
            }
        }

        public static void DisableButtons(this Control root) {
            foreach (Control control in root.Controls) {
                if (control is Button) {
                    control.Enabled = false;                }                
                if (control.Controls != null) {
                    DisableButtons(control);
                }
            }
        }

        public static void EnableButtons(this Control root) {
            foreach (Control control in root.Controls) {
                if (control is Button) {
                    control.Enabled = true;
                }
                if (control.Controls != null) {
                    EnableButtons(control);
                }
            }
        }

    }
}
