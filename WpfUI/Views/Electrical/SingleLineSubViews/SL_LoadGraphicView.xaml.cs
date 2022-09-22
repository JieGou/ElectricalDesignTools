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

namespace WpfUI.Views.Electrical.SingleLineSubViews;
/// <summary>
/// Interaction logic for LoadGraphicView.xaml
/// </summary>
public partial class SL_LoadGraphicView : UserControl
{
    public static RoutedEvent LoadEquipmentSelectedEvent = EventManager.RegisterRoutedEvent("LoadEquipmentSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SL_LoadGraphicView));
    public static RoutedEvent LoadCableSelectedEvent = EventManager.RegisterRoutedEvent("LoadCableSelectedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SL_LoadGraphicView));


    public SL_LoadGraphicView()
    {
        InitializeComponent();
    }

    public event RoutedEventHandler LoadEquipmentSelected
    {
        add { AddHandler(LoadEquipmentSelectedEvent, value); }
        remove { RemoveHandler(LoadEquipmentSelectedEvent, value); }
    }
    protected virtual void OnLoadEquipmentSelected(IEquipment eq)
    {
        RaiseEvent(new RoutedEventArgs(LoadEquipmentSelectedEvent, eq));
    }

    public event RoutedEventHandler LoadCableSelected
    {
        add { AddHandler(LoadCableSelectedEvent, value); }
        remove { RemoveHandler(LoadCableSelectedEvent, value); }
    }
    protected virtual void OnLoadCableSelected(IEquipment eq)
    {
        RaiseEvent(new RoutedEventArgs(LoadCableSelectedEvent, eq));
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
