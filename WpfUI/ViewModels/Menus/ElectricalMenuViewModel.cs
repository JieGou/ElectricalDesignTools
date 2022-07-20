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


    public ElectricalMenuViewModel(MainViewModel mainViewModel, ListManager listManager)
    {
        _mainViewModel = mainViewModel;
        _listManager = listManager;


        _dteqFactory = new DteqFactory(listManager);
        _mjeqViewModel = new MjeqViewModel(_listManager);

        //Navigation
        NavigateMjeqCommand = new RelayCommand(NavigateMjeq);

    }

    #endregion

    TestWindow testWindow = null;
    private void NavigateMjeq()
    {
        _mjeqViewModel.CreateValidators();
        _mjeqViewModel.CreateComboBoxLists();
        _mjeqViewModel.DteqGridHeight = AppSettings.Default.DteqGridHeight;
        _mjeqViewModel.LoadGridHeight = AppSettings.Default.LoadGridHeight;

        CurrentViewModel = _mjeqViewModel;
        _mainViewModel.CurrentViewModel = CurrentViewModel;

        if (testWindow == null ) {
            testWindow = new TestWindow();
            testWindow.DataContext = _mjeqViewModel;
            testWindow.Show();
        }
        else if (testWindow.IsLoaded == false) {
            testWindow = new TestWindow();
            testWindow.DataContext = _mjeqViewModel;
            testWindow.Show();
        } 
            
        
    }


    #region Public Commands

    // Equipment Commands
    public ICommand NavigateMjeqCommand { get; }
    #endregion


}

