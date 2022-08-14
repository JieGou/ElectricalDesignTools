using EDTLibrary.Models;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfUI.Views.Electrical.MjeqSubviews;
/// <summary>
/// Interaction logic for LoadGraphicView.xaml
/// </summary>
public partial class LoadGraphicView : UserControl
{
    public LoadGraphicView()
    {
        InitializeComponent();
    }

    private void ContentControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            LoadModel load = (LoadModel)dataContext;
            MessageBox.Show($"PD Trip: {load.PdSizeTrip.ToString()} \n" +
                $"PD: Frame: {load.PdSizeFrame}");
        }
    }

    private void ContentControl_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            ComponentModel component = (ComponentModel)dataContext;
            MessageBox.Show($"Cable Tag: {component.PowerCable.Tag}");
        }
    }

    private void ContentControl_PreviewMouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            ComponentModel component = (ComponentModel)dataContext;
            MessageBox.Show($"Component Tag: {component.Tag}");
        }
    }

    private void ContentControl_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            ComponentModel component = (ComponentModel)dataContext;
            MessageBox.Show($"Component Tag: {component.Tag}");
        }
    }

    private void ContentControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            LoadModel load = (LoadModel)dataContext;
            MessageBox.Show($"Load Cable: {load.PowerCable.Tag}");
        }
    }

    private void ContentControl_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            LoadModel load = (LoadModel)dataContext;
            MessageBox.Show($"Load: {load.Tag}");
        }
    }
}
