using EDTLibrary;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Validators;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace EDTLibrary.Models.Loads
{
    [AddINotifyPropertyChangedInterface]
    public class LoadToAddValidator : INotifyDataErrorInfo
    {
        private ListManager _listManager;
        private IArea _areaModel;
        private IDteq _feedingDteq;

        public LoadToAddValidator(ListManager listManager)
        {
            _listManager = listManager;
        }

        public LoadToAddValidator(ListManager listManager, ILoad loadToAdd)
        {
            _listManager = listManager;
            _areaModel = loadToAdd.Area;
            Tag = loadToAdd.Tag;
            Type = loadToAdd.Type;
            Description = loadToAdd.Description;
            AreaTag = loadToAdd.Area.Tag;
            FedFromTag = loadToAdd.FedFromTag;
            //FedFromTag = listManager.DteqList.FirstOrDefault(d => d.Id == loadToAdd.FedFrom.Id && d.Type == loadToAdd.FedFrom.Type).Tag;
            Size = loadToAdd.Size.ToString();
            Unit = loadToAdd.Unit;
            Voltage = loadToAdd.Voltage.ToString();
        }
        private string _tag;
        public string Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                ClearErrors(nameof(Tag));

                if (TagAndNameValidator.IsTagAvailable(_tag, _listManager) == false) {
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

        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;

                _loadFactor = "0.8";
                LoadFactor = "";
                LoadFactor = "0.8";
                ClearErrors(nameof(Type));
                if (GlobalConfig.SelectingNew == true) { return; }

                if (string.IsNullOrWhiteSpace(_type) || _type == "") {
                    AddError(nameof(Type), "Invalid Load Type");
                }
                else if (_type == LoadTypes.TRANSFORMER.ToString()) {
                    Unit = "";
                    Unit = Units.kVA.ToString();

                }
                else if (_type == LoadTypes.MOTOR.ToString()) {

                }
                else if (_type == LoadTypes.HEATER.ToString()) {
                    Unit = "";
                    Unit = Units.kW.ToString();

                }
                else if (_type == LoadTypes.WELDING.ToString()) {
                    Unit = "";
                    Unit = Units.A.ToString();

                }
                else if (_type == LoadTypes.PANEL.ToString()) {
                    Unit = "";
                    Unit = Units.A.ToString();

                }
                else if (_type == "") {
                    Unit = "";
                }
            }
        }

        private string _description;
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
                if (_areaModel== null) {
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
        private string _fedFromTag;
        public string FedFromTag
        {
            get { return _fedFromTag; }
            set
            {
                _fedFromTag = value;
                ClearErrors(nameof(FedFromTag));
                if (GlobalConfig.SelectingNew == true) { return; }

                _feedingDteq = _listManager.DteqList.FirstOrDefault(d => d.Tag == _fedFromTag);

                if (_feedingDteq != null && _fedFromTag != GlobalConfig.Utility) {
                    Voltage = _feedingDteq.LoadVoltage.ToString();
                }
                else if (_fedFromTag == GlobalConfig.Utility) {
                    AddError(nameof(FedFromTag), "Utility cannot supply loads");
                }
                else {
                    AddError(nameof(FedFromTag), "Invalid Supplier / Fed From");
                }
            }
        }

        private string _size;
        public string Size
        {
            get { return _size; }
            set
            {
                double newLoad_LoadSize;
                _size = value;
                // TODO - enforce Motor sizes
                ClearErrors(nameof(Size));
                if (GlobalConfig.SelectingNew == true) { return; }

                if (double.TryParse(_size, out newLoad_LoadSize) == false) {
                    AddError(nameof(Size), "Invalid Size");
                    if (_errorDict.ContainsKey(nameof(Tag)) == false) {
                        //ClearErrors(nameof(Size));
                    }
                }
                else if (double.Parse(_size) <= 0) {
                    AddError(nameof(Size), "Value must be larger than 0");
                }
                else {
                    _isValid = true;
                }
            }
        }

        private string _unit;
        public string Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                ClearErrors(nameof(Unit));
                if (GlobalConfig.SelectingNew == true) { return; }

                if (_type != "" && _type == null || _unit == "") {
                    AddError(nameof(Unit), "Invalid Unit");
                }
                else if (_type == LoadTypes.TRANSFORMER.ToString() && _unit != Units.kVA.ToString()) {
                    AddError(nameof(Unit), "Incorrect Units for Equipment");
                }
                else if (_type == LoadTypes.MOTOR.ToString()
                    && _unit != Units.HP.ToString() && _unit != Units.kW.ToString()) {
                    AddError(nameof(Unit), "Incorrect Units for Equipment");
                }

                else if (_type == LoadTypes.HEATER.ToString() && _unit != Units.kW.ToString()) {
                    AddError(nameof(Unit), "Incorrect Units for Equipment");
                }
                else {
                    _isValid = true;
                }
            }

        }

        private string _voltage;
        public string Voltage
        {
            get { return _voltage; }
            set
            {
                _voltage = value;
                ClearErrors(nameof(Voltage));
                if (GlobalConfig.SelectingNew == true) { return; }

                double parsedVoltage;
                if (string.IsNullOrWhiteSpace(_voltage)) {
                    AddError(nameof(Voltage), "Invalid Voltage");
                }
                else if (double.TryParse(value.ToString(), out parsedVoltage) == true) {
                    if (_feedingDteq != null) {
                        if (_voltage == _feedingDteq.LoadVoltage.ToString()) {
                            if (_type == LoadTypes.MOTOR.ToString()) {
                                if (Voltage == "600") {
                                    Voltage = "575";
                                }
                                else if (Voltage == "480") {
                                    Voltage = "460";
                                }
                            }
                            _isValid = true;
                        }
                        else {
                            if (_type == LoadTypes.MOTOR.ToString()) {
                                if (Voltage == "600") {
                                    Voltage = "575";
                                    _isValid = true;
                                }
                                else if (Voltage == "480") {
                                    Voltage = "460";
                                    _isValid = true;
                                }
                            }
                            else
                            AddError(nameof(Voltage), "Voltage does not match supply Equipment");
                        }
                    }
                }
               
            }
        }

        private string _loadFactor = "0.8";
        public string LoadFactor
        {
            get { return _loadFactor; }
            set
            {
                ClearErrors(nameof(LoadFactor));
                _loadFactor = value;
                if (GlobalConfig.SelectingNew == true) { return; }

                double newLoad_LoadFactor;
                if (double.TryParse(_loadFactor, out newLoad_LoadFactor) == false) {
                    AddError(nameof(LoadFactor), "Value must be between 0 and 1");
                    _isValid = false;

                }
                else if (double.Parse(_loadFactor) < 0 || double.Parse(_loadFactor) > 1) {
                    AddError(nameof(LoadFactor), "Value must be between 0 and 1");
                }
                else {
                    _isValid = true;
                }
            }
        }



        private bool _isValid;
        public bool IsValid()
        {

            string temp;
            string fake = "fake";

            temp = Tag;
            Tag = fake;
            Tag = temp;

            temp = Type;
            Type = fake;
            Type = temp;

            temp = Description;
            Description = fake;
            Description = temp;

            temp = FedFromTag;
            FedFromTag = fake;
            FedFromTag = temp;

            temp = Size;
            Size = fake;
            Size = temp;

            temp = Unit;
            Unit = fake;
            Unit = temp;

            temp = Voltage;
            Voltage = fake;
            Voltage = temp;

            temp = LoadFactor;
            LoadFactor = fake;
            LoadFactor = temp;

#if DEBUG
            if (GlobalConfig.Testing == false) {
                if (string.IsNullOrWhiteSpace(Type))
                    Type = "HEATER";
                if (string.IsNullOrWhiteSpace(FedFromTag))
                    FedFromTag = _listManager.DteqList.FirstOrDefault(d => d.Tag.Contains("MCC")).Tag;
                if (string.IsNullOrWhiteSpace(Size))
                    Size = "50";
                if (string.IsNullOrWhiteSpace(Unit))
                    Unit = "kW";
                if (string.IsNullOrWhiteSpace(Voltage))
                    Voltage = "460";
                LoadFactor = "0.8";
            }
#endif
            if (Tag == GlobalConfig.EmptyTag) {
                ClearErrors();
                return false;
            }

            if (_isValid && HasErrors == false) {
                return true;
            }
            return false;
        }


        public string Error { get; }

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
        public void ClearErrors(string propertyName)
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
}
