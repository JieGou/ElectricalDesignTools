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

        public static void UpdateFedFrom(IPowerConsumer caller, IDteq newSupplier, IDteq oldSupplier)
        {
            if (caller.FedFrom != null) {
                caller.FedFromId = newSupplier.Id;
                caller.FedFromTag = newSupplier.Tag;
                caller.FedFromType = newSupplier.GetType().ToString();
            }

            if (GlobalConfig.GettingRecords == false) {
                if (oldSupplier != null) {
                    caller.LoadingCalculated -= oldSupplier.OnDteqAssignedLoadReCalculated;
                    oldSupplier.AssignedLoads.Remove(caller);
                    oldSupplier.CalculateLoading();
                }
                caller.LoadingCalculated += newSupplier.OnDteqAssignedLoadReCalculated;
                newSupplier.AssignedLoads.Add(caller);
                newSupplier.CalculateLoading();
                caller.OnFedFromChanged();
                caller.CalculateLoading();
                caller.CreateCable();
                caller.PowerCable.AssignTagging(caller);
            }
        }


    }
}
