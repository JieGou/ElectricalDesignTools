using AutocadLibrary;
using EDTLibrary;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
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


    public TagSettingsViewModel(ListManager listManager)
    {
        RestoreDefaultTagsCommand = new RelayCommand(RestoreDefaultTags);
        ListManager = listManager;

        //commands
        TestCommand = new RelayCommand(Test_GetSequenceNumberTest);

        RestoreDefaultTagsCommand = new RelayCommand(RestoreDefaultTags);
    }


    #region General

    private string _autoTagEquipment;

    public string AutoTagEquipment
    {
        get { return _autoTagEquipment; }
        set { _autoTagEquipment = value;
            SaveTagSetting(nameof(AutoTagEquipment), value);
                }
    }


    #endregion
    private string _componentSuffixSeparator;
    public string ComponentSuffixSeparator
    {
        get { return _componentSuffixSeparator; }
        set
        {
            _componentSuffixSeparator = value;
            SaveTagSetting(nameof(ComponentSuffixSeparator), value);
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

    private string _eqNumberDigitCount;

    public string EqNumberDigitCount
    {
        get { return _eqNumberDigitCount; }
        set {

            var num = 0;
            ClearErrors(nameof(EqNumberDigitCount));
            if (int.TryParse(value, out num)) {
                if (num >=1) {
                    SaveTagSetting(nameof(EqNumberDigitCount), value);
                }
                else {
                    AddError(nameof(EqNumberDigitCount), "Invalid value. Must be a positive integer");
                }
            }
            else {
                AddError(nameof(EqNumberDigitCount), "Invalid value. \nMust be a positive integer");
            }
            _eqNumberDigitCount = value;

        }
    }

    private string _eqIdentifierSeparator;
    public string EqIdentifierSeparator
    {
        get { return _eqIdentifierSeparator; }
        set { _eqIdentifierSeparator = value; 
                    SaveTagSetting(nameof(EqIdentifierSeparator), value);
        }
    }

    #region Eq Identifiers

    private string _transformerIdentifier;
    public string TransformerIdentifier
    {
        get { return _transformerIdentifier; }
        set { _transformerIdentifier = value; 
                    SaveTagSetting(nameof(TransformerIdentifier), value);
        }
    }


    private string _lvTransformerIdentifier;
    public string LvTransformerIdentifier
    {
        get { return _lvTransformerIdentifier; }
        set { _lvTransformerIdentifier = value; 
                    SaveTagSetting(nameof(LvTransformerIdentifier), value);
        }
    }


    private string _swgIdentifier;
    public string SwgIdentifier
    {
        get { return _swgIdentifier; }
        set { _swgIdentifier = value; 
                    SaveTagSetting(nameof(SwgIdentifier), value);
        }
    }


    private string _mccIdentifier;
    public string MccIdentifier
    {
        get { return _mccIdentifier; }
        set { _mccIdentifier = value; 
                    SaveTagSetting(nameof(MccIdentifier), value);
        }
    }


    private string _cdpIdentifier;
    public string CdpIdentifier
    {
        get { return _cdpIdentifier; }
        set { _cdpIdentifier = value; 
                    SaveTagSetting(nameof(CdpIdentifier), value);
        }
    }


    private string _dpnIdentifier;
    public string DpnIdentifier
    {
        get { return _dpnIdentifier; }
        set { _dpnIdentifier = value; 
                    SaveTagSetting(nameof(DpnIdentifier), value);
        }
    }


    private string _splitterIdentifier;
    public string SplitterIdentifier
    {
        get { return _splitterIdentifier; }
        set
        {
            _splitterIdentifier = value;
            SaveTagSetting(nameof(SplitterIdentifier), value);
        }
    }

    #endregion


    public ICommand TestCommand { get; }
    public void Test_GetSequenceNumberTest()
    {
        TagManager.AssignEqTag(new XfrModel { Type = DteqTypes.MCC.ToString()}, ListManager);
    }


    public ICommand RestoreDefaultTagsCommand { get; }
    public ListManager ListManager { get; }

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
