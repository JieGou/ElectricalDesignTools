using EDTLibrary;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.TemplateSelectors;
public class LoadGraphicTemplateSelector : DataTemplateSelector
{
    public DataTemplate MotorTemplate { get; set; }
    public DataTemplate HeaterTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var selectedTemplate = HeaterTemplate;
        if (item == null) return MotorTemplate;

        var load = (LoadModel)item;

        if (load.Type == LoadTypes.MOTOR.ToString()) {

            selectedTemplate = MotorTemplate;
        }

        else if (load.Type == LoadTypes.HEATER.ToString()) {
            selectedTemplate = HeaterTemplate;
        }

        return selectedTemplate;
    }
}
