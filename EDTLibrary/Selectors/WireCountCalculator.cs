using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.Selectors;
internal class WireCountSelector
{
    internal static int GetWireCount(IPowerConsumer eq)
    {
        int wireCount = 0;
        if (eq.VoltageType.Voltage == 208 && eq.VoltageType.Phase == 3)
        {
            wireCount = 4;
        }
        else if (eq.VoltageType.Voltage == 208 && eq.VoltageType.Phase == 1)
        {
            wireCount = 3;
        }
        else if (eq.VoltageType.Voltage == 240)
        {
            wireCount = 3;
        }
        else if (eq.VoltageType.Voltage == 120)
        {
            wireCount = 2;
        }
        else {
            wireCount = 3;
        }
        return wireCount;
    }
}

