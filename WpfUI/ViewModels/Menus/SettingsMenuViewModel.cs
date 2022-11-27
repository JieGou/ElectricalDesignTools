using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.Cables;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
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
using WpfUI.ViewModels.Settings;
using WpfUI.Views.Settings;

namespace WpfUI.ViewModels.Menus;

[AddINotifyPropertyChangedInterface]

public class SettingsMenuViewModel : ViewModelBase
{
    public SettingsMenuViewModel(MainViewModel mainViewModel, EdtSettings edtSettings, TypeManager typeManager, ListManager listManager = null)
    {
        _mainViewModel = mainViewModel;
        _edtSettings = edtSettings;
        _typeManager = typeManager;
        _listManager = listManager;

        // Create commands

        _generalSettingsViewModel = new GeneralSettingsViewModel(_edtSettings, _typeManager, _listManager);
        NavigateGeneralSettingsCommand = new RelayCommand(NavigateGeneralSettings);

        _equipmentSettingsViewModel = new EquipmentSettingsViewModel(_edtSettings, _typeManager);
        NavigateEquipmentSettingsCommand = new RelayCommand(NavigateEquipmentSettings);

        _cableSettingsViewModel = new CableSettingsViewModel(edtSettings, _typeManager);
        NavigateCableSettingsCommand = new RelayCommand(NavigateCableSettings);

        _tagSettingsViewModel = new TagSettingsViewModel(_listManager);
        NavigateTagSettingsCommand = new RelayCommand(NavigateTagSettings);

        _exportSettingsViewModel = new ExportSettingsViewModel(edtSettings, _typeManager);
        NavigateExportSettingsCommand = new RelayCommand(NavigateExportSettings);
    }


    #region Commands

    public ICommand NavigateGeneralSettingsCommand { get; }
    public ICommand NavigateEquipmentSettingsCommand { get; }
    public ICommand NavigateCableSettingsCommand { get; }
    public ICommand NavigateTagSettingsCommand { get; }
    public ICommand NavigateExportSettingsCommand { get; }


    #endregion

    private GeneralSettingsViewModel _generalSettingsViewModel;
    private void NavigateGeneralSettings()
    {
        _generalSettingsViewModel.LoadVmSettings(_generalSettingsViewModel);
        CurrentViewModel = _generalSettingsViewModel;
        _mainViewModel.CurrentViewModel = CurrentViewModel;

    }

    private EquipmentSettingsViewModel _equipmentSettingsViewModel;
    private void NavigateEquipmentSettings()
    {
        _equipmentSettingsViewModel.LoadVmSettings(_equipmentSettingsViewModel);
        CurrentViewModel = _equipmentSettingsViewModel;
        _mainViewModel.CurrentViewModel = CurrentViewModel;
    }


    private CableSettingsViewModel _cableSettingsViewModel;
    private void NavigateCableSettings()
    {
        _cableSettingsViewModel.LoadVmSettings(_cableSettingsViewModel);
        CurrentViewModel = _cableSettingsViewModel;
        _mainViewModel.CurrentViewModel = CurrentViewModel;
    }

    private TagSettingsViewModel _tagSettingsViewModel;
    private void NavigateTagSettings()
    {
        _tagSettingsViewModel.LoadTagSettings();
        CurrentViewModel = _tagSettingsViewModel;
        _mainViewModel.CurrentViewModel = CurrentViewModel;
    }

    private ExportSettingsViewModel _exportSettingsViewModel;
    private void NavigateExportSettings()
    {
        _exportSettingsViewModel.LoadVmSettings(_exportSettingsViewModel);
        CurrentViewModel = _exportSettingsViewModel;
        _mainViewModel.CurrentViewModel = CurrentViewModel;

        //Select the first report type by default instead of a blank list with no selection
        if (_exportSettingsViewModel.ReportTypes.Count > 0 &&
            _exportSettingsViewModel.SelectedReportType == null) {
            _exportSettingsViewModel.SelectedReportType = _exportSettingsViewModel.ReportTypes[0];

        }
    }


    #region Properties and Backing Fields

    public ObservableCollection<SettingModel> StringSettings { get; set; }
    public SettingModel SelectedStringSetting { get; set; }

    private readonly MainViewModel _mainViewModel;
    private EdtSettings _edtSettings;
    public EdtSettings EdtSettings
    {
        get { return _edtSettings; }
        set { _edtSettings = value; }
    }
    private TypeManager _typeManager;
    private readonly ListManager _listManager;

    public TypeManager TypeManager
    {
        get { return _typeManager; }
        set { _typeManager = value; }
    }


    ViewModelBase CurrentViewModel { get; set; }
    public UserControl SelectedSettingView
    {
        get => _selectedSettingView;
        set
        {
            _selectedSettingView = value;
            LoadVmSettings();
        }
    }
    public ObservableCollection<SettingModel> TableSettings { get; set; }
    public SettingModel SelectedTableSetting { get; set; }

