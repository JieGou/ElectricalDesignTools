using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components;
public class ComponentFactory
{
    public static ComponentModel CreateCircuitComponent(IComponentUser componentUser, string subCategory, string type, string subType, ListManager listManager)
    {
        IComponent newComponent = new ComponentModel();

        if (listManager.CompList.Count<1) {
            newComponent.Id = 1;
        }
        else {
            newComponent.Id = listManager.CompList.Select(c => c.Id).Max() + 1;
        }
        newComponent.Category = Categories.COMPONENT.ToString();
        newComponent.SubCategory = subCategory;
        newComponent.Owner = componentUser;
        newComponent.OwnerId = componentUser.Id;
        newComponent.OwnerType = componentUser.GetType().ToString();
        newComponent.Type = type;
        newComponent.SubType = subType;
        

        //Disconnect
        if (subType == ComponentSubTypes.DefaultDcn.ToString()) {
            newComponent.Tag = componentUser.Tag + TagSettings.SuffixSeparator + TagSettings.DisconnectSuffix;
            newComponent.Area = componentUser.Area;
            var load = (IPowerConsumer)componentUser;
            newComponent.Size = DataTableManager.GetDisconnectSize(load);
            newComponent.SequenceNumber = componentUser.CctComponents.Count+1;
        }

        //Drive
        else if (subType == ComponentSubTypes.DefaultDrive.ToString()) {
            newComponent.Tag = componentUser.Tag + TagSettings.SuffixSeparator + TagSettings.DriveSuffix;
            var powerConsumer = (IPowerConsumer)componentUser;
            newComponent.Area = powerConsumer.FedFrom.Area;
            newComponent.SequenceNumber = 0;
        }


        //Size


        //Order of components
        componentUser.CctComponents.Add(newComponent);
        componentUser.CctComponents = new ObservableCollection<IComponent>(componentUser.CctComponents.OrderBy(c => c.SequenceNumber).ToList());


        listManager.CompList.Add(newComponent);
        DaManager.UpsertComponent((ComponentModel)newComponent);
        newComponent.PropertyUpdated += DaManager.OnComponentPropertyUpdated;
        
        return (ComponentModel)newComponent;
    }

    public static ComponentModel CreateDrive(IComponentUser componentUser, string type, string subType, ListManager listManager)
    {
        IComponent newDrive = new ComponentModel();

        if (listManager.CompList.Count < 1) {
            newDrive.Id = 1;
        }
        else {
            newDrive.Id = listManager.CompList.Select(c => c.Id).Max() + 1;
        }
        newDrive.Category = Categories.COMPONENT.ToString();
        newDrive.SubCategory = SubCategories.CctComponent.ToString();
        newDrive.Owner = componentUser;
        newDrive.OwnerId = componentUser.Id;
        newDrive.OwnerType = componentUser.GetType().ToString();
        newDrive.Type = type;
        newDrive.SubType = subType;


            newDrive.Tag = componentUser.Tag + TagSettings.SuffixSeparator + TagSettings.DriveSuffix;
            var powerConsumer = (IPowerConsumer)componentUser;
            newDrive.Area = powerConsumer.FedFrom.Area;


        //Size


        //Order of components
        if (newDrive.SubCategory == SubCategories.CctComponent.ToString()) {
            var defaultDcn = componentUser.CctComponents.FirstOrDefault(c => c.SubType == ComponentSubTypes.DefaultDcn.ToString());
            if (newDrive.SubType != ComponentSubTypes.DefaultDcn.ToString() && defaultDcn != null) {
                componentUser.CctComponents.Insert(componentUser.CctComponents.Count - 1, newDrive);
            }
            else {
                componentUser.CctComponents.Add(newDrive);
            }
        }

        listManager.CompList.Add(newDrive);
        DaManager.UpsertComponent((ComponentModel)newDrive);
        newDrive.PropertyUpdated += DaManager.OnComponentPropertyUpdated;


        return (ComponentModel)newDrive;
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
            newLcs.Type = EdtSettings.LcsTypeDolLoad;
        }
        else if (owner.DriveBool==true) {
            newLcs.Type = EdtSettings.LcsTypeVsdLoad;
        }
        else {
            newLcs.Type = EdtSettings.LcsTypeDolLoad;
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
