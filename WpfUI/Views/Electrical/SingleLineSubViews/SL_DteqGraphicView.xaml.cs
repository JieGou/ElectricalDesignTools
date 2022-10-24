using EDTLibrary.Models;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
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

namespace WpfUI.Views.Electrical.SingleLineSubViews;
/// <summary>
/// Interaction logic for LoadGraphicView.xaml
/// </summary>
public partial class SL_DteqGraphicView : UserControl
{
    public static RoutedEvent DteqEquipmentSelectedEvent = EventManager.RegisterRoutedEvent("LoadEquipmentSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SL_LoadGraphicView));
    public static RoutedEvent DteqCableSelectedEvent = EventManager.RegisterRoutedEvent("LoadCableSelectedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SL_LoadGraphicView));


    public SL_DteqGraphicView()
    {
        InitializeComponent();
    }

    public event RoutedEventHandler LoadEquipmentSelected
    {
        add { AddHandler(DteqEquipmentSelectedEvent, value); }
        remove { RemoveHandler(DteqEquipmentSelectedEvent, value); }
    }
    protected virtual void OnLoadEquipmentSelected(IEquipment eq)
    {
        RaiseEvent(new RoutedEventArgs(DteqEquipmentSelectedEvent, eq));
    }

    public event RoutedEventHandler LoadCableSelected
    {
        add { AddHandler(DteqCableSelectedEvent, value); }
        remove { RemoveHandler(DteqCableSelectedEvent, value); }
    }
    protected virtual void OnLoadCableSelected(IEquipment eq)
    {
        RaiseEvent(new RoutedEventArgs(DteqCableSelectedEvent, eq));
    }

    


    #region Click Events
    private void Bucket_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        try {

            if (dataContext is IEquipment) {
               OnLoadEquipmentSelected(dataContext as IEquipment);
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
                OnLoadCableSelected(component);
            }
            else if (dataContext is DistributionEquipment) {
                IPowerConsumer dteq = DteqFactory.Recast(dataContext);
                OnLoadCableSelected(dteq);
            }
        }
    }

    private void Component_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            OnLoadEquipmentSelected(dataContext as IEquipment);
        }
    }

    private void LoadCable_ContentControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            if (dataContext is ILoad) {
                ILoad load = (LoadModel)dataContext;
                OnLoadCableSelected(load);
            }
            else if (dataContext is DistributionEquipment) {
                IPowerConsumer  dteq = DteqFactory.Recast(dataContext);
                OnLoadCableSelected(dteq);
            }
        }
    }

    private void Load_ContentControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        ContentControl senderControl = (ContentControl)sender;
        var dataContext = senderControl.DataContext;

        if (dataContext is IEquipment) {
            OnLoadEquipmentSelected(dataContext as IEquipment);
        }
    }
    #endregion
}
