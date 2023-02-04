using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Selectors;
internal class DemandFactorSelector
{
    public static void SetDemandFactor(ILoad load)
    {
        
        if (load.Type == LoadTypes.HEATER.ToString()) {
            load.DemandFactor = double.Parse(EdtSettings.DemandFactorDefault_Heater);
        }
        else if (load.Type == LoadTypes.PANEL.ToString()) {
            load.DemandFactor = double.Parse(EdtSettings.DemandFactorDefault_Panel);
        }
        else if (load.Type == LoadTypes.OTHER.ToString()) {
            load.DemandFactor = double.Parse(EdtSettings.DemandFactorDefault_Other);
        }
        else if (load.Type == LoadTypes.WELDING.ToString()) {
            load.DemandFactor = double.Parse(EdtSettings.DemandFactorDefault_Welding);
        }
        else {
            load.DemandFactor = double.Parse(EdtSettings.DemandFactorDefault);

        }
    }

    public static double GetDemandFactor(string loadType)
    {

        if (loadType == LoadTypes.HEATER.ToString()) {
            return double.Parse(EdtSettings.DemandFactorDefault_Heater);
        }
        else if (loadType == LoadTypes.PANEL.ToString()) {
            return double.Parse(EdtSettings.DemandFactorDefault_Panel);
        }
        else if (loadType == LoadTypes.OTHER.ToString()) {
            return double.Parse(EdtSettings.DemandFactorDefault_Other);
        }
        else if (loadType == LoadTypes.WELDING.ToString()) {
            return double.Parse(EdtSettings.DemandFactorDefault_Welding);
        }
        else {
            return double.Parse(EdtSettings.DemandFactorDefault);

        }
    }
}
