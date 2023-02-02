using EDTLibrary.LibraryData;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Calculators;
internal class ProtectionDeviceAicCalculator
{
    internal static double GetMinimumBreakerAicRating(IPowerConsumer load)
    {
        var validBreakerAicRatings = TypeManager.BreakerAicRatings.Where(b => b.kAicRating >= load.SCCA);
        return validBreakerAicRatings.Min(b => b.kAicRating);
    }
}
