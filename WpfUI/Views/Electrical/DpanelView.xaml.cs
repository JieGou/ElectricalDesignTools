using CefSharp;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.Loads;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.XlsIO;
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
using WpfUI.PopupWindows;
using WpfUI.SyncFusion.Renderers;
using WpfUI.ViewModels.Electrical;
using WpfUI.Windows;

namespace WpfUI.Views.Electrical;
/// <summary>
/// Interaction logic for SinlgeLineView.xaml
/// </summary>
public partial class DpanelView : UserControl
{
    private DpanelViewModel vm { get { return DataContext as DpanelViewModel; } }

    public DpanelView()
    {
        InitializeComponent();

        

        //To set the combobox behavior on the datatrids
        //Mainly for single click selecting combobox
        LeftGrid.CellRenderers.Remove("ComboBox");
        LeftGrid.CellRenderers.Add("ComboBox", new ComboBoxRenderer());
        LeftGrid.CellRenderers["ComboBox"] = new CustomGridCellComboBoxRenderer();

        RightGrid.CellRenderers.Remove("ComboBox");
        RightGrid.CellRenderers.Add("ComboBox", new ComboBoxRenderer());
        RightGrid.CellRenderers["ComboBox"] = new CustomGridCellComboBoxRenderer();

        //to set the row heights based on the number of poles
        LeftGrid.QueryRowHeight += LeftGrid_QueryRowHeight;
        RightGrid.QueryRowHeight += RightGrid_QueryRowHeight;
        gridRowResizingOptions.ExcludeColumns = excludeColumns;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        vm.UpdatePanelList();
        if (vm != null) {
            if (vm.ViewableDteqList.Count > 0 & vm.SelectedDteq == null) {
                vm.SelectedDteq = vm.ViewableDteqList[0];
            }
        }
    }

    private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) {
            TextBox tBox = (TextBox)sender;
            DependencyProperty prop = TextBox.TextProperty;

            BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
            if (binding != null) { binding.UpdateSource(); }
        }
    }





    GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();

    //To get the calculated height from GetAutoRowHeight method.    
    double autoHeight = double.NaN;
    double rowHeight = 22;

    // The list contains the column names that will excluded from the height calculation in GetAutoRowHeight method.
    List<string> excludeColumns = new List<string>() { "Id", "Cable", "PowerCable.Size", "Description", "FLA", "Fla", "Tag", "Breaker", "PdSizeTrip", "Size", "DemandKw", "kW" }; 


    private void LeftGrid_QueryRowHeight(object sender, Syncfusion.UI.Xaml.Grid.QueryRowHeightEventArgs e)
    {
        if (LeftGrid.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out autoHeight)) {

            if (autoHeight > 50) {
                e.Height = rowHeight*3;
                e.Handled = true;
            }
            else if (autoHeight > 25) {
                e.Height = rowHeight*2;
                e.Handled = true;
            }
            else {
                e.Height = rowHeight;
                e.Handled = true;
            }
            if (e.RowIndex==0) {
                //hides the header row
                //e.Height = 0;
            }
        }
    }

    private void RightGrid_QueryRowHeight(object sender, Syncfusion.UI.Xaml.Grid.QueryRowHeightEventArgs e)
    {
        if (RightGrid.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out autoHeight)) {

            if (autoHeight > 50) {
                e.Height = rowHeight * 3;
                e.Handled = true;
            }
            else if (autoHeight > 25) {
                e.Height = rowHeight * 2;
                e.Handled = true;
            }
            else {
                e.Height = rowHeight;
                e.Handled = true;
            }
            if (e.RowIndex == 0) {
                //e.Height = 0;
                e.Handled = true;
            }
        }
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
        eventArg.RoutedEvent = UIElement.MouseWheelEvent;
        eventArg.Source = e.Source;

        ScrollViewer scv = (ScrollViewer)sender;
        scv.RaiseEvent(eventArg);
        e.Handled = true;
    }

    private void btnAddLoad_Click(object sender, RoutedEventArgs e)
    {
        loadAdder.Visibility = loadAdder.Visibility == Visibility.Collapsed ? Visibility.Visible: Visibility.Collapsed;
    }

    //left
    private void LeftGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
    {
        if (_clearingGrid) return;

        if (vm == null) return;
        vm.SelectedLoad = (IPowerConsumer)LeftGrid.SelectedItem;
        vm.SelectedLoadRight = null;

        vm.SelectedLoads.Clear();
        foreach (var item in LeftGrid.SelectedItems) {
            vm.SelectedLoads.Add((IPowerConsumer)item);
        }

        _clearingGrid = true;
            RightGrid.ClearSelections(true);
            RightGrid.SelectedIndex = -1; 
        _clearingGrid = false;

    }
    private void LeftGrid_MouseUp(object sender, MouseEventArgs e)
    {
        if (vm == null) return;
        vm.SelectedLoad = (IPowerConsumer)LeftGrid.SelectedItem;
        vm.SelectedLoadRight = null;
    }
    private bool _clearingGrid;
    //right
    private void RightGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
    {
        if (_clearingGrid) return;


        if (vm == null) return;
        vm.SelectedLoad = (IPowerConsumer)RightGrid.SelectedItem;
        vm.SelectedLoadLeft = null;

        vm.SelectedLoads.Clear();
        foreach (var item in RightGrid.SelectedItems) {
            vm.SelectedLoads.Add((IPowerConsumer)item);
        }        

        _clearingGrid = true;
        LeftGrid.ClearSelections(true);
        LeftGrid.SelectedIndex = -1;
        _clearingGrid = false;
    }

    private void RightGrid_MouseUp(object sender, MouseEventArgs e)
    {
        if (vm == null) return;
        vm.SelectedLoad = (IPowerConsumer)RightGrid.SelectedItem;
        vm.SelectedLoadLeft = null;
    }




    private void tutorial_Click(object sender, RoutedEventArgs e)
    {
        var tw = new PdfViewer();
        tw.Owner = Window.GetWindow(this);
        tw.Show();
    }

    private void RightGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
    {
        //var datagrid = (SfDataGrid)sender;
        //if (datagrid.SelectedItem.GetType() == typeof(LoadCircuit)) {
        //    var loadCircuit = (LoadCircuit)datagrid.SelectedItem;
        //    DpnCircuitManager.ConvertToLoad(loadCircuit);
        //}
    }

    private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        vm.SelectedEquipment = vm.SelectedDteq;
    }
}

