using EDTLibrary;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
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
    public class PowerConsumerTypeVisbilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Hidden;

            IPowerConsumer powerConsumer;
            if (value is IPowerConsumer) {
                powerConsumer = (IPowerConsumer)value;
            }
            else {
                return Visibility.Hidden;
            }



            string type = parameter.ToString();

            if (type == Categories.DTEQ.ToString()) {
                return (powerConsumer is DistributionEquipment) ? Visibility.Visible : (object)Visibility.Hidden;
            }
            return (powerConsumer is ILoad) ? Visibility.Visible : (object)Visibility.Hidden;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
       
    }
}
