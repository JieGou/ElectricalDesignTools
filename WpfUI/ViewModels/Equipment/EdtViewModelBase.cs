using EdtLibrary.Managers;
using EDTLibrary;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Settings;
using PropertyChanged;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfUI.Commands;
using WpfUI.Helpers;
using WpfUI.Windows.SelectionWindows;
using static System.Windows.Forms.MonthCalendar;

namespace WpfUI.ViewModels.Equipment;

[AddINotifyPropertyChangedInterface]
public abstract class EdtViewModelBase: ViewModelBase
{
    protected EdtViewModelBase(ListManager listManager)
    {
        _listManager = listManager;

        DteqToAddValidator = new DteqToAddValidator(_listManager);
        LoadToAddValidator = new LoadToAddValidator(_listManager);


        //Sub-Menu
        CalculateAllCommand = new RelayCommand(CalculateAll);
        AutoSizeAllCablesCommand = new RelayCommand(AutoSizeAllCables);

        //Window Commands
        CloseWindowCommand = new RelayCommand(CloseSelectionWindow);

        //ContextMenu
        ShowChangeAreaCommand = new RelayCommand(ShowChangeArea);
        ChangeAreaCommand = new RelayCommand(ChangeArea);

        ShowChangeFedFromCommand = new RelayCommand(ShowChangeFedFrom);
        ChangeFedFromCommand = new RelayCommand(ChangeFedFrom);


        ShowChangeLoadTypeCommand = new RelayCommand(ShowChangeLoadType);
        ChangeLoadTypeCommand = new RelayCommand(ChangeLoadType);



        ShowSetDemandFactorCommand = new RelayCommand(ShowSetDemandFactor);
        SetDemandFactorCommand = new RelayCommand(SetDemandFactor);

        ShowSetPowerFactorCommand = new RelayCommand(ShowSetPowerFactor);
        SetPowerFactorCommand = new RelayCommand(SetPowerFactor);

        ShowSetEfficiencyCommand = new RelayCommand(ShowSetEfficiency);
        SetEfficiencyCommand = new RelayCommand(SetEfficiency);


        AddDisconnectCommand = new RelayCommand(AddDisconnect);
        AddDriveCommand = new RelayCommand(AddDrive);
        AddLcsCommand = new RelayCommand(AddLcs);

        RemoveDisconnectCommand = new RelayCommand(RemoveDisconnect);
        RemoveDriveCommand = new RelayCommand(RemoveDrive);
        RemoveLcsCommand = new RelayCommand(RemoveLcs);


        DeleteLoadCommand = new RelayCommand(DeleteLoad);

    }
    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }
    private ListManager _listManager;


    public virtual IPowerConsumer SelectedLoad { get; set; }
    public virtual ObservableCollection<IPowerConsumer> SelectedLoads { get; set; } = new ObservableCollection<IPowerConsumer>();

    public DteqToAddValidator DteqToAddValidator { get; set; }
    public LoadToAddValidator LoadToAddValidator { get; set; }
    public object SelectedEquipment { get; set; }

    public virtual ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> { };


    public bool IsBusy
    {
        get { return _isBusy; }
        set { _isBusy = value; }
    }
    private bool _isBusy;




    public Window SelectionWindow { get; set; }
    public ICommand CloseWindowCommand { get; }
    public void CloseSelectionWindow()
    {
        SelectionWindow.Close();
        SelectionWindow = null;
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

    public ICommand CalculateAllCommand { get; }
    public void CalculateAll()
    {
        IsBusy = true;

        CalculateAllAsync();
    }
    public async Task CalculateAllAsync()
    {
        try {
            await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => {
                IsBusy = true;
            }));

            await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {

                foreach (var item in _listManager.LoadList) {
                    item.CalculateLoading();
                }

                foreach (var item in _listManager.DteqList) {
                    item.CalculateLoading();
                }
                IsBusy = false;

            }));

        }
        catch (Exception ex) {
            //NotificationHandler.ShowErrorMessage(ex);
            IsBusy = false;

        }
        finally {
            IsBusy = false;

        }
    }

    public ICommand AutoSizeAllCablesCommand { get; }
    private void AutoSizeAllCables()
    {
        IsBusy = true;

        AutoSizeAllCablesAsync();
    }
    private async Task AutoSizeAllCablesAsync()
    {
        try {
            await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => {
                IsBusy = true;
            }));
            await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {


                foreach (var item in _listManager.IDteqList) {
                    CableManager.IsAutosizing = true;
                    item.SizePowerCable();
                    CableManager.IsAutosizing = false;
                    item.PowerCable.OnPropertyUpdated();
                }
                foreach (var item in _listManager.LoadList) {
                    CableManager.IsAutosizing = true;
                    item.SizePowerCable();
                    CableManager.IsAutosizing = false;
                    item.PowerCable.OnPropertyUpdated();
                }
                IsBusy = false;
            }));

        }
        catch (Exception ex) {
            //NotificationHandler.ShowErrorMessage(ex);
            IsBusy = false;

        }
        finally {
            IsBusy = false;

        }
    }



    #region Load List Context Menu 

    //Area
    public ICommand ShowChangeAreaCommand { get; }
    public void ShowChangeArea()
    {
        if (SelectionWindow == null) {

            ChangeAreaWindow areaSelectionWindow = new ChangeAreaWindow();
            areaSelectionWindow.DataContext = this;
            SelectionWindow = areaSelectionWindow;
            areaSelectionWindow.ShowDialog();
        }
    }
    public ICommand ChangeAreaCommand { get; }
    public void ChangeArea()
    {

        if (SelectedLoads == null) return;
        IEquipment load;

        foreach (var item in SelectedLoads) {
            load = (IEquipment)item;
            //dteq.Tag = "New Tag";
            load.Area = ListManager.AreaList.FirstOrDefault(d => d.Tag == LoadToAddValidator.AreaTag);
        }

        if (SelectionWindow != null) {
            CloseSelectionWindow();
        }
    }

    //Fed From
    public ICommand ShowChangeFedFromCommand { get; }
    public void ShowChangeFedFrom()
    {
        if (SelectionWindow == null) {
            ChangeFedFromWindow fedFromSelectionWindow = new ChangeFedFromWindow();
            fedFromSelectionWindow.DataContext = this;
            SelectionWindow = fedFromSelectionWindow;
            fedFromSelectionWindow.ShowDialog();
        }
    }
    public ICommand ChangeFedFromCommand { get; }
    public void ChangeFedFrom()
    {
        
        if (SelectedLoads == null) return;
        IPowerConsumer load;


        var list = new List<UpdateFedFromItem>();
        

        foreach (var loadItem in SelectedLoads) {
            load = (IPowerConsumer)loadItem;
            //load.FedFrom = ListManager.IDteqList.FirstOrDefault(d => d.Tag == LoadToAddValidator.FedFromTag);
            var newSupplier = ListManager.IDteqList.FirstOrDefault(d => d.Tag == LoadToAddValidator.FedFromTag);


            if (newSupplier!=null) {
                list.Add(new UpdateFedFromItem {
                    Caller = load,
                    NewSupplier = newSupplier,
                    OldSupplier = load.FedFrom
                }); ;  
            }

        }

        FedFromManager.UpdateFedFrom_List(list);

        if (SelectionWindow != null) {
            CloseSelectionWindow();
        }
    }


    //Change Load Type
    public ICommand ShowChangeLoadTypeCommand { get; }
    public void ShowChangeLoadType()
    {
        if (SelectionWindow == null) {
            var selectionWindow = new ChangeLoadTypeWindow();
            selectionWindow.DataContext = this;
            SelectionWindow = selectionWindow;
            selectionWindow.ShowDialog();
        }
    }

    public ICommand ChangeLoadTypeCommand { get; }
    public void ChangeLoadType()
    {
        
        if (SelectedLoads == null) return;

        ILoad load;
        foreach (var loadItem in SelectedLoads) {
            if (loadItem is ILoad) {
                load = (ILoad)loadItem;
                load.Type =  LoadType;
            }
        }

        if (SelectionWindow != null) {
            CloseSelectionWindow();
        }
    }
    public string LoadType { get; set; } = "MOTOR";


    

    //Demand Factor

    public ICommand ShowSetDemandFactorCommand { get; }
    public void ShowSetDemandFactor()
    {
        DemandFactor = EdtProjectSettings.DemandFactorDefault;
        if (SelectionWindow == null) {
            SetDemandFactorWindow selectionWindow = new SetDemandFactorWindow();
            selectionWindow.DataContext = this;
            SelectionWindow = selectionWindow;
            selectionWindow.ShowDialog();
        }
    }

    public ICommand SetDemandFactorCommand { get; }
    public void SetDemandFactor()
    {
        
        if (SelectedLoads == null) return;

        ILoad load;
        double outVal;

        if (double.TryParse(DemandFactor, out outVal)) {

            if (outVal >= 0 || outVal <= 1) {
                foreach (var loadItem in SelectedLoads) {
                    if (loadItem is ILoad) {
                        load = (ILoad)loadItem;
                        load.DemandFactor = outVal;
                    }
                }
            }
        }

        if (SelectionWindow != null) {
            CloseSelectionWindow();
        }
    }
    public string DemandFactor
    {
        get { return _demandFactor; }
        set
        {
            _demandFactor = value;
            double outVal;
            ClearErrors();
            if (double.TryParse(value, out outVal) == false) {
                AddError(nameof(DemandFactor), "Invalid Value");
            }
            else {
                if (outVal < 0 || outVal > 1) {
                    AddError(nameof(DemandFactor), "Invalid Value");
                }
            }
        }
    }
    private string _demandFactor;


    //Power Factor
    public ICommand ShowSetPowerFactorCommand { get; }
    public void ShowSetPowerFactor()
    {
        PowerFactor = EdtProjectSettings.LoadDefaultPowerFactor_Other;
        if (SelectionWindow == null) {
            var selectionWindow = new SetPowerFactorWindow();
            selectionWindow.DataContext = this;
            SelectionWindow = selectionWindow;
            selectionWindow.ShowDialog();
        }
    }

    public ICommand SetPowerFactorCommand { get; }
    public void SetPowerFactor()
    {
        if (SelectedLoads == null) return;

        ILoad load;
        double outVal;

        if (double.TryParse(PowerFactor, out outVal)) {

            if (outVal > 0 || outVal <= 1) {
                foreach (var loadItem in SelectedLoads) {
                    if (loadItem is ILoad) {
                        load = (ILoad)loadItem;
                        load.PowerFactor = outVal;
                    }
                }
            }
        }

        if (SelectionWindow != null) {
            CloseSelectionWindow();
        }
    }
    public string PowerFactor
    {
        get { return _powerFactor; }
        set
        {
            _powerFactor = value;
            double outVal;
            ClearErrors();
            if (double.TryParse(value, out outVal) == false) {
                AddError(nameof(PowerFactor), "Invalid Value");
            }
            else {
                if (outVal <= 0 || outVal > 1) {
                    AddError(nameof(PowerFactor), "Invalid Value");
                }
            }
        }
    }
    private string _powerFactor;


    //Efficiency
    public ICommand ShowSetEfficiencyCommand { get; }
    public void ShowSetEfficiency()
    {
        Efficiency = EdtProjectSettings.LoadDefaultEfficiency_Other;
        if (SelectionWindow == null) {
            var selectionWindow = new SetEfficiencyWindow();
            selectionWindow.DataContext = this;
            SelectionWindow = selectionWindow;
            selectionWindow.ShowDialog();
        }
    }


    public ICommand SetEfficiencyCommand { get; }
    public void SetEfficiency()
    {

        if (SelectedLoads == null) return;

        ILoad load;
        double outVal;

        if (double.TryParse(Efficiency, out outVal)) {

            if (outVal > 0 || outVal <= 1) {
                foreach (var loadItem in SelectedLoads) {
                    if (loadItem is ILoad) {
                        load = (ILoad)loadItem;
                        load.Efficiency = outVal;
                    }
                }
            }
        }

        if (SelectionWindow != null) {
            CloseSelectionWindow();
        }
    }
    public string Efficiency
    {
        get { return _efficiency; }
        set
        {
            _efficiency = value;
            double outVal;
            ClearErrors();
            if (double.TryParse(value, out outVal) == false) {
                AddError(nameof(Efficiency), "Invalid Value");
            }
            else {
                if (outVal <= 0 || outVal > 1) {
                    AddError(nameof(Efficiency), "Invalid Value");
                }
            }
        }
    }
    private string _efficiency;



    public ICommand AddDisconnectCommand { get; }
    private void AddDisconnect()
    {
        AddDisconnectAsync();
    }
    private async Task AddDisconnectAsync()
    {

        await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {

            foreach (var loadObject in SelectedLoads) {
                if (loadObject.GetType() == typeof(LoadModel)) {
                    IPowerConsumer load = (IPowerConsumer)loadObject;
                    load.DisconnectBool = true;
                }
            }

        }));

    }


    public ICommand AddDriveCommand { get; }
    private void AddDrive()
    {
        AddDriveAsync();
    }
    private async Task AddDriveAsync()
    {
        await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            foreach (var loadObject in SelectedLoads) {
                if (loadObject.GetType() == typeof(LoadModel)) {
                    IPowerConsumer load = (IPowerConsumer)loadObject;
                    if (load.Type == LoadTypes.MOTOR.ToString()) {
                        load.StandAloneStarterBool = true;
                    }
                }
            }
        }));
    }

    public ICommand AddLcsCommand { get; }
    private void AddLcs()
    {
        AddLcsAsync();
    }
    private async Task AddLcsAsync()
    {
        await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            foreach (var loadObject in SelectedLoads) {
                if (loadObject.GetType() == typeof(LoadModel)) {
                    IPowerConsumer load = (IPowerConsumer)loadObject;
                    if (load.Type == LoadTypes.MOTOR.ToString()) {
                        load.LcsBool = true;
                    }
                }

            }
        }));
    }

    public ICommand RemoveDisconnectCommand { get; }
    private void RemoveDisconnect()
    {
        RemoveDisconnectAsync();
    }
    private async Task RemoveDisconnectAsync()
    {
        await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            foreach (var loadObject in SelectedLoads) {

                IPowerConsumer load = (IPowerConsumer)loadObject;
                load.DisconnectBool = false;
            }
        }));
    }

    public ICommand RemoveDriveCommand { get; }
    private void RemoveDrive()
    {
        RemoveDriveAsync();
    }
    private async Task RemoveDriveAsync()
    {
        await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            foreach (var loadObject in SelectedLoads) {
                if (loadObject.GetType() == typeof(LoadModel)) {
                    IPowerConsumer load = (IPowerConsumer)loadObject;
                    load.StandAloneStarterBool = false;
                }

            }
        }));
    }

    public ICommand RemoveLcsCommand { get; }
    private void RemoveLcs()
    {
        RemoveLcsAsync();
    }
    private async Task RemoveLcsAsync()
    {
        await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            foreach (var loadObject in SelectedLoads) {
                if (loadObject.GetType() == typeof(LoadModel)) {
                    IPowerConsumer load = (IPowerConsumer)loadObject;
                    load.LcsBool = false;
                }
            }
        }));
    }



    public ICommand DeleteLoadCommand { get; }

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
        LoadModel load;

        var selectedLoads = new ObservableCollection<LoadModel>();
        foreach (var item in SelectedLoads) {
            load = (LoadModel)item;
            selectedLoads.Add(load);
        }
        foreach (var load2 in selectedLoads) {
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                DeleteLoadAsync(load2);
            }));
        }

        foreach (var dteq in ListManager.DteqList) {
            dteq.CalculateLoading();
        }
    }

    public async Task DeleteLoadAsync(object selectedLoadObject)
    {
        if (selectedLoadObject == null) return;

        try {

            LoadModel load = (LoadModel)selectedLoadObject;
            await LoadManager.DeleteLoadAsync(selectedLoadObject, _listManager);
            //var loadId = await LoadManager.DeleteLoad(selectedLoadObject, _listManager);
            //var loadToRemove = AssignedLoads.FirstOrDefault(load => load.Id == loadId);
            AssignedLoads.Remove(load);

            LoadToAddValidator.ResetTag();

        }
        catch (Exception ex) {

            if (ex.Message.ToLower().Contains("sql")) {
                NotificationHandler.ShowErrorMessage(ex);
            }
            else {
                NotificationHandler.ShowErrorMessage(ex);
            }
            throw;
        }
    }

    #endregion
}
