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
        private ListManager _listManager;

        public DistributionManager(ListManager listManager)
        {
            _listManager = listManager;
        }
        //TODO - create Distribution Manager instance inside eqVm

        /// <summary>
        /// Transfers the load from the old to the new supplier. (Id, Tag, Type, events, load calculation, cable tag , etc.
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="newSupplier"></param>
        /// <param name="oldSupplier"></param>
        public static void UpdateFedFrom(IPowerConsumer caller, IDteq newSupplier, IDteq oldSupplier)
        {
            
                if (caller.FedFrom != null) 
                {
                    caller.FedFromId = newSupplier.Id;
                    caller.FedFromTag = newSupplier.Tag;
                    caller.FedFromType = newSupplier.GetType().ToString();
                }

                if (GlobalConfig.GettingRecords == false) 
                {
                    if (oldSupplier != null) 
                    {
                        caller.LoadingCalculated -= oldSupplier.OnAssignedLoadReCalculated;
                        oldSupplier.AssignedLoads.Remove(caller);
                        oldSupplier.CalculateLoading();
                    }
                    caller.LoadingCalculated += newSupplier.OnAssignedLoadReCalculated;
                    newSupplier.AssignedLoads.Add(caller);
                    newSupplier.CalculateLoading();

                    if (caller.Tag != "" &&
                        caller.Voltage != 0 &&
                        caller.Fla != 0) 
                    {
                        caller.CalculateLoading();
                        caller.PowerCable.AssignTagging(caller);
                    }
                }

        }

        //TODO - Load and DTEQ type changes
    }
}
