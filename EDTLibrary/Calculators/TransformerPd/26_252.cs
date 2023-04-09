using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.Calculators.TransformerPd;
public class _26_252
{
    private const double MAX_PRIMARY_PROTECTION = 1.5; // 150%
    private const double MAX_SECONDARY_PROTECTION = 1.25; // 125%
    private const double MAX_SMALL_PRIMARY_PROTECTION = 1.67; // 167%
    private const double MAX_SMALL_PRIMARY_PROTECTION_SPECIAL = 3; // 300%
    private const double MAX_FEEDER_PROTECTION = 3; // 300%
    private const double MAX_PRIMARY_COORDINATED_PROTECTION_7P5 = 6;
    private const double MAX_PRIMARY_COORDINATED_PROTECTION_10 = 4;

    public double CalculatePrimaryProtection(double ratedPrimaryCurrent, double ratedSecondaryCurrent, double impedance)
    {
        double primaryProtection = 0;

        if (ratedSecondaryCurrent > 0 && ratedSecondaryCurrent <= 9) {
            primaryProtection = ratedSecondaryCurrent * MAX_SMALL_PRIMARY_PROTECTION_SPECIAL;
        }
        else if (ratedSecondaryCurrent > 9 && ratedSecondaryCurrent * MAX_SECONDARY_PROTECTION > 0) {
            double maxSecondaryProtection = GetMaxSecondaryProtection(ratedSecondaryCurrent);
            primaryProtection = Math.Min(ratedPrimaryCurrent * MAX_PRIMARY_PROTECTION, maxSecondaryProtection * MAX_FEEDER_PROTECTION);
        }

        if (primaryProtection == 0 && ratedPrimaryCurrent <= 9) {
            primaryProtection = ratedPrimaryCurrent * MAX_SMALL_PRIMARY_PROTECTION;
        }

        if (primaryProtection == 0) {
            primaryProtection = ratedPrimaryCurrent * MAX_PRIMARY_PROTECTION;
        }

        if (impedance > 0 && impedance <= 7.5) {
            primaryProtection = Math.Min(primaryProtection, ratedPrimaryCurrent * MAX_PRIMARY_COORDINATED_PROTECTION_7P5);
        }
        else if (impedance > 7.5 && impedance <= 10) {
            primaryProtection = Math.Min(primaryProtection, ratedPrimaryCurrent * MAX_PRIMARY_COORDINATED_PROTECTION_10);
        }

        return primaryProtection;
    }

    private double GetMaxSecondaryProtection(double ratedSecondaryCurrent)
    {
        double maxSecondaryProtection = ratedSecondaryCurrent * MAX_SECONDARY_PROTECTION;

        if (maxSecondaryProtection <= 9) {
            maxSecondaryProtection = ratedSecondaryCurrent * MAX_SMALL_PRIMARY_PROTECTION_SPECIAL;
        }
        else {
            maxSecondaryProtection = Math.Ceiling(maxSecondaryProtection);
        }

        return maxSecondaryProtection;
    }
}

