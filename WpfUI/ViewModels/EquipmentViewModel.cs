using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
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
            CalculateAllCableAmpsCommand = new RelayCommand(CalculateCableAmps);

            CalculateSingleDteqCableSizeCommand = new RelayCommand(CalculateSingleDteqCableSize);
            CalculateSingleDteqCableAmpsCommand = new RelayCommand(CalculateSingleDteqCableAmps);

            

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


        private System.Windows.GridLength _dteqGridRight = new System.Windows.GridLength(AppSettings.Default.DteqGridRight, GridUnitType.Pixel);
        public System.Windows.GridLength DteqGridRight
        {
            get { return _dteqGridRight; }
            set
            {
                _dteqGridRight = value;
                AppSettings.Default.DteqGridRight = _dteqGridRight.Value;
                AppSettings.Default.Save();
            }
        }

        //DTEQ GridSplitter Position
        private System.Windows.GridLength _dteqGridBottom = new System.Windows.GridLength(AppSettings.Default.DteqGridBottom, GridUnitType.Pixel);
        public System.Windows.GridLength DteqGridBottom
        {
            get { return _dteqGridBottom; }
            set
            {
                double oldBottom = _dteqGridBottom.Value;
                _dteqGridBottom = value;
                AppSettings.Default.DteqGridBottom = _dteqGridBottom.Value;
                AppSettings.Default.Save();
                //DteqGridHeight = 275;
                DteqGridHeight += (_dteqGridBottom.Value - oldBottom);
                AppSettings.Default.DteqGridHeight = DteqGridHeight;
                AppSettings.Default.Save();

                LoadGridHeight -= (_dteqGridBottom.Value - oldBottom);
                AppSettings.Default.LoadGridHeight = LoadGridHeight;
                AppSettings.Default.Save();
            }
        }
        public double DteqGridHeight { get; set; }


        private System.Windows.GridLength _loadGridRight = new System.Windows.GridLength(AppSettings.Default.LoadGridRight, GridUnitType.Pixel);
        public System.Windows.GridLength LoadGridRight
        {
            get { return _loadGridRight; }
            set
            {
                _loadGridRight = value;
                AppSettings.Default.LoadGridRight = _loadGridRight.Value;
                AppSettings.Default.Save();
            }
        }

        //Load GridSplitter Position
        private System.Windows.GridLength _loadGridTop = new System.Windows.GridLength(AppSettings.Default.LoadGridTop, GridUnitType.Pixel);
        public System.Windows.GridLength LoadGridTop
        {
            get { return _loadGridTop; }
            set
            {
                double oldTop = _loadGridTop.Value;
                _loadGridTop = value;
                AppSettings.Default.LoadGridTop = _loadGridTop.Value;
                AppSettings.Default.Save();

                //LoadGridHeight = 350;
                LoadGridHeight -= (_loadGridTop.Value - oldTop);
                AppSettings.Default.LoadGridHeight = LoadGridHeight;
                AppSettings.Default.Save();
            }
        }

        private System.Windows.GridLength _loadGridBottom = new System.Windows.GridLength(AppSettings.Default.LoadGridBottom, GridUnitType.Pixel);
        public System.Windows.GridLength LoadGridBottom
        {
            get { return _loadGridBottom; }
            set
            {
                _loadGridBottom = value;
                AppSettings.Default.LoadGridBottom = _loadGridBottom.Value;
                AppSettings.Default.Save();

               
            }
        }
        public double LoadGridHeight { get; set; }
        #endregion


        #region Properties
        public NavigationBarViewModel NavigationBarViewModel { get; }

        //ComboBox Lists
        private ObservableCollection<string> _fedFromList; //used only on View
        public ObservableCollection<string> FedFromList
        {
            get
            {
                _fedFromList = new ObservableCollection<string>(_listManager.IDteqList.Select(dteq => dteq.Tag).ToList());
                _fedFromList.Insert(0, "UTILITY");
                return _fedFromList;
            }
            set { }
        } //Used only on View
        public ObservableCollection<string> DteqTypes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> VoltageTypes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<CableTypeModel> CableTypes { get; set; } = new ObservableCollection<CableTypeModel>();



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
        private IDteq _selectedDteq;
        public IDteq SelectedDteq
        {
            get { return _selectedDteq; }
            set
            {
                //used for fedfrom Validation
                DictionaryStore.CreateDteqDict(_listManager.IDteqList);
                _selectedDteq = value;
                LoadListLoaded = false;

                if (_selectedDteq != null) {
                    AssignedLoads = new ObservableCollection<IPowerConsumer>(_selectedDteq.AssignedLoads);

                    LoadToAdd.FedFrom = "";
                    LoadToAdd.FedFrom = _selectedDteq.Tag;
                    LoadToAdd.Voltage = _selectedDteq.LoadVoltage.ToString();

                    BuildDteqCableTypeList(_selectedDteq);
                }
            }
        }
        public DteqToAddValidator DteqToAdd { get; set; }

        // LOADS

        private IPowerConsumer _selectedLoad;
        public IPowerConsumer SelectedLoad
        {
            get { return _selectedLoad; }
            set
            {
                _selectedLoad = value;
                if (_selectedLoad != null) {

                    BuildLoadCableTypeList(_selectedLoad);
                    CopySelectedLoad();

                }
            }
        }
        private async Task CopySelectedLoad()
        {
            LoadToAdd.FedFrom = "";
            LoadToAdd.FedFrom = _selectedLoad.FedFrom;
            LoadToAdd.Type = "";
            LoadToAdd.Type = _selectedLoad.Type;
            LoadToAdd.Size = "";
            LoadToAdd.Size = _selectedLoad.Size.ToString();
            LoadToAdd.Unit = "";
            LoadToAdd.Unit = _selectedLoad.Unit;
            LoadToAdd.Voltage = "";
            LoadToAdd.Voltage = _selectedLoad.Voltage.ToString();

        }
        public LoadToAddValidator LoadToAdd { get; set; }


        // LISTS
        public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> { };
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
        public ICommand CalculateAllCableAmpsCommand { get; }
        public ICommand CalculateSingleDteqCableSizeCommand { get; }
        public ICommand CalculateSingleDteqCableAmpsCommand { get; }

        
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

            //_listManager.SetDteq();
            _listManager.DteqList = _listManager.GetDteq();
            _listManager.LoadList = _listManager.GetLoads();
            ShowAllLoads();
            CalculateAll();
            _listManager.AssignLoadsToAllDteq();

            _listManager.CableList = _listManager.GetCables();
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
        private void CalculateSingleDteqCableSize()
        {
            _selectedDteq.Cable.SetCableParameters(_selectedDteq);
            _selectedDteq.Cable.CalculateCableQtySizeNew();
        }
        private void CalculateSingleDteqCableAmps()
        {
            _selectedDteq.Cable.CalculateAmpacityNew();
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
                _listManager.IDteqList.Add(newDteq);

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

                //Delete Cables
                if (_selectedDteq.Cable != null) {
                    int cableId = _selectedDteq.Cable.Id;
                    DbManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, cableId);
                    _listManager.CableList.Remove(_selectedDteq.Cable);
                }
                //Retag Cables
                if (_selectedDteq.AssignedLoads != null) {
                    foreach (var load in _selectedDteq.AssignedLoads) {
                        load.Cable.Destination = "DTEQ Deleted";
                        load.Cable.CreateTag();
                    }
                }
                
                _listManager.DeteteDteq(_selectedDteq);
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
            _listManager.UnassignLoadsAllDteq();
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
                       
        }

        private void BuildDteqCableTypeList(IDteq dteq)
        {
            if (dteq == null || dteq.Cable == null)
                return;

            string selectedDteqCableType = _selectedDteq.Cable.Type;
            string selectedLoadCableType = "None Selected";
            if (_selectedLoad != null && _selectedLoad.Cable != null) {
                selectedLoadCableType = _selectedLoad.Cable.Type;
            }

            CableTypes.Clear();
            foreach (var cableType in TypeManager.CableTypes) {
                var voltageClass = LibraryManager.GetCableVoltageClass(dteq.LineVoltage);
                if (voltageClass == cableType.VoltageClass) {
                    CableTypes.Add(cableType);
                }
            }
            dteq.Cable.Type = selectedDteqCableType;

            if (_selectedLoad != null && _selectedLoad.Cable != null) {
                _selectedLoad.Cable.Type = selectedLoadCableType;
            }
            
        }

        private async Task BuildLoadCableTypeList(IPowerConsumer load)
        {
            if (load == null)
                return;

            string selectedDteqCableType = "None Selected"; 
            if (_selectedDteq != null && _selectedDteq.Cable != null) {
                selectedDteqCableType = _selectedDteq.Cable.Type;
            }
            string selectedLoadCableType = _selectedLoad.Cable.Type;

            CableTypes.Clear();
                foreach (var cableType in TypeManager.CableTypes) {
                    var voltageClass = LibraryManager.GetCableVoltageClass(load.Voltage);
                    if (voltageClass == cableType.VoltageClass) {
                        CableTypes.Add(cableType);
                    }
                }

            if (_selectedDteq != null && _selectedDteq.Cable != null) {
                _selectedDteq.Cable.Type = selectedDteqCableType;
            }
            load.Cable.Type = selectedLoadCableType;
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
