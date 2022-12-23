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

        IProtectionDevice pd = null;
        try {
            if (item.GetType() == typeof(ProtectionDeviceModel)) {
                pd = (ProtectionDeviceModel)item;
            }
        }
        catch (Exception) { }


        if (pd == null) {
            return selectedTemplate;
        }

        if (pd.IsStandAlone) {
            return EmptyTemplate;
        }

        //Splitter
        if (pd!=null) {
            if (pd.IsStandAlone == true) {
                if (pd.Type == "FDS") return FdsTemplate_StandAlone;
                if (pd.Type.Contains("MCP")) return DolTemplate_StandAlone;
            } 
        }

        if (pd.Type == "FDS") return FdsTemplate;
        if (pd.Type.Contains("MCP")) return DolTemplate;


        var pdOnwer = (IPowerConsumer)pd.Owner;

        if (pd.Type == "BKR" && pdOnwer.GetType() != typeof(XfrModel)) {
            return BreakerTemplate;
        }

        if (pd.Type == DteqTypes.DPN.ToString() && pdOnwer.Type == DteqTypes.XFR.ToString()) {
            return EmptyTemplate;
        }
        return selectedTemplate;
    }
}
