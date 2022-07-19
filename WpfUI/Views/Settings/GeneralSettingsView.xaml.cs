using EDTLibrary.ProjectSettings;
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
using WpfUI.ViewModels.Menus;

namespace WpfUI.Views.Settings;
/// <summary>
/// Interaction logic for GeneralSettingsView.xaml
/// </summary>
public partial class GeneralSettingsView : UserControl
{
    private SettingsMenuViewModel settingsVm { get { return DataContext as SettingsMenuViewModel; } }

    public GeneralSettingsView()
    {
        InitializeComponent();
    }

    private void dgdTableSetting_MouseLeave(object sender, MouseEventArgs e)
    {
    }


    /// <summary>
    /// Changes checkbox columsn to Template checkbox columsn for single clickable check boxes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    private void btnSaveSettings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        foreach (var prop in typeof(EdtSettings).GetProperties()) {
            foreach (var setting in SettingsManager.SettingList) {
                if (prop.Name==setting.Name) {
                    setting.Value = prop.GetValue(settingsVm.EdtSettings).ToString();
                }
            }
        }
    }
}
