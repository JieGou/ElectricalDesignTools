using AutocadLibrary;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using EdtLibrary.AutocadInterop.TitleBlocks;
using EDTLibrary.Autocad.Interop;
using EDTLibrary.AutocadInterop.Interop;
using EDTLibrary.ErrorManagement;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Services;
using EDTLibrary.Settings;
using Syncfusion.CompoundFile.XlsIO.Native;
using Syncfusion.ProjIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Forms;
using WpfUI.Helpers;
using Task = System.Threading.Tasks.Task;

namespace WpfUI.Services;

public delegate void DrawCommandDelegate(IDteq dteq, bool addNewDrawing);
public partial class AutocadService
{

    public static AutocadHelper _acadHelper;
    public static string drawingName;
    private int _attempts;
    private const int DeleteDrawingContents_MaxAttempts = 1000;

    public TitleBlockImporter TitleBlockImporter { get; set; } = new TitleBlockImporter(_acadHelper);
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


    private void StartAutocadAsync()
    {
        try {

            //for persisting the Document Name

            //if (_acadHelper == null) {
            //    _acadHelper = new AutocadHelper();
            //}

            _acadHelper = new AutocadHelper();

            _acadHelper.StartAutocad();

            //await Task.Run(() => {
            //    _acadHelper.StartAutocad();
            //});

        }
        catch (Exception ex) {

            NotificationHandler.ShowErrorMessage(ex);
        }
        finally {

            
        }
    }


    public async Task<List<CadAttribute>> ImportTitleBlockAsync(string blockPath) 
    {
        var attributeList = new List<CadAttribute>();

        try {
            EdtNotificationService.SendPopupNotification(this, $"Starting Autocad");
            StartAutocadAsync();
            EdtNotificationService.CloseoPupNotification(this);

            _acadHelper.AddDrawing(drawingName);
            drawingName = _acadHelper.DocName;


            TitleBlockImporter.AcadHelper = _acadHelper;
            attributeList = TitleBlockImporter.ImportTitleBlock(blockPath);
            _acadHelper.AcadApp.ZoomExtents();

            //await Task.Run(() => {
            //    TitleBlockImporter.AcadHelper = _acadHelper;
            //    attributeList = TitleBlockImporter.ImportTitleBlock(blockPath);
            //    _acadHelper.AcadApp.ZoomExtents();
            //});

            drawingName = "";

            return attributeList;
        }
        catch (Exception ex) {

            return await RetryTitleBlock(ex, ImportTitleBlockAsync, blockPath);
        }
        finally {
            _isRunningTasks = false;
            EdtNotificationService.CloseoPupNotification(this);
        }
    }

    private async Task<List<CadAttribute>> RetryTitleBlock(Exception ex, Func<string, Task<List<CadAttribute>>> action, string blockPath)
    {
        if (ex.Message.Contains("rejected")) { //erase partial drawing and retry
            if (_acadHelper.AcadDoc != null) {
                DeleteDrawingContents();
            }
            return await action(blockPath);
        }

        else if (ex.Message.Contains("busy")) {
            Task.Delay(500); // wait for acad to not be busy
            if (_acadHelper.AcadDoc != null) {
                DeleteDrawingContents();
            }
            return await action(blockPath);
        }

        else if (ex.Message.Contains("instance")) {
            if (_acadHelper.AcadDoc != null) {
                DeleteDrawingContents();
            }
            return await action(blockPath);
        }

        else if (ex.Message.Contains("dispatch")) {
            //NotificationHandler.ShowErrorMessage(ex);
            if (_acadHelper.AcadDoc != null) {
                DeleteDrawingContents();
            }
            return await action(blockPath);
        }
        else {
            NotificationHandler.ShowErrorMessage(ex);
            return null;

        }
    }


    public void CreateSingleLine(IDteq dteq)
    {
        drawCommand = new DrawCommandDelegate(DrawSingleLineAsync);

        if (_isRunningTasks) {
            EdtNotificationService.SendPopupNotification(this, $"Autocad is busy");
        }
        else {
            StartAutocadAsync();
            DrawSingleLineAsync(dteq);
            _acadHelper.AcadApp.Visible = true;

        }
    }
    public void DrawSingleLineAsync(IDteq dteq, bool newDrawing = true)
    {
        try {
            _isRunningTasks = true;
            if (dteq == null) return;


            EdtNotificationService.SendPopupNotification(this, $"Starting Autocad");
            StartAutocadAsync();
            EdtNotificationService.CloseoPupNotification(this);


            _acadHelper.AddDrawing(drawingName);
            drawingName = _acadHelper.DocName;
            EdtNotificationService.SendPopupNotification(this, $"Creating drawing for {dteq.Tag}");

            SingleLineDrawer.AcadHelper = _acadHelper;
            SingleLineDrawer.DrawSingleLine(dteq, 1.5);
            _acadHelper.AcadApp.ZoomExtents();

            //await Task.Run(() => {
            //    SingleLineDrawer.AcadHelper = _acadHelper;
            //    SingleLineDrawer.DrawSingleLine(dteq, 1.5);
            //    _acadHelper.AcadApp.ZoomExtents();
            //});

            drawingName = "";


        }

        catch (Exception ex) {
            Retry(dteq, ex);
        }
        finally {
            _isRunningTasks = false;
            EdtNotificationService.CloseoPupNotification(this);
        }
    }


