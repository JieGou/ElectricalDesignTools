using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.aMain;
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
using WpfUI.Views.Settings;

namespace WpfUI.ViewModels.Settings;

[AddINotifyPropertyChangedInterface]

public class EquipmentSettingsViewModel : SettingsViewModelBase
{

    #region Properties and Backing Fields


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

    #endregion

    public EquipmentSettingsViewModel(EdtSettings edtSettings, TypeManager typeManager)
    {
        _edtSettings = edtSettings;
        _typeManager = typeManager;
    }



    #region
    // fields
    private string loadFactorDefault;
    private string _dteqMaxPercentLoaded;
    private string _defaultXfrImpedance;
    private string _loadDefaultPdTypeLV_NonMotor;
    private string _loadDefaultPdTypeLV_Motor;
    private string _dteqDefaultPdTypeLV;

    private static string _lcsTypeDolLoad;
    private static string _localDisconnectType;


    private string _defaultLcsControlCableSize;
    private string _lcsTypeVsdLoad;
    private string _loadDefaultEfficiency_Other;
    private string _loadDefaultPowerFactor_Other;
    private string _loadDefaultEfficiency_Panel;
    private string _loadDefaultPowerFactor_Panel;
    private string _dteqLoadCableDerating;


    //Dteq

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

    public string DteqLoadCableDerating { get => _dteqLoadCableDerating; set 
        { 
            var oldValue = _dteqLoadCableDerating;
            double dblOut;
            _dteqLoadCableDerating = value; 

            ClearErrors(nameof(DteqLoadCableDerating));

            if (Double.TryParse(_dteqLoadCableDerating, out dblOut) == false) {
                AddError(nameof(DteqLoadCableDerating), "Invalid Value");
            }
            else if (Double.Parse(_dteqLoadCableDerating) > 1 || Double.Parse(_dteqLoadCableDerating) < 0) {
                AddError(nameof(DteqLoadCableDerating), "Invalid Value");
            }
            else {
                _dteqLoadCableDerating = value;
                SaveVmSetting(nameof(DteqLoadCableDerating), _dteqLoadCableDerating);
                foreach (var dteq in ScenarioManager.ListManager.IDteqList) {
                    dteq.LoadCableDerating = double.Parse(_dteqLoadCableDerating);
                }
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
    public string LoadDefaultEfficiency_Other
    {
        get => _loadDefaultEfficiency_Other;
        set
        {
            var oldValue = _loadDefaultEfficiency_Other;
            double dblOut;
            _loadDefaultEfficiency_Other = value;

            ClearErrors(nameof(LoadDefaultEfficiency_Other));

            if (Double.TryParse(_loadDefaultEfficiency_Other, out dblOut) == false) {
                AddError(nameof(LoadDefaultEfficiency_Other), "Invalid Value");
            }
            else if (Double.Parse(_loadDefaultEfficiency_Other) > 1 || Double.Parse(_loadDefaultEfficiency_Other) < 0) {
                AddError(nameof(LoadDefaultEfficiency_Other), "Invalid Value");
            }
            else {
                _loadDefaultEfficiency_Other = value;
                SaveVmSetting(nameof(LoadDefaultEfficiency_Other), _loadDefaultEfficiency_Other);
            }
        }
    }
    public string LoadDefaultPowerFactor_Other
    {
        get => _loadDefaultPowerFactor_Other;

        set
        {
            var oldValue = _loadDefaultPowerFactor_Other;
            double dblOut;
            _loadDefaultPowerFactor_Other = value;

            ClearErrors(nameof(LoadDefaultPowerFactor_Other));

            if (Double.TryParse(_loadDefaultPowerFactor_Other, out dblOut) == false) {
                AddError(nameof(LoadDefaultPowerFactor_Other), "Invalid Value");
            }
            else if (Double.Parse(_loadDefaultPowerFactor_Other) > 1 || Double.Parse(_loadDefaultPowerFactor_Other) < 0) {
                AddError(nameof(LoadDefaultPowerFactor_Other), "Invalid Value");
            }
            else {
                _loadDefaultPowerFactor_Other = value;
                SaveVmSetting(nameof(LoadDefaultPowerFactor_Other), _loadDefaultPowerFactor_Other);
            }
        }
    }

    public string LoadDefaultEfficiency_Panel
    {
        get => _loadDefaultEfficiency_Panel;

        set
        {
            var oldValue = _loadDefaultEfficiency_Panel;
            double dblOut;
            _loadDefaultEfficiency_Panel = value;

            ClearErrors(nameof(LoadDefaultEfficiency_Panel));

            if (Double.TryParse(_loadDefaultEfficiency_Panel, out dblOut) == false) {
                AddError(nameof(LoadDefaultEfficiency_Panel), "Invalid Value");
            }
            else if (Double.Parse(_loadDefaultEfficiency_Panel) > 1 || Double.Parse(_loadDefaultEfficiency_Panel) < 0) {
                AddError(nameof(LoadDefaultEfficiency_Panel), "Invalid Value");
            }
            else {
                _loadDefaultEfficiency_Panel = value;
                SaveVmSetting(nameof(LoadDefaultEfficiency_Panel), _loadDefaultEfficiency_Panel);
            }
        }
    }
    public string LoadDefaultPowerFactor_Panel
    {
        get => _loadDefaultPowerFactor_Panel;

        set
        {
            var oldValue = _loadDefaultPowerFactor_Panel;
            double dblOut;
            _loadDefaultPowerFactor_Panel = value;

            ClearErrors(nameof(LoadDefaultPowerFactor_Panel));

            if (Double.TryParse(_loadDefaultPowerFactor_Panel, out dblOut) == false) {
                AddError(nameof(LoadDefaultPowerFactor_Panel), "Invalid Value");
            }
            else if (Double.Parse(_loadDefaultPowerFactor_Panel) > 1 || Double.Parse(_loadDefaultPowerFactor_Panel) < 0) {
                AddError(nameof(LoadDefaultPowerFactor_Panel), "Invalid Value");
            }
            else {
                _loadDefaultPowerFactor_Panel = value;
                SaveVmSetting(nameof(LoadDefaultPowerFactor_Panel), _loadDefaultPowerFactor_Panel);
            }
        }
    }



    //Components
    public string LcsTypeDolLoad
    {
        get => _lcsTypeDolLoad;
        set
        {
            _lcsTypeDolLoad = value;
            SaveVmSetting(nameof(LcsTypeDolLoad), _lcsTypeDolLoad);

        }
    }
    public string LcsTypeVsdLoad
    {
        get => _lcsTypeVsdLoad;
        set
        {
            _lcsTypeVsdLoad = value;
            SaveVmSetting(nameof(LcsTypeVsdLoad), _lcsTypeVsdLoad);

        }
    }
    public string LocalDisconnectType
    {
        get => _localDisconnectType;
        set
        {
            _localDisconnectType = value;
            SaveVmSetting(nameof(LocalDisconnectType), _localDisconnectType);

        }
    }


}
