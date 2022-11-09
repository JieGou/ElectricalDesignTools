using EDTLibrary;
using EDTLibrary.Models.Equipment;
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
    public class EquipmentTypeToDashedBorderVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            var eq = (IEquipment)value;

            if (eq.Type == DteqTypes.SWG.ToString() || 
                
                eq.Type == DteqTypes.CDP.ToString() ||
                eq.Type == DteqTypes.DPN.ToString()) {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
       
    }
}
