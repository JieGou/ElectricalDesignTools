﻿using EdtLibrary.LibraryData.TypeModels;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Selectors;
using EDTLibrary.Services;
using EDTLibrary.Validators;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

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
            VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage.ToString() == Voltage);
        }
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
        private string _tag;

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;

                //_demandFactor = DemandFactorSelector.GetDemandFactor(_type).ToString();
                DemandFactor = "";
                DemandFactor = DemandFactorSelector.GetDemandFactor(_type).ToString();
                ClearErrors(nameof(Type));
                if (GlobalConfig.SelectingNew == true) { return; }

                if (string.IsNullOrWhiteSpace(_type) || _type == "") {
                    AddError(nameof(Type), "Invalid Load Type");
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
        private string _type;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _description;

        public string AreaTag
        {
            get { return _areaTag; }
            set
            {
                _areaTag = value;
                ClearErrors(nameof(AreaTag));
                _areaModel = null;
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
        private string _areaTag;


        public string FedFromTag
        {
            get { return _fedFromTag; }
            set
            {
                _fedFromTag = value;
                ClearErrors(nameof(FedFromTag));
                if (GlobalConfig.SelectingNew == true) { return; }

                _feedingDteq = _listManager.DteqList.FirstOrDefault(d => d.Tag == _fedFromTag);

                if (_feedingDteq != null && _fedFromTag != GlobalConfig.UtilityTag) {
                    Voltage = _feedingDteq.LoadVoltage.ToString();
                }
                else if (_fedFromTag == GlobalConfig.UtilityTag) {
                    AddError(nameof(FedFromTag), "Utility cannot supply loads");
                }
                else {
                    AddError(nameof(FedFromTag), "Invalid Supplier / Fed From");
                }
                //CanAdd();
            }
        }
        private string _fedFromTag;

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
        private string _size;

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
        private string _unit;

        public string Voltage
        {
            get { return _voltage; }
            set
            {
                _voltage = value;
                //_voltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage.ToString() == _voltage);

                _feedingDteq = _listManager.DteqList.FirstOrDefault(d => d.Tag == _fedFromTag);

                ClearErrors(nameof(Voltage));
                if (GlobalConfig.SelectingNew == true) { return; }

                double parsedVoltage;
                if (string.IsNullOrWhiteSpace(_voltage)) {
                    AddError(nameof(Voltage), "Invalid Voltage");
                }
                else if (double.TryParse(_voltage.ToString(), out parsedVoltage) == true) {
                    if (_feedingDteq != null) {
                        if (_voltage == _feedingDteq.LoadVoltage.ToString()) {
                            if (_type == LoadTypes.MOTOR.ToString()) {
                                if (_voltage == "600") {
                                    _voltage = "575";
                                }
                                else if (_voltage == "480") {
                                    _voltage = "460";
                                }
                            }
                            _isValid = true;
                            return;
                        }
                        else {
                            if (_type == LoadTypes.MOTOR.ToString()) {
                                if (_voltage == "575") {
                                    _voltage = "600";
                                    _isValid = true;
                                    return;
                                }
                                else if (_voltage == "460") {
                                    _voltage = "480";
                                    _isValid = true;
                                    return;
                                }
                            }
                            if (_feedingDteq.LoadVoltage == 208) {
                                if (_voltage == "208" || _voltage == "120") {
                                    _isValid = true;
                                    return;
                                }
                            }
                            else if (_feedingDteq.LoadVoltage == 240) {
                                if (_voltage == "240" || _voltage == "120") {
                                    _isValid = true;
                                    return;
                                }
                            }

                            else
                        AddError(nameof(Voltage), "Voltage does not match supply Equipment");
                        }
                    }
                }
               
            }
        }
        private string _voltage;

        public VoltageType VoltageType
        {
            get { return _voltageType; }
            set
            {
                if (value == null) return;
                _voltageType = value;
                if (_voltageType.Voltage == null) return;
                


                _feedingDteq = _listManager.DteqList.FirstOrDefault(d => d.Tag == _fedFromTag);

                ClearErrors(nameof(VoltageType));
                //if (GlobalConfig.SelectingNew == true) { return; }

                double parsedVoltage;
                if (string.IsNullOrWhiteSpace(_voltage)) {
                    AddError(nameof(VoltageType), "Invalid Voltage");
                    return;
                }
                if (_feedingDteq == null) return;

                
                if (_voltageType == _feedingDteq.LoadVoltageType) {
                    if (_type == LoadTypes.MOTOR.ToString()) {
                        if (_voltageType.Voltage == 600) {
                            _voltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==575);
                        }
                        else if (_voltageType.Voltage == 480) {
                            _voltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 460);
                        }
                    }
                    _isValid = true;
                    Voltage = _voltageType.Voltage.ToString();

                    return;
                }
                else {
                    if (_type == LoadTypes.MOTOR.ToString()) {
                        if (_feedingDteq.LoadVoltageType.Voltage == 600 && _voltageType.Voltage == 575) {
                            //_voltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 600);
                            _isValid = true;
                            Voltage = _voltageType.Voltage.ToString();

                            return;
                        }
                        else if (_feedingDteq.LoadVoltageType.Voltage == 480 && _voltageType.Voltage == 460) {
                            //_voltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 480);
                            _isValid = true;
                            Voltage = _voltageType.Voltage.ToString();

                            return;
                        }
                        else if (_feedingDteq.LoadVoltageType.Voltage == 240) {
                            if (_voltageType.Voltage == 208 && _voltageType.Phase == 1) {
                                _voltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 240);
                            }
                            else if (_voltageType.Voltage == 120 ) {
                                _voltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120);
                            }
                            _isValid = true;
                            Voltage = _voltageType.Voltage.ToString();
                            return;
                        }
                        else if (_feedingDteq.LoadVoltageType.Voltage == 208) {
                            if (_voltageType.Voltage == 240 && _voltageType.Phase == 1) {
                                _voltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 208);
                            }
                            else if (_voltageType.Voltage == 208) {
                                _voltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 208);
                            }
                            else if (_voltageType.Voltage == 120) {
                                _voltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120);
                            }
                            _isValid = true;
                            Voltage = _voltageType.Voltage.ToString();
                            return;
                        }
                        else
                            AddError(nameof(VoltageType), "Voltage does not match supply Equipment");
                    }
                    
                    else if (_feedingDteq.LoadVoltage == 208 && (_voltageType.Voltage == 208 || _voltageType.Voltage == 120)) {
                        _isValid = true;
                        Voltage = _voltageType.Voltage.ToString();

                        return;
                    }
                    else if (_feedingDteq.LoadVoltage == 240 && (_voltageType.Voltage == 240 || _voltageType.Voltage == 120))  {
                        _isValid = true;
                        Voltage = _voltageType.Voltage.ToString();

                        return;
                    }
                    else
                        AddError(nameof(VoltageType), "Voltage does not match supply Equipment");
                }
                
            }
        }
        private VoltageType _voltageType;

        public string DemandFactor
        {
            get { return _demandFactor; }
            set
            {
                ClearErrors(nameof(DemandFactor));
                _demandFactor = value;
                if (GlobalConfig.SelectingNew == true) { return; }

                double newLoad_DemandFactor;
                if (double.TryParse(_demandFactor, out newLoad_DemandFactor) == false) {
                    AddError(nameof(DemandFactor), "Value must be between 0 and 1");
                    _isValid = false;

                }
                else if (double.Parse(_demandFactor) < 0 || double.Parse(_demandFactor) > 1) {
                    AddError(nameof(DemandFactor), "Value must be between 0 and 1");
                }
                else {
                    _isValid = true;
                }
            }
        }
        private string _demandFactor;


        public string PanelSide
        {
            get { return _panelSide; }
            set { _panelSide = value; }
        }
        private string _panelSide;

        public int SequenceNumber
        {
            get { return _sequenceNumber; }
            set { _sequenceNumber = value; }
        }
        private int _sequenceNumber;
        public int CircuitNumber
        {
            get { return _circuitNumber; }
            set { _circuitNumber = value; }
        }
        private int _circuitNumber;


        private bool CanAdd()
        {
            LoadModel newLoad = new LoadModel();

            newLoad.VoltageType = VoltageType;
            if (_feedingDteq == null) return false;
            
            ClearErrors(nameof(FedFromTag));
            if (_feedingDteq.CanAdd(newLoad)) {
                return true;
            }
            AddError(nameof(FedFromTag), "Not enough space for load");
            return false;
        }

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

            temp = Voltage;
            Voltage = fake;
            Voltage = temp;

            var tempVt = VoltageType;
            VoltageType = new VoltageType();
            VoltageType = tempVt;

            temp = DemandFactor;
            DemandFactor = fake;
            DemandFactor = temp;

#if DEBUG
            //if (GlobalConfig.Testing == false) {
            //    if (string.IsNullOrWhiteSpace(Type))
            //        Type = "HEATER";
            //    if (string.IsNullOrWhiteSpace(FedFromTag))
            //        try {
            //            if (FedFromTag != null) {

            //            FedFromTag = _listManager.IDteqList.FirstOrDefault(d => d.Type.Contains("MCC")).Tag;
            //            }
            //        }
            //        catch { }
            //    if (string.IsNullOrWhiteSpace(Size))
            //        Size = "50";
            //    if (string.IsNullOrWhiteSpace(Unit))
            //        Unit = "kW";
            //    if (string.IsNullOrWhiteSpace(Voltage))
            //        Voltage = "460";
            //    DemandFactor = "0.8";
            //}
#endif
            if (Tag == GlobalConfig.EmptyTag) {
                ClearErrors();
                return false;
            }


            if (CanAdd() && _isValid && HasErrors == false) {
                return true;
            }

            var sb = new StringBuilder();
            foreach (var item in _errorDict) {
                sb.Append(item.Value + ", ");
            }
            return false;
        }
        private bool _isValid;

        public void ResetTag()
        {
            var tag = Tag;
            Tag = " ";
            Tag = tag;
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
