using EDTLibrary;
using EDTLibrary.Models.Loads;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.SyncFusion.Renderers;

public class CustomGridCellCheckBoxRenderer : GridCellCheckBoxRenderer
{
    public override void OnInitializeEditElement(DataColumnBase dataColumn, CheckBox uiElement, object dataContext)
    {
        base.OnInitializeEditElement(dataColumn, uiElement, dataContext);

		try {
            var record = dataContext as IPowerConsumer;

            if (record.Category != Categories.LOAD3P.ToString()) {
                uiElement.Visibility = Visibility.Hidden;

            }
            else if (dataColumn.GridColumn.MappingName != "DisconnectBool") {
				if (record.Type != LoadTypes.MOTOR.ToString())
					uiElement.Visibility = Visibility.Hidden; 
			}
            
        }
		catch (Exception) {

		}
    }
}