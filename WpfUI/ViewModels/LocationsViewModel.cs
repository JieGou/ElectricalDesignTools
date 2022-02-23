using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfUI.Commands;

namespace WpfUI.ViewModels
{
    public class LocationsViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        public ObservableCollection<string> NemaTypes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Categories { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AreaClassifications { get; set; } = new ObservableCollection<string>();

        //public ObservableCollection<LocationModel> LocationList { 
        //    get { return _listManager.LocationList; } 
        //    set { _listManager.LocationList = value; }
        //}

        private ListManager _listManager;
        public ListManager ListManager
        {
            get { return _listManager; }
            set { _listManager = value; }
        }

        LocationModel _selectedLocation;
        public LocationModel SelectedLocation
        {
            get { return _selectedLocation; }
            set 
            {
                _selectedLocation = value;
                //TODO - set control values
            }
        }

        //Location To ADD
        #region ADD
        private string _locationToAddName = "test";
        public string LocationToAddName 
        {   get => _locationToAddName;
            set 
            { 
                _locationToAddName = value;
                ClearErrors(nameof(LocationToAddName));
                if (_locationToAddName == null) {
                    AddError(nameof(LocationToAddName), "Name cannot be empty");
                }
                else if (_locationToAddName == "") {
                    AddError(nameof(LocationToAddName), "Name cannot be empty");
                }
                else if (IsNameAvailable(_locationToAddName) == false) {
                    AddError(nameof(LocationToAddName), "Name already exists");
                }
                
               
            }
        }

        private string _locationToAddTag;
        public string LocationToAddTag
        {
            get => _locationToAddTag;
            set
            {
                _locationToAddTag = value;

                ClearErrors(nameof(LocationToAddTag));

                if (_locationToAddTag == null) {
                    AddError(nameof(_locationToAddTag), "Tag cannot be empty");
                }
                else if (_locationToAddTag == "") {
                    AddError(nameof(_locationToAddTag), "Tag cannot be empty");
                }
                else if (IsTagAvailable(_locationToAddTag) == false) {
                    AddError(nameof(_locationToAddTag), "Tag already exists");
                }

            }
        }

        private string _locationToAddDescription;
        public string LocationToAddDescription
        {
            get => _locationToAddDescription;
            set
            {
                _locationToAddDescription = value;
            }
        }

        private string _locationToAddCategory = "Category 1";
        public string LocationToAddCategory
        {
            get => _locationToAddCategory;
            set
            {
                _locationToAddCategory = value;

                ClearErrors(nameof(LocationToAddCategory));

                if (_locationToAddCategory == null) {
                    AddError(nameof(LocationToAddCategory), "Invalid Category");
                }
                else if (_locationToAddCategory == "") {
                    AddError(nameof(LocationToAddCategory), "Invalid Category");
                }
                else if (IsCategoryValid(_locationToAddCategory)==false) {
                    AddError(nameof(LocationToAddCategory), "Invalid Category");
                }

            }
        }

        private string _locationToAddAreaClass = "Non-Hazardous";
        public string LocationToAddAreaClass
        {
            get => _locationToAddAreaClass;
            set
            {
                if (value != null && value != "") {
                    _locationToAddAreaClass = value;

                    ClearErrors(nameof(LocationToAddAreaClass));

                    if (_locationToAddAreaClass == null) {
                        AddError(nameof(LocationToAddAreaClass), "Invalid Area Class");
                    }
                    else if (_locationToAddAreaClass == "") {
                        AddError(nameof(LocationToAddAreaClass), "Invalid Area Class");
                    }
                    else if (IsAreaClassValid(_locationToAddAreaClass) == false) {
                        AddError(nameof(LocationToAddAreaClass), "Invalid Area Class");
                    }
                }
                
            }
        }

