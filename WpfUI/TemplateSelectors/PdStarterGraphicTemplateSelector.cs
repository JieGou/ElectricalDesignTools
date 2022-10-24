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
public class PdStarterGraphicTemplateSelector : DataTemplateSelector
{
    public DataTemplate EmptyTemplate { get; set; }
    public DataTemplate BreakerTemplate { get; set; }
    public DataTemplate FvnrTemplate { get; set; }
    public DataTemplate FvrTemplate { get; set; }
    public DataTemplate FdsTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {

        var selectedTemplate = EmptyTemplate;
        if (item == null) return EmptyTemplate;

        IPowerConsumer load = null;
        try {
            if (item.GetType() == typeof(LoadModel)) {
                load = (LoadModel)item;
            }
            else if (item is (DistributionEquipment)) {
                load = DteqFactory.Recast(item);
            }

        }
        catch (Exception) { }


        if (load == null) {
            return selectedTemplate;
        }

        if (load.PdType == "BKR" && load.FedFrom.GetType() != typeof(XfrModel)) {
            selectedTemplate = BreakerTemplate; 
        }

        if (load.Type == DteqTypes.DPN.ToString() && load.FedFrom.Type== DteqTypes.XFR.ToString()) {
            selectedTemplate = EmptyTemplate;
        }

        if (load.PdType.Contains("FVNR"))  selectedTemplate = FvnrTemplate;

        if (load.PdType.Contains("FVR")) selectedTemplate = FvnrTemplate;
        
        if (load.PdType == "FDS")  selectedTemplate = FdsTemplate;
        

        return selectedTemplate;
    }
}
