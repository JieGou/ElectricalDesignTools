using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.DistributionEquipment
{
    public class DistributionManager
    {

        public static void UpdateFedFrom(IPowerConsumer caller, IDteq newFedFrom, IDteq oldFedFrom)
        {
            if (caller.FedFrom != null) {
                caller.FedFromId = newFedFrom.Id;
                caller.FedFromTag = newFedFrom.Tag;
                caller.FedFromType = newFedFrom.GetType().ToString();
            }

            if (GlobalConfig.GettingRecords == false) {
                if (oldFedFrom != null) {
                    caller.LoadingCalculated -= oldFedFrom.OnDteqAssignedLoadReCalculated;
                    oldFedFrom.AssignedLoads.Remove(caller);
                    oldFedFrom.CalculateLoading();
                }
                caller.LoadingCalculated += newFedFrom.OnDteqAssignedLoadReCalculated;
                newFedFrom.AssignedLoads.Add(caller);
                newFedFrom.CalculateLoading();
                caller.OnFedFromChanged();
                caller.CalculateLoading();
                caller.CreateCable();
                caller.PowerCable.AssignTagging(caller);
            }
        }
    }
}
