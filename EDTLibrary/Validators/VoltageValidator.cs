using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDTLibrary.Models.DistributionEquipment;
using System.Reflection.Metadata.Ecma335;

namespace EDTLibrary.Selectors;
internal class VoltageValidator
{
    internal static bool IsValid(IPowerConsumer load)
    {
        if (load is LoadModel) {
            return ValidateLoadVoltage(load);
        }
        else if (load is DistributionEquipment) {
            return ValidateDteqVoltage(load);
        }
        return false;
    }

    private static bool ValidateLoadVoltage(IPowerConsumer load)
    {
        var fedFromVoltage = load.FedFrom.LoadVoltageType;
        if (load.Type == LoadTypes.MOTOR.ToString()) {
            if (fedFromVoltage.Voltage == 480 && load.VoltageType.Voltage == 460) {
                return true;
            }
            if (fedFromVoltage.Voltage == 600 && load.VoltageType.Voltage == 575) {
                return true;
            }
            if (fedFromVoltage.Voltage == 4160 && load.VoltageType.Voltage == 4000) {
                return true;
            }
        }
        if (load.VoltageType == fedFromVoltage) {
            return true;
        }

        return false;
    }

    private static bool ValidateDteqVoltage(IPowerConsumer load)
    {
        if (load == GlobalConfig.UtilityModel || load.FedFrom == GlobalConfig.UtilityModel) return true;
        
        var dteq = (DistributionEquipment)load;
        if (load.FedFrom == null || load.FedFrom.VoltageType==null) {
            return true;
        }
        var fedFromVoltage = load.FedFrom.LoadVoltageType;
        
        if (dteq.LineVoltageType == fedFromVoltage) {
            return true;
        }

        return false;
    }
}


