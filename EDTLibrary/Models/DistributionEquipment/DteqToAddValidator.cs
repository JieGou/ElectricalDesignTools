using EDTLibrary.A_Helpers;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Loads;
using EDTLibrary.Validators;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace EDTLibrary.Models.DistributionEquipment;

[AddINotifyPropertyChangedInterface]
public class DteqToAddValidator : INotifyDataErrorInfo
{

    public DteqToAddValidator(ListManager listManager)
    {
        _listManager = listManager;
    }

    public DteqToAddValidator(ListManager listManager, IDteq dteqToAdd)
    {
        _listManager = listManager;
        _areaModel = dteqToAdd.Area;

        Tag = dteqToAdd.Tag;
        Type = dteqToAdd.Type;
        Description = dteqToAdd.Description;
        AreaTag = dteqToAdd.Area.Tag;

        FedFromTag = dteqToAdd.FedFromTag;

        Size = dteqToAdd.Size.ToString();
        Unit = dteqToAdd.Unit;
        LineVoltage = dteqToAdd.LineVoltage.ToString();
        LineVoltageType = dteqToAdd.LineVoltageType;
        LoadVoltage = dteqToAdd.LoadVoltage.ToString();
        LoadVoltageType = dteqToAdd.LoadVoltageType;
    }

    private ListManager _listManager;
    private IArea _areaModel;
    private IDteq _feedingDteq;
   

    private string _tag = "";
    public string Tag
    {
        get { return _tag; }
        set
        {
            _tag = value;
            ClearErrors(nameof(Tag));

            if (TagAndNameValidator.IsTagAvailable(_tag, _listManager, false) == false) {
                AddError(nameof(Tag), "Tag already exists");
            }
            else if (string.IsNullOrWhiteSpace(_tag)) { // TODO - create method for invalid tags
                if (_tag == GlobalConfig.EmptyTag) {
                    _isValid = false;
                }
                else {
                    AddError(nameof(Tag), "Invalid Tag");
                }
            }
            else {
                _isValid = true;
            }
        }
    }

    private string _type = "";
    public string Type
    {
        get { return _type; }
        set
        {
            _type = value;

            ClearErrors(nameof(Type));
            if (string.IsNullOrWhiteSpace(_type) || _type == "") {
                AddError(nameof(Type), "Invalid Load Type");
            }
            //set units
            else if (_type == DteqTypes.XFR.ToString()) {
                Unit = "";
                Unit = Units.kVA.ToString();

            }
            else if (_type == DteqTypes.SWG.ToString()) {
                Unit = "";
                Unit = Units.A.ToString();

            }
            else if (_type == DteqTypes.MCC.ToString()) {
                Unit = "";
                Unit = Units.A.ToString();

            }
            else if (_type == DteqTypes.SPL.ToString()) {
                Unit = "";
                Unit = Units.A.ToString();
            }
            else if (_type == DteqTypes.DPN.ToString()) {
                Unit = "";
                Unit = Units.A.ToString();
            }
            else if (_type == DteqTypes.CDP.ToString()) {
                Unit = "";
                Unit = Units.A.ToString();
            }
            else if (_type == "") {
                Unit = "";
            }

            if (_type != DteqTypes.XFR.ToString()) {
            }
        }
    }

    private string _description = "";
    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    private string _areaTag;

    public string AreaTag
    {
        get { return _areaTag; }
        set 
        { 
            _areaTag = value; 
            ClearErrors(nameof(AreaTag));
            _areaModel= null;
            if (_areaModel == null) {
                _areaModel = _listManager.AreaList.FirstOrDefault(a => a.Tag == _areaTag);
            }

            if (_areaModel != null) {
                _isValid = true;
            }
            else {
                AddError(nameof(AreaTag), "Selected area doesn't exist");
                _isValid = false;
            }
        }
    }


    private string _fedFromTag = "";
    public string FedFromTag
    {
        get { return _fedFromTag; }
        set
        {
            _fedFromTag = value;
            ClearErrors(nameof(FedFromTag));
            _feedingDteq = _listManager.DteqList.FirstOrDefault(d => d.Tag == _fedFromTag);
            var tag = Tag;


            //validate
            if (_fedFromTag == GlobalConfig.UtilityTag) {
                _isValid = true;
                _fedFromTag = GlobalConfig.UtilityTag;

            }
            else if (_feedingDteq != null) {
                _isValid = true;
            }
            else {
                AddError(nameof(FedFromTag), "Selected equipment doesn' exist");
                _isValid = false;
            }
            CanAdd();

        }
    }


