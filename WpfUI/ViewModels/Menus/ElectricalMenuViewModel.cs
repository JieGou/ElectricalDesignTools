using EDTLibrary.DataAccess;
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
using WpfUI.Windows;

namespace WpfUI.ViewModels.Menus;

[AddINotifyPropertyChangedInterface]
public class ElectricalMenuViewModel : ViewModelBase, INotifyDataErrorInfo
{

    #region Constructor
    private DteqFactory _dteqFactory;

    private MainViewModel _mainViewModel;
    public MainViewModel MainViewModel
    {
        get { return _mainViewModel; }
        set { _mainViewModel = value; }
    }
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
    private SingleLineViewModel _singleLineViewModel;
    private DistributionPanelsViewModel _distributionPanelsViewModel;


    public ElectricalMenuViewModel(MainViewModel mainViewModel, ListManager listManager)
    {
        _mainViewModel = mainViewModel;
        _listManager = listManager;


        _dteqFactory = new DteqFactory(listManager);

        _mjeqViewModel = new MjeqViewModel(_listManager);
        _singleLineViewModel = new SingleLineViewModel(_listManager);
        _distributionPanelsViewModel = new DistributionPanelsViewModel(_listManager);

        //Navigation
        NavigateMjeqCommand = new RelayCommand(NavigateMjeq);
        NavigateSingleLineCommand = new RelayCommand(NavigateSingleLine);
        NavigateDistributionPanelsCommand = new RelayCommand(NavigateDistributionPanels);

    }

    

    #endregion

    private void NavigateMjeq()
    {
        _mjeqViewModel.CreateValidators();
        _mjeqViewModel.CreateComboBoxLists();
        _mjeqViewModel.DteqGridHeight = AppSettings.Default.DteqGridHeight;
        _mjeqViewModel.LoadGridHeight = AppSettings.Default.LoadGridHeight;

        CurrentViewModel = _mjeqViewModel;
        _mainViewModel.CurrentViewModel = CurrentViewModel;

        
    }

    private void NavigateSingleLine()
    {
        CurrentViewModel = _singleLineViewModel;
        _mainViewModel.CurrentViewModel = CurrentViewModel;
    }

    private void NavigateDistributionPanels()
    {
        CurrentViewModel = _distributionPanelsViewModel;
        _mainViewModel.CurrentViewModel = CurrentViewModel;
    }
    #region Public Commands

    // Equipment Commands
    public ICommand NavigateMjeqCommand { get; }
    public ICommand NavigateSingleLineCommand { get; }
    public ICommand NavigateDistributionPanelsCommand { get; }
    #endregion


}

