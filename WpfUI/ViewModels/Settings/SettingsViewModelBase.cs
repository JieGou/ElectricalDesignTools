using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.ViewModels.Settings;
public class SettingsViewModelBase: ViewModelBase
{
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

    //New Settings
    public void LoadVmSettings(SettingsViewModelBase settingsViewModel)
    {
        Type projectSettingsClass = typeof(EdtSettings);
        Type viewModelSettings = settingsViewModel.GetType();

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
