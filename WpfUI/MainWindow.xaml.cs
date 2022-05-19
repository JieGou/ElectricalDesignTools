using EDTLibrary;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WpfUI.Helpers;
using WpfUI.Services;
using WpfUI.ViewModels;
using Syncfusion.Windows.Tools.Controls;

namespace WpfUI
{
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
        }

        private void AreaMenuButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed) {
                mainVm.NewWindow(new AreasViewModel(mainVm._listManager));
            }
        }

        private void CableMenuButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed) {
                mainVm.NewWindow(new CableListViewModel(mainVm._listManager));
            }
        }

        private void ElectricalMenuButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed) {
                mainVm.NewWindow(new ElectricalViewModel(mainVm._listManager));
            }
        }
    }
}
