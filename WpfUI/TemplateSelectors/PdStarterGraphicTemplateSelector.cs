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
    public DataTemplate DolTemplate { get; set; }
    public DataTemplate FdsTemplate { get; set; }


    public DataTemplate FdsTemplate_StandAlone { get; set; }
    public DataTemplate DolTemplate_StandAlone { get; set; }

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

        //Splitter
        if (load.ProtectionDevice.IsStandAlone== true) {
            if (load.PdType == "FDS") return FdsTemplate_StandAlone;
            if (load.PdType.Contains("MCP")) return DolTemplate_StandAlone;
        }

        if (load.PdType == "FDS") return FdsTemplate;
        if (load.PdType.Contains("MCP")) return DolTemplate;

        if (load.PdType == "BKR" && load.FedFrom.GetType() != typeof(XfrModel)) {
            return BreakerTemplate;
        }

        if (load.Type == DteqTypes.DPN.ToString() && load.FedFrom.Type == DteqTypes.XFR.ToString()) {
            return EmptyTemplate;
        }
        return selectedTemplate;
    }
}
