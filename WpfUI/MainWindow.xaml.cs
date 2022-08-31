using EDTLibrary;
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
using WpfUI.ViewModels.AreasAndSystems;
using WpfUI.ViewModels.Electrical;
using WpfUI.ViewModels.Menus;
using WpfUI.Windows;
using EDTLibrary.UndoSystem;
using MahApps.Metro.Controls;
using EDTLibrary.Managers;
using System.Collections.ObjectModel;
using WpfUI.ViewModels.Home;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace WpfUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
///

public partial class MainWindow : MetroWindow
{
    public ObservableCollection<PreviousProject> PreviousProjects { get; set; } = new ObservableCollection<PreviousProject>();
    private MainViewModel mainVm { get { return DataContext as MainViewModel; } }

    public MainWindow()
    {
        InitializeComponent();
        btnHome.IsChecked = true;
        string[] args = Environment.GetCommandLineArgs();

        if (args[0] != "") {
            
        }

#if DEBUG
        //args = new string[2];
        //args[0] = "C:\\C - Visual Studio Projects\\ElectricalDesignTools\\WpfUI\\bin\\Debug\\net6.0-windows10.0.22000.0\\Electrical Design Tools.dll";
        //args[1] = @"C:\Users\pdeau\Desktop\asdf.edp";

#endif

        //File Open
        if (args.Length >= 2) {
            //MessageBox.Show(args[1]);

            try {
                if ((args[1]).Contains(".edp") && File.Exists(args[1])) {
                    string fullFilePath = args[1];

                    StartupService ss = new StartupService(new ListManager(), PreviousProjects);
                    ss.SetSelectedProject(fullFilePath);
                }
            }
            catch (Exception ex) {
                ErrorHelper.ShowErrorMessage(ex);
            }
        }
    }

    DebugWindow debugWindow = null;
    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
            if (e.Key == Key.Z) {
                UndoManager.UndoCommand(mainVm._listManager);
            }
            if (e.Key == Key.D) {
                if (debugWindow == null || debugWindow.IsLoaded == false) {
                    debugWindow = new DebugWindow();
                    //debugWindow.Owner = this;
                    debugWindow.Show();
                }
                else {
                    debugWindow.Focus();
                }
                e.Handled = true;
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

    private void winMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (debugWindow!= null) {
            debugWindow.Close();

        }

        try {
            //Create Recent Projects DTO List
            var ppDtos = new ObservableCollection<PreviousProjectDto>();
            foreach (var item in mainVm.StartupService.PreviousProjects) {
                ppDtos.Add(new PreviousProjectDto(item.Project));
            }


            //Serialize Recent Projects DTOs
            using (Stream stream = File.Open("RecentProjects.bin", FileMode.Create)) {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, ppDtos);
            }
        }

        catch (IOException) {
        }

    }
}



