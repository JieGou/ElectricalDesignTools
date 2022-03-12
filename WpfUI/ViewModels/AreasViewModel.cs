using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models;
using PropertyChanged;
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
    [AddINotifyPropertyChangedInterface]

    public class AreasViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        public ObservableCollection<string> NemaTypes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Categories { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AreaClassifications { get; set; } = new ObservableCollection<string>();


        private ListManager _listManager;

        public ListManager ListManager
        {
            get { return _listManager; }
            set { _listManager = value; }
        }

     
        AreaModel _selectedArea;
        public AreaModel SelectedArea
        {
            get { return _selectedArea; }
            set 
            {
                _selectedArea = value;
                //TODO - set control values
            }
        }

        //Area To ADD
        #region ADD
        private string _locationToAddName = "test";
        public string AreaToAddName 
        {   get => _locationToAddName;
            set 
            { 
                _locationToAddName = value;
                ClearErrors(nameof(AreaToAddName));
                if (_locationToAddName == null) {
                    AddError(nameof(AreaToAddName), "Name cannot be empty");
                }
                else if (_locationToAddName == "") {
                    AddError(nameof(AreaToAddName), "Name cannot be empty");
                }
                else if (IsNameAvailable(_locationToAddName) == false) {
                    AddError(nameof(AreaToAddName), "Name already exists");
                }
                
               
            }
        }

        private string _locationToAddTag;
        public string AreaToAddTag
        {
            get => _locationToAddTag;
            set
            {
                _locationToAddTag = value;

                ClearErrors(nameof(AreaToAddTag));

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
        public string AreaToAddDescription
        {
            get => _locationToAddDescription;
            set
            {
                _locationToAddDescription = value;
            }
        }

        private string _locationToAddCategory = "Category 1";
        public string AreaToAddCategory
        {
            get => _locationToAddCategory;
            set
            {
                _locationToAddCategory = value;

                ClearErrors(nameof(AreaToAddCategory));

                if (_locationToAddCategory == null) {
                    AddError(nameof(AreaToAddCategory), "Invalid Category");
                }
                else if (_locationToAddCategory == "") {
                    AddError(nameof(AreaToAddCategory), "Invalid Category");
                }
                else if (IsCategoryValid(_locationToAddCategory)==false) {
                    AddError(nameof(AreaToAddCategory), "Invalid Category");
                }

            }
        }

        private string _locationToAddAreaClass = "Non-Hazardous";
        public string AreaToAddAreaClass
        {
            get => _locationToAddAreaClass;
            set
            {
                if (value != null && value != "") {
                    _locationToAddAreaClass = value;

                    ClearErrors(nameof(AreaToAddAreaClass));

                    if (_locationToAddAreaClass == null) {
                        AddError(nameof(AreaToAddAreaClass), "Invalid Area Class");
                    }
                    else if (_locationToAddAreaClass == "") {
                        AddError(nameof(AreaToAddAreaClass), "Invalid Area Class");
                    }
                    else if (IsAreaClassValid(_locationToAddAreaClass) == false) {
                        AddError(nameof(AreaToAddAreaClass), "Invalid Area Class");
                    }
                }
                
            }
        }

        private string _locationToAddMinTemp = "0";
        public string AreaToAddMinTemp
        {
            get => _locationToAddMinTemp;
            set
            {
                _locationToAddMinTemp = value;

                double minTemp;

                ClearErrors(nameof(AreaToAddMinTemp));

                if (Double.TryParse(_locationToAddMinTemp, out minTemp) == false ) {
                    AddError(nameof(AreaToAddMinTemp), "Invalid value");
                }

                else if (_locationToAddMinTemp == null) {
                    AddError(nameof(AreaToAddMinTemp), "Min Temp cannot be empty");
                }
                else if (_locationToAddMinTemp == "") {
                    AddError(nameof(AreaToAddMinTemp), "Min Temp cannot be empty");
                }
                else if (double.Parse(_locationToAddMinTemp) > double.Parse(_locationToAddMaxTemp)) {
                    AddError(nameof(AreaToAddMinTemp), "Min Temp must be lower than Max Temp");

                }

            }
        }

        private string _locationToAddMaxTemp ="10";
        public string AreaToAddMaxTemp
        {
            get => _locationToAddMaxTemp;
            set
            {
                _locationToAddMaxTemp = value;

                double maxTemp;

                ClearErrors(nameof(AreaToAddMaxTemp));

                if (Double.TryParse(_locationToAddMaxTemp, out maxTemp) == false) {
                    AddError(nameof(AreaToAddMaxTemp), "Invalid value");
                }

                else if (_locationToAddMaxTemp == null) {
                    AddError(nameof(AreaToAddMaxTemp), "Max Temp cannot be empty");
                }
                else if (_locationToAddMaxTemp == "") {
                    AddError(nameof(AreaToAddMaxTemp), "Max Temp cannot be empty");
                }
                else if (double.Parse(_locationToAddMinTemp) > double.Parse(_locationToAddMaxTemp)) {
                    AddError(nameof(AreaToAddMaxTemp), "Max Temp must be higher than Min Temp");

                }

            }
        }

        private string _locationToAddNemaType = "Type 12";
        public string AreaToAddNemaType
        {
            get => _locationToAddNemaType;
            set
            {
                _locationToAddNemaType = value;

                ClearErrors(nameof(AreaToAddNemaType));

                if (_locationToAddNemaType == null) {
                    AddError(nameof(AreaToAddNemaType), "Invalid Nema Type");
                }
                else if (_locationToAddNemaType == "") {
                    AddError(nameof(AreaToAddNemaType), "Invalid Nema Type");
                }
                else if (IsNemaTypeValid(_locationToAddNemaType) == false) {
                    AddError(nameof(AreaToAddNemaType), "Invalid Nema Type");
                }
            }
        }

       
        #endregion

        #region Public Commands

        // Equipment Commands
        public ICommand GetAreasCommand { get; }
        public ICommand SaveAreasCommand { get; }
        public ICommand DeleteAreaCommand { get; }

        public ICommand AddAreaCommand { get; }
        public ICommand GetAreaByIdCommand { get; }


        #endregion
        public AreasViewModel(ListManager listManager)
        {
            _listManager = listManager;

            GetAreasCommand = new RelayCommand(GetAreas);
            SaveAreasCommand = new RelayCommand(SaveAreas);
            DeleteAreaCommand = new RelayCommand(DeleteArea);
            AddAreaCommand = new RelayCommand(AddArea);
            GetAreaByIdCommand = new RelayCommand(GetAreaById);

        }

        public int AreaToGetId { get; set; } = 10;
        public AreaModel AreaReceived { get; set; } = new AreaModel() { Tag = "test" };

        private void GetAreaById()
        {
            AreaReceived = DbManager.GetArea(AreaToGetId);
            if (AreaReceived==null) {
                AreaReceived = new AreaModel() { Tag = "null" };
            }
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

        private void GetAreas()
        {
            ListManager.AreaList.Clear();
            ListManager.AreaList = new ObservableCollection<AreaModel>(DbManager.prjDb.GetRecords<AreaModel>(GlobalConfig.AreaTable));
        }
        private void SaveAreas()
        {
            if (ListManager.AreaList.Count != 0) {

                Tuple<bool, string> update;
                bool error = false;
                string message = "";

                foreach (var location in ListManager.AreaList) {
                    update = DbManager.prjDb.UpsertRecord<AreaModel>(location, GlobalConfig.AreaTable, SaveLists.AreaSaveList);
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
        private void DeleteArea()
        {
            if (_selectedArea != null) {
                DbManager.prjDb.DeleteRecord(GlobalConfig.LoadTable, _selectedArea.Id);
            }
        }
        private void AddArea()
        {
            AreaModel location = new AreaModel();

            //TODO = proper add location validation
            

            bool newAreaIsValid = true;

            //Name
            if (IsNameAvailable(AreaToAddName) == false) {
                newAreaIsValid = false;
            }

            //Tag
            if (IsTagAvailable(AreaToAddTag) == false) {
                newAreaIsValid = false;
            }

            //Category
            if (IsCategoryValid(AreaToAddCategory) == false) {
                newAreaIsValid = false;
            }

            //AreaClass
            if (IsAreaClassValid(AreaToAddAreaClass) == false) {
                newAreaIsValid = false;
            }

            //Temperature
            double outMinTemp;
            double outMaxTemp;
            if (double.TryParse(AreaToAddMinTemp, out outMinTemp) == false || double.TryParse(AreaToAddMaxTemp, out outMaxTemp) == false) {
                newAreaIsValid = false;
            }
            else if (outMinTemp > outMaxTemp) {
                newAreaIsValid = false;
            }

            //NemaRating
            if (IsNemaTypeValid(AreaToAddNemaType) == false) {
                newAreaIsValid = false;
            }

            if (newAreaIsValid) {

                location.Name = _locationToAddName;
                location.Tag = _locationToAddTag;
                location.Description = _locationToAddDescription;
                location.AreaCategory = _locationToAddCategory;
                location.AreaClassification = _locationToAddAreaClass;
                location.MinTemp = double.Parse(_locationToAddMinTemp);
                location.MaxTemp = double.Parse(_locationToAddMaxTemp);
                location.NemaType = _locationToAddNemaType;

                ListManager.AreaList.Add(location);
            }
        }

        #endregion


        #region Helper Methods
        private bool IsNameAvailable(string name)
        {
            if (name == null) {
                return false;
            }

            var locationName = ListManager.AreaList.FirstOrDefault(l => l.Name.ToLower() == name.ToLower());
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

            var locationTag = ListManager.AreaList.FirstOrDefault(l => l.Tag.ToLower() == tag.ToLower());
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