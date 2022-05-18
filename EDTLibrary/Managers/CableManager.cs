using EDTLibrary.DataAccess;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables;
public class CableManager
{
    public static ICableSizer CableSizer { get; set; }

    public static async Task DeletePowerCableAsync(IPowerConsumer powerCableUser, ListManager listManager)
    {
        //TODO - Move to Cable Manager

        if (powerCableUser.PowerCable != null) {
            int cableId = powerCableUser.PowerCable.Id;
            DaManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, cableId); //await
            listManager.CableList.Remove(powerCableUser.PowerCable);
        }
        return;
    }

    
    public static void AssignPowerCables(IPowerConsumer load, ListManager listManager)
    {
        IComponent previousComponent = null;
        foreach (var component in load.CctComponents) {
            PowerCableModel cable = new PowerCableModel();
            cable.Tag = load.FedFrom.Tag + TagSettings.CableTagSeparator + component.Tag;
            cable.TypeModel = load.PowerCable.TypeModel;
            listManager.CableList.Add(cable);
            previousComponent = component;
        }
        if (previousComponent == null) return;
        load.PowerCable.Tag = previousComponent.Tag + TagSettings.CableTagSeparator + load.Tag;
    }
}
