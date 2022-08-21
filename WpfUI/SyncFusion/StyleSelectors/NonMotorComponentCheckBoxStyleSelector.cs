using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using EDTLibrary.Models.Loads;
using EDTLibrary;

namespace WpfUI.SyncFusion.StyleSelectors;
public class NonMotorComponentCheckBoxStyleSelector : StyleSelector
{
    public override Style SelectStyle(object item, DependencyObject container)
    {
        var row = item as IPowerConsumer;
        if (row != null) {
            if (row.Type != LoadTypes.MOTOR.ToString())
                return App.Current.Resources["NonMotorComponentCheckBoxStyle"] as Style;
        }
        return base.SelectStyle(item, container);
    }
}
