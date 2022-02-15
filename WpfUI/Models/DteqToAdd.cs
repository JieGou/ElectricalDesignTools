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
    public class DteqToAdd : INotifyDataErrorInfo  
    {
        
        public DteqToAdd(ObservableCollection<DteqModel> dteqList, ObservableCollection<LoadModel> loadList)
        {
            _dteqList = dteqList;
            _loadList = loadList;
        }

        private ObservableCollection<DteqModel> _dteqList;
        private ObservableCollection<LoadModel> _loadList;

        private string? _tag="asdf";
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
                else if (_tag == "") { // TODO - create method for invalid tags
                    AddError(nameof(Tag), "Tag cannot be empty");
                }
            }
        }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? FedFrom { get; set; }
        public string? Size { get; set; }
        public string? Unit { get; set; }
        public string? LineVoltage { get; set; }
        public string? LoadVoltage { get; set; }

        public string? Error { get; }



        public bool HasErrors => _errorDict.Any();
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public readonly Dictionary<string, List<string>> _errorDict = new Dictionary<string, List<string>>();

       

        private void ClearErrors(string propertyName)
        {
            _errorDict.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }

        public void AddError(string propertyName, string errorMessage)
        {
            if (!_errorDict.ContainsKey(propertyName)) { // check if error Key exists
                _errorDict.Add(propertyName, new List<string>()); // create if not
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
