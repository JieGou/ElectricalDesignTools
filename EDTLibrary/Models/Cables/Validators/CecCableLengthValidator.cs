using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DPanels;
using EDTLibrary.Models.Loads;
using EDTLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables.Validators;
internal class CecCableLengthValidator: ICableLengthValidator
{
    public bool IsCableLengthValid (ICable cable, double length)
    {
        if (cable == null) return false;

        IDteq load = null;

        if (cable.Load is IDteq) {
            load = DteqFactory.Recast(cable.Load);
            if (cable.Load.FedFrom.GetType() == typeof(XfrModel)) {
                if (length > 7.5) {
                    EdtNotificationService.SendAlert(cable.Load.Tag, 
                        "Maximum cable length for Transformer-Fed equimpent without secondary protection is 7.5 meters. " + "\n\n" +
                        "An OCPD must be added to protect the transformer secondary cable for its length to be more than 7.5 meters." + "\n\n" +
                        "All other items of Rule 14-100(d) must also be considered." , 
                        "Cable length violation - CEC Rule 14-100(d).");
                    //_length = oldValue;
                    return false;
                }
            }
        }
        
        return true;
    }
}
