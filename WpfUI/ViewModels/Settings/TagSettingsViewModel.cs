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

        CreateExampleDteqTag(MccIdentifier);
        CreateExampleLoadTag(MotorLoadIdentifier);
        CreateExmapleComponentTag(DisconnectSuffix);

        
        CreateExampleCableTag(PowerCableTypeIdentifier);


    }


    #region General


    public string AutoTagEquipment
    {
        get { return _autoTagEquipment; }
        set { _autoTagEquipment = value;
            SaveTagSetting(nameof(AutoTagEquipment), value);
                }
    }
    private string _autoTagEquipment;

    public string EqNumberDigitCount
    {
        get { return _eqNumberDigitCount; }
        set
        {

            var num = 0;
            ClearErrors(nameof(EqNumberDigitCount));
            if (int.TryParse(value, out num)) {
                if (num >= 1 && num < 10) {
                    SaveTagSetting(nameof(EqNumberDigitCount), value);
                    _eqNumberDigitCount = value;
                    CreateExampleDteqTag(MccIdentifier);
                    CreateExampleLoadTag(MotorLoadIdentifier);
                    CreateExmapleComponentTag(DisconnectSuffix);
                    CreateExampleCableTag(PowerCableTypeIdentifier);

                }
                else {
                    AddError(nameof(EqNumberDigitCount), "Invalid value. \nMust be from 1 to 9");
                }
            }
            else {
                AddError(nameof(EqNumberDigitCount), "Invalid value. \nMust be from 1 to 9");
            }
            
        }
    }
    private string _eqNumberDigitCount = "3";

    public string EqIdentifierSeparator
    {
        get { return _eqIdentifierSeparator; }
        set
        {
            _eqIdentifierSeparator = value;
            SaveTagSetting(nameof(EqIdentifierSeparator), value);
            CreateExampleDteqTag(MccIdentifier);
            CreateExampleLoadTag(MotorLoadIdentifier);
            CreateExmapleComponentTag(DisconnectSuffix);
        }
    }
    private string _eqIdentifierSeparator;

    #endregion


    private string GetNumerticalTag()
    {
        var numberTag = "";
        for (int i = 0; i < int.Parse(EqNumberDigitCount)-1; i++) {
            numberTag += "0";
        }
        numberTag += "1";
        return numberTag;
    }


    #region DteqEq Identifiers


    public string ExampleDteqTag
    {
        get { return _exampleDteqTag; }
        set { _exampleDteqTag = value; }
    }
    private string _exampleDteqTag;

    private void CreateExampleDteqTag(string eqTypIdentifier)
    {
        ExampleDteqTag = eqTypIdentifier + EqIdentifierSeparator + GetNumerticalTag();
    }


    public string TransformerIdentifier
    {
        get { return _transformerIdentifier; }
        set
        {
            _transformerIdentifier = value;
            SaveTagSetting(nameof(TransformerIdentifier), value);
            CreateExampleDteqTag(TransformerIdentifier);
        }
    }

    private string _transformerIdentifier;


    public string LvTransformerIdentifier
    {
        get { return _lvTransformerIdentifier; }
        set 
        { 
            _lvTransformerIdentifier = value; 
            SaveTagSetting(nameof(LvTransformerIdentifier), value);
            CreateExampleDteqTag(LvTransformerIdentifier);
        }
    }
    private string _lvTransformerIdentifier;


    public string SwgIdentifier
    {
        get { return _swgIdentifier; }
        set
        { 
            _swgIdentifier = value; 
            SaveTagSetting(nameof(SwgIdentifier), value);
            CreateExampleDteqTag(SwgIdentifier);

        }
    }
    private string _swgIdentifier;


    public string MccIdentifier
    {
        get { return _mccIdentifier; }
        set 
        { 
            _mccIdentifier = value; 
            SaveTagSetting(nameof(MccIdentifier), value);
            CreateExampleDteqTag(MccIdentifier);

        }
    }
    private string _mccIdentifier;


    public string CdpIdentifier
    {
        get { return _cdpIdentifier; }
        set
        { 
            _cdpIdentifier = value; 
            SaveTagSetting(nameof(CdpIdentifier), value);
            CreateExampleDteqTag(CdpIdentifier);

        }
    }
    private string _cdpIdentifier;


    public string DpnIdentifier
    {
        get { return _dpnIdentifier; }
        set 
        { 
            _dpnIdentifier = value; 
            SaveTagSetting(nameof(DpnIdentifier), value);
            CreateExampleDteqTag(DpnIdentifier);

        }
    }
    private string _dpnIdentifier;


    public string SplitterIdentifier
    {
        get { return _splitterIdentifier; }
        set
        {
            _splitterIdentifier = value;
            SaveTagSetting(nameof(SplitterIdentifier), value);
            CreateExampleDteqTag(SplitterIdentifier);

        }
    }
    private string _splitterIdentifier;

    #endregion


    #region Loads
    public string ExampleLoadTag
    {
        get { return _exampleLoadTag; }
        set { _exampleLoadTag = value; }
    }
    private string _exampleLoadTag;
    private void CreateExampleLoadTag(string eqTypIdentifier)
    {
        ExampleLoadTag = eqTypIdentifier + EqIdentifierSeparator + GetNumerticalTag();
    }
    public string MotorLoadIdentifier
    {
        get { return _motorLoadIdentifier; }
        set
        {
            _motorLoadIdentifier = value;
            SaveTagSetting(nameof(MotorLoadIdentifier), value);
            CreateExampleLoadTag(MotorLoadIdentifier);
        }
    }
    private string _motorLoadIdentifier;

    public string HeaterLoadIdentifier
    {
        get { return _heaterLoadIdentifier; }
        set
        {
            _heaterLoadIdentifier = value;
            SaveTagSetting(nameof(_heaterLoadIdentifier), value);
            CreateExampleLoadTag(HeaterLoadIdentifier);
        }
    }
    private string _heaterLoadIdentifier;

    public string PanelLoadIdentifier
    {
        get { return _panelLoadIdentifier; }
        set
        {
            _panelLoadIdentifier = value;
            SaveTagSetting(nameof(PanelLoadIdentifier), value);
            CreateExampleLoadTag(PanelLoadIdentifier);
        }
    }
    private string _panelLoadIdentifier;

    public string WeldingLoadIdentifier
    {
        get { return _weldingLoadIdentifier; }
        set
        {
            _weldingLoadIdentifier = value;
            SaveTagSetting(nameof(WeldingLoadIdentifier), value);
            CreateExampleLoadTag(WeldingLoadIdentifier);

        }
    }
    private string _weldingLoadIdentifier;

    public string OtherLoadIdentifier
    {
        get { return _otherLoadIdentifier; }
        set
        {
            _otherLoadIdentifier = value;
            SaveTagSetting(nameof(OtherLoadIdentifier), value);
            CreateExampleLoadTag(OtherLoadIdentifier);
        }
    }
    private string _otherLoadIdentifier;



    #endregion


    #region Components


    private string _exampleComponentTag;

    public string ExampleComponentTag
    {
        get { return _exampleComponentTag; }
        set { _exampleComponentTag = value; }
    }

    private void CreateExmapleComponentTag(string compTypeIdentifier)
    {
        ExampleComponentTag = ExampleLoadTag + ComponentSuffixSeparator + compTypeIdentifier;
    }
    public string ComponentSuffixSeparator
    {
        get { return _componentSuffixSeparator; }
        set
        {
            _componentSuffixSeparator = value;
            SaveTagSetting(nameof(ComponentSuffixSeparator), value);
            CreateExmapleComponentTag(ProtectionDeviceSuffix);
        }
    }
    private string _componentSuffixSeparator;

    public string ProtectionDeviceSuffix
    {
        get { return _protectionDeviceSuffix; }
        set 
        { 
            _protectionDeviceSuffix = value;
            SaveTagSetting(nameof(ProtectionDeviceSuffix), value);
            CreateExmapleComponentTag(ProtectionDeviceSuffix);
        }
    }
    private string _protectionDeviceSuffix;

    public string DisconnectSuffix
    {
        get { return _disconnectSuffix; }
        set
        {
            _disconnectSuffix = value;
            SaveTagSetting(nameof(DisconnectSuffix), value);
            CreateExmapleComponentTag(DisconnectSuffix);
        }
    }
    private string _disconnectSuffix;

    public string DriveSuffix
    {
        get { return _driveSuffix; }
        set
        {
            _driveSuffix = value;
            SaveTagSetting(nameof(DriveSuffix), value);
            CreateExmapleComponentTag(DriveSuffix);
        }
    }
    private string _driveSuffix;

    public string StarterSuffix
    {
        get { return _starterSuffix; }
        set
        {
            _starterSuffix = value;
            SaveTagSetting(nameof(StarterSuffix), value);
            CreateExmapleComponentTag(StarterSuffix);
        }
    }
    private string _starterSuffix;

    public string LcsSuffix
    {
        get { return _lcsSuffix; }
        set
        {
            _lcsSuffix = value;
            SaveTagSetting(nameof(LcsSuffix), value);
            CreateExmapleComponentTag(LcsSuffix);
        }
    }
    private string _lcsSuffix;
    #endregion


    #region Cables


    public string ExampleCableTag
    {
        get { return _exampleCableTag; }
        set { _exampleCableTag = value; }
    }
    private string _exampleCableTag;
    private void CreateExampleCableTag(string cableTypeIdentifier)
    {
        ExampleCableTag = ExampleDteqTag + CableTagSeparator + ExampleLoadTag + CableTagSeparator + cableTypeIdentifier;
    }



    public string CableTagSeparator
    {
        get { return _cableTagSeparator; }
        set
        {
            _cableTagSeparator = value;
            SaveTagSetting(nameof(CableTagSeparator), value);
            CreateExampleCableTag(PowerCableTypeIdentifier);
        }
    }
    private string _cableTagSeparator;

    public string PowerCableTypeIdentifier
    {
        get { return _powerCableTypeIdentifier; }
        set
        {
            _powerCableTypeIdentifier = value;
            SaveTagSetting(nameof(PowerCableTypeIdentifier), value);
            CreateExampleCableTag(PowerCableTypeIdentifier);

        }
    }
    private string _powerCableTypeIdentifier;

    public string ControlCableTypeIdentifier
    {
        get { return _controlCableTypeIdentifier; }
        set
        {
            _controlCableTypeIdentifier = value;
            SaveTagSetting(nameof(ControlCableTypeIdentifier), value);
            CreateExampleCableTag(ControlCableTypeIdentifier);

        }
    }
    private string _controlCableTypeIdentifier;

    public string InstrumentCableTypeIdentifier
    {
        get { return _instrumentCableTypeIdentifier; }
        set
        {
            _instrumentCableTypeIdentifier = value;
            SaveTagSetting(nameof(InstrumentCableTypeIdentifier), value);
            CreateExampleCableTag(InstrumentCableTypeIdentifier);

        }
    }
    private string _instrumentCableTypeIdentifier;

    public string EthernetCableTypeIdentifier
    {
        get { return _ethernetCableTypeIdentifier; }
        set
        {
            _ethernetCableTypeIdentifier = value;
            SaveTagSetting(nameof(EthernetCableTypeIdentifier), value);
            CreateExampleCableTag(EthernetCableTypeIdentifier);

        }
    }
    private string _ethernetCableTypeIdentifier;

    public string FiberCableTypeIdentifier
    {
        get { return _fiberCableTypeIdentifier; }
        set
        {
            _fiberCableTypeIdentifier = value;
            SaveTagSetting(nameof(FiberCableTypeIdentifier), value);
            CreateExampleCableTag(FiberCableTypeIdentifier);

        }
    }
    private string _fiberCableTypeIdentifier;

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
