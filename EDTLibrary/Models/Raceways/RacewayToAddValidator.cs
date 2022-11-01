using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.LocalControlStations;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Raceways;
using EDTLibrary.Models.Validators;
using EDTLibrary.ProjectSettings;
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
    public class RacewayToAddValidator : INotifyDataErrorInfo
    {
        private ListManager _listManager;

        public RacewayToAddValidator(ListManager listManager)
        {
            _listManager = listManager;
        }
        public RacewayToAddValidator(RacewayModel racewayToAdd, ListManager listManager)
        {
            _listManager = listManager;

            Type = racewayToAdd.Type;
            Material = racewayToAdd.Material;
            Width = racewayToAdd.Width.ToString() ;
            Height = racewayToAdd.Height.ToString();
            Diameter = racewayToAdd.Diameter.ToString();

        }


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
        private string _tag;

        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;

               
                ClearErrors(nameof(Type));
                if (GlobalConfig.SelectingNew == true) { return; }

                if (string.IsNullOrWhiteSpace(_type) || _type == "") {
                    AddError(nameof(Type), "Invalid Type. Type cannot be empty.");
                }
              
            }
        }

        private string _description;
        public string Material
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _areaTag;

        private string _width;

        public string Width
        {
            get { return _width; }
            set
            {

                _width = value;

                double qty;
                ClearErrors(nameof(Width));

                if (Type != RacewayTypes.LadderTray.ToString()) {
                    _isValid = true;
                    return;
                }

                if (Double.TryParse(_width, out qty) == false ||
                    string.IsNullOrWhiteSpace(_width) || _width == "") {
                    AddError(nameof(Width), "Invalid value");
                    _isValid = false;
                }

                else if (double.Parse(_width) < 0) {
                    AddError(nameof(Width), "Invalid Value");
                    _isValid = false;
                }

                _isValid = true;
            }
        }

        private string _height;
        public string Height
        {
            get { return _height; }
            set 
            {
                _height = value;

                int qty;
                ClearErrors(nameof(Height));

                if (Type != RacewayTypes.LadderTray.ToString() ) {
                    _isValid = true;
                    return;
                }

                if (int.TryParse(_height, out qty) == false ||
                    string.IsNullOrWhiteSpace(_height) || _height == "") {
                    AddError(nameof(Height), "Invalid value");
                    _isValid = false;
                    return;
                }

                else if (double.Parse(_height) < 0) {
                    AddError(nameof(Height), "Invalid Value");
                    _isValid = false;
                }

                _isValid = true;
            }
        }

        private string _diameter;
        public string Diameter
        {
            get { return _diameter; }
            set
            {
                _diameter = value;

                int qty;
                ClearErrors(nameof(Diameter));
                
                if (Type != RacewayTypes.Conduit.ToString() && Type != RacewayTypes.DuctBank.ToString()) {
                    _isValid = true;
                    return;
                }
                
                if (int.TryParse(_diameter, out qty) == false ||
                    string.IsNullOrWhiteSpace(_diameter) || _diameter == "") {
                    AddError(nameof(Diameter), "Invalid value");
                    _isValid = false;
                    return;
                }

                else if (double.Parse(_diameter) < 0) {
                    AddError(nameof(Width), "Invalid Value");
                    _isValid = false;
                }

                _isValid = true;
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

            temp = Material;
            Material = fake;
            Material = temp;

            temp = Width;
            Width = fake;
            Width = temp;

            temp = Height;
            Height = fake;
            Height = temp;

            temp = Diameter;
            Diameter = fake;
            Diameter = temp;

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
