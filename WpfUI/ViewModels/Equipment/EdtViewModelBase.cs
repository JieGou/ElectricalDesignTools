﻿using EDTLibrary;
using EDTLibrary.DistributionControl;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
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
        SetAreaCommand = new RelayCommand(SetArea);
        SetFedFromCommand = new RelayCommand(SetFedFrom);

        AddDisconnectCommand = new RelayCommand(AddDisconnect);
        AddDriveCommand = new RelayCommand(AddDrive);
        AddLcsCommand = new RelayCommand(AddLcs);

        RemoveDisconnectCommand = new RelayCommand(RemoveDisconnect);
        RemoveDriveCommand = new RelayCommand(RemoveDrive);
        RemoveLcsCommand = new RelayCommand(RemoveLcs);


        DeleteLoadCommand = new RelayCommand(DeleteLoad);

    }
    private ListManager _listManager;
    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }


    public virtual IPowerConsumer SelectedLoad { get; set; }
    public virtual IList SelectedLoads { get; internal set; }

    public DteqToAddValidator DteqToAddValidator { get; set; }
    public LoadToAddValidator LoadToAddValidator { get; set; }
    public object SelectedEquipment { get; set; }

    public virtual ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> { };



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
            ErrorHelper.ShowErrorMessage(ex);
        }
    }

    public ICommand CalculateAllCommand { get; }
    public void CalculateAll()
    {
            CalculateAllAsync();
    }
    public async Task CalculateAllAsync()
    {
        try {
            await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {

                foreach (var item in _listManager.LoadList) {
                    item.CalculateLoading();
                }

                foreach (var item in _listManager.DteqList) {
                    item.CalculateLoading();
                }


            }));
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }

    public ICommand AutoSizeAllCablesCommand { get; }
    private void AutoSizeAllCables()
    {
        AutoSizeAllCablesAsync();
    }
    private async Task AutoSizeAllCablesAsync()
    {
        try {
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
            }));
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }



    #region oad List Context Menu 
    public ICommand SetAreaCommand { get; }
    public void SetArea()
    {
        if (SelectionWindow == null) {
        
            AreaSelectionWindow areaSelectionWindow = new AreaSelectionWindow();
            areaSelectionWindow.DataContext = this;
            SelectionWindow = areaSelectionWindow;
            areaSelectionWindow.ShowDialog();
        }

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

    public ICommand SetFedFromCommand { get; }
    public void SetFedFrom()
    {
        if (SelectionWindow == null) {
            FedFromSelectionWindow fedFromSelectionWindow = new FedFromSelectionWindow();
            fedFromSelectionWindow.DataContext = this;
            SelectionWindow = fedFromSelectionWindow;
            fedFromSelectionWindow.ShowDialog();
        }

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

        DistributionManager.UpdateFedFrom_List(list);

        if (SelectionWindow != null) {
            CloseSelectionWindow();
        }
    }

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
                ErrorHelper.ShowErrorMessage(ex);
            }
            else {
                ErrorHelper.ShowErrorMessage(ex);
            }
            throw;
        }
    }

    #endregion
}
