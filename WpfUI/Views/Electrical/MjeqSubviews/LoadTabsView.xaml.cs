using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WpfUI.Views.Electrical.MjeqSubviews;
/// <summary>
/// Interaction logic for LoadDetailsView.xaml
/// </summary>
public partial class LoadTabsView : UserControl
{
    public LoadTabsView()
    {
        InitializeComponent();
    }

    private void LoadGraphicView_EquipmentSelected(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is IEquipment) {
            MjeqViewModel mjeqVm = (MjeqViewModel)DataContext;
            mjeqVm.SelectedLoadEquipment = (IEquipment)e.OriginalSource;
            mjeqVm.IsSelectedLoadCable = false;
        }
        

    }

    private void LoadGraphicView_EquipmentCableSelected(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is IEquipment) {
            MjeqViewModel mjeqVm = (MjeqViewModel)DataContext;
            mjeqVm.SelectedLoadCable = (IEquipment)e.OriginalSource;
            mjeqVm.IsSelectedLoadCable = true;
        }
    }
}
