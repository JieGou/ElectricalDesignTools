using EDTLibrary.Models.Components.ProtectionDevices;
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
internal class CecCableLengthValidator : ICableLengthValidator
{
    public (bool, double) IsCableLengthValid(ICable cable, double length, bool sendAlerts = false)
    {
        if (cable == null) return (false, 1);

        IDteq load = null;

        //14-100  c) & b)
        if (cable.DestinationModel is ProtectionDeviceModel) {
            var pd = (ProtectionDeviceModel)cable.DestinationModel;
            if (pd.IsStandAlone) {

                //14-100 b) pass
                if (length < 3) {
                    return (true, cable.Length);
                }

                //14-100 c) fail - 7.5m, 1/3 rule
                if (cable.DeratedAmps * 3 >= cable.Load.FedFrom.PowerCable.DeratedAmps) {
                    if (length > 7.5) {
                        if (sendAlerts == true) {
                            EdtNotificationService.SendAlert(cable.Load.Tag,
                                               "Maximum conductor length for tap conductors where the smaller conductoris 7.5m" + "\n\n" +
                                               "All other items of Rule 14-100(b) must also be considered.",
                                               "Cable length violation - CEC Rule 14-100(c).");
                        }
                        return (false, 7.5);
                    }
                }

                //14-100 b) fail - 3m rule
                else if (length > 3) {
                    if (sendAlerts == true) {
                        EdtNotificationService.SendAlert(cable.Load.Tag,
                        "Maximum conductor length for smaller tap conductors is 3m" + "\n\n" +
                        "All other items of Rule 14-100(b) must also be considered.",
                        "Cable length violation - CEC Rule 14-100(b).");
                    }
                    return (false, 3);
                }
            }
        }

        //14-100 d
        if (cable.Load is IDteq) {
            load = DteqFactory.Recast(cable.Load);
            if (cable.Load.FedFrom.GetType() == typeof(XfrModel)) {
                if (cable.Load.FedFrom.LineVoltageType.Voltage > 750) {
                    if (length > 7.5) {
                        if (sendAlerts == true) {
                            EdtNotificationService.SendAlert(cable.Load.Tag,
                            "Maximum conductor length for equipment fed from tranformers rated over 750V without secondary protection is 7.5 meters. " + "\n\n" +
                            "An OCPD must be added to protect the transformer secondary conductor for its length to be more than 7.5 meters." + "\n\n" +
                            "All other items of Rule 14-100(d) must also be considered.",
                            "Cable length violation - CEC Rule 14-100(d).");
                        }
                        return (false, 7.5);
                    }
                }
                //14-100 b) fail - 3m rule
                else if (length > 3) {
                    if (sendAlerts == true) {
                        EdtNotificationService.SendAlert(cable.Load.Tag,
                        "Maximum conductor length for smaller tap conductors is 3m" + "\n\n" +
                        "All other items of Rule 14-100(c) must also be considered.",
                        "Cable length violation - CEC Rule 14-100(b).");
                    }
                    return (false, 3);
                }
            }

        }




        return (true, cable.Length);
    }
}
