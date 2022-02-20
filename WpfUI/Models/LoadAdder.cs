using EDTLibrary;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WpfUI.Validators;

namespace WpfUI.Models
{
    [AddINotifyPropertyChangedInterface]
    public class LoadAdder : INotifyDataErrorInfo  
    {
        private ObservableCollection<DteqModel> _dteqList;
        private ObservableCollection<LoadModel> _loadList;
        private DteqModel _selectedDteq;

        public LoadAdder(ObservableCollection<DteqModel> dteqList, ObservableCollection<LoadModel> loadList, DteqModel selectedDteq)
        {
            _dteqList = dteqList;
            _loadList = loadList;
            //_selectedDteq = selectedDteq;
        }

        private string? _tag;
        public string? Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                ClearErrors(nameof(Tag));
                if (TagValidator.IsTagAvailable(_tag, _dteqList, _loadList) == false) {
                    AddError(nameof(Tag), "Tag already exists");
                }
                else if (string.IsNullOrWhiteSpace(_tag)) { // TODO - create method for invalid tags
                    if (_tag == GlobalConfig.EmptyTag) {
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
                if (string.IsNullOrWhiteSpace(_type) || _type == "") {
                    AddError(nameof(Type), "Invalid Load Type");
                }
                else if (_type == LoadTypes.TRANSFORMER.ToString()) {
                    Unit = "";
                    Unit = Units.kVA.ToString();

                }
                else if (_type == LoadTypes.MOTOR.ToString()) {
                    Unit = "";
                    Unit = Units.HP.ToString();

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

        private string _fedFrom;
        public string FedFrom
        {
            get { return _fedFrom; }
            set
            {
                _fedFrom = value;
                _selectedDteq = _dteqList.FirstOrDefault(d => d.Tag == _fedFrom);
                if (_selectedDteq != null) {
                    Voltage = _selectedDteq.LoadVoltage.ToString();
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
                if (double.TryParse(_size, out newLoad_LoadSize) == false) {
                    AddError(nameof(Size), "Invalid Size");
                    if (_errorDict.ContainsKey(nameof(Tag))==false) {
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
                if (_type != "" && _type == null || _unit == "") {
                    AddError(nameof(Unit), "Invalid Unit");
                }
                else if (_type == EDTLibrary.LoadTypes.TRANSFORMER.ToString() && _unit != Units.kVA.ToString()) {
                    AddError(nameof(Unit), "Incorrect Units for Equipment");
                }
                else if (_type == EDTLibrary.LoadTypes.MOTOR.ToString()
                    && _unit != Units.HP.ToString() && _unit != Units.kW.ToString()) {
                    AddError(nameof(Unit), "Incorrect Units for Equipment");
                }

                else if (_type == EDTLibrary.LoadTypes.HEATER.ToString() && _unit != Units.kW.ToString()) {
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
                double parsedVoltage;
                ClearErrors(nameof(Voltage));
                if (_voltage != null) {
                    if (double.TryParse(value.ToString(), out parsedVoltage) == true) {

                        if (_voltage == null) {
                            AddError(nameof(Voltage), "Voltage does not match supply Equipment");
                        }

                        else if (_voltage != _selectedDteq.LoadVoltage.ToString()) {
                            if (_type == LoadTypes.MOTOR.ToString()) {
                                if (Voltage == "600") {
                                    Voltage = "575";
                                }
                                else if (Voltage == "480") {
                                    Voltage = "460";
                                }
                                _isValid = true;
                            }
                            else {
                                AddError(nameof(Voltage), "Voltage does not match supply Equipment");
                            }
                        }
                        else {
                            if (_type == LoadTypes.MOTOR.ToString()) {
                                if (Voltage == "600") {
                                    Voltage = "575";
                                }
                                else if (Voltage == "480") {
                                    Voltage = "460";
                                }
                                _isValid = true;
                            }
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
                double newLoad_LoadFactor;
                ClearErrors(nameof(LoadFactor));

                _loadFactor = value;
                if (double.TryParse(_loadFactor, out newLoad_LoadFactor) == false) {
                    AddError(nameof(LoadFactor), "Value must be between 0 and 1");

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
            string fakeChange = "fake";

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

            temp = FedFrom;
            FedFrom = fake;
            FedFrom = temp;

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

            if (Tag == GlobalConfig.EmptyTag) {
                ClearErrors();
                return false;
            }

            if (_isValid && HasErrors == false) {
                return true;
            }
            return false;
        }


        public string? Error { get; }

        public bool HasErrors => _errorDict.Any();
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
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

        public IEnumerable GetErrors(string? propertyName)
        {
            return _errorDict.GetValueOrDefault(propertyName, null);
        }

        private void OnErrorsChanged(string? propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
