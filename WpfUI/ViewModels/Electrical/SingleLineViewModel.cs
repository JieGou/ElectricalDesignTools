using AutocadLibrary;
using EdtLibrary.Commands;
using EDTLibrary;
using EDTLibrary.Autocad.Interop;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using WpfUI.Helpers;
using WpfUI.PopupWindows;
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

    public SolidColorBrush SingleLineViewBackground { get; set; } = new SolidColorBrush(Colors.White);


    public ObservableCollection<IDteq> ViewableDteqList

    {
        get
        {
            List<IDteq> subList = new List<IDteq>();
            subList = _listManager.IDteqList.Where(d => d.Type == DteqTypes.MCC.ToString()).ToList();
            return new ObservableCollection<IDteq>(subList);
        }
    }


    public SingleLineViewModel(ListManager listManager)
    {
        ListManager = listManager;

        DrawSingleLineAcadCommand = new RelayCommand(DrawSingleLineRelay);
    }


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
                
            }
        }
    }
    public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer> { };



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

            IDteq mcc = SelectedDteq;

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
                DrawSingleLine(false);
            }
            else {
                ErrorHelper.ShowErrorMessage(ex);
            }
        }
    }
    #endregion

}
