using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
using PropertyChanged;
using System;

namespace WpfUI.ViewModels.Settings;

[AddINotifyPropertyChangedInterface]

public class EquipmentSettingsViewModel : SettingsViewModelBase
{

    #region Properties and Backing Fields


    private EdtProjectSettings _edtSettings;
    public EdtProjectSettings EdtProjectSettings
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

    public EquipmentSettingsViewModel(EdtProjectSettings edtSettings, TypeManager typeManager)
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
                //EdtProjectSettings.CableSpacingMaxAmps_3C1kV = _cableSpacingMaxAmps_3C1kV;
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
                //EdtProjectSettings.CableSpacingMaxAmps_3C1kV = _cableSpacingMaxAmps_3C1kV;
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


    //Primary
    public string XfrConnection_Primary
    {
        get { return _xfrConnection_Primary; }
        set
        {
            var oldValue = _xfrConnection_Primary;
            _xfrConnection_Primary = value;
            SaveVmSetting(nameof(XfrConnection_Primary), _xfrConnection_Primary);

        }
    }
    private string _xfrConnection_Primary;
     public string XfrGrounding_Primary
    {
        get { return _xfrGrounding_Primary; }
        set
        {
            var oldValue = _xfrGrounding_Primary;
            _xfrGrounding_Primary = value;
            SaveVmSetting(nameof(XfrGrounding_Primary), _xfrGrounding_Primary);

        }
    }
    private string _xfrGrounding_Primary;


    //Secondary
    public string XfrConnection_Secondary
    {
        get { return _xfrConnection_Secondary; }
        set
        {
            var oldValue = _xfrConnection_Secondary;
            _xfrConnection_Secondary = value;
            SaveVmSetting(nameof(XfrConnection_Secondary), _xfrConnection_Secondary);

        }
    }
    private string _xfrConnection_Secondary;

    public string XfrGrounding_Secondary
    {
        get { return _xfrGrounding_Secondary; }
        set
        {
            var oldValue = _xfrGrounding_Secondary;
            _xfrGrounding_Secondary = value;
            SaveVmSetting(nameof(XfrGrounding_Secondary), _xfrGrounding_Secondary);

        }
    }
    private string _xfrGrounding_Secondary;



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


    //DemandFactor
    public string DemandFactorDefault
    {
        get => _demandFactorDefault;
        set
        {
            var oldValue = _demandFactorDefault;
            double dblOut;
            _demandFactorDefault = value;
            ClearErrors(nameof(DemandFactorDefault));

            if (Double.TryParse(_demandFactorDefault, out dblOut) == false) {
                AddError(nameof(DemandFactorDefault), "Invalid Value");
            }
            else if (Double.Parse(_demandFactorDefault) > 1 || Double.Parse(_demandFactorDefault) < 0) {
                AddError(nameof(DemandFactorDefault), "Invalid Value");
            }
            else {
                _demandFactorDefault = value;
                SaveVmSetting(nameof(DemandFactorDefault), _demandFactorDefault);
            }
        }
    }
    private string _demandFactorDefault;


    public string DemandFactorDefault_Heater
    {
        get { return _demandFactorDefault_Heater; }
        set
        {
            var oldValue = _demandFactorDefault_Heater;
            double dblOut;
            _demandFactorDefault_Heater = value;
            ClearErrors(nameof(DemandFactorDefault_Heater));

            if (Double.TryParse(_demandFactorDefault_Heater, out dblOut) == false) {
                AddError(nameof(DemandFactorDefault_Heater), "Invalid Value");
            }
            else if (Double.Parse(_demandFactorDefault_Heater) > 1 || Double.Parse(_demandFactorDefault_Heater) < 0) {
                AddError(nameof(DemandFactorDefault_Heater), "Invalid Value");
            }
            else {
                _demandFactorDefault_Heater = value;
                SaveVmSetting(nameof(DemandFactorDefault_Heater), _demandFactorDefault_Heater);
            }
        }
    }
    private string _demandFactorDefault_Heater;
    public string DemandFactorDefault_Panel
    {
        get { return _demandFactorDefault_Panel; }
        set
        {
            var oldValue = _demandFactorDefault_Panel;
            double dblOut;
            _demandFactorDefault_Panel = value;
            ClearErrors(nameof(DemandFactorDefault_Panel));

            if (Double.TryParse(_demandFactorDefault_Panel, out dblOut) == false) {
                AddError(nameof(DemandFactorDefault_Panel), "Invalid Value");
            }
            else if (Double.Parse(_demandFactorDefault_Panel) > 1 || Double.Parse(_demandFactorDefault_Panel) < 0) {
                AddError(nameof(DemandFactorDefault_Panel), "Invalid Value");
            }
            else {
                _demandFactorDefault_Panel = value;
                SaveVmSetting(nameof(DemandFactorDefault_Panel), _demandFactorDefault_Panel);
            }
        }
    }
    private string _demandFactorDefault_Panel;

    public string DemandFactorDefault_Other
    {
        get { return _demandFactorDefault_Other; }
        set
        {
            var oldValue = _demandFactorDefault_Other;
            double dblOut;
            _demandFactorDefault_Other = value;
            ClearErrors(nameof(DemandFactorDefault_Other));

            if (Double.TryParse(_demandFactorDefault_Other, out dblOut) == false) {
                AddError(nameof(DemandFactorDefault_Other), "Invalid Value");
            }
            else if (Double.Parse(_demandFactorDefault_Other) > 1 || Double.Parse(_demandFactorDefault_Other) < 0) {
                AddError(nameof(DemandFactorDefault_Other), "Invalid Value");
            }
            else {
                _demandFactorDefault_Other = value;
                SaveVmSetting(nameof(DemandFactorDefault_Other), _demandFactorDefault_Other);
            }
        }
    }
    private string _demandFactorDefault_Other;

    public string DemandFactorDefault_Welding
    {
        get { return _demandFactorDefault_Welding; }
        set
        {
            var oldValue = _demandFactorDefault_Welding;
            double dblOut;
            _demandFactorDefault_Welding = value;
            ClearErrors(nameof(DemandFactorDefault_Welding));

            if (Double.TryParse(_demandFactorDefault_Welding, out dblOut) == false) {
                AddError(nameof(DemandFactorDefault_Welding), "Invalid Value");
            }
            else if (Double.Parse(_demandFactorDefault_Welding) > 1 || Double.Parse(_demandFactorDefault_Welding) < 0) {
                AddError(nameof(DemandFactorDefault_Welding), "Invalid Value");
            }
            else {
                _demandFactorDefault_Welding = value;
                SaveVmSetting(nameof(DemandFactorDefault_Welding), _demandFactorDefault_Welding);
            }
        }
    }
    private string _demandFactorDefault_Welding;


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
