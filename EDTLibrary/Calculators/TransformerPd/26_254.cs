using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media;

namespace EdtLibrary.Calculators.TransformerPd;
internal class _26_254
{
    private double ratedPrimaryCurrent;
    private double ratedSecondaryCurrent;

    public _26_254(double ratedPrimaryCurrent, double ratedSecondaryCurrent)
    {
        this.ratedPrimaryCurrent = ratedPrimaryCurrent;
        this.ratedSecondaryCurrent = ratedSecondaryCurrent;
    }

    public double CalculateOvercurrentProtectionSize()
    {
        double primaryOvercurrentProtectionSize = 1.25 * ratedPrimaryCurrent;

        if (ratedSecondaryCurrent <= 0.8 * primaryOvercurrentProtectionSize) {
            return primaryOvercurrentProtectionSize;
        }

        double secondaryOvercurrentProtectionSize = 1.25 * ratedSecondaryCurrent;
        double primaryFeederOvercurrentProtectionSize = 3.0 * ratedPrimaryCurrent;

        if (secondaryOvercurrentProtectionSize <= 1.25 * primaryFeederOvercurrentProtectionSize) {
            return secondaryOvercurrentProtectionSize;
        }

        double standardRating = 1.25 * ratedPrimaryCurrent;

        while (standardRating < primaryOvercurrentProtectionSize) {
            standardRating += 1.0;
        }

        return standardRating;
    }
}


