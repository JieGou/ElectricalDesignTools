using EDTLibrary.DataAccess;
using EDTLibrary.Managers;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components;
public class ComponentManager
{
    public static void AddLcs(IComponentUser componentUser, ListManager listManager)
    {
        if (listManager == null) return;
        string subCategory = Categories.AuxComponent.ToString();
        string type = ComponentTypes.LCS.ToString();
        string subType = "Type Of LCS";

        LocalControlStationModel newLcs = ComponentFactory.CreateLocalControlStation(componentUser, subCategory, type, subType, listManager);
        CableManager.AddLcsControlCableForLoad(componentUser, newLcs, listManager);

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            load.Lcs = newLcs;
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
        }
    }
    public static void AddDrive(IComponentUser componentUser, ListManager listManager)
    {
        if (listManager == null) return;
        string subCategory = Categories.CctComponent.ToString();
        string type = ComponentTypes.DefaultDrive.ToString();
        string subType = "Type of Drive";
        ComponentModel newComponent = ComponentFactory.CreateComponent(componentUser, subCategory, type, subType, listManager);
        componentUser.Drive = newComponent;

    }

    public static void RemoveDrive(IComponentUser componentUser, ListManager listManager)
    {
        if (componentUser.GetType() == typeof(LoadModel)) {
            foreach (var component in componentUser.CctComponents) {
                if (component.Type == ComponentTypes.DefaultDrive.ToString()) {
                    componentUser.CctComponents.Remove(component);
                    int componentId = component.Id;
                    var componentToRemote = listManager.CompList.FirstOrDefault(c => c.Id == componentId);
                    listManager.CompList.Remove(componentToRemote);
                    DaManager.DeleteComponent((ComponentModel)component);
                    componentUser.Drive = null;
                    break;
                }
            }
        }
    }
    public static void AddDisconnect(IComponentUser componentUser, ListManager listManager)
    {
        if (listManager == null) return;
        string subCategory = Categories.CctComponent.ToString();
        string type = ComponentTypes.DefaultDcn.ToString();
        string subType = "Type of Disconnect";
        ComponentModel newComponent = ComponentFactory.CreateComponent(componentUser, subCategory, type, subType, listManager);
        componentUser.Disconnect = newComponent;

    }

    public static void RemoveDisconnect(IComponentUser componentUser, ListManager listManager)
    {

        if (componentUser.GetType() == typeof(LoadModel)) {
            foreach (var component in componentUser.CctComponents) {
                if (component.Type == ComponentTypes.DefaultDcn.ToString()) {
                    componentUser.CctComponents.Remove(component);
                    int componentId = component.Id;
                    var componentToRemote = listManager.CompList.FirstOrDefault(c => c.Id == componentId);
                    listManager.CompList.Remove(componentToRemote);
                    DaManager.DeleteComponent((ComponentModel)component);
                    componentUser.Disconnect = new ComponentModel();
                    break;
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

    public static void DeleteComponent(IComponentUser componentUser, IComponent component, ListManager listManager)
    {
        if (component == null) return;

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            if (component.Type == ComponentTypes.DefaultDcn.ToString()) {
                load.DisconnectBool = false;
            }
            if (component.Type == ComponentTypes.DefaultDrive.ToString()) {
                load.DriveBool = false;
            }
        }
        
        componentUser.AuxComponents.Remove(component);
        componentUser.CctComponents.Remove(component);
        listManager.CompList.Remove(component);
        DaManager.DeleteComponent((ComponentModel)component);
    }
}
