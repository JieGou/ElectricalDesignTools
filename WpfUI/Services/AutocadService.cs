using AutocadLibrary;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using EDTLibrary.Autocad.Interop;
using EDTLibrary.AutocadInterop.Interop;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Services;
using EDTLibrary.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using WpfUI.Helpers;
using Task = System.Threading.Tasks.Task;

namespace WpfUI.Services;

public delegate Task DrawCommandDelegate(IDteq dteq, bool newDrawing);
public class AutocadService
{

    public static AutocadHelper _acadHelper;
    public static string drawingName;
    private int _attempts;
    private readonly int _maxAttemps = 10;

    public ISingleLineDrawer SingleLineDrawer { get; set; } = new SingleLineDrawer_EdtV1(_acadHelper);
    public IPanelScheduleDrawer PanelScheduleDrawer { get; set; } = new PanelScheduleDrawer_EdtV1(_acadHelper);

    DrawCommandDelegate drawCommand;

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


    #region Actions

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

            //for persisting the Document Name

            //if (_acadHelper == null) {
            //    _acadHelper = new AutocadHelper();
            //}

            _acadHelper = new AutocadHelper();

            await Task.Run(() => {
                _acadHelper.StartAutocad();
            });

        }
        catch (Exception ex) {

            NotificationHandler.ShowErrorMessage(ex);
        }
        finally {

            
        }
    }
   


    public async Task CreateSingleLine(IDteq dteq)
    {
        drawCommand = new DrawCommandDelegate(DrawSingleLineAsync);

        if (_isRunningTasks) {
            EdtNotificationService.SendPopupNotification(this, $"Autocad is busy");
        }
        else {
            await DrawSingleLineAsync(dteq);
        }
    }
    public async Task DrawSingleLineAsync(IDteq dteq, bool newDrawing = true)
    {
        try {
            _isRunningTasks = true;
            if (dteq == null) return;


            EdtNotificationService.SendPopupNotification(this, $"Starting Autocad");
            await StartAutocadAsync();
            EdtNotificationService.CloseoPupNotification(this);


            _acadHelper.AddDrawing(drawingName);
            drawingName = _acadHelper.DocName;
            EdtNotificationService.SendPopupNotification(this, $"Creating drawing for {dteq.Tag}");

            await Task.Run(() => {
                SingleLineDrawer.AcadHelper = _acadHelper;
                SingleLineDrawer.DrawSingleLine(dteq, 1.5);
                _acadHelper.AcadApp.ZoomExtents();
            });

            drawingName = "";
            _isRunningTasks = false;
            EdtNotificationService.CloseoPupNotification(this);

        }

        catch (Exception ex) {
            Retry(dteq, ex);
        }
        finally {
            _isRunningTasks = false;
            EdtNotificationService.CloseoPupNotification(this);
        }
    }


    public async Task CreatePanelSchedule(IDteq dteq)
    {
        drawCommand = new DrawCommandDelegate(DrawPanelScheduleAsync);

        if (_isRunningTasks) {
            EdtNotificationService.SendPopupNotification(this, $"Autocad is busy");
        }
        else {
            await DrawPanelScheduleAsync(dteq);
        }
    }
    public async Task DrawPanelScheduleAsync(IDteq dteq, bool newDrawing = true)
    {
        try {
            _isRunningTasks = true;
            if (dteq == null) return;


            EdtNotificationService.SendPopupNotification(this, $"Starting Autocad");
            await StartAutocadAsync();
            EdtNotificationService.CloseoPupNotification(this);

            _acadHelper.AddDrawing(drawingName);
            drawingName = _acadHelper.DocName;
            EdtNotificationService.SendPopupNotification(this, $"Creating drawing for {dteq.Tag}");

            await Task.Run(() => {
                PanelScheduleDrawer.AcadHelper = _acadHelper;
                PanelScheduleDrawer.DrawPanelSchedule(dteq, 1.5);
                _acadHelper.AcadApp.ZoomExtents();
            });

            drawingName = "";
            _isRunningTasks = false;
            EdtNotificationService.CloseoPupNotification(this);

        }

        catch (Exception ex) {

            Retry(dteq, ex);

        }

        finally {
            _isRunningTasks = false;
            EdtNotificationService.CloseoPupNotification(this);
        }
    }



    private void Retry(IDteq dteq, Exception ex)
    {
        if (ex.Message.Contains("not found")) {
            MessageBox.Show(
                "Check the Blocks Source Folder path and make sure that the selected blocks exist.",
                "Error - File Not Found",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        else if (ex.Message.Contains("rejected")) { //erase partial drawing and retry
            if (_acadHelper.AcadDoc != null) {
                DeleteDrawingContents();
            }
            drawCommand(dteq, false);
        }

        else if (ex.Message.Contains("busy")) {
            Task.Delay(500); // wait for acad to not be busy
            if (_acadHelper.AcadDoc != null) {
                DeleteDrawingContents();
            }
            drawCommand(dteq, false);
        }

        else if (ex.Message.Contains("instance")) {
            if (_acadHelper.AcadDoc != null) {
                DeleteDrawingContents();
            }
            drawCommand(dteq, false);
        }

        else if (ex.Message.Contains("dispatch")) {
            //NotificationHandler.ShowErrorMessage(ex);
            if (_acadHelper.AcadDoc != null) {
                DeleteDrawingContents();
            }
            drawCommand(dteq, false);
        }
        else {
            NotificationHandler.ShowErrorMessage(ex);
        }
    }
    private void DeleteDrawingContents()
    {
        int _maxDeleteAttempts = 10;
        int _deleteAttempts = 0;
        try {
            dynamic sSet = _acadHelper.AcadDoc.SelectionSets.Add("sSetAll");
            sSet.Select(AcSelect.acSelectionSetAll);
            foreach (dynamic item in sSet) {
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
            EdtNotificationService.CloseoPupNotification(this);
            _attempts = 0;
            _deleteAttempts = 0;
        }
    }
}
