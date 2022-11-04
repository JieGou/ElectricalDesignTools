using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Managers
{
    public class DistributionManager
    {
        private ListManager _listManager;

        public DistributionManager(ListManager listManager)
        {
            _listManager = listManager;
        }



        /// <summary>
        /// Transfers the load from the old to the new supplier. (Id, Tag, Type, events, load calculation, cable tag , etc.
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="newSupplier"></param>
        /// <param name="oldSupplier"></param>
        public static void UpdateFedFrom(IPowerConsumer caller, IDteq newSupplier, IDteq oldSupplier)
        {
            if (caller.FedFrom != null) {
                caller.FedFromId = newSupplier.Id;
                caller.FedFromTag = newSupplier.Tag;
                caller.FedFromType = newSupplier.GetType().ToString();
            }


            if (DaManager.GettingRecords == false) {

                if (oldSupplier != null) {
                    caller.LoadingCalculated -= oldSupplier.OnAssignedLoadReCalculated;
                  
                    oldSupplier.RemoveAssignedLoad(caller);

                    if (oldSupplier.Tag != GlobalConfig.Deleted) {
                        oldSupplier.CalculateLoading(); //possible cause of resaving dteq to databse
                    }
                }

                if (caller.CalculationFlags != null) { 
                    if (caller.CalculationFlags.CanUpdateFedFrom) {
                        newSupplier.AdddNewLoad(caller);
                    }
                    
                }
                else {
                    newSupplier.AdddNewLoad(caller);
                }
                caller.LoadingCalculated += newSupplier.OnAssignedLoadReCalculated;

                caller.VoltageType = newSupplier.LoadVoltageType;

                newSupplier.CalculateLoading();

                if (caller.VoltageType != null) {
                    if (caller.Tag != "" && caller.VoltageType.Voltage != 0 && caller.Fla != 0) {
                        caller.CalculateLoading();
                        caller.PowerCable.SetSourceAndDestinationTags(caller);
                    }
                }
            }
        }

        public static void RetagLoadsOfDeleted(IDteq dteq)
        {
            //Loads
            List<IPowerConsumer> assignedLoads = new List<IPowerConsumer>();
            if (dteq.AssignedLoads != null) {
                assignedLoads.AddRange(dteq.AssignedLoads);
            }

            //Retag supplier of loads to "Deleted"
            IDteq deleted = GlobalConfig.DteqDeleted;
            for (int i = 0; i < assignedLoads.Count; i++) {
                var load = assignedLoads[i];
                load.FedFromTag = GlobalConfig.Deleted;
                load.FedFrom = deleted;
                load.PowerCable.SetSourceAndDestinationTags(load);

                load.OnPropertyUpdated();
                load.PowerCable.OnPropertyUpdated();
            }
            return;
        }
        //TODO - Load and DTEQ type changes

        public async Task CalculateDteqLoadingAsync() // LoadList Manager
        {
            await Task.Run(() => {
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                Debug.Print("CalculateDteqLoadingAsync Start");
                _listManager.UnregisterAllDteqFromAllLoadEvents();
                _listManager.AssignLoadsAndEventsToAllDteq();
                double total = 0;
                double subTotal = 0;
                Debug.Print(sw.Elapsed.TotalMilliseconds.ToString());

                //Loads
                DaManager.GettingRecords = false;
                {
                    foreach (var load in _listManager.LoadList) {
                        sw.Restart();

                        load.CalculateLoading();
                        load.PowerCable.AutoSize();
                        load.PowerCable.CalculateAmpacity(load);

                        subTotal += sw.Elapsed.TotalMilliseconds;

                    }
                }
                DaManager.GettingRecords = false;

                Debug.Print("Loads: " + subTotal);
                total += subTotal;
                subTotal = 0;

                //Dteq
                foreach (var dteq in _listManager.IDteqList) {
                    sw.Restart();

                    dteq.CalculateLoading();
                    dteq.PowerCable.AutoSize();
                    dteq.PowerCable.CalculateAmpacity(dteq);

                    subTotal += sw.Elapsed.TotalMilliseconds;
                }
                Debug.Print("Dteq: " + subTotal);
                total += subTotal;
                Debug.Print("Total: " + total);
            });
        }
    }
}
