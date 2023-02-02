using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Selectors;
internal class LoadFactorSelector
{
    public static void SetLoadFactor(ILoad load)
    {
        
        if (load.Type == LoadTypes.HEATER.ToString()) {
            load.LoadFactor = double.Parse(EdtSettings.LoadFactorDefault_Heater);
        }
        else if (load.Type == LoadTypes.PANEL.ToString()) {
            load.LoadFactor = double.Parse(EdtSettings.LoadFactorDefault_Panel);
        }
        else if (load.Type == LoadTypes.OTHER.ToString()) {
            load.LoadFactor = double.Parse(EdtSettings.LoadFactorDefault_Other);
        }
        else if (load.Type == LoadTypes.WELDING.ToString()) {
            load.LoadFactor = double.Parse(EdtSettings.LoadFactorDefault_Welding);
        }
        else {
            load.LoadFactor = double.Parse(EdtSettings.LoadFactorDefault);

        }
    }

    public static double GetLoadFactor(string loadType)
    {

        if (loadType == LoadTypes.HEATER.ToString()) {
            return double.Parse(EdtSettings.LoadFactorDefault_Heater);
        }
        else if (loadType == LoadTypes.PANEL.ToString()) {
            return double.Parse(EdtSettings.LoadFactorDefault_Panel);
        }
        else if (loadType == LoadTypes.OTHER.ToString()) {
            return double.Parse(EdtSettings.LoadFactorDefault_Other);
        }
        else if (loadType == LoadTypes.WELDING.ToString()) {
            return double.Parse(EdtSettings.LoadFactorDefault_Welding);
        }
        else {
            return double.Parse(EdtSettings.LoadFactorDefault);

        }
    }
}
