using EDTLibrary.DataAccess;
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
    public static void AddDrive(IComponentUser componentUser, ListManager listManager)
    {
        if (listManager == null) return;
        string category = Categories.CctComponent.ToString();
        string type = ComponentTypes.DefaultDrive.ToString();
        string subType = "Type of Drive";
        ComponentModel newComponent = ComponentFactory.CreateComponent(componentUser, category, type, subType, listManager);
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
                    break;
                }
            }
        }
    }
    public static void AddDisconnect(IComponentUser componentUser, ListManager listManager)
    {
        if (listManager == null) return;
        string category = Categories.CctComponent.ToString();
        string type = ComponentTypes.DefaultDcn.ToString();
        string subType = "Type of Disconnect";
        ComponentModel newComponent = ComponentFactory.CreateComponent(componentUser, category, type, subType, listManager);
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

                    break;
                }
            }
        }
    }

    public static void AddLcs(IComponentUser componentUser, ListManager listManager)
    {
        if (listManager == null) return;
        string category = Categories.AuxComponent.ToString();
        string type = ComponentTypes.LCS.ToString();
        string subType = "Type Of LCS";
        ComponentModel newComponent = ComponentFactory.CreateComponent(componentUser, category, type, subType, listManager);

        //Todo Local Control Station Model
        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            load.Lcs = newComponent;
        }
    }

    public static void RemoveLcs(IComponentUser componentUser, ListManager listManager)
    {

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            load.AuxComponents.Remove(load.Lcs);
            int componentId = load.Lcs.Id;
            var componentToRemote = listManager.CompList.FirstOrDefault(c => c.Id == componentId);
            listManager.CompList.Remove(componentToRemote);
            DaManager.DeleteComponent((ComponentModel)load.Lcs);

            load.Lcs = null;
        }
    }


    public static void RetagCctComponents(IComponentUser componentUser)
    {

    }
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
    }
}
