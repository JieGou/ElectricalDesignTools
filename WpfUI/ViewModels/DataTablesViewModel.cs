﻿using EDTLibrary.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUI.Commands;

namespace WpfUI.ViewModels
{
    public class DataTablesViewModel : ViewModelBase
    {
        private ArrayList _dataTableList = new ArrayList();

        public ArrayList DataTableList
        {
            get { return _dataTableList; }
            set { _dataTableList = value; }
        }

        string _selectedDataTable;
        public string SelectedDataTable 
        { 
            get { return _selectedDataTable; } 
            set 
            { 
                _selectedDataTable = value; 
                _dataTableToLoad = DbManager.libDb.GetDataTable(_selectedDataTable);
                DataTableToLoad = DbManager.libDb.GetDataTable(_selectedDataTable);
            }
        }

        DataTable _dataTableToLoad;
        public DataTable DataTableToLoad 
        {
            get { return _dataTableToLoad; }
            set { _dataTableToLoad = value; }
        }


        public DataTablesViewModel()
        {
            GetDataTablesCommand = new RelayCommand(GetDataTables);
        }

        public ICommand GetDataTablesCommand { get; }


        public void GetDataTables()
        {
            _dataTableList = DbManager.libDb.GetListOfTablesNamesInDb();
            DataTableList = DbManager.libDb.GetListOfTablesNamesInDb();
        }

    }
}
