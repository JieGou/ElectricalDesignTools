using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Validators.Cable;
internal interface ICableLengthValidator
{
    (bool, double) IsCableLengthValid(ICable cable, double length, bool sendAlerts);
}
