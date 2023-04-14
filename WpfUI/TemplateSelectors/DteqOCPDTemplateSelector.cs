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
    public DataTemplate MvBreakerTemplate { get; set; }
    public DataTemplate FdsTemplate { get; set; }
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var selectedTemplate = BreakerTemplate;
        if (item == null) return selectedTemplate;

        var pd = (IProtectionDevice)item;
        var dteq = (IPowerConsumer)pd.Owner;

        if (pd == null) return selectedTemplate;

        if (pd.Type == "Breaker" && dteq.Voltage>=750) {

            return MvBreakerTemplate;
        }

        if (pd.Type == "Breaker") {

            return BreakerTemplate;
        }

        if (pd.Type == "FDS") {

            return FdsTemplate;
        }
        return selectedTemplate;
    }
}
