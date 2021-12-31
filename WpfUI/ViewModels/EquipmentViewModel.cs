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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinFormCoreUI;
using WpfUI.Commands;
using WpfUI.Services;
using WpfUI.Stores;
using WpfUI.ValidationRules;

namespace WpfUI.ViewModels {

    public class EquipmentViewModel : ViewModelBase, INotifyDataErrorInfo
    {

        #region Properties
        public NavigationBarViewModel NavigationBarViewModel { get; }

        private ObservableCollection<string> _fedFromList;
        public ObservableCollection<string> FedFromList
        {
            get
            {
                _fedFromList = new ObservableCollection<string>(DteqList.Select(dteq => dteq.Tag).ToList());
                _fedFromList.Insert(0, "UTILITY");
                return _fedFromList;
            }
            set { }
        }

        //Type Lists
        public List<string> DteqTypes { get; set; } = new List<string>();
        public List<string> VoltageTypes { get; set; } = new List<string>();

        // DTEQ

        private ObservableCollection<DteqModel> _dteqList = new ObservableCollection<DteqModel>();
        public ObservableCollection<DteqModel> DteqList
        {
            get { return _dteqList;  }

            set
            {
                _dteqList = value;
            }
        }

        private DteqModel _selectedDteq;
        public DteqModel SelectedDteq
        {
            get { return _selectedDteq; }
            set
            {
                _selectedDteq = value;
                LoadListLoaded = false;

                if (_selectedDteq != null) {
                    AssignedLoads = new ObservableCollection<ILoadModel>(_selectedDteq.AssignedLoads);

                    LoadToAddFedFrom = "";
                    LoadToAddFedFrom = _selectedDteq.Tag;

                    LoadToAddVoltage = _selectedDteq.Voltage.ToString();
                }

            }
        }
        public ObservableCollection<ILoadModel> AssignedLoads { get; set; } = new ObservableCollection<ILoadModel> { };


        private string _dteqToAddTag;
        public string DteqToAddTag
        {
            get { return _dteqToAddTag; }
            set
            {
                _dteqToAddTag = value;

                ClearErrors(nameof(DteqToAddTag));
                if (IsTagAvailable(_dteqToAddTag) == false) {
                    AddError(nameof(DteqToAddTag), "Tag already exists");
                }
                else if (_dteqToAddTag == "") { // TODO - create method for invalid tags
                    AddError(nameof(DteqToAddTag), "Tag cannot be empty");
                }
            }
        }

        private string _dteqToAddType;
        public string DteqToAddType 
        {
            get { return _dteqToAddType; }
            set 
            {
                _dteqToAddType = value;
                if (_dteqToAddType == EDTLibrary.DteqTypes.XFR.ToString()) {
                    DteqToAddUnit = "";
                    DteqToAddUnit = Units.kVA.ToString();
                    _dteqToAddUnit = Units.kVA.ToString();
                }
                else{
                    DteqToAddUnit = "";
                    DteqToAddUnit = Units.A.ToString();
                    _dteqToAddUnit = Units.A.ToString();
                }
            }
        }

        private string _dteqToAddFedFrom;
        public string DteqToAddFedFrom
        {
            get { return _dteqToAddFedFrom; }
            set { _dteqToAddFedFrom = value; }
        }

        private string _dteqToAddVoltage;
        public string DteqToAddVoltage
        {
            get { return _dteqToAddVoltage; }
            set { _dteqToAddVoltage = value; }
        }

        private double _dteqToAddSize;
        public double DteqToAddSize
        {
            get { return _dteqToAddSize; }
            set { _dteqToAddSize = value; }
        }

        private string _dteqToAddUnit;
        public string DteqToAddUnit
        {
            get { return _dteqToAddUnit; }
            set 
            { 
                _dteqToAddUnit = value;

                ClearErrors(nameof(DteqToAddUnit));
                if (_dteqToAddType != "" && _dteqToAddUnit == "") {
                    AddError(nameof(DteqToAddUnit), "Select valid Unit");
                }
                else if (_dteqToAddType == EDTLibrary.DteqTypes.XFR.ToString() && _dteqToAddUnit != Units.kVA.ToString()) {
                    AddError(nameof(DteqToAddUnit), "Incorrect Units for Equipment");
                }
                
            }
        }

