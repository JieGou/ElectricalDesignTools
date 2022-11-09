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
public class DteqGraphicTemplateSelector : DataTemplateSelector
{
    public DataTemplate TransformerTemplate { get; set; }
    public DataTemplate SwgTemplate { get; set; }
    public DataTemplate MccTemplate { get; set; }
    public DataTemplate SplitterTemplate { get; set; }
    public DataTemplate CdpTemplate { get; set; }
    public DataTemplate DpnTemplate { get; set; }
    public DataTemplate OtherTemplate { get; set; }

    
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var selectedTemplate = OtherTemplate;

        IPowerConsumer load = null;
        if (item != null) {
            try {
                if (item.GetType() == typeof(LoadModel)) {
                    load = (LoadModel)item;
                }
                else {
                    load = DteqFactory.Recast(item);
                }
            }
            catch (Exception) { }
        }

        if (load == null) return selectedTemplate;

        if (load.Type == DteqTypes.XFR.ToString()) {

            selectedTemplate = TransformerTemplate;
        }

        else if (load.Type == DteqTypes.SWG.ToString()) {
            selectedTemplate = SwgTemplate;
        }
        else if (load.Type == DteqTypes.MCC.ToString()) {
            selectedTemplate = MccTemplate;
        }
        
        else if (load.Type == DteqTypes.SPL.ToString()) {
            selectedTemplate = SplitterTemplate;
        }

        else if (load.Type == DteqTypes.CDP.ToString()) {
            selectedTemplate = CdpTemplate;
        }

        else if (load.Type == DteqTypes.DPN.ToString() || load.Type == DteqTypes.DPN.ToString()) {
            selectedTemplate = DpnTemplate;
        }

        return selectedTemplate;
    }
}
