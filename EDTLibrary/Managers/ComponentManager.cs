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
    public static void AddDrive(IPowerConsumer componentUser, ListManager listManager)
    {
        if (listManager == null) return;
        CctComponentModel newComponent = new CctComponentModel();

        //TODO - move to component factory
        newComponent.Owner = componentUser;
        newComponent.OwnerId = componentUser.Id;
        newComponent.Tag = componentUser.Tag + TagSettings.SuffixSeparator + TagSettings.DriveSuffix;
        newComponent.Type = "VFD";

        componentUser.CctComponents.Add(newComponent);
        listManager.CompList.Add(newComponent);

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            
        }
    }

    public static void RemoveDrive(IPowerConsumer componentUser, ListManager listManager)
    {
        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            foreach (var component in componentUser.CctComponents) {
                if (component.Type == "VFD") {
                    load.CctComponents.Remove(component);
                    listManager.CompList.Remove(component);
                    break;
                }
            }
        }
    }
    public static void AddDisconnect(IPowerConsumer componentUser, ListManager listManager)
    {
        if (listManager == null) return;
        LocalControlStation newComponent = new LocalControlStation();

        //TODO - move to component factory
        newComponent.Owner = componentUser;
        newComponent.OwnerId = componentUser.Id;
        newComponent.Tag = componentUser.Tag + TagSettings.SuffixSeparator + TagSettings.DisconnectSuffix;
        newComponent.Type = "DCN";

        componentUser.CctComponents.Add(newComponent);
        listManager.CompList.Add(newComponent);

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            load.Lcs = newComponent;
        }
    }

    public static void RemoveDisconnect(IPowerConsumer componentUser, ListManager listManager)
    {

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            foreach (var component in componentUser.CctComponents) {
                if (component.Type == "DCN") {
                    load.CctComponents.Remove(component);
                    listManager.CompList.Remove(component);
                    break;
                }
            }
        }
    }

    public static void AddLcs(IComponentUser componentUser, ListManager listManager)
    {
        if (listManager == null) return;

        //TODO - move to component factory
        LocalControlStation newLcs = new LocalControlStation();
        newLcs.Owner = componentUser;
        newLcs.OwnerId = componentUser.Id;
        newLcs.Tag = componentUser.Tag + TagSettings.SuffixSeparator + TagSettings.LcsSuffix;

        componentUser.Components.Add(newLcs);
        listManager.CompList.Add(newLcs);

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            load.Lcs = newLcs;
        }
    }

    public static void RemoveLcs(IComponentUser componentUser, ListManager listManager)
    {

        if (componentUser.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)componentUser;
            load.Components.Remove(load.Lcs);
            listManager.CompList.Remove(load.Lcs);
            load.Lcs = null;
        }
    }

    public static void DeleteComponents(IComponentUser componentUser, ListManager listManager)
    {
        foreach (var item in componentUser.Components) {
            listManager.CompList.Remove(item);
        }
        IPowerConsumer load = (IPowerConsumer)componentUser;
        foreach (var item in load.CctComponents) {
            listManager.CompList.Remove(item);
        }
    }
}
