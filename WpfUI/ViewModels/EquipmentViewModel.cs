using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Models.Validators;
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
using WpfUI.Commands;
using WpfUI.Services;
using WpfUI.Stores;
using WpfUI.ViewModifiers;

namespace WpfUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class EquipmentViewModel : ViewModelBase, INotifyDataErrorInfo
    {

        #region Constructor

        private ListManager _listManager;
        public ListManager ListManager
        {
            get { return _listManager; }
            set { _listManager = value; }
        }

        private StartupService _startupService;
        public ViewModifier DteqGridViewModifier { get; set; }

        public EquipmentViewModel(ListManager listManager)
        {
            //fields
            _listManager = listManager;

            //members
            DteqGridViewModifier = new ViewModifier();

            DteqToAdd = new DteqToAddValidator(_listManager, _selectedDteq);
            LoadToAdd = new LoadToAddValidator(_listManager, _selectedDteq);

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

        private System.Windows.GridLength _loadGridWidth = new System.Windows.GridLength(AppSettings.Default.LoadGridWidth, GridUnitType.Pixel);
        public System.Windows.GridLength LoadGridWidth
        {
            get { return _loadGridWidth; }
            set
            {
                _loadGridWidth = value;
                AppSettings.Default.LoadGridWidth = _loadGridWidth.Value;
                AppSettings.Default.Save();
            }
        }
        private System.Windows.GridLength _loadGridHeight = new System.Windows.GridLength(AppSettings.Default.LoadGridHeight, GridUnitType.Pixel);
        public System.Windows.GridLength LoadGridHeight
        {
            get { return _loadGridHeight; }
            set
            {
                _loadGridHeight = value;
                AppSettings.Default.LoadGridHeight = _loadGridHeight.Value;
                AppSettings.Default.Save();

                LoadGridActualHeight = _loadGridHeight.Value - 20;
            }
        }
        public double LoadGridActualHeight { get; set; }
        #endregion


        #region Properties
        public NavigationBarViewModel NavigationBarViewModel { get; }

        //ComboBox Lists
        private ObservableCollection<string> _fedFromList; //used only on View
        public ObservableCollection<string> FedFromList
        {
            get
            {
                _fedFromList = new ObservableCollection<string>(_listManager.DteqList.Select(dteq => dteq.Tag).ToList());
                _fedFromList.Insert(0, "UTILITY");
                return _fedFromList;
            }
            set { }
        } //Used only on View
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

        //TODO - check for and stop duplicate tags in datagrid (might just need an edit/update)


        // DTEQ
        private ObservableCollection<DteqModel> _dteqList = new ObservableCollection<DteqModel>();
        //public ObservableCollection<DteqModel> _listManager.DteqList

        //{
        //    get { return _dteqList; }

        //    set
        //    {
        //        _dteqList = value;
        //        _listManager.CreateEqDict();
        //        _listManager.CreateDteqDict();
        //        _listManager.DteqList = _dteqList;
        //        DteqToAdd = new DteqToAddValidator(_listManager.DteqList, LoadList, _selectedDteq);
        //    }
        //}

        private DteqModel _selectedDteq;
        public DteqModel SelectedDteq
        {
            get { return _selectedDteq; }
            set
            {
                //used for fedfrom Validation
                DictionaryStore.CreateDteqDict(_listManager.DteqList);
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

        public DteqToAddValidator DteqToAdd { get; set; }

        // LOADS
        private ObservableCollection<LoadModel> _loadList = new ObservableCollection<LoadModel>();
        //public ObservableCollection<LoadModel> LoadList
        //{
        //    get { return _loadList; }

        //    set
        //    {
        //        _loadList = value;
        //        _listManager.CreateEqDict();
        //        _listManager.CreateILoadDict();
        //        _listManager.LoadList = _loadList;
        //        LoadToAdd = new LoadToAddValidator(_listManager.DteqList,LoadList, _selectedDteq); 
        //    }
        //}
        public bool LoadListLoaded { get; set; }
        private IPowerConsumer _selectedLoad;
        public IPowerConsumer SelectedLoad
        {
            get { return _selectedLoad; }
            set { _selectedLoad = value; }
        }
        public LoadToAddValidator LoadToAdd { get; set; }
   

        //Cables


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
        public async void DbGetAll() {

            GlobalConfig.GettingRecords = true;

            _listManager.SetDteq();
            _listManager.GetDteq();
            _listManager.GetLoads();
            ShowAllLoads();
            CalculateAll();
            _listManager.AssignLoadsToDteq();

            _listManager.GetCables();
            _listManager.AssignCables();
            DteqToAdd = new DteqToAddValidator(_listManager, _selectedDteq);
            LoadToAdd = new LoadToAddValidator(_listManager, _selectedDteq);

            GlobalConfig.GettingRecords = false;
        }

        private void DbSaveAll()
        {
            if (_listManager.DteqList.Count != 0) {
                CalculateAll();

                Tuple<bool, string> dbSaveResult;
                bool error = false;
                string message = "";

                //Dteq
                foreach (var item in _listManager.DteqList) {
                    var dteqTag = item.Tag;
                    item.Cable.AssignOwner(item);
                    dbSaveResult = DbManager.prjDb.UpsertRecord<DteqModel>(item, GlobalConfig.DteqTable, SaveLists.DteqSaveList);
                    if (dbSaveResult.Item1 == false) {
                        error = true;
                        message = dbSaveResult.Item2;
                    }
                }

                //Load
                foreach (var item in _listManager.LoadList) {
                    var dteqTag = item.Tag;
                    item.Cable.AssignOwner(item);
                    dbSaveResult = DbManager.prjDb.UpsertRecord<LoadModel>(item, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
                    if (dbSaveResult.Item1 == false) {
                        error = true;
                        message = dbSaveResult.Item2;
                    }
                }

                //Cables
                _listManager.CreateCableList();
                foreach (var item in _listManager.CableList) {
                    var cableTag = item.Tag;
                    dbSaveResult = DbManager.prjDb.UpsertRecord<PowerCableModel>(item, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList);
                    if (dbSaveResult.Item1 == false) {
                        error = true;
                        message = dbSaveResult.Item2;
                    }
                }
                if (error) {
                    MessageBox.Show(message);
                }
            }
        }
        private void SizeCables()
        {
            foreach (var item in _listManager.DteqList) {
                item.SizeCable();
            }
            foreach (var item in _listManager.LoadList) {
                item.SizeCable();
            }
        }
        private void CalculateCableAmps()
        {
            foreach (var item in _listManager.DteqList) {
                item.Cable.CalculateAmpacity();
            }
            foreach (var item in _listManager.LoadList) {
                item.Cable.CalculateAmpacity();
            }
        }

        private void AddDteq()
        {
            var test = _listManager.DteqList;
            var errors = DteqToAdd._errorDict;
            var IsValid = DteqToAdd.IsValid();
            if (IsValid) {
                DteqModel newDteq = new DteqModel();
                newDteq.Tag = DteqToAdd.Tag;
                newDteq.Category = Categories.DTEQ.ToString();
                newDteq.Type = DteqToAdd.Type;

                newDteq.Size = Double.Parse(DteqToAdd.Size);
                newDteq.Unit = DteqToAdd.Unit;
                newDteq.Description = DteqToAdd.Description;
                newDteq.FedFrom = DteqToAdd.FedFrom;
                newDteq.LineVoltage = Double.Parse(DteqToAdd.LineVoltage);
                newDteq.LoadVoltage = Double.Parse(DteqToAdd.LoadVoltage);

                DteqModel dteqSubscriber = _listManager.DteqList.FirstOrDefault(d => d.Tag == newDteq.FedFrom);
                if (dteqSubscriber != null) {
                    newDteq.LoadingCalculated += dteqSubscriber.OnDteqLoadingCalculated;
                    newDteq.LoadingCalculated += DbManager.OnDteqLoadingCalculated;
                }
                newDteq.Id = DbManager.prjDb.InsertRecordGetId(newDteq, GlobalConfig.DteqTable, SaveLists.DteqSaveList).Item3;
                _listManager.DteqList.Add(newDteq);

                newDteq.SizeCable();
                newDteq.CalculateCableAmps();
                newDteq.Cable.Id = DbManager.prjDb.InsertRecordGetId(newDteq.Cable, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList).Item3;
                _listManager.CableList.Add(newDteq.Cable);
                RefreshDteqTagValidation();
            }
        }


        private void DeleteDteq()
        {
            if (_selectedDteq != null) {
                //children first

                if (_selectedDteq.Cable != null) {
                    int cableId = _selectedDteq.Cable.Id;
                    DbManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, cableId);
                    _listManager.CableList.Remove(_selectedDteq.Cable);
                }

                foreach (var load in _selectedDteq.AssignedLoads) {
                    load.Cable.Destination = "Dteq Deleted";
                    load.Cable.CreateTag();

                }

                DbManager.prjDb.DeleteRecord(GlobalConfig.DteqTable, _selectedDteq.Id);
                DbManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, _selectedDteq.PowerCableId);
                _listManager.DteqList.Remove(_selectedDteq);
                RefreshDteqTagValidation();
                BuildAssignedLoads();
                SelectedDteq = _listManager.DteqList[_listManager.DteqList.Count - 1];
            }
        }

        public void DteqList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }
        public void LoadList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }

        public void AddLoad()
        {
            var IsValid = LoadToAdd.IsValid();
            if (IsValid) {
                LoadModel newLoad = new LoadModel();
                newLoad.Tag = LoadToAdd.Tag;
                newLoad.Category = Categories.LOAD3P.ToString();
                newLoad.Type = LoadToAdd.Type;
                newLoad.Size = Double.Parse(LoadToAdd.Size);
                newLoad.Description = LoadToAdd.Description;
                newLoad.FedFrom = LoadToAdd.FedFrom;
                newLoad.Voltage = Double.Parse(LoadToAdd.Voltage);
                newLoad.Unit = LoadToAdd.Unit;
                newLoad.LoadFactor = Double.Parse(LoadToAdd.LoadFactor);

                DteqModel dteqSubscriber = _listManager.DteqList.FirstOrDefault(d => d.Tag == newLoad.FedFrom);
                if (dteqSubscriber != null) {
                    dteqSubscriber.AssignedLoads.Add(newLoad);
                    newLoad.LoadingCalculated += dteqSubscriber.OnDteqLoadingCalculated;
                }
                newLoad.CalculateLoading();

                Tuple<bool, string, int> insertResult;

                insertResult = DbManager.prjDb.InsertRecordGetId(newLoad, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
                newLoad.Id = insertResult.Item3;
                if (insertResult.Item1 == false || newLoad.Id ==0) {
                    MessageBox.Show($"ADD NEW LOAD   {insertResult.Item2}");
                }
                _listManager.LoadList.Add(newLoad);
                newLoad.LoadingCalculated += DbManager.OnLoadLoadingCalculated;

                newLoad.SizeCable();
                newLoad.CalculateCableAmps();
                

                insertResult = DbManager.prjDb.InsertRecordGetId(newLoad.Cable, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList);
                if (insertResult.Item1 == false || insertResult.Item3 == 0) {
                    MessageBox.Show($"ADD NEW CABLE   {insertResult.Item2}      ");
                }
                newLoad.Cable.Id = insertResult.Item3;
                _listManager.CableList.Add(newLoad.Cable);

                BuildAssignedLoads();

                RefreshLoadTagValidation();
            }
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

            if (_listManager.LoadList.Count != 0 && LoadListLoaded == true) {
                CalculateAll();

                Tuple<bool, string> update;
                bool error = false;
                string message = "";

                foreach (var load in _listManager.LoadList) {
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
                DteqModel dteqToRecalculate = _listManager.DteqList.FirstOrDefault(d =>d.Tag == _selectedLoad.FedFrom);
                    int loadId = _selectedLoad.Id;

                if (_selectedLoad.Cable!= null) {
                    int cableId = _selectedLoad.Cable.Id;
                    DbManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, cableId);
                    _listManager.CableList.Remove(_selectedLoad.Cable);
                }

                DbManager.prjDb.DeleteRecord(GlobalConfig.LoadTable, loadId);
                

                var loadToRemove = AssignedLoads.FirstOrDefault(load => load.Id == loadId);
                AssignedLoads.Remove(loadToRemove);

                var loadToRemove2 = _listManager.LoadList.FirstOrDefault(load => load.Id == loadId);
                _listManager.LoadList.Remove(loadToRemove2);

                if (_selectedDteq != null) {
                    _selectedDteq.AssignedLoads.Remove(loadToRemove2);
                }
                if (dteqToRecalculate != null) {
                    dteqToRecalculate.CalculateLoading();
                }
                RefreshLoadTagValidation();
                BuildAssignedLoads();
                SelectedLoad = AssignedLoads[AssignedLoads.Count - 1];
            }
        }


        public void CalculateAll()
        {
            BuildAssignedLoads();
            _listManager.UnassignLoads();
            _listManager.CreateDteqDict(_listManager.DteqList);
            _listManager.CalculateDteqLoading();
        }
        #endregion


        #region Helper Methods
        public void CreateValidators()
        {
            this.DteqToAdd = new DteqToAddValidator(_listManager, this.SelectedDteq);
            this.LoadToAdd = new LoadToAddValidator(_listManager, this.SelectedDteq);
        }
        private void RefreshDteqTagValidation()
        {
            var tag = DteqToAdd.Tag;
            DteqToAdd.Tag = " ";
            DteqToAdd.Tag = tag;
        }
        private void RefreshLoadTagValidation()
        {
            var tag = LoadToAdd.Tag;
            LoadToAdd.Tag = " ";
            LoadToAdd.Tag = tag;
        }
        private void BuildFedFromList() //Used in View only
        {
            _fedFromList = new ObservableCollection<string>(_listManager.DteqList.Select(dteq => dteq.Tag).ToList());
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
            foreach (var load in _listManager.LoadList) {
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
