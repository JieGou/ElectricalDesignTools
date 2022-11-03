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
using WpfUI.ViewModels.Cables;
using WpfUI.ViewModels.Electrical;

namespace WpfUI.Views.Cable;
/// <summary>
/// Interaction logic for CableListView.xaml
/// </summary>
public partial class CableListView : UserControl
{
    private CableListViewModel vm { get { return DataContext as CableListViewModel; } }

    public CableListView()
    {
        InitializeComponent();
    }

    private void SfDataGrid_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key ==Key.Escape) {
            dgdCableList.ClearFilters();
        }
    }

    private void dgdCableList_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
    {
        if (vm!= null) {
            vm.SelectedCables = dgdCableList.SelectedItems;
        }
    }
}
