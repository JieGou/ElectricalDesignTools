using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary.Models.DistributionEquipment
{

    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class SplitterModel : DistributionEquipment
    {


        public override bool AddNewLoad(IPowerConsumer load)
        {

            var newLoadAdded = base.AddNewLoad(load);
            if (load == null) return false;
            SetLoadProtectionDevice(load);

            CableManager.AddAndUpdateLoadPowerComponentCablesAsync(load, ScenarioManager.ListManager);

            return newLoadAdded;
        }

        /// <summary>
        /// changes the load protection device to a standalone and adds it to the Cct Componnents List
        /// </summary>
        /// <param name="load"></param>
        public override void SetLoadProtectionDevice(IPowerConsumer load)
        {
            if (DaManager.GettingRecords) return;

            if (load.ProtectionDevice != null) {
                load.ProtectionDevice.IsStandAlone = true;

                ProtectionDeviceManager.SetProtectionDeviceType(load);

                var pdInCctComponentList = load.CctComponents.FirstOrDefault(c => c == load.ProtectionDevice);
                if (pdInCctComponentList == null) {
                    load.CctComponents.Insert(0, load.ProtectionDevice);
                }
            }
        }

        public override void RemoveAssignedLoad(IPowerConsumer load)
        {
            load.ProtectionDevice.IsStandAlone = false;
            load.CctComponents.Remove(load.ProtectionDevice);
            ProtectionDeviceManager.SetProtectionDeviceType(load);

            CableManager.AddAndUpdateLoadPowerComponentCablesAsync(load, ScenarioManager.ListManager);

            base.RemoveAssignedLoad(load);
        }
    }

}
