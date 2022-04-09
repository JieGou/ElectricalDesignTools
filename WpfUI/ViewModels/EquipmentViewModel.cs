using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
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
using WpfUI.Helpers;
using WpfUI.Stores;
using WpfUI.ViewModifiers;
using PropertyChanged;

namespace WpfUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class EquipmentViewModel : ViewModelBase, INotifyDataErrorInfo
{

    #region Constructor
    private DteqFactory _dteqFactory;
    private LoadFactory _loadFactory;

    private ListManager _listManager;
    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }

    public EquipmentViewModel(ListManager listManager)
    {
        //fields
        _listManager = listManager;
        _dteqFactory = new DteqFactory(listManager);
        _loadFactory = new LoadFactory(listManager);
        //members
        DteqGridViewModifier = new DataGridColumnViewControl();

        DteqToAddValidator = new DteqToAddValidator(_listManager);
        LoadToAddValidator = new LoadToAddValidator(_listManager);

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

        //DeleteDteqCommand = new AsyncRelayCommand(DeleteDteqAsync, (ex) => MessageBox.Show(ex.Message));
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


    //View States
    #region Views States

    //Dteq
    public string? ToggleRowViewDteqProp { get; set; } = "Collapsed";
    public string? PerPhaseLabelDteq { get; set; } = "Hidden";
    public DataGridColumnViewControl DteqGridViewModifier { get; set; }


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

                GlobalConfig.SelectingNew = true;
                DteqToAddValidator.FedFromTag = _selectedDteq.Tag;
                LoadToAddValidator.FedFromTag = "";
                LoadToAddValidator.FedFromTag = _selectedDteq.Tag;
                LoadToAddValidator.Voltage = _selectedDteq.LoadVoltage.ToString();
                GlobalConfig.SelectingNew = false;

                //BuildDteqCableTypeList(_selectedDteq);

                PerPhaseLabelDteq = "Hidden";
                if (_selectedDteq.PowerCable.TypeModel != null) {
                    if (_selectedDteq.PowerCable.TypeModel.Conductors == 1) {
                        PerPhaseLabelDteq = "Visible";
                    }
                }
                
            }
        }
    }
    public DteqToAddValidator DteqToAddValidator { get; set; }

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
                GlobalConfig.SelectingNew = true;
                    CopySelectedLoad();
                GlobalConfig.SelectingNew = false;

            }
        }
    }
    private async Task CopySelectedLoad()
    {
        try {
            await CopySelectedLoadAsyn();
        }
        catch (Exception ex) {
            ErrorHelper.EdtErrorMessage(ex);
        }

        async Task CopySelectedLoadAsyn()
        {
            LoadToAddValidator.FedFromTag = "";
            LoadToAddValidator.FedFromTag = _selectedLoad.FedFromTag;
            LoadToAddValidator.Type = "";
            LoadToAddValidator.Type = _selectedLoad.Type;
            LoadToAddValidator.Size = "";
            LoadToAddValidator.Size = _selectedLoad.Size.ToString();
            LoadToAddValidator.Unit = "";
            LoadToAddValidator.Unit = _selectedLoad.Unit;
            LoadToAddValidator.Voltage = "";
            LoadToAddValidator.Voltage = _selectedLoad.Voltage.ToString();
        }

    }
    public LoadToAddValidator LoadToAddValidator { get; set; }


    // LISTS
    public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> { };
    public bool LoadListLoaded { get; set; }


    //Cables



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
        _listManager.GetProjectTablesAndAssigments();
        AssignedLoads.Clear();

    }

    private void DbSaveAll()
    {
        //Task.Run(() => CalculateAll());

        try {
            //Dteq
            foreach (var dteq in _listManager.DteqList) {
                if (dteq.Tag != GlobalConfig.Utility) {
                    dteq.PowerCable.AssignOwner(dteq);
                    DaManager.UpsertDteq(dteq);
                }
            }

            //Load
            foreach (var load in _listManager.LoadList) {
                load.PowerCable.AssignOwner(load);
                DaManager.UpsertLoad((LoadModel)load);
            }

            //Cables
            _listManager.CreateCableList();
            foreach (var cable in _listManager.CableList) {
                DaManager.UpsertCable(cable);
            }
        }

        catch (Exception ex) {
            ErrorHelper.SqlErrorMessage(ex);
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

    public void AddDteq(object dteqToAddObject) //typeOf DteqToAddValidator
    {

        DteqToAddValidator dteqToAddValidator = (DteqToAddValidator)dteqToAddObject;
        try {
            var IsValid = dteqToAddValidator.IsValid(); //to help debug
            var errors = dteqToAddValidator._errorDict; //to help debug
            if (IsValid) {

                IDteq newDteq = _dteqFactory.CreateDteq(dteqToAddValidator);

                IDteq dteqSubscriber = _listManager.DteqList.FirstOrDefault(d => d == newDteq.FedFrom);
                if (dteqSubscriber != null) {
                    //dteqSubscriber.AssignedLoads.Add(newDteq); //newDteq is somehow already getting added to Assigned Loads
                    newDteq.LoadingCalculated += dteqSubscriber.OnAssignedLoadReCalculated;
                    newDteq.LoadingCalculated += DaManager.OnDteqLoadingCalculated;
                }

                //Save to Db
                newDteq.Id = DaManager.SaveDteqGetId(newDteq);

                _listManager.AddDteq(newDteq);
                newDteq.CalculateLoading(); //after dteq is inserted to get a new Id

                //Cable
                newDteq.CreateCable();
                newDteq.SizeCable();
                newDteq.CalculateCableAmps();
                newDteq.PowerCable.Id = DaManager.prjDb.InsertRecordGetId(newDteq.PowerCable, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList);
                _listManager.CableList.Add(newDteq.PowerCable); // newCable is already getting added
                RefreshDteqTagValidation();
            }
        }
        catch (Exception ex) {
            ErrorHelper.SqlErrorMessage(ex);
        }
    }

    public void DeleteDteq(object dteqObject)
    {
        try {

            IDteq dteqToDelete = DteqFactory.Recast(dteqObject);
            if (dteqToDelete != null) {
                //children first

                _listManager.UnregisterDteqFromLoadEvents(dteqToDelete);
                DeletePowerCable(dteqToDelete); //await 
                RetagLoadsOfDeleted(dteqToDelete); //await

                if (dteqToDelete.FedFrom!=null) {
                    dteqToDelete.FedFrom.AssignedLoads.Remove(dteqToDelete);
                }
                DaManager.DeleteDteq(dteqToDelete);
                _listManager.DeleteDteq(dteqToDelete); //await
                RefreshDteqTagValidation();

                if (_listManager.IDteqList.Count > 0) {
                    SelectedDteq = _listManager.IDteqList[_listManager.IDteqList.Count - 1];
                }
            }
        }
        catch (Exception ex){
            MessageBox.Show(ex.Message);
        }
        return;
    }

    private void DeletePowerCable(IPowerConsumer powerCableUser)
    {
        
        if (powerCableUser.PowerCable != null) {
            int cableId = powerCableUser.PowerCable.Id;
            DaManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, cableId); //await
            _listManager.CableList.Remove(powerCableUser.PowerCable);
        }
        return;
    }
    private async Task DeletePowerCableAsync(IPowerConsumer powerCableUser)
    {

        if (powerCableUser.PowerCable != null) {
            int cableId = powerCableUser.PowerCable.Id;
            DaManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, cableId); //await
            _listManager.CableList.Remove(powerCableUser.PowerCable);
        }
        return;
    }
    private void RetagLoadsOfDeleted(IDteq selectedDteq)
    {

        //Loads
        List<IPowerConsumer> assignedLoads = new List<IPowerConsumer>();
        if (selectedDteq.AssignedLoads != null) {
            assignedLoads.AddRange(selectedDteq.AssignedLoads);
        }

        //Retag Loads to "Deleted"
        IDteq deleted = GlobalConfig.DteqDeleted;
        for (int i = 0; i < assignedLoads.Count; i++) {
            var tag = assignedLoads[i].Tag;
            var load = assignedLoads[i];
            load.FedFromTag = GlobalConfig.Deleted;
            load.FedFromType = GlobalConfig.Deleted;
            load.FedFrom = deleted;
        }
        return ;
    }
    public void DteqList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }
    public void LoadList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }

    public void AddLoad(object loadToAddObject)
    {

        LoadToAddValidator loadToAddValidator = (LoadToAddValidator)loadToAddObject;
        var IsValid = loadToAddValidator.IsValid();
        try {
            var errors = loadToAddValidator._errorDict;
            if (IsValid) {

                LoadModel newLoad = _loadFactory.CreateLoad(loadToAddValidator);

                IDteq dteqSubscriber = _listManager.DteqList.FirstOrDefault(d => d == newLoad.FedFrom);
                if (dteqSubscriber != null) {
                    //dteqSubscriber.AssignedLoads.Add(newLoad); //newLoad is somehow already getting added to Assigned Loads
                    newLoad.LoadingCalculated += dteqSubscriber.OnAssignedLoadReCalculated;
                    newLoad.LoadingCalculated += DaManager.OnLoadLoadingCalculated;
                }

                //Save to Db

                newLoad.Id = DaManager.SaveLoadGetId(newLoad);
                _listManager.LoadList.Add(newLoad);
                newLoad.CalculateLoading(); //after load is inserted to get new Id

                //Cable
                newLoad.SizeCable();
                newLoad.CalculateCableAmps();
                newLoad.PowerCable.Id = DaManager.prjDb.InsertRecordGetId(newLoad.PowerCable, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList);
                _listManager.CableList.Add(newLoad.PowerCable);

                BuildAssignedLoadsAsync(); //await

                RefreshLoadTagValidation();
            }
        }
        catch (Exception ex) {
            ErrorHelper.SqlErrorMessage(ex);
        }
    }

    public void DeleteLoad(object selectedLoadObject)
    {
        DeleteLoadAsync(selectedLoadObject);
    }
    public async Task DeleteLoadAsync(object selectedLoadObject)
    {
        LoadModel selectedLoad = (LoadModel)selectedLoadObject;
        if (selectedLoad != null) {
            IDteq dteqToRecalculate = _listManager.DteqList.FirstOrDefault(d => d == selectedLoad.FedFrom);
            int loadId = selectedLoad.Id;

            try {
                selectedLoad.LoadingCalculated -= DaManager.OnLoadLoadingCalculated;
                await DeletePowerCableAsync(selectedLoad); //await
                await DaManager.prjDb.DeleteRecordAsync(GlobalConfig.LoadTable, loadId); //await

                var loadToRemove = AssignedLoads.FirstOrDefault(load => load.Id == loadId);
                AssignedLoads.Remove(loadToRemove);

                var loadToRemove2 = _listManager.LoadList.FirstOrDefault(load => load.Id == loadId);
                _listManager.LoadList.Remove(loadToRemove2);

                if (dteqToRecalculate != null) {
                    selectedLoad.LoadingCalculated -= dteqToRecalculate.OnAssignedLoadReCalculated;
                    dteqToRecalculate.AssignedLoads.Remove(loadToRemove2);
                    dteqToRecalculate.CalculateLoading();
                }
                RefreshLoadTagValidation();
                await BuildAssignedLoadsAsync();
                if (AssignedLoads.Count > 0) {
                    SelectedLoad = AssignedLoads[AssignedLoads.Count - 1];
                }
            }
            catch (Exception ex) {

                if (ex.Message.ToLower().Contains("sql")) {
                    ErrorHelper.SqlErrorMessage(ex);
                }
                else {
                    ErrorHelper.EdtErrorMessage(ex);
                }
            }
        }
    }

    // Loads
    public async void ShowAllLoads()
    {
        LoadListLoaded = true;
        //LoadList = new ObservableCollection<LoadModel>(DbManager.prjDb.GetRecords<LoadModel>(GlobalConfig.LoadListTable));
        await BuildAssignedLoadsAsync();
    }
    private void SaveLoadList()
    {

        if (_listManager.LoadList.Count != 0 && LoadListLoaded == true) {
      
            try {
                foreach (var load in _listManager.LoadList) {
                    DaManager.prjDb.UpsertRecord<LoadModel>((LoadModel)load, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
                }
            }
            catch (Exception ex) {
                ErrorHelper.SqlErrorMessage(ex);
            }

        }
    }
    


    public void CalculateAll()
    {
        try {
            BuildAssignedLoadsAsync();
            _listManager.UnregisterAllDteqFromAllLoadEvents();
            _listManager.CreateDteqDict();
            _listManager.CalculateDteqLoading();
        }
        catch (Exception ex) {

            if (ex.Message.ToLower().Contains("sql")) {
                ErrorHelper.SqlErrorMessage(ex);
            }
            else {
                ErrorHelper.EdtErrorMessage(ex);
            }
        }

    }
    #endregion


    #region Helper Methods
    public void CreateValidators()
    {
        this.DteqToAddValidator = new DteqToAddValidator(_listManager);
        this.LoadToAddValidator = new LoadToAddValidator(_listManager);
    }
    private void RefreshDteqTagValidation()
    {
        var tag = DteqToAddValidator.Tag;
        DteqToAddValidator.Tag = " ";
        DteqToAddValidator.Tag = tag;
    }
    private void RefreshLoadTagValidation()
    {
        var tag = LoadToAddValidator.Tag;
        LoadToAddValidator.Tag = " ";
        LoadToAddValidator.Tag = tag;
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
    private async Task BuildAssignedLoadsAsync()
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

    private async Task BuildDteqCableTypeList(IDteq dteq)
    {
        await BuildDteqCableTypeListAsync(dteq);
    }
    private async Task BuildDteqCableTypeListAsync(IDteq dteq)
    {
        if (dteq == null || dteq.PowerCable == null)
            return;

        string selectedDteqCableType = _selectedDteq.PowerCable.Type;
        string selectedLoadCableType = "None Selected";
        if (_selectedLoad != null && _selectedLoad.PowerCable != null && _selectedLoad.PowerCable.Type != null) {
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
        return;
    }

    private async Task BuildLoadCableTypeList(IPowerConsumer load)
    {
        try {
            await BuildLoadCableTypeListAsync(load);
        }
        catch (NullReferenceException ex) {
            ErrorHelper.EdtErrorMessage(ex);
        }
    }
    private async Task BuildLoadCableTypeListAsync(IPowerConsumer load)
    {
            if (load == null)
                return;

            string selectedDteqCableType = "None Selected";
            if (_selectedDteq != null && _selectedDteq.PowerCable != null) {
                selectedDteqCableType = _selectedDteq.PowerCable.Type;
            }
        try {
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
        catch (NullReferenceException ex) {
            ex.Data.Add("UserMessage", $"The selected load doesn't have a power cable");
            if (load.PowerCable==null) {
                load.CreateCable();
            }
            throw;
        }
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

    #endregion

}