        private string _locationToAddMinTemp = "0";
        public string LocationToAddMinTemp
        {
            get => _locationToAddMinTemp;
            set
            {
                _locationToAddMinTemp = value;

                double minTemp;

                ClearErrors(nameof(LocationToAddMinTemp));

                if (Double.TryParse(_locationToAddMinTemp, out minTemp) == false ) {
                    AddError(nameof(LocationToAddMinTemp), "Invalid value");
                }

                else if (_locationToAddMinTemp == null) {
                    AddError(nameof(LocationToAddMinTemp), "Min Temp cannot be empty");
                }
                else if (_locationToAddMinTemp == "") {
                    AddError(nameof(LocationToAddMinTemp), "Min Temp cannot be empty");
                }
                else if (double.Parse(_locationToAddMinTemp) > double.Parse(_locationToAddMaxTemp)) {
                    AddError(nameof(LocationToAddMinTemp), "Min Temp must be lower than Max Temp");

                }

            }
        }

        private string _locationToAddMaxTemp ="10";
        public string LocationToAddMaxTemp
        {
            get => _locationToAddMaxTemp;
            set
            {
                _locationToAddMaxTemp = value;

                double maxTemp;

                ClearErrors(nameof(LocationToAddMaxTemp));

                if (Double.TryParse(_locationToAddMaxTemp, out maxTemp) == false) {
                    AddError(nameof(LocationToAddMaxTemp), "Invalid value");
                }

                else if (_locationToAddMaxTemp == null) {
                    AddError(nameof(LocationToAddMaxTemp), "Max Temp cannot be empty");
                }
                else if (_locationToAddMaxTemp == "") {
                    AddError(nameof(LocationToAddMaxTemp), "Max Temp cannot be empty");
                }
                else if (double.Parse(_locationToAddMinTemp) > double.Parse(_locationToAddMaxTemp)) {
                    AddError(nameof(LocationToAddMaxTemp), "Max Temp must be higher than Min Temp");

                }

            }
        }

        private string _locationToAddNemaType = "Type 12";
        public string LocationToAddNemaType
        {
            get => _locationToAddNemaType;
            set
            {
                _locationToAddNemaType = value;

                ClearErrors(nameof(LocationToAddNemaType));

                if (_locationToAddNemaType == null) {
                    AddError(nameof(LocationToAddNemaType), "Invalid Nema Type");
                }
                else if (_locationToAddNemaType == "") {
                    AddError(nameof(LocationToAddNemaType), "Invalid Nema Type");
                }
                else if (IsNemaTypeValid(_locationToAddNemaType) == false) {
                    AddError(nameof(LocationToAddNemaType), "Invalid Nema Type");
                }
            }
        }

       
        #endregion

        #region Public Commands

        // Equipment Commands
        public ICommand GetLocationsCommand { get; }
        public ICommand SaveLocationsCommand { get; }
        public ICommand DeleteLocationCommand { get; }

        public ICommand AddLocationCommand { get; }


        #endregion
        public LocationsViewModel(ListManager listManager)
        {
            _listManager = listManager;

            GetLocationsCommand = new RelayCommand(GetLocations);
            SaveLocationsCommand = new RelayCommand(SaveLocations);
            DeleteLocationCommand = new RelayCommand(DeleteLocation);
            AddLocationCommand = new RelayCommand(AddLocation);

        }



        #region Error Validation

