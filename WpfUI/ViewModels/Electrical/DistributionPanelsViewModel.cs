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
using WpfUI.Stores;

namespace WpfUI.ViewModels.Electrical;

[AddINotifyPropertyChangedInterface]
internal class DistributionPanelsViewModel: ViewModelBase
{

    private DteqFactory _dteqFactory;
    private ListManager _listManager;

    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }


    public SolidColorBrush SingleLineViewBackground { get; set; } = new SolidColorBrush(Colors.LightCyan);



    //CTOR
    public DistributionPanelsViewModel(ListManager listManager)
    {
        ListManager = listManager;

        AddLoadToPanelCommand = new RelayCommand(AddPanelLoad);
        MoveUpCommand = new RelayCommand(MoveUp);
        MoveDownCommand = new RelayCommand(MoveDown);

    }

    
    public IDpn SelectedDpnl
    {
        get { return _selectedDpnl; }
        set
        {
            if (value == null) return;

            //used for fedfrom Validation
            _selectedDpnl = value;

            if (_selectedDpnl != null) {
                AssignedLoads = new ObservableCollection<IPowerConsumer>(_selectedDpnl.AssignedLoads);

                GlobalConfig.SelectingNew = true;
                GlobalConfig.SelectingNew = false;
                var dpn = (DpnModel)_selectedDpnl;
                dpn.SetLeftCircuits();
                dpn.SetRightCircuits();

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
                subList.Add((IDpn) dteq);
            }
            return new ObservableCollection<IDpn>(subList);
        }
    }

    public IPowerConsumer SelectedLoad
    {
        get { return _selectedLoad; }
        set {
            if (value == null) return;
            _selectedLoad = value; 

            var selectedCircuits = new ObservableCollection<IPowerConsumer>();

            IPowerConsumer load;
            if (SelectedDpnl.LeftCircuits.FirstOrDefault(ld => ld == _selectedLoad) != null) {
                SelectedCircuitList = SelectedDpnl.LeftCircuits;
            }
            if (SelectedDpnl.RightCircuits.FirstOrDefault(ld => ld == _selectedLoad) != null) {
                SelectedCircuitList = SelectedDpnl.RightCircuits;
            }


        }
    }
    private IPowerConsumer _selectedLoad;

    public ObservableCollection<IPowerConsumer> SelectedCircuitList { get; set; } = new ObservableCollection<IPowerConsumer>();
    public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> ();

    public ICommand AddLoadToPanelCommand { get; }
    private void AddPanelLoad()
    {
        if (SelectedDpnl == null || SelectedLoad == null ) {
            MessageBox.Show("Select a Panel and a Load.", "Selection Required");
            return;
        }
        var dpn = (DpnModel)SelectedDpnl;
        DpnCircuitManager.AddLoad(dpn, SelectedLoad, ListManager);
    }




    public ICommand MoveUpCommand { get; }

    public void MoveUp()
    {
        int loadIndex;
        if (SelectedLoad == null) return;

        for (int i = 0; i < SelectedCircuitList.Count; i++) {
            if (SelectedLoad == SelectedCircuitList[i]) {
                loadIndex = Math.Max(0, i - 1);
                SelectedCircuitList.Move(i, loadIndex);
                break;
            }
        }
        for (int i = 0; i < SelectedCircuitList.Count; i++) {
            SelectedCircuitList[i].SequenceNumber = i;
        }
        SelectedCircuitList.OrderBy(c => c.SequenceNumber);
    }
    public ICommand MoveDownCommand { get; }

    public void MoveDown()
    {
        int loadIndex;
        if (SelectedLoad == null) return;

        for (int i = 0; i < SelectedCircuitList.Count; i++) {
            if (SelectedLoad == SelectedCircuitList[i]) {
                loadIndex = Math.Min(i + 1, SelectedCircuitList.Count - 1);
                SelectedCircuitList.Move(i, loadIndex);
                break;
            }
        }
        for (int i = 0; i < SelectedCircuitList.Count; i++) {
            SelectedCircuitList[i].SequenceNumber = i;
        }
        SelectedCircuitList.OrderBy(c => c.SequenceNumber);
    }







    #region Autocad
    public AutocadHelper Acad { get; set; }
    public NotificationPopup NotificationPopup { get; set; }

    public void StartAutocad()
    {
        try {
            Acad = new AutocadHelper();
            NotificationPopup = new NotificationPopup();
            NotificationPopup.DataContext = new Notification("Starting Autocad");
            NotificationPopup.Show();
            Acad.StartAutocad();
            NotificationPopup.Close();

        }
        catch (Exception ex) {

            ErrorHelper.ShowErrorMessage(ex);
        }
        finally {
            NotificationPopup.Close();
        }
    }

   
    public ICommand AddAcadDrawingCommand { get; }
    public void AddDrawing()
    {
        if (Acad == null) {
            StartAutocad();
        }

        try {
            Acad.AddDrawing();
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }

    public ICommand DrawSingleLineAcadCommand { get; }
    public void DrawSingleLineRelay()
    {
        DrawSingleLine();
    }
    public void DrawSingleLine(bool newDrawing = true)
    {
        if (Acad == null) {
            StartAutocad();
        }

        //if (Acad.AcadDoc == null) {
        //    Acad.AddDrawing();
        //}

        if (newDrawing == true) {
            Acad.AddDrawing();
        }

        try {
            SingleLineDrawer slDrawer = new SingleLineDrawer(Acad, EdtSettings.AcadBlockFolder);

            IDteq mcc = SelectedDpnl;

            if (mcc == null) return;
            slDrawer.DrawMccSingleLine(mcc, 1.5);
            Acad.AcadApp.ZoomExtents();
        }

        catch (Exception ex) {

            if (ex.Message.Contains("not found")) {
                MessageBox.Show(
                    "Check the Blocks Source Folder path and make sure that the selected blocks exist.",
                    "Error - File Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else if (ex.Message.Contains("rejected")) {
                if (Acad.AcadDoc!= null) {
                    AcadSelectionSet sSet = Acad.AcadDoc.SelectionSets.Add("sSetAll");
                    sSet.Select(AcSelect.acSelectionSetAll);
                    foreach (AcadEntity item in sSet) {
                        item.Delete();
                    }
                }
                DrawSingleLine(false);
            }
            else {
                ErrorHelper.ShowErrorMessage(ex);
            }
        }
    }


    #endregion

}
