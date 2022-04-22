using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.Cables;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Services;
using WpfUI.Stores;

namespace WpfUI.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {

        #region Properties and Backing Fields

        public ObservableCollection<SettingModel> StringSettings { get; set; }
        public SettingModel SelectedStringSetting { get; set; }

        public ObservableCollection<CableTypeModel> CableTypes { get; set; }
        public ObservableCollection<CableSizeModel> SelectedCableSizes { get; set; } = new ObservableCollection<CableSizeModel>();

        private Settings_GeneralViewModel _generalSettings = new Settings_GeneralViewModel();

        private ViewModelBase _currentSettingsVm;
        public ViewModelBase CurrentSettingsVm

        {
            get { return _currentSettingsVm; }
            set { _currentSettingsVm = value; }
        }
        private EdtSettings _edtSettings;
        public EdtSettings EdtSettings
        {
            get { return _edtSettings; }
            set { _edtSettings = value; }
        }


        public ObservableCollection<SettingModel> TableSettings { get; set; }
        public SettingModel SelectedTableSetting { get; set; }

        #endregion

        #region Commands
        public ICommand TestCommand { get; }


        public ICommand NavigateStartupCommand { get; }
        public ICommand OpenProjectCommand { get; }

        public ICommand SelectProjectCommand { get; }
        public ICommand ReloadSettingsCommand { get; }

        public ICommand SaveStringSettingCommand { get; }
        public ICommand SaveTableSettingCommand { get; }

        #endregion

        public SettingsViewModel(EdtSettings edtSettings)
        {
            _edtSettings = edtSettings;
            // Create commands

            ReloadSettingsCommand = new RelayCommand(LoadSettings);

            SaveStringSettingCommand = new RelayCommand(SaveStringSetting);
            SaveTableSettingCommand = new RelayCommand(SaveTableSetting);

            CableTypes = TypeManager.CableTypes;
        }

        private string projectName;
        public string ProjectName 
        {
            get => projectName;
            set {
                var oldValue = projectName;
                projectName = value;
                ClearErrors(nameof(ProjectName));
                bool isValid = Regex.IsMatch(projectName, @"^[\sA-Z0-9_@#$%^&*()-]+$", RegexOptions.IgnoreCase);
               if (isValid==false) {
                    AddError(nameof(ProjectName), "Invalid character");
                }
                EdtSettings.ProjectName = projectName;
                SettingsManager.SaveStringSettingToDb(nameof(ProjectName), projectName);
            }
        }
        public string ProjectNumber { get; set; }
        public string ProjectTitleLine1 { get; set; }
        public string ProjectTitleLine2 { get; set; }
        public string ProjectTitleLine3 { get; set; }

        public string ClientNameLine1 { get; set; }
        public string ClientNameLine2 { get; set; }
        public string ClientNameLine3 { get; set; }





        public string Code { get; set; }

        //Cable
        public ObservableCollection<CableSizeModel> CableSizesUsedInProject { get; set; }

        public string CableType3C1kVPower { get; set; }
        public string CableInsulation1kVPower { get; set; }
        public string CableInsulation5kVPower { get; set; }
        public string CableInsulation15kVPower { get; set; }
        public string CableInsulation35kVPower { get; set; }

        public string DefaultCableInstallationType { get; set; }
        public string DefaultCableTypeLoad_3ph1kV { get; set; }
        public string DefaultCableTypeDteq_3ph1kVLt1200A { get; set; }
        public string DefaultCableTypeDteq_3ph1kVGt1200A { get; set; }
        public string DefaultCableType_3ph5kV { get; set; }
        public string DefaultCableType_3ph15kV { get; set; }

        private string _cableSpacingMaxAmps_3C1kV;

        public string CableSpacingMaxAmps_3C1kV
        {
            get { return _cableSpacingMaxAmps_3C1kV; }
            set
            {
                var oldValue = _cableSpacingMaxAmps_3C1kV;
                double dblOut;
                _cableSpacingMaxAmps_3C1kV = value;
                ClearErrors(nameof(CableSpacingMaxAmps_3C1kV));

                if (Double.TryParse(_cableSpacingMaxAmps_3C1kV, out dblOut) == false) {
                    AddError(nameof(CableSpacingMaxAmps_3C1kV), "Invalid Value");
                }
                else {
                    EdtSettings.CableSpacingMaxAmps_3C1kV = _cableSpacingMaxAmps_3C1kV;
                    SettingsManager.SaveStringSettingToDb(nameof(CableSpacingMaxAmps_3C1kV), _cableSpacingMaxAmps_3C1kV);
                }
            }
        }


        //Dteq
        public string DteqMaxPercentLoaded { get; set; }
        public string DteqDefaultPdTypeLV { get; set; }

        public string LoadDefaultPdTypeLV { get; set; }
        public string LoadFactorDefault { get; set; }
        public string DefaultXfrImpedance { get; set; }

        //Voltage
        public string VoltageDefault1kV { get; set; }


        //Loads
        public string LoadDefaultEfficiency_Other { get; set; }
        public string LoadDefaultPowerFactor_Other { get; set; }
        public string LoadDefaultEfficiency_Panel { get; set; }
        public string LoadDefaultPowerFactor_Panel { get; set; }


        #region Helper Methods
        public void LoadSettings()
        {
            SettingsManager.LoadProjectSettings();
            StringSettings = new ObservableCollection<SettingModel>(SettingsManager.StringSettingList);
            TableSettings = new ObservableCollection<SettingModel>(SettingsManager.TableSettingList);

            //StringSettings = new ObservableCollection<SettingModel>(SettingManager.GetStringSettings());
            //TableSettings = new ObservableCollection<SettingModel>(SettingManager.GetTableSettings());
            //SettingManager.CreateCableAmpsUsedInProject();
            CableTypes = TypeManager.CableTypes;


        }
        private void SaveStringSetting()
        {
            SettingsManager.SaveStringSetting(SelectedStringSetting);
        }
        private void SaveTableSetting()
        {
            ArrayList tables = DaManager.prjDb.GetListOfTablesNamesInDb();

            //only saves to Db if Db table exists

            if (SelectedTableSetting != null) {
                if (tables.Contains(SelectedTableSetting.Name)) {
                    SettingsManager.SaveTableSetting(SelectedTableSetting);
                    //SettingManager.CreateCableAmpsUsedInProject();
                }
            }


            foreach (CableSizeModel cable in SelectedCableSizes) {
                DaManager.prjDb.UpsertRecord(cable, "CableSizesUsedInProject", new List<string>());
            }
            EdtSettings.CableSizesUsedInProject = DaManager.prjDb.GetRecords<CableSizeModel>("CableSizesUsedInProject");
        }
        #endregion

        public void LoadVmSettings()
        {
            Type projectSettingsClass = typeof(EdtSettings);
            Type viewModelSettings = typeof(SettingsViewModel);

            foreach (var classProperty in projectSettingsClass.GetProperties()) {
                foreach (var viewModelProperty in viewModelSettings.GetProperties()) {
                    if (classProperty.Name == viewModelProperty.Name) {
                        viewModelProperty.SetValue(this, classProperty.GetValue(null));
                        break;
                    }
                }
            }
        }

    }
}
