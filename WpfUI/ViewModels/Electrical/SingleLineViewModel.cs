using EdtLibrary.Commands;
using EDTLibrary;
using EDTLibrary.Managers;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfUI.PopupWindows;
using WpfUI.Services;
using WpfUI.ViewModels.Equipment;

namespace WpfUI.ViewModels.Electrical;
internal class SingleLineViewModel: EdtViewModelBase
{
    public SingleLineViewModel(ListManager listManager) : base(listManager)
    {
        _listManager = listManager;

        _ViewStateManager.ElectricalViewUpdate += OnElectricalViewUpdated;

        //Commands
        DeleteLoadCommand = new RelayCommand(DeleteLoad);

        DrawSingleLineAcadCommand = new RelayCommand(DrawSingleLineRelay);

        MoveLoadUpCommand = new RelayCommand(MoveLoadUp);
        MoveLoadDownCommand = new RelayCommand(MoveLoadDown);
    }


    #region View State
    public void OnElectricalViewUpdated(object source, EventArgs e)
    {
        RefreshDteqTreeView();
        RefreshSingleLine();
    }

    public void RefreshDteqTreeView()
    {
        DteqCollectionView = new ListCollectionView(ViewableDteqList);
    }
    
    public void RefreshSingleLine()
    {
        AssignedLoads.Clear();
        if (SelectedDteq == null) return; 

        var dteq = ListManager.DteqList.FirstOrDefault(d => d.Tag == SelectedDteq.Tag);

        if (dteq == null) return;
        foreach (var item in dteq.AssignedLoads) {
            AssignedLoads.Add(item);
        }
        SelectedDteq = dteq;
    }

    
    internal void ClearSelections()
    {
        //Equipment
        ListManager.CreateEquipmentList();
        foreach (var item in ListManager.EqList) {
            item.IsSelected = false;
        }

        //Cable
        foreach (var item in ListManager.CableList) {
            item.IsSelected = false;
        }

    }
    #endregion


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

    public SolidColorBrush SingleLineViewBackground { get; set; } = new SolidColorBrush(Colors.LightCyan);


    public ObservableCollection<IPowerConsumer> ViewableDteqList

    {
        get
        {
            if (ListManager.DteqList.Count>0) {
                List<IPowerConsumer> subList = new List<IPowerConsumer>();
                subList.AddRange(ListManager.DteqList.FirstOrDefault(d => d.Tag == GlobalConfig.UtilityTag).AssignedLoads);
                return new ObservableCollection<IPowerConsumer>(subList);
            }
            else {
                _ViewStateManager.ElectricalViewUpdate -= OnElectricalViewUpdated;
                List<IPowerConsumer> subList = new List<IPowerConsumer>();
                subList.AddRange(ScenarioManager.ListManager.DteqList.FirstOrDefault(d => d.Tag == GlobalConfig.UtilityTag).AssignedLoads);
                return new ObservableCollection<IPowerConsumer>(subList);
            }
        }
            
    }

    private ListCollectionView _dteqCollectionView;
    public ListCollectionView DteqCollectionView
    {
        get
        {
            return _dteqCollectionView;
        }
        set
        {
            //_dteqCollectionView = new ListCollectionView(ViewableDteqList);

            _dteqCollectionView = value;

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
        }
    }

   

