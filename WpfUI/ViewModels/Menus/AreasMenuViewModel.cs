using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfUI.Commands;
using WpfUI.Helpers;
using WpfUI.ViewModels.Areas_and_Systems;

namespace WpfUI.ViewModels.Menus
{
    [AddINotifyPropertyChangedInterface]
    public class AreasMenuViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        public ObservableCollection<string> NemaTypes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Categories { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AreaClassifications { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<AreaClassificationType> AreaClassificationsInfoTableItems { get; set; } = new ObservableCollection<AreaClassificationType>();


        private AreasViewModel _areasViewModel;


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

        //CONSTRUCTOR
        public AreasMenuViewModel(MainViewModel mainViewModel, ListManager listManager)
        {
            _listManager = listManager;
            _mainViewModel = mainViewModel;

            _areasViewModel = new AreasViewModel(listManager);
            NavigateAreasCommand = new RelayCommand(NavigateAreas);
            NavigateSystemsCommand = new RelayCommand(NavigateAreas);
        }

        private void NavigateAreas()
        {
            _areasViewModel.CreateComboBoxLists();
            CurrentViewModel = _areasViewModel;
            _mainViewModel.CurrentViewModel = CurrentViewModel;

        }



        #region Public Commands

        // Equipment Commands
        public ICommand NavigateAreasCommand { get; }
        public ICommand NavigateSystemsCommand { get; }
        #endregion

    }

}