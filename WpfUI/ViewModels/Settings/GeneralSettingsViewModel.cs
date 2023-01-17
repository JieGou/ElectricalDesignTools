using AutocadLibrary;
using EDTLibrary.Autocad.Interop;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Helpers;

namespace WpfUI.ViewModels.Settings;

[AddINotifyPropertyChangedInterface]

public class GeneralSettingsViewModel : SettingsViewModelBase
{

    #region Properties and Backing Fields

    private EdtSettings _edtSettings;
    public EdtSettings EdtSettings
    {
        get { return _edtSettings; }
        set { _edtSettings = value; }
    }
    private TypeManager _typeManager;
    private readonly ListManager _listManager;

    public TypeManager TypeManager
    {
        get { return _typeManager; }
        set { _typeManager = value; }
    }
  
    #endregion

    public GeneralSettingsViewModel(EdtSettings edtSettings, TypeManager typeManager, ListManager listManager = null)
    {
        _edtSettings = edtSettings;
        _typeManager = typeManager;
        _listManager = listManager;
        SelectAcadSaveFolderCommand = new RelayCommand(SelectAcadSaveFolder);
        SelectAcadBlockFolderCommand = new RelayCommand(SelectAcadBlockFolder);
        TestAcadCommand = new RelayCommand(StartAutocad);
        AddAcadDrawingCommand = new RelayCommand(AddDrawing);
        AddBlockCommand = new RelayCommand(AddBlock);
    }

    //General

    private string _projectName;
    public string ProjectName
    {
        get => _projectName;
        set
        {
            var oldValue = _projectName;
            _projectName = value;
            ClearErrors(nameof(ProjectName));

            bool isValid = Regex.IsMatch(_projectName, @"^[\sA-Z0-9_@#$%^&*()-]+$", RegexOptions.IgnoreCase);
            if (isValid == false) {
                AddError(nameof(ProjectName), "Invalid character");
            }
            else if (isValid) {
                SaveVmSetting(nameof(ProjectName), _projectName);
            }
        }
    }

    //Project Details

    private string _projectNumber;
    public string ProjectNumber

    {
        get { return _projectNumber; }
        set
        {
            var oldValue = _projectNumber;
            _projectNumber = value;
            ClearErrors(nameof(ProjectNumber));

            SaveVmSetting(nameof(ProjectNumber), _projectNumber);
        }
    }

    private string _projectTitleLine1;
    public string ProjectTitleLine1
    {
        get { return _projectTitleLine1; }
        set
        {
            _projectTitleLine1 = value;
            SaveVmSetting(nameof(ProjectTitleLine1), _projectTitleLine1);
        }
    }

    private string _projectTitleLine2;
    public string ProjectTitleLine2
    {
        get { return _projectTitleLine2; }
        set
        {
            _projectTitleLine2 = value;
            SaveVmSetting(nameof(ProjectTitleLine2), _projectTitleLine2);
        }
    }

    private string _projectTitleLine3;
    public string ProjectTitleLine3
    {
        get { return _projectTitleLine3; }
        set
        {
            _projectTitleLine3 = value;
            SaveVmSetting(nameof(ProjectTitleLine3), _projectTitleLine3);
        }
    }

    private string _clientTitleLine1;
    public string ClientNameLine1
    {
        get { return _clientTitleLine1; }
        set
        {
            _clientTitleLine1 = value;
            SaveVmSetting(nameof(ClientNameLine1), _clientTitleLine1);
        }
    }

    private string _clientTitleLine2;
    public string ClientNameLine2
    {
        get { return _clientTitleLine2; }
        set
        {
            _clientTitleLine2 = value;
            SaveVmSetting(nameof(ClientNameLine2), _clientTitleLine2);
        }
    }

    private string _clientTitleLine3;
    public string ClientNameLine3
    {
        get { return _clientTitleLine3; }
        set
        {
            _clientTitleLine3 = value;
            SaveVmSetting(nameof(ClientNameLine3), _clientTitleLine3);
        }
    }


    //General

    private string _code;
    public string Code
    {
        get { return _code; }
        set
        {
            _code = value;
            SaveVmSetting(nameof(Code), _code);
        }
    }

    private bool _areaColumnBool;
    public bool AreaColumnBool
    {
        get { return _areaColumnBool; }
        set { _areaColumnBool = value;
            if (_areaColumnBool) {
                AreaColumnVisible = "Visible";
                return;
            }
            AreaColumnVisible = "Collapsed";

        }
    }


    private string _areaColumnVisible;
    public string AreaColumnVisible
    {
        get { return _areaColumnVisible; }
        set 
        { 
            _areaColumnVisible = value; 
            SaveVmSetting(nameof(AreaColumnVisible), _areaColumnVisible);
        }
    }


    public ICommand SelectAcadBlockFolderCommand { get; }

    private string _acadBlockFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    public string AcadBlockFolder
    {
        get => _acadBlockFolder;

        set
        {
            _acadBlockFolder = value;
            SaveVmSetting(nameof(AcadBlockFolder), _acadBlockFolder);
        }
    }
    private void SelectAcadBlockFolder()
    {
        using var dialog = new FolderBrowserDialog {
            Description = "Select save location for new project",

            UseDescriptionForTitle = true,

            SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)

            + Path.DirectorySeparatorChar,

            ShowNewFolderButton = true
        };

        if (dialog.ShowDialog() == DialogResult.OK) {

            AcadBlockFolder = dialog.SelectedPath;
        }
    }


    public ICommand SelectAcadSaveFolderCommand { get; }

    private string _acadSaveFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    public string AcadSaveFolder
    {
        get => _acadSaveFolder;

        set
        {
            _acadSaveFolder = value;
            SaveVmSetting(nameof(AcadSaveFolder), _acadSaveFolder);

        }
    }
    private void SelectAcadSaveFolder()
    {
        using var dialog = new FolderBrowserDialog {
            Description = "Select save location for new project",

            UseDescriptionForTitle = true,

            SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)

            + Path.DirectorySeparatorChar,

            ShowNewFolderButton = true
        };

        if (dialog.ShowDialog() == DialogResult.OK) {

            AcadSaveFolder = dialog.SelectedPath;
        }
    }


    public ICommand TestAcadCommand { get; }
    public AutocadHelper Acad { get; set; }

    #region Autocad
    public void StartAutocad()
    {
        Acad = new AutocadHelper();
        Acad.StartAutocad();
    }


    public ICommand AddAcadDrawingCommand { get; }
    public void AddDrawing()
    {
        if (Acad == null) {
            MessageBox.Show("Connect to Autocad First");
            return;
        }
        Acad.AddDrawing();
    }


    public ICommand AddBlockCommand { get; }
    public void AddBlock()
    {
        if (Acad == null ) {
            MessageBox.Show("Connect to Autocad First");
            return;
        }
        if (Acad.AcadDoc==null) {
            Acad.AcadDoc = Acad.AcadApp.ActiveDocument;
        }

        try {
            SingleLineDrawer slDrawer = new SingleLineDrawer(Acad, EdtSettings.AcadBlockFolder);

            MccModel mcc = _listManager.MccList.FirstOrDefault(m => m.Tag.Contains("MCC"));

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
            else {
                ErrorHelper.ShowErrorMessage(ex);
            }
        }
    }
    #endregion
}
