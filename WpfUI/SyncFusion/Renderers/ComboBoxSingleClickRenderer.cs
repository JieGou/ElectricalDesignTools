using Syncfusion.UI.Xaml.Grid.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace WpfUI.SyncFusion.Renderers;

public class ComboBoxRenderer : GridCellComboBoxRenderer
{
    protected override void OnEditElementLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        base.OnEditElementLoaded(sender, e);
        var combobox = sender as ComboBox;
        combobox.IsDropDownOpen = true;
    }
}
