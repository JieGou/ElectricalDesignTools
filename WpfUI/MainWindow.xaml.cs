using EDTLibrary;
using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EDTLibrary;
using WinFormCoreUI;
using EDTLibrary.DataAccess;
using WpfUI.ViewModels;
using WpfUI.Views;

namespace WpfUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window {

        private readonly ProjectSettingsViewModel _projectSettingsVM = new ProjectSettingsViewModel();
        private readonly EquipmentViewModel _equipmentVM = new EquipmentViewModel();
        private readonly CableListView _cableListVM = new CableListView();

        private readonly UserControl _startUpV = new EquipmentView();
        private readonly UserControl _projectSettingsView = new EquipmentView();
        private readonly UserControl _equipmentView = new EquipmentView();
        private readonly UserControl _cableListView = new EquipmentView();

        public MainWindow() {
                InitializeComponent();
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Browsable = false
            if (((PropertyDescriptor)e.PropertyDescriptor).IsBrowsable == false) {
                e.Cancel = true;
            }
            //Description == "x"
            if (((PropertyDescriptor)e.PropertyDescriptor).Description == "GroupName") {
                e.Cancel = true;
            }
            if ((e.PropertyName) == "Type") {
                var cb = new DataGridComboBoxColumn();
                cb.ItemsSource = new List<string> { "ADD LOAD TYPES" };
                //e.Column = cb;
            }
            //Display name
            e.Column.Header = ((PropertyDescriptor)e.PropertyDescriptor).DisplayName;
            e.Column.MinWidth = 50;

            e.Column.HeaderStyle = new Style(typeof(DataGridColumnHeader));
            e.Column.HeaderStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));

            e.Column.CellStyle = new Style(typeof(DataGridCell));
            e.Column.CellStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));


            if (e.PropertyName == "FedFrom") {

                var cb = new DataGridComboBoxColumn();
                cb.ItemsSource = ListManager.dteqList;
                cb.SelectedValuePath = "Tag";
                cb.DisplayMemberPath = "Tag";
                cb.SelectedValueBinding = new Binding("FedFrom"); //allows binding to the property

                cb.CellStyle = new Style(typeof(DataGridCell));
                cb.CellStyle.Setters.Add(new Setter(BackgroundProperty, Brushes.Transparent));
                cb.CellStyle.Setters.Add(new Setter(ForegroundProperty, Brushes.Black));
                cb.Header = ((PropertyDescriptor)e.PropertyDescriptor).DisplayName;
                e.Column = cb;
            }
        }

        /// <summary>
        /// Changes checkbox columsn to Template checkbox columsn for single clickable check boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAutoGeneratingColumn2(object sender, DataGridAutoGeneratingColumnEventArgs e)
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
        }

        private void btnStartup_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnProjectSettings_Click(object sender, RoutedEventArgs e)
        {
            //ccMain.DataContext = _projectSettingsView;
            //ccMain.Content = _equipmentView;

        }

        private void btnEquipment_Click(object sender, RoutedEventArgs e)
        {
            //ccMain.DataContext = _equipmentVM;
            //ccMain.Content = _equipmentView;
        }
    }
}
