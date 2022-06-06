using System.Windows;
using System.Windows.Controls;

namespace WpfUI.Views.Electrical.MjeqSubviews;

public static class DataGridExtensions
{
    public static void FastEdit(this DataGridCell dataGridCell, RoutedEventArgs args)
    {
        if (dataGridCell == null || dataGridCell.IsEditing || dataGridCell.IsReadOnly) {
            return;
        }

        var dataGrid = dataGridCell.FindVisualParent<DataGrid>();

        if (dataGrid == null) {
            return;
        }

        if (!dataGridCell.IsFocused) {
            dataGridCell.Focus();
        }

        dataGrid.Dispatcher.InvokeAsync(() => {
            dataGrid.BeginEdit(args);
        });
    }
}



