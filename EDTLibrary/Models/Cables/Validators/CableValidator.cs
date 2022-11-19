using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables.Validators;
internal class CableValidator
{
    internal static ICableLengthValidator CableLengthValidator { get; set; }

    public static bool IsCableLengthValid(ICable cable, double length)
    {
        return CableLengthValidator.IsCableLengthValid(cable, length);
    }

}
