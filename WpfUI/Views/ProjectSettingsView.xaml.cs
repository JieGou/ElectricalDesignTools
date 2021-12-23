using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfUI.Views
{
    /// <summary>
    /// Interaction logic for ProjectSettingsView.xaml
    /// </summary>
    public partial class ProjectSettingsView : UserControl
    {
        public ProjectSettingsView() {
            InitializeComponent();
        }

        private void dgdTableSetting_MouseLeave(object sender, MouseEventArgs e)
        {
            dgdTableSetting.CommitEdit();
        }

        private void dgdTableSetting_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

            if (e.Column is DataGridCheckBoxColumn && !e.Column.IsReadOnly) {
                var checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));
                checkboxFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                checkboxFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
                checkboxFactory.SetBinding(ToggleButton.IsCheckedProperty, new Binding(e.PropertyName) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                e.Column = new DataGridTemplateColumn {
                    Header = e.Column.Header,
                    CellTemplate = new DataTemplate {
                        VisualTree = checkboxFactory
                    },
                    SortMemberPath = e.Column.SortMemberPath
                };
            }
            if (e.PropertyName == "Id") {
                e.Cancel = true;

            }

        }
    }
}
