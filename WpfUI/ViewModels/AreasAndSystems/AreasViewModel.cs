using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfUI.Commands;
using WpfUI.Helpers;

namespace WpfUI.ViewModels.AreasAndSystems
{
    [AddINotifyPropertyChangedInterface]
    public class AreasViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        public ObservableCollection<string> NemaTypes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Categories { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AreaClassifications { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<AreaClassificationType> AreaClassificationsInfoTableItems { get; set; } = new ObservableCollection<AreaClassificationType>();


        private ListManager _listManager;

        public ListManager ListManager
        {
            get { return _listManager; }
            set { _listManager = value; }
        }


        private IArea _selectedArea;
        private ObservableCollection<IEquipment> _equipmentList;
        private IEquipment _selectedEquipment;

        public IEquipment SelectedEquipment 
        { 
            get => _selectedEquipment;
            set 
            {
                DteqHeatLossCalculator = new DteqHeatLossCalculator();
                _selectedEquipment = value;
                bool typeCheck = _selectedEquipment is DistributionEquipment;
                if (_selectedEquipment is DistributionEquipment)   {
                    DteqHeatLossCalculator.CalculateHeatLoss((DistributionEquipment)_selectedEquipment);
                    _selectedEquipment.HeatLoss = DteqHeatLossCalculator.TotalHeatLoss;
                }
            } 
        }

        public DteqHeatLossCalculator DteqHeatLossCalculator {get;set;} = new DteqHeatLossCalculator();
        public IArea SelectedArea
        {
            get { return _selectedArea; }
            set
            {
                if (value == null) return;
                _selectedArea = value;


                _selectedArea.HeatLoss = 0;
                _selectedArea.EquipmentList.Clear();

                foreach (var eq in _listManager.CreateEquipmentList()) {
                    eq.HeatLoss = 0;
                    
                    if (eq.Area == _selectedArea) {
                        if (eq is DistributionEquipment) {
                            var dteq = (DistributionEquipment)eq;
                            var dteqHeatLossCalculator = new DteqHeatLossCalculator();
                            dteqHeatLossCalculator.CalculateHeatLoss(dteq);
                            eq.HeatLoss = dteqHeatLossCalculator.TotalHeatLoss;
                        }
                        _selectedArea.HeatLoss += eq.HeatLoss;
                        _selectedArea.EquipmentList.Add(eq);
                    }

                    
                }

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
            AreaToAddValidator = new AreaToAddValidator(_listManager);

            GetAreasCommand = new RelayCommand(GetAreas);
            SaveAreasCommand = new RelayCommand(SaveAreas);
            DeleteAreaCommand = new RelayCommand(DeleteArea);
            AddAreaCommand = new RelayCommand(AddArea);

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

        private async void GetAreas()
        {

            ListManager.GetAreas();
            await Task.Run(() => {
                ListManager.AssignAreas();
            });

        }
        private void SaveAreas()
        {
            if (ListManager.AreaList.Count != 0) {

                try {
                    foreach (AreaModel area in ListManager.AreaList) {
                        DaManager.prjDb.UpsertRecord<AreaModel>(area, GlobalConfig.AreaTable, NoSaveLists.AreaNoSaveList);
                    }
                }
                catch (Exception ex) {
                    ErrorHelper.ShowErrorMessage(ex);
                }
            }
        }
        private void DeleteArea(object areaToDeleteObject)
        {
            if (areaToDeleteObject is null) return;
            AreaModel areaToDelete = (AreaModel)areaToDeleteObject;
            if (areaToDelete.Id == 0) return; //do not delete last area
            if (AreaManager.IsAreaInUse(areaToDelete, _listManager) == true) {
                MessageBox.Show("Cannot delete Area.\n\nThere is equipment assigned to this area. Equipment must be re-assigned to another area.",
                    "Invalid Operation", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            DaManager.prjDb.DeleteRecord(GlobalConfig.AreaTable, areaToDelete.Id);
            _listManager.AreaList.Remove(areaToDelete);
            RefreshAreaTagValidation();

            if (_listManager.AreaList.Count >= 1) {
                var areaCount = _listManager.AreaList.Count;
                SelectedArea = _listManager.AreaList[areaCount - 1];
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
                    newArea.DisplayTag = areaToAdd.DisplayTag;
                    newArea.Name = areaToAdd.Name;
                    newArea.Description = areaToAdd.Description;
                    newArea.AreaCategory = areaToAdd.AreaCategory;
                    newArea.AreaClassification = areaToAdd.AreaClassification;
                    newArea.MinTemp = double.Parse(areaToAdd.MinTemp);
                    newArea.MaxTemp = double.Parse(areaToAdd.MaxTemp);
                    newArea.NemaRating = areaToAdd.NemaRating;


                    newArea.Id = DaManager.prjDb.InsertRecordGetId(newArea, GlobalConfig.AreaTable, NoSaveLists.AreaNoSaveList);

                    ListManager.AreaList.Add(newArea);
                    RefreshAreaTagValidation();
                }
            }
            catch (Exception ex) {
                ErrorHelper.ShowErrorMessage(ex);
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
            Categories.Clear();
            Categories.Add("Category 1");
            Categories.Add("Category 2");

            NemaTypes.Clear();
            foreach (var item in TypeManager.NemaTypes) {
                NemaTypes.Add(item.Type.ToString());
            }

            AreaClassifications.Clear();
            foreach (var item in TypeManager.AreaClassifications) {
                AreaClassifications.Add(item.Zone);
            }
        }

        #endregion

    }

}