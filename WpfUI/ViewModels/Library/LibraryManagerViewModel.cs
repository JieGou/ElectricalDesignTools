using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.LocalControlStations;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Helpers;

namespace WpfUI.ViewModels.Library
{
    public class LibraryManagerViewModel : ViewModelBase
    {

        public LibraryManagerViewModel()
        {
            LcsToAddValidator = new LcsToAddValidator();


            //Commands
            GetDataTablesCommand = new RelayCommand(GetDataTables);
            AddLcsCommand = new RelayCommand(AddLcs);
        }


        private ArrayList _dataTableList = new ArrayList();
        public LcsToAddValidator LcsToAddValidator { get; set; }

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
                _dataTableToLoad = DaManager.libDb.GetDataTable(_selectedDataTable);
                DataTableToLoad = DaManager.libDb.GetDataTable(_selectedDataTable);
            }
        }

        DataTable _dataTableToLoad;
        public DataTable DataTableToLoad
        {
            get { return _dataTableToLoad; }
            set { _dataTableToLoad = value; }
        }


        
        public ICommand GetDataTablesCommand { get; }
        public void GetDataTables()
        {
            DataTableList = DaManager.libDb.GetListOfTablesNamesInDb();
        }

        public ICommand AddLcsCommand { get; }
        public void AddLcs()
        {
            try {
                var IsValid = LcsToAddValidator.IsValid(); //to help debug
                var errors = LcsToAddValidator._errorDict; //to help debug

                if (IsValid) {

                    LcsTypeModel lcsToAdd = new LcsTypeModel();
                    lcsToAdd.Type = LcsToAddValidator.Type;
                    lcsToAdd.Description = LcsToAddValidator.Description;
                    lcsToAdd.DigitalConductorQty = int.Parse(LcsToAddValidator.DigitalConductorQty);
                    lcsToAdd.AnalogConductorQty = int.Parse(LcsToAddValidator.AnalogConductorQty);
                    TypeManager.LcsTypes.Add(lcsToAdd);
                    DaManager.libDb.InsertRecord<LcsTypeModel>(lcsToAdd, "LocalControlStationTypes", new List<string>());
             
                }

            }
            catch (Exception ex) {
                NotificationHandler.ShowErrorMessage(ex);
            }
        }

    }
}
