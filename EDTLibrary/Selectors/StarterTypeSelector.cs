using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace EDTLibrary.Selectors;
internal class StarterTypeSelector
{
    public static string SelectStarterType(ILoad load)
    {

        if (load.FedFrom.Type == DteqTypes.DPN.ToString() || load.FedFrom.Type == DteqTypes.CDP.ToString())
        {
            return EdtProjectSettings.LoadDefaultPdTypeLV_Motor;
        }

        return CctComponentTypes.VFD.ToString();
    }
}
