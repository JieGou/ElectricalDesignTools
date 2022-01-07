using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using EDTLibrary.TypeTables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfUI.ViewModels
{
    public class LocationsViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        public List<string> NemaTypes { get; set; } = new List<string>();
        public List<string> AreaClassifications { get; set; } = new List<string>();
        public ObservableCollection<LocationModel> LocationList { get; set; }
        
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
        private string _locationToAddName;
        public string LocationToAddName 
        {   get => _locationToAddName;
            set 
            { 
                _locationToAddName = value;
                ClearErrors(nameof(LocationToAddName));
                if (_locationToAddName == null) {
                    AddError(nameof(_locationToAddName), "Name cannot be empty");
                }
                else if (_locationToAddName == "") {
                    AddError(nameof(_locationToAddName), "Name cannot be empty");
                }
                else if (IsNameAvailable(_locationToAddName) == false) {
                    AddError(nameof(_locationToAddName), "Name already exists");
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
                    AddError(nameof(LocationToAddCategory), "Category cannot be empty");
                }
                else if (_locationToAddCategory == "") {
                    AddError(nameof(LocationToAddCategory), "Category cannot be empty");
                }
            
            }
        }

        private string _locationToAddAreaClass = "Non-Hazardous";
        public string LocationToAddAreaClass
        {
            get => _locationToAddAreaClass;
            set
            {
                _locationToAddAreaClass = value;

                ClearErrors(nameof(LocationToAddAreaClass));

                if (_locationToAddAreaClass == null) {
                    AddError(nameof(LocationToAddAreaClass), "Area Class cannot be empty");
                }
                else if (_locationToAddAreaClass == "") {
                    AddError(nameof(LocationToAddAreaClass), "Area Class cannot be empty");
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

        private string _locationToAddNemaRating;
        public string LocationToAddNemaRating
        {
            get => _locationToAddNemaRating;
            set
            {
                _locationToAddNemaRating = value;

                ClearErrors(nameof(LocationToAddNemaRating));

                if (_locationToAddNemaRating == null) {
                    AddError(nameof(LocationToAddNemaRating), "Nema Rating cannot be empty");
                }
                else if (_locationToAddNemaRating == "") {
                    AddError(nameof(LocationToAddNemaRating), "Nema Rating cannot be empty");
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
        public LocationsViewModel()
        {

            GetLocationsCommand = new RelayCommand(GetLocations);
            SaveLocationsCommand = new RelayCommand(SaveLocations);
            DeleteLocationCommand = new RelayCommand(DeleteLocation);

            AddLocationCommand = new RelayCommand(AddLocation);

        }



        #region Error Validation

        // INotifyDataErrorInfo
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

        IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
        {
            return _errorDict.GetValueOrDefault(propertyName, null);
        }

        public string Error { get; }
        

        #endregion




        #region Command Methods

        private void GetLocations()
        {
            LocationList = new ObservableCollection<LocationModel>(DbManager.prjDb.GetRecords<LocationModel>(GlobalConfig.locationTable));
        }
        private void SaveLocations()
        {
            if (LocationList.Count != 0) {

                Tuple<bool, string> update;
                bool error = false;
                string message = "";

                foreach (var location in LocationList) {
                    update = DbManager.prjDb.UpsertRecord<LocationModel>(location, GlobalConfig.locationTable, SaveLists.LoadSaveList);
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
                DbManager.prjDb.DeleteRecord(GlobalConfig.loadListTable, _selectedLocation.Id);
            }
        }



        private void AddLocation()
        {
            LocationModel location = new LocationModel();

            //TODO = proper add location validation
            if (_errorDict.Count == 0) {

                location.Name = _locationToAddName;
                location.Tag = _locationToAddTag;
                location.Description = _locationToAddDescription;
                location.LocationCategory = _locationToAddCategory;
                location.AreaClassification = _locationToAddAreaClass;
                location.MinTemp = double.Parse(_locationToAddMinTemp);
                location.MaxTemp = double.Parse(_locationToAddMaxTemp);
                location.NemaRating = _locationToAddNemaRating;
                
                LocationList.Add(location);
            }
        }








        #endregion


        #region Helper Methods

        private bool IsNameAvailable(string name)
        {
            if (name == null) {
                return false;
            }

            var locationName = LocationList.FirstOrDefault(l => l.Name.ToLower() == name.ToLower());
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
            var locationTag = LocationList.FirstOrDefault(l => l.Tag.ToLower() == tag.ToLower());
            List<DteqModel> dteqList = new List<DteqModel>();
            var dteqTag = dteqList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());
            List<LoadModel> loadList = new List<LoadModel>();
            var loadTag = loadList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());

            if (locationTag != null ||
                dteqTag != null ||
                loadTag != null) {
                return false;
            }

            return true;
        }

      
        public void CreateComboBoxLists()
        {
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