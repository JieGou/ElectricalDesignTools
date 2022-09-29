using AutoCAD;
using AutocadLibrary;
using EdtLibrary.Commands;
using EDTLibrary;
using EDTLibrary.Autocad.Interop;
using EDTLibrary.Managers;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using Syncfusion.Windows.Controls.PivotGrid;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using WpfUI.Helpers;
using WpfUI.PopupWindows;
using WpfUI.Services;
using WpfUI.Stores;

namespace WpfUI.ViewModels.Electrical;

[AddINotifyPropertyChangedInterface]
internal class DpanelViewModel : ViewModelBase
{

    private DteqFactory _dteqFactory;
    private ListManager _listManager;

    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }
    public LoadToAddValidator LoadToAddValidator { get; set; }


    public SolidColorBrush SingleLineViewBackground { get; set; } = new SolidColorBrush(Colors.LightCyan);



    //CTOR
    public DpanelViewModel(ListManager listManager)
    {
        ListManager = listManager;
        LoadToAddValidator = new LoadToAddValidator(listManager);

        AddLoadCommand = new RelayCommand(AddLoad);

        MoveUpLeftCommand = new RelayCommand(MoveUpLeft);
        MoveDownLeftCommand = new RelayCommand(MoveDownLeft);
        DeleteLoadLeftCommand = new RelayCommand(DeleteLoadLeft);



        MoveUpRightCommand = new RelayCommand(MoveUpRight);
        MoveDownRightCommand = new RelayCommand(MoveDownRight);
        DeleteLoadRightCommand = new RelayCommand(DeleteLoadRight);

        DrawSingleLineAcadCommand = new RelayCommand(DrawSingleLineRelay);

    }


    public IDpn SelectedDpnl
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
        }
    }
    private IDpn _selectedDpnl;

    public ObservableCollection<IDpn> ViewableDteqList

    {
        get
        {
            List<IDpn> subList = new List<IDpn>();
            var dteqSubList = _listManager.IDteqList.Where(d => d.Type == DteqTypes.DPN.ToString() || d.Type == DteqTypes.CDP.ToString()).ToList();

            foreach (var dteq in dteqSubList) {
                subList.Add((IDpn)dteq);
            }
            return new ObservableCollection<IDpn>(subList);
        }
    }


    public IPowerConsumer SelectedLoadLeft
    {
        get { return _selectedLeftLoad; }
        set {
            if (value == null) return;
            _selectedLeftLoad = value;

            var selectedCircuits = new ObservableCollection<IPowerConsumer>();

            IPowerConsumer load;
            if (SelectedDpnl.LeftCircuits.FirstOrDefault(ld => ld == _selectedLeftLoad) != null) {
                SelectedCircuitList = SelectedDpnl.LeftCircuits;
            }
            if (SelectedDpnl.RightCircuits.FirstOrDefault(ld => ld == _selectedLeftLoad) != null) {
                SelectedCircuitList = SelectedDpnl.RightCircuits;
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
            if (SelectedDpnl.LeftCircuits.FirstOrDefault(ld => ld == _selectedRightLoad) != null) {
                SelectedCircuitList = SelectedDpnl.LeftCircuits;
            }
            if (SelectedDpnl.RightCircuits.FirstOrDefault(ld => ld == _selectedRightLoad) != null) {
                SelectedCircuitList = SelectedDpnl.RightCircuits;
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
            ErrorHelper.ShowErrorMessage(ex);
        }
    }



    //Todo - move logic to DpanelCctManager
    public ICommand MoveUpLeftCommand { get; }
    public void MoveUpLeft()
    {
        if (SelectedLoadLeft == null) return;

        int loadIndex;
        for (int i = 0; i < SelectedDpnl.LeftCircuits.Count; i++) {
            if (SelectedLoadLeft == SelectedDpnl.LeftCircuits[i]) {
                loadIndex = Math.Max(0, i - 1);
                SelectedDpnl.LeftCircuits.Move(i, loadIndex);
                break;
            }
        }
        for (int i = 0; i < SelectedDpnl.LeftCircuits.Count; i++) {
            SelectedDpnl.LeftCircuits[i].SequenceNumber = i;
        }
        SelectedDpnl.LeftCircuits.OrderBy(c => c.SequenceNumber);
        SelectedDpnl.CalculateLoading();
    }

    public ICommand MoveDownLeftCommand { get; }
    public void MoveDownLeft()
    {
        if (SelectedLoadLeft == null) return;

        int loadIndex;
        for (int i = 0; i < SelectedDpnl.LeftCircuits.Count; i++) {
            if (SelectedLoadLeft == SelectedDpnl.LeftCircuits[i]) {
                loadIndex = Math.Min(i + 1, SelectedDpnl.LeftCircuits.Count - 1);
                SelectedDpnl.LeftCircuits.Move(i, loadIndex);
                break;
            }
        }
        for (int i = 0; i < SelectedDpnl.LeftCircuits.Count; i++) {
            SelectedDpnl.LeftCircuits[i].SequenceNumber = i;
        }
        SelectedDpnl.LeftCircuits.OrderBy(c => c.SequenceNumber);
        SelectedDpnl.CalculateLoading();

    }

    public ICommand DeleteLoadLeftCommand { get; }
    public void DeleteLoadLeft()
    {
        if (SelectedLoadLeft == null) return;

        int loadIndex;
        DpnCircuitManager.DeleteLoad(SelectedDpnl, SelectedLoadLeft, ScenarioManager.ListManager);
    }



    public ICommand MoveUpRightCommand { get; }
    public void MoveUpRight()
    {
        if (SelectedLoadRight == null) return;

        int loadIndex;
        for (int i = 0; i < SelectedDpnl.RightCircuits.Count; i++) {
            if (SelectedLoadRight == SelectedDpnl.RightCircuits[i]) {
                loadIndex = Math.Max(0, i - 1);
                SelectedDpnl.RightCircuits.Move(i, loadIndex);
                break;
            }
        }
        for (int i = 0; i < SelectedDpnl.RightCircuits.Count; i++) {
            SelectedDpnl.RightCircuits[i].SequenceNumber = i;
        }
        SelectedDpnl.RightCircuits.OrderBy(c => c.SequenceNumber);
        SelectedDpnl.CalculateLoading();

    }

    public ICommand MoveDownRightCommand { get; }
    public void MoveDownRight()
    {
        if (SelectedLoadRight == null) return;

        int loadIndex;
        for (int i = 0; i < SelectedDpnl.RightCircuits.Count; i++) {
            if (SelectedLoadRight == SelectedDpnl.RightCircuits[i]) {
                loadIndex = Math.Min(i + 1, SelectedDpnl.RightCircuits.Count - 1);
                SelectedDpnl.RightCircuits.Move(i, loadIndex);
                break;
            }
        }
        for (int i = 0; i < SelectedDpnl.RightCircuits.Count; i++) {
            SelectedDpnl.RightCircuits[i].SequenceNumber = i;
        }
        SelectedDpnl.RightCircuits.OrderBy(c => c.SequenceNumber);
        SelectedDpnl.CalculateLoading();

    }

    public ICommand DeleteLoadRightCommand { get; }
    public void DeleteLoadRight()
    {
        if (SelectedLoadLeft == null) return;

        int loadIndex;
        DpnCircuitManager.DeleteLoad(SelectedDpnl, SelectedLoadLeft, ScenarioManager.ListManager);
    }



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
        acadService.CreateSingleLine(SelectedDpnl);
    }


    #endregion

}
