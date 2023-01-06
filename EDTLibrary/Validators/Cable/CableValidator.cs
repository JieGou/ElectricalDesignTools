using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Validators.Cable;
internal class CableValidator
{
    internal static ICableLengthValidator CableLengthValidator { get; set; }
    internal static ICableAmpacityValidator CableSizeValidator { get; set; }
    internal static ICableVoltageDropValidator CableVoltageDropValidator { get; set; }

    public static (bool, double) IsCableLengthValid(ICable cable, double length, bool sendAlerts = false)
    {
        return CableLengthValidator.IsCableLengthValid(cable, length, sendAlerts);
    }

}
