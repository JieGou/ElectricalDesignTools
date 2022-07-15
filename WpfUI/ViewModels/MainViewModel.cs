﻿using EDTLibrary;
using EDTLibrary.A_Helpers;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Mappers;
using EDTLibrary.Models.aMain;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using ExcelLibrary;
using Portable.Licensing;
using Portable.Licensing.Security.Cryptography;
using Portable.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfUI.Commands;
using WpfUI.PopupWindows;
using WpfUI.Services;
using WpfUI.ViewModels.Cables;
using WpfUI.ViewModels.Electrical;
using WpfUI.Views.Settings;

namespace WpfUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
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
            set { _currentViewModel = value; }
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

        private EdtSettings _edtSettings;

        public EdtSettings EdtSettings
        {
            get { return _edtSettings; }
            set { _edtSettings = value; }
        }

        public string ProjectName { get; set; }
        public void UpdateProjectName()
        {
            if (string.IsNullOrWhiteSpace(EdtSettings.ProjectName)) {
                ProjectName = "Electrical Design Tools";
            }
            else {
                ProjectName = "Electrical Design Tools - " + EdtSettings.ProjectName;
            }
        }
        public  void OnProjectNameUpdated(object source, EventArgs e)
        {
            UpdateProjectName();
        }
        private StartupService _startupService;

        public StartupService StartupService
        {
            get { return _startupService; }
            set { _startupService = value; }
        }



        public readonly HomeViewModel _homeViewModel;
        public readonly SettingsMenuViewModel _settingsMenuViewModel;
        public readonly AreasMenuViewModel _areasMenuViewModel;

        //Electrical
        public readonly ElectricalMenuViewModel _electricalMenuViewModel;
        public readonly MjeqViewModel _mjeqViewModel;


        public readonly CableMenuViewModel _cableMenuViewModel;
        public readonly CableListViewModel _cableListViewModel;

        public readonly DataTablesViewModel _dataTablesViewModel = new DataTablesViewModel();

      

        public MainViewModel(StartupService startupService, ListManager listManager, TypeManager typeManager, EdtSettings edtSettings, string type="")
        {
            ValidateLicense();
            EdtSettings.ProjectNameUpdated += OnProjectNameUpdated;

            _listManager = listManager;
            ScenarioManager.ListManager = _listManager;

            _typeManager = typeManager;
            _startupService = startupService;
            _edtSettings = edtSettings;

            _homeViewModel = new HomeViewModel(this, startupService, listManager);
            _settingsMenuViewModel = new SettingsMenuViewModel(this, edtSettings, typeManager);

            //Areas & Systems
            _areasMenuViewModel = new AreasMenuViewModel(this, listManager);

            //Electrical
            _electricalMenuViewModel = new ElectricalMenuViewModel(this, listManager);
            _mjeqViewModel = new MjeqViewModel(listManager);

            //Cables
            _cableMenuViewModel = new CableMenuViewModel(this, listManager);
            _cableListViewModel = new CableListViewModel(listManager);


            NavigateStartupCommand = new RelayCommand(NavigateHome);
            NavigateSettingsCommand = new RelayCommand(NavigateSettings, CanExecute_IsProjectLoaded);


            //Settings
            NavigateGeneralSettingsCommand = new RelayCommand(NavigateGeneralSettings, CanExecute_IsProjectLoaded);
            NavigateCableSettingsCommand = new RelayCommand(NavigateCableSettings, CanExecute_IsProjectLoaded);



            NavigateAreasCommand = new RelayCommand(NavigateAreasSystems, CanExecute_IsProjectLoaded);

            NavigateElectricalCommand = new RelayCommand(NavigateElectical, startupService);


            NavigateCablesCommand = new RelayCommand(NavigateCables, CanExecute_IsProjectLoaded);
            NavigateDataTablesCommand = new RelayCommand(NavigateDataTables, CanExecute_IsLibraryLoaded);



            ExportCommand = new RelayCommand(ExcelTest);

            ScenarioCommand = new RelayCommand(NewWindow);

            startupService.InitializeLibrary();
            

            //TODO - Application Setting for auto load previous project
            if (type == "NewInstance") {
                _startupService.InitializeProject(AppSettings.Default.ProjectDb);
            }

            //_startupService.ProjectLoaded += _electricalViewModel.OnProjectLoaded;
        }







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

        public NotificationPopup NotificationPopup { get; set; }
        private void ExcelTest()
        {
            NotificationPopup = new NotificationPopup();
            NotificationPopup.DataContext = new Notification("Exporting to Excel");
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
                    Application.Current.Dispatcher.Invoke(NotificationPopup.Close);
                }
                catch (Exception) {

                    excel.Release();
                }
            });
        }

        #region Navigation
        public ICommand NavigateStartupCommand { get; }

        //Settings
        public ICommand NavigateSettingsCommand { get; }
        public ICommand NavigateGeneralSettingsCommand { get; }
        public ICommand NavigateCableSettingsCommand { get; }

        //Area & Systems
        public ICommand NavigateAreasCommand { get; }

        //Electrical
        public ICommand NavigateElectricalCommand { get; }

        //Cables
        public ICommand NavigateCablesCommand { get; }

        public ICommand NavigateDataTablesCommand { get; }

        public ICommand ExportCommand { get; }

        public ICommand ScenarioCommand { get; }

        //HOME
        private void NavigateHome()
        {
            MenuViewModel = null;
            CurrentViewModel = _homeViewModel;
        }


        //SETTINGS
        private void NavigateSettings()
        {
            _settingsMenuViewModel.LoadVmSettings();
            MenuViewModel = _settingsMenuViewModel;
            //CurrentViewModel = _settingsViewModel;
        }
        private void NavigateGeneralSettings()
        {
            CurrentViewModel = _settingsMenuViewModel;
            _settingsMenuViewModel.SelectedSettingView = new GeneralSettingsView();
        }
        private void NavigateCableSettings()
        {
            CurrentViewModel = _settingsMenuViewModel;
            _settingsMenuViewModel.SelectedSettingView = new CableSettingsView();
        }



        private void NavigateAreasSystems()
        {
            MenuViewModel = _areasMenuViewModel;
        }
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

        private void NavigateCables()
        {
            MenuViewModel = _cableMenuViewModel;
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
        public void NewWindow()
        {
            ListManager listManager = new ListManager();
            StartupService startupService = new StartupService(listManager);
            TypeManager typeManager = new TypeManager();

            Window window = new MainWindow() {
                //DataContext = new MainViewModel(startupService, listManager)
                DataContext = new MainViewModel(_startupService, _listManager, typeManager, _edtSettings, "ExtraWindow")
                
            };
            
            window.Show();
            var newMainVm = (MainViewModel)window.DataContext;
            newMainVm.MenuViewModel = MenuViewModel;
            newMainVm.CurrentViewModel = CurrentViewModel;
        }

        internal void NewWindow(ViewModelBase CurrentVm)
        {
            ListManager listManager = new ListManager();
            StartupService startupService = new StartupService(listManager);
            TypeManager typeManager = new TypeManager();

            Window window = new MainWindow() {

                //DataContext = new MainViewModel(startupService, listManager)
                DataContext = new MainViewModel(_startupService, _listManager, typeManager, _edtSettings, "ExtraWindow")

            };
            var newMainVm = (MainViewModel)window.DataContext;
            newMainVm.MenuViewModel= MenuViewModel;
            newMainVm.CurrentViewModel = CurrentVm;
            window.Show();
            
        }
        #endregion

       
        private void OnCurrentViewModelChanged() {
            OnPropertyChanged(nameof(CurrentViewModel));
        }


        private static void ValidateLicense()
        {
            string licenseFilePath = @"C:/temp/License.lic";
            FileInfo licenseFile = new FileInfo(licenseFilePath);

            string privateKeyFilePath = @"C:/temp/PrivateKey.text";
            FileInfo privateKeyFile = new FileInfo(privateKeyFilePath);

            string publicKeyFilePath = @"C:/temp/PublicKey.text";
            FileInfo publicKeyFile = new FileInfo(publicKeyFilePath);



            KeyGenerator keyGenerator;
            KeyPair keyPair;
            string passPhrase = "1048Mandrake!@#";
            string privateKey = "";
            string publicKey = "";

            License license = null;

            try {

                if (licenseFile.Exists) {
                    license = License.Load(File.OpenText(licenseFilePath));
                }
                if (privateKeyFile.Exists) {
                    privateKey = File.ReadAllText(privateKeyFilePath);
                }
                if (publicKeyFile.Exists) {
                    publicKey = File.ReadAllText(publicKeyFilePath);
                }


                keyGenerator = KeyGenerator.Create();
                keyPair = keyGenerator.GenerateKeyPair();

                if (File.Exists(privateKeyFilePath) == false) {
                    privateKey = keyPair.ToEncryptedPrivateKeyString(passPhrase);
                    File.WriteAllText(privateKeyFilePath, privateKey.ToString(), Encoding.UTF8);
                }

                if (File.Exists(publicKeyFilePath) == false) {
                    publicKey = keyPair.ToPublicKeyString();
                    File.WriteAllText(publicKeyFilePath, publicKey.ToString(), Encoding.UTF8);
                }

                if (licenseFile.Exists == false) {
                    string ComputerGuid = ComputerInfo.GetComputerGuid();



                    license = License.New().WithUniqueIdentifier(Guid.NewGuid())
                                            .As(LicenseType.Trial)
                                            .ExpiresAt(DateTime.Now.AddDays(30))
                                            .WithMaximumUtilization(30)
                                            .WithProductFeatures(new Dictionary<string, string>
                                                                          {
                                                                          {"Sales Module", "yes"},
                                                                          {"Purchase Module", "yes"},
                                                                          {"Maximum Transactions", "10000"}
                                                                          })
                                            .WithAdditionalAttributes(new Dictionary<string, string> {
                                                                          {"ComputerName", Environment.MachineName.ToString()},
                                                                          {"ComputerGuid", ComputerGuid},
                                                                          })
                                            .LicensedTo("John Doe", "john.doe@yourmail.here")
                                            .CreateAndSignWithPrivateKey(File.ReadAllText(privateKeyFilePath), passPhrase);
                    licenseFile.Directory.Create();
                    File.WriteAllText(licenseFilePath, license.ToString(), Encoding.UTF8);
                }

                license = License.Load(File.OpenText(licenseFilePath));
                var validationFailures = license.Validate()
                                                .ExpirationDate()
                                                .When(lic => lic.Type == LicenseType.Trial)
                                                .And()
                                                .Signature(publicKey)
                                                .And()
                                                .AssertThat(lic => lic.AdditionalAttributes.Get("ComputerName") == Environment.MachineName.ToString(),
                                                                                                    new GeneralValidationFailure() {
                                                                                                        Message = "The license file is not registered for this machine. This can be caused If you changed your computer name or re-installed Windows.",
                                                                                                        HowToResolve = "Contact administrator"
                                                                                                    })
                                                .And()
                                                .AssertThat(lic => lic.AdditionalAttributes.Get("ComputerGuid") == ComputerInfo.GetComputerGuid(),
                                                                                                    new GeneralValidationFailure() {
                                                                                                        Message = "The license file is not registered to this machine.",
                                                                                                        HowToResolve = "Contact administrator"
                                                                                                    })
                                                .AssertValidLicense();
#if !DEBUG
                foreach (var failure in validationFailures) {
                    if (failure.GetType().Name.Contains("ValidationFailure")) {
                        MessageBox.Show(failure.Message + " \n\n" + "Pleaes contact DCS.Inc to resolve.", "EDT - License Validation Failure");
                    }
                }
#endif
            }
            catch (Exception ex) {
                MessageBox.Show("Public Key or License file is corrupt or has been modified." + "\n\n" + ex.Message, "EDT - License Validation Failure");
            }
        }

    }
}
 