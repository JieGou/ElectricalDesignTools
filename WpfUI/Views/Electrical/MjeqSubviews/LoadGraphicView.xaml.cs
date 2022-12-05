using EDTLibrary.Models;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WpfUI.Controls;
using WpfUI.ViewModels.Electrical;

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
    
    public static RoutedEvent LoadEquipmentSelectedEvent = EventManager.RegisterRoutedEvent("EquipmentSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LoadGraphicView));
    public static RoutedEvent LoadEquipmentCableSelectedEvent = EventManager.RegisterRoutedEvent("LoadEquipmentCableSelectedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LoadGraphicView));


    
    public event RoutedEventHandler EquipmentSelected
    {
        add { AddHandler(LoadEquipmentSelectedEvent, value); }
        remove { RemoveHandler(LoadEquipmentSelectedEvent, value); }
    }
    protected virtual void OnEquipmentSelected(IEquipment eq)
    {
        RaiseEvent(new RoutedEventArgs(LoadEquipmentSelectedEvent, eq));
    }

    public event RoutedEventHandler EquipmentCableSelected
    {
        add { AddHandler(LoadEquipmentCableSelectedEvent, value); }
        remove { RemoveHandler(LoadEquipmentCableSelectedEvent, value); }
    }
    protected virtual void OnEquipmentCableSelected(IEquipment eq)
    {
        RaiseEvent(new RoutedEventArgs(LoadEquipmentCableSelectedEvent, eq));
    }

    


    #region Click Events
    private void Bucket_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        try {

            if (dataContext is IEquipment) {
               OnEquipmentSelected(dataContext as IEquipment);
            }

        }
        catch (Exception) {

        }
    }

    private void ComponentCable_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            if (dataContext is ComponentModel) {
                ComponentModel component = (ComponentModel)dataContext;
                OnEquipmentCableSelected(component);
            }
        }
    }

    private void Component_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            OnEquipmentSelected(dataContext as IEquipment);
        }
    }

    private void EquipmentCable_ContentControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            if (dataContext is LoadModel) {
                LoadModel load = (LoadModel)dataContext;
                OnEquipmentCableSelected(load);
            }
        }
    }

    private void Equipment_ContentControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            OnEquipmentSelected(dataContext as IEquipment);
        }
    }
    #endregion
}
