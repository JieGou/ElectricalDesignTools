using CefSharp.DevTools.Profiler;
using EdtLibrary.LibraryData.TypeModels;
using EdtLibrary.LibraryData.TypeValidators;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Helpers;

namespace WpfUI.ViewModels.Library
{
    public class LibraryManagerViewModel : ViewModelBase
    {

        public LibraryManagerViewModel()
        {

            //Commands
            GetDataTablesCommand = new RelayCommand(GetDataTables);
            AddLcsCommand = new RelayCommand(AddLcs);
            AddVoltageCommand = new RelayCommand(AddVoltage);
        }


        private ArrayList _dataTableList = new ArrayList();

        public TypeValidatorBase TypeValidator { get; set; }
        public ArrayList DataTableList
        {
            get { return _dataTableList; }
            set { _dataTableList = value; }
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
                }
                else if (_selectedDataTable == "VoltageTypes") {
                    TypeValidator = new VoltageTypeValidator();
                }
                else 
                {
                    TypeValidator = null;
                }
            }
        }
        string _selectedDataTable;


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

        public ICommand GetDataTablesCommand { get; }
        public void GetDataTables()
        {
            DataTableList = DaManager.libDb.GetListOfTablesNamesInDb();
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


        public ICommand AddLcsCommand { get; }
        public void AddLcs(object addOrEdit)
        {
            try {
                var IsValid = TypeValidator.IsValid(); //to help debug
                var errors = TypeValidator._errorDict; //to help debug

                if (IsValid) {

                    if (addOrEdit.ToString() == "Add") {
                        var typeToAdd = TypeValidator.CreateType(new LcsTypeModel());
                        TypeManager.LcsTypes.Add((LcsTypeModel)typeToAdd);
                        typeToAdd.Id = DaManager.libDb.InsertRecordGetId((LcsTypeModel)typeToAdd, SelectedDataTable, new List<string>());
                    }
                    else if (addOrEdit.ToString() == "Edit") {
                        var typeToUpdate = TypeManager.LcsTypes.FirstOrDefault(t => t.Id == TypeValidator.Id);
                        CloneAndAddTimeStamp(TypeValidator, typeToUpdate);
                        DaManager.libDb.UpsertRecord(typeToUpdate, SelectedDataTable, new List<string>());
                    }
                    else if (addOrEdit.ToString() == "Delete") {
                        
                    }
                    ReloadDataTable();
                }
            }
            catch (Exception ex) {
                NotificationHandler.ShowErrorMessage(ex);
            }
        }

        public ICommand AddVoltageCommand { get; }



        public void AddVoltage(object addOrEdit)
        {
            try {
                var IsValid = TypeValidator.IsValid(); //to help debug
                var errors = TypeValidator._errorDict; //to help debug

                if (IsValid) {                                        

                    if (addOrEdit.ToString() == "Add") {
                        var typeToAdd = TypeValidator.CreateType(new VoltageType());
                        TypeManager.VoltageTypes.Add((VoltageType)typeToAdd);
                        typeToAdd.Id =  DaManager.libDb.InsertRecordGetId((VoltageType)typeToAdd, SelectedDataTable, new List<string>());
                    }
                    else if (addOrEdit.ToString() == "Edit") {
                        var typeToUpdate = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Id == TypeValidator.Id);
                        CloneAndAddTimeStamp(TypeValidator, typeToUpdate);
                        DaManager.libDb.UpsertRecord(typeToUpdate, SelectedDataTable, new List<string>());

                    }
                    else if (addOrEdit.ToString() == "Delete") {

                    }
                    ReloadDataTable();

                }

            }
            catch (Exception ex) {
                NotificationHandler.ShowErrorMessage(ex);
            }
        }


        
    }
}
