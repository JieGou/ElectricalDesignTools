using EDTLibrary;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        //Window Commands
        CloseWindowCommand = new RelayCommand(CloseSelectionWindow);
        SetAreaCommand = new RelayCommand(SetArea);
        SetFedFromCommand = new RelayCommand(SetFedFrom);

        AddDisconnectCommand = new RelayCommand(AddDisconnect);
        AddDriveCommand = new RelayCommand(AddDrive);
        AddLcsCommand = new RelayCommand(AddLcs);

        RemoveDisconnectCommand = new RelayCommand(RemoveDisconnect);
        RemoveDriveCommand = new RelayCommand(RemoveDrive);
        RemoveLcsCommand = new RelayCommand(RemoveLcs);

    }
    private ListManager _listManager;
    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }


    public IList SelectedLoads { get; internal set; }

    public DteqToAddValidator DteqToAddValidator { get; set; }
    public LoadToAddValidator LoadToAddValidator { get; set; }

    public Window SelectionWindow { get; set; }
    public ICommand CloseWindowCommand { get; }
    


    public void CloseSelectionWindow()
    {
        SelectionWindow.Close();
        SelectionWindow = null;
    }

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

        foreach (var item in SelectedLoads) {
            load = (IPowerConsumer)item;
            //dteq.Tag = "New Tag";
            load.FedFrom = ListManager.IDteqList.FirstOrDefault(d => d.Tag == LoadToAddValidator.FedFromTag);
        }
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
                        load.DriveBool = true;
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
                    load.DriveBool = false;
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
}
