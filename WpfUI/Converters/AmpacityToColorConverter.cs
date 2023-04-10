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
    public class AmpacityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CableModel cable = (CableModel)value;
            var bc = new BrushConverter();

            if (cable.DeratedAmps < cable.RequiredAmps) {
                return (Brush)bc.ConvertFrom("#FFD40000");
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
