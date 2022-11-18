using Microsoft.AspNetCore.Components;
using Microsoft.Diagnostics.Tracing.Parsers.IIS_Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfUI.Windows;
/// <summary>
/// Interaction logic for LoadingWindow.xaml
/// </summary>
public partial class LoadingWindow : Window
{
    public LoadingWindow()
    {
        InitializeComponent();
    }


    public static LoadingWindow CreateAsync(FrameworkElement overlayedElement)
    {

        var locationFromScreen = overlayedElement.PointToScreen(new Point(0, 0));

        var windowLauncher = new AsyncWindowLauncher(
            overlayedElement,
            locationFromScreen.X, 
            locationFromScreen.Y, 
            overlayedElement.ActualHeight, 
            overlayedElement.ActualHeight
        );

        var windowThread = new Thread(windowLauncher.CreateWindow);
        windowThread.SetApartmentState(ApartmentState.STA);
        windowThread.Start();

        //while (windowLauncher.Window == null) {

        //}

        return windowLauncher.Window;
    }

    private void OnWindowClosed(object sender, EventArgs args)
    {
        Dispatcher.InvokeShutdown();
    }

    private class AsyncWindowLauncher
    {
        private LoadingWindow _window;
        private FrameworkElement _overlayedElement;
        private double _x;
        private double _y;
        private double _width;
        private double _height;

        public AsyncWindowLauncher(FrameworkElement overlayedElement, double x, double y, double width, double height)
        {
            _overlayedElement = overlayedElement;
            _x = x;
            _y = y+500;
            _width = width;
            _height = height;
        }

        public LoadingWindow Window { get => _window; set => _window = value; }

        public void CreateWindow()
        {
            Window = new LoadingWindow();
            Window.Left = _x;
            Window.Top = _y;
            Window.Width = _width;
            Window.Height = _height;

            Window.Closed += Window.OnWindowClosed;
        }
    }

}