        private string _dteqToAddDescription;
        public string DteqToAddDescription
        {
            get { return _dteqToAddDescription; }
            set { _dteqToAddDescription = value; }
        }

        // LOADS

        private ObservableCollection<LoadModel> _loadList = new ObservableCollection<LoadModel>();
        public ObservableCollection<LoadModel> LoadList { get; set; }
        public bool LoadListLoaded { get; set; }

        private ILoadModel _selectedLoad;
        public ILoadModel SelectedLoad
        {
            get { return _selectedLoad; }
            set { _selectedLoad = value; }
        }


        private string _loadToAddTag;
        public string LoadToAddTag
        {
            get { return _loadToAddTag; }
            set
            {
                _loadToAddTag = value;

                ClearErrors(nameof(LoadToAddTag));
                if (IsTagAvailable(_loadToAddTag) == false) {
                    AddError(nameof(LoadToAddTag), "Tag already exists");
                }
                else if (_loadToAddTag == "") { // TODO - create method for invalid tags
                    AddError(nameof(LoadToAddTag), "Tag cannot be empty");
                }
            }
        }

        private string _loadToAddType;
        public string LoadToAddType
        {
            get { return _loadToAddType; }
            set
            {
                _loadToAddType = value;
                _loadToAddLoadFactor = "0.8";
                LoadToAddLoadFactor = "";
                LoadToAddLoadFactor = "0.8";

                if (_loadToAddType == LoadTypes.TRANSFORMER.ToString()) {
                    LoadToAddUnit = ""; 
                    LoadToAddUnit = Units.kVA.ToString();
                    _loadToAddUnit = Units.kVA.ToString();

                }
                else if (_loadToAddType == LoadTypes.MOTOR.ToString()) {
                    LoadToAddUnit = ""; 
                    LoadToAddUnit = Units.HP.ToString();
                    _loadToAddUnit = Units.HP.ToString();

                }
                else if (_loadToAddType == LoadTypes.HEATER.ToString()) {
                    LoadToAddUnit = ""; 
                    LoadToAddUnit = Units.kW.ToString();
                    _loadToAddUnit = Units.kW.ToString();

                }
                else if (_loadToAddType == LoadTypes.WELDING.ToString()) {
                    LoadToAddUnit = "";
                    LoadToAddUnit = Units.A.ToString();
                    _loadToAddUnit = Units.A.ToString();

                }
                else if (_loadToAddType == LoadTypes.PANEL.ToString()) {
                    LoadToAddUnit = "";
                    LoadToAddUnit = Units.A.ToString();
                    _loadToAddUnit = Units.A.ToString();

                }
                else if (_loadToAddType =="") {
                    LoadToAddUnit = "";
                    _loadToAddUnit = "";
                }
                LoadToAddUnit = _loadToAddUnit;
            }
        }

        private string _loadToAddFedFrom;
        public string LoadToAddFedFrom
        {
            get { return _loadToAddFedFrom; }
            set { _loadToAddFedFrom = value; }
        }

        private string _loadToAddVoltage;
        public string LoadToAddVoltage
        {
            get { return _loadToAddVoltage; }
            set 
            { 
                _loadToAddVoltage = value;

                double parsedVoltage;
                ClearErrors(nameof(LoadToAddVoltage));
                if(_selectedDteq != null && _loadToAddVoltage != null) {
                    if (double.TryParse(_loadToAddVoltage.ToString(), out parsedVoltage) == true) {
                        //AddError(nameof(LoadToAddVoltage), "Invalid Voltage");

                        if (_loadToAddVoltage == null) {
                            AddError(nameof(LoadToAddVoltage), "Voltage does not match supply Equipment");
                        }

                        else if (_loadToAddVoltage != _selectedDteq.Voltage.ToString()) {
                            AddError(nameof(LoadToAddVoltage), "Voltage does not match supply Equipment");
                        }
                    }
                }
            }
        }

        private string _loadToAddSize;
        public string LoadToAddSize
        {
            get { return _loadToAddSize; }
            set
            {
                double newLoad_LoadSize;
                _loadToAddSize = value;

                // TODO - enforce Motor sizes
                ClearErrors(nameof(LoadToAddSize));
                if (double.TryParse(_loadToAddSize, out newLoad_LoadSize) == false) {
                    AddError(nameof(LoadToAddSize), "Invalid Size");
                }
                else if (double.Parse(_loadToAddSize) <= 0) {
                    AddError(nameof(LoadToAddSize), "Value must be larger than 0");
                }
            }
        }