    #endregion







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
            else if (isValid) {
                SaveVmSetting(nameof(ProjectName), _projectName);
            }


        }
    }

    //Project Details

    private string _projectNumber;
    public string ProjectNumber

    {
        get { return _projectNumber; }
        set
        {
            var oldValue = _projectNumber;
            _projectNumber = value;
            ClearErrors(nameof(ProjectNumber));

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


    //General

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
    #region
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

            if (double.TryParse(_cableSpacingMaxAmps_3C1kV, out dblOut) == false) {
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
    private string _defaultLcsControlCableType;


    public string CableType3C1kVPower { get; set; }
    public string CableInsulation1kVPower { get; set; }
    public string CableInsulation5kVPower { get; set; }
    public string CableInsulation15kVPower { get; set; }
    public string CableInsulation35kVPower { get; set; }

    public string DefaultLcsControlCableType
    {
        get => _defaultLcsControlCableType;
        set
        {
            _defaultLcsControlCableType = value;
            SaveVmSetting(nameof(DefaultLcsControlCableType), _defaultLcsControlCableType);
        }
    }

    public string DefaultLcsControlCableSize
    {
        get => _defaultLcsControlCableSize;
        set
        {
            _defaultLcsControlCableSize = value;
            SaveVmSetting(nameof(DefaultLcsControlCableSize), _defaultLcsControlCableSize);
        }
    }

    #endregion


    //Dteq
    #region
    private string loadFactorDefault;

    private string _defaultLcsControlCableSize;
    private string _dteqMaxPercentLoaded;
    private string _defaultXfrImpedance;
    private string _loadDefaultPdTypeLV_NonMotor;
    private string _loadDefaultPdTypeLV_Motor;
    private string _dteqDefaultPdTypeLV;

    private static string _defaultLcsType;
    private UserControl _selectedSettingView;

    public string DteqMaxPercentLoaded
    {
        get { return _dteqMaxPercentLoaded; }
        set
        {
            var oldValue = _dteqMaxPercentLoaded;
            double dblOut;
            _dteqMaxPercentLoaded = value;
            ClearErrors(nameof(DteqMaxPercentLoaded));

            if (double.TryParse(_dteqMaxPercentLoaded, out dblOut) == false) {
                AddError(nameof(DteqMaxPercentLoaded), "Invalid Value");
            }
            else {
                //EdtSettings.CableSpacingMaxAmps_3C1kV = _cableSpacingMaxAmps_3C1kV;
                //SettingsManager.SaveStringSettingToDb(nameof(CableSpacingMaxAmps_3C1kV), _cableSpacingMaxAmps_3C1kV);
                SaveVmSetting(nameof(DteqMaxPercentLoaded), _dteqMaxPercentLoaded);
            }
        }
    }


    public string DteqDefaultPdTypeLV
    {
        get => _dteqDefaultPdTypeLV;
        set
        {
            _dteqDefaultPdTypeLV = value;
            SaveVmSetting(nameof(DteqDefaultPdTypeLV), _dteqDefaultPdTypeLV);

        }
    }
    public string DefaultXfrImpedance
    {
        get { return _defaultXfrImpedance; }
        set
        {
            var oldValue = _defaultXfrImpedance;
            double dblOut;
            _defaultXfrImpedance = value;
            ClearErrors(nameof(DefaultXfrImpedance));

            if (double.TryParse(_defaultXfrImpedance, out dblOut) == false) {
                AddError(nameof(DefaultXfrImpedance), "Invalid Value");
            }
            else {
                //EdtSettings.CableSpacingMaxAmps_3C1kV = _cableSpacingMaxAmps_3C1kV;
                //SettingsManager.SaveStringSettingToDb(nameof(CableSpacingMaxAmps_3C1kV), _cableSpacingMaxAmps_3C1kV);
                SaveVmSetting(nameof(DefaultXfrImpedance), _defaultXfrImpedance);
            }
        }
    }
    #endregion




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

            if (double.TryParse(loadFactorDefault, out dblOut) == false) {
                AddError(nameof(LoadFactorDefault), "Invalid Value");
            }
            else if (double.Parse(loadFactorDefault) > 1 || double.Parse(loadFactorDefault) < 0) {
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



    //Components
    public string DefaultLcsType
    {
        get => _defaultLcsType;
        set
        {
            _defaultLcsType = value;
            SaveVmSetting(nameof(DefaultLcsType), _defaultLcsType);

        }
    }




    //Misc
    //Voltage
    public string VoltageDefault1kV { get; set; }


    //New Settings
    public void LoadVmSettings() // old way when SettingsMenuViewModel was just SettingsViewModel
    {
        Type projectSettingsClass = typeof(EdtSettings);
        Type viewModelSettings = typeof(SettingsMenuViewModel);

        foreach (var classProperty in projectSettingsClass.GetProperties()) {
            foreach (var viewModelProperty in viewModelSettings.GetProperties()) {
                if (classProperty.Name == viewModelProperty.Name) {


                    try {
                        viewModelProperty.SetValue(this, classProperty.GetValue(null));
                        break;
                    }
                    catch (Exception) {

                        viewModelProperty.SetValue(this, classProperty.GetValue(null));
                        break;
                    }
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
                SettingsManager.SaveSettingToDb(settingName, settingValue);
                break;
            }
        }
    }

    public void SaveVmSetting(string settingName, SettingModel settingModel)
    {
        Type projectSettingsClass = typeof(EdtSettings);

        foreach (var classProperty in projectSettingsClass.GetProperties()) {
            if (classProperty.Name == settingName) {
                classProperty.SetValue(null, settingModel);
                SettingsManager.SaveSettingToDb(settingName, settingModel.Value);
                break;
            }
        }
    }

}
