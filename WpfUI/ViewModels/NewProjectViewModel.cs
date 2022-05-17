﻿using EDTLibrary;
using EDTLibrary.LibraryData.TypeTables;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Helpers;
using WpfUI.Services;

namespace WpfUI.ViewModels;

[AddINotifyPropertyChangedInterface]

public class NewProjectViewModel : ViewModelBase, INotifyDataErrorInfo
{
    public string ProjectName
    {
        get => _projectName;
        set
        {
            _projectName = value;
            if (SameName == true) {
                FileName = _projectName;
                _fileName = _projectName;
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
            ClearErrors(nameof(FileName));
            if (SameName == true) {
                ProjectName = _fileName;
            }
            if (String.IsNullOrEmpty(_fileName) || _fileName.Contains("/") || _fileName.Contains("\\")) {
                ClearErrors(nameof(FileName));
                AddError(nameof(FileName), $"The file name '{_fileName}' is not valid.");

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

   private string _folderName = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    public string FolderName
    {
        get => _folderName;

        set
        {
            _folderName = value;
            ClearErrors(nameof(FolderName));

            if (Directory.Exists(_folderName) == false) {
                AddError(nameof(FolderName), $"The folder '{_folderName}' does not exist.");
            }
        }
    }


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
            _folderName = dialog.SelectedPath;
        }

    }

    public void CreateProject()
    {

        ClearErrors();
        try {
            if (String.IsNullOrEmpty(FileName) || FileName.Contains("/") || FileName.Contains("\\")) {
                AddError(nameof(FileName), $"The file name '{FileName}' is not valid.");

            }
            if (Directory.Exists(FolderName) == false) {
                AddError(nameof(FolderName), $"The folder name '{FolderName}' is not valid.");
            }

            if (HasErrors == false) {
                string fullFileName = FolderName + "\\" + FileName + ".edp";

                File.Copy(GlobalConfig.DevDb, fullFileName, true);

                _startupService.InitializeLibrary();
                _homeViewModel.StartupService.SetSelectedProject(fullFileName);
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

            ErrorHelper.ShowErrorMessage(ex);
        }
    }
}
