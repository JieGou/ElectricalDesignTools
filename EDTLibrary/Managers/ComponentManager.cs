using EDTLibrary.DataAccess;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.UndoSystem;
using System;
using System.Collections.Generic;
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
        CableManager.AddLcsControlCableForLoad(componentUser, newLcs, listManager);
        UndoManager.CanAdd = true;

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            load.Lcs = newLcs;
            load.AreaChanged += newLcs.MatchOwnerArea;
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
            CableManager.DeleteLcsControlCable(componentUser, componentToRemove, listManager);
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
            var load = (LoadModel)componentUser;
            foreach (var component in load.CctComponents) {
                if (component.SubType == ComponentSubTypes.DefaultDrive.ToString()) {
                    load.CctComponents.Remove(component);
                    int componentId = component.Id;
                    var componentToRemote = listManager.CompList.FirstOrDefault(c => c.Id == componentId);
                    listManager.CompList.Remove(componentToRemote);
                    DaManager.DeleteComponent((ComponentModel)component);
                    componentUser.Drive = null;
                    break;
                    load.FedFrom.AreaChanged -= component.MatchOwnerArea;

                }
            }
        }
    }

    public static void AddDefaultDisconnect(IComponentUser componentUser, ListManager listManager)
    {
        if (listManager == null) return;
        string subCategory = SubCategories.CctComponent.ToString();
        //Todo - Add setting for local disconnect default type
        string type = "UDS";
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
            foreach (var component in componentUser.CctComponents) {
                if (component.SubType == ComponentSubTypes.DefaultDcn.ToString()) {
                    componentUser.CctComponents.Remove(component);
                    int componentId = component.Id;
                    var componentToRemote = listManager.CompList.FirstOrDefault(c => c.Id == componentId);
                    listManager.CompList.Remove(componentToRemote);
                    DaManager.DeleteComponent((ComponentModel)component);
                    componentUser.Disconnect = new ComponentModel();
                    break;
                    load.AreaChanged -= component.MatchOwnerArea;
                }
            }
        }
    }

    public static void RetagCctComponents(IComponentUser componentUser)
    {

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
}
