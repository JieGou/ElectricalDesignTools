using EDTLibrary;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.TemplateSelectors;
public class ComponentGraphicTemplateSelector : DataTemplateSelector
{
    public DataTemplate UdsTemplate { get; set; }
    public DataTemplate FdsTemplate { get; set; }
    public DataTemplate VsdTemplate { get; set; }
    public DataTemplate RvsTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var selectedTemplate = UdsTemplate;
        var component = (IComponent)item;
        if (component == null) return selectedTemplate;

        if (component.Type == ComponentTypes.UDS.ToString()) {

            selectedTemplate = UdsTemplate;
        }
        else if (component.Type == ComponentTypes.FDS.ToString()) {

            selectedTemplate = FdsTemplate;
        }

        else if (component.Type == ComponentTypes.VFD.ToString()  || component.Type == ComponentTypes.VSD.ToString()) {
            selectedTemplate = VsdTemplate;
        }
        else if (component.Type == ComponentTypes.RVS.ToString()) {
            selectedTemplate = RvsTemplate;
        }

        return selectedTemplate;
    }
}
