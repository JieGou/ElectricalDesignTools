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
using WpfUI.Helpers;

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

     
        private IArea _selectedArea;
        public IArea SelectedArea
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
            GetAreaByIdCommand = new RelayCommand(TestCommand);

        }

        public int AreaToGetId { get; set; } = 10;
        public AreaModel AreaReceived { get; set; } = new AreaModel() { Tag = "test" };

        private void TestCommand()
        {
            AreaReceived = DaManager.GetArea(AreaToGetId);
            if (AreaReceived==null) {
                AreaReceived = new AreaModel() { Tag = "null" };
            }
            AreaModel areaTest = new AreaModel { Tag = "ML" };
            List<string> list = new List<string>();

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
            ListManager.AreaList = ListManager.GetAreas();
            ListManager.AssignAreas();
        }
        private void SaveAreas()
        {
            if (ListManager.AreaList.Count != 0) {

                try {
                    foreach (AreaModel area in ListManager.AreaList) {
                        DaManager.prjDb.UpsertRecord<AreaModel>(area, GlobalConfig.AreaTable, SaveLists.AreaSaveList);
                    }
                }
                catch (Exception ex) {
                        ErrorHelper.SqlErrorMessage(ex);
                    }
            }
        }
        private void DeleteArea(object areaToDeleteObject)
        {
            AreaModel areaToDelete = (AreaModel)areaToDeleteObject;
            DaManager.prjDb.DeleteRecord(GlobalConfig.AreaTable, areaToDelete.Id);
            _listManager.AreaList.Remove(areaToDelete);
            RefreshAreaTagValidation();

            if (_listManager.AreaList.Count >= 1) {
                var areaCount = _listManager.AreaList.Count;
                SelectedArea = _listManager.AreaList[areaCount-1];
            }
        }
        public void AddArea(object areaToAddObject)
        {
            AreaToAddValidator areaToAdd = (AreaToAddValidator)areaToAddObject;
            AreaModel newArea = new AreaModel();
            try {
                bool newAreaIsValid = areaToAdd.IsValid();
                var errors = areaToAdd._errorDict;
                if (newAreaIsValid) {

                    newArea.Tag = areaToAdd.Tag;
                    newArea.Name = areaToAdd.Name;
                    newArea.Description = areaToAdd.Description;
                    newArea.AreaCategory = areaToAdd.AreaCategory;
                    newArea.AreaClassification = areaToAdd.AreaClassification;
                    newArea.MinTemp = double.Parse(areaToAdd.MinTemp);
                    newArea.MaxTemp = double.Parse(areaToAdd.MaxTemp);
                    newArea.NemaRating = areaToAdd.NemaRating;


                    newArea.Id = DaManager.prjDb.InsertRecordGetId(newArea, GlobalConfig.AreaTable, SaveLists.AreaSaveList);

                    ListManager.AreaList.Add(newArea);
                    RefreshAreaTagValidation();
                }
            }
            catch(Exception ex) {
                ErrorHelper.SqlErrorMessage(ex);
            }
        }

        private void RefreshAreaTagValidation()
        {
            var tag = AreaToAddValidator.Tag;
            AreaToAddValidator.Tag = " ";
            AreaToAddValidator.Tag = tag;
        }

        #endregion


        #region Helper Methods

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