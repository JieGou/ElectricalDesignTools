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
    public static ComponentModel CreateComponent(IComponentUser componentUser, string componentSubCategory, string componentType, string componentSubType, ListManager listManager)
    {
        ComponentModel newComponent = new ComponentModel();

        if (listManager.CompList.Count<1) {
            newComponent.Id = 1;
        }
        else {
            newComponent.Id = listManager.CompList.Select(c => c.Id).Max() + 1;
        }
        newComponent.Category = Categories.Component.ToString();
        newComponent.SubCategory = componentSubCategory;
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
        if (componentSubCategory == Categories.AuxComponent.ToString()) {
            var lcs = componentUser.AuxComponents.FirstOrDefault(c => c.Type == ComponentTypes.LCS.ToString());

            if (componentType == ComponentTypes.LCS.ToString() && lcs != null) {
                componentUser.AuxComponents.Add(newComponent);

            }
        }

        else if (componentSubCategory == Categories.CctComponent.ToString()) {
            var defaultDcn = componentUser.CctComponents.FirstOrDefault(c => c.Type == ComponentSubTypes.DefaultDcn.ToString());
            if (componentType != ComponentTypes.DefaultDcn.ToString() && defaultDcn != null) {
                componentUser.CctComponents.Insert(componentUser.CctComponents.Count - 1, newComponent);
            }
            else {
                componentUser.CctComponents.Add(newComponent);
            }
        }

        listManager.CompList.Add(newComponent);
        DaManager.UpsertComponent(newComponent);
        newComponent.PropertyUpdated += DaManager.OnComponentPropertyUpdated;


        return newComponent;
    }
}
