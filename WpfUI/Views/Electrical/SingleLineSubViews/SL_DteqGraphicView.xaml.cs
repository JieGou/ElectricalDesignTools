using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfUI.Views.Electrical.SingleLineSubViews;
/// <summary>
/// Interaction logic for SL_DteqGraphicView.xaml
/// </summary>
public partial class SL_DteqGraphicView : UserControl
{
    public SL_DteqGraphicView()
    {
        InitializeComponent();
    }

    public string DisplayMode
    {
        get { return (string)GetValue(DisplayModeProperty); }
        set { SetValue(DisplayModeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Display Mode.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DisplayModeProperty =
        DependencyProperty.Register("DisplayMode", typeof(string), typeof(SL_DteqGraphicView), new PropertyMetadata("Display Mode"));



    public static RoutedEvent EquipmentSelectedEvent = EventManager.RegisterRoutedEvent("EquipmentSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SL_DteqGraphicView));
    public static RoutedEvent EquipmentCableSelectedEvent = EventManager.RegisterRoutedEvent("EquipmentCableSelectedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SL_DteqGraphicView));


    public event RoutedEventHandler EquipmentSelected
    {
        add { AddHandler(EquipmentSelectedEvent, value); }
        remove { RemoveHandler(EquipmentSelectedEvent, value); }
    }
    protected virtual void OnEquipmentSelected(IEquipment eq)
    {
        RaiseEvent(new RoutedEventArgs(EquipmentSelectedEvent, eq));
    }

    public event RoutedEventHandler EquipmentCableSelected
    {
        add { AddHandler(EquipmentCableSelectedEvent, value); }
        remove { RemoveHandler(EquipmentCableSelectedEvent, value); }
    }
    protected virtual void OnEquipmentCableSelected(IEquipment eq)
    {
        RaiseEvent(new RoutedEventArgs(EquipmentCableSelectedEvent, eq));
    }




    #region Click Events
    private void Bucket_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var senderControl = (FrameworkElement)sender;
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
        var senderControl = (FrameworkElement)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            if (dataContext is ComponentModel) {
                ComponentModel component = (ComponentModel)dataContext;
                OnEquipmentCableSelected(component);
            }
            else if (dataContext is DistributionEquipment) {
                IPowerConsumer dteq = DteqFactory.Recast(dataContext);
                OnEquipmentCableSelected(dteq);
            }
        }
    }

    private void Component_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var senderControl = (FrameworkElement)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            OnEquipmentSelected(dataContext as IEquipment);
        }
    }

    private void EquipmentCable_ContentControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var senderControl = (FrameworkElement)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            if (dataContext is ILoad) {
                ILoad load = (LoadModel)dataContext;
                OnEquipmentCableSelected(load);
            }
            else if (dataContext is DistributionEquipment) {
                IPowerConsumer dteq = DteqFactory.Recast(dataContext);
                OnEquipmentCableSelected(dteq);
            }
        }
    }

    private void Equipment_ContentControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        var senderControl = (FrameworkElement)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            OnEquipmentSelected(dataContext as IEquipment);
        }
    }
    #endregion
}
