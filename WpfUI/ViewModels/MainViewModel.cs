using EDTLibrary;
using EDTLibrary.A_Helpers;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using Microsoft.Win32;
using Portable.Licensing;
using Portable.Licensing.Security.Cryptography;
using Portable.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Windows.Storage.Streams;
using Windows.System.Profile;
using WpfUI.Commands;
using WpfUI.Services;

namespace WpfUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ListManager _listManager;

        public ListManager ListManager
        {
            get { return _listManager; }
            set { _listManager = value; }
        }

        private StartupService _startupService;

        private readonly StartupViewModel _startupViewModel;
        private readonly ProjectSettingsViewModel _projectSettingsViewModel = new ProjectSettingsViewModel();
        private AreasViewModel _areasViewModel;
        private readonly EquipmentViewModel _equipmentViewModel;
        private readonly CableListViewModel _cableListViewModel;
        private readonly DataTablesViewModel _dataTablesViewModel = new DataTablesViewModel();

        private TypeManager _typeManager;

        public TypeManager TypeManager
        {
            get { return _typeManager; }
            set { _typeManager = value; }
        }


        public MainViewModel(StartupService startupService, ListManager listManager, TypeManager typeManager, string type="")
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
                                                                                                    new GeneralValidationFailure() { Message = "The license file is not registered for this machine. This can be caused If you changed your computer name or re-installed Windows.", 
                                                                                                    HowToResolve = "Contact administrator" })
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

            _listManager = listManager;
            _typeManager = typeManager;
            _startupService = startupService;

            _startupViewModel = new StartupViewModel(startupService);
            _areasViewModel = new AreasViewModel(listManager);
            _equipmentViewModel = new EquipmentViewModel(listManager);
            _cableListViewModel = new CableListViewModel(listManager);

            NavigateStartupCommand = new RelayCommand(NavigateStartup);
            NavigateProjectSettingsCommand = new RelayCommand(NavigateProjectSettings, CanExecute_IsProjectLoaded);
            NavigateAreasCommand = new RelayCommand(NavigateAreas, CanExecute_IsProjectLoaded);

            NavigateEquipmentCommand = new RelayCommand(NavigateEquipment, startupService);

            NavigateCableListCommand = new RelayCommand(NavigateCableList, CanExecute_IsProjectLoaded);
            NavigateDataTablesCommand = new RelayCommand(NavigateDataTables, CanExecute_IsLibraryLoaded);
            ScenarioCommand = new RelayCommand(NewWindow);

            startupService.InitializeLibrary();
            _areasViewModel.CreateComboBoxLists();
            _equipmentViewModel.CreateComboBoxLists();


#if DEBUG
            if (type == "dev") {
                _startupService.InitializeProject(AppSettings.Default.ProjectDb);
            }
#endif

        }

        

#region Navigation
        public ICommand NavigateStartupCommand { get; }
        public ICommand NavigateProjectSettingsCommand { get; }
        public ICommand NavigateAreasCommand { get; }
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
        private void NavigateAreas()
        {
            CurrentViewModel = _areasViewModel;
        }
        private void NavigateEquipment()
        {
            CurrentViewModel = _equipmentViewModel;
            _equipmentViewModel.CreateValidators();

            _equipmentViewModel.DteqGridHeight = AppSettings.Default.DteqGridHeight;
            _equipmentViewModel.LoadGridHeight = AppSettings.Default.LoadGridHeight;
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
        private void NewWindow()
        {
            ListManager listManager = new ListManager();
            StartupService startupService = new StartupService(listManager);
            TypeManager typeManager = new TypeManager();

            Window scenario = new MainWindow() {
                //DataContext = new MainViewModel(startupService, listManager)
                DataContext = new MainViewModel(_startupService, _listManager, typeManager)
                
            };
            
            scenario.Show();
            var newMainVm = (MainViewModel)scenario.DataContext;
            newMainVm.CurrentViewModel = CurrentViewModel;
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
 