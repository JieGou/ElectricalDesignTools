using EDTLibrary;
using EDTLibrary.Models.Cables;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfUI.Converters
{
    public class CableModelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CableModel cable = (CableModel)value;
            var bc = new BrushConverter();

            if (cable.UsageType == CableUsageTypes.Power.ToString()) {
                if (cable.VoltageRating < 5000) {
                    return new SolidColorBrush(Colors.Black);
                }
                else if (cable.VoltageRating < 15000) {
                    return new SolidColorBrush(Colors.Orange);
                }
                else if (cable.VoltageRating >= 15000) {
                    return (Brush)bc.ConvertFrom("#FFD40000"); //red
                }
            }
            else if (cable.UsageType == CableUsageTypes.Control.ToString()) {
                return new SolidColorBrush(Colors.Gray);
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
