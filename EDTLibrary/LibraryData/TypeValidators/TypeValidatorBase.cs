using EdtLibrary.LibraryData.TypeModels;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace EdtLibrary.LibraryData.TypeValidators
{
    [AddINotifyPropertyChangedInterface]
    public class TypeValidatorBase : INotifyDataErrorInfo
    {


        public int Id { get; set; }

        private bool _isValid { get; set; }
        public virtual bool IsValid()
        {

            if (_isValid && HasErrors == false)
            {
                return true;
            }
            return false;
        }

        public UserEditableTypeBase CreateType(UserEditableTypeBase typeObject)
        {

            var objectProperties = typeObject.GetType().GetProperties();
            var validatorProperties = GetType().GetProperties();


            foreach (var objectProp in objectProperties)
            {

                foreach (var validatorProp in validatorProperties)
                {
                    if (objectProp.Name == validatorProp.Name)
                    {
                        objectProp.SetValue(typeObject, Convert.ChangeType(validatorProp.GetValue(this), objectProp.PropertyType));
                        var value = validatorProp.GetValue(this);
                        value = new string("asdf");
                    }

                }
            }
            typeObject.AddedByUser = true;
            typeObject.LastEdited = DateTime.UtcNow;
            return typeObject;
        }


        public string Error { get; }

        public bool HasErrors => _errorDict.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public readonly Dictionary<string, ObservableCollection<string>> _errorDict = new Dictionary<string, ObservableCollection<string>>();

        public void ClearErrors()
        {
            foreach (var item in _errorDict)
            {
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
            if (!_errorDict.ContainsKey(propertyName))
            { // check if error Key exists
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
