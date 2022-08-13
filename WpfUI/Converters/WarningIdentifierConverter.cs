using EDTLibrary.Models.Cables;
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
using static Syncfusion.Windows.Controls.SfNavigator;

namespace WpfUI.Converters
{
    public class WarningIdentifierConverter : IValueConverter
    {
        enum Parameters
        {
            Error, Warning
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string errorContent = (string)value;
            var type = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);


            if (type == Parameters.Warning)
                return errorContent.Contains("Temp") ? Visibility.Visible : Visibility.Collapsed;

            return errorContent.Contains("Temp") ? Visibility.Collapsed : Visibility.Visible;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
