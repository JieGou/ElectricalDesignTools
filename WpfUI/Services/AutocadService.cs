using AutoCAD;
using AutocadLibrary;
using EDTLibrary.Autocad.Interop;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WpfUI.Helpers;
using WpfUI.PopupWindows;

namespace WpfUI.Services;
public class AutocadService
{
   

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
    public void DrawSingleLine(IDteq dteq, bool newDrawing = true)
    {
        try {

            StartAutocad();

            if (newDrawing == true) {
                Acad.AddDrawing();
            }

            SingleLineDrawer slDrawer = new SingleLineDrawer(Acad, EdtSettings.AcadBlockFolder);



            if (dteq == null) return;
            slDrawer.DrawMccSingleLine(dteq, 1.5);
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
                if (Acad.AcadDoc != null) {
                    DeleteDrawingContents();
                }
                DrawSingleLine(dteq, false);
            }
            else {
                ErrorHelper.ShowErrorMessage(ex);
            }
        }
    }

    private void DeleteDrawingContents()
    {
        int _maxAttempts = 10;
        int _attempts = 0;
        try {
            AcadSelectionSet sSet = Acad.AcadDoc.SelectionSets.Add("sSetAll");
            sSet.Select(AcSelect.acSelectionSetAll);
            foreach (AcadEntity item in sSet) {
                item.Delete();
            }
            sSet.Clear(); //clear selection sets in case this error happens more than once
        }
        catch (Exception ex) {
            if (ex.Message.Contains("rejected") && _attempts <= _maxAttempts) {
                _attempts++;
                DeleteDrawingContents();
            }
            else {
                throw;
            }
        }
    }
}
