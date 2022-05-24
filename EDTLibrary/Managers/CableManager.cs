using EDTLibrary.DataAccess;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.Managers;
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


    public static async Task AssignPowerCables(IPowerConsumer powerComponentOwner, ListManager listManager)
    {

        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {


            Stopwatch sw = new Stopwatch();
            sw.Start();
            Debug.Print(sw.Elapsed.TotalMilliseconds.ToString());
            try {

                //Remove Cables
                List<PowerCableModel> cablesToRemove = new List<PowerCableModel>();
                foreach (var item in listManager.CableList) {
                    if (item.OwnerId == powerComponentOwner.Id
                        && item.OwnerType == typeof(IComponent).ToString()) {
                        cablesToRemove.Add(item);
                    }
                }

                foreach (var item in cablesToRemove) {
                    listManager.CableList.Remove(item);
                    DaManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, item.Id);
                }

                //Add Cables
                IComponent previousComponent = null;
                foreach (var component in powerComponentOwner.CctComponents) {

                    if (component.Category != Categories.CctComponent.ToString()) continue;

                    PowerCableModel cable = new PowerCableModel();

                    if (previousComponent == null) {
                        cable.Source = powerComponentOwner.FedFrom.Tag;

                    }
                    else if (previousComponent != null) {
                        cable.Source = previousComponent.Tag;
                    }
                    cable.Destination = component.Tag;
                    cable.Tag = GetCableTag(cable.Source, cable.Destination);

                    cable.Id = listManager.CableList.Max(l => l.Id) + 1;  //DaManager.SavePowerCableGetId(cable);
                    cable.Load = powerComponentOwner;

                    cable.OwnerId = powerComponentOwner.Id;
                    cable.OwnerType = typeof(IComponent).ToString();

                    cable.TypeModel = powerComponentOwner.PowerCable.TypeModel;
                    cable.TypeList = powerComponentOwner.PowerCable.TypeList;
                    cable.UsageType = powerComponentOwner.PowerCable.UsageType;
                    cable.ConductorQty = powerComponentOwner.PowerCable.ConductorQty;
                    cable.VoltageClass = powerComponentOwner.PowerCable.VoltageClass;
                    cable.Insulation = powerComponentOwner.PowerCable.Insulation;
                    cable.QtyParallel = powerComponentOwner.PowerCable.QtyParallel;
                    cable.Size = powerComponentOwner.PowerCable.Size;
                    cable.BaseAmps = powerComponentOwner.PowerCable.BaseAmps;
                    cable.Spacing = powerComponentOwner.PowerCable.Spacing;
                    cable.Derating = powerComponentOwner.PowerCable.Derating;
                    cable.DeratedAmps = powerComponentOwner.PowerCable.DeratedAmps;
                    cable.RequiredAmps = powerComponentOwner.PowerCable.RequiredAmps;
                    cable.Outdoor = powerComponentOwner.PowerCable.Outdoor;
                    cable.InstallationType = powerComponentOwner.PowerCable.InstallationType;

                    cable.InstallationType = powerComponentOwner.PowerCable.InstallationType;


                    listManager.CableList.Add(cable);
                    DaManager.UpsertCable(cable);
                    previousComponent = component;
                }
                UpdateLoadCable(powerComponentOwner, previousComponent);
            }
            catch (Exception ex) {
                ex.Data.Add("UserMessage", "Adding cable for components error");
                throw;
            }
            Debug.Print(sw.Elapsed.TotalMilliseconds.ToString());
        }));


    }

    private static void UpdateLoadCable(IPowerConsumer load, IComponent previousComponent)
    {
        if (previousComponent == null) {
            load.PowerCable.Source = load.FedFrom.Tag;
        }
        else if (previousComponent != null) {
            load.PowerCable.Source = previousComponent.Tag;
        }
        load.PowerCable.Tag = GetCableTag(load.PowerCable.Source, load.Tag);

        DaManager.UpsertCable(load.PowerCable);

    }

    public static string GetCableTag(string cableSource, string cableDestination)
    {
        cableSource = cableSource.Replace("-", "");
        cableDestination = cableDestination.Replace("-", "");
        string tag = cableSource + TagSettings.CableTagSeparator + cableDestination;
        return tag;
    }
}
