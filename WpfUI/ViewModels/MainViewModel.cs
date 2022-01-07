using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WpfUI.Services;
using WpfUI.Stores;
using WpfUI.Views;

namespace WpfUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private readonly StartupViewModel _startupViewModel = new StartupViewModel();
        private readonly ProjectSettingsViewModel _projectSettingsViewModel = new ProjectSettingsViewModel();
        private readonly LocationsViewModel _locationsViewModel = new LocationsViewModel();
        private readonly EquipmentViewModel _equipmentViewModel = new EquipmentViewModel();
        private readonly CableListViewModel _cableListViewModel = new CableListViewModel();
        private readonly DataTablesViewModel _dataTablesViewModel = new DataTablesViewModel();

        public MainViewModel()
        {
            NavigateStartupCommand = new RelayCommand(NavigateStartup);
            NavigateProjectSettingsCommand = new RelayCommand(NavigateProjectSettings);
            NavigateLocationsCommand = new RelayCommand(NavigateLocations);
            NavigateEquipmentCommand = new RelayCommand(NavigateEquipment);
            NavigateCableListCommand = new RelayCommand(NavigateCableList);
            NavigateDataTablesCommand = new RelayCommand(NavigateDataTables);

            //Gets data from Project Database
            DataBaseService.InitializeLibrary();
            DataBaseService.InitializeProject();


            //TODO - event on project loaded;
            _locationsViewModel.CreateComboBoxLists();
            _locationsViewModel.LocationList = new ObservableCollection<LocationModel>(DbManager.prjDb.GetRecords<LocationModel>(GlobalConfig.locationTable));

            _equipmentViewModel.DteqList = ListManager.GetDteq();
            _equipmentViewModel.LoadList = new ObservableCollection<LoadModel>(ListManager.GetLoads());
            _equipmentViewModel.CalculateAll();
            _equipmentViewModel.CreateComboBoxLists();
        }

        


        #region Navigation
        public ICommand NavigateStartupCommand { get; }
        public ICommand NavigateProjectSettingsCommand { get; }
        public ICommand NavigateLocationsCommand { get; }
        public ICommand NavigateEquipmentCommand { get; }
        public ICommand NavigateCableListCommand { get; }
        public ICommand NavigateDataTablesCommand { get; }

        private void NavigateStartup()
        {
            CurrentViewModel = _startupViewModel;
        }
        private void NavigateProjectSettings()
        {
            CurrentViewModel = _projectSettingsViewModel;
        }
        private void NavigateLocations()
        {
            CurrentViewModel = _locationsViewModel;
        }
        private void NavigateEquipment()
        {
            
            CurrentViewModel = _equipmentViewModel;
        }
        private void NavigateCableList()
        {
            CurrentViewModel = _cableListViewModel;
        }
        private void NavigateDataTables()
        {
            CurrentViewModel = _dataTablesViewModel;
        }
        #endregion

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { _currentViewModel = value; }
        }

        //private readonly NavigationStore _navigationStore;
        //public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        //public MainViewModel(NavigationStore navigationStore) {
        //    //Initialze Db Connections
        //    DbManager.SetProjectDb(Settings.Default.ProjectDb);
        //    DbManager.SetLibraryDb(Settings.Default.LibraryDb);

        //    _navigationStore = navigationStore;
        //    _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

        //    NavigateProjectSettingsCommand = new RelayCommand(NavigateProjectSettings);

        //}

        private void OnCurrentViewModelChanged() {
            OnPropertyChanged(nameof(CurrentViewModel));
        } 
    }
}
 