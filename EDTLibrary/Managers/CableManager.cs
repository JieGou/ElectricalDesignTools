using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeTables;
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

        if (powerCableUser.PowerCable != null) {
            int cableId = powerCableUser.PowerCable.Id;
            DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, cableId); //await
            listManager.CableList.Remove(powerCableUser.PowerCable);

            var list = new List<CableModel>();
            foreach (var cable in listManager.CableList) {
                if (cable.OwnerType == typeof(IComponent).ToString() && cable.OwnerId == powerCableUser.Id) {
                    list.Add(cable);
                    DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, cable.Id);
                }
            }

            foreach (var cable in list) {
                listManager.CableList.Remove(cable);
            }
        }
        return;
    }


    public static bool IsUpdatingPowerCables { get; set; }
    public static string PreviousEq { get; set; }
    public static int count { get; set; } = 0;
    public static async Task UpdateLoadPowerComponentCablesAsync(IPowerConsumer powerComponentOwner, ListManager listManager)
    {
        if (PreviousEq == powerComponentOwner.Tag) {
            count +=1;
        }
        if (count >= 2) {
            count = 0;
            return;
        }
        PreviousEq = powerComponentOwner.Tag;

        IsUpdatingPowerCables = true;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Debug.Print($"Start {sw.Elapsed.TotalMilliseconds.ToString()}");

        if (powerComponentOwner == null) return;

        try {
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                //Remove Cables
                List<CableModel> cablesToRemove = new List<CableModel>();
                foreach (var item in listManager.CableList) {
                    
                    if (item.OwnerId == powerComponentOwner.Id
                        && item.OwnerType == typeof(IComponent).ToString()) {
                        cablesToRemove.Add(item);
                    }
                }

                foreach (var item in cablesToRemove) {
                    listManager.CableList.Remove(item);
                    DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, item.Id);
                }

                //Add Cables
                IComponent previousComponent = null;
                foreach (var component in powerComponentOwner.CctComponents) {

                    if (component.SubCategory != Categories.CctComponent.ToString()) continue;

                    CableModel cable = new CableModel();

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

                    component.PowerCable = cable;

                    listManager.CableList.Add(cable);
                    DaManager.UpsertCable(cable);
                    previousComponent = component;
                }
                UpdateLoadCable(powerComponentOwner, previousComponent);
            }));
        }
        catch (Exception ex) {
            ex.Data.Add("UserMessage", "Adding cable for components error");
            throw;
        }
        sw.Stop();
        Debug.Print(sw.Elapsed.TotalMilliseconds.ToString());
        IsUpdatingPowerCables = false;



        //Local method
        void UpdateLoadCable(IPowerConsumer load, IComponent previousComponent)
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

    }

    public static string GetCableTag(string cableSource, string cableDestination)
    {
        cableSource = cableSource.Replace("-", "");
        cableDestination = cableDestination.Replace("-", "");
        string tag = cableSource + TagSettings.CableTagSeparator + cableDestination;
        return tag;
    }

    public static void AddLcsControlCableForLoad(IComponentUser componentUser, LocalControlStationModel lcs, ListManager listManager)
    {
        ILoad lcsOwner = componentUser as LoadModel;
        CableModel cable = new CableModel();

        cable.Source = lcsOwner.FedFrom.Tag;
        cable.Destination = lcs.Tag;
        cable.Tag = GetCableTag(cable.Source, cable.Destination);

        cable.Id = listManager.CableList.Max(l => l.Id) + 1;  //DaManager.SavePowerCableGetId(cable);

        cable.OwnerId = lcs.Id;
        cable.OwnerType = typeof(LocalControlStationModel).ToString();
        cable.UsageType = CableUsageTypes.Control.ToString();

        cable.Size = EdtSettings.DefaultLcsControlCableSize;
        cable.ConductorQty = lcs.TypeModel.DigitalConductorQty;
        var voltageClass = TypeManager.ControlCableTypes.FirstOrDefault(c => c.Type == EdtSettings.DefaultLcsControlCableType).VoltageClass;
        IsUpdatingPowerCables = true;
        cable.TypeModel = TypeManager.GetLcsControlCableTypeModel(lcs);
        IsUpdatingPowerCables = false;
        cable.VoltageClass = voltageClass;
        cable.QtyParallel = 1;

        cable.Spacing = 0;
        cable.Derating = 1;
       
        cable.Outdoor = lcsOwner.PowerCable.Outdoor;
        cable.InstallationType = lcsOwner.PowerCable.InstallationType;

        lcs.ControlCable = cable;

        listManager.CableList.Add(cable);
        DaManager.UpsertCable(cable);
    }

    internal static void DeleteLcsControlCable(IComponentUser componentUser, ILocalControlStation lcsToRemove, ListManager listManager)
    {
        if (lcsToRemove.ControlCable != null) {
            int cableId = lcsToRemove.ControlCable.Id;
            DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, cableId); //await
            listManager.CableList.Remove((CableModel)lcsToRemove.ControlCable);

            var list = new List<CableModel>();
            foreach (var cable in listManager.CableList) {
                if (cable.OwnerType == typeof(LocalControlStationModel).ToString() && cable.OwnerId == lcsToRemove.ControlCable.Id) {
                    list.Add(cable);
                    DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, cable.Id);
                }
            }

            foreach (var cable in list) {
                listManager.CableList.Remove(cable);
            }
        }
        return;
    }
}
