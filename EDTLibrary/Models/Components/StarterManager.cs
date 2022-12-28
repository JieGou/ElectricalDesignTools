using EDTLibrary.LibraryData;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace EDTLibrary.Models.Components;
internal class StarterManager
{
    public static string SelectStarterType(ILoad load)
    {

        if (load.FedFrom.Type == DteqTypes.DPN.ToString() || load.FedFrom.Type == DteqTypes.CDP.ToString()) {
            return CctComponentTypes.DOL.ToString();
        }

        return ComponentTypes.VFD.ToString() ;
    }

}
