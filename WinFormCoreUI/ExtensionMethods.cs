using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WinFormCoreUI {
    //DoubleBuffering
    public static class ExtensionMethods {
        public static void DoubleBuffer(this DataGridView dgv) {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, true, null);
        }

        public static void DoubleBufferDataGridViews(this Control parent) {
            foreach (Control control in parent.Controls) {
                string n = control.Name;
                if (control is DataGridView) {
                    DoubleBuffer(control as DataGridView);
                }
                if (control.Controls != null) {
                    DoubleBufferDataGridViews(control);
                }
            }
        }


        public static void EnumerateChildren(this Control parent) {
            foreach (Control control in parent.Controls) {
                MessageBox.Show($"Control {control.Name} - Parent {parent.Name}");
                if (control.Controls != null) {
                    EnumerateChildren(control);
                }
            }
        }

        public static void DisableButtons(this Control parent) {
            foreach (Control control in parent.Controls) {
                if (control is Button) {
                    control.Enabled = false;
                }
                if (control.Controls != null) {
                    DisableButtons(control);
                }
            }
        }

        public static void EnableButtons(this Control parent) {
            foreach (Control control in parent.Controls) {
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
