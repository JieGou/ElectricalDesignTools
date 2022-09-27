using AutoCAD;
using AutocadLibrary;
using EDTLibrary.Autocad.Interop;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Services;
using Syncfusion.ProjIO;
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
using Task = System.Threading.Tasks.Task;

namespace WpfUI.Services;
public class AutocadService
{

    public static AutocadHelper _acad;
    private int _attempts;
    private readonly int _maxAttemps = 10;


    #region Tasks
    private static List<Task> _tasks = new List<Task>();
    private static bool _isRunningTasks;
    private async Task ScheduleTask(Task task)
    {
        _tasks.Add(task);
        if (_isRunningTasks == false) {
            RunTasks();
        }
    }
    private async Task RunTasks()
    {
        if (_isRunningTasks) return;

        _isRunningTasks = true;
        List<Task> tasksToRun = new List<Task>(); 
        //Copy tasks
        foreach (var item in _tasks) {
            tasksToRun.Add(item);
        }
        _tasks.Clear();

        //Run tasks;
        foreach (var item in tasksToRun) {
            await Task.Run(() => item);
        }
        //check for new tasks
        if (_tasks.Count!=0) {
            RunTasks();
        }
        _isRunningTasks = false;

    }

    #endregion

    #region Action

    private static List<Action> _actions = new List<Action>();
    private static bool _isRunningActions;

    private void ScheduleAction(Action action)
    {
        _actions.Add(action);
        if (_isRunningActions == false) {
            RunActions();
        }

    }

    private async Task RunActions()
    {
        _isRunningActions = true;

        List<Action> actionsToRun = new List<Action>();
        //Copy tasks
        foreach (var item in _actions) {
            actionsToRun.Add(item);
        }
        _actions.Clear();

        foreach (var item in actionsToRun) {
            item();
        }

        if (_actions.Count != 0) RunActions();
        _isRunningActions = false;

    }
    #endregion
    private async Task StartAutocadAsync()
    {
        try {
            _acad = new AutocadHelper();

            

            await Task.Run(() => {
                _acad.StartAutocad();
            });

        }
        catch (Exception ex) {

            ErrorHelper.ShowErrorMessage(ex);
        }
        finally {

            
        }
    }
    public async Task CreateSingleLine(IDteq dteq)
    {
        if (_isRunningTasks) {
            EdtNotificationService.ShowNotification(this, $"Autocad is busy");
        }
        else {
            await DrawSingleLineAsync(dteq);

            //if (dteq == typeof(MccModel)) {

            //}
            //else if(dteq == typeof(DpnModel)) {
            //    await DrawPanelScheduleAsync(dteq);
            //}
        }
    }

    private void DrawSingleLine(IDteq dteq, bool newDrawing = true)
    {
        try {
            if (dteq == null) return;


            EdtNotificationService.ShowNotification(this, "Starting Autocad");
            StartAutocadAsync();

            if (newDrawing == true) {
                _acad.AddDrawing();
            }

            EdtNotificationService.ShowNotification(this, $"Creating drawing for {dteq.Tag}");
            
            SingleLineDrawer slDrawer = new SingleLineDrawer(_acad, EdtSettings.AcadBlockFolder);
            slDrawer.DrawMccSingleLine(dteq, 1.5);
            _acad.AcadApp.ZoomExtents();
            
            EdtNotificationService.CloseNotification(this);
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
                if (_acad.AcadDoc != null) {
                    DeleteDrawingContents();
                }
                DrawSingleLineAsync(dteq, false);
            }
            else if (ex.Message.Contains("busy")) {
                if (_acad.AcadDoc != null) {
                    DeleteDrawingContents();
                }
                DrawSingleLineAsync(dteq, false);
            }
            else {
                ErrorHelper.ShowErrorMessage(ex);
            }
        }
        finally {
            EdtNotificationService.CloseNotification(this);
        }
    }

    public async Task DrawSingleLineAsync(IDteq dteq, bool newDrawing = true)
    {
        try {
            _isRunningTasks = true;
            if (dteq == null) return;


            EdtNotificationService.ShowNotification(this, $"Starting Autocad");
            await StartAutocadAsync();
            EdtNotificationService.CloseNotification(this);

            if (newDrawing == true || _acad.AcadDoc == null) {
                _acad.AddDrawing();
            }

            EdtNotificationService.ShowNotification(this, $"Creating drawing for {dteq.Tag}");

            await Task.Run(() => {
                SingleLineDrawer slDrawer = new SingleLineDrawer(_acad, EdtSettings.AcadBlockFolder);
                slDrawer.DrawMccSingleLine(dteq, 1.5);
                _acad.AcadApp.ZoomExtents();
            });

            _isRunningTasks = false;
            EdtNotificationService.CloseNotification(this);

        }

        catch (Exception ex) {

            if (ex.Message.Contains("not found")) {
                MessageBox.Show(
                    "Check the Blocks Source Folder path and make sure that the selected blocks exist.",
                    "Error - File Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            else if (ex.Message.Contains("rejected")) { //erase partial drawing and retry
                if (_acad.AcadDoc != null) {
                    DeleteDrawingContents();
                }
                DrawSingleLineAsync(dteq, false);
            }
            else if (ex.Message.Contains("busy")) {
                Task.Delay(500); // wait for acad to not be busy
                if (_acad.AcadDoc != null) {
                    DeleteDrawingContents();
                }
                DrawSingleLineAsync(dteq, false);
            }
            else if (ex.Message.Contains("instance")) {
                if (_acad.AcadDoc != null) {
                    DeleteDrawingContents();
                    DrawSingleLineAsync(dteq, false);
                }
            }
            else {
                ErrorHelper.ShowErrorMessage(ex);
            }
        }
        finally {
            _isRunningTasks = false;
            EdtNotificationService.CloseNotification(this);
        }
    }

    private void DeleteDrawingContents()
    {
        int _maxDeleteAttempts = 10;
        int _deleteAttempts = 0;
        try {
            AcadSelectionSet sSet = _acad.AcadDoc.SelectionSets.Add("sSetAll");
            sSet.Select(AcSelect.acSelectionSetAll);
            foreach (AcadEntity item in sSet) {
                item.Delete();
            }
            sSet.Clear(); //clear selection sets in case this error happens more than once
        }
        catch (Exception ex) {
            if (ex.Message.Contains("rejected") && _deleteAttempts <= _maxDeleteAttempts) {
                _deleteAttempts++;
                DeleteDrawingContents();
            }
            else {
                throw;
            }
        }
        finally {
            EdtNotificationService.CloseNotification(this);
            _attempts = 0;
            _deleteAttempts = 0;
        }
    }
}
