using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.TypeTables;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfUI.Models;
using WpfUI.Services;
using WpfUI.Stores;
using WpfUI.Validators;
using WpfUI.ViewModifiers;

namespace WpfUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class EquipmentViewModel : ViewModelBase, INotifyDataErrorInfo
    {

        #region Constructor

        private ListManager _listManager;
        private StartupService _startupService;
        public ViewModifier DteqGridViewModifier { get; set; }

        public EquipmentViewModel(ListManager listManager)
        {
            _listManager = listManager;
            //_startupService = startupService;

            DteqGridViewModifier = new ViewModifier();
            _loadToAdd = new LoadAdder(DteqList, LoadList, _selectedDteq);

            // Create commands

            ToggleRowViewDteqCommand = new RelayCommand(ToggleRowViewDteq);

            ToggleLoadingViewDteqCommand = new RelayCommand(DteqGridViewModifier.ToggleLoading);
            ToggleOcpdViewDteqCommand = new RelayCommand(DteqGridViewModifier.ToggleOcpd);
            ToggleCableViewDteqCommand = new RelayCommand(DteqGridViewModifier.ToggleCable);

            ToggleOcpdViewLoadCommand = new RelayCommand(TestCommand);



            GetAllCommand = new RelayCommand(DbGetAll);
            SaveAllCommand = new RelayCommand(DbSaveAll);
            SizeCablesCommand = new RelayCommand(SizeCables);
            CalcCableAmpsCommand = new RelayCommand(CalculateCableAmps);

            DeleteDteqCommand = new RelayCommand(DeleteDteq);

            AddDteqCommand = new RelayCommand(AddDteq);
            AddLoadCommand = new RelayCommand(AddLoad);


            ShowAllLoadsCommand = new RelayCommand(ShowAllLoads);
            SaveLoadListCommand = new RelayCommand(SaveLoadList);
            DeleteLoadCommand = new RelayCommand(DeleteLoad);


            CalculateAllCommand = new RelayCommand(CalculateAll);
            //CalculateAllCommand = new RelayCommand(CalculateAll, startupService.IsProjectLoaded);

        }
       
        private void TestCommand()
        {
            _listManager.CreateCableList();
        }

        private System.Windows.GridLength _dteqGridSize = new System.Windows.GridLength(AppSettings.Default.DteqGridSize, GridUnitType.Pixel);
        public System.Windows.GridLength DteqGridSize
        {
            get { return _dteqGridSize; }
            set
            {
                _dteqGridSize = value;
                AppSettings.Default.DteqGridSize = _dteqGridSize.Value;
                AppSettings.Default.Save();
            }
        }
        
        /// <summary>
        /// Default Constructor
        /// </summary>
        public EquipmentViewModel(NavigationBarViewModel navigationBarViewModel, NavigationStore navigationStore)
        {
            NavigationBarViewModel = navigationBarViewModel;

        }

        #endregion


        public ObservableCollection<LocationModel> LocationList
        {
            get { return _listManager.LocationList; }
            set { _listManager.LocationList = value; }
        }

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
        public ObservableCollection<string> DteqTypes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> VoltageTypes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> CableTypes { get; set; } = new ObservableCollection<string>();



        //View States
        #region Views States

        //Dteq
        public string? ToggleRowViewDteqProp { get; set; } = "Collapsed";
        public bool? ToggleLoadingViewDteqProp { get; set; }

        public string? ToggleOcpdViewDteqProp { get; set; } = "Hidden";
        public string? ToggleCableViewDteqProp { get; set; } = "Visible";



        public string? ToggleLoadingViewLoadProp { get; set; } = "Hidden";
        public string? ToggleOcpdViewLoadProp { get; set; } = "Hidden";
        public string? ToggleCableViewLoadProp { get; set; } = "Hidden";


        #endregion  

        // DTEQ

        //TODO - check for and stop duplicate tags in datagrid (might just need an edit/update)
        private ObservableCollection<DteqModel> _dteqList = new ObservableCollection<DteqModel>();
        public ObservableCollection<DteqModel> DteqList

        {
            get { return _dteqList; }

            set
            {
                _dteqList = value;
                _listManager.CreateEqDict();
                _listManager.CreateDteqDict();
                _listManager.DteqList = _dteqList;
            }
        }

        private DteqModel _selectedDteq;
        public DteqModel SelectedDteq
        {
            get { return _selectedDteq; }
            set
            {
                //used for fedfrom Validation
                DictionaryStore.CreateDteqDict(DteqList);
                _selectedDteq = value;
                LoadListLoaded = false;

                if (_selectedDteq != null) {
                    AssignedLoads = new ObservableCollection<IPowerConsumer>(_selectedDteq.AssignedLoads);

                    LoadToAdd.FedFrom = "";
                    LoadToAdd.FedFrom = _selectedDteq.Tag;

                    LoadToAdd.Voltage = _selectedDteq.LoadVoltage.ToString();
                }
            }
        }
        public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> { };

        private DteqToAdd _dteqToAdd;

        public DteqToAdd dteqToadd
        {
            get { return _dteqToAdd; }
            set { _dteqToAdd = value; }
        }


        private string _dteqToAddTag;
        public string DteqToAddTag
        {
            get { return _dteqToAddTag; }
            set
            {
                _dteqToAddTag = value;

                ClearErrors(nameof(DteqToAddTag));
                if (TagValidator.IsTagAvailable(_dteqToAddTag, DteqList, LoadList) == false) {
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
            set 
            {
                DictionaryStore.CreateDteqDict(DteqList);
                _dteqToAddFedFrom = value; 
            }
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
        public ObservableCollection<LoadModel> LoadList
        {
            get { return _loadList; }

            set
            {
                _loadList = value;
                _listManager.CreateEqDict();
                _listManager.CreateILoadDict();
                _listManager.LoadList = _loadList;
                LoadToAdd = new LoadAdder(DteqList,LoadList, _selectedDteq); 
            }
        }
        public bool LoadListLoaded { get; set; }

        private IPowerConsumer _selectedLoad;
        public IPowerConsumer SelectedLoad
        {
            get { return _selectedLoad; }
            set { _selectedLoad = value; }
        }


        private LoadAdder _loadToAdd;

        public LoadAdder LoadToAdd
        {
            get { return _loadToAdd; }
            set { _loadToAdd = value; }
        }

        private string _loadToAddTag;
        public string LoadToAddTag
        {
            get { return _loadToAddTag; }
            set
            {
                _loadToAddTag = value;

                ClearErrors(nameof(LoadToAddTag));
                if (IsTagAvailable(_loadToAddTag, DteqList, LoadList) == false) {
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

        public ObservableCollection<IPowerConsumer> MasterLoadList { get; set; }
        public ObservableCollection<PowerCableModel> CableList { get; set; }


        #endregion


        #region Public Commands

        // Equipment Commands

        public ICommand ToggleRowViewDteqCommand { get; }
        public ICommand ToggleLoadingViewDteqCommand { get; }
        public ICommand ToggleOcpdViewDteqCommand { get; }
        public ICommand ToggleCableViewDteqCommand { get; }


        public ICommand GetAllCommand { get; }
        public ICommand SaveAllCommand { get; }
        public ICommand DeleteDteqCommand { get; }
        public ICommand SizeCablesCommand { get; }
        public ICommand CalcCableAmpsCommand { get; }


        public ICommand AddDteqCommand { get; }
        public ICommand AddLoadCommand { get; }


        // Load Commands
        public ICommand ShowAllLoadsCommand { get; }
        public ICommand SaveLoadListCommand { get; }
        public ICommand DeleteLoadCommand { get; }

        public ICommand CalculateAllCommand { get; }

        public ICommand ToggleOcpdViewLoadCommand { get; }
        #endregion






        #region View Toggles
        //View
        private void ToggleRowViewDteq()
        {
            if (ToggleRowViewDteqProp == "VisibleWhenSelected") {
                ToggleRowViewDteqProp = "Collapsed";
            }
            else if (ToggleRowViewDteqProp == "Collapsed") {
                ToggleRowViewDteqProp = "VisibleWhenSelected";
            }
        }

        private void ToggleLoadingViewDteq()
        {

            if (ToggleLoadingViewDteqProp == true) {
                ToggleLoadingViewDteqProp = false;
            }
            else if (ToggleLoadingViewDteqProp == false) {
                ToggleLoadingViewDteqProp = true;
            }
        }
        #endregion

        #region Command Methods

        // Dteq
        private async void DbGetAll() {

            GlobalConfig.GettingRecords = true;

            _listManager.SetDteq();
            DteqList = _listManager.GetDteq();
            LoadList = _listManager.GetLoads();
            ShowAllLoads();
            CalculateAll();
            _listManager.AssignLoadsToDteq();

            CableList =  _listManager.GetCables();
            _listManager.AssignCables();

            GlobalConfig.GettingRecords = false;
        }

        private void DbSaveAll()
        {
            if (DteqList.Count != 0) {
                CalculateAll();

                Tuple<bool, string> didSaveCorrectly;
                bool error = false;
                string message = "";

                //Dteq
                foreach (var item in DteqList) {
                    var dteqTag = item.Tag;
                    item.Cable.AssignOwner(item);
                    didSaveCorrectly = DbManager.prjDb.UpsertRecord<DteqModel>(item, GlobalConfig.DteqListTable, SaveLists.DteqSaveList);
                    if (didSaveCorrectly.Item1 == false) {
                        error = true;
                        message = didSaveCorrectly.Item2;
                    }
                }

                //Load
                foreach (var item in LoadList) {
                    var dteqTag = item.Tag;
                    item.Cable.AssignOwner(item);
                    didSaveCorrectly = DbManager.prjDb.UpsertRecord<LoadModel>(item, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
                    if (didSaveCorrectly.Item1 == false) {
                        error = true;
                        message = didSaveCorrectly.Item2;
                    }
                }

                //Cables
                _listManager.CreateCableList();
                foreach (var item in _listManager.CableList) {
                    var cableTag = item.Tag;
                    didSaveCorrectly = DbManager.prjDb.UpsertRecord<PowerCableModel>(item, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList);
                    if (didSaveCorrectly.Item1 == false) {
                        error = true;
                        message = didSaveCorrectly.Item2;
                    }
                }
                if (error) {
                    MessageBox.Show(message);
                }
            }
        }
        private void DeleteDteq()
        {
            if (_selectedDteq !=null) {
                DbManager.prjDb.DeleteRecord(GlobalConfig.DteqListTable, _selectedDteq.Id);
                DbManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, _selectedDteq.PowerCableId);
                DteqList.Remove(_selectedDteq);
            }
        }
        private void SizeCables()
        {
            foreach (var item in DteqList) {
                item.SizeCable();
            }
            foreach (var item in LoadList) {
                item.SizeCable();
            }
        }
        private void CalculateCableAmps()
        {
            foreach (var item in DteqList) {
                item.Cable.CalculateAmpacity();
            }
            foreach (var item in LoadList) {
                item.Cable.CalculateAmpacity();
            }
        }

        private void AddDteq()
        {
            // TODO - add Dteq Validation

            bool isTagAvailable = IsTagAvailable(_dteqToAddTag, DteqList, LoadList);

            

            if (isTagAvailable && 
                _dteqToAddTag != "" && 
                _dteqToAddTag != " " && 
                _dteqToAddTag != null) 
            {
                DteqModel dteq = new DteqModel();

                dteq.Tag = _dteqToAddTag;

                DteqList.Add(dteq);

                //TODO = centralize dictionaries
                DictionaryStore.CreateDteqDict(DteqList);

                BuildFedFromList();

                //Refreshes the validation
                    var tag = DteqToAddTag;
                    DteqToAddTag = "";
                    DteqToAddTag = tag;

                CalculateAll();
            }
        }
        public void DteqList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }

        public void AddLoad()
        {
            var IsValid = LoadToAdd.IsValid();
            if (IsValid) {
                LoadModel newLoad = new LoadModel();
                newLoad.Tag = _loadToAdd.Tag;
                newLoad.Category = Categories.LOAD3P.ToString();
                newLoad.Type = _loadToAdd.Type;
                newLoad.Size = Double.Parse(_loadToAdd.Size);
                newLoad.Description = _loadToAdd.Description;
                newLoad.FedFrom = _loadToAdd.FedFrom;
                newLoad.Voltage = Double.Parse(_loadToAdd.Voltage);
                newLoad.Unit = _loadToAdd.Unit;
                newLoad.LoadFactor = Double.Parse(_loadToAdd.LoadFactor);
                newLoad.CalculateLoading();
                newLoad.Id = DbManager.prjDb.InsertRecordGetId(newLoad, GlobalConfig.LoadTable, SaveLists.LoadSaveList).Item3;
                LoadList.Add(newLoad);

                newLoad.SizeCable();
                newLoad.CalculateCableAmps();
                newLoad.Cable.Id = DbManager.prjDb.InsertRecordGetId(newLoad.Cable, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList).Item3;
                CableList.Add(newLoad.Cable);

                CalculateAll();

                //Refresh Tag validation to s
                var tag = _loadToAdd.Tag;
                _loadToAdd.Tag = " ";
                _loadToAdd.Tag = tag;
            }   
        }
        public void LoadList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        // Loads
        public void ShowAllLoads()
        {
            LoadListLoaded = true;
            //LoadList = new ObservableCollection<LoadModel>(DbManager.prjDb.GetRecords<LoadModel>(GlobalConfig.LoadListTable));
            BuildAssignedLoads();
        }
        private void SaveLoadList()
        {

            if (LoadList.Count != 0 && LoadListLoaded == true) {
                CalculateAll();

                Tuple<bool, string> update;
                bool error = false;
                string message = "";

                foreach (var load in LoadList) {
                    update = DbManager.prjDb.UpsertRecord<LoadModel>(load, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
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
        private void DeleteLoad()
        {
           if(_selectedLoad != null) {
                int loadId = _selectedLoad.Id;
                if (_selectedLoad.Cable!= null) {
                    int cableId = _selectedLoad.Cable.Id;
                    DbManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, cableId);
                    CableList.Remove(_selectedLoad.Cable);
                }

                var dteq = DteqList.First(d => d.Tag == _selectedLoad.FedFrom);
                DbManager.prjDb.DeleteRecord(GlobalConfig.LoadTable, loadId);
                

                var loadToRemove = AssignedLoads.FirstOrDefault(load => load.Id == loadId);
                AssignedLoads.Remove(loadToRemove);

                var loadToRemove2 = LoadList.FirstOrDefault(load => load.Id == loadId);
                LoadList.Remove(loadToRemove2);

                if (LoadListLoaded == false) {
                    _selectedDteq.AssignedLoads.Remove(loadToRemove2);
                }
                CalculateAll();
            }
        }


        public void CalculateAll()
        {
            BuildAssignedLoads();
            _listManager.CreateDteqDict(DteqList);
            _listManager.CalculateDteqLoading();
        }
        #endregion


        #region Helper Methods
        private void BuildFedFromList()
        {
            _fedFromList = new ObservableCollection<string>(DteqList.Select(dteq => dteq.Tag).ToList());
            _fedFromList.Insert(0, "UTILITY");
        }
        public bool IsTagAvailable(string tag, ObservableCollection<DteqModel> dteqList, ObservableCollection<LoadModel> loadList) {
            if (tag == null) {
                return false;
            }
            var dteqTag = dteqList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());
            var loadTag = loadList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());

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
        public void CreateComboBoxLists()
        {
            foreach (var item in Enum.GetNames(typeof(EDTLibrary.DteqTypes))) {
                DteqTypes.Add(item.ToString());
            }
            
            foreach (var item in TypeManager.VoltageTypes) {
                VoltageTypes.Add(item.Voltage.ToString());
            }

            foreach (var item in TypeManager.CableTypes) {
                CableTypes.Add(item.Type.ToString());
            }
            //MasterLoadList = new ObservableCollection<IHasLoading>();

        }
        #endregion

        #region Error Validation //INotifyDataErrorInfo
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
        public string Error { get; }

        #endregion

    }

}
