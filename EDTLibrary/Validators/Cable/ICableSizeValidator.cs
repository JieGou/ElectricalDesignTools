using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Validators.Cable;
internal interface ICableAmpacityValidator
{
    void IsCableAmpacityValid(ICable cable);
}
