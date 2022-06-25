using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
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
    private static string _lcsTypeVsdLoad;


    private string _defaultLcsControlCableSize;


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
    public string LoadDefaultEfficiency_Other { get; set; }
    public string LoadDefaultPowerFactor_Other { get; set; }
    public string LoadDefaultEfficiency_Panel { get; set; }
    public string LoadDefaultPowerFactor_Panel { get; set; }



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
}
