using AutoCAD;
using AutocadLibrary;
using EdtLibrary.Commands;
using EDTLibrary;
using EDTLibrary.Autocad.Interop;
using EDTLibrary.Managers;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfUI.Helpers;
using WpfUI.PopupWindows;
using WpfUI.Services;
using WpfUI.Stores;

namespace WpfUI.ViewModels.Electrical;
internal class SingleLineViewModel: ViewModelBase
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


    public ObservableCollection<IDteq> ViewableDteqList

    {
        get
        {
            List<IDteq> subList = new List<IDteq>();
            subList = _listManager.IDteqList.Where(d => 
                d.Type == DteqTypes.MCC.ToString() || 
                d.Type == DteqTypes.CDP.ToString() ||
                d.Type== DteqTypes.DPN.ToString()
                ).ToList();
            return new ObservableCollection<IDteq>(subList);
        }
    }


    public SingleLineViewModel(ListManager listManager)
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

            //used for fedfrom Validation
            _selectedDteq = value;

            if (_selectedDteq != null) {
                AssignedLoads = new ObservableCollection<IPowerConsumer>(_selectedDteq.AssignedLoads);

                GlobalConfig.SelectingNew = true;
                GlobalConfig.SelectingNew = false;
                SelectedLoad = AssignedLoads[0];
            }
        }
    }
    private IPowerConsumer _selectedLoad;

    public IPowerConsumer SelectedLoad
    {
        get { return _selectedLoad; }
        set 
        { 
            _selectedLoad = value; 

        }
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
