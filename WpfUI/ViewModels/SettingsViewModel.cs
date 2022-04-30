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
using System.Windows.Controls;
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


        private EdtSettings _edtSettings;
        public EdtSettings EdtSettings
        {
            get { return _edtSettings; }
            set { _edtSettings = value; }
        }
        private TypeManager _typeManager;
        public TypeManager TypeManager
        {
            get { return _typeManager; }
            set { _typeManager = value; }
        }


        public UserControl SelectedSettingView { get; set; }
        public ObservableCollection<SettingModel> TableSettings { get; set; }
        public SettingModel SelectedTableSetting { get; set; }

        #endregion

        #region Commands

        public ICommand SelectProjectCommand { get; }
        public ICommand ReloadSettingsCommand { get; }

        public ICommand SaveStringSettingCommand { get; }
        public ICommand SaveTableSettingCommand { get; }

        #endregion

        public SettingsViewModel(EdtSettings edtSettings, TypeManager typeManager)
        {
            _edtSettings = edtSettings;
            _typeManager = typeManager;
            // Create commands

            ReloadSettingsCommand = new RelayCommand(LoadSettings);

            SaveStringSettingCommand = new RelayCommand(SaveStringSetting);
            SaveTableSettingCommand = new RelayCommand(SaveTableSettings);

        }

        //General

        private string _projectName;
        public string ProjectName
        {
            get => _projectName;
            set
            {
                var oldValue = _projectName;
                _projectName = value;
                ClearErrors(nameof(ProjectName));
                bool isValid = Regex.IsMatch(_projectName, @"^[\sA-Z0-9_@#$%^&*()-]+$", RegexOptions.IgnoreCase);
                if (isValid == false) {
                    AddError(nameof(ProjectName), "Invalid character");
                }
                SaveVmSetting(nameof(ProjectName), _projectName);
            }
        }
        private string _projectNumber;
        public string ProjectNumber

        {
            get { return _projectNumber; }
            set
            {
                _projectNumber = value;
                SaveVmSetting(nameof(ProjectNumber), _projectNumber);
            }
        }
        private string _projectTitleLine1;
        public string ProjectTitleLine1
        {
            get { return _projectTitleLine1; }
            set
            {
                _projectTitleLine1 = value;
                SaveVmSetting(nameof(ProjectTitleLine1), _projectTitleLine1);
            }
        }

        private string _projectTitleLine2;
        public string ProjectTitleLine2
        {
            get { return _projectTitleLine2; }
            set
            {
                _projectTitleLine2 = value;
                SaveVmSetting(nameof(ProjectTitleLine2), _projectTitleLine2);
            }
        }
        private string _projectTitleLine3;
        public string ProjectTitleLine3
        {
            get { return _projectTitleLine3; }
            set
            {
                _projectTitleLine3 = value;
                SaveVmSetting(nameof(ProjectTitleLine3), _projectTitleLine3);
            }
        }
        private string _clientTitleLine1;
        public string ClientNameLine1
        {
            get { return _clientTitleLine1; }
            set
            {
                _clientTitleLine1 = value;
                SaveVmSetting(nameof(ClientNameLine1), _clientTitleLine1);
            }
        }
        private string _clientTitleLine2;
        public string ClientNameLine2
        {
            get { return _clientTitleLine2; }
            set
            {
                _clientTitleLine2 = value;
                SaveVmSetting(nameof(ClientNameLine2), _clientTitleLine2);
            }
        }
        private string _clientTitleLine3;
        public string ClientNameLine3
        {
            get { return _clientTitleLine3; }
            set
            {
                _clientTitleLine3 = value;
                SaveVmSetting(nameof(ClientNameLine3), _clientTitleLine3);
            }
        }




        private string _code;
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                SaveVmSetting(nameof(Code), _code);
            }
        }


        //Cable
        public ObservableCollection<CableSizeModel> CableSizesUsedInProject { get; set; }
        private CableTypeModel _selectedCableType;

        public CableTypeModel SelectedCableType
        {
            get { return _selectedCableType; }
            set
            {
                _selectedCableType = value;
                var cableSizes = EdtSettings.CableSizesUsedInProject.Where(ct => ct.Type == _selectedCableType.Type).ToList();
                SelectedCableSizes = new ObservableCollection<CableSizeModel>(cableSizes);
            }
        }


        public ObservableCollection<CableSizeModel> SelectedCableSizes { get; set; } = new ObservableCollection<CableSizeModel>();




        private string _defaultCableInstallationType;
        public string DefaultCableInstallationType
        {
            get { return _defaultCableInstallationType; }
            set
            {
                _defaultCableInstallationType = value;
                SaveVmSetting(nameof(DefaultCableInstallationType), _defaultCableInstallationType);
            }
        }
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
                    //EdtSettings.CableSpacingMaxAmps_3C1kV = _cableSpacingMaxAmps_3C1kV;
                    //SettingsManager.SaveStringSettingToDb(nameof(CableSpacingMaxAmps_3C1kV), _cableSpacingMaxAmps_3C1kV);
                    SaveVmSetting(nameof(CableSpacingMaxAmps_3C1kV), _cableSpacingMaxAmps_3C1kV);
                }
            }
        }

        public string DefaultCableTypeLoad_3ph300to1kV { get; set; }
        public string DefaultCableTypeLoad_3ph5kV { get; set; }
        public string DefaultCableTypeDteq_3ph1kVLt1200A { get; set; }
        public string DefaultCableTypeDteq_3ph1kVGt1200A { get; set; }
        public string DefaultCableType_3ph5kV { get; set; }
        public string DefaultCableType_3ph15kV
        {
            get => _defaultCableType_3ph15kV;
            set
            {
                _defaultCableType_3ph15kV = value;
                SaveVmSetting(nameof(DefaultCableType_3ph15kV), _defaultCableType_3ph15kV);
            }
        }

        private string _defaultCableType_3ph15kV;
        private string _cableSpacingMaxAmps_3C1kV;
        private string loadFactorDefault;


        public string CableType3C1kVPower { get; set; }
        public string CableInsulation1kVPower { get; set; }
        public string CableInsulation5kVPower { get; set; }
        public string CableInsulation15kVPower { get; set; }
        public string CableInsulation35kVPower { get; set; }



        //Dteq

        private string _dteqMaxPercentLoaded;
        private string _defaultXfrImpedance;
        private string _loadDefaultPdTypeLV_NonMotor;
        private string _loadDefaultPdTypeLV_Motor;

        public string DteqMaxPercentLoaded
        {
            get { return _dteqMaxPercentLoaded; }
            set
            {
                var oldValue = _dteqMaxPercentLoaded;
                double dblOut;
                _dteqMaxPercentLoaded = value;
                ClearErrors(nameof(DteqMaxPercentLoaded));

                if (Double.TryParse(_dteqMaxPercentLoaded, out dblOut) == false) {
                    AddError(nameof(DteqMaxPercentLoaded), "Invalid Value");
                }
                else {
                    //EdtSettings.CableSpacingMaxAmps_3C1kV = _cableSpacingMaxAmps_3C1kV;
                    //SettingsManager.SaveStringSettingToDb(nameof(CableSpacingMaxAmps_3C1kV), _cableSpacingMaxAmps_3C1kV);
                    SaveVmSetting(nameof(DteqMaxPercentLoaded), _dteqMaxPercentLoaded);
                }
            }
        }


        public string DteqDefaultPdTypeLV { get; set; }
        public string DefaultXfrImpedance
        {
            get { return _defaultXfrImpedance; }
            set
            {
                var oldValue = _defaultXfrImpedance;
                double dblOut;
                _defaultXfrImpedance = value;
                ClearErrors(nameof(DefaultXfrImpedance));

                if (Double.TryParse(_defaultXfrImpedance, out dblOut) == false) {
                    AddError(nameof(DefaultXfrImpedance), "Invalid Value");
                }
                else {
                    //EdtSettings.CableSpacingMaxAmps_3C1kV = _cableSpacingMaxAmps_3C1kV;
                    //SettingsManager.SaveStringSettingToDb(nameof(CableSpacingMaxAmps_3C1kV), _cableSpacingMaxAmps_3C1kV);
                    SaveVmSetting(nameof(DefaultXfrImpedance), _defaultXfrImpedance);
                }
            }
        }



        //Voltage
        public string VoltageDefault1kV { get; set; }


        //Load
        public string LoadDefaultPdTypeLV_NonMotor 
        { 
            get => _loadDefaultPdTypeLV_NonMotor;
            set 
            { 
                _loadDefaultPdTypeLV_NonMotor = value;
                SaveVmSetting(nameof(LoadDefaultPdTypeLV_NonMotor), _loadDefaultPdTypeLV_NonMotor);

            }
        }

        public string LoadDefaultPdTypeLV_Motor
        {
            get => _loadDefaultPdTypeLV_Motor;
            set
            {
                _loadDefaultPdTypeLV_Motor = value;
                SaveVmSetting(nameof(LoadDefaultPdTypeLV_Motor), _loadDefaultPdTypeLV_Motor);

            }
        }

        public string LoadFactorDefault
        {
            get => loadFactorDefault;
            set
            {
                var oldValue = loadFactorDefault;
                double dblOut;
                loadFactorDefault = value;
                ClearErrors(nameof(LoadFactorDefault));

                if (Double.TryParse(loadFactorDefault, out dblOut) == false) {
                    AddError(nameof(LoadFactorDefault), "Invalid Value");
                }
                else if (Double.Parse(loadFactorDefault) > 1 || Double.Parse(loadFactorDefault) < 0) {
                    AddError(nameof(LoadFactorDefault), "Invalid Value");
                }
                else {
                    loadFactorDefault = value;
                    SaveVmSetting(nameof(LoadFactorDefault), loadFactorDefault);
                }
            }
        }
        public string LoadDefaultEfficiency_Other { get; set; }
        public string LoadDefaultPowerFactor_Other { get; set; }
        public string LoadDefaultEfficiency_Panel { get; set; }
        public string LoadDefaultPowerFactor_Panel { get; set; }


        //New Settings
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
        public void SaveVmSetting(string settingName, string settingValue)
        {
            Type projectSettingsClass = typeof(EdtSettings);

            foreach (var classProperty in projectSettingsClass.GetProperties()) {
                if (classProperty.Name == settingName) {
                    classProperty.SetValue(null, settingValue);
                    SettingsManager.SaveStringSettingToDb(settingName, settingValue);
                    break;
                }
            }
        }


        //Developer (old)
        public void LoadSettings()
        {
            SettingsManager.LoadProjectSettings();
            StringSettings = new ObservableCollection<SettingModel>(SettingsManager.StringSettingList);
            TableSettings = new ObservableCollection<SettingModel>(SettingsManager.TableSettingList);

        }
        private void SaveStringSetting()
        {
            SettingsManager.SaveStringSetting(SelectedStringSetting);
        }
        public void SaveTableSettings()
        {
            ArrayList tables = DaManager.prjDb.GetListOfTablesNamesInDb();

            //only saves to Db if Db table exists

            if (SelectedTableSetting != null) {
                if (tables.Contains(SelectedTableSetting.Name)) {
                    SettingsManager.SaveTableSetting(SelectedTableSetting);
                }
            }

            foreach (CableTypeModel cable in TypeManager.CableTypes) {
                DaManager.libDb.UpsertRecord(cable, "CableTypes", new List<string>());
            }


            foreach (CableSizeModel cable in SelectedCableSizes) {
                DaManager.prjDb.UpsertRecord(cable, "CableSizesUsedInProject", new List<string>());
            }
            EdtSettings.CableSizesUsedInProject = DaManager.prjDb.GetRecords<CableSizeModel>("CableSizesUsedInProject");
        }
    }
}
