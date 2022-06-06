using EDTLibrary;
using EDTLibrary.A_Helpers;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.aMain;
using EDTLibrary.ProjectSettings;
using ExcelLibrary;
using Portable.Licensing;
using Portable.Licensing.Security.Cryptography;
using Portable.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Services;
using WpfUI.Views.SettingsSubViews;

namespace WpfUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
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



        private StartupService _startupService;

        public StartupService StartupService
        {
            get { return _startupService; }
            set { _startupService = value; }
        }


        public readonly HomeViewModel _homeViewModel;
        public readonly SettingsViewModel _settingsViewModel;
        public readonly AreasViewModel _areasViewModel;
        public readonly ElectricalViewModel _electricalViewModel;
        public readonly CableListViewModel _cableListViewModel;
        public readonly DataTablesViewModel _dataTablesViewModel = new DataTablesViewModel();

        public ICommand NavigateGeneralSettingsCommand { get; }
        public ICommand NavigateCableSettingsCommand { get; }

        public MainViewModel(StartupService startupService, ListManager listManager, TypeManager typeManager, EdtSettings edtSettings, string type="")
        {
            ValidateLicense();
           

            _listManager = listManager;
            ScenarioManager.ListManager = _listManager;

            _typeManager = typeManager;
            _startupService = startupService;
            _edtSettings = edtSettings;

            _homeViewModel = new HomeViewModel(startupService, listManager);
            _settingsViewModel = new SettingsViewModel(edtSettings, typeManager);
            _areasViewModel = new AreasViewModel(listManager);
            _electricalViewModel = new ElectricalViewModel(listManager);
            _cableListViewModel = new CableListViewModel(listManager);



            NavigateStartupCommand = new RelayCommand(NavigateStartup);
            NavigateSettingsCommand = new RelayCommand(NavigateSettings, CanExecute_IsProjectLoaded);


            //Settings
            NavigateGeneralSettingsCommand = new RelayCommand(NavigateGeneralSettings, CanExecute_IsProjectLoaded);
            NavigateCableSettingsCommand = new RelayCommand(NavigateCableSettings, CanExecute_IsProjectLoaded);



            NavigateAreasCommand = new RelayCommand(NavigateAreas, CanExecute_IsProjectLoaded);

            NavigateElectricalCommand = new RelayCommand(NavigateEquipment, startupService);

            NavigateCableListCommand = new RelayCommand(NavigateCableList, CanExecute_IsProjectLoaded);
            NavigateDataTablesCommand = new RelayCommand(NavigateDataTables, CanExecute_IsLibraryLoaded);



            ExportCommand = new RelayCommand(ExcelTest);

            ScenarioCommand = new RelayCommand(NewWindow);

            startupService.InitializeLibrary();
            _areasViewModel.CreateComboBoxLists();
            _electricalViewModel.CreateComboBoxLists();

            //TODO - Application Setting for auto load previous project
            if (type == "NewInstance") {
                _startupService.InitializeProject(AppSettings.Default.ProjectDb);
            }

            _startupService.ProjectLoaded += _electricalViewModel.OnProjectLoaded;
        }


        //Settings
        private void NavigateGeneralSettings()
        {
            CurrentViewModel = _settingsViewModel;
            _settingsViewModel.SelectedSettingView = new GeneralSettingsView();
        }
        private void NavigateCableSettings()
        {
            CurrentViewModel = _settingsViewModel;
            _settingsViewModel.SelectedSettingView = new CableSettingsView();
        }
        private void ExcelTest()
        {
            HelperExcelInterop.WriteToExcel();
        }



        #region Navigation
        public ICommand NavigateStartupCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand NavigateAreasCommand { get; }
        public ICommand NavigateElectricalCommand { get; }
        public ICommand NavigateCableListCommand { get; }
        public ICommand NavigateDataTablesCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ScenarioCommand { get; }

        private void NavigateStartup()
        {
            CurrentViewModel = _homeViewModel;
        }
        private void NavigateSettings()
        {
            _settingsViewModel.LoadVmSettings();
            CurrentViewModel = _settingsViewModel;
        }
        private void NavigateAreas()
        {
            CurrentViewModel = _areasViewModel;
            //Todo - Map settings to ViewModel
        }
        private void NavigateEquipment()
        {
            CurrentViewModel = _electricalViewModel;
            _electricalViewModel.CreateValidators();
            _electricalViewModel.DteqGridHeight = AppSettings.Default.DteqGridHeight;
            _electricalViewModel.LoadGridHeight = AppSettings.Default.LoadGridHeight;
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
            newMainVm.CurrentViewModel = CurrentViewModel;
        }

        internal void NewWindow(ViewModelBase viewModel)
        {
            ListManager listManager = new ListManager();
            StartupService startupService = new StartupService(listManager);
            TypeManager typeManager = new TypeManager();

            Window window = new MainWindow() {

                //DataContext = new MainViewModel(startupService, listManager)
                DataContext = new MainViewModel(_startupService, _listManager, typeManager, _edtSettings, "ExtraWindow")

            };
            var newMainVm = (MainViewModel)window.DataContext;
            newMainVm.CurrentViewModel = viewModel;
            window.Show();
            
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
 