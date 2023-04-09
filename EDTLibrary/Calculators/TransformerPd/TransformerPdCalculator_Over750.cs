using EDTLibrary.LibraryData;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.Calculators.TransformerPd;
/// <summary>
/// Overcurrent protection for power and distribution transformer circuits rated over 750 V
/// Rule 26-250
/// </summary>
internal class TransformerPdCalculator_Over750
{
    private const double StandardRatingFactor = 1.25;

    private const double FuseRatingFactor = 1.5;
    private const double BreakerRatingFactor = 3.0;

    public double CalculateOvercurrentProtectionSize(XfrModel xfr)
    {
        return DataTableSearcher.GetBreakerTrip(xfr.Fla * StandardRatingFactor);
    }

    private bool IsStandardRating(double rating)
    {
        // Assume that standard ratings are multiples of 5
        return Math.Abs(Math.Round(rating) - rating) < 0.0001 && (rating % 5 == 0);
    }

    private double GetNextStandardRating(double rating)
    {
        // Assume that standard ratings are multiples of 5
        double nextStandardRating = Math.Ceiling(rating / 5) * 5;
        return nextStandardRating;
    }
}
