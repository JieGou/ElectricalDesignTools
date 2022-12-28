using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.UndoSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components;
public class ComponentFactory
{
    public static ProtectionDeviceModel CreateProtectionDevice(IComponentUser componentUser, string subCategory, string type, string subType, ListManager listManager)
    {
        var ProtectionDevice = new ProtectionDeviceModel();

        if (listManager.PdList.Count < 1) {
            ProtectionDevice.Id = 1;
        }
        else {
            ProtectionDevice.Id = listManager.PdList.Select(c => c.Id).Max() + 1;
        }
        ProtectionDevice.Category = Categories.CctComponent.ToString();
        ProtectionDevice.SubCategory = subCategory;
        ProtectionDevice.Owner = componentUser;
        ProtectionDevice.OwnerId = componentUser.Id;
        ProtectionDevice.OwnerType = componentUser.GetType().ToString();
        ProtectionDevice.Type = type;
        ProtectionDevice.SubType = subType;


        // Disconnect
        if (subType == PdTypes.FDS.ToString()) {
            ProtectionDevice.Tag = componentUser.Tag + ".PD";
            var load = (IPowerConsumer)componentUser;
            ProtectionDevice.Area = load.FedFrom.Area;
            ProtectionDevice.FrameAmps = DataTableSearcher.GetDisconnectSize(load);
            ProtectionDevice.TripAmps = DataTableSearcher.GetDisconnectFuse(load);
            ProtectionDevice.SequenceNumber = 0;

        }

        // starter
        else if (subType.Contains("MCP")) {
            ProtectionDevice.Tag = componentUser.Tag + ".PD";
            var load = (IPowerConsumer)componentUser;
            ProtectionDevice.Area = load.FedFrom.Area;
            ProtectionDevice.FrameAmps = DataTableSearcher.GetDisconnectSize(load);
            ProtectionDevice.TripAmps = DataTableSearcher.GetDisconnectFuse(load);
            ProtectionDevice.SequenceNumber = 0;
        }

        // breaker
        else if (subType == PdTypes.BKR.ToString()) {
            ProtectionDevice.Tag = componentUser.Tag + ".PD";
            var load = (IPowerConsumer)componentUser;
            ProtectionDevice.Area = load.FedFrom.Area;
            ProtectionDevice.FrameAmps = DataTableSearcher.GetDisconnectSize(load);
            ProtectionDevice.TripAmps = DataTableSearcher.GetDisconnectFuse(load);
            ProtectionDevice.SequenceNumber = 0;
        }


        listManager.PdList.Add(ProtectionDevice);
        ProtectionDevice.PropertyUpdated += DaManager.OnProtectioneDevicePropertyUpdated;

        return ProtectionDevice;
    }
    
