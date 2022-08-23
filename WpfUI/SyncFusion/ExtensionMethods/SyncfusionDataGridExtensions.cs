using Syncfusion.UI.Xaml.Diagram.Utility;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.SyncFusion.ExtensionMethods;

public static class SyncfusionDataGridExtensions
{
    public static void FastEdit(this Syncfusion.UI.Xaml.Grid.GridCell dataGridCell, RoutedEventArgs args)
    {
        if (dataGridCell == null ) {
            return;
        }

        var dataGrid = dataGridCell.FindVisualParent< Syncfusion.UI.Xaml.Grid.SfDataGrid> ();

        if (dataGrid == null) {
            return;
        }

        if (!dataGridCell.IsFocused) {
            dataGridCell.Focus();
        }

        dataGrid.Dispatcher.InvokeAsync(() => {
            //dataGrid.CurrentCellBeginEdit(args);
        });
    }
}



