using AutocadLibrary;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.Windows.Forms;
using System.Windows.Input;
using WpfUI.Commands;

namespace WpfUI.ViewModels.Settings;

[AddINotifyPropertyChangedInterface]

public class TagSettingsViewModel : ViewModelBase
{


    public TagSettingsViewModel()
    {
        RestoreDefaultTagsCommand = new RelayCommand(RestoreDefaultTags);
    }
       

    //General

    private string _suffixSeparator;
    public string SuffixSeparator
    {
        get { return _suffixSeparator; }
        set
        {
            _suffixSeparator = value;
            SaveTagSetting(nameof(SuffixSeparator), value);
        }
    }


    private string _cableTagSeparator;
    public string CableTagSeparator
    {
        get { return _cableTagSeparator; }
        set
        {
            _cableTagSeparator = value;
            SaveTagSetting(nameof(CableTagSeparator), value);
        }
    }



    public ICommand RestoreDefaultTagsCommand { get; }
    public void RestoreDefaultTags()
    {
        MessageBox.Show("Not Implemented");
    }

    public void LoadTagSettings()
    {
        
        var tagSettingsClass = typeof(TagSettings);
        var tagSettingsViewModel = typeof(TagSettingsViewModel);

        foreach (var classProperty in tagSettingsClass.GetProperties()) {
            foreach (var viewModelProperty in tagSettingsViewModel.GetProperties()) {
                if (classProperty.Name == viewModelProperty.Name) {
                    viewModelProperty.SetValue(this, classProperty.GetValue(null));
                }

            }
        }
    }

    /// <summary>
    /// Saves a String setting to the DB and to the settings class
    /// </summary>
    /// <param name="settingName">Name of setting being saved</param>
    /// <param name="settingValue">New value of setting</param>
    public void SaveTagSetting(string settingName, string settingValue)
    {
        Type tagSettingsClass = typeof(TagSettings);

        foreach (var classProperty in tagSettingsClass.GetProperties()) {
            if (classProperty.Name == settingName) {
                classProperty.SetValue(null, settingValue);
                TagManager.SaveSettingToDb(settingName, settingValue);
                break;
            }
        }
    }
}
