using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.TestDataFolder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WpfUI.Helpers;
using WpfUI.ViewModels;
using WpfUI.Views.SubViews;

namespace WpfUI.Views;

/// <summary>
/// Interaction logic for EqView.xaml
/// </summary>
public partial class ElectricalMenuView : UserControl
{
    private ElectricalMenuViewModel elecVm { get { return DataContext as ElectricalMenuViewModel; } }

    DteqTabsView _dteqDetailsView = new DteqTabsView();
    LoadDetailView _loaDetailView = new LoadDetailView();

    public ElectricalMenuView()
    {
        InitializeComponent();
       
    }

}
