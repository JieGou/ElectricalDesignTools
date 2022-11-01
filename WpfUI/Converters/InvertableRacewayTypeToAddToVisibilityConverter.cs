using EDTLibrary;
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
    public class InvertableRacewayTypeToAddToVisibilityConverter : IValueConverter
    {
        enum Parameters
        {
            Tray, Conduit
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
            if (value == null) return direction == Parameters.Tray ? Visibility.Visible : Visibility.Collapsed;
            string stringValue = value.ToString();

            if (stringValue == RacewayTypes.Conduit.ToString() || stringValue == RacewayTypes.DuctBank.ToString()) {
                return direction == Parameters.Conduit ? Visibility.Visible : Visibility.Collapsed;
            }
            return direction == Parameters.Tray ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
