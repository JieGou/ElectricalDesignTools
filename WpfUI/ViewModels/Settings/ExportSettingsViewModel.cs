﻿using EDTLibrary.DataAccess;
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

public class ExportSettingsViewModel : SettingsViewModelBase
{

    #region Properties and Backing Fields

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

    public ExportSettingsViewModel(EdtSettings edtSettings, TypeManager typeManager)
    {
        _edtSettings = edtSettings;
        _typeManager = typeManager;

        SaveMappingCommand = new RelayCommand(SaveMapping);
    }

    public ObservableCollection<string> ReportTypes { get; set; } = new ObservableCollection<string>() {
         "Distribution Equipment List",
         "Load List",
         "Cable List",
    };
       
    private string _selectedReportType;

    public string SelectedReportType
    {
        get { return _selectedReportType; }
        set
        {
            _selectedReportType = value;
            var selectedMappings = EdtSettings.ExportMappings.Where(em => em.Type == _selectedReportType).ToList();
            SelectedExportMappings = new ObservableCollection<ExportMappingModel>(selectedMappings);
        }
    }
    public ExportMappingModel SelectedExportMappingModel 
    { 
        get; 
        set; 
    }
    public ICommand SaveMappingCommand { get; }
    public void SaveMapping()
    {
        if (SelectedExportMappingModel == null) {
            return;
        }
        SettingsManager.SaveExportMappingToDb(SelectedExportMappingModel);
    }
    public ObservableCollection<ExportMappingModel> SelectedExportMappings { get; set; } = new ObservableCollection<ExportMappingModel>();

    //General



}