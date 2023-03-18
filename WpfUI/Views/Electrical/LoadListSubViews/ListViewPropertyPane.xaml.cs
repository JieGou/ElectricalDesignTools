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

namespace WpfUI.Views.Electrical.LoadListSubViews;
/// <summary>
/// Interaction logic for LoadDetailsView.xaml
/// </summary>
public partial class ListViewPropertyPane : UserControl
{
    public ListViewPropertyPane()
    {
        InitializeComponent();
    }

    private void LoadGraphicView_EquipmentSelected(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is IEquipment) {
            LoadListViewModel mjeqVm = (LoadListViewModel)DataContext;
            mjeqVm.SelectedLoadEquipment = (IEquipment)e.OriginalSource;
            mjeqVm.IsSelectedLoadCable = false;
        }
        

    }

    private void LoadGraphicView_EquipmentCableSelected(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is IEquipment) {
            LoadListViewModel mjeqVm = (LoadListViewModel)DataContext;
            mjeqVm.SelectedLoadCable = (IEquipment)e.OriginalSource;
            mjeqVm.IsSelectedLoadCable = true;
        }
    }
}
