using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using EDTLibrary.Models.Loads;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WpfUI.Services;

namespace WpfUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ListManager _listManager;

        private readonly StartupViewModel _startupViewModel = new StartupViewModel();
        private readonly ProjectSettingsViewModel _projectSettingsViewModel = new ProjectSettingsViewModel();
        private readonly LocationsViewModel _locationsViewModel;
        private readonly EquipmentViewModel _equipmentViewModel;
        private readonly CableListViewModel _cableListViewModel = new CableListViewModel();
        private readonly DataTablesViewModel _dataTablesViewModel = new DataTablesViewModel();

        private readonly ObservableCollection<object> _viewModels;
        public MainViewModel(ListManager listManager)
        {
            _listManager = listManager;

            _locationsViewModel = new LocationsViewModel(listManager);
            _equipmentViewModel = new EquipmentViewModel(listManager);

            NavigateStartupCommand = new RelayCommand(NavigateStartup);
            NavigateProjectSettingsCommand = new RelayCommand(NavigateProjectSettings);
            NavigateLocationsCommand = new RelayCommand(NavigateLocations);
            NavigateEquipmentCommand = new RelayCommand(NavigateEquipment);
            NavigateCableListCommand = new RelayCommand(NavigateCableList);
            NavigateDataTablesCommand = new RelayCommand(NavigateDataTables);
            ScenarioCommand = new RelayCommand(StartScenarioManager);

            DataBaseService.InitializeLibrary();
            _locationsViewModel.CreateComboBoxLists();
            _equipmentViewModel.CreateComboBoxLists();


#if DEBUG
             DataBaseService.InitializeProject();

            //TODO - event on project loaded;
            _locationsViewModel.ListManager.LocationList = new ObservableCollection<LocationModel>(DbManager.prjDb.GetRecords<LocationModel>(GlobalConfig.LocationTable));

            GlobalConfig.GettingRecords = true;
            _equipmentViewModel.DteqList = ListManager.GetDteq();
            _equipmentViewModel.LoadList = new ObservableCollection<LoadModel>(ListManager.GetLoads());
            GlobalConfig.GettingRecords = false;

            //_equipmentViewModel.CalculateAll();
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
        }
        private void NavigateCableList()
        {
            CurrentViewModel = _cableListViewModel;
        }
        private void NavigateDataTables()
        {
            CurrentViewModel = _dataTablesViewModel;
        }
        private void StartScenarioManager()
        {
            ListManager listManager = new ListManager();

            Window scenario = new MainWindow() {
                DataContext = new MainViewModel(_listManager)
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
 