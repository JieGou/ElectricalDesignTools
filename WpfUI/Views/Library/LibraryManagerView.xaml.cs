using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using WpfUI.ViewModels.Library;

namespace WpfUI.Views.Library
{
    /// <summary>
    /// Interaction logic for DataTablesView.xaml
    /// </summary>
    public partial class LibraryManagerView : UserControl
    {
        private LibraryManagerViewModel vm { get { return DataContext as LibraryManagerViewModel; } }

        public LibraryManagerView()
        {
            InitializeComponent();
        }

        private void dgdDataTables_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
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

            e.Column.Header = ((PropertyDescriptor)e.PropertyDescriptor).DisplayName;


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (vm != null) {
                vm.GetDataTables();
            }
        }

        
    }
}
