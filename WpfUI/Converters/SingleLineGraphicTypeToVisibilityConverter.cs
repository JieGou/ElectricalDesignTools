using EDTLibrary;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DPanels;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
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
    public class SingleLineGraphicTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string level = (string)parameter;
            IPowerConsumer load = null;

            if (value == null) {
                return Visibility.Collapsed;
            }

            if (value is DistributionEquipment) {
                load = DteqFactory.Recast(value);
            }
            else if (value.GetType() == typeof(LoadModel)) {
                load = (LoadModel)value;
            }

            switch (level) {

                case "Load":
                    if (load.GetType() == typeof(LoadModel)) {
                        return Visibility.Visible;
                    }
                    break;

                case "DteqAsLoad":
                    if (load is DistributionEquipment) {
                        return Visibility.Visible;
                    }

                    break;

                case "Dteq_Load":
                    if (load != null) {
                        if (load.FedFrom.Type == DteqTypes.XFR.ToString()) {
                            return Visibility.Visible;
                        } 
                    }
                    break;

            }


            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
       
    }
}
