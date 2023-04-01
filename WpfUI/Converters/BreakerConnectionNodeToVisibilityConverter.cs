using EdtLibrary.LibraryData.TypeModels;
using EDTLibrary;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfUI.Converters
{
    public class BreakerConnectionNodeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cct = (DpnCircuit)value;
            int cctNumber = cct.CircuitNumber;

            string phase = parameter.ToString();

            //3phase panel
            if (cct.VoltageType.Phase == 3) {
                if (phase == "A" && (cctNumber == 1 || (cctNumber - 1) % 3 == 0)) return Visibility.Visible;
                if (phase == "B" && (cctNumber) % 3 == 0) return Visibility.Visible;
                if (phase == "C" && (cctNumber + 1) % 3 == 0) return Visibility.Visible; 
            }

            //split phase panel
            else if (cct.VoltageType.Phase == 1 && cct.VoltageType.Voltage == 240) {
                if (phase == "A" && (cctNumber == 1 || (cctNumber - 1) % 4 == 0)) return Visibility.Visible;
                if (phase == "B") return Visibility.Collapsed;
                if (phase == "C" && (cctNumber + 1) % 4 == 0) return Visibility.Visible;
            }

            //single phase panel
            else if (cct.VoltageType.Phase == 1 && cct.VoltageType.Voltage == 120) {
                if (phase == "A") return Visibility.Visible;
                if (phase == "B") return Visibility.Collapsed;
                if (phase == "C") return Visibility.Collapsed;
            }
            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
       
    }
}
