using EDTLibrary;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.DPanels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfUI.Converters;
public class TypeOfConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        string level = (string)parameter;
        if (value == null) {
            return null;
        }

        else if (value.GetType() ==typeof( XfrModel)) {
            return typeof(XfrModel); ;
        }

        else if (value.GetType() == typeof(SwgModel)) {
            return typeof(SwgModel); ;
        }

        else if (value.GetType() == typeof(MccModel)) {
            return typeof(MccModel); ;
        }

        else if (value.GetType() == typeof(DpnModel)) {
            var pnl = (DpnModel)value;

            switch (level)  {
             
                case "Load":
                    return typeof(DpnModel).ToString();
                    break;
                case "Dteq_Load":
                    if (pnl.FedFrom.Type== DteqTypes.XFR.ToString()) {
                        return typeof(DpnModel).ToString();
                    }
                    break;
                    
            }
            return null;
        }

        else if (value is DistributionEquipment) {
            return typeof(DistributionEquipment); ;
        }

        else {
            return value.GetType();
        }
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
