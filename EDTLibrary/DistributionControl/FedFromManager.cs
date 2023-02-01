using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.DistributionControl
{
    public class FedFromManager
    {
        private ListManager _listManager;

        public FedFromManager(ListManager listManager)
        {
            _listManager = listManager;
        }

        public static bool IsUpdatingFedFrom_List { get; set; } = false;

        public static bool UpdateFedFrom_Single(IPowerConsumer caller, IDteq newSupplier, IDteq oldSupplier)
        {
            if (DaManager.GettingRecords == false) {
                var wasFedFromUpdated = UpdateFedFrom(caller, newSupplier, oldSupplier);
                if (DaManager.Importing == false) {
                    OnFedFromUpdated(); 
                }
                return wasFedFromUpdated;
            }
            return true;
        }

        /// <summary>
        /// List is used so that bulk updates only refresh the views once.
        /// </summary>
        /// <param name="loadsToRefeed"></param>
        public static void UpdateFedFrom_List(List<UpdateFedFromItem> loadsToRefeed)
        {
            IsUpdatingFedFrom_List = true;
            foreach (var item in loadsToRefeed)
            {
                UpdateFedFrom(item.Caller, item.NewSupplier, item.OldSupplier);
                item.Caller.FedFrom = item.NewSupplier;
            }

            if (DaManager.Importing == false) {
                OnFedFromUpdated();
            }
            IsUpdatingFedFrom_List = false;

        }

       
        /// <summary>
        /// Transfers the load from the old to the new supplier. (Id, Tag, Type, events, load calculation, cable tag , etc.
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="newSupplier"></param>
        /// <param name="oldSupplier"></param>
        private static bool UpdateFedFrom(IPowerConsumer caller, IDteq newSupplier, IDteq oldSupplier)
        {
            if (newSupplier.CanAdd(caller) == false) return false;

            caller.FedFromId = newSupplier.Id;
            caller.FedFromTag = newSupplier.Tag;
            caller.FedFromType = newSupplier.GetType().ToString();


            if (DaManager.GettingRecords == false)
            {

                if (oldSupplier != null)
                {
                    caller.LoadingCalculated -= oldSupplier.OnAssignedLoadReCalculated;

                    oldSupplier.RemoveAssignedLoad(caller);

                    if (oldSupplier.Tag != GlobalConfig.Deleted)
                    {
                        oldSupplier.CalculateLoading(); //possible cause of resaving dteq to databse
                    }
                }

                if (caller.CalculationFlags != null)
                {
                    if (caller.CalculationFlags.CanUpdateFedFrom)
                    {
                        newSupplier.AddNewLoad(caller);
                    }
                }

                else
                {
                    newSupplier.AddNewLoad(caller);
                }

                caller.LoadingCalculated += newSupplier.OnAssignedLoadReCalculated;

                caller.VoltageType = newSupplier.LoadVoltageType;

                newSupplier.CalculateLoading();

                if (caller.VoltageType != null)
                {
                    if (caller.Tag != "" && caller.VoltageType.Voltage != 0 && caller.Fla != 0)
                    {
                        //caller.CalculateLoading();
                        //caller.PowerCable.SetSourceAndDestinationTags(caller);
                    }
                }
            }
            return true;
        }

        public static void RetagLoadsOfDeleted(IDteq dteq)
        {
            //Loads
            List<IPowerConsumer> assignedLoads = new List<IPowerConsumer>();
            if (dteq.AssignedLoads != null)
            {
                assignedLoads.AddRange(dteq.AssignedLoads);
            }

            //Retag supplier of loads to "Deleted"
            IDteq deleted = GlobalConfig.DteqDeleted;
            for (int i = 0; i < assignedLoads.Count; i++)
            {
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


        public static event EventHandler FedFromUpdated;
        public static void OnFedFromUpdated()
        {
            if (FedFromUpdated != null)
            {
                FedFromUpdated(nameof(FedFromManager), EventArgs.Empty);
            }
        }

    }
}
