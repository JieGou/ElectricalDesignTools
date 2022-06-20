﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfUI.PopupWindows;
/// <summary>
/// Interaction logic for NotificationPopup.xaml
/// </summary>
public partial class NotificationPopup : Window
{
    public NotificationPopup()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
        this.Left = desktopWorkingArea.Right - this.Width - 10 ;

        //Bottom
        this.Top = desktopWorkingArea.Bottom - this.Height - 10;

        //Top
        //this.Top = desktopWorkingArea.Top + 50;

    }

    private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        this.Close();
    }
}
