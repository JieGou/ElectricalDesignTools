using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
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
using WpfUI.UserControls.Editors;
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
    private string _dteqMaxPercentLoaded;
    private string _xfrImpedance;
    private string _loadDefaultPdTypeLV_NonMotor;
    private string _loadDefaultPdTypeLV_Motor;
    private string _dteqDefaultPdTypeLV;

    private static string _lcsTypeDolLoad;
    private static string _localDisconnectType;


    private string _defaultLcsControlCableSize;
    private string _lcsTypeVsdLoad;
    
    private string _dteqLoadCableDerating;
    private string _xfrSubType;
    private string _xfrGrounding;


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
    public string XfrImpedance
    {
        get { return _xfrImpedance; }
        set
        {
            var oldValue = _xfrImpedance;
            double dblOut;
            _xfrImpedance = value;
            ClearErrors(nameof(XfrImpedance));

            if (Double.TryParse(_xfrImpedance, out dblOut) == false) {
                AddError(nameof(XfrImpedance), "Invalid Value");
            }
            else {
                //EdtSettings.CableSpacingMaxAmps_3C1kV = _cableSpacingMaxAmps_3C1kV;
                //SettingsManager.SaveStringSettingToDb(nameof(CableSpacingMaxAmps_3C1kV), _cableSpacingMaxAmps_3C1kV);
                SaveVmSetting(nameof(XfrImpedance), _xfrImpedance);
            }
        }
    }

    public string XfrSubType
    {
        get { return _xfrSubType; }
        set
        {
            var oldValue = _xfrSubType;
            _xfrSubType = value;
            SaveVmSetting(nameof(XfrSubType), _xfrSubType);
            
        }
    }

    public string XfrGrounding
    {
        get { return _xfrGrounding; }
        set
        {
            var oldValue = _xfrGrounding;
            _xfrGrounding = value;
            SaveVmSetting(nameof(XfrGrounding), _xfrGrounding);

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


    //LoadFactor
    public string LoadFactorDefault
    {
        get => _loadFactorDefault;
        set
        {
            var oldValue = _loadFactorDefault;
            double dblOut;
            _loadFactorDefault = value;
            ClearErrors(nameof(LoadFactorDefault));

            if (Double.TryParse(_loadFactorDefault, out dblOut) == false) {
                AddError(nameof(LoadFactorDefault), "Invalid Value");
            }
            else if (Double.Parse(_loadFactorDefault) > 1 || Double.Parse(_loadFactorDefault) < 0) {
                AddError(nameof(LoadFactorDefault), "Invalid Value");
            }
            else {
                _loadFactorDefault = value;
                SaveVmSetting(nameof(LoadFactorDefault), _loadFactorDefault);
            }
        }
    }
    private string _loadFactorDefault;


    public string LoadFactorDefault_Heater
    {
        get { return _loadFactorDefault_Heater; }
        set
        {
            var oldValue = _loadFactorDefault_Heater;
            double dblOut;
            _loadFactorDefault_Heater = value;
            ClearErrors(nameof(LoadFactorDefault_Heater));

            if (Double.TryParse(_loadFactorDefault_Heater, out dblOut) == false) {
                AddError(nameof(LoadFactorDefault_Heater), "Invalid Value");
            }
            else if (Double.Parse(_loadFactorDefault_Heater) > 1 || Double.Parse(_loadFactorDefault_Heater) < 0) {
                AddError(nameof(LoadFactorDefault_Heater), "Invalid Value");
            }
            else {
                _loadFactorDefault_Heater = value;
                SaveVmSetting(nameof(LoadFactorDefault_Heater), _loadFactorDefault_Heater);
            }
        }
    }
    private string _loadFactorDefault_Heater;
    public string LoadFactorDefault_Panel
    {
        get { return _loadFactorDefault_Panel; }
        set
        {
            var oldValue = _loadFactorDefault_Panel;
            double dblOut;
            _loadFactorDefault_Panel = value;
            ClearErrors(nameof(LoadFactorDefault_Panel));

            if (Double.TryParse(_loadFactorDefault_Panel, out dblOut) == false) {
                AddError(nameof(LoadFactorDefault_Panel), "Invalid Value");
            }
            else if (Double.Parse(_loadFactorDefault_Panel) > 1 || Double.Parse(_loadFactorDefault_Panel) < 0) {
                AddError(nameof(LoadFactorDefault_Panel), "Invalid Value");
            }
            else {
                _loadFactorDefault_Panel = value;
                SaveVmSetting(nameof(LoadFactorDefault_Panel), _loadFactorDefault_Panel);
            }
        }
    }
    private string _loadFactorDefault_Panel;

    public string LoadFactorDefault_Other
    {
        get { return _loadFactorDefault_Other; }
        set
        {
            var oldValue = _loadFactorDefault_Other;
            double dblOut;
            _loadFactorDefault_Other = value;
            ClearErrors(nameof(LoadFactorDefault_Other));

            if (Double.TryParse(_loadFactorDefault_Other, out dblOut) == false) {
                AddError(nameof(LoadFactorDefault_Other), "Invalid Value");
            }
            else if (Double.Parse(_loadFactorDefault_Other) > 1 || Double.Parse(_loadFactorDefault_Other) < 0) {
                AddError(nameof(LoadFactorDefault_Other), "Invalid Value");
            }
            else {
                _loadFactorDefault_Other = value;
                SaveVmSetting(nameof(LoadFactorDefault_Other), _loadFactorDefault_Other);
            }
        }
    }
    private string _loadFactorDefault_Other;

    public string LoadFactorDefault_Welding
    {
        get { return _loadFactorDefault_Welding; }
        set
        {
            var oldValue = _loadFactorDefault_Welding;
            double dblOut;
            _loadFactorDefault_Welding = value;
            ClearErrors(nameof(LoadFactorDefault_Welding));

            if (Double.TryParse(_loadFactorDefault_Welding, out dblOut) == false) {
                AddError(nameof(LoadFactorDefault_Welding), "Invalid Value");
            }
            else if (Double.Parse(_loadFactorDefault_Welding) > 1 || Double.Parse(_loadFactorDefault_Welding) < 0) {
                AddError(nameof(LoadFactorDefault_Welding), "Invalid Value");
            }
            else {
                _loadFactorDefault_Welding = value;
                SaveVmSetting(nameof(LoadFactorDefault_Welding), _loadFactorDefault_Welding);
            }
        }
    }
    private string _loadFactorDefault_Welding;


    //Efficiency
    public string LoadDefaultEfficiency_Heater
    {
        get => _loadDefaultEfficiency_Heater;
        set
        {
            var oldValue = _loadDefaultEfficiency_Heater;
            double dblOut;
            _loadDefaultEfficiency_Heater = value;

            ClearErrors(nameof(LoadDefaultEfficiency_Heater));

            if (Double.TryParse(_loadDefaultEfficiency_Heater, out dblOut) == false) {
                AddError(nameof(LoadDefaultEfficiency_Heater), "Invalid Value");
            }
            else if (Double.Parse(_loadDefaultEfficiency_Heater) > 1 || Double.Parse(_loadDefaultEfficiency_Heater) < 0) {
                AddError(nameof(LoadDefaultEfficiency_Heater), "Invalid Value");
            }
            else {
                _loadDefaultEfficiency_Heater = value;
                SaveVmSetting(nameof(LoadDefaultEfficiency_Heater), _loadDefaultEfficiency_Heater);
            }
        }
    }
    private string _loadDefaultEfficiency_Heater;
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
    private string _loadDefaultEfficiency_Panel;

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
    private string _loadDefaultEfficiency_Other;

    //Power Factor
    public string LoadDefaultPowerFactor_Heater
    {
        get => _loadDefaultPowerFactor_Heater;

        set
        {
            var oldValue = _loadDefaultPowerFactor_Heater;
            double dblOut;
            _loadDefaultPowerFactor_Heater = value;

            ClearErrors(nameof(LoadDefaultPowerFactor_Heater));

            if (Double.TryParse(_loadDefaultPowerFactor_Heater, out dblOut) == false) {
                AddError(nameof(LoadDefaultPowerFactor_Heater), "Invalid Value");
            }
            else if (Double.Parse(_loadDefaultPowerFactor_Heater) > 1 || Double.Parse(_loadDefaultPowerFactor_Heater) < 0) {
                AddError(nameof(LoadDefaultPowerFactor_Heater), "Invalid Value");
            }
            else {
                _loadDefaultPowerFactor_Heater = value;
                SaveVmSetting(nameof(LoadDefaultPowerFactor_Heater), _loadDefaultPowerFactor_Heater);
            }
        }
    }
    private string _loadDefaultPowerFactor_Heater;

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
    private string _loadDefaultPowerFactor_Panel;

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
    private string _loadDefaultPowerFactor_Other;

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
