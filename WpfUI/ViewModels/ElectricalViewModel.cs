using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Helpers;
using WpfUI.Stores;
using WpfUI.ViewModels.Electrical;
using WpfUI.ViewModifiers;

namespace WpfUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class ElectricalViewModel : ViewModelBase, INotifyDataErrorInfo
{

    #region Constructor
    private DteqFactory _dteqFactory;
    private readonly MainViewModel _mainViewModel;
    private ListManager _listManager;
    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }

    private ViewModelBase _currentViewModel;
    public ViewModelBase CurrentViewModel
    {
        get { return _currentViewModel; }
        set { _currentViewModel = value; }
    }
    private MjeqViewModel _mjeqViewModel;
  

    public ElectricalViewModel(MainViewModel mainViewModel, ListManager listManager)
    {
        _mainViewModel = mainViewModel;
        _listManager = listManager;


        _dteqFactory = new DteqFactory(listManager);
        _mjeqViewModel = new MjeqViewModel(_listManager);

        //Navigation
        NavigateMjeqCommand = new RelayCommand(NavigateMjeq);

    }

    private void NavigateMjeq()
    {
        _mainViewModel.CurrentViewModel = _mjeqViewModel;
        _mjeqViewModel.CreateComboBoxLists();
        _mjeqViewModel.CreateValidators();
        _mjeqViewModel.DteqGridHeight = AppSettings.Default.DteqGridHeight;
        _mjeqViewModel.LoadGridHeight = AppSettings.Default.LoadGridHeight;
    }


    #endregion

    #region Public Commands

    // Equipment Commands
    public ICommand NavigateMjeqCommand { get; }
    #endregion


}

