using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Selectors;
internal class ComponentTypeSelector
{
    internal static List<string> GetComponentTypeList(ComponentModelBase comp)
    {

        var list = new List<string>();

        if (comp.SubCategory == CctCompSubCategories.Disconnect.ToString()) {
            list.Add(DisconnectTypes.UDS.ToString());
            list.Add(DisconnectTypes.FDS.ToString());
        }
        else if (comp.SubCategory == CctCompSubCategories.Starter.ToString()) {
            //list.Add(StarterTypes.MCP_FVNR.ToString());
            //list.Add(StarterTypes.MCP_FVR.ToString());
            list.Add(StarterTypes.VSD.ToString());
            list.Add(StarterTypes.VFD.ToString());
            list.Add(StarterTypes.RVS.ToString());
        }

        return list;
    }
}
