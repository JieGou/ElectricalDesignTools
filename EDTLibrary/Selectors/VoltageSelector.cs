using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Selectors;
internal class VoltageSelector
{
    internal static void SetVoltage(ILoad load)
    {
        if (load.Type == LoadTypes.MOTOR.ToString())
        {
            load.VoltageType = load.VoltageType.Voltage == 4160 ? TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 4000) : load.VoltageType;
            load.VoltageType = load.VoltageType.Voltage == 600 ? TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 575) : load.VoltageType;
            load.VoltageType = load.VoltageType.Voltage == 480 ? TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 460) : load.VoltageType;
        }
    }
}


