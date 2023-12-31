﻿using EDTLibrary;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
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
    public class VoltageToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush(Colors.Transparent);

            IPowerConsumer load = new LoadModel();
            if (value.GetType() is DistributionEquipment) {
                load = DteqFactory.Recast(value);
            }
            else if (value.GetType() == typeof(LoadModel)) {
                load = (LoadModel)value;
            }

            if (load.VoltageType == null || load.FedFrom.LoadVoltageType == null) {
                return new SolidColorBrush(Colors.Transparent);
            }

            //Brush colors
            var bc = new BrushConverter();
            var alertBrush = (Brush)bc.ConvertFrom("#FFFFBABA"); //red
            var transparentBrush = new SolidColorBrush(Colors.Transparent);


            if (load.FedFrom.Type==DteqTypes.DPN.ToString()) {
                if (load.FedFrom.LoadVoltageType.Voltage == 208 && 
                    (load.VoltageType.Voltage == 208 || load.VoltageType.Voltage == 120)) {
                    return transparentBrush;
                }
                else if (load.FedFrom.LoadVoltageType.Voltage == 240 &&
                    (load.VoltageType.Voltage == 240 || load.VoltageType.Voltage == 120)) {
                    return transparentBrush;
                }
                else if (load.FedFrom.LoadVoltageType.Voltage == 120 && load.VoltageType.Voltage == 120) {
                    return transparentBrush;
                }
                else {
                    return alertBrush;
                }
            }

            if (load.Type == LoadTypes.MOTOR.ToString()) {
                if (load.FedFrom.LoadVoltageType.Voltage == 480 && load.VoltageType.Voltage != 460) {
                    return alertBrush;
                }
                else if (load.FedFrom.LoadVoltageType.Voltage == 600 && load.VoltageType.Voltage != 575) {
                    return alertBrush;
                }
            }

            else if (load.VoltageType.Voltage != load.FedFrom.LoadVoltageType.Voltage) {
                return alertBrush;
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
