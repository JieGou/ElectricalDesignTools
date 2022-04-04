using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Models.Validators;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace EDTLibrary.Models.Areas
{
    [AddINotifyPropertyChangedInterface]
    public class AreaToAddValidator : INotifyDataErrorInfo
    {

        public AreaToAddValidator(ListManager listManager)
        {
            _listManager = listManager;
        }

        public AreaToAddValidator(ListManager listManager, IArea areaToAdd)
        {
            _listManager = listManager;

            Tag = areaToAdd.Tag;
            Name = areaToAdd.Name;
            Description = areaToAdd.Description;
            AreaCategory = areaToAdd.AreaCategory;
            AreaClassification = areaToAdd.AreaClassification;
            MinTemp = areaToAdd.MinTemp.ToString();
            MaxTemp = areaToAdd.MaxTemp.ToString();
            NemaRating = areaToAdd.NemaRating;

        }

        private ListManager _listManager;
        private IDteq _feedingDteq;

        private string _tag = "";
        public string Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                ClearErrors(nameof(Tag));
                if (TagValidator.IsTagAvailable(_tag, _listManager) == false) {
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

        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                ClearErrors(nameof(Name));
                if (string.IsNullOrWhiteSpace(_name) || _name == "") {
                    AddError(nameof(Name), "Invalid Area Name");
                    _isValid = false;
                }
                else {
                    _isValid = true;
                }

            }
        }

        private string _description = "";
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
            }
        }

        private string _areaCategory = "";
        public string AreaCategory
        {
            get { return _areaCategory; }
            set
            {
                var oldAreaCategory = _areaCategory;
                _areaCategory = value;
                ClearErrors(nameof(AreaCategory));
                if (_areaCategory == null) {
                    _areaCategory = oldAreaCategory;
                }
                var category = TypeManager.AreaCategories.FirstOrDefault(ac => ac.CategoryName.ToLower() == _areaCategory.ToLower());
                if (category == null) {
                    AddError(nameof(AreaCategory), "Invalid Category");
                    _isValid = false;
                }
                else {
                    _isValid = true;
                }

            }
        }


        private string _areaClassification = "";
        public string AreaClassification
        {
            get { return _areaClassification; }
            set
            {
                _areaClassification = value;
                ClearErrors(AreaClassification);
                var locationAreaClass = TypeManager.AreaClassifications.FirstOrDefault(ac => ac.Zone.ToLower() == _areaClassification.ToLower());
                if (locationAreaClass == null) {
                    _isValid = false;
                }
                _isValid = true;
            }
        }



        private string _minTemp = "";
        public string MinTemp
        {
            get { return _minTemp; }
            set
            {
                _minTemp = value;
                double minTemp;
                double maxTemp;
                ClearErrors(nameof(MinTemp));

                if (Double.TryParse(_minTemp, out minTemp) == false ||
                    string.IsNullOrWhiteSpace(_minTemp) || _minTemp == "")
                    {
                    AddError(nameof(MinTemp), "Invalid value");
                    _isValid = false;
                }
              
                else if (Double.TryParse(MaxTemp, out maxTemp) != false ||
                        string.IsNullOrWhiteSpace(MaxTemp) == false || MaxTemp != "") {
                    ClearErrors(nameof(MinTemp));
                    ClearErrors(nameof(MaxTemp));

                    if (double.Parse(_minTemp) > double.Parse(_maxTemp)) {
                        AddError(nameof(MinTemp), "Min Temp must be lower than Max Temp");
                        _isValid = false;
                    }
                }
                _isValid = true;

            }
        }

        private string _maxTemp = "";
        public string MaxTemp
        {
            get { return _maxTemp; }
            set
            {
                _maxTemp = value;
                double maxTemp;
                double minTemp;
                ClearErrors(nameof(MaxTemp));

                if (Double.TryParse(_maxTemp, out maxTemp) == false ||
                    string.IsNullOrWhiteSpace(_maxTemp) || _maxTemp == "") { 
                        AddError(nameof(MaxTemp), "Invalid value");
                        _isValid = false;
                }
                else if (Double.TryParse(MinTemp, out minTemp) != false ||
                        string.IsNullOrWhiteSpace(MinTemp) == false || MinTemp != "") {
                    ClearErrors(nameof(MinTemp));
                    ClearErrors(nameof(MaxTemp));

                    if (double.Parse(_minTemp) > double.Parse(_maxTemp)) {
                        AddError(nameof(MaxTemp), "Max Temp must be higher than Min Temp");
                        _isValid = false;
                    }
                    

                }
                _isValid = true;

            }
        }

        private string _nemaType = "";
        public string NemaRating
        {
            get { return _nemaType; }
            set
            {
                var nemaTypeOld = _nemaType;
                _nemaType = value;
                if (_nemaType == null) {
                    _nemaType = nemaTypeOld;
                }
                ClearErrors(nameof(NemaRating));

                if (_nemaType != null) {
                    var locationNemaType = TypeManager.NemaTypes.FirstOrDefault(nt => nt.Type.ToLower() == _nemaType.ToLower());
                    if (locationNemaType == null) {
                        _isValid = false;

                    }
                    else {
                        _isValid = true;

                    }
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

            temp = Name;
            Name = fake;
            Name = temp;

            temp = Description;
            Description = fake;
            Description = temp;

            temp = AreaCategory;
            AreaCategory = fake;
            AreaCategory = temp;

            temp = AreaClassification;
            AreaClassification = fake;
            AreaClassification = temp;

            temp = NemaRating;
            NemaRating = fake;
            NemaRating = temp;

            temp = MaxTemp;
            MaxTemp = fake;
            MaxTemp = temp;

            temp = MinTemp;
            MinTemp = fake;
            MinTemp = temp;

#if DEBUG
            if (GlobalConfig.Testing == false) {
                if (string.IsNullOrWhiteSpace(Name))
                    Name = "Mill";
                if (string.IsNullOrWhiteSpace(AreaCategory))
                    AreaCategory = "Category 1";
                if (string.IsNullOrWhiteSpace(AreaClassification))
                    AreaClassification = "Zone 21";
                if (string.IsNullOrWhiteSpace(NemaRating))
                    NemaRating = "Type 12";
                if (string.IsNullOrWhiteSpace(MinTemp))
                    MinTemp = "-20";
                if (string.IsNullOrWhiteSpace(MaxTemp))
                    MaxTemp = "30";
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
