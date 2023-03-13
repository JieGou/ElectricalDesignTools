using CefSharp.DevTools.Profiler;
using EdtLibrary.LibraryData.TypeModels;
using EdtLibrary.LibraryData.TypeValidators;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using Syncfusion.UI.Xaml.Diagram;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Windows.Data.Xml.Dom;
using WpfUI.Commands;
using WpfUI.Converters;
using WpfUI.Helpers;

namespace WpfUI.ViewModels.Library
{
    public class LibraryManagerViewModel : ViewModelBase
    {

        public LibraryManagerViewModel()
        {


            //Commands
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);

        }


        private ObservableCollection<string> _dataTableList = new ObservableCollection<string>();

        public TypeValidatorBase TypeValidator { get; set; }
        public ObservableCollection<string> DataTableList 
        {
            get { return _dataTableList; }
            set { _dataTableList = value; }
        }
        public void GetDataTables()
        {
            var dataTablesList = DaManager.libDb.GetListOfTablesNamesInDb();
            foreach (var dataTable in dataTablesList) {
                DataTableList.Add(dataTable.ToString());
            }

        }
        private ListCollectionView _filteredOptions;
        public ListCollectionView FilteredOptions
        {
            get 
            { 
                if (_filteredOptions == null) {
                    _filteredOptions = new ListCollectionView(DataTableList);
                }
                return _filteredOptions;

            }
            set
            {
                _filteredOptions = value;
                
            }
        }

        public DataTable DataTableToLoad
        {
            get { return _dataTableToLoad; }
            set { _dataTableToLoad = value; }
        }
        DataTable _dataTableToLoad;
        public string SelectedDataTable
        {
            get { return _selectedDataTable; }
            set
            {
                _selectedDataTable = value;
                DataTableToLoad = DaManager.libDb.GetDataTable(_selectedDataTable);
                if (_selectedDataTable == "LocalControlStationTypes") {
                    TypeValidator = new LcsTypeValidator();
                    ModelType = typeof(LcsTypeModel);
                    TypeList = TypeManager.LcsTypes;
                }
                else if (_selectedDataTable == "VoltageTypes") {
                    TypeValidator = new VoltageTypeValidator();
                    ModelType = typeof(VoltageType);
                    TypeList = TypeManager.VoltageTypes;

                }
                else 
                {
                    TypeValidator = null;
                }
            }
        }
        string _selectedDataTable;

        public Type ModelType { get; set; }
        public dynamic TypeList { get; set; }
        public object SelectedTypeDataRow
        {
            get { return _selectedTypeDataRow; }
            set 
            { 
                _selectedTypeDataRow = value;
                if (_selectedTypeDataRow == null) return;
                if (TypeValidator == null) return;

                PopulateValidator(_selectedTypeDataRow);
            }
        }
        private object _selectedTypeDataRow;






        private void PopulateValidator(object selectedRow)
        {
            if (_selectedTypeDataRow == null) return;
            try {
                //Doesn't work becuase this is using DataTables instead of object lists
                var validatorProperties = TypeValidator.GetType().GetProperties();
                var dataRowView = (DataRowView)selectedRow;
                var dataRow = dataRowView.Row;

                foreach (DataColumn column in dataRow.Table.Columns) {

                    foreach (var validatorProp in validatorProperties) {
                        if (column.ColumnName == validatorProp.Name) {
                            try {
                                var type = validatorProp.PropertyType;
                                validatorProp.SetValue(TypeValidator, Convert.ChangeType(dataRow[column.ColumnName],validatorProp.PropertyType));
                            }
                            catch (Exception) {

                            }   
                        }
                    }
                }
            }
            catch (Exception) {

            }
        }

        


        private void ReloadDataTable()
        {
            var selectedDataTable = SelectedDataTable;
            SelectedDataTable = "sqlite_sequence";
            SelectedDataTable = selectedDataTable;
        }
        public void CloneAndAddTimeStamp(object fromObject, object toObject)
        {
            var fromProperties = fromObject.GetType().GetProperties();
            var toProperties = toObject.GetType().GetProperties();


            foreach (var fromProp in fromProperties) {

                foreach (var toProp in toProperties) {
                    if (fromProp.Name == toProp.Name) {
                        toProp.SetValue(toObject, Convert.ChangeType(fromProp.GetValue(fromObject), toProp.PropertyType));
                    }

                }
            }
            ((UserEditableTypeBase)toObject).LastEdited = DateTime.UtcNow;

        }


        public ICommand AddCommand { get; }
        public void Add(object addOrEdit)
        {
            try {
                var IsValid = TypeValidator.IsValid(); //to help debug
                var errors = TypeValidator._errorDict; //to help debug

                if (IsValid) {

                    dynamic instanceToAdd = Activator.CreateInstance(ModelType);
                    instanceToAdd = TypeValidator.CreateType(instanceToAdd);
                    TypeList.Add(instanceToAdd);
                    instanceToAdd.Id = DaManager.libDb.InsertRecordGetId(instanceToAdd, SelectedDataTable, new List<string>());

                    ReloadDataTable();
                }
            }
            catch (Exception ex) {
                NotificationHandler.ShowErrorMessage(ex);
            }
        }

        public ICommand EditCommand { get; }

        public void Edit(object addOrEdit)
        {
            try {
                var IsValid = TypeValidator.IsValid(); //to help debug
                var errors = TypeValidator._errorDict; //to help debug

                if (IsValid) {

                    dynamic instanceToUpdate = null;
                    foreach (dynamic item in TypeList) {
                        if (item.Id == TypeValidator.Id) {
                            instanceToUpdate = item; 
                            break;
                        }
                    }

                    if (instanceToUpdate == null) return;

                    CloneAndAddTimeStamp(TypeValidator, instanceToUpdate);
                    DaManager.libDb.UpsertRecord(instanceToUpdate, SelectedDataTable, new List<string>());
                

                    ReloadDataTable();
                }
            }
            catch (Exception ex) {
                NotificationHandler.ShowErrorMessage(ex);
            }
        }

        public ICommand DeleteCommand { get; }
        public void Delete()
        {
            try {
                var IsValid = TypeValidator.IsValid(); //to help debug
                var errors = TypeValidator._errorDict; //to help debug

                dynamic instanceToDelete = null;
                foreach (dynamic instance in TypeList) {
                    if (instance.Id == TypeValidator.Id) {
                        instanceToDelete = instance;
                        break;
                    }
                }
                if (instanceToDelete == null) return;

                if (instanceToDelete.AddedByUser == true) { 
                    TypeList.Remove(instanceToDelete);
                    DaManager.libDb.DeleteRecord(SelectedDataTable, instanceToDelete.Id);
                }
                
                ReloadDataTable();
                }
            
            catch (Exception ex) {
                NotificationHandler.ShowErrorMessage(ex);
            }
        }
    }
}
