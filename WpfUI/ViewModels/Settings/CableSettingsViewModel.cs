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

public class CableSettingsViewModel : SettingsViewModelBase
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


    #endregion

    #region Commands

    public ICommand NavigateGeneralSettingsCommand { get; }
    private GeneralSettingsView _generalSettingsView = new GeneralSettingsView();
    public ICommand ReloadSettingsCommand { get; }

    public ICommand SaveStringSettingCommand { get; }
    public ICommand SaveTableSettingCommand { get; }

    #endregion

    public CableSettingsViewModel(EdtSettings edtSettings, TypeManager typeManager)
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
            var cableSizes = EdtSettings.CableSizesUsedInProject.Where(ct => ct.Type == _selectedCableType.Type).ToList();
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


}
