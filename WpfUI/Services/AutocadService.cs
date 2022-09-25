using AutoCAD;
using AutocadLibrary;
using EDTLibrary.Autocad.Interop;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Services;
using Syncfusion.Windows.Controls.PivotGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Threading;
using Windows.UI.Notifications;
using WpfUI.Helpers;
using WpfUI.PopupWindows;
using static EDTLibrary.Services.EdtNotificationService;
using Notification = WpfUI.PopupWindows.Notification;

namespace WpfUI.Services;
public class AutocadService
{
   
    public AutocadHelper Acad { get; set; }
    public static NotificationPopup NotificationPopup { get; set; }

    private void ShowNotification(string notification = "")
    {
        NotificationPopup = new NotificationPopup();
        NotificationPopup.DataContext = new Notification(notification);
        NotificationPopup.Show();
    }

    private void CloseNotification()
    {
        if (NotificationPopup!= null) {
            NotificationPopup.Close();
            NotificationPopup = null;
        }
    }

    public async Task StartAutocadAsync()
    {
        try {
            Acad = new AutocadHelper();

            //EdtNotificationService.ShowNotification(this, notification);

            await Task.Run(() => {
                Acad.StartAutocad();
            });

        }
        catch (Exception ex) {

            ErrorHelper.ShowErrorMessage(ex);
        }
        finally {

            
        }
    }
    public async Task DrawSingleLine(IDteq dteq, bool newDrawing = true)
    {
        try {
            if (dteq == null) return;

            ShowNotification("Starting Autocad");
            await StartAutocadAsync();
            CloseNotification();

            if (newDrawing == true) {
                Acad.AddDrawing();
            }

            ShowNotification($"Creating Drawing for {dteq.Tag}");

            SingleLineDrawer slDrawer = new SingleLineDrawer(Acad, EdtSettings.AcadBlockFolder);


            await Task.Run(() => {
                slDrawer.DrawMccSingleLine(dteq, 1.5);
                Acad.AcadApp.ZoomExtents();
            });
            CloseNotification();


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
        finally {
            CloseNotification();
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
