using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Selectors;
internal class LoadUnitSelector
{
    internal static void SelectUnits(ILoad load)
    {
        if (load.Type == LoadTypes.HEATER.ToString()) {
            load.Unit = Units.kW.ToString();
        }
        else if (load.Type == LoadTypes.MOTOR.ToString()) {
            var unit = Units.HP.ToString();
            load.Unit = Units.HP.ToString();
        }
        else if (load.Type == LoadTypes.PANEL.ToString()) {
            load.Unit = Units.A.ToString();
        }
        else if (load.Type == LoadTypes.WELDING.ToString()) {
            load.Unit = Units.A.ToString();
        }
        else if (load.Type == LoadTypes.OTHER.ToString()) {
            load.Unit = Units.A.ToString();
        }
        else load.Unit = Units.A.ToString();
    }
}
