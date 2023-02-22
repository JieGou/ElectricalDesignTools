using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Managers;
internal class ValidationManager
{

    internal static bool ValidateDteq(ref bool isValid, DistributionEquipment dteq)
    {
        if (dteq.PowerCable != null) {
            if (!dteq.PowerCable.IsValid) {
                isValid = false;
            }
        }
        if (dteq.ProtectionDevice != null) {
            if (!dteq.ProtectionDevice.IsValid) {
                isValid = false;
            }
        }
        ValidateLoadList(ref isValid, dteq.AssignedLoads);

        return isValid;


    }

    internal static bool ValidateLoadList(ref bool isValid, ObservableCollection<IPowerConsumer> loadList)
    {

        foreach (var load in loadList) {
            if (load.Category == Categories.DTEQ.ToString()) {
                continue;
            }
            if (!load.IsValid) {
                load.Validate();
                isValid = false;
            }
            if (load.ProtectionDevice != null) {
                load.ProtectionDevice.Validate();
                if (!load.ProtectionDevice.IsValid) {
                    isValid = false;
                }
            }
            if (load.PowerCable != null) {

                load.PowerCable.Validate(load.PowerCable);
                if (!load.PowerCable.IsValid) {
                    isValid = false;
                }
            }
            isValid = ValidateCctComponents(ref isValid, load);
        }

        return isValid;


    }

    internal static bool ValidateCctComponents(ref bool isValid, IPowerConsumer load)
    {

        foreach (var comp in load.CctComponents) {
            comp.Validate();
            if (!comp.IsValid) {
                isValid = false;
            }
            if (comp.PowerCable != null) {
                comp.PowerCable.Validate(comp.PowerCable);
                if (!comp.PowerCable.IsValid) {
                    isValid = false;
                }
            }
        }

        return isValid;
    }
}
