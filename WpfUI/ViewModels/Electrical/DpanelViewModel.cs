using EdtLibrary.Commands;
using EDTLibrary;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.DPanels;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using WpfUI.Helpers;
using WpfUI.Services;

namespace WpfUI.ViewModels.Electrical;

[AddINotifyPropertyChangedInterface]
internal class DpanelViewModel : ElectricalViewModelBase
{

    private DteqFactory _dteqFactory;
    private ListManager _listManager;

    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }
    public LoadToAddValidator LoadToAddValidator { get; set; }



    //CTOR
    public DpanelViewModel(ListManager listManager) : base(listManager)
    {
        ListManager = listManager;
        LoadToAddValidator = new LoadToAddValidator(listManager);

        _ViewStateManager.ElectricalViewUpdate += OnElectricalViewUpdated;


        AddLoadCommand = new RelayCommand(AddLoad);

        MoveUpLeftCommand = new RelayCommand(MoveUpLeft);
        MoveDownLeftCommand = new RelayCommand(MoveDownLeft);






        MoveUpCommand = new RelayCommand(MoveUp);
        MoveDownCommand = new RelayCommand(MoveDown);
        MoveLeftCommand = new RelayCommand(MoveLeft);
        MoveRightCommand = new RelayCommand(MoveRight);

        ConvertToLoadCommand = new RelayCommand(ConvertToLoad);
        DeleteLoadCommand = new RelayCommand(DeleteLoad);


        DrawPanelScheduleAcadCommand = new RelayCommand(DrawPanelScheduleRelay);

    }

    #region ViewState
    public void OnElectricalViewUpdated(object source, EventArgs e)
    {
        UpdatePanelList();
        RefreshSelectedPanel();
    }
    public void UpdatePanelList()
    {

        List<IDpn> subList = new List<IDpn>();
        var dteqSubList = _listManager.IDteqList.Where(d => d.Type == DteqTypes.DPN.ToString() || d.Type == DteqTypes.CDP.ToString()).ToList();

        foreach (var dteq in dteqSubList) {
            subList.Add((IDpn)dteq);
        }
        ViewableDteqList = new ObservableCollection<IDpn>(subList);

    }
    public void RefreshSelectedPanel()
    {
        if (SelectedDteq == null) return;

        var dteq = ListManager.DteqList.FirstOrDefault(d => d.Tag == SelectedDteq.Tag);

        if (dteq == null) return;

        SelectedDteq = (IDpn)dteq;
    }

    #endregion

    public IDpn SelectedDteq
    {
        get { return _selectedDpnl; }
        set
        {
            if (value == null) return;

            //used for fedfrom Validation
            _selectedDpnl = value;
            LoadToAddValidator.FedFromTag = _selectedDpnl.Tag;
            if (_selectedDpnl != null) {
                //AssignedLoads = new ObservableCollection<IPowerConsumer>(_selectedDpnl.AssignedLoads);

                GlobalConfig.SelectingNew = true;
                GlobalConfig.SelectingNew = false;
                var dpn = (DpnModel)_selectedDpnl;
                dpn.SetCircuits();

            }
            SelectedEquipment = _selectedDpnl;
        }
    }
    private IDpn _selectedDpnl;


    public ObservableCollection<IDpn> ViewableDteqList

    {
        get
        {
            return _viewableDteqList;
        }
        set 
        {
            _viewableDteqList = value;
        }

        
    }

   
    private ObservableCollection<IDpn> _viewableDteqList;

    public override IPowerConsumer SelectedLoad
    {
        get { return _selectedLoad; }
        set
        {
            if (value == null) return;
            _selectedLoad = value;
            SelectedEquipment = _selectedLoad;
        }
    }
    private IPowerConsumer _selectedLoad;
    public IPowerConsumer SelectedLoadLeft
    {
        get { return _selectedLeftLoad; }
        set {
            if (value == null) return;
            _selectedLeftLoad = value;

            var selectedCircuits = new ObservableCollection<IPowerConsumer>();

            IPowerConsumer load;
            if (SelectedDteq.LeftCircuits.FirstOrDefault(ld => ld == _selectedLeftLoad) != null) {
                SelectedCircuitList = SelectedDteq.LeftCircuits;
            }
            if (SelectedDteq.RightCircuits.FirstOrDefault(ld => ld == _selectedLeftLoad) != null) {
                SelectedCircuitList = SelectedDteq.RightCircuits;
            }


        }
    }
    private IPowerConsumer _selectedLeftLoad;

    public IPowerConsumer SelectedLoadRight
    {
        get { return _selectedRightLoad; }
        set
        {
            if (value == null) return;
            _selectedRightLoad = value;

            var selectedCircuits = new ObservableCollection<IPowerConsumer>();

            IPowerConsumer load;
            if (SelectedDteq.LeftCircuits.FirstOrDefault(ld => ld == _selectedRightLoad) != null) {
                SelectedCircuitList = SelectedDteq.LeftCircuits;
            }
            if (SelectedDteq.RightCircuits.FirstOrDefault(ld => ld == _selectedRightLoad) != null) {
                SelectedCircuitList = SelectedDteq.RightCircuits;
            }

        }
    }
    private IPowerConsumer _selectedRightLoad;

    public ObservableCollection<IPowerConsumer> SelectedCircuitList { get; set; } = new ObservableCollection<IPowerConsumer>();
    //public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> ();

    
    public ICommand AddLoadCommand { get; }

    public void AddLoad(object loadToAddObject)
    {
        AddLoadAsync(loadToAddObject);
    }

    public async Task AddLoadAsync(object loadToAddObject)
    {
        try {
            LoadModel newLoad = await LoadManager.AddLoad(loadToAddObject, _listManager);
            LoadToAddValidator.ResetTag();
        }
        catch (Exception ex) {
            NotificationHandler.ShowErrorMessage(ex);
        }
    }







    //Todo - move logic to DpanelCctManager
    public ICommand MoveUpLeftCommand { get; }
    public void MoveUpLeft()
    {
        DpnCircuitManager.MoveCircuitUp(SelectedDteq, SelectedLoad);
    }

    public ICommand MoveDownLeftCommand { get; }
    public void MoveDownLeft()
    {
        DpnCircuitManager.MoveCircuitDown(SelectedDteq, SelectedLoad);
    }

    



    public ICommand MoveUpCommand { get; }
    public void MoveUp()
    {
        DpnCircuitManager.MoveCircuitUp(SelectedDteq, SelectedLoad);
    }

    public ICommand MoveDownCommand { get; }
    public void MoveDown()
    {
        DpnCircuitManager.MoveCircuitDown(SelectedDteq, SelectedLoad);
    }

    public ICommand MoveLeftCommand { get; }
    public void MoveLeft()
    {
        DpnCircuitManager.MoveCircuitLeft(SelectedDteq, SelectedLoad);
    }

    public ICommand MoveRightCommand { get; }
    public void MoveRight()
    {
        DpnCircuitManager.MoveCircuitRight(SelectedDteq, SelectedLoad);
    }



    public ICommand ConvertToLoadCommand { get; }
    public void ConvertToLoad()
    {

        if (SelectedLoad.GetType() == typeof(LoadCircuit)) {
            var loadCircuit = (LoadCircuit)SelectedLoad;
            DpnCircuitManager.ConvertToLoad(loadCircuit);
        }

    }

    public ICommand DeleteLoadCommand { get; }

    //public void DeleteLoad()
    //{
    //    DpnCircuitManager.DeleteLoad(SelectedDteq, SelectedLoad, ScenarioManager.ListManager);
    //}

    #region Autocad

    public ICommand DrawPanelScheduleAcadCommand { get; }
    public void DrawPanelScheduleRelay()
    {
        AutocadService.CreatePanelSchedule(SelectedDteq);
    }

    #endregion

}
