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
public partial class ElectricalView : UserControl
{
    private ElectricalViewModel elecVm { get { return DataContext as ElectricalViewModel; } }

    DteqDetailView _dteqDetailsView = new DteqDetailView();
    LoadDetailView _loaDetailView = new LoadDetailView();

    public ElectricalView()
    {
        InitializeComponent();
       
    }

}
