using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfUI.Commands;
using WpfUI.Helpers;
using WpfUI.Stores;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Debug;
using WpfUI.ViewModifiers;
using WpfUI.Windows;
using IComponent = EDTLibrary.Models.Components.IComponent;

namespace WpfUI.ViewModels.Electrical;

[AddINotifyPropertyChangedInterface]
public class MjeqViewModel : ViewModelBase, INotifyDataErrorInfo
{

    #region Constructor
    private DteqFactory _dteqFactory;
    private ListManager _listManager;

    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }

    public bool AreaColumnVisible
    {
        get
        {
            if (EdtSettings.AreaColumnVisible == "True") {
                return true;
            }
            return false;
        }
    }

    public SolidColorBrush SingleLineViewBackground { get; set; } = new SolidColorBrush(Colors.White);

    public DebugViewModel DebugViewModel { get; set; }

    //CONSTRUCTOR
    public MjeqViewModel(ListManager listManager)
    {
        DebugViewModel = new DebugViewModel();

        //fields
        _listManager = listManager;
        _dteqFactory = new DteqFactory(listManager);


        //members
        DteqGridViewModifier = new DataGridColumnViewToggle();
        LoadGridViewModifier = new DataGridColumnViewToggle();

        DteqToAddValidator = new DteqToAddValidator(_listManager);
        LoadToAddValidator = new LoadToAddValidator(_listManager);

        // Create commands
        ToggleRowDetailViewCommand = new RelayCommand(ToggleDatagridRowdetailView);

        TogglePowerViewDteqCommand = new RelayCommand(DteqGridViewModifier.TogglePower);
        ToggleOcpdViewDteqCommand = new RelayCommand(DteqGridViewModifier.ToggleOcpd);
        ToggleCableViewDteqCommand = new RelayCommand(DteqGridViewModifier.ToggleCable);

        TogglePowerViewLoadCommand = new RelayCommand(LoadGridViewModifier.TogglePower);
        ToggleOcpdViewLoadCommand = new RelayCommand(LoadGridViewModifier.ToggleOcpd);
        ToggleCableViewLoadCommand = new RelayCommand(LoadGridViewModifier.ToggleCable);
        ToggleCompViewLoadCommand = new RelayCommand(LoadGridViewModifier.ToggleComp);


        GetAllCommand = new RelayCommand(DbGetAll);
        SaveAllCommand = new RelayCommand(DbSaveAll);
        SizeCablesCommand = new RelayCommand(SizeAllCables);
        CalculateAllCableAmpsCommand = new RelayCommand(CalculateAllCableAmps);

        CalculateSingleEqCableSizeCommand = new RelayCommand(CalculateSingleEqCableSize);
        CalculateSingleEqCableAmpsCommand = new RelayCommand(CalculateSingleEqCableAmps);

        //DeleteDteqCommand = new AsyncRelayCommand(DeleteDteqAsync, (ex) => MessageBox.Show(ex.Message));
        DeleteDteqCommand = new RelayCommand(DeleteDteq);

        AddDteqCommand = new RelayCommand(AddDteq);
        AddLoadCommand = new RelayCommand(AddLoad);


        GetLoadListCommand = new RelayCommand(GetLoadList);
        CalculateLoadCommand = new RelayCommand(CalculateLoad);
        SaveLoadListCommand = new RelayCommand(SaveLoadList);
        DeleteLoadCommand = new RelayCommand(DeleteLoad);


        CalculateAllCommand = new RelayCommand(CalculateAll);
        //CalculateAllCommand = new RelayCommand(CalculateAll, startupService.IsProjectLoaded);

        ToggleLoadDisconnectCommand = new RelayCommand(ToggleLoadDisconnect);
        ToggleLoadDriveCommand = new RelayCommand(ToggleLoadDrive);

        ComponentMoveUpCommand = new RelayCommand(ComponentMoveUp);
        ComponentMoveDownCommand = new RelayCommand(ComponentMoveDown);

        DeleteComponentCommand = new RelayCommand(DeleteComponent);


        //Window Commands
        CloseWindowCommand = new RelayCommand(CloseSelectionWindow);
        SetFedFromCommand = new RelayCommand(SetFedFrom);


    }



    public UserControl SelectedElectricalVieModel { get; set; }
    #endregion

    #region Public Commands

    // Equipment Commands
    public ICommand NavigateMjeqCommand { get; }


    //view commands
    public ICommand ToggleRowDetailViewCommand { get; }

    public ICommand TogglePowerViewDteqCommand { get; }
    public ICommand ToggleOcpdViewDteqCommand { get; }
    public ICommand ToggleCableViewDteqCommand { get; }

    public ICommand TogglePowerViewLoadCommand { get; }
    public ICommand ToggleOcpdViewLoadCommand { get; }
    public ICommand ToggleCableViewLoadCommand { get; }
    public ICommand ToggleCompViewLoadCommand { get; }


    public ICommand GetAllCommand { get; }
    public ICommand SaveAllCommand { get; }
    public ICommand DeleteDteqCommand { get; }
    public ICommand SizeCablesCommand { get; }
    public ICommand CalculateAllCableAmpsCommand { get; }
    public ICommand CalculateSingleEqCableSizeCommand { get; }
    public ICommand CalculateSingleEqCableAmpsCommand { get; }

    public ICommand AddDteqCommand { get; }
    public ICommand AddLoadCommand { get; }


    // Load Commands
    public ICommand GetLoadListCommand { get; }
    public ICommand CalculateLoadCommand { get; }
    public ICommand DeleteLoadCommand { get; }

    public ICommand CalculateAllCommand { get; }


    public ICommand SaveLoadListCommand { get; }



    public ICommand ToggleLoadDisconnectCommand { get; }
    public ICommand ToggleLoadDriveCommand { get; }


    public ICommand ComponentMoveUpCommand { get; }
    public ICommand ComponentMoveDownCommand { get; }
    public ICommand DeleteComponentCommand { get; }


    public Window SelectionWindow { get; set; }
    public ICommand CloseWindowCommand { get; }
    public ICommand SetFedFromCommand { get; }


    #endregion

    #region Window Commands
    public void CloseSelectionWindow()
    {
        SelectionWindow.Close();
        SelectionWindow = null;
    }
    public void SetFedFrom()
    {
        IPowerConsumer load;
        foreach (var item in SelectedLoads) {
            load = (IPowerConsumer)item;
            //dteq.Tag = "New Tag";
            load.FedFrom = ListManager.IDteqList.FirstOrDefault(d => d.Tag == LoadToAddValidator.FedFromTag);
        }
        if (SelectionWindow != null) {
            CloseSelectionWindow();
        }
    }

    #endregion


    #region Views States

    //Dteq
    public string? ToggleRowDetailViewProp { get; set; } = "Collapsed";
    public string? PerPhaseLabelDteq { get; set; } = "Hidden";
    public DataGridColumnViewToggle DteqGridViewModifier { get; set; }
    public DataGridColumnViewToggle LoadGridViewModifier { get; set; }

    #endregion


    #region Properties
    public bool DteqFilter { get; set; }
    private ObservableCollection<IDteq> _dteqList = new ObservableCollection<IDteq>();

    public ObservableCollection<IDteq> DteqList
    {
        get
        {
            if (DteqFilter == true) {
                return _dteqList;
            }

            return ListManager.IDteqList;
        }

        set
        {
            _dteqList = value;
        }
    }

    private ListCollectionView _dteqCollectionView;
    public ListCollectionView DteqCollectionView
    {
        get
        {
            if (_dteqCollectionView == null) {
                //View = CollectionViewSource.GetDefaultView(_listManager.CableList);
                _dteqCollectionView = new ListCollectionView(DteqList);
            }
            return _dteqCollectionView;
        }
        set
        {
            _dteqCollectionView = DteqCollectionView;
        }
    }



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
                CopySelectedDteq();
                GlobalConfig.SelectingNew = false;

                PerPhaseLabelDteq = "Hidden";
                if (_selectedDteq.PowerCable.TypeModel != null) {
                    if (_selectedDteq.PowerCable.TypeModel.ConductorQty == 1) {
                        PerPhaseLabelDteq = "Visible";
                    }
                }

                //TODO move to AssingedLoads Summary
                DteqMotorLoads = new Tuple<int, double, double>(_selectedDteq.AssignedLoads.Count(al => al.Type == LoadTypes.MOTOR.ToString()),
                    _selectedDteq.AssignedLoads.Where(al => al.Type == LoadTypes.MOTOR.ToString()).Sum(al => al.DemandKva),
                    _selectedDteq.AssignedLoads.Where(al => al.Type == LoadTypes.MOTOR.ToString()).Sum(al => al.DemandKw));
                DteqHeaterLoads = new Tuple<int, double, double>(_selectedDteq.AssignedLoads.Count(al => al.Type == LoadTypes.HEATER.ToString()),
                    _selectedDteq.AssignedLoads.Where(al => al.Type == LoadTypes.HEATER.ToString()).Sum(al => al.DemandKva),
                    _selectedDteq.AssignedLoads.Where(al => al.Type == LoadTypes.HEATER.ToString()).Sum(al => al.DemandKw));

                DteqPanelLoads = _selectedDteq.AssignedLoads.Count(al => al.Type == LoadTypes.PANEL.ToString());
                DteqWeldingLoads = _selectedDteq.AssignedLoads.Count(al => al.Type == LoadTypes.WELDING.ToString());
                DteqOtherLoads = _selectedDteq.AssignedLoads.Count(al => al.Type == LoadTypes.OTHER.ToString());
            }
        }
    }

    private int _selectedDteqTab;
    public int SelectedDteqTab
    {
        get { return _selectedDteqTab; }
        set { _selectedDteqTab = value; }
    }
    public Tuple<int, double, double> DteqMotorLoads { get; set; }
    public Tuple<int, double, double> DteqHeaterLoads { get; set; }
    public int DteqPanelLoads { get; set; }
    public int DteqWeldingLoads { get; set; }
    public int DteqOtherLoads { get; set; }

    private async Task CopySelectedDteq()
    {
        try {
            await CopySelectedDteqAsync();
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
        async Task CopySelectedDteqAsync()
        {
            try {
                DteqToAddValidator.FedFromTag = "";
                DteqToAddValidator.FedFromTag = _selectedDteq.FedFromTag;
                DteqToAddValidator.AreaTag = "";
                DteqToAddValidator.AreaTag = _selectedDteq.Area.Tag;
                DteqToAddValidator.Type = "";
                DteqToAddValidator.Type = _selectedDteq.Type;
                DteqToAddValidator.Size = "";
                DteqToAddValidator.Size = _selectedDteq.Size.ToString();
                DteqToAddValidator.Unit = "";
                DteqToAddValidator.Unit = _selectedDteq.Unit;
                DteqToAddValidator.LineVoltage = "";
                DteqToAddValidator.LineVoltage = _selectedDteq.LineVoltage.ToString();
                DteqToAddValidator.LoadVoltage = "";
                DteqToAddValidator.LoadVoltage = _selectedDteq.LoadVoltage.ToString();

                LoadToAddValidator.FedFromTag = "";
                LoadToAddValidator.FedFromTag = _selectedDteq.Tag;
                LoadToAddValidator.AreaTag = "";
                LoadToAddValidator.AreaTag = _selectedDteq.Area.Tag;
                LoadToAddValidator.Voltage = "";
                LoadToAddValidator.Voltage = _selectedDteq.Voltage.ToString();
            }
            catch (Exception ex) {
                ErrorHelper.ShowErrorMessage(ex);
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
                _selectedLoad.PowerCable.GetRequiredAmps(_selectedLoad);
                GlobalConfig.SelectingNew = true;
                CopySelectedLoad();
                GlobalConfig.SelectingNew = false;
                if (_selectedLoad.CctComponents.Count > 0) {
                    SelectedComponent = (ComponentModel)_selectedLoad.CctComponents[0];

                }
            }
        }
    }

    public IList SelectedLoads { get; internal set; }

    private int _selectedLoadTab;
    public int SelectedLoadTab
    {
        get { return _selectedLoadTab; }
        set { _selectedLoadTab = value; }
    }


    private async Task CopySelectedLoad()
    {
        try {
            await CopySelectedLoadAsync();
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }

        async Task CopySelectedLoadAsync()
        {
            LoadToAddValidator.FedFromTag = "";
            LoadToAddValidator.FedFromTag = _selectedLoad.FedFromTag;
            LoadToAddValidator.AreaTag = "";
            LoadToAddValidator.AreaTag = _selectedLoad.Area.Tag;
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


    // LISTS                                  //Must be named AssignedLoads to match DTEQ.AssignedLoads property
    public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> { };
    public bool LoadListLoaded { get; set; }


    //Components
    private ComponentModel _selectedComponent;
    public ComponentModel SelectedComponent
    {
        get { return _selectedComponent; }
        set { _selectedComponent = value; }
    }


    //Cables


    #endregion



    #region View Toggles
    //View
    private void ToggleDatagridRowdetailView()
    {
        if (ToggleRowDetailViewProp == "VisibleWhenSelected") {
            ToggleRowDetailViewProp = "Collapsed";
        }
        else if (ToggleRowDetailViewProp == "Collapsed") {
            ToggleRowDetailViewProp = "VisibleWhenSelected";
        }
    }

    #endregion

    #region Command Methods

    // Dteq
    public async void DbGetAll()
    {
        _listManager.GetProjectTablesAndAssigments();
        AssignedLoads.Clear();

        //To clear errors on reload
        DteqToAddValidator.ClearErrors();
        LoadToAddValidator.ClearErrors();
    }

    public void OnProjectLoaded(object source, EventArgs e)
    {
        AssignedLoads.Clear();
    }

    public void DbSaveAll()
    {

        try {
            _listManager.SaveAll();
        }

        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }
    private void SizeAllCables()
    {
        Task.Run(() => SizeAllCablesAsync());
    }
    private async Task SizeAllCablesAsync()
    {
        try {
            foreach (var item in _listManager.IDteqList) {
                item.SizePowerCable();
            }
            foreach (var item in _listManager.LoadList) {
                item.SizePowerCable();
            }
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }
    private void CalculateAllCableAmps()
    {
        foreach (var item in _listManager.IDteqList) {
            item.PowerCable.CalculateAmpacity(item);
        }
        foreach (var item in _listManager.LoadList) {
            item.PowerCable.CalculateAmpacity(item);
        }
    }
    private void CalculateSingleEqCableSize()
    {
        CalculateSingleDteqCableSize();
        CalculateSingleLoadCableSize();
    }
    private void CalculateSingleEqCableAmps()
    {
        CalculateSingleDteqCableAmps();
        CalculateSingleLoadCableAmps();
    }
    private void CalculateSingleDteqCableSize()
    {
        if (_selectedDteq != null && _selectedDteq.PowerCable != null) {
            _selectedDteq.SizePowerCable();
        }

    }
    private void CalculateSingleDteqCableAmps()
    {
        if (_selectedDteq != null && _selectedDteq.PowerCable != null) {
            _selectedDteq.PowerCable.CalculateAmpacity(_selectedDteq);
        }
    }
    private void CalculateSingleLoadCableSize()
    {
        if (_selectedLoad != null && _selectedLoad.PowerCable != null) {
            _selectedLoad.SizePowerCable();
        }

    }
    private void CalculateSingleLoadCableAmps()
    {
        if (_selectedLoad != null && _selectedLoad.PowerCable != null) {
            _selectedLoad.PowerCable.CalculateAmpacity(_selectedLoad);
        }
    }

    public void AddDteq(object dteqToAddObject) //typeOf DteqToAddValidator
    {
        //Move AddDteq to DteqManager
        DteqToAddValidator dteqToAddValidator = (DteqToAddValidator)dteqToAddObject;
        try {
            var IsValid = dteqToAddValidator.IsValid(); //to help debug
            var errors = dteqToAddValidator._errorDict; //to help debug
            if (IsValid) {

                IDteq newDteq = _dteqFactory.CreateDteq(dteqToAddValidator);

                IDteq dteqSubscriber = _listManager.DteqList.FirstOrDefault(d => d == newDteq.FedFrom);
                if (dteqSubscriber != null) {
                    //dteqSubscriber.AssignedLoads.Add(newDteq); load gets added to AssignedLoads inside DistributionManager.UpdateFedFrom();
                    newDteq.LoadingCalculated += dteqSubscriber.OnAssignedLoadReCalculated;
                    newDteq.PropertyUpdated += DaManager.OnDteqPropertyUpdated;
                }

                //Save to Db
                newDteq.Id = DaManager.SaveDteqGetId(newDteq);

                _listManager.AddDteq(newDteq);
                newDteq.CalculateLoading(); //after dteq is inserted to get a new Id

                //Cable
                newDteq.CreatePowerCable();
                newDteq.SizePowerCable();
                newDteq.CalculateCableAmps();
                newDteq.PowerCable.Id = DaManager.prjDb.InsertRecordGetId(newDteq.PowerCable, GlobalConfig.CableTable, SaveLists.PowerCableSaveList);
                _listManager.CableList.Add(newDteq.PowerCable); // newCable is already getting added
                RefreshDteqTagValidation();
            }
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }

    public void DeleteDteq(object dteqObject)
    {
        //Move DeleteDteq to DteqManager

        try {

            IDteq dteqToDelete = DteqFactory.Recast(dteqObject);
            if (dteqToDelete != null) {
                //children first

                dteqToDelete.Tag = GlobalConfig.Deleted;
                _listManager.UnregisterDteqFromLoadEvents(dteqToDelete);
                DeletePowerCable(dteqToDelete);
                DistributionManager.RetagLoadsOfDeleted(dteqToDelete);

                if (dteqToDelete.FedFrom != null) {
                    dteqToDelete.FedFrom.AssignedLoads.Remove(dteqToDelete);
                }
                _listManager.DeleteDteq(dteqToDelete);
                DaManager.DeleteDteq(dteqToDelete);
                RefreshDteqTagValidation();

                if (_listManager.IDteqList.Count > 0) {
                    SelectedDteq = _listManager.IDteqList[_listManager.IDteqList.Count - 1];
                }
            }
        }
        catch (Exception ex) {
            MessageBox.Show(ex.Message);
        }
        return;
    }

    private void DeletePowerCable(IPowerConsumer powerCableUser)
    {
        //TODO - Move to Cable Manager

        if (powerCableUser.PowerCable != null) {
            int cableId = powerCableUser.PowerCable.Id;
            DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, cableId); //await
            _listManager.CableList.Remove(powerCableUser.PowerCable);
        }
        return;
    }

    public void AddLoad(object loadToAddObject)
    {
        AddLoadAsync(loadToAddObject);
    }

    public async Task AddLoadAsync(object loadToAddObject)
    {
        try {
            LoadModel newLoad = await LoadManager.AddLoad(loadToAddObject, _listManager);
            if (newLoad != null) AssignedLoads.Add(newLoad);
            RefreshLoadTagValidation();
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }

    ObservableCollection<IPowerConsumer> Selectedloads = new ObservableCollection<IPowerConsumer>();


    // Loads
    public async void GetLoadList()
    {
        await GetLoadListAsync();
    }
    private async Task GetLoadListAsync()
    {
        AssignedLoads.Clear(); //Must be named AssignedLoads  to match DTEQ.AssignedLoads
        foreach (var load in _listManager.LoadList) {
            AssignedLoads.Add(load);
        }
        LoadListLoaded = true;
    }
    private void CalculateLoad()
    {
        if (SelectedLoad != null) {
            SelectedLoad.CalculateLoading();
        }
    }
    public void DeleteLoad(object selectedLoadObject)
    {

        if (SelectedLoad == null || SelectedLoad is DistributionEquipment) return;

        var message = $"Delete load {SelectedLoad.Tag}? \n\nThis cannot be undone.";

        if (SelectedLoads.Count > 1) {
            message = $"Delete {SelectedLoads.Count} loads? \n\nThis cannot be undone.";
        }

        if (ConfirmationHelper.Confirm(message)) {
            if (SelectedLoads.Count == 1) {
                DeleteLoadAsync(selectedLoadObject);
            }
            else {
                DeleteLoadsAsync();
            }
        }
    }

    private async Task DeleteLoadsAsync()
    {
        ILoad load;
        while (SelectedLoads.Count > 0) {
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                load = (LoadModel)SelectedLoads[0];
                DeleteLoadAsync(load);
                SelectedLoads.Remove(load);
            }));

        }
    }

    public async Task DeleteLoadAsync(object selectedLoadObject)
    {
        if (selectedLoadObject == null) return;

        try {
            int loadId = await LoadManager.DeleteLoad(selectedLoadObject, _listManager);
            var loadToRemove = AssignedLoads.FirstOrDefault(load => load.Id == loadId);
            AssignedLoads.Remove(loadToRemove);

            RefreshLoadTagValidation();

        }
        catch (Exception ex) {

            if (ex.Message.ToLower().Contains("sql")) {
                ErrorHelper.ShowErrorMessage(ex);
            }
            else {
                ErrorHelper.ShowErrorMessage(ex);
            }
            throw;
        }
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
                ErrorHelper.ShowErrorMessage(ex);
            }

        }
    }


    public void CalculateAll()
    {
        CalculateAllAsync();
    }

    public async Task CalculateAllAsync()
    {
        try {
            GetLoadListAsync();
            //Task.Run(() => _listManager.CalculateDteqLoadingAsync());
            _listManager.CalculateDteqLoadingAsync();

        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }
    #endregion


    #region ComponentCommands
    private void DeleteComponent()
    {
        ComponentManager.DeleteComponent(SelectedLoad, SelectedComponent, _listManager);
    }

    int _compIndex;
    private void ComponentMoveUp()
    {
        if (SelectedLoad == null || SelectedComponent == null) return;

        for (int i = 0; i < SelectedLoad.CctComponents.Count; i++) {
            if (SelectedComponent.Id == SelectedLoad.CctComponents[i].Id) {
                _compIndex = Math.Max(0, i - 1);
                SelectedLoad.CctComponents.Move(i, _compIndex);
                SelectedComponent = (ComponentModel)SelectedLoad.CctComponents[_compIndex];
            }
        }
        for (int i = 0; i < SelectedLoad.CctComponents.Count; i++) {
            SelectedLoad.CctComponents[i].SequenceNumber = i;
        }
        SelectedLoad.CctComponents = new ObservableCollection<IComponent>(SelectedLoad.CctComponents.OrderBy(c => c.SequenceNumber).ToList());

        CableManager.UpdateLoadPowerComponentCablesAsync(SelectedLoad, _listManager);

    }

    private void ComponentMoveDown()
    {
        if (SelectedLoad == null || SelectedComponent == null) return;

        for (int i = 0; i < SelectedLoad.CctComponents.Count; i++) {
            if (SelectedComponent.Id == SelectedLoad.CctComponents[i].Id) {
                _compIndex = Math.Min(i + 1, SelectedLoad.CctComponents.Count - 1);
                SelectedLoad.CctComponents.Move(i, _compIndex);

                SelectedComponent = (ComponentModel)SelectedLoad.CctComponents[_compIndex];

            }
        }
        for (int i = 0; i < SelectedLoad.CctComponents.Count; i++) {
            SelectedLoad.CctComponents[i].SequenceNumber = i;
        }
        SelectedLoad.CctComponents.OrderBy(x => x.SequenceNumber);
        CableManager.UpdateLoadPowerComponentCablesAsync(SelectedLoad, _listManager);

    }

    private void ToggleLoadDisconnect()
    {
        LoadModel selectedLoad = (LoadModel)SelectedLoad;
        //selectedLoad.DisconnectBool = !selectedLoad.DisconnectBool;
        try {
            CableManager.UpdateLoadPowerComponentCablesAsync(selectedLoad, _listManager);
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }
    private void ToggleLoadDrive()
    {
        LoadModel selectedLoad = (LoadModel)SelectedLoad;
        //selectedLoad.DisconnectBool = !selectedLoad.DisconnectBool;
        try {
            CableManager.UpdateLoadPowerComponentCablesAsync(selectedLoad, _listManager);
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
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

    public bool IsTagAvailable(string tag, ObservableCollection<DteqModel> dteqList, ObservableCollection<LoadModel> loadList)
    {
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
    #endregion

    #region ComboBox Lists
    public ObservableCollection<string> DteqTypes { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<string> VoltageTypes { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<CableTypeModel> DteqCableTypes { get; set; } = new ObservableCollection<CableTypeModel>();
    public ObservableCollection<CableTypeModel> LoadCableTypes { get; set; } = new ObservableCollection<CableTypeModel>();
    public ObservableCollection<double> CableSpacing { get; set; } = new ObservableCollection<double>();
    public ObservableCollection<string> CableInstallationTypes { get; set; } = new ObservableCollection<string>();
    public void CreateComboBoxLists()
    {
        DteqTypes.Clear();
        foreach (var item in Enum.GetNames(typeof(EDTLibrary.DteqTypes))) {
            DteqTypes.Add(item.ToString());
        }

        VoltageTypes.Clear();
        foreach (var item in TypeManager.VoltageTypes) {
            VoltageTypes.Add(item.Voltage.ToString());
        }

        CableSpacing.Clear();
        CableSpacing.Add(100);
        CableSpacing.Add(0);

        CableInstallationTypes.Clear();
        CableInstallationTypes.Add(GlobalConfig.CableInstallationType_LadderTray);
        CableInstallationTypes.Add(GlobalConfig.CableInstallationType_DirectBuried);
        CableInstallationTypes.Add(GlobalConfig.CableInstallationType_RacewayConduit);
    }
    #endregion


    #region WindowSizing

    //Dteq
    private System.Windows.GridLength _dteqGridRight = new System.Windows.GridLength(AppSettings.Default.DteqGridRight, GridUnitType.Star);
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

            //DteqGridHeight = 275; //Uncomment to Position, Comment to Lock

            DteqGridHeight += (_dteqGridBottom.Value - oldBottom);
            AppSettings.Default.DteqGridHeight = DteqGridHeight;
            AppSettings.Default.Save();

            LoadGridHeight -= (_dteqGridBottom.Value - oldBottom);
            AppSettings.Default.LoadGridHeight = LoadGridHeight;
            AppSettings.Default.Save();
        }
    }
    public double DteqGridHeight { get; set; }

    //Load
    private System.Windows.GridLength _loadGridRight = new System.Windows.GridLength(AppSettings.Default.LoadGridRight, GridUnitType.Star);
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

            //LoadGridHeight = 350; //Uncomment to position, Comment to Lock

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
    private double _loadGridHeight;
    public double LoadGridHeight
    {
        get { return _loadGridHeight; }
        set
        {
            _loadGridHeight = value;
            AppSettings.Default.LoadGridBottom = _loadGridBottom.Value;
            AppSettings.Default.Save();
        }
    }



    #endregion

}

