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
    public static void CreateLcs(IComponentUser componentUser, ListManager listManager)
    {
        if (listManager == null) return;
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
}
