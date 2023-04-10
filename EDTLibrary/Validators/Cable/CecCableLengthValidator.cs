using EDTLibrary.Models.Cables;
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

namespace EDTLibrary.Validators.Cable;
internal class CecCableLengthValidator : ICableLengthValidator
{
    public (bool, double) IsCableLengthValid(ICable cable, double length, bool sendAlerts = false)
    {
        if (cable == null) return (false, 1);

        bool isValid = true;
        double returnLength = length;
        string message = "";
        string caption = "";


        IDteq load = null;

        //14-100  c) & b)
        if (cable.DestinationModel is ProtectionDeviceModel) {
            var pd = (ProtectionDeviceModel)cable.DestinationModel;
            if (pd.IsStandAlone) {

                //14-100 b) pass
                if (length < 3) {
                    returnLength = length;
                }

                //14-100 c) fail - 7.5m, 1/3 rule
                if (cable.DeratedAmps * 3 >= cable.Load.FedFrom.PowerCable.DeratedAmps) {
                    if (length > 7.5) {
                        message = CecCableLengthValidationMessages.Rule14_100_C;
                        caption = CecCableLengthValidationMessages.Rule14_100_C_Caption;
                        isValid = false;
                        returnLength = 7.5;
                    }
                }

                //14-100 b) fail - 3m rule
                else if (length > 3) {
                   
                    message = CecCableLengthValidationMessages.Rule14_100_B;
                    caption = CecCableLengthValidationMessages.Rule14_100_B_Caption;
                    isValid = false;
                    returnLength = 3;
                }
            }
        }

        if (cable.Load is IDteq) {
            load = DteqFactory.Recast(cable.Load);
            if (cable.Load.FedFrom.GetType() == typeof(XfrModel)) {

                //14-100 d
                if (cable.Load.FedFrom.LineVoltageType.Voltage > 750) {
                    if (length > 7.5) {
                       
                        message = CecCableLengthValidationMessages.Rule14_100_D;
                        caption = CecCableLengthValidationMessages.Rule14_100_D_Caption;
                        isValid = false;
                        returnLength = 7.5;
                    }
                }
                //14-100 b) fail - 3m rule
                else if (length > 3) {

                    message = CecCableLengthValidationMessages.Rule14_100_B;
                    caption = CecCableLengthValidationMessages.Rule14_100_B_Caption;
                    isValid = false;
                    returnLength = 3;
                }
            }

        }

        if (isValid == false) {

            if (sendAlerts == true) {
                EdtNotificationService.SendAlert(cable.Tag, message, caption);
            }

            // App Setting for auto-correct cable lengths
            if (true) {
                cable.InvalidLengthMessage = message;
                cable.SetCableInvalid(cable);
                return (isValid, length);

            }

        }
        return (isValid, returnLength);
    }
}

