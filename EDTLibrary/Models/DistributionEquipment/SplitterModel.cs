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


        public override bool AdddNewLoad(IPowerConsumer load)
        {

            if (load == null) return false;


            ComponentManager.AddSplitterLoadProtectionDevice(this, load, ScenarioManager.ListManager);
            CableManager.AddAndUpdateLoadPowerComponentCablesAsync(load, ScenarioManager.ListManager);



            return base.AdddNewLoad(load);
        }

        public override void RemoveAssignedLoad(IPowerConsumer load)
        {

            ComponentManager.RemoveSplitterLoadProtectionDevice(this, load, ScenarioManager.ListManager);
            CableManager.AddAndUpdateLoadPowerComponentCablesAsync(load, ScenarioManager.ListManager);

            base.RemoveAssignedLoad(load);
        }
    }

}