    private string _size = "";
    public string Size
    {
        get { return _size; }
        set
        {
            double newLoad_LoadSize;
            _size = value;

            ClearErrors(nameof(Size));
            if (double.TryParse(_size, out newLoad_LoadSize) == false) {
                AddError(nameof(Size), "Invalid Size");
            }
            else if (double.Parse(_size) <= 0) {
                AddError(nameof(Size), "Value must be larger than 0");
            }
            else {
                _isValid = true;
            }
        }
    }

    private string _unit = "";
    public string Unit
    {
        get { return _unit; }
        set
        {
            _unit = value;

            ClearErrors(nameof(Unit));
            if (_type != "" && _type == null || _unit == "") {
                AddError(nameof(Unit), "Invalid Unit");
            }
            else if (_type == DteqTypes.XFR.ToString() && _unit != Units.kVA.ToString()) {
                AddError(nameof(Unit), "Incorrect Units for Equipment");
            }
            else if ((_type == DteqTypes.SWG.ToString() ||
                      _type == DteqTypes.CDP.ToString() ||
                      _type == DteqTypes.MCC.ToString() ||
                      _type == DteqTypes.DPN.ToString() ||
                      _type == DteqTypes.SPL.ToString()) &&
                      _unit != Units.A.ToString()) {
                AddError(nameof(Unit), "Incorrect Units for Equipment");
            }
            else {
                _isValid = true;
            }
        }

    }

    public string LineVoltage
    {
        get { return _lineVoltage; }
        set
        {
            _lineVoltage = value;
            double parsedVoltage;
            ClearErrors(nameof(LineVoltage));

            if (string.IsNullOrWhiteSpace(_lineVoltage)) {
                AddError(nameof(LineVoltage), "Invalid Voltage");
            }
            else if (double.TryParse(_lineVoltage.ToString(), out parsedVoltage) == true) {
                if (_fedFromTag == GlobalConfig.UtilityTag) {
                    _isValid = true;
                }
                else if (_feedingDteq != null && _feedingDteq.Tag != "UTILITY") {
                    if (_lineVoltage != _feedingDteq.LoadVoltage.ToString()) {
                        AddError(nameof(LineVoltage), "Voltage does not match supply voltage");
                    }
                }
                else { 
                    _isValid = true; }
            }
        }
    }
    private string _lineVoltage = "";

    public string LoadVoltage
    {
        get { return _loadVoltage; }
        set
        {
            _loadVoltage = value;
            ClearErrors(nameof(LoadVoltage));
            if (string.IsNullOrWhiteSpace(_loadVoltage)) {
                AddError(nameof(LoadVoltage), "Invalid voltage");
            }
            else if (Type != DteqTypes.XFR.ToString()
                && _loadVoltage != LineVoltage) {
                AddError(nameof(LoadVoltage), "Voltage does not match supply voltage");
            }

            else if (_fedFromTag == GlobalConfig.UtilityTag) {
                _isValid = true;
            }
        }
    }
    private string _loadVoltage = "";

    public VoltageType LineVoltageType
    {
        get { return _lineVoltageType; }
        set
        {
            if (value == null || value == _lineVoltageType) return;
            _lineVoltageType = value;
            if (_lineVoltageType.Voltage == null) return;

            LineVoltage = _lineVoltageType.Voltage.ToString();

            ClearErrors(nameof(LineVoltageType));

            if (string.IsNullOrWhiteSpace(_lineVoltage)) {
                AddError(nameof(LineVoltageType), "Invalid Voltage");
            }
            
            if (_fedFromTag == GlobalConfig.UtilityTag) {
                _isValid = true;
            }
            else if (_feedingDteq != null && _feedingDteq.Tag != "UTILITY") {
                if (_feedingDteq.LoadVoltageType.Voltage == 208 && _feedingDteq.LoadVoltageType.Phase==3) {
                    if (_lineVoltageType.Voltage==208 || _lineVoltageType.Voltage == 120) {
                        _isValid = true;
                        if (Type != DteqTypes.XFR.ToString()) {
                            LoadVoltageType = LineVoltageType;
                        }
                    }
                }
                else if (_feedingDteq.LoadVoltageType.Voltage == 240) {
                    if (_lineVoltageType.Voltage == 120) {
                        _isValid = true;
                        if (Type != DteqTypes.XFR.ToString()) {
                            LoadVoltageType = LineVoltageType;
                        }
                    }
                }
                else if (_lineVoltageType != _feedingDteq.LoadVoltageType) {
                    AddError(nameof(LineVoltageType), "Voltage does not match supply voltage");
                }
            }
            else { 
                _isValid = true;
                if (Type != DteqTypes.XFR.ToString()) {
                    LoadVoltageType = LineVoltageType;
                }
            }
    }
    }
    private VoltageType _lineVoltageType;

