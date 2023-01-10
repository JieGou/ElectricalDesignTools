using EDTLibrary.ErrorManagement;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.UndoSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EDTLibrary.Validators;
internal class FedFromValidator
{
    internal static bool IsFedFromValid(IDteq load, IDteq newFedFrom)
    {
        //Fed from validation - Checks if the equipment is fed from itself and does not allow the change to proceed.
        if (newFedFrom == null) {
            return true;
        }
        else {
            //invalid
            if (newFedFrom.Tag == load.Tag) {
                return false;
                  
            }
            //Valid
            else if (newFedFrom.Tag == GlobalConfig.UtilityTag || newFedFrom.Tag == GlobalConfig.Deleted) {
                return true;
            }
            else {
                return IsFedFromValid(load, newFedFrom.FedFrom);
                newFedFrom = newFedFrom.FedFrom;
            }
        }
        return true;
    }
}