        private string _loadToAddUnit;
        public string LoadToAddUnit
        {
            get { return _loadToAddUnit; }
            set
            {
                _loadToAddUnit = value;

                ClearErrors(nameof(LoadToAddUnit));
                if (_loadToAddType != "" && _loadToAddType == null || _loadToAddUnit == "") {
                    AddError(nameof(LoadToAddUnit), "Select valid Unit");
                }
                else if (_loadToAddType == EDTLibrary.LoadTypes.TRANSFORMER.ToString() && _loadToAddUnit != Units.kVA.ToString()) {
                    AddError(nameof(LoadToAddUnit), "Incorrect Units for Equipment");
                }
                else if (_loadToAddType == EDTLibrary.LoadTypes.MOTOR.ToString() 
                    && _loadToAddUnit != Units.HP.ToString() && _loadToAddUnit != Units.kW.ToString()) {
                    AddError(nameof(LoadToAddUnit), "Incorrect Units for Equipment");
                }

                else if (_loadToAddType == EDTLibrary.LoadTypes.HEATER.ToString() && _loadToAddUnit != Units.kW.ToString()) {
                    AddError(nameof(LoadToAddUnit), "Incorrect Units for Equipment");
                }
            }
                
        }

        private string _loadToAddDescription;
        public string LoadToAddDescription
        {
            get { return _loadToAddDescription; }
            set { _loadToAddDescription = value; }
        }
        private string _loadToAddLoadFactor = "0.8";
        public string LoadToAddLoadFactor
        {
            get { return _loadToAddLoadFactor; }
            set 
            {
                double newLoad_LoadFactor;
                ClearErrors(nameof(LoadToAddLoadFactor));

                _loadToAddLoadFactor = value;
                if (double.TryParse(_loadToAddLoadFactor, out newLoad_LoadFactor) == false) {
                    AddError(nameof(LoadToAddLoadFactor), "Value must be between 0 and 1");

                }
                else if (double.Parse(_loadToAddLoadFactor) < 0 || double.Parse(_loadToAddLoadFactor) > 1) {
                    AddError(nameof(LoadToAddLoadFactor), "Value must be between 0 and 1");
                }
            }
        }


        public ObservableCollection<ILoadModel> MasterLoadList { get; set; }
        #endregion

       


        #region Public Commands

        // Equipment Commands
        public ICommand GetDteqCommand { get; }
        public ICommand CalculateDteqCommand { get; }
        public ICommand SaveDteqListCommand { get; }
        public ICommand DeleteDteqListCommand { get; }


        public ICommand AddDteqCommand { get; }
        public ICommand AddLoadCommand { get; }


        // Load Commands
        public ICommand ShowLoadListCommand { get; }
        public ICommand SaveLoadListCommand { get; }
        public ICommand DeleteLoadCommand { get; }

        #endregion

        #region Constructor
        public EquipmentViewModel()
        {
            
            // Create commands
            GetDteqCommand = new RelayCommand(GetDteq);
            CalculateDteqCommand = new RelayCommand(CalculateDteq);
            SaveDteqListCommand = new RelayCommand(SaveDteq);
            DeleteDteqListCommand = new RelayCommand(DeleteDteq);

            AddDteqCommand = new RelayCommand(AddDteq);
            AddLoadCommand = new RelayCommand(AddLoad);


            ShowLoadListCommand = new RelayCommand(ShowLoadList);
            SaveLoadListCommand = new RelayCommand(SaveLoadList);
            DeleteLoadCommand = new RelayCommand(DeleteLoad);

        }

        




        /// <summary>
        /// Default Constructor
        /// </summary>
        public EquipmentViewModel(NavigationBarViewModel navigationBarViewModel, NavigationStore navigationStore)
        {
            NavigationBarViewModel = navigationBarViewModel;

        }


        #endregion


        #region Error Validation

        // INotifyDataErrorInfo
        public bool HasErrors => _errorDict.Any();
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public readonly Dictionary<string, List<string>> _errorDict  = new Dictionary<string, List<string>>();

        private void ClearErrors(string propertyName) {
            _errorDict.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }

