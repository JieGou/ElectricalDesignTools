﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace WpfUI.PopupWindows;
/// <summary>
/// Interaction logic for NotificationPopup.xaml
/// </summary>
public partial class PopupNotifcationWindow : Window
{
    private int popupDuration = 3000;
    public static MainWindow MainWindow { get; set; }

    public PopupNotifcationWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));
    }

    private async Task StartCloseSequenceAsync()
    {
        await Task.Run(() => {

            Thread.Sleep(popupDuration);

            App.Current.Dispatcher.Invoke( () => this.Close()  );
        });

    }

    public void SetWindowScreen(Window window, Screen screen)
    {
        if (screen != null) {
            if (!window.IsLoaded) {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
            }

            var workingArea = screen.WorkingArea;
            window.Left = workingArea.Left + workingArea.Width - this.Width - 10;
            window.Top = workingArea.Top + workingArea.Height - this.Height - 10;
        }
    }
    public Screen GetWindowScreen(Window window)
    {
        return Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(window).Handle);
    }
    private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        this.Close();
    }

}
