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
public class PdStarterGraphicTemplateSelector : DataTemplateSelector
{
    public DataTemplate BreakerTemplate { get; set; }
    public DataTemplate FvnrTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var selectedTemplate = BreakerTemplate;
        var load = (LoadModel)item;
        if (load == null) return selectedTemplate;

        if (load.PdType.Contains("MCP")) {

            selectedTemplate = FvnrTemplate;
        }

        if (load.PdType == "BKR") {

            selectedTemplate = BreakerTemplate;
        }

        return selectedTemplate;
    }
}
