using EdtLibrary.Managers;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Mappers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
using ExcelLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using WpfUI._Authentication;
using WpfUI._Authentication.OfflineLicense;
using WpfUI._Authentication.Stores;
using WpfUI.Commands;
using WpfUI.Helpers;
using WpfUI.PopupWindows;
using WpfUI.Services;
using WpfUI.ViewModels.Cables;
using WpfUI.ViewModels.Electrical;
using WpfUI.ViewModels.Library;
using WpfUI.ViewModels.Menus;
using WpfUI.Views.Settings;

namespace WpfUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private StartupService _startupService;
        public StartupService StartupService
        {
            get { return _startupService; }
            set { _startupService = value; }
        }
        public ListManager _listManager;
        public ListManager ListManager
        {
            get { return _listManager; }
            set
            {
                _listManager = value;
            }
        }
        private TypeManager _typeManager;
        public TypeManager TypeManager
        {
            get { return _typeManager; }
            set { _typeManager = value; }
        }
        private EdtProjectSettings _edtSettings;
        public EdtProjectSettings EdtProjectSettings
        {
            get { return _edtSettings; }
            set { _edtSettings = value; }
        }

        public MainViewModel(AuthenticationStore authenticationStore, StartupService startupService, TypeManager typeManager, EdtProjectSettings edtSettings, string type = "")
        {



            FedFromManager.FedFromUpdated += _ViewStateManager.OnFedFromUpdated;
            LoadManager.LoadDeleted += _ViewStateManager.OnLoadDeleted;
            LoadCircuit.LoadCircuitVoltageChanged += _ViewStateManager.OnLoadCircuitVoltageChanged;

           
            EdtProjectSettings.ProjectNameUpdated += OnProjectNameUpdated;

            _listManager = startupService.ListManager;
            ScenarioManager.ListManager = _listManager;

            AuthenticationStore = authenticationStore;
            _typeManager = typeManager;

            _startupService = startupService;
            _edtSettings = edtSettings;
            InitializeViewModels();

            CurrentViewModel = _homeViewModel;

            NavigateStartupCommand = new RelayCommand(NavigateHome);
            NavigateSettingsCommand = new RelayCommand(NavigateSettings, CanExecute_IsProjectLoaded);


            //Settings
            NavigateGeneralSettingsCommand = new RelayCommand(NavigateGeneralSettings, CanExecute_IsProjectLoaded);
            NavigateCableSettingsCommand = new RelayCommand(NavigateCableSettings, CanExecute_IsProjectLoaded);



            NavigateAreasCommand = new RelayCommand(NavigateAreasSystems, CanExecute_IsProjectLoaded);

            NavigateElectricalCommand = new RelayCommand(NavigateElectical, _startupService);


            NavigateCablesCommand = new RelayCommand(NavigateCables, CanExecute_IsProjectLoaded);

            NavigateLibraryCommand = new RelayCommand(NavigateLibrary, CanExecute_IsLibraryLoaded);


            ExportCommand = new RelayCommand(ExcelTest);

            ScenarioCommand = new RelayCommand(NewWindow);



            ShowUserInfoCommand = new RelayCommand(ShowUserInfo);
            ReloadDbCommand = new RelayCommand(ReloadDb);



            startupService.InitializeLibrary();


            //TODO - Application Setting for auto load previous project
            //if (type == "NewInstance") {
            //    _startupService.InitializeProject(AppSettings.Default.ProjectDb);
            //}
        }



        private void LoadManager_LoadDeleted(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        internal void InitializeViewModels()
        {
            _homeViewModel = new HomeViewModel(this, _startupService, _startupService.ListManager);

            _settingsMenuViewModel = new SettingsMenuViewModel(this, _edtSettings, _typeManager, _startupService.ListManager);

            //Areas & Systems
            _areasMenuViewModel = new AreasMenuViewModel(this, _startupService.ListManager);

            //Electrical
            _electricalMenuViewModel = new ElectricalMenuViewModel(this, _startupService.ListManager);


            //Cables
            _cableMenuViewModel = new CableMenuViewModel(this, _startupService.ListManager);

            //Library
            _libraryMenuViewModel = new LibraryMenuViewModel(this);
        }

        private ViewModelBase _menuViewModel;
        public ViewModelBase MenuViewModel
        {
            get { return _menuViewModel; }
            set { _menuViewModel = value; }
        }

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
            }
        }


        private TagSettings _tagSettings;

        public TagSettings TagSettings
        {
            get { return _tagSettings; }
            set { _tagSettings = value; }
        }

        public string ProjectName { get; set; }
        public void UpdateProjectName()
        {
            if (string.IsNullOrWhiteSpace(EdtProjectSettings.ProjectName)) {
                ProjectName = "Electrical Design Tools";
            }
            else {
                ProjectName = EdtProjectSettings.ProjectName;
            }
        }
        public void OnProjectNameUpdated(object source, EventArgs e)
        {
            UpdateProjectName();
        }




        public HomeViewModel _homeViewModel;
        public SettingsMenuViewModel _settingsMenuViewModel;
        public AreasMenuViewModel _areasMenuViewModel;

        //Electrical
        public ElectricalMenuViewModel _electricalMenuViewModel;
        public LoadListViewModel _mjeqViewModel;

        //Cables
        public CableMenuViewModel _cableMenuViewModel;
        public CableListViewModel _cableListViewModel;

        public LibraryMenuViewModel _libraryMenuViewModel;
        public DataTablesViewModel _dataTablesViewModel;



        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10d);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            NotificationPopup.Close();
        }

        public PopupNotifcationWindow NotificationPopup { get; set; }


        #region Navigation
        public ICommand NavigateStartupCommand { get; }
        private void NavigateHome()
        {
            MenuViewModel = null;
            CurrentViewModel = _homeViewModel;
        }

        //Settings
        public ICommand NavigateSettingsCommand { get; }
        private void NavigateSettings()
        {
            _settingsMenuViewModel.LoadVmSettings();
            MenuViewModel = _settingsMenuViewModel;
            //CurrentViewModel = _settingsViewModel;
        }

        public ICommand NavigateGeneralSettingsCommand { get; }
        private void NavigateGeneralSettings()
        {
            CurrentViewModel = _settingsMenuViewModel;
            _settingsMenuViewModel.SelectedSettingView = new GeneralSettingsView();
        }

        public ICommand NavigateCableSettingsCommand { get; }
        private void NavigateCableSettings()
        {
            CurrentViewModel = _settingsMenuViewModel;
            _settingsMenuViewModel.SelectedSettingView = new CableSettingsView();
        }



        public ICommand NavigateAreasCommand { get; }
        private void NavigateAreasSystems()
        {
            MenuViewModel = _areasMenuViewModel;
        }


        public ICommand NavigateElectricalCommand { get; }
        private void NavigateElectical()
        {
            MenuViewModel = _electricalMenuViewModel;


            //Below is For Ribbon Window

            //CurrentViewModel = MenuViewModel.CurrentViewModel;
            //    _mjeqViewModel.CreateValidators();
            //    _mjeqViewModel.CreateComboBoxLists();
            //    _mjeqViewModel.DteqGridHeight = AppSettings.Default.DteqGridHeight;
            //    _mjeqViewModel.LoadGridHeight = AppSettings.Default.LoadGridHeight;

        }



        public ICommand NavigateCablesCommand { get; }
        private void NavigateCables()
        {
            MenuViewModel = _cableMenuViewModel;
        }


        public ICommand NavigateLibraryCommand { get; }
        private void NavigateLibrary()
        {
            MenuViewModel = _libraryMenuViewModel;
        }


        public ICommand ExportCommand { get; }
        private void ExcelTest()
        {
            NotificationPopup = new PopupNotifcationWindow();
            NotificationPopup.DataContext = new PopupNotificationModel("Exporting to Excel");
            NotificationPopup.Show();
            StartCloseTimer();
            ExcelTestAsync();
        }

        private async Task ExcelTestAsync()
        {
            await Task.Run(() => {
                ExcelHelper excel = new ExcelHelper(visible: false);
                try {

                    excel.Initialize();

                    LoadMapper load;
                    excel.AddSheet("Load List");
                    List<LoadMapper> loadList = new List<LoadMapper>();
                    foreach (var item in ListManager.LoadList) {
                        loadList.Add(new LoadMapper((LoadModel)item));
                    }

                    var exportManager = new ExportPropertyManager();

                    excel.ExportListOfObjects<LoadMapper>(loadList, exportManager.GetPropertyList("Load List"));


                    DteqMapper dteq;
                    excel.AddSheet("Distribution Equipment List");
                    List<DteqMapper> dteqList = new List<DteqMapper>();
                    foreach (var item in ListManager.IDteqList) {
                        dteqList.Add(new DteqMapper((DistributionEquipment)item));
                    }
                    excel.ExportListOfObjects<DteqMapper>(dteqList, exportManager.GetPropertyList("Distribution Equipment List"));

                    CableMapper cable;
                    excel.AddSheet("Cable List");
                    List<CableMapper> cableList = new List<CableMapper>();
                    foreach (var item in ListManager.CableList) {
                        cableList.Add(new CableMapper((CableModel)item));
                    }
                    excel.ExportListOfObjects<CableMapper>(cableList, exportManager.GetPropertyList("Cable List"));


                    excel.SaveWorkbook();
                    excel.SetVisibility(true);
                    excel.Release();
                    System.Windows.Application.Current.Dispatcher.Invoke(NotificationPopup.Close);
                }
                catch (Exception) {

                    excel.Release();
                }
            });
        }

        public ICommand ScenarioCommand { get; }
        public void NewWindow()
        {
            IntPtr active = GetActiveWindow();
            Window activeWindow = System.Windows.Application.Current.Windows.OfType<Window>()
                .SingleOrDefault(window => new WindowInteropHelper(window).Handle == active);

            WindowController.SnapWindow(activeWindow, true);

            TypeManager typeManager = new TypeManager();

            Window newWindow = new MainWindow() {
                //DataContext = new MainViewModel(startupService, listManager)
                DataContext = new MainViewModel(AuthenticationStore, _startupService, typeManager, _edtSettings, "ExtraWindow")

            };

            var newMainVm = (MainViewModel)newWindow.DataContext;
            newMainVm.MenuViewModel = MenuViewModel;
            newMainVm.CurrentViewModel = null;
            newWindow.Show();
            WindowController.SnapWindow(newWindow, false);

        }


        public AuthenticationStore AuthenticationStore { get; }
        public ICommand ShowUserInfoCommand { get; }
        private void ShowUserInfo()
        {
            var userInfoWindow = new UserInfoWindow();
            userInfoWindow.DataContext = AuthenticationStore;
            userInfoWindow.ShowDialog();
        }

        public ICommand ReloadDbCommand { get; }
        private void ReloadDb()
        {

            _listManager.GetProjectTablesAndAssigments();


            //if (_mjeqViewModel.AssignedLoads != null) {
            //    _mjeqViewModel.AssignedLoads.Clear();
            //    _mjeqViewModel.DteqToAddValidator.ClearErrors();
            //    _mjeqViewModel.LoadToAddValidator.ClearErrors();
            //}

            //if (_singleLineViewModel.AssignedLoads != null) {
            //    _singleLineViewModel.AssignedLoads.Clear();
            //    _singleLineViewModel.RefreshSingleLine();
            //    _singleLineViewModel.ClearSelections();
            //    _singleLineViewModel.DteqCollectionView = new ListCollectionView(_singleLineViewModel.ViewableDteqList);
            //}

            //if (_dpanelViewModel.SelectedDteq != null) {
            //    _dpanelViewModel.UpdatePanelList();
            //}


            _ViewStateManager.OnElectricalViewUpdated();
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

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();




        internal void NewWindow(ViewModelBase menuToSet, ViewModelBase viewModelToSet)
        {
            IntPtr active = GetActiveWindow();
            Window activeWindow = System.Windows.Application.Current.Windows.OfType<Window>()
                .SingleOrDefault(window => new WindowInteropHelper(window).Handle == active);

            WindowController.SnapWindow(activeWindow, true);

            TypeManager typeManager = new TypeManager();

            Window newWindow = new MainWindow() {

                //DataContext = new MainViewModel(startupService, listManager)
                DataContext = new MainViewModel(AuthenticationStore, _startupService, typeManager, _edtSettings, "ExtraWindow")

            };

            if (viewModelToSet.GetType().ToString().Contains("Mjeq")) {
                viewModelToSet = new LoadListViewModel(_listManager);
            }
            var newMainVm = (MainViewModel)newWindow.DataContext;
            menuToSet.MainViewModel = newMainVm;
            newMainVm.MenuViewModel = menuToSet;
            newMainVm.CurrentViewModel = viewModelToSet;
            newMainVm.UpdateProjectName();
            newWindow.Show();
            WindowController.SnapWindow(newWindow, false);

        }
        #endregion


        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

    }

        
}
 