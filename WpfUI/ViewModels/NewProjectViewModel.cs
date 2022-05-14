using EDTLibrary;
using EDTLibrary.LibraryData.TypeTables;
using PropertyChanged;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Helpers;
using WpfUI.Services;

namespace WpfUI.ViewModels;

[AddINotifyPropertyChangedInterface]

public class NewProjectViewModel
{
    public string ProjectName
    {
        get => _projectName;
        set
        {
            _projectName = value;
            if (SameName == true) {
                FileName = _projectName;
            }
        }
    }
    public string ProjectNumber { get; set; }
    public string Code { get; set; } = "CEC";


    public string FileName 
    { 
        get => _fileName;
        set 
        { 
            _fileName = value;
            if (SameName == true) {
                ProjectName = _fileName;
            }
        }
    }

    public bool SameName
    {
        get => _sameName;
        set
        {
            _sameName = value;
            if (_sameName == true) {
                FileName = ProjectName;
            }
        }
    }
    public string FolderName { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);


    private readonly StartupService _startupService;
    private readonly HomeViewModel _homeViewModel;
    private bool _sameName = true;
    private string _projectName;
    private string _fileName;

    private readonly TypeManager _typeManager;
    public TypeManager TypeManager => _typeManager; // for Code selection in the View

        public NewProjectViewModel(TypeManager typeManager, StartupService startupService, HomeViewModel homeViewModel)
    {
        _typeManager = typeManager;
        _startupService = startupService;
        _homeViewModel = homeViewModel;
        SelectFolderCommand = new RelayCommand(SelectFolder);
        CreateProjectCommand = new RelayCommand(CreateProject);
    }

    public ICommand SelectFolderCommand { get; }
    public ICommand CreateProjectCommand { get; }

    private void SelectFolder()
    {
        using var dialog = new FolderBrowserDialog {
            Description = "Select save location for new project",

            UseDescriptionForTitle = true,

            SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)

            + Path.DirectorySeparatorChar,

            ShowNewFolderButton = true
        };

        if (dialog.ShowDialog() == DialogResult.OK) {

            FolderName = dialog.SelectedPath;
        }

    }

    public void CreateProject()
    {
        bool error = false;
        string errorMessage = "";

        try {
            if (String.IsNullOrEmpty(FileName)|| FileName.Contains("/") || FileName.Contains("\\")) {
                errorMessage += $"The file name '{FileName}' is not valid.\n\n";
                error = true;
            }
            if (Directory.Exists(FolderName) == false) {
                errorMessage += $"The folder '{FolderName} does not exist.'\n\n";
                error = true;
            }

            if (error) {
                System.Windows.MessageBox.Show(errorMessage,
                    "File Error", System.Windows.MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else {
                string fullFileName = FolderName + "\\" + FileName + ".edp";

                File.Copy(GlobalConfig.DevDb, fullFileName, true);

                _startupService.InitializeLibrary();
                _homeViewModel.SetSelectedProject(fullFileName);
                _startupService.InitializeProject(fullFileName);
                var edtSettings = new SettingsViewModel(new EDTLibrary.ProjectSettings.EdtSettings(), _typeManager);
                edtSettings.ProjectName = ProjectName;
                _homeViewModel.NewProjectWindow.Close();
            }
        }
        catch (IOException ex) {
            System.Windows.MessageBox.Show("Invalid File Name\n\nThere are invalid characters in the File name or Folder path.",
                    "File Error", System.Windows.MessageBoxButton.OK, MessageBoxImage.Stop);
        }
        catch (Exception ex) {

            ErrorHelper.EdtErrorMessage(ex);
        }

    }


}