    public static ComponentModel CreateCircuitComponent(IComponentUser componentUser, string subCategory, string type, string subType, ListManager listManager)
    {
        IComponentEdt newComponent = new ComponentModel();

        if (listManager.CompList.Count < 1) {
            newComponent.Id = 1;
        }
        else {
            newComponent.Id = listManager.CompList.Select(c => c.Id).Max() + 1;
        }
        newComponent.Category = Categories.CctComponent.ToString();
        newComponent.SubCategory = subCategory;
        newComponent.Owner = componentUser;
        newComponent.OwnerId = componentUser.Id;
        newComponent.OwnerType = componentUser.GetType().ToString();
        newComponent.Type = type;
        newComponent.SubType = subType;

        //Drive
        if (subType == CctComponentSubTypes.DefaultDrive.ToString()) {
            newComponent.SettingTag = true;
                newComponent.Tag = componentUser.Tag + TagSettings.ComponentSuffixSeparator + TagSettings.DriveSuffix;
            newComponent.SettingTag = false;
            var load = (IPowerConsumer)componentUser;
            newComponent.Area = load.FedFrom.Area;
            newComponent.SequenceNumber = 0;
            newComponent.FrameAmps = DataTableSearcher.GetDisconnectSize(load);
            newComponent.FrameAmps = DataTableSearcher.GetDisconnectFuse(load);
            if (load.FedFrom.Type == DteqTypes.SPL.ToString()) {
                newComponent.SequenceNumber = 1;
            }
        }

        //Local Disconnect
        if (subType == CctComponentSubTypes.DefaultDcn.ToString()) {
            newComponent.Tag = componentUser.Tag + TagSettings.ComponentSuffixSeparator + TagSettings.DisconnectSuffix;
            newComponent.Area = componentUser.Area;
            var load = (IPowerConsumer)componentUser;
            newComponent.FrameAmps = DataTableSearcher.GetDisconnectSize(load);
            newComponent.FrameAmps = DataTableSearcher.GetDisconnectFuse(load);
            newComponent.SequenceNumber = componentUser.CctComponents.Count + 1;
        }


        //Order of components
        componentUser.CctComponents.Add(newComponent);
        componentUser.CctComponents = new ObservableCollection<IComponentEdt>(componentUser.CctComponents.OrderBy(c => c.SequenceNumber).ToList());

        listManager.CompList.Add(newComponent);
        DaManager.UpsertComponentAsync((ComponentModel)newComponent);
        newComponent.PropertyUpdated += DaManager.OnComponentPropertyUpdated;
        return (ComponentModel)newComponent;
    }

    
    public static ComponentModel CreateDrive(IComponentUser componentUser, string type, string subType, ListManager listManager)
    {
        IComponentEdt newDrive = new ComponentModel();

        if (listManager.CompList.Count < 1) {
            newDrive.Id = 1;
        }
        else {
            newDrive.Id = listManager.CompList.Select(c => c.Id).Max() + 1;
        }
        newDrive.Category = Categories.CctComponent.ToString();
        newDrive.SubCategory = SubCategories.CctComponent.ToString();
        newDrive.Owner = componentUser;
        newDrive.OwnerId = componentUser.Id;
        newDrive.OwnerType = componentUser.GetType().ToString();
        newDrive.Type = type;
        newDrive.SubType = subType;


            newDrive.Tag = componentUser.Tag + TagSettings.ComponentSuffixSeparator + TagSettings.DriveSuffix;
            var powerConsumer = (IPowerConsumer)componentUser;
            newDrive.Area = powerConsumer.FedFrom.Area;


        //Size


        //Order of components
        if (newDrive.SubCategory == SubCategories.CctComponent.ToString()) {
            var defaultDcn = componentUser.CctComponents.FirstOrDefault(c => c.SubType == CctComponentSubTypes.DefaultDcn.ToString());
            if (newDrive.SubType != CctComponentSubTypes.DefaultDcn.ToString() && defaultDcn != null) {
                componentUser.CctComponents.Insert(componentUser.CctComponents.Count - 1, newDrive);
            }
            else {
                componentUser.CctComponents.Add(newDrive);
            }
        }

        listManager.CompList.Add(newDrive);
        DaManager.UpsertComponentAsync((ComponentModel)newDrive);
        newDrive.PropertyUpdated += DaManager.OnComponentPropertyUpdated;


        return (ComponentModel)newDrive;
    }

    public static LocalControlStationModel CreateLocalControlStation(IComponentUser componentOwner, ListManager listManager)
    {
        UndoManager.CanAdd = false;
        LocalControlStationModel newLcs = new LocalControlStationModel();
        ILoad owner = componentOwner as LoadModel;

        //Id
        if (listManager.LcsList.Count < 1) {
            newLcs.Id = 1;
        }
        else {
            newLcs.Id = listManager.LcsList.Select(c => c.Id).Max() + 1;
        }
        newLcs.Category = Categories.LCS.ToString();
        newLcs.SubCategory = SubCategories.AuxComponent.ToString();
        newLcs.Type = GetLcsType(owner);
        newLcs.TypeId = int.Parse(GetLcsTypeId(owner));
        newLcs.TypeModel = TypeManager.GetLcsTypeModel(newLcs.TypeId);


        newLcs.Owner = componentOwner;
        newLcs.OwnerId = componentOwner.Id;
        newLcs.OwnerType = componentOwner.GetType().ToString();
        newLcs.Tag = componentOwner.Tag + TagSettings.ComponentSuffixSeparator + TagSettings.LcsSuffix;
        newLcs.Area = componentOwner.Area;
        UndoManager.CanAdd = false;

        listManager.LcsList.Add(newLcs);
        DaManager.UpsertLcs(newLcs);
        newLcs.PropertyUpdated += DaManager.OnLcsPropertyUpdated;
        UndoManager.CanAdd = true;

        return newLcs;
    }

    internal static string GetLcsType(ILoad owner)
    {
        if (owner.StandAloneStarterBool == true) {
            return EdtSettings.LcsTypeVsdLoad;
        }
 
        return EdtSettings.LcsTypeDolLoad;
    
    }

    internal static string GetLcsTypeId(ILoad owner)
    {
        if (owner.StandAloneStarterBool == true) {
            return EdtSettings.LcsTypeVsdLoad;
        }

        return EdtSettings.LcsTypeDolLoad;

    }
}
