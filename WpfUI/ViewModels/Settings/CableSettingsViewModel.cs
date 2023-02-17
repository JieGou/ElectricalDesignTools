using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.Cables;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Cables;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
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

public class CableSettingsViewModel : SettingsViewModelBase
{

    #region Properties and Backing Fields

    public ObservableCollection<SettingModel> StringSettings { get; set; }
    public SettingModel SelectedStringSetting { get; set; }


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

    #region Commands

    public ICommand NavigateGeneralSettingsCommand { get; }
    private GeneralSettingsView _generalSettingsView = new GeneralSettingsView();
    public ICommand ReloadSettingsCommand { get; }

    public ICommand SaveStringSettingCommand { get; }
    public ICommand SaveTableSettingCommand { get; }

    #endregion

    public CableSettingsViewModel(EdtProjectSettings edtSettings, TypeManager typeManager)
    {
        _edtSettings = edtSettings;
        _typeManager = typeManager;
    }

    public ObservableCollection<CableSizeModel> CableSizesUsedInProject { get; set; }
    private CableTypeModel _selectedCableType;

    public CableTypeModel SelectedCableType
    {
        get { return _selectedCableType; }
        set
        {
            _selectedCableType = value;
            var cableSizes = EdtProjectSettings.CableSizesUsedInProject.Where(ct => ct.Type == _selectedCableType.Type).ToList();
            SelectedCableSizes = new ObservableCollection<CableSizeModel>(cableSizes);
        }
    }


    public ObservableCollection<CableSizeModel> SelectedCableSizes { get; set; } = new ObservableCollection<CableSizeModel>();



    private string _defaultLcsControlCableType;
    private string _defaultLcsControlCableSize;
    private string _defaultCableType_3ph15kV;
    private string _cableSpacingMaxAmps_3C1kV;
    private string _defaultCableInstallationType;


