﻿using EDTLibrary.Models.Loads;
using EDTLibrary.Models.Validators;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace EDTLibrary.Models.DistributionEquipment
{
    [AddINotifyPropertyChangedInterface]
    public class DteqToAddValidator : INotifyDataErrorInfo
    {

        public DteqToAddValidator(ListManager listManager, DteqModel selectedDteq )
        {
            _dteqList = listManager.DteqList;
            _loadList = listManager.LoadList;
            _selectedDteq = selectedDteq;
        }

        private ObservableCollection<DteqModel> _dteqList;
        private ObservableCollection<LoadModel> _loadList;
        private DteqModel _selectedDteq;

        private string _tag="";
        public string Tag
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
                        _isValid= false;
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
                //else if (_type == DteqTypes.XFR.ToString()) {
                //    Unit = "";
                //    Unit = Units.kVA.ToString();

                //}
                //else if (_type == DteqTypes.SWG.ToString()) {
                //    Unit = "";
                //    Unit = Units.A.ToString();

                //}
                //else if (_type == DteqTypes.MCC.ToString()) {
                //    Unit = "";
                //    Unit = Units.A.ToString();

                //}
                //else if (_type == DteqTypes.SPL.ToString()) {
                //    Unit = "";
                //    Unit = Units.A.ToString();
                //}
                //else if (_type == "") {
                //    Unit = "";
                //}
            }
        }

        private string _description = "";
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _fedFrom = "";
        public string FedFrom
        {
            get { return _fedFrom; }
            set
            {
                _fedFrom = value;
                ClearErrors(nameof(FedFrom));
                _selectedDteq = _dteqList.FirstOrDefault(d => d.Tag == _fedFrom);
                DteqModel feedingDteq = _dteqList.FirstOrDefault(d => d.Tag == _fedFrom);

                //set voltage
                if (_selectedDteq != null) {
                    //LineVoltage = _selectedDteq.LoadVoltage.ToString();
                }

                //validate
                if (_fedFrom == GlobalConfig.Utility) {
                    _isValid = true;
                }
                else if (_dteqList.Count == 0) {
                    if (string.IsNullOrWhiteSpace(_fedFrom)) {
                        _fedFrom = GlobalConfig.Utility;
                        FedFrom = _fedFrom;
                        _isValid = true;
                    }
                }
                else if (feedingDteq != null) {
                    _isValid = true;
                }
                else {
                    AddError(nameof(FedFrom), "Selected equipment doesn' exist");
                    _isValid = false;
                }
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

        private string _lineVoltage = "";
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
                    if (_fedFrom == GlobalConfig.Utility) {
                        _isValid = true;
                    }
                    else if (_selectedDteq != null) {
                        if (_lineVoltage != _selectedDteq.LoadVoltage.ToString()) {
                            AddError(nameof(LineVoltage), "Voltage does not match supply Equipment");
                        }
                    }
                    else { _isValid = true; }
                }
            }
        }

        private string _loadVoltage = "";
        public string LoadVoltage
        {
            get { return _loadVoltage; }
            set 
            { 
                _loadVoltage = value;
                ClearErrors(nameof(LoadVoltage));
                if (string.IsNullOrWhiteSpace(_loadVoltage)) {
                    AddError(nameof(LoadVoltage), "Voltage does not match supply Equipment");
                }
                else if (_fedFrom == GlobalConfig.Utility) {
                    _isValid = true;
                }
            }
        }


        public string Error { get; }


        private bool _isValid = false;
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

            temp = FedFrom;
            FedFrom = fake;
            FedFrom = temp;

            temp = Size;
            Size = fake;
            Size = temp;

            temp = Unit;
            Unit = fake;
            Unit = temp;

            temp = LoadVoltage;
            LoadVoltage = fake;
            LoadVoltage = temp;

            temp = LineVoltage;
            LineVoltage = fake;
            LineVoltage = temp;

#if DEBUG
            if (GlobalConfig.Testing == false) {
                if (string.IsNullOrWhiteSpace(Type)) 
                    Type = "MCC";
                if (string.IsNullOrWhiteSpace(FedFrom)) 
                    FedFrom = "SWG-01";
                if (string.IsNullOrWhiteSpace(Size))
                    Size = "800";
                if (string.IsNullOrWhiteSpace(Unit))
                    Unit = "A";
                if (string.IsNullOrWhiteSpace(LineVoltage))
                    LineVoltage = "480";
                if (string.IsNullOrWhiteSpace(LoadVoltage))
                    LoadVoltage = "480";
            }
#endif
            if (Tag == GlobalConfig.EmptyTag) {
                ClearErrors();
                return false;
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