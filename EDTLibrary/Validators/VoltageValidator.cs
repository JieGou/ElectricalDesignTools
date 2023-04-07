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
    internal static bool IsValid(IPowerConsumer equipment)
    {
        if (equipment is LoadModel) {
            return ValidateLoadVoltage(equipment);
        }
        else if (equipment is DistributionEquipment) {
            return ValidateDteqVoltage(equipment);
        }
        return false;
    }

    private static bool ValidateLoadVoltage(IPowerConsumer equipment)
    {
        if (equipment.FedFrom == null ) return false;
        

        var fedFromVoltage = equipment.FedFrom.LoadVoltageType;
        if (fedFromVoltage == null) return false;

        //motors
        if (equipment.Type == LoadTypes.MOTOR.ToString()) {
            if (fedFromVoltage.Voltage == 480 && equipment.VoltageType.Voltage == 460) {
                return true;
            }
            if (fedFromVoltage.Voltage == 600 && equipment.VoltageType.Voltage == 575) {
                return true;
            }
            if (fedFromVoltage.Voltage == 4160 && equipment.VoltageType.Voltage == 4000) {
                return true;
            }
        }

        //208V panel
        if (equipment.FedFrom.Type == DteqTypes.DPN.ToString() && equipment.FedFrom.LoadVoltageType.Voltage == 208) {
            if (equipment.VoltageType.Voltage == 208) {
                return true;
            }
            if (equipment.VoltageType.Voltage == 120) {
                return true;
            }
        }

        //240V panel
        if (equipment.FedFrom.Type == DteqTypes.DPN.ToString() && equipment.FedFrom.LoadVoltageType.Voltage == 240) {
            if (equipment.VoltageType.Voltage == 240) {
                return true;
            }
            if (equipment.VoltageType.Voltage == 120) {
                return true;
            }
        }


        if (equipment.VoltageType == fedFromVoltage) {
            return true;
        }

        return false;
    }

    private static bool ValidateDteqVoltage(IPowerConsumer equipment)
    {
        if (equipment == GlobalConfig.UtilityModel || equipment.FedFrom == GlobalConfig.UtilityModel) return true;
        
        if (equipment.FedFrom == null || equipment.FedFrom.VoltageType==null) {
            return true;
        }

        var dteq = (DistributionEquipment)equipment;
        if (dteq.LineVoltageType == dteq.FedFrom.LoadVoltageType) {
            return true;
        }

        return false;
    }
}


