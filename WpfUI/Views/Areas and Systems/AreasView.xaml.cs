using EDTLibrary;
using EDTLibrary.LibraryData.TypeTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Areas_and_Systems;
using WpfUI.Views.Electrical.MjeqSubviews;

namespace WpfUI.Views.Areas_and_Systems;
/// <summary>
/// Interaction logic for AreasView.xaml
/// </summary>
public partial class AreasView : UserControl
{
    private AreasViewModel areaVm { get { return DataContext as AreasViewModel; } }

    public AreasView()
    {
        InitializeComponent();
    }
    private void txtAreaTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (txtAreaTag.Text == "" || txtAreaTag.Text == GlobalConfig.EmptyTag) txtAreaTag.Text = "";
    }

    private void txtAreaTag_LostFocus(object sender, RoutedEventArgs e)
    {
        if (txtAreaTag.Text == "") txtAreaTag.Text = GlobalConfig.EmptyTag;
    }



    private void FastEditEvent(object sender, RoutedEventArgs args)
    {
        var dataGridCell = (sender as UIElement)?.FindVisualParent<DataGridCell>();

        dataGridCell.FastEdit(args);
    }

    private void lstAreaClassification_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {

        if (lstAreaClassification.SelectedItem == null) return;

        areaVm.AreaClassificationsInfoTableItems.Clear();
        var areaClass = lstAreaClassification.SelectedItem as AreaClassificationType;
        areaVm.AreaClassificationsInfoTableItems.Add(areaClass);

    }

    private void dgdAreas_MouseLeave(object sender, MouseEventArgs e)
    {
        var dataGrid = (DataGrid)sender;
        dataGrid.CancelEdit();
    }
}
