using AutoCAD;
using AutocadLibrary;
using EdtLibrary.Commands;
using EDTLibrary;
using EDTLibrary.Autocad.Interop;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfUI.Helpers;
using WpfUI.PopupWindows;
using WpfUI.Services;
using WpfUI.Stores;
using WpfUI.ViewModels.Cables;
using WpfUI.ViewModels.Equipment;

namespace WpfUI.ViewModels.Electrical;
internal class SingleLineViewModel: EdtViewModelBase
{

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

    public SolidColorBrush SingleLineViewBackground { get; set; } = new SolidColorBrush(Colors.LightCyan);


    public ObservableCollection<IPowerConsumer> ViewableDteqList

    {
        get
        {
            List<IPowerConsumer> subList = new List<IPowerConsumer>();
            subList.AddRange(ListManager.DteqList.FirstOrDefault(d => d.Tag == GlobalConfig.Utility).AssignedLoads);
            return new ObservableCollection<IPowerConsumer>(subList);
        }
    }

    private ListCollectionView _dteqCollectionView;
    public ListCollectionView DteqCollectionView
    {
        get
        {
            if (_dteqCollectionView == null) {
                _dteqCollectionView = new ListCollectionView(ViewableDteqList);
            }

            _dteqCollectionView.Filter = (d) => {
                IEquipment dteq = (IEquipment)d;
                if (dteq != null)
                // If filter is turned on, filter completed items.
                {
                    if (dteq != null) {
                        if (dteq is DistributionEquipment) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                }
                return false;
            };
            return _dteqCollectionView;
        }
        set
        {
            _dteqCollectionView = DteqCollectionView;

            
        }
    }

    public SingleLineViewModel(ListManager listManager) : base(listManager)
    {
        ListManager = listManager;

        DrawSingleLineAcadCommand = new RelayCommand(DrawSingleLineRelay);
    }

    //Equipment
    private IEquipment _selectedLoadEquipment;
    public IEquipment SelectedLoadEquipment
    {
        get { return _selectedLoadEquipment; }
        set { _selectedLoadEquipment = value; }
    }

    public bool IsSelectedLoadCable { get; set; }

    private IEquipment _selectedLoadCable;
    public IEquipment SelectedLoadCable
    {
        get { return _selectedLoadCable; }
        set
        {
            _selectedLoadCable = value;

            if (_selectedLoadCable.GetType() == typeof(LoadModel)) {
                var load = (LoadModel)_selectedLoadCable;
                load.PowerCable.ValidateCableSize(load.PowerCable);
                load.PowerCable.CreateTypeList(load);
            }
            else if (_selectedLoadCable is (DistributionEquipment)) {
                var dteq = DteqFactory.Recast(_selectedLoadCable);
                dteq.PowerCable.ValidateCableSize(dteq.PowerCable);
                dteq.PowerCable.CreateTypeList(dteq);
            }
            else if (_selectedLoadCable.GetType() == typeof(ComponentModel)) {
                var component = (ComponentModel)_selectedLoadCable;
                //TODO - Style for cable graphic so that IsValid is detected without reloading
                component.PowerCable.ValidateCableSize(component.PowerCable);
                component.PowerCable.CreateTypeList((LoadModel)component.Owner);
            }

        }
    }


    //SelectedItems
    private IDteq _selectedDteq;
    public IDteq SelectedDteq
    {
        get { return _selectedDteq; }
        set
        {
            if (value == null) return;
            AssignedLoads = new ObservableCollection<IPowerConsumer>();
            IsBusy = true;

            System.Windows.Application.Current.Dispatcher.Invoke(() => {

                //used for fedfrom Validation
                _selectedDteq = value;

                if (_selectedDteq != null) {
                    //AssignedLoads = new ObservableCollection<IPowerConsumer>(_selectedDteq.AssignedLoads);
                    
                    foreach (var item in _selectedDteq.AssignedLoads) {
                        AssignedLoads.Add(item);
                        //AllowUIToUpdate();
                    }

                    GlobalConfig.SelectingNew = true;
                    GlobalConfig.SelectingNew = false;
                    if (AssignedLoads.Count > 0) {
                        SelectedLoad = AssignedLoads[0];

                    }
                }

            }, DispatcherPriority.Background,null);

            System.Windows.Application.Current.Dispatcher.BeginInvoke(() => IsBusy = false, DispatcherPriority.ContextIdle, null);


        }

    }

    private static void AllowUIToUpdate()
    {
        DispatcherFrame frame = new();
        // DispatcherPriority set to Input, the highest priority
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate (object parameter)
        {
            frame.Continue = false;
            Thread.Sleep(10); // Stop all processes to make sure the UI update is perform
            return null;
        }), null);
        Dispatcher.PushFrame(frame);
        // DispatcherPriority set to Input, the highest priority
        System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, new Action(delegate { }));
    }

    public bool IsBusy { get; set; }



    private IPowerConsumer _selectedLoad;

    public IPowerConsumer SelectedLoad
    {
        get { return _selectedLoad; }
        set 
        { 
            _selectedLoad = value;
            if (_selectedLoad.CctComponents.Count > 0) {
                SelectedComponent = (ComponentModel)_selectedLoad.CctComponents[0];

            }
        }
    }


    //Components
    private ComponentModel _selectedComponent;
    public ComponentModel SelectedComponent
    {
        get { return _selectedComponent; }
        set { _selectedComponent = value; }
    }

    public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> { };



    public static NotificationPopup NotificationPopup { get; set; }


    #region Autocad

    public ICommand DrawSingleLineAcadCommand { get; }
    public void DrawSingleLineRelay()
    {
        DrawSingleLineAsycn();
    }
    
    public void DrawSingleLineAsycn()
    {
        var acadService = new AutocadService();
        //acadService.DrawSingleLineAsync(SelectedDteq);
        acadService.CreateSingleLine(SelectedDteq);
    }
    #endregion

}