    public void CreatePanelSchedule(IDteq dteq)
    {
        drawCommand = new DrawCommandDelegate(DrawPanelScheduleAsync);

        if (_isRunningTasks) {
            EdtNotificationService.SendPopupNotification(this, $"Autocad is busy");
        }
        else {

            StartAutocadAsync();
            DrawPanelScheduleAsync(dteq);
            _acadHelper.AcadApp.Visible = true;

        }
    }
    public void DrawPanelScheduleAsync(IDteq dteq, bool newDrawing = true)
    {
        try {
            _isRunningTasks = true;
            if (dteq == null) return;


            EdtNotificationService.SendPopupNotification(this, $"Starting Autocad");
            StartAutocadAsync();
            ErrorHelper.Log("Started Autocad");

            EdtNotificationService.CloseoPupNotification(this);

            _acadHelper.AddDrawing(drawingName);
            drawingName = _acadHelper.DocName;
            EdtNotificationService.SendPopupNotification(this, $"Creating drawing for {dteq.Tag}");



            PanelScheduleDrawer.AcadHelper = _acadHelper;
            PanelScheduleDrawer.DrawPanelSchedule(dteq, 1.5);
            ErrorHelper.Log("Draw Panel Schedule");

            _acadHelper.AcadApp.ZoomExtents();

            //await Task.Run(() => {
            //    PanelScheduleDrawer.AcadHelper = _acadHelper;
            //    PanelScheduleDrawer.DrawPanelSchedule(dteq, 1.5);
            //    ErrorHelper.Log("Draw Panel Schedule");

            //    _acadHelper.AcadApp.ZoomExtents();
            //});

            drawingName = "";

        }

        catch (Exception ex) {

            ErrorHelper.Log("Retry Panel Schedule");

            Retry(dteq, ex);

        }

        finally {
            _isRunningTasks = false;
            EdtNotificationService.CloseoPupNotification(this);
            ErrorHelper.Log("Finally Panel Schedule");

        }
    }



    
    private void Retry(IDteq dteq, Exception ex)
    {
        if (ex.Message.Contains("not found")) {
            MessageBox.Show(
                "Check the Blocks Source Folder path and make sure that the selected blocks exist." + ex.Message,
                "Error - File Not Found",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        else if (ex.Message.Contains("rejected")) { //erase partial drawing and retry
            ErrorHelper.Log("Rejected error");

            Task.Delay(500); // wait for acad to not be busy

            if (_acadHelper.AcadDoc != null) {
                DeleteDrawingContents();
            }
            drawCommand(dteq, false);
        }

        else if (ex.Message.Contains("busy")) {
            ErrorHelper.Log("Busy error");

            Task.Delay(500); // wait for acad to not be busy
            if (_acadHelper.AcadDoc != null) {
                DeleteDrawingContents();
            }
            drawCommand(dteq, false);
        }

        else if (ex.Message.Contains("instance")) {
            ErrorHelper.Log("instance error");

            Task.Delay(500); // wait for acad to not be busy

            if (_acadHelper.AcadDoc != null) {
                DeleteDrawingContents();
            }
            drawCommand(dteq, false);
        }

        else if (ex.Message.Contains("dispatch")) {
            ErrorHelper.Log("dispatch error");

            Task.Delay(500); // wait for acad to not be busy

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
        int _maxDeleteAttempts = DeleteDrawingContents_MaxAttempts;
        int _deleteAttempts = 0;
        try {
            ErrorHelper.Log("Delete Drawing Contents");

            Task.Delay(500);

            dynamic sSet = _acadHelper.AcadDoc.SelectionSets.Add("sSetAll");
            sSet.Select(AcSelect.acSelectionSetAll);
            foreach (dynamic item in sSet) {
                item.Delete();
            }
            sSet.Clear(); //clear selection sets in case this error happens more than once
        }
        catch (Exception ex) {
            
            //if (ex.Message.Contains("rejected") && _deleteAttempts <= _maxDeleteAttempts) {

            if (_deleteAttempts <= _maxDeleteAttempts) {
                ErrorHelper.Log($"Delete Drawing Contents error {ex.Message}");
                _deleteAttempts++;
                Task.Delay(500);

                DeleteDrawingContents();
            }
            else {
                ErrorHelper.Log($"Max Delete attempts {_deleteAttempts} Reached");
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
