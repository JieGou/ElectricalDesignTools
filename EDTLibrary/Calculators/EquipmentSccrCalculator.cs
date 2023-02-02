using EDTLibrary.LibraryData;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Calculators;
internal class EquipmentSccrCalculator
{
    internal static double GetMinimumSccr(IPowerConsumer load)
    {
        var validBreakerAicRatings = TypeManager.EquipmentSccrValues.Where(b => b.SCCR >= load.SCCA);
        return validBreakerAicRatings.Min(b => b.SCCR);
    }
}
