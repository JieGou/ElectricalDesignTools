using EDTLibrary;
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
        var dteq = (DistributionEquipment)item;
        if (dteq == null) return selectedTemplate;

        if (dteq.PdType == "BKR") {

            selectedTemplate = BreakerTemplate;
        }

        if (dteq.PdType == "FDS") {

            selectedTemplate = FdsTemplate;
        }

        return selectedTemplate;
    }
}
