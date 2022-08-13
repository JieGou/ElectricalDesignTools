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

namespace WpfUI.Converters
{
    public class WarningStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string errorContent = (string)value;
            

            if (errorContent.Contains("Temp")) {
                //return new SolidColorBrush(Colors.DarkOrange);
                return errorContent.Replace("Temp", "");
            }

            //return new SolidColorBrush(Colors.Red);
            return errorContent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
