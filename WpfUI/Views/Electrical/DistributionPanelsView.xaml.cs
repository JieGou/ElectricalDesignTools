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

namespace WpfUI.Views.Electrical;
/// <summary>
/// Interaction logic for SinlgeLineView.xaml
/// </summary>
public partial class DistributionPanelsView : UserControl
{
    private DistributionPanelsViewModel vm { get { return DataContext as DistributionPanelsViewModel; } }

    public DistributionPanelsView()
    {
        InitializeComponent();

        if (vm != null) {
            if (vm.ViewableDteqList.Count > 0) {
                vm.SelectedDteq = vm.ViewableDteqList[0];
            }
        }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (vm != null) {
            if (vm.ViewableDteqList.Count > 0) {
                vm.SelectedDteq = vm.ViewableDteqList[0];
            }
        }
    }

    private void SfDataGrid_QueryRowHeight(object sender, Syncfusion.UI.Xaml.Grid.QueryRowHeightEventArgs e)
    {
        //var load = (LoadModel)sender;
        //if (load.Type == "MOTOR") {
        //    e.Height = 50;
        //}
        //else if (load.Type == "PANEL") {
        //    e.Height = 75;
        //}
    }
}
