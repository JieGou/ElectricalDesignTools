using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Services;

namespace WpfUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ListManager _listManager;
        private StartupService _startupService;


        private readonly StartupViewModel _startupViewModel;
        private readonly ProjectSettingsViewModel _projectSettingsViewModel = new ProjectSettingsViewModel();
        private readonly LocationsViewModel _locationsViewModel;
        private readonly EquipmentViewModel _equipmentViewModel;
        private readonly CableListViewModel _cableListViewModel;
        private readonly DataTablesViewModel _dataTablesViewModel = new DataTablesViewModel();

        public MainViewModel(StartupService startupService, ListManager listManager, string type="")
        {
            _listManager = listManager;
            _startupService = startupService;

            _startupViewModel = new StartupViewModel(startupService);
            _locationsViewModel = new LocationsViewModel(listManager);
            _equipmentViewModel = new EquipmentViewModel(listManager);
            _cableListViewModel = new CableListViewModel(listManager);

            NavigateStartupCommand = new RelayCommand(NavigateStartup);
            NavigateProjectSettingsCommand = new RelayCommand(NavigateProjectSettings, CanExecute_IsProjectLoaded);
            NavigateLocationsCommand = new RelayCommand(NavigateLocations, CanExecute_IsProjectLoaded);

            NavigateEquipmentCommand = new RelayCommand(NavigateEquipment, startupService);

            NavigateCableListCommand = new RelayCommand(NavigateCableList, CanExecute_IsProjectLoaded);
            NavigateDataTablesCommand = new RelayCommand(NavigateDataTables, CanExecute_IsLibraryLoaded);
            ScenarioCommand = new RelayCommand(NewAppInstance);

            startupService.InitializeLibrary();
            _locationsViewModel.CreateComboBoxLists();
            _equipmentViewModel.CreateComboBoxLists();


#if DEBUG
            if (type == "main") {
                _startupService.InitializeProject();
            }
#endif

        }

        

        #region Navigation
        public ICommand NavigateStartupCommand { get; }
        public ICommand NavigateProjectSettingsCommand { get; }
        public ICommand NavigateLocationsCommand { get; }
        public ICommand NavigateEquipmentCommand { get; }
        public ICommand NavigateCableListCommand { get; }
        public ICommand NavigateDataTablesCommand { get; }
        public ICommand ScenarioCommand { get; }

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
            _equipmentViewModel.CreateValidators();
        }
        private void NavigateCableList()
        {
            CurrentViewModel = _cableListViewModel;
        }
        private void NavigateDataTables()
        {
            CurrentViewModel = _dataTablesViewModel;
        }

        private bool CanExecute_IsProjectLoaded()
        {
            return _startupService.IsProjectLoaded;
        }
        private bool CanExecute_IsLibraryLoaded()
        {
            return _startupService.IsLibraryLoaded;
        }

        //NEW WINDOW
        private void NewAppInstance()
        {
            ListManager listManager = new ListManager();
            StartupService startupService = new StartupService(listManager);

            Window scenario = new MainWindow() {
                //DataContext = new MainViewModel(startupService, listManager)
                DataContext = new MainViewModel(_startupService, _listManager)
            };
            scenario.Show();
        }
        #endregion

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { _currentViewModel = value; }
        }

        private void OnCurrentViewModelChanged() {
            OnPropertyChanged(nameof(CurrentViewModel));
        } 
    }
}
 