    public VoltageType LoadVoltageType
    {
        get { return _loadVoltageType; }
        set
        {
            if (value == null || value == _loadVoltageType) return;
            _loadVoltageType = value;
            if (_loadVoltageType.Voltage == null) return;

            LoadVoltage = _loadVoltageType.Voltage.ToString();

            ClearErrors(nameof(LoadVoltageType));
            if (string.IsNullOrWhiteSpace(_loadVoltage)) {
                AddError(nameof(LoadVoltageType), "Invalid voltage");
            }
            else if (_loadVoltageType != LineVoltageType && Type != DteqTypes.XFR.ToString()) {
                AddError(nameof(LoadVoltageType), "Voltage does not match supply voltage");
            }

            else if (_fedFromTag == GlobalConfig.UtilityTag) {
                _isValid = true;
                if (Type != DteqTypes.XFR.ToString()) {
                    LineVoltageType = LoadVoltageType;
                }
            }
        }
    }
    private VoltageType _loadVoltageType;

    private bool CanAdd()
    {
        DummyLoad newLoad = new DummyLoad();

        newLoad.VoltageType = LineVoltageType;
        if (_feedingDteq == null) return false;

        ClearErrors(nameof(FedFromTag));
        if (_feedingDteq.CanAdd(newLoad)) {
            return true;
        }
        AddError(nameof(FedFromTag), "Not enough space for load");
        return false;
    }

    public string Error { get; }


    private bool _isValid = false;
    public bool IsValid()
    {
        string temp;
        string fake = "fake";
        var tempVt = new VoltageType();

        temp = Tag;
        Tag = fake;
        Tag = temp;

        temp = Type;
        Type = fake;
        Type = temp;

        temp = Description;
        Description = fake;
        Description = temp;

        temp = AreaTag;
        AreaTag = fake;
        AreaTag = temp; 
        
        temp = FedFromTag;
        FedFromTag = fake;
        FedFromTag = temp;

        temp = Size;
        Size = fake;
        Size = temp;

        temp = Unit;
        Unit = fake;
        Unit = temp;
        
        temp = LineVoltage;
        LineVoltage = fake;
        LineVoltage = temp;

        tempVt = LineVoltageType;
        LineVoltageType = new VoltageType();
        LineVoltageType = tempVt;

        temp = LoadVoltage;
        LoadVoltage = fake;
        LoadVoltage = temp;

        tempVt = LoadVoltageType;
        LoadVoltageType = new VoltageType();
        LoadVoltageType = tempVt;


#if DEBUG
        //if (GlobalConfig.Testing == false) {
        //    if (string.IsNullOrWhiteSpace(Type))
        //        Type = "MCC";
        //    if (string.IsNullOrWhiteSpace(FedFromTag))
        //        FedFromTag = "SWG-01";
        //    if (string.IsNullOrWhiteSpace(Size))
        //        Size = "800";
        //    if (string.IsNullOrWhiteSpace(Unit))
        //        Unit = "A";
        //    if (string.IsNullOrWhiteSpace(LineVoltage))
        //        LineVoltage = "480";
        //    if (string.IsNullOrWhiteSpace(LoadVoltage))
        //        LoadVoltage = "480";
        //}

        //foreach (var item in _errorDict) {
        //    ErrorHelper.Log($"DteqToAddValidator_IsValid - Error: {item.Key}, {item.Value}");
        //}
#endif


        if (Tag == GlobalConfig.EmptyTag) {
            ClearErrors();
            return false;
        }

        if (CanAdd() && _isValid && HasErrors == false) {
            return true;
        }

        var errors = _errorDict;
        if (_isValid && HasErrors == false) {

            return true;
        }
        return false;
    }


    public bool HasErrors => _errorDict.Any();
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    public readonly Dictionary<string, ObservableCollection<string>> _errorDict = new Dictionary<string, ObservableCollection<string>>();


    public void ClearErrors()
    {
        foreach (var item in _errorDict) {
            string errorType = item.Key;
            ClearErrors(errorType);
            OnErrorsChanged(errorType);
        }
    }
    private void ClearErrors(string propertyName)
    {
        _errorDict.Remove(propertyName);
        OnErrorsChanged(propertyName);
    }

    public void AddError(string propertyName, string errorMessage)
    {
        if (!_errorDict.ContainsKey(propertyName)) { // check if error Key exists
            _errorDict.Add(propertyName, new ObservableCollection<string>()); // create if not
        }
        _errorDict[propertyName].Add(errorMessage); //add error message to list of error messages
        OnErrorsChanged(propertyName);

#if DEBUG
        //ErrorHelper.Log($"DteqToAddValidator_AddError - Error: {Tag}, {propertyName}, {errorMessage}");
#endif
    }

    public IEnumerable GetErrors(string propertyName)
    {
        return _errorDict.GetValueOrDefault(propertyName, null);
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
