using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.Areas;
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

        public AreaToAddValidator AreaToAddValidator { get; set; }


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
            AreaToAddValidator  = new AreaToAddValidator(_listManager);

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
        private void DeleteArea(object areaToDeleteObject)
        {
            AreaModel areaToDelete = (AreaModel)areaToDeleteObject;
            DbManager.prjDb.DeleteRecord(GlobalConfig.AreaTable, areaToDelete.Id);
            _listManager.AreaList.Remove(areaToDelete);
        }
        public void AddArea(object areaToAddObject)
        {
            AreaToAddValidator areaToAdd = (AreaToAddValidator)areaToAddObject;
            AreaModel newArea = new AreaModel();
            bool newAreaIsValid = areaToAdd.IsValid();
            var errors = areaToAdd._errorDict;
            if (newAreaIsValid) {

                newArea.Name = areaToAdd.Name;
                newArea.Tag = areaToAdd.Tag;
                newArea.Description = areaToAdd.Description;
                newArea.AreaCategory = areaToAdd.AreaCategory;
                newArea.AreaClassification = areaToAdd.AreaClassification;
                newArea.MinTemp = double.Parse(areaToAdd.MinTemp);
                newArea.MaxTemp = double.Parse(areaToAdd.MaxTemp);
                newArea.NemaType = areaToAdd.NemaType;

                Tuple<bool, string, int> insertResult;
                insertResult = DbManager.prjDb.InsertRecordGetId(newArea, GlobalConfig.AreaTable, SaveLists.AreaSaveList);
                newArea.Id = insertResult.Item3;
                if (insertResult.Item1 == false || newArea.Id == 0) {
                    MessageBox.Show($"ADD NEW AREA   {insertResult.Item2}");
                }
                ListManager.AreaList.Add(newArea);
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