using EDTLibrary.DataAccess;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components;
public class ComponentFactory
{
    public static ComponentModel CreateComponent(IComponentUser componentUser, string componentCategory, string componentType, string componentSubType, ListManager listManager)
    {
        ComponentModel newComponent = new ComponentModel();

        if (listManager.CompList.Count<1) {
            newComponent.Id = 1;
        }
        else {
            newComponent.Id = listManager.CompList.Select(c => c.Id).Max() + 1;
        }
        newComponent.Category = componentCategory;
        newComponent.Owner = componentUser;
        newComponent.OwnerId = componentUser.Id;
        newComponent.OwnerType = componentUser.GetType().ToString();
        newComponent.Type = componentType;
        newComponent.SubType = componentSubType;


        //Tag and Area
        if (componentType == ComponentTypes.DefaultDcn.ToString()) {
            newComponent.Tag = componentUser.Tag + TagSettings.SuffixSeparator + TagSettings.DisconnectSuffix;
            newComponent.Area = componentUser.Area;
        }
        else if (componentType == ComponentTypes.DefaultDrive.ToString()) {
            newComponent.Tag = componentUser.Tag + TagSettings.SuffixSeparator + TagSettings.DriveSuffix;
            var powerConsumer = (IPowerConsumer)componentUser;
            newComponent.Area = powerConsumer.FedFrom.Area;
        }
        else if (componentType == ComponentTypes.LCS.ToString()) {
            newComponent.Tag = componentUser.Tag + TagSettings.SuffixSeparator + TagSettings.LcsSuffix;
            newComponent.Area = componentUser.Area;
        }

        //AuxComponents vs CctComponents
        if (componentCategory == Categories.AuxComponent.ToString()) {
            componentUser.AuxComponents.Add(newComponent);
        }
        else if (componentCategory == Categories.CctComponent.ToString()) {
            componentUser.CctComponents.Add(newComponent);
        }

        listManager.CompList.Add(newComponent);
        DaManager.UpsertComponent(newComponent);

        return newComponent;
    }
}
