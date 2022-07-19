﻿using EDTLibrary;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WpfUI.Helpers;
using WpfUI.Services;
using WpfUI.ViewModels;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Interop;
using WpfUI.ViewModels.Cables;
using WpfUI.ViewModels.Areas_and_Systems;
using WpfUI.ViewModels.Electrical;
using WpfUI.ViewModels.Menus;

namespace WpfUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
///

public partial class MainWindow : Window
{

    private MainViewModel mainVm { get { return DataContext as MainViewModel; } }
    public MainWindow()
    {
        InitializeComponent();
        string[] args = Environment.GetCommandLineArgs();

        if (args[0] != "") {
            //MessageBox.Show(args[0]);
        }

        //mainVm.StartupService.SelectProject("C:\\Users\\pdeau\\Desktop\\test.edp");
        //StartupService ssTest = new StartupService(new ListManager());
        //ssTest.SetSelectedProject("C:\\Users\\pdeau\\Desktop\\test.edp");

        if (args.Length >= 2) {
            //MessageBox.Show(args[1]);

            try {
                if ((args[1]).Contains(".edp") && File.Exists(args[1])) {
                    string fullFilePath = args[1];
                    StartupService ss = new StartupService(new ListManager());
                    ss.SetSelectedProject(fullFilePath);
                }
            }
            catch (Exception ex) {
                ErrorHelper.ShowErrorMessage(ex);
            }
        }
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
            if (e.Key == Key.Z) {
                Undo.UndoCommand(mainVm._listManager);
            }
        }
        if (Keyboard.IsKeyDown(Key.Escape)) {
            //Keyboard.ClearFocus();
        }
    }

    private void AreaMenuButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed) {
            MainViewModel newMainVm = new MainViewModel(mainVm.StartupService, mainVm.ListManager, mainVm.TypeManager, mainVm.EdtSettings, "ExtraWindow");
            mainVm.NewWindow(new AreasMenuViewModel(newMainVm, mainVm._listManager), new AreasViewModel(mainVm._listManager));
        }
    }
    private void ElectricalMenuButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed) {
            MainViewModel newMainVm = new MainViewModel(mainVm.StartupService,mainVm.ListManager,mainVm.TypeManager, mainVm.EdtSettings,"ExtraWindow");
            mainVm.NewWindow(new ElectricalMenuViewModel(newMainVm, mainVm._listManager), new MjeqViewModel(mainVm._listManager));
        }
    }

    private void CableMenuButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed) {
            MainViewModel newMainVm = new MainViewModel(mainVm.StartupService, mainVm.ListManager, mainVm.TypeManager, mainVm.EdtSettings, "ExtraWindow");
            mainVm.NewWindow(new CableMenuViewModel(newMainVm, mainVm._listManager), new CableListViewModel(mainVm._listManager));
        }
    }

    
    private void _ribbon_SelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        //if (mainVm != null) {
        //    if (_ribbon.SelectedIndex == 0) {
        //        mainVm.CurrentViewModel = mainVm._homeViewModel;
        //    }
        //}

            //else if (_ribbon.SelectedIndex == 1) {
            //    mainVm.CurrentViewModel = mainVm._settingsViewModel;
            //}
            //else if (_ribbon.SelectedIndex == 2) {
            //    mainVm.CurrentViewModel = mainVm._areasViewModel;
            //}
            //else if (_ribbon.SelectedIndex == 3) {
            //    mainVm.CurrentViewModel = mainVm._electricalViewModel;
            //}
            //else if (_ribbon.SelectedIndex == 4) {
            //    mainVm.CurrentViewModel = mainVm._cableListViewModel;
            //}
        
    }

    private void RibbonRadioButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        //mainVm.CurrentViewModel = mainVm._settingsViewModel;
        //mainVm._settingsViewModel.SelectedSettingView = new GeneralSettingsView();
        //mainVm._settingsViewModel.SelectedSettingView.DataContext = mainVm.CurrentViewModel;


    }
}



