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
    public class PercentLoadedToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = Double.Parse(value.ToString());
            double per = Double.Parse(EdtSettings.DteqMaxPercentLoaded.ToString());
            var bc = new BrushConverter();

            //yel
            if (val >= per + 10) {
                return (Brush)bc.ConvertFrom("#FFFFBABA");
            }
            else if (val >= per + 5) {
                return (Brush)bc.ConvertFrom("#FFF9B173");
            }

            else if (val >= per) {
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
