using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
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
using WpfUI.ViewModels.Electrical;
using WpfUI.Views.Electrical.MjeqSubviews;

namespace WpfUI.Views.Electrical;
/// <summary>
/// Interaction logic for SinlgeLineView.xaml
/// </summary>
public partial class SinlgeLineView : UserControl
{
    private SingleLineViewModel vm { get { return DataContext as SingleLineViewModel; } }

    public SinlgeLineView()
    {
        InitializeComponent();

        if (vm != null) {
            if (vm.ViewableDteqList.Count > 0) {
                vm.SelectedDteq = vm.ViewableDteqList[0];
            }
        }
    }

    DteqTabsView _dteqTabsView = new DteqTabsView();
    LoadTabsView _loadTabsView = new LoadTabsView();

    //Sets the datacontext for the details view panel on the right
    private void dgdDteq_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }



    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (vm != null) {
            if (vm.ViewableDteqList.Count > 0) {
                vm.SelectedDteq = vm.ViewableDteqList[0];
            }
        }
    }

    //Sets the datacontext for the details view panel on the right

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //_loadTabsView.DataContext = this.DataContext;
        //LoadDetailsContent.Content = _loadTabsView;
    }


    private void LoadGraphicView_LoadEquipmentSelected(object sender, RoutedEventArgs e)
    {

        SingleLineViewModel slVm = (SingleLineViewModel)DataContext;
        slVm.SelectedLoadEquipment = (IEquipment)e.OriginalSource;
        slVm.IsSelectedLoadCable = false;

    }

    private void LoadGraphicView_LoadCableSelected(object sender, RoutedEventArgs e)
    {
        SingleLineViewModel slVm = (SingleLineViewModel)DataContext;
        slVm.SelectedLoadCable = (IEquipment)e.OriginalSource;
        slVm.IsSelectedLoadCable = true;

    }
}
