using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.Cables;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Views.Settings;

namespace WpfUI.ViewModels.Settings;

[AddINotifyPropertyChangedInterface]

public class GeneralSettingsViewModel : SettingsViewModelBase
{

    #region Properties and Backing Fields

    public ObservableCollection<SettingModel> StringSettings { get; set; }
    public SettingModel SelectedStringSetting { get; set; }


    private EdtSettings _edtSettings;
    public EdtSettings EdtSettings
    {
        get { return _edtSettings; }
        set { _edtSettings = value; }
    }
    private TypeManager _typeManager;
    public TypeManager TypeManager
    {
        get { return _typeManager; }
        set { _typeManager = value; }
    }


    #endregion

    #region Commands

    public ICommand NavigateGeneralSettingsCommand { get; }
    private GeneralSettingsView _generalSettingsView = new GeneralSettingsView();
    public ICommand ReloadSettingsCommand { get; }

    public ICommand SaveStringSettingCommand { get; }
    public ICommand SaveTableSettingCommand { get; }

    #endregion

    public GeneralSettingsViewModel(EdtSettings edtSettings, TypeManager typeManager)
    {
        _edtSettings = edtSettings;
        _typeManager = typeManager;
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

}
