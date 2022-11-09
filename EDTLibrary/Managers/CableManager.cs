using EDTLibrary.A_Helpers;
using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.UndoSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.Managers;
public class CableManager
{
    public static ICableSizer CableSizer { get; set; }

    //reference for quick navigation
    private CecCableSizer cecCableSizer;

   

    public static async Task DeletePowerCableAsync(IPowerConsumer powerCableUser, ListManager listManager)
    {

        if (powerCableUser.PowerCable != null) {
            int cableId = powerCableUser.PowerCable.Id;
            DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, cableId); //await
            listManager.CableList.Remove(powerCableUser.PowerCable);
        }
        return;
    }

    public static async Task DeleteLoadComponentsCablesAsync(ILoad loadModel, ListManager listManager)
    {

        if (loadModel.PowerCable != null) {
            int cableId = loadModel.PowerCable.Id;
            DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, cableId); //await
            listManager.CableList.Remove(loadModel.PowerCable);

            var cablesToRemove = new List<CableModel>();

            foreach (var item in listManager.CableList) {

                if (item.LoadId == loadModel.Id && item.LoadType == loadModel.GetType().ToString()) {
                    cablesToRemove.Add(item);
                }
            }

            foreach (var item in cablesToRemove) {
                listManager.CableList.Remove(item);
                DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, item.Id);
            }

            if (loadModel.Lcs != null) {
                listManager.CableList.Remove((CableModel)loadModel.Lcs.Cable);
                DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, loadModel.Lcs.Cable.Id);
            }


        }
        return;
    }

    internal static double GetLength(string category)
    {
        double length = 3;
        if (category == Categories.DTEQ.ToString()) {
            length = double.Parse(EdtSettings.CableLengthDteq);
        }
        else if (category == Categories.LOAD.ToString()) {
            length = double.Parse(EdtSettings.CableLengthLoad);
        }
        else if (category == Categories.DRIVE.ToString() || category == ComponentSubTypes.DefaultDrive.ToString()) {
            length = double.Parse(EdtSettings.CableLengthDrive);
        }
        else if (category == Categories.LCLDCN.ToString() || category == ComponentSubTypes.DefaultDcn.ToString()) {
            length = double.Parse(EdtSettings.CableLengthLocalDisconnect);
        }
        else if (category == Categories.LCS.ToString()) {
            length = double.Parse(EdtSettings.CableLengthLocalControlStation);
        }

        return length;
    }

    public static bool IsUpdatingPowerCables { get; set; }
    public static string PreviousEq { get; set; }


    public static async Task ValidateLoadPowerComponentCablesAsync(IPowerConsumer powerComponentOwner, ListManager listManager)
    {
        foreach (var components in powerComponentOwner.CctComponents) {
            components.PowerCable.ValidateCableSize(components.PowerCable);
        }
    }

    public static async Task AddAndUpdateLoadPowerComponentCablesAsync(IPowerConsumer powerComponentOwner, ListManager listManager)
    {

        //cable length to load
        double loadCableLength = powerComponentOwner.PowerCable.Length;


        PreviousEq = powerComponentOwner.Tag;

        try {
            await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {

                //Remove Cables
                List<CableModel> cablesToRemove = new List<CableModel>();

                //TODO add Load Id and LoadType to cable model
                //TODO add PowerCableId to Equipment
                foreach (var cable in listManager.CableList) {

                    if (cable.LoadId == powerComponentOwner.Id && cable.LoadType == powerComponentOwner.GetType().ToString()) {
                        loadCableLength = Math.Max(cable.Length, loadCableLength);
                        cablesToRemove.Add(cable);
                    }
                }

                foreach (var cable in cablesToRemove) {
                    listManager.CableList.Remove(cable);
                    DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, cable.Id);
                }

                //Add Cables
                IsUpdatingPowerCables = true;
                UndoManager.CanAdd = false;

                IComponentEdt previousComponent = null;
                foreach (var component in powerComponentOwner.CctComponents) {

                    if (component.SubCategory != SubCategories.CctComponent.ToString()) continue;

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
                    cable.LoadId = powerComponentOwner.Id;
                    cable.LoadType = powerComponentOwner.GetType().ToString();

                    cable.OwnerId = component.Id;
                    cable.OwnerType = component.GetType().ToString();
                    cable.UsageType = powerComponentOwner.PowerCable.UsageType;

                    cable.TypeModel = powerComponentOwner.PowerCable.TypeModel;
                    cable.TypeList = powerComponentOwner.PowerCable.TypeList;
                    cable.ConductorQty = powerComponentOwner.PowerCable.ConductorQty;
                    cable.VoltageRating = powerComponentOwner.PowerCable.VoltageRating;
                    cable.InsulationPercentage = powerComponentOwner.PowerCable.InsulationPercentage;
                    cable.QtyParallel = powerComponentOwner.PowerCable.QtyParallel;

                    cable.Size = powerComponentOwner.PowerCable.Size;

                    //Length
                    if (component.SubType == ComponentSubTypes.DefaultDrive.ToString()) {
                        cable.Length = double.Parse(EdtSettings.CableLengthDrive);
                    }
                    else if (component.SubType == ComponentSubTypes.DefaultDcn.ToString()) {
                        //TODO - Rename CableLenght variabls (LocalDcnToLoad)
                        cable.Length = loadCableLength;
                    }

                    cable.BaseAmps = powerComponentOwner.PowerCable.BaseAmps;
                    cable.Spacing = powerComponentOwner.PowerCable.Spacing;
                    cable.Derating = powerComponentOwner.PowerCable.Derating;
                    cable.DeratedAmps = powerComponentOwner.PowerCable.DeratedAmps;
                    cable.RequiredAmps = powerComponentOwner.PowerCable.RequiredAmps;
                    cable.IsOutdoor = powerComponentOwner.PowerCable.IsOutdoor;
                    cable.InstallationType = powerComponentOwner.PowerCable.InstallationType;

                    cable.SetTypeProperties();

                    cable.ValidateCableSize(cable);

                    component.PowerCable = cable;

                    listManager.CableList.Add(cable);
                    DaManager.UpsertCable(cable);
                    previousComponent = component;
                }
                UpdateLoadCable(powerComponentOwner, previousComponent);

                //needs to be inside awaited method
                IsUpdatingPowerCables = false;
                UndoManager.CanAdd = true;
            }));
        }
        catch (Exception ex) {
            ex.Data.Add("UserMessage", "Adding cable for components error");
            throw;
        }

        //Local method
        void UpdateLoadCable(IPowerConsumer load, IComponentEdt previousComponent)
        {
            if (previousComponent == null) {
                load.PowerCable.Source = load.FedFrom.Tag;
                load.PowerCable.Length = loadCableLength;
            }
            else if (previousComponent != null) {

                if (previousComponent.SubType == ComponentSubTypes.DefaultDcn.ToString()) {
                    load.PowerCable.Length = double.Parse(EdtSettings.CableLengthLocalDisconnect);
                }

                else { //the length of the cable
                    load.PowerCable.Length = loadCableLength;
                }
                load.PowerCable.Source = previousComponent.Tag;
            }
            load.PowerCable.Tag = GetCableTag(load.PowerCable.Source, load.Tag);
            load.PowerCable.Id = listManager.CableList.Max(l => l.Id) + 1;  //DaManager.SavePowerCableGetId(cable);
            listManager.CableList.Add(load.PowerCable);
            DaManager.UpsertCable(load.PowerCable);
        }
    }


    public static string GetCableTag(string cableSource, string cableDestination, [CallerMemberName] string callerMethod = "")
    {
        try {
            cableSource = cableSource.Replace("-", "");
            cableDestination = cableDestination.Replace("-", "");
            string tag = cableSource + TagSettings.CableTagSeparator + cableDestination;
            return tag;
        }
        catch (Exception) {

            return "error";
        }
    }

    public static void AddLcsCables(IComponentUser componentUser, LocalControlStationModel lcs, ListManager listManager)
    {
        ILoad lcsOwner = componentUser as LoadModel;
        CreateLcsControlCable(lcs, listManager, lcsOwner);
    }

    private static void CreateLcsControlCable(LocalControlStationModel lcs, ListManager listManager, ILoad lcsOwner)
    {
        CableModel cable = new CableModel();

        cable.Source = lcsOwner.FedFrom.Tag;
        cable.Destination = lcs.Tag;
        cable.Tag = GetCableTag(cable.Source, cable.Destination);

        cable.Id = listManager.CableList.Max(c => c.Id) + 1;  //DaManager.SavePowerCableGetId(cable);

        cable.OwnerId = lcs.Id;
        cable.OwnerType = typeof(LocalControlStationModel).ToString();
        cable.UsageType = CableUsageTypes.Control.ToString();

        cable.Size = EdtSettings.LcsControlCableSize;
        cable.Length = double.Parse(EdtSettings.CableLengthLocalControlStation);

        cable.ConductorQty = lcs.TypeModel.DigitalConductorQty;
        var voltageClass = TypeManager.ControlCableTypes.FirstOrDefault(c => c.Type == EdtSettings.LcsControlCableType).VoltageRating;
        IsUpdatingPowerCables = true;
        cable.TypeModel = TypeManager.GetLcsControlCableTypeModel(lcs);
        IsUpdatingPowerCables = false;
        cable.VoltageRating = voltageClass;
        cable.QtyParallel = 1;

        cable.Spacing = 0;
        cable.Derating = 1;

        cable.IsOutdoor = lcsOwner.PowerCable.IsOutdoor;
        cable.InstallationType = lcsOwner.PowerCable.InstallationType;

        lcs.Cable = cable;

        cable.SetTypeProperties();

        listManager.CableList.Add(cable);
        DaManager.UpsertCable((CableModel)cable);
    }

    internal static void DeleteLcsControlCable(IComponentUser componentUser, ILocalControlStation lcsToRemove, ListManager listManager)
    {
        if (lcsToRemove.Cable != null) {
            int cableId = lcsToRemove.Cable.Id;
            DaManager.prjDb.DeleteRecord(GlobalConfig.CableTable, cableId); //await
            listManager.CableList.Remove((CableModel)lcsToRemove.Cable);

            var list = new List<CableModel>();
            foreach (var cable in listManager.CableList) {
                if (cable.OwnerType == typeof(LocalControlStationModel).ToString() && cable.OwnerId == lcsToRemove.Cable.Id) {
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