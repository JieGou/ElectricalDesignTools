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
        try {
            //Todo - delete only component cables
            List<PowerCableModel> cablesToRemove = new List<PowerCableModel>();
            foreach (var item in listManager.CableList) {
                if (item.OwnerId == load.Id
                    && item.UsageType == "test") {
                    cablesToRemove.Add(item);
                }
            }

            foreach (var item in cablesToRemove) {
                listManager.CableList.Remove(item);
            }

            IComponent previousComponent = null;
            foreach (var component in load.CctComponents) {

                PowerCableModel cable = new PowerCableModel();
                cable.OwnerId = load.Id;

                if (previousComponent == null) {
                    cable.Tag = GetCableTag(load.FedFrom.Tag, component.Tag);
                }
                else if (previousComponent != null) {
                    cable.Tag = GetCableTag(previousComponent.Tag, component.Tag);
                }

                cable.Load = load;
                cable.TypeModel = load.PowerCable.TypeModel;
                cable.UsageType = "test";

                listManager.CableList.Add(cable);
                previousComponent = component;
            }
            if (previousComponent == null) {
                load.PowerCable.Tag = GetCableTag(load.FedFrom.Tag, load.Tag);
            }
            else if (previousComponent != null) {
                load.PowerCable.Tag = GetCableTag(previousComponent.Tag, load.Tag);
            }
        }
        catch (Exception ex) {
            ex.Data.Add("UserMessage", "Adding cable for components erro");
            throw;
        }
    }

    public static string GetCableTag(string from, string to)
    {
        from = from.Replace("-", "");
        to = to.Replace("-", "");
        string tag = from + TagSettings.CableTagSeparator + to;
        return tag;
    }
}