        // INotifyDataErrorInfo
        public bool HasErrors => _errorDict.Any();
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public readonly Dictionary<string, ObservableCollection<string>> _errorDict = new Dictionary<string, ObservableCollection<string>>();

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
        public IEnumerable GetErrors(string? propertyName)
        {
            return _errorDict.GetValueOrDefault(propertyName, null);
        }
        private void OnErrorsChanged(string? propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
        {
            return _errorDict.GetValueOrDefault(propertyName, null);
        }

        public string Error { get; }

        #endregion




        #region Command Methods

        private void GetLocations()
        {
            ListManager.LocationList.Clear();
            var loadList = ListManager.LoadList;
            ListManager.LocationList = new ObservableCollection<LocationModel>(DbManager.prjDb.GetRecords<LocationModel>(GlobalConfig.LocationTable));
        }
        private void SaveLocations()
        {
            if (ListManager.LocationList.Count != 0) {

                Tuple<bool, string> update;
                bool error = false;
                string message = "";

                foreach (var location in ListManager.LocationList) {
                    update = DbManager.prjDb.UpsertRecord<LocationModel>(location, GlobalConfig.LocationTable, SaveLists.LocationSaveList);
                    if (update.Item1 == false) {
                        error = true;
                        message = update.Item2;
                    }
                }
                if (error) {
                    MessageBox.Show(message);
                }
            }
        }
        private void DeleteLocation()
        {
            if (_selectedLocation != null) {
                int locationId = _selectedLocation.Id;
                DbManager.prjDb.DeleteRecord(GlobalConfig.LoadTable, _selectedLocation.Id);
            }
        }
        private void AddLocation()
        {
            LocationModel location = new LocationModel();

            //TODO = proper add location validation
            

            bool newLocationIsValid = true;

            //Name
            if (IsNameAvailable(LocationToAddName) == false) {
                newLocationIsValid = false;
            }

            //Tag
            if (IsTagAvailable(LocationToAddTag) == false) {
                newLocationIsValid = false;
            }

            //Category
            if (IsCategoryValid(LocationToAddCategory) == false) {
                newLocationIsValid = false;
            }

            //AreaClass
            if (IsAreaClassValid(LocationToAddAreaClass) == false) {
                newLocationIsValid = false;
            }

            //Temperature
            double outMinTemp;
            double outMaxTemp;
            if (double.TryParse(LocationToAddMinTemp, out outMinTemp) == false || double.TryParse(LocationToAddMaxTemp, out outMaxTemp) == false) {
                newLocationIsValid = false;
            }
            else if (outMinTemp > outMaxTemp) {
                newLocationIsValid = false;
            }

            //NemaRating
            if (IsNemaTypeValid(LocationToAddNemaType) == false) {
                newLocationIsValid = false;
            }

            if (newLocationIsValid) {

                location.Name = _locationToAddName;
                location.Tag = _locationToAddTag;
                location.Description = _locationToAddDescription;
                location.LocationCategory = _locationToAddCategory;
                location.AreaClassification = _locationToAddAreaClass;
                location.MinTemp = double.Parse(_locationToAddMinTemp);
                location.MaxTemp = double.Parse(_locationToAddMaxTemp);
                location.NemaType = _locationToAddNemaType;

                ListManager.LocationList.Add(location);
            }
        }

        #endregion


        #region Helper Methods
        private bool IsNameAvailable(string name)
        {
            if (name == null) {
                return false;
            }

            var locationName = ListManager.LocationList.FirstOrDefault(l => l.Name.ToLower() == name.ToLower());
            if (locationName != null) {
                return false;
            }

            return true;
        }
        private bool IsTagAvailable(string tag)
        {
            if (tag == null) {
                return false;
            }

            var locationTag = ListManager.LocationList.FirstOrDefault(l => l.Tag.ToLower() == tag.ToLower());
            if (locationTag != null) { 
                return false;
            }

            return true;
        }
        private bool IsCategoryValid(string category)
        {
            var locationCat = Categories.FirstOrDefault(c => c.ToLower() == category.ToLower());
            if (locationCat == null) {
                return false;
            }
            return true;
        }
        private bool IsAreaClassValid(string areaClass)
        {
            var locationAreaClass = AreaClassifications.FirstOrDefault(c => c.ToLower() == areaClass.ToLower());
            if (locationAreaClass == null) {
                return false;
            }
            return true;
        }
        private bool IsNemaTypeValid(string nemaType)
        {
            var locationNemaType = NemaTypes.FirstOrDefault(c => c.ToLower() == nemaType.ToLower());
            if (locationNemaType == null) {
                return false;
            }
            return true;
        }
        public void CreateComboBoxLists()
        {
            Categories.Add("Category 1");
            Categories.Add("Category 2");

            foreach (var item in TypeManager.NemaTypes) {
                NemaTypes.Add(item.Type.ToString());
            }

            foreach (var item in TypeManager.AreaClassifications) {
                AreaClassifications.Add(item.Zone);
            }
        }

        #endregion

    }

}