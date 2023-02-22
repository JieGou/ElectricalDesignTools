using EDTLibrary.LibraryData;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.Loads;
using EDTLibrary.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Calculators;
internal class EquipmentSccrCalculator
{
    internal static double GetMinimumAicRating(IComponentEdt pd)
    {
        if (true) {
            var validSccrValues = TypeManager.BreakerAicRatings.Where(b => b.kAicRating >= pd.SCCA);
            return validSccrValues.Min(b => b.kAicRating);
        }
    }

    internal static double GetMinimumSccr(IPowerConsumer load)
    {
        if (EdtAppSettings.Default.AutoSize_SCCR) {
            var validSccrValues = TypeManager.EquipmentSccrValues.Where(b => b.SCCR >= load.SCCA);
            return validSccrValues.Min(b => b.SCCR); 
        }
        return load.SCCR;
    }

    internal static double GetMinimumSccr(IComponentEdt comp)
    {
        if (EdtAppSettings.Default.AutoSize_SCCR) {
            var validSccrValues = TypeManager.EquipmentSccrValues.Where(b => b.SCCR >= comp.SCCA);
            return validSccrValues.Min(b => b.SCCR); 
        }
        return comp.SCCR;

    }
}
