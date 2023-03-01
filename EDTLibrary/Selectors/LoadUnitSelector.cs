using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Selectors;
internal class LoadUnitSelector
{
    internal static void SetUnit(ILoad load)
    {
        if (load.Type == LoadTypes.HEATER.ToString())
        {
            load.Unit = Units.kW.ToString();
        }
        else if (load.Type == LoadTypes.MOTOR.ToString())
        {
            var unit = Units.HP.ToString();
            load.Unit = Units.HP.ToString();
        }
        else if (load.Type == LoadTypes.PANEL.ToString())
        {
            load.Unit = Units.A.ToString();
        }
        else if (load.Type == LoadTypes.WELDING.ToString())
        {
            load.Unit = Units.A.ToString();
        }
        else if (load.Type == LoadTypes.OTHER.ToString())
        {
            load.Unit = Units.A.ToString();
        }
        else load.Unit = Units.A.ToString();
    }

    internal static List<string> GetUnitList(IPowerConsumer load)
    {

        var unitList = new List<string>();
        if (load.Type == LoadTypes.MOTOR.ToString()) {
            unitList.Add(Units.HP.ToString());
            unitList.Add(Units.kW.ToString());
        }
        else if (load.Type == LoadTypes.HEATER.ToString()) {
            unitList.Add(Units.kW.ToString());
        }
        else if (load.Type == LoadTypes.PANEL.ToString()) {
            unitList.Add(Units.A.ToString());
            unitList.Add(Units.kW.ToString());
        }
        else if (load.Type == LoadTypes.WELDING.ToString()) {
            unitList.Add(Units.A.ToString());
        }
        else if (load.Type == LoadTypes.OTHER.ToString()) {
            unitList.Add(Units.A.ToString());
            unitList.Add(Units.kW.ToString());
            unitList.Add(Units.HP.ToString());
            unitList.Add(Units.kVA.ToString());
        }
        return unitList;
    }
}


