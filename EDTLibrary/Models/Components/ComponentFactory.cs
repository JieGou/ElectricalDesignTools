using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeTables;
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
        

        //Tag, Area, Size
        if (componentSubType == ComponentSubTypes.DefaultDcn.ToString()) {
            newComponent.Tag = componentUser.Tag + TagSettings.SuffixSeparator + TagSettings.DisconnectSuffix;
            newComponent.Area = componentUser.Area;
            var load = (IPowerConsumer)componentUser;
            newComponent.Size = LibraryManager.GetDisconnectSize(load);
        }
        else if (componentSubType == ComponentSubTypes.DefaultDrive.ToString()) {
            newComponent.Tag = componentUser.Tag + TagSettings.SuffixSeparator + TagSettings.DriveSuffix;
            var powerConsumer = (IPowerConsumer)componentUser;
            newComponent.Area = powerConsumer.FedFrom.Area;
        }

        //Size


        //Order of components
       if (newComponent.SubCategory == SubCategories.CctComponent.ToString()) {
            var defaultDcn = componentUser.CctComponents.FirstOrDefault(c => c.SubType == ComponentSubTypes.DefaultDcn.ToString());
            if (newComponent.SubType != ComponentSubTypes.DefaultDcn.ToString() && defaultDcn != null) {
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


    public static LocalControlStationModel CreateLocalControlStation(IComponentUser componentOwner, ListManager listManager)
    {
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

        if (owner.PdType.Contains("MCP")) {
            newLcs.Type = EdtSettings.DefaultLcsTypeDolLoad;
        }
        else if (owner.DriveBool==true) {
            newLcs.Type = EdtSettings.DefaultLcsTypeVsdLoad;
        }
        else {
            newLcs.Type = EdtSettings.DefaultLcsTypeDolLoad;
        }
        newLcs.TypeModel = TypeManager.GetLcsTypeModel(newLcs.Type);
        

        newLcs.Owner = componentOwner;
        newLcs.OwnerId = componentOwner.Id;
        newLcs.OwnerType = componentOwner.GetType().ToString();
        newLcs.Tag = componentOwner.Tag + TagSettings.SuffixSeparator + TagSettings.LcsSuffix;
        newLcs.Area = componentOwner.Area;

        listManager.LcsList.Add(newLcs);
        DaManager.UpsertLcs(newLcs);
        newLcs.PropertyUpdated += DaManager.OnComponentPropertyUpdated;

        return newLcs;
    }
}
