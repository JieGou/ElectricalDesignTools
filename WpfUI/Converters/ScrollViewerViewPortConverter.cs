using ControlzEx.Standard;
using EDTLibrary.Models.Cables;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WpfUI.Extension_Methods;

namespace WpfUI.Converters
{
    public class ScrollViewerViewPortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListView listview = (ListView)value;

            ScrollViewer sv = listview.GetChildOfType<ScrollViewer>();
            var width = sv.ViewportWidth + sv.ScrollableWidth;

            return width;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        
    }

}
