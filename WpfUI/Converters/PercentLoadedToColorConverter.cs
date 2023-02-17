using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
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
    public class PercentLoadedToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = Double.Parse(value.ToString());
            double percentage = Double.Parse(EdtProjectSettings.DteqMaxPercentLoaded.ToString());
            var bc = new BrushConverter();

            //yel
            if (val >= percentage + 10) {
                return (Brush)bc.ConvertFrom("#FFFFBABA");
            }
            else if (val >= percentage + 5) {
                return (Brush)bc.ConvertFrom("#FFF9B173");
            }

            else if (val >= percentage) {
                //return new SolidColorBrush(Colors.Yellow);
                return (Brush)bc.ConvertFrom("#FFFFED71");
            }

                return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
