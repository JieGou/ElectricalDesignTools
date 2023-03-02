using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.Cables;
using EDTLibrary.Managers;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
using PropertyChanged;
using System;
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
using WpfUI.ViewModels.Equipment;
using WpfUI.ViewModifiers;
using IComponentEdt = EDTLibrary.Models.Components.IComponentEdt;

namespace WpfUI.ViewModels.Electrical;

[AddINotifyPropertyChangedInterface]
public class MjeqViewModel : EdtViewModelBase, INotifyDataErrorInfo
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
            if (EdtProjectSettings.AreaColumnVisible == "True") {
                return true;
            }
            return false;
        }
    }

    public SolidColorBrush SingleLineViewBackground { get; set; } = new SolidColorBrush(Colors.White);


    //CONSTRUCTOR
    public MjeqViewModel(ListManager listManager) : base(listManager)
    {

        _ViewStateManager.ElectricalViewUpdate += OnElectricalViewUpdated;

        //fields
        _listManager = listManager;
        _dteqFactory = new DteqFactory(listManager);


        //members
        DteqGridViewModifier = new DataGridColumnViewToggle();
        LoadGridViewModifier = new DataGridColumnViewToggle();

        //DteqToAddValidator = new DteqToAddValidator(_listManager);
        //LoadToAddValidator = new LoadToAddValidator(_listManager);

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

        CalculateSingleEqCableSizeCommand = new RelayCommand(CalculateSingleEqCableSize);
        CalculateSingleEqCableAmpsCommand = new RelayCommand(CalculateSingleEqCableAmps);

        //DeleteDteqCommand = new AsyncRelayCommand(DeleteDteqAsync, (ex) => MessageBox.Show(ex.Message));
        DeleteDteqCommand = new RelayCommand(DeleteDteq);

        AddDteqCommand = new RelayCommand(AddDteq);
        AddLoadCommand = new RelayCommand(AddLoad);


        GetLoadListCommand = new RelayCommand(GetLoadList);
        CalculateLoadCommand = new RelayCommand(CalculateLoad);
        DeleteLoadCommand = new RelayCommand(DeleteLoad);


        //CalculateAllCommand = new RelayCommand(CalculateAll, startupService.IsProjectLoaded);

        ToggleLoadDisconnectCommand = new RelayCommand(ToggleLoadDisconnect);
        ToggleLoadDriveCommand = new RelayCommand(ToggleLoadDrive);

        ComponentMoveUpCommand = new RelayCommand(ComponentMoveUp);
        ComponentMoveDownCommand = new RelayCommand(ComponentMoveDown);

        DeleteComponentCommand = new RelayCommand(DeleteComponent);


       


    }

    
    public UserControl SelectedElectricalViewModel { get; set; }
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
    public ICommand DeleteDteqCommand { get; }
    public ICommand SizeCablesCommand { get; }
    public ICommand CalculateSingleEqCableSizeCommand { get; }
    public ICommand CalculateSingleEqCableAmpsCommand { get; }

    public ICommand AddDteqCommand { get; }
    public ICommand AddLoadCommand { get; }


    // Load Commands
    public ICommand GetLoadListCommand { get; }
    public ICommand CalculateLoadCommand { get; }
    public ICommand DeleteLoadCommand { get; }






    public ICommand ToggleLoadDisconnectCommand { get; }
    public ICommand ToggleLoadDriveCommand { get; }


    public ICommand ComponentMoveUpCommand { get; }
    public ICommand ComponentMoveDownCommand { get; }
    public ICommand DeleteComponentCommand { get; }

    #endregion


    #region Views States
    public void OnElectricalViewUpdated(object source, EventArgs e)
    {
        RefreshLoadList();
    }

    string loadTag = "";

    public void RefreshLoadList()
    {
        if (SelectedLoad == null) {
            return;
        }
        if (SelectedLoad != null) {
            loadTag = SelectedLoad.Tag;
        }
        if (LoadListLoaded == false && SelectedDteq != null) {
            AssignedLoads.Clear();
            foreach (var load in SelectedDteq.AssignedLoads) {
                AssignedLoads.Add(load);
            }
        }
        else {
            GetLoadList();
        }
        if (loadTag !="") {
            SelectedLoad = ListManager.LoadList.FirstOrDefault(l => l.Tag == loadTag); 
        }
    }   

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

    public ListCollectionView DteqCollectionView
    {
        get
        {
            if (_dteqCollectionView == null) {
                _dteqCollectionView = new ListCollectionView(DteqList);
            }
            return _dteqCollectionView;
        }
        set
        {
            _dteqCollectionView = DteqCollectionView;
        }
    }
    private ListCollectionView _dteqCollectionView;


    //Equipment
    public IEquipment SelectedLoadEquipment
    {
        get { return _selectedLoadEquipment; }
        set 
        { 
            _selectedLoadEquipment = value;
            if (_selectedLoadEquipment is IComponentEdt) {
                // commented out because xaml doesn't know how to show the graphic when SelectedEquipment is a component
                //SelectedEquipment = _selectedLoadEquipment;

            }
            else {
                SelectedEquipment = _selectedLoadEquipment;

            }

            if (value == null) return;

            
        }
    }
    private IEquipment _selectedLoadEquipment;

    public bool IsSelectedLoadCable { get; set; }

    public IEquipment SelectedLoadCable
    {
        get { return _selectedLoadCable; }
        set { 

            _selectedLoadCable = value;
            if (value == null) return;

            if (_selectedLoadCable is DistributionEquipment) {
                var eq = DteqFactory.Recast(_selectedLoadCable);
                eq.PowerCable.Validate(eq.PowerCable);
                eq.PowerCable.CreateTypeList(eq);
            }
            else if (_selectedLoadCable.GetType() == typeof(LoadModel)) {
                var load = (LoadModel)(_selectedLoadCable);
                load.PowerCable.Validate(load.PowerCable);
                load.PowerCable.CreateTypeList(load);
            }
            else if (_selectedLoadCable.GetType() == typeof(ComponentModel)) {
                var component = (ComponentModel)(_selectedLoadCable);
                //TODO - Style for cable graphic so that IsValid is detected without reloading
                component.PowerCable.Validate(component.PowerCable);
                component.PowerCable.CreateTypeList((LoadModel)component.Owner);
            }
        }
    }
    private IEquipment _selectedLoadCable;



    // DTEQ
    public IDteq SelectedDteq
    {
        get { return _selectedDteq; }
        set
        {
            _selectedDteq = value;
            SelectedEquipment = _selectedDteq;
            SelectedLoadCable = _selectedDteq;
            LoadListLoaded = false;
            if (value == null) return;

            //used for fedfrom Validation
            DictionaryStore.CreateDteqDict(_listManager.IDteqList);
            

            if (_selectedDteq != null) {
                AssignedLoads = new ObservableCollection<IPowerConsumer>(_selectedDteq.AssignedLoads);

                //UpdateAssignedLoadsAsync();

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
    private IDteq _selectedDteq;

    private async Task UpdateAssignedLoadsAsync()
    {
       await Task.Run(() =>  UpdateAssignedLoads());
    }

    private void UpdateAssignedLoads()
    {
        AssignedLoads.Clear();
        foreach (var item in _selectedDteq.AssignedLoads) {
            AssignedLoads.Add(item);
        }
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
            NotificationHandler.ShowErrorMessage(ex);
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
                DteqToAddValidator.LineVoltageType = _selectedDteq.LineVoltageType;
                DteqToAddValidator.LoadVoltage = "";
                DteqToAddValidator.LoadVoltage = _selectedDteq.LoadVoltage.ToString();
                DteqToAddValidator.LoadVoltageType = _selectedDteq.LoadVoltageType;

                LoadToAddValidator.FedFromTag = "";
                LoadToAddValidator.FedFromTag = _selectedDteq.Tag;
                LoadToAddValidator.AreaTag = "";
                LoadToAddValidator.AreaTag = _selectedDteq.Area.Tag;
                LoadToAddValidator.Voltage = "";
                LoadToAddValidator.Voltage = _selectedDteq.Voltage.ToString();
                LoadToAddValidator.VoltageType = _selectedDteq.VoltageType;
            }
            catch (Exception ex) {
                NotificationHandler.ShowErrorMessage(ex);
            }
        }
    }

    public IPowerConsumer SelectedLoadObject { get; set; }
    // LOADS
    public IPowerConsumer SelectedLoad
    {
        get { return _selectedLoad; }
        set
        {
            if (value == null) return;
            _selectedLoad = value;
            base.SelectedLoad = _selectedLoad;
            SelectedLoadEquipment = _selectedLoad;
            SelectedLoadCable = _selectedLoad; 
            
            SelectedLoadObject = value;
            

            _selectedLoad.PowerCable.GetRequiredAmps(_selectedLoad);
            GlobalConfig.SelectingNew = true;
            CopySelectedLoad();
            GlobalConfig.SelectingNew = false;
            if (_selectedLoad.CctComponents.Count > 0) {
                SelectedComponent = (ComponentModelBase)_selectedLoad.CctComponents[0];
            }

        }
    }
    private IPowerConsumer _selectedLoad;

    public ObservableCollection<object> Selectedloads { get; set; } = new ObservableCollection<object>();

    private async Task CopySelectedLoad()
    {
        try {
            await CopySelectedLoadAsync();
        }
        catch (Exception ex) {
            NotificationHandler.ShowErrorMessage(ex);
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
            LoadToAddValidator.Voltage = _selectedLoad.VoltageType.Voltage.ToString();
            LoadToAddValidator.VoltageType = null;
            LoadToAddValidator.VoltageType = _selectedLoad.VoltageType;
        }

    }


    // LISTS                                  //Must be named AssignedLoads to match DTEQ.AssignedLoads property
    public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> { };
    public bool LoadListLoaded { get; set; }


    //Components
    public ComponentModelBase SelectedComponent
    {
        get { return _selectedComponent; }
        set { _selectedComponent = value; }
    }
    private ComponentModelBase _selectedComponent;


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

    public void AddDteq(object loadToAddObject)
    {
        AddDteqAsync(loadToAddObject);
    }
    public async Task AddDteqAsync(object dteqToAddObject) //typeOf DteqToAddValidator
    {
        try {
            var newDteq = await DteqManager.AddDteq(dteqToAddObject, _listManager);
            RefreshDteqTagValidation();
        }
        catch (Exception ex) {
            NotificationHandler.ShowErrorMessage(ex);
        }
    }

    public void DeleteDteq(object dteqObject)
    {
        //Move DeleteDteq to DteqManager

        try {

            IDteq dteqToDelete = DteqFactory.Recast(dteqObject);
            if (dteqToDelete != null) {

                DteqManager.DeleteDteqAsync(dteqToDelete, _listManager);
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
            LoadToAddValidator.ResetTag();
        }
        catch (Exception ex) {
            NotificationHandler.ShowErrorMessage(ex);
        }
    }



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
                SelectedComponent = (ComponentModelBase)SelectedLoad.CctComponents[_compIndex];
                break;
            }
        }
        for (int i = 0; i < SelectedLoad.CctComponents.Count; i++) {
            SelectedLoad.CctComponents[i].SequenceNumber = i;
        }
        SelectedLoad.CctComponents = new ObservableCollection<IComponentEdt>(SelectedLoad.CctComponents.OrderBy(c => c.SequenceNumber).ToList());

        CableManager.AddAndUpdateLoadPowerComponentCablesAsync(SelectedLoad, _listManager);

    }

    private void ComponentMoveDown()
    {
        if (SelectedLoad == null || SelectedComponent == null) return;

        for (int i = 0; i < SelectedLoad.CctComponents.Count; i++) {
            if (SelectedComponent.Id == SelectedLoad.CctComponents[i].Id) {
                _compIndex = Math.Min(i + 1, SelectedLoad.CctComponents.Count - 1);
                SelectedLoad.CctComponents.Move(i, _compIndex);

                SelectedComponent = (ComponentModel)SelectedLoad.CctComponents[_compIndex];
                break;
            }
        }
        for (int i = 0; i < SelectedLoad.CctComponents.Count; i++) {
            SelectedLoad.CctComponents[i].SequenceNumber = i;
        }
        SelectedLoad.CctComponents.OrderBy(x => x.SequenceNumber);
        CableManager.AddAndUpdateLoadPowerComponentCablesAsync(SelectedLoad, _listManager);

    }

    private void ToggleLoadDisconnect()
    {
        LoadModel selectedLoad = (LoadModel)SelectedLoad;
        //selectedLoad.DisconnectBool = !selectedLoad.DisconnectBool;
        //try {
        //    CableManager.AddAndUpdateLoadPowerComponentCablesAsync(selectedLoad, _listManager);
        //}
        //catch (Exception ex) {
        //    ErrorHelper.ShowErrorMessage(ex);
        //}
    }
    private void ToggleLoadDrive()
    {
        LoadModel selectedLoad = (LoadModel)SelectedLoad;
        //selectedLoad.DisconnectBool = !selectedLoad.DisconnectBool;
        //try {
        //    CableManager.AddAndUpdateLoadPowerComponentCablesAsync(selectedLoad, _listManager);
        //}
        //catch (Exception ex) {
        //    ErrorHelper.ShowErrorMessage(ex);
        //}
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

