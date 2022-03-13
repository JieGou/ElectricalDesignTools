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

        public DataGridColumnViewControl DteqGridViewModifier { get; set; }

        public EquipmentViewModel(ListManager listManager)
        {
            //fields
            _listManager = listManager;

            //members
            DteqGridViewModifier = new DataGridColumnViewControl();

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
            CalculateAllCableAmpsCommand = new RelayCommand(CalculateAllCableAmps);

            CalculateSingleDteqCableSizeCommand = new RelayCommand(CalculateSingleDteqCableSize);
            CalculateSingleDteqCableAmpsCommand = new RelayCommand(CalculateSingleDteqCableAmps);

            CalculateSingleLoadCableSizeCommand = new RelayCommand(CalculateSingleLoadCableSize);
            CalculateSingleLoadCableAmpsCommand = new RelayCommand(CalculateSingleLoadCableAmps);

            DeleteDteqCommand = new RelayCommand(DeleteDteq);

            AddDteqCommand = new RelayCommand(AddDteq);
            AddLoadCommand = new RelayCommand(AddLoad);


            ShowAllLoadsCommand = new RelayCommand(ShowAllLoads);
            SaveLoadListCommand = new RelayCommand(SaveLoadList);
            DeleteLoadCommand = new RelayCommand(DeleteLoad);


            CalculateAllCommand = new RelayCommand(CalculateAll);
            //CalculateAllCommand = new RelayCommand(CalculateAll, startupService.IsProjectLoaded);
        }

        #endregion
        private void TestCommand()
        {
            _listManager.CreateCableList();
        }

        #region WindowSizing
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

        



        //View States
        #region Views States

        //Dteq
        public string? ToggleRowViewDteqProp { get; set; } = "Collapsed";
        public string? PerPhaseLabelDteq { get; set; } = "Hidden";

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

                    DteqToAdd.FedFromTag = _selectedDteq.Tag;
                    LoadToAdd.FedFromTag = "";
                    LoadToAdd.FedFromTag = _selectedDteq.Tag;
                    LoadToAdd.Voltage = _selectedDteq.LoadVoltage.ToString();

                    BuildDteqCableTypeList(_selectedDteq);

                    PerPhaseLabelDteq = "Hidden";
                    if (_selectedDteq.PowerCable.TypeModel.Conductors==1) {
                        PerPhaseLabelDteq = "Visible";
                    }
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
            LoadToAdd.FedFromTag = "";
            LoadToAdd.FedFromTag = _selectedLoad.FedFromTag;
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

        public ICommand CalculateSingleLoadCableSizeCommand { get; }
        public ICommand CalculateSingleLoadCableAmpsCommand { get; }
        
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
      
        #endregion

        #region Command Methods

        // Dteq
        public async void DbGetAll() {

            GlobalConfig.GettingRecords = true;

            _listManager.UnregisterAllDteqFromAllLoadEvents();

            _listManager.GetDteq();
            _listManager.GetLoads();
            _listManager.AssignLoadsToAllDteq();

            _listManager.GetCables();
            _listManager.AssignCables();
            ShowAllLoads();
            DteqToAdd = new DteqToAddValidator(_listManager, _selectedDteq);
            LoadToAdd = new LoadToAddValidator(_listManager, _selectedDteq);

            GlobalConfig.GettingRecords = false;
        }

        private void DbSaveAll()
        {
            if (_listManager.DteqList.Count != 0) {
                CalculateAll();

                Tuple<bool, string> dbSaveResult = new Tuple<bool, string>(true, "err.0");
                bool error = false;
                string message = "";

                //Dteq
                foreach (var dteq in _listManager.DteqList) {
                    var dteqTag = dteq.Tag;
                    if (dteq.Tag!= "UTILITY") {
                        dteq.PowerCable.AssignOwner(dteq);
                        dbSaveResult = DbManager.prjDb.UpsertRecord<DteqModel>(dteq, GlobalConfig.DteqTable, SaveLists.DteqSaveList);
                    }
                    if (dbSaveResult.Item1 == false) {
                        error = true;
                        message = dbSaveResult.Item2;
                    }
                }

                //Load
                foreach (var item in _listManager.LoadList) {
                    var dteqTag = item.Tag;
                    item.PowerCable.AssignOwner(item);
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
            foreach (var item in _listManager.IDteqList) {
                item.SizeCable();
            }
            foreach (var item in _listManager.LoadList) {
                item.SizeCable();
            }
        }
        private void CalculateAllCableAmps()
        {
            foreach (var item in _listManager.IDteqList) {
                item.PowerCable.CalculateAmpacityNew(item);
            }
            foreach (var item in _listManager.LoadList) {
                item.PowerCable.CalculateAmpacityNew(item);
            }
        }

        private void CalculateSingleDteqCableSize()
        {
            if (_selectedDteq.PowerCable != null) {
                _selectedDteq.PowerCable.SetCableParameters(_selectedDteq);
                _selectedDteq.PowerCable.CalculateCableQtySizeNew();
            }
            
        }
        private void CalculateSingleDteqCableAmps()
        {
            if (_selectedDteq.PowerCable != null) {
                _selectedDteq.PowerCable.CalculateAmpacityNew(_selectedDteq);
            }
        }
        private void CalculateSingleLoadCableSize()
        {
            if (_selectedLoad.PowerCable != null) {
                _selectedLoad.PowerCable.SetCableParameters(_selectedLoad);
                _selectedLoad.PowerCable.CalculateCableQtySizeNew();
            }

        }
        private void CalculateSingleLoadCableAmps()
        {
            if (_selectedLoad.PowerCable != null) {
                _selectedLoad.PowerCable.CalculateAmpacityNew(_selectedLoad);
            }
        }
        private void AddDteq()
        {
            var test = _listManager.DteqList;
            var errors = DteqToAdd._errorDict;
            var IsValid = DteqToAdd.IsValid();
            if (IsValid) {
                
                DteqModel newDteq = new DteqModel();
                newDteq.FedFrom = _listManager.DteqList.FirstOrDefault(d => d.Tag == DteqToAdd.FedFromTag);

                newDteq.Tag = DteqToAdd.Tag;
                newDteq.Category = Categories.DTEQ.ToString();
                newDteq.Type = DteqToAdd.Type;

                newDteq.Size = Double.Parse(DteqToAdd.Size);
                newDteq.Unit = DteqToAdd.Unit;
                newDteq.Description = DteqToAdd.Description;
                newDteq.FedFromTag = DteqToAdd.FedFromTag;
                newDteq.LineVoltage = Double.Parse(DteqToAdd.LineVoltage);
                newDteq.LoadVoltage = Double.Parse(DteqToAdd.LoadVoltage);

                DteqModel dteqSubscriber = _listManager.DteqList.FirstOrDefault(d => d == newDteq.FedFrom);
                if (dteqSubscriber != null) {
                    //dteqSubscriber.AssignedLoads.Add(newDteq); //newDteq is somehow already getting added to Assigned Loads
                    newDteq.LoadingCalculated += dteqSubscriber.OnDteqAssignedLoadReCalculated;
                    newDteq.LoadingCalculated += DbManager.OnDteqLoadingCalculated;
                }

                //Save to Db
                Tuple<bool, string, int> insertResult;
                insertResult = DbManager.prjDb.InsertRecordGetId(newDteq, GlobalConfig.DteqTable, SaveLists.DteqSaveList);
                newDteq.Id = insertResult.Item3;
                if (insertResult.Item1 == false || newDteq.Id == 0) {
                    MessageBox.Show($"ADD NEW LOAD   {insertResult.Item2}");
                }
                _listManager.DteqList.Add(newDteq);
                _listManager.IDteqList.Add(newDteq);
                newDteq.CalculateLoading(); //after dteq is inserted to get a new Id

                //Cable
                newDteq.SizeCable();
                newDteq.CalculateCableAmps();
                newDteq.PowerCable.Id = DbManager.prjDb.InsertRecordGetId(newDteq.PowerCable, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList).Item3;
                _listManager.CableList.Add(newDteq.PowerCable);
                RefreshDteqTagValidation();
            }
        }


        private void DeleteDteq()
        {
            if (_selectedDteq != null) {
                //children first

                //Delete Cables
                if (_selectedDteq.PowerCable != null) {
                    int cableId = _selectedDteq.PowerCable.Id;
                    DbManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, cableId);
                    _listManager.CableList.Remove(_selectedDteq.PowerCable);
                }

                List<IPowerConsumer> loadsToRetag = new List<IPowerConsumer>();
                if (_selectedDteq.AssignedLoads != null) {
                    loadsToRetag.AddRange(_selectedDteq.AssignedLoads);
                }

                //Retag Loads to "Deleted"
                DteqModel deleted = new DteqModel() { Tag = "* Deleted *"};
                for (int i = 0; i < loadsToRetag.Count; i++) {
                    var tag = loadsToRetag[i].Tag;
                    loadsToRetag[i].FedFromTag = "Deleted";
                    loadsToRetag[i].FedFromType = "Deleted"; 
                    loadsToRetag[i].FedFrom = deleted;
                }
                _selectedDteq.FedFrom.AssignedLoads.Remove(_selectedDteq);
                
                _listManager.DeleteDteq (_selectedDteq);
                RefreshDteqTagValidation();
                BuildAssignedLoads();

                if (_listManager.IDteqList.Count>0) {
                    SelectedDteq = _listManager.DteqList[_listManager.IDteqList.Count - 1];
                }
            }
        }

        public void DteqList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }
        public void LoadList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }

        public void AddLoad()
        {
            var IsValid = LoadToAdd.IsValid();
            if (IsValid) {
                LoadModel newLoad = new LoadModel();
                
                newLoad.FedFrom = _listManager.DteqList.FirstOrDefault(d => d.Tag == LoadToAdd.FedFromTag);
                newLoad.Tag = LoadToAdd.Tag;
                newLoad.Category = Categories.LOAD3P.ToString();
                newLoad.Type = LoadToAdd.Type;
                newLoad.Size = Double.Parse(LoadToAdd.Size);
                newLoad.Description = LoadToAdd.Description;
                newLoad.FedFromTag = LoadToAdd.FedFromTag;
                newLoad.Voltage = Double.Parse(LoadToAdd.Voltage);
                newLoad.Unit = LoadToAdd.Unit;
                newLoad.LoadFactor = Double.Parse(LoadToAdd.LoadFactor);

                DteqModel dteqSubscriber = _listManager.DteqList.FirstOrDefault(d => d == newLoad.FedFrom);
                if (dteqSubscriber != null) {
                    //dteqSubscriber.AssignedLoads.Add(newLoad); //newLoad is somehow already getting added to Assigned Loads
                    newLoad.LoadingCalculated += dteqSubscriber.OnDteqAssignedLoadReCalculated;
                    newLoad.LoadingCalculated += DbManager.OnLoadLoadingCalculated;
                }

                //Save to Db
                Tuple<bool, string, int> insertResult;
                insertResult = DbManager.prjDb.InsertRecordGetId(newLoad, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
                newLoad.Id = insertResult.Item3;
                if (insertResult.Item1 == false || newLoad.Id ==0) {
                    MessageBox.Show($"ADD NEW LOAD   {insertResult.Item2}");
                }
                _listManager.LoadList.Add(newLoad);
                newLoad.CalculateLoading(); //after load is inserted to get new Id

                //Cable
                newLoad.SizeCable();
                newLoad.CalculateCableAmps();
                insertResult = DbManager.prjDb.InsertRecordGetId(newLoad.PowerCable, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList);
                if (insertResult.Item1 == false || insertResult.Item3 == 0) {
                    MessageBox.Show($"ADD NEW CABLE   {insertResult.Item2}      ");
                }
                newLoad.PowerCable.Id = insertResult.Item3;
                _listManager.CableList.Add(newLoad.PowerCable);

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
                DteqModel dteqToRecalculate = _listManager.DteqList.FirstOrDefault(d =>d == _selectedLoad.FedFrom);
                    int loadId = _selectedLoad.Id;

                if (_selectedLoad.PowerCable!= null) {
                    int cableId = _selectedLoad.PowerCable.Id;
                    DbManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, cableId);
                    _listManager.CableList.Remove(_selectedLoad.PowerCable);
                }

                DbManager.prjDb.DeleteRecord(GlobalConfig.LoadTable, loadId);
                

                var loadToRemove = AssignedLoads.FirstOrDefault(load => load.Id == loadId);
                AssignedLoads.Remove(loadToRemove);

                var loadToRemove2 = _listManager.LoadList.FirstOrDefault(load => load.Id == loadId);
                _listManager.LoadList.Remove(loadToRemove2);

                //if (_selectedDteq != null) {
                //    _selectedDteq.AssignedLoads.Remove(loadToRemove2);
                //}
                if (dteqToRecalculate != null) {
                    dteqToRecalculate.AssignedLoads.Remove(loadToRemove2);
                    dteqToRecalculate.CalculateLoading();
                }
                RefreshLoadTagValidation();
                BuildAssignedLoads();
                if (AssignedLoads.Count>0) {
                    SelectedLoad = AssignedLoads[AssignedLoads.Count - 1];
                }
            }
        }


        public void CalculateAll()
        {
            BuildAssignedLoads();
            _listManager.UnregisterAllDteqFromAllLoadEvents();
            _listManager.CreateDteqDict();
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


        //ComboBox Lists
        public ObservableCollection<string> DteqTypes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> VoltageTypes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<CableTypeModel> DteqCableTypes { get; set; } = new ObservableCollection<CableTypeModel>();
        public ObservableCollection<CableTypeModel> LoadCableTypes { get; set; } = new ObservableCollection<CableTypeModel>();
        public ObservableCollection<double> CableSpacing { get; set; } = new ObservableCollection<double>();
        public ObservableCollection<string> CableInstallationTypes { get; set; } = new ObservableCollection<string>();
        public void CreateComboBoxLists()
        {
            foreach (var item in Enum.GetNames(typeof(EDTLibrary.DteqTypes))) {
                DteqTypes.Add(item.ToString());
            }
            
            foreach (var item in TypeManager.VoltageTypes) {
                VoltageTypes.Add(item.Voltage.ToString());
            }
            CableSpacing.Clear();
            CableSpacing.Add(100);
            CableSpacing.Add(0);

            CableInstallationTypes.Add("LadderTray");
            CableInstallationTypes.Add("DirectBuried");
            CableInstallationTypes.Add("RacewayConduit");
        }

        private void BuildDteqCableTypeList(IDteq dteq)
        {
            if (dteq == null || dteq.PowerCable == null)
                return;

            string selectedDteqCableType = _selectedDteq.PowerCable.Type;
            string selectedLoadCableType = "None Selected";
            if (_selectedLoad != null && _selectedLoad.PowerCable != null) {
                selectedLoadCableType = _selectedLoad.PowerCable.Type;
            }

            DteqCableTypes.Clear();
            foreach (var cableType in TypeManager.CableTypes) {
                var voltageClass = LibraryManager.GetCableVoltageClass(dteq.LineVoltage);
                if (voltageClass == cableType.VoltageClass) {
                    DteqCableTypes.Add(cableType);
                }
            }
            dteq.PowerCable.Type = selectedDteqCableType;

            if (_selectedLoad != null && _selectedLoad.PowerCable != null) {
                _selectedLoad.PowerCable.Type = selectedLoadCableType;
            }
        }

        private async Task BuildLoadCableTypeList(IPowerConsumer load)
        {
            if (load == null)
                return;

            string selectedDteqCableType = "None Selected"; 
            if (_selectedDteq != null && _selectedDteq.PowerCable != null) {
                selectedDteqCableType = _selectedDteq.PowerCable.Type;
            }
            string selectedLoadCableType = _selectedLoad.PowerCable.Type;

            LoadCableTypes.Clear();
                foreach (var cableType in TypeManager.CableTypes) {
                    var voltageClass = LibraryManager.GetCableVoltageClass(load.Voltage);
                    if (voltageClass == cableType.VoltageClass) {
                    LoadCableTypes.Add(cableType);
                    }
                }

            if (_selectedDteq != null && _selectedDteq.PowerCable != null) {
                _selectedDteq.PowerCable.Type = selectedDteqCableType;
            }
            load.PowerCable.Type = selectedLoadCableType;
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
