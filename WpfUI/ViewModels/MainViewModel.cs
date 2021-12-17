using EDTLibrary.DataAccess;
using System;
using System.Collections.Generic;
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
        private readonly EquipmentViewModel _equipmentViewModel = new EquipmentViewModel();
        private readonly CableListViewModel _cableListViewModel = new CableListViewModel();

        public MainViewModel()
        {
            NavigateStartupCommand = new RelayCommand(NavigateStartup);
            NavigateProjectSettingsCommand = new RelayCommand(NavigateProjectSettings);
            NavigateEquipmentCommand = new RelayCommand(NavigateEquipment);
            NavigateCableListCommand = new RelayCommand(NavigateCableList);
            DataBaseService.Initialize();
        }

        #region Navigation
        public ICommand NavigateStartupCommand { get; }
        public ICommand NavigateProjectSettingsCommand { get; }
        public ICommand NavigateEquipmentCommand { get; }
        public ICommand NavigateCableListCommand { get; }
        private void NavigateStartup()
        {
            CurrentViewModel = _startupViewModel;
        }
        private void NavigateProjectSettings()
        {
            CurrentViewModel = _projectSettingsViewModel;
        }
        private void NavigateEquipment()
        {
            CurrentViewModel = _equipmentViewModel;
        }
        private void NavigateCableList()
        {
            CurrentViewModel = _cableListViewModel;
        }
        #endregion

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { _currentViewModel = value; }
        }

        private UserControl _currentView;
        public UserControl CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; }
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
 