    public string CableInstallationType
    {
        get { return _defaultCableInstallationType; }
        set
        {
            _defaultCableInstallationType = value;
            SaveVmSetting(nameof(CableInstallationType), _defaultCableInstallationType);
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
                //EdtProjectSettings.CableSpacingMaxAmps_3C1kV = _cableSpacingMaxAmps_3C1kV;
                //SettingsManager.SaveStringSettingToDb(nameof(CableSpacingMaxAmps_3C1kV), _cableSpacingMaxAmps_3C1kV);
                SaveVmSetting(nameof(CableSpacingMaxAmps_3C1kV), _cableSpacingMaxAmps_3C1kV);
            }
        }
    }

    
    //Loads
    public string DefaultCableTypeLoad_2wire
    {
        get => _defaultCableTypeLoad_2wire;
        set
        {
            _defaultCableTypeLoad_2wire = value;
            SaveVmSetting(nameof(DefaultCableTypeLoad_2wire), _defaultCableTypeLoad_2wire);
        }
    }
    public string DefaultCableTypeLoad_3ph300to1kV
    {
        get => _defaultCableTypeLoad_3ph300to1kV;
        set
        {
            _defaultCableTypeLoad_3ph300to1kV = value;
            SaveVmSetting(nameof(DefaultCableTypeLoad_3ph300to1kV), _defaultCableTypeLoad_3ph300to1kV);

        }
    }
    public string DefaultCableTypeLoad_3ph5kV
    {
        get => _defaultCableTypeLoad_3ph5kV;
        set
        {
            _defaultCableTypeLoad_3ph5kV = value;
            SaveVmSetting(nameof(DefaultCableTypeLoad_3ph5kV), _defaultCableTypeLoad_3ph5kV);
        }
    }

    //Dteq
    public string DefaultCableTypeLoad_4wire
    {
        get => _defaultCableTypeLoad_4wire;
        set
        {
            _defaultCableTypeLoad_4wire = value;
            SaveVmSetting(nameof(DefaultCableTypeLoad_4wire), _defaultCableTypeLoad_4wire);
        }
    }
    public string DefaultCableTypeDteq_3ph1kVLt1200A
    {
        get => _defaultCableTypeDteq_3ph1kVLt1200A; set
        {
            _defaultCableTypeDteq_3ph1kVLt1200A = value;
            SaveVmSetting(nameof(DefaultCableTypeDteq_3ph1kVLt1200A), _defaultCableTypeDteq_3ph1kVLt1200A);
        }
    }
    public string DefaultCableTypeDteq_3ph1kVGt1200A
    {
        get => _defaultCableTypeDteq_3ph1kVGt1200A; set
        {
            _defaultCableTypeDteq_3ph1kVGt1200A = value;
            SaveVmSetting(nameof(DefaultCableTypeDteq_3ph1kVGt1200A), _defaultCableTypeDteq_3ph1kVGt1200A);
        }
    }
    public string DefaultCableType_3ph5kV
    {
        get => _defaultCableType_3ph5kV; set
        {
            _defaultCableType_3ph5kV = value;
            SaveVmSetting(nameof(DefaultCableType_3ph5kV), _defaultCableType_3ph5kV);
        }
    }
    public string DefaultCableType_3ph15kV
    {
        get => _defaultCableType_3ph15kV;
        set
        {
            _defaultCableType_3ph15kV = value;
            SaveVmSetting(nameof(DefaultCableType_3ph15kV), _defaultCableType_3ph15kV);
        }
    }


    public string CableType3C1kVPower { get; set; }
    public string CableInsulation1kVPower { get; set; }
    public string CableInsulation5kVPower { get; set; }
    public string CableInsulation15kVPower { get; set; }
    public string CableInsulation35kVPower { get; set; }

    public string LcsControlCableType
    {
        get => _defaultLcsControlCableType;
        set
        {
            _defaultLcsControlCableType = value;
            SaveVmSetting(nameof(LcsControlCableType), _defaultLcsControlCableType);
        }
    }

    public string LcsControlCableSize
    {
        get => _defaultLcsControlCableSize;
        set
        {
            _defaultLcsControlCableSize = value;
            SaveVmSetting(nameof(LcsControlCableSize), _defaultLcsControlCableSize);
        }
    }

    private string _cableLengthDteq;

    public string CableLengthDteq
    {
        get { return _cableLengthDteq; }
        set
        {
            var oldValue = _cableLengthDteq;
            double dblOut;
            _cableLengthDteq = value;
            ClearErrors(nameof(CableLengthDteq));

            if (Double.TryParse(_cableLengthDteq, out dblOut) == false) {
                AddError(nameof(CableLengthDteq), "Invalid Value");
            }
            if (dblOut < 3) {
                AddError(nameof(CableLengthDteq), "Invalid Value");
            }
            else {
                SaveVmSetting(nameof(CableLengthDteq), _cableLengthDteq);
            }
        }
    }

    private string _cableLengthLoad;

    public string CableLengthLoad
    {
        get { return _cableLengthLoad; }
        set
        {
            var oldValue = _cableLengthLoad;
            double dblOut;
            _cableLengthLoad = value;
            ClearErrors(nameof(CableLengthLoad));

            if (Double.TryParse(_cableLengthLoad, out dblOut) == false) {
                AddError(nameof(CableLengthLoad), "Invalid Value");
            }
            if (dblOut < 3) {
                AddError(nameof(CableLengthLoad), "Invalid Value");
            }
            else {
                SaveVmSetting(nameof(CableLengthLoad), _cableLengthLoad);
            }
        }
    }

    private string _cableLengthDrive;

    public string CableLengthDrive
    {
        get { return _cableLengthDrive; }
        set
        {
            var oldValue = _cableLengthDrive;
            double dblOut;
            _cableLengthDrive = value;
            ClearErrors(nameof(CableLengthDrive));

            if (Double.TryParse(_cableLengthDrive, out dblOut) == false) {
                AddError(nameof(CableLengthDrive), "Invalid Value");
            }
            if (dblOut < 3) {
                AddError(nameof(CableLengthDrive), "Invalid Value");
            }
            else {
                SaveVmSetting(nameof(CableLengthDrive), _cableLengthDrive);
            }
        }
    }

    private string _cableLengthLocalDisconnect;

    public string CableLengthLocalDisconnect
    {
        get { return _cableLengthLocalDisconnect; }
        set
        {
            var oldValue = _cableLengthLocalDisconnect;
            double dblOut;
            _cableLengthLocalDisconnect = value;
            ClearErrors(nameof(CableLengthLocalDisconnect));

            if (Double.TryParse(_cableLengthLocalDisconnect, out dblOut) == false) {
                AddError(nameof(CableLengthLocalDisconnect), "Invalid Value");
            }
            if (dblOut < 3) {
                AddError(nameof(CableLengthLocalDisconnect), "Invalid Value");
            }
            else {
                SaveVmSetting(nameof(CableLengthLocalDisconnect), _cableLengthLocalDisconnect);
            }
        }
    }


    private string _cableLengthLocalControlStation;
    private string _defaultCableTypeLoad_2wire;
    private string _defaultCableTypeLoad_4wire;
    private string _defaultCableType_3ph5kV;
    private string _defaultCableTypeDteq_3ph1kVGt1200A;
    private string _defaultCableTypeDteq_3ph1kVLt1200A;
    private string _defaultCableTypeLoad_3ph5kV;
    private string _defaultCableTypeLoad_3ph300to1kV;

    public string CableLengthLocalControlStation
    {
        get { return _cableLengthLocalControlStation; }
        set
        {
            var oldValue = _cableLengthLocalControlStation;
            double dblOut;
            _cableLengthLocalControlStation = value;
            ClearErrors(nameof(CableLengthLocalControlStation));

            if (Double.TryParse(_cableLengthLocalControlStation, out dblOut) == false) {
                AddError(nameof(CableLengthLocalControlStation), "Invalid Value");
            }
            if (dblOut < 3) {
                AddError(nameof(CableLengthLocalControlStation), "Invalid Value");
            }
            else {
                SaveVmSetting(nameof(CableLengthLocalControlStation), _cableLengthLocalControlStation);
            }
        }
    }




}
