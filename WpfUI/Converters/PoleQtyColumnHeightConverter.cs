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
    public class PoleQtyColumnHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string poles = value.ToString();
            

            if (poles == "2") {
                return "\n2\n";
            }
            else if (poles == "3") {
                return "\n\n3\n\n";
            }
            //return new SolidColorBrush(Colors.Red);
            return poles;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
