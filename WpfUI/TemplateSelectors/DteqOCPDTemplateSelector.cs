using EDTLibrary;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.TemplateSelectors;

public class DteqOCPDTemplateSelector : DataTemplateSelector
{
    public DataTemplate BreakerTemplate { get; set; }
    public DataTemplate FdsTemplate { get; set; }
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var selectedTemplate = BreakerTemplate;
        var pd = (IProtectionDevice)item;

        if (pd == null) return selectedTemplate;

        if (pd.Type == "BKR") {

            selectedTemplate = BreakerTemplate;
        }

        if (pd.Type == "FDS") {

            selectedTemplate = FdsTemplate;
        }

        return selectedTemplate;
    }
}
