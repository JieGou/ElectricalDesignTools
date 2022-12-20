﻿using EDTLibrary.DataAccess;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.UndoSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Managers;
public class ComponentManager
{
    public static void AddLcs(IComponentUser componentUser, ListManager listManager)
    {
        
        if (listManager == null) return;

        LocalControlStationModel newLcs = ComponentFactory.CreateLocalControlStation(componentUser, listManager);
        UndoManager.CanAdd = false;
        componentUser.Lcs = newLcs;
        CableManager.AddLcsCables(componentUser, newLcs, listManager);
        UndoManager.CanAdd = true;

        if (componentUser.GetType() == typeof(LoadModel)) {
            var eq = (IEquipment)componentUser;
            eq.AreaChanged += newLcs.MatchOwnerArea;
        }
    }

    public static void RemoveLcs(IComponentUser componentUser, ListManager listManager)
    {
        if (componentUser.Lcs == null) return;

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            int componentId = load.Lcs.Id;
            var componentToRemove = listManager.LcsList.FirstOrDefault(c => c.Id == componentId);
            listManager.LcsList.Remove(componentToRemove);
            DaManager.DeleteLcs((LocalControlStationModel)load.Lcs);
            CableManager.DeleteLcslCables(componentUser, componentToRemove, listManager);
            load.Lcs.PropertyUpdated -= DaManager.OnLcsPropertyUpdated;
            load.Lcs = null;
            load.AreaChanged -= componentToRemove.MatchOwnerArea;
        }
    }
    public static void AddDefaultDrive(IComponentUser componentUser, ListManager listManager)
    {
        if (listManager == null) return;

        string subCategory = SubCategories.CctComponent.ToString();
        string type = ComponentTypes.VSD.ToString();
        string subType = ComponentSubTypes.DefaultDrive.ToString();

        ComponentModel newComponent = ComponentFactory.CreateCircuitComponent(componentUser, subCategory, type, subType, listManager);
        componentUser.Drive = newComponent;

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            load.FedFrom.AreaChanged += newComponent.MatchOwnerArea;
        }

    }

    public static void RemoveDefaultDrive(IComponentUser componentUser, ListManager listManager)
    {
        if (componentUser.Drive == null) return;
        if (componentUser.GetType() == typeof(LoadModel)) {
            IComponentEdt componentToRemove = new ComponentModel();
            var load = (LoadModel)componentUser;
            foreach (var component in load.CctComponents) {
                if (component.SubType == ComponentSubTypes.DefaultDrive.ToString()) {
                    //load.CctComponents.Remove(component);
                    int componentId = component.Id;
                    componentToRemove = listManager.CompList.FirstOrDefault(c => c.Id == componentId);
                    listManager.CompList.Remove(componentToRemove);
                    DaManager.DeleteComponent((ComponentModel)component);
                    componentUser.Drive.PropertyUpdated -= DaManager.OnComponentPropertyUpdated;
                    componentUser.Drive = null;
                    //break;
                    load.FedFrom.AreaChanged -= component.MatchOwnerArea;

                }
            }
            load.CctComponents.Remove(componentToRemove);
        }
    }

    public static void AddDefaultDisconnect(IComponentUser componentUser, ListManager listManager)
    {
        //Todo - Add setting for local disconnect default type

        if (listManager == null) return;

        string subCategory = SubCategories.CctComponent.ToString();
        string type = ComponentTypes.UDS.ToString();
        string subType = ComponentSubTypes.DefaultDcn.ToString();

        ComponentModel newComponent = ComponentFactory.CreateCircuitComponent(componentUser, subCategory, type, subType, listManager);

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            load.Disconnect = newComponent;
            load.AreaChanged += newComponent.MatchOwnerArea;
        }
    }

    public static void RemoveDefaultDisconnect(IComponentUser componentUser, ListManager listManager)
    {
        if (componentUser.Disconnect == null) return;
        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            IComponentEdt componentToRemove = new ComponentModel();
            foreach (var component in componentUser.CctComponents) {
                if (component.SubType == ComponentSubTypes.DefaultDcn.ToString()) {
                    //componentUser.CctComponents.Remove(component);
                    int componentId = component.Id;
                    componentToRemove = listManager.CompList.FirstOrDefault(c => c.Id == componentId);
                    listManager.CompList.Remove(componentToRemove);
                    DaManager.DeleteComponent((ComponentModel)component);
                    componentUser.Disconnect.PropertyUpdated -= DaManager.OnComponentPropertyUpdated;
                    componentUser.Disconnect = null;
                    //break;
                    load.AreaChanged -= component.MatchOwnerArea;
                }
            }
            componentUser.CctComponents.Remove(componentToRemove);
        }
    }

 
    /// <summary>
    /// Deletes all components owned by a components user. Typically when deleting a component user.
    /// </summary>
    /// <param name="componentUser"></param>
    /// <param name="listManager"></param>
    public static void DeleteComponents(IComponentUser componentUser, ListManager listManager)
    {
        foreach (var item in componentUser.AuxComponents) {
            listManager.CompList.Remove(item);
            DaManager.DeleteComponent((ComponentModel)item);
        }
        foreach (var item in componentUser.CctComponents) {
            listManager.CompList.Remove(item);
            DaManager.DeleteComponent((ComponentModel)item);
        }
        DaManager.DeleteLcs((LocalControlStationModel)componentUser.Lcs);
    }

    public static void DeleteComponent(IComponentUser componentUser, IComponentEdt component, ListManager listManager)
    {
        if (component == null) return;

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            if (component.SubType == ComponentSubTypes.DefaultDcn.ToString()) {
                load.DisconnectBool = false;
            }
            if (component.SubType == ComponentSubTypes.DefaultDrive.ToString()) {
                load.DriveBool = false;
            }
        }

        componentUser.AuxComponents.Remove(component);
        componentUser.CctComponents.Remove(component);
        listManager.CompList.Remove(component);
        DaManager.DeleteComponent((ComponentModel)component);
    }

    internal static void AddSplitterLoadProtectionDevice(IPowerConsumer supplier, IPowerConsumer componentUser, ListManager listManager)
    {
        //Todo - Add setting for local disconnect default type

        if (listManager == null) return;

        string category = Categories.COMPONENT.ToString();
        string subCategory = SubCategories.ProtectionDevice.ToString();
        string type = CctComponentTypes.FDS.ToString();
        string subType = ComponentSubTypes.StandAloneDcn.ToString();

        //was working on this, cables are added seperately by AddAndUpdateLoadPowerComponentCablesAsync
        ProtectionDeviceModel newPd = ComponentFactory.CreateProtectionDevice(componentUser, subCategory, type, subType, listManager);
        newPd.PropertyUpdated += DaManager.OnProtectioneDevicePropertyUpdated;
        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            load.Disconnect = newPd;
            load.FedFrom.AreaChanged += newPd.MatchOwnerArea;
        }
    }

    internal static void RemoveSplitterLoadProtectionDevice(SplitterModel splitterModel, IPowerConsumer load, ListManager listManager)
    {
        IComponentEdt componentToRemove = new ComponentModel();

        foreach (var component in load.CctComponents) {
            if (component.SubType == ComponentSubTypes.StandAloneDcn.ToString()) {
                componentToRemove = component;

                int pdId = component.Id;
                var pdToRemove = listManager.ProtectionDeviceList.FirstOrDefault(c => c.Id == pdId);
                //listManager.ProtectionDeviceList.Remove(pdToRemove);
                //DaManager.DeleteProtectionDevice(pdToRemove);
                pdToRemove.PropertyUpdated -= DaManager.OnProtectioneDevicePropertyUpdated;

                load.ProtectionDevice = null;
                //break;
                splitterModel.AreaChanged -= component.MatchOwnerArea;
            }
        }
        load.CctComponents.Remove(componentToRemove);

    }
}