        public void AddError(string propertyName, string errorMessage) {
            if (!_errorDict.ContainsKey(propertyName)) { // check if error Key exists
                _errorDict.Add(propertyName, new List<string>()); // create if not
            }
            _errorDict[propertyName].Add(errorMessage); //add error message to list of error messages
            OnErrorsChanged(propertyName);
        }

        public IEnumerable GetErrors(string? propertyName) {
            return _errorDict.GetValueOrDefault(propertyName, null);
        }

        private void OnErrorsChanged(string? propertyName) {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        public string Error { get; }

        #endregion


        #region Command Methods

        // Dteq
        private void GetDteq() {
            DteqList = new ObservableCollection<DteqModel>(DbManager.prjDb.GetRecords<DteqModel>(GlobalConfig.dteqListTable));
        }
        public void CalculateDteq()
        {
            ListManager.AssignLoadsToDteq(DteqList, LoadList);
            foreach (var dteq in DteqList) {
                dteq.CalculateLoading();
            }
        }
        private void SaveDteq()
        {
            //if (DteqList.Count !=0) {
            //    DbManager.prjDb.DeleteAllRecords(GlobalConfig.dteqListTable);
            //    foreach (var dteq in DteqList) {
            //        DbManager.prjDb.InsertRecord<DteqModel>(dteq, GlobalConfig.dteqListTable, SaveLists.DteqSaveList);
            //    }
            //}

            foreach (var dteq in DteqList) {
                DbManager.prjDb.UpsertRecord<DteqModel>(dteq, GlobalConfig.dteqListTable, SaveLists.DteqSaveList);
            }
            CalculateDteq();
        }
        private void DeleteDteq()
        {
            if (_selectedDteq !=null) {
                DbManager.prjDb.DeleteRecord(GlobalConfig.dteqListTable, _selectedDteq.Id);
                DteqList.Remove(_selectedDteq);
            }
        }


        private void AddDteq()
        {
            // TODO - methods for invalid tags
            if (IsTagAvailable(_dteqToAddTag) && _dteqToAddTag != "" && _dteqToAddTag != " " && _dteqToAddTag != null) {
                DteqList.Add(new DteqModel() { Tag = _dteqToAddTag });

                CreateMasterLoadList();
                DictionaryStore.CreateDteqDict(DteqList);

                BuildFedFromList();

                //Refreshes the validation
                var tag = DteqToAddTag;
                DteqToAddTag = "";
                DteqToAddTag = tag;
            }
        }
        private void AddLoad()
        {
            bool newLoadIsValid = true;
            LoadModel newLoad = new LoadModel();

            //Tag
            if (IsTagAvailable(_loadToAddTag) && _loadToAddTag != "" && _loadToAddTag != " " && _loadToAddTag != null) {
                newLoadIsValid = false;
            }

            //Type
            if (_loadToAddType == "" || _loadToAddType == null) {
                newLoadIsValid = false;
            }

            //Motor Voltages
            if ( Double.TryParse(_loadToAddVoltage, out double sasdf) ==false) {
                newLoadIsValid=false;
            }
            if (_loadToAddType == LoadTypes.MOTOR.ToString()) {
                if (_loadToAddVoltage == "600") {
                    _loadToAddVoltage = "575";
                }
                else if (_loadToAddVoltage == "480") {
                    _loadToAddVoltage = "460";
                }
            }



            //Unit
            if (   (_loadToAddUnit == "")  
                || (_loadToAddType == EDTLibrary.LoadTypes.TRANSFORMER.ToString() && _loadToAddUnit != Units.kVA.ToString())
                || (_loadToAddType == EDTLibrary.LoadTypes.MOTOR.ToString() && _loadToAddUnit != Units.HP.ToString() || _loadToAddUnit != Units.kW.ToString())
                || (_loadToAddType == EDTLibrary.LoadTypes.HEATER.ToString() && _loadToAddUnit != Units.kW.ToString())  )
            {
                newLoadIsValid = false;
            }

            //Size
            //TODO - Enforce valid Motor Sizes
            double newLoad_LoadSize;
            if (double.TryParse(_loadToAddSize, out newLoad_LoadSize) == false) {
                newLoadIsValid = false;
            }
            else if (double.Parse(_loadToAddSize) <=0) {
                newLoadIsValid = false;
            }
                      
            //LoadFactor
            double newLoad_LoadFactor;
            if (double.TryParse(_loadToAddLoadFactor, out newLoad_LoadFactor) == false) {
                newLoadIsValid = false;
            }
            else if(double.Parse(_loadToAddLoadFactor) <0 || double.Parse(_loadToAddLoadFactor) > 1) {
                newLoadIsValid = false;
            }

            newLoad.Tag = _loadToAddTag;
            newLoad.Type = _loadToAddType;
            newLoad.Description = _loadToAddDescription;
            newLoad.FedFrom = _loadToAddFedFrom;
            newLoad.Voltage = Double.Parse(_loadToAddVoltage);
            newLoad.Size = Double.Parse(_loadToAddSize);
            newLoad.Unit = _loadToAddUnit;
            newLoad.LoadFactor = Double.Parse(_loadToAddLoadFactor);


            if (newLoadIsValid == true) {
                newLoad.CalculateLoading();
                LoadList.Add(newLoad);

                BuildAssignedLoads();
                CreateMasterLoadList();

                //Refreshes the validation
                var tag = LoadToAddTag;
                LoadToAddTag = " ";
                //LoadTagToAdd = tag;
                CalculateDteq();
            }

        }


        // Loads
        private void ShowLoadList()
        {
            LoadListLoaded = true;
            //LoadList = new ObservableCollection<LoadModel>(DbManager.prjDb.GetRecords<LoadModel>(GlobalConfig.loadListTable));
            BuildAssignedLoads();
        }
        private void SaveLoadList()
        {
            if (LoadList.Count != 0 && LoadListLoaded==true) {
                foreach (var load in LoadList) {
                    DbManager.prjDb.UpsertRecord<LoadModel>(load, GlobalConfig.loadListTable, SaveLists.LoadSaveList);
                }
            }
            CalculateDteq();
        }
        private void DeleteLoad()
        {
           if(_selectedLoad != null) {
                int loadId = _selectedLoad.Id;
                DbManager.prjDb.DeleteRecord(GlobalConfig.loadListTable, _selectedLoad.Id);

                var loadToRemove = AssignedLoads.FirstOrDefault(load => load.Id == loadId);
                AssignedLoads.Remove(loadToRemove);

                var loadToRemove2 = LoadList.FirstOrDefault(load => load.Id == loadId);
                LoadList.Remove(loadToRemove2);

                if (LoadListLoaded == false) {
                    _selectedDteq.AssignedLoads.Remove(loadToRemove2);
                }
            }
            //BuildAssignedLoads();
        }

        #endregion

        #region Helper Methods


        public void AddLoadErrors()
        {
            //ClearErrors(nameof(DteqToAddUnit));
            //if (_dteqToAddType== EDTLibrary.DteqTypes.XFR.ToString() && _dteqToAddUnit != Units.kVA.ToString()) {
            //    AddError(nameof(DteqToAddUnit), "Incorrect Units for Equipmet");
            //}
        }
        private void BuildFedFromList()
        {
            _fedFromList = new ObservableCollection<string>(DteqList.Select(dteq => dteq.Tag).ToList());
            _fedFromList.Insert(0, "UTILITY");
        }

        private bool IsTagAvailable(string tag) {
            if (tag == null) {
                return false;
            }
            var dteqTag = DteqList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());
            var loadTag = LoadList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());

            if (dteqTag != null || 
                loadTag != null) {
                return false;
            }
            
            return true;
        }

        private void BuildAssignedLoads()
        {
            AssignedLoads.Clear();
            foreach (var load in LoadList) {
                AssignedLoads.Add(load);
            }
            LoadListLoaded = true;
        }
        private void CreateMasterLoadList() {
            MasterLoadList.Clear();
            foreach (var dteq in DteqList) {
                MasterLoadList.Add(dteq);
            }
            foreach (var load in LoadList) {
                MasterLoadList.Add(load);
            }
        }
        public void CreateComboBoxLists()
        {
            foreach (var item in Enum.GetNames(typeof(EDTLibrary.DteqTypes))) {
                DteqTypes.Add(item.ToString());
            }
            
            foreach (var item in TypeManager.VoltageTypes) {
                VoltageTypes.Add(item.Voltage.ToString());
            }

            //Instatiates the required properties
            //TODO = FigureOut MasterLoad List

            MasterLoadList = new ObservableCollection<ILoadModel>();

        }

        #endregion

    }

}