    //Equipment
    private IEquipment _selectedLoadEquipment;
    public IEquipment SelectedLoadEquipment
    {
        get { return _selectedLoadEquipment; }
        set 
        { 
            _selectedLoadEquipment = value;
            if (_selectedLoadEquipment is ComponentModelBase) {
                var comp = (ComponentModelBase)_selectedLoadEquipment;
                var compOwner = (IPowerConsumer)comp.Owner;
                compOwner.SelectedComponent = comp;
                //_selectedLoadEquipment = compOwner;
                _selectedLoadEquipment = comp;
            }
            if (SelectedLoadEquipment is ILoad) {
                SelectedLoad = (ILoad)_selectedLoadEquipment; 
            }

            if (_selectedLoadEquipment is IPowerConsumer) {
                var selectedMovableEquipment = (IPowerConsumer)_selectedLoadEquipment;
                if (selectedMovableEquipment.FedFrom == SelectedDteq) {
                    SelectedMoveableEquipment = selectedMovableEquipment;
                }
                else if (selectedMovableEquipment.FedFrom.FedFrom == SelectedDteq){
                    SelectedMoveableEquipment = selectedMovableEquipment.FedFrom;
                }
            }
        }
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
                if (load.PowerCable != null) {
                    load.PowerCable.Validate(load.PowerCable);
                    load.PowerCable.CreateTypeList(load); 
                }
            }
            else if (_selectedLoadCable is (DistributionEquipment)) {
                var dteq = DteqFactory.Recast(_selectedLoadCable);
                if (dteq.PowerCable != null) {
                    dteq.PowerCable.Validate(dteq.PowerCable);
                    dteq.PowerCable.CreateTypeList(dteq); 
                }
            }
            else if (_selectedLoadCable is ComponentModelBase) {
                var component = (ComponentModelBase)_selectedLoadCable;
                if (component.PowerCable != null) {
                    component.PowerCable.Validate(component.PowerCable);
                    component.PowerCable.CreateTypeList((LoadModel)component.Owner); 
                }
            }

        }
    }


    //SelectedItems
    public IDteq SelectedDteq
    {
        get { return _selectedDteq; }
        set
        {
            if (value == null) {
                return;
            }
            AssignedLoads = new ObservableCollection<IPowerConsumer>();
            IsBusy = true; //for loading animations on single line

            //Assigns dteq loads to AssignedLoads property
            System.Windows.Application.Current.Dispatcher.Invoke(() => {

                //used for fedfrom Validation
                _selectedDteq = value;
                _selectedDteq.Validate();
                SelectedEquipment = _selectedDteq;
                SelectedLoadEquipment = _selectedDteq;
                SelectedLoadCable = _selectedDteq;
                if (_selectedDteq != null) {
                    //AssignedLoads = new ObservableCollection<IPowerConsumer>(_selectedDteq.AssignedLoads);
                    
                    foreach (var item in _selectedDteq.AssignedLoads) {
                        AssignedLoads.Add(item);
                    }

                    GlobalConfig.SelectingNew = true;
                    GlobalConfig.SelectingNew = false;
                    if (AssignedLoads.Count > 0) {
                        SelectedLoad = AssignedLoads[0];

                    }
                }

            }, DispatcherPriority.Background,null);

            //hide loading animation;
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() => IsBusy = false, DispatcherPriority.ContextIdle, null);

        }

    }
    private IDteq _selectedDteq;

    public bool IsBusy { get; set; }


    public IPowerConsumer SelectedMoveableEquipment
    {
        get { return _selectedMoveableEquipment; }
        set { _selectedMoveableEquipment = value; }
    }
    private IPowerConsumer _selectedMoveableEquipment;

    public IPowerConsumer SelectedLoad
    {
        get { return _selectedLoad; }
        set 
        { 

            _selectedLoad = value;
            base.SelectedLoad = _selectedLoad;
            SelectedLoadEquipment = _selectedLoad;
            SelectedLoadCable = _selectedLoad;


            if (value == null) return;
            if (_selectedLoad.CctComponents.Count > 0) {
                SelectedComponent = (ComponentModelBase)_selectedLoad.CctComponents[0];

            }
        }
    }
    private IPowerConsumer _selectedLoad;


    //Components
    private ComponentModelBase _selectedComponent;
    public ComponentModelBase SelectedComponent
    {
        get { return _selectedComponent; }
        set { _selectedComponent = value; }
    }

    public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> { };

    public static PopupNotifcationWindow NotificationPopup { get; set; }


    public ICommand DeleteLoadCommand { get; }


    public ICommand MoveLoadUpCommand { get; }
    public void MoveLoadUp()
    {
        SelectedDteq.MoveLoadUp(SelectedMoveableEquipment);
        AssignedLoads.Clear();
        foreach (var load in SelectedDteq.AssignedLoads) {
            AssignedLoads.Add(load);
        }
    }

    public ICommand MoveLoadDownCommand { get; }
    public void MoveLoadDown()
    {
        SelectedDteq.MoveLoadDown(SelectedMoveableEquipment);
        AssignedLoads.Clear();
        foreach (var load in SelectedDteq.AssignedLoads) {
            AssignedLoads.Add(load);
        }
    }


    #region Autocad

    public ICommand DrawSingleLineAcadCommand { get; }
    public void DrawSingleLineRelay()
    {
        DrawSingleLineAsycn();
    }
    
    public void DrawSingleLineAsycn()
    {
        AutocadService.CreateSingleLine(SelectedDteq);
    }
    #endregion

}
