using EDTLibrary.DataAccess;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace EDTLibrary.Managers;
public class ProtectionDeviceManager
{
    internal static void AddProtectionDevice(IPowerConsumer load, ListManager listManager)
    {

        if (listManager == null) return;

        string category = Categories.COMPONENT.ToString();
        string subCategory = SubCategories.ProtectionDevice.ToString();
        string type = "";
        string subType = "";

        //Motor
        if (load.Type == LoadTypes.MOTOR.ToString()) {

            type = CctComponentTypes.DOL.ToString();
            subType = ComponentSubTypes.StandAloneStarter.ToString();
        }
        //Else
        else {
            type = CctComponentTypes.FDS.ToString();
            subType = ComponentSubTypes.StandAloneDcn.ToString();
        }


        ProtectionDeviceModel newPd = ComponentFactory.CreateProtectionDevice(load, subCategory, type, subType, listManager);
        load.ProtectionDevice = newPd;

        load.FedFrom.AreaChanged += newPd.MatchOwnerArea;
        newPd.PropertyUpdated += DaManager.OnProtectioneDevicePropertyUpdated;

    }

    public static void DeleteProtectionDevices(IPowerConsumer load, ListManager listManager)
    {
        List<IComponentEdt> componentsToRemove = new List<IComponentEdt>();
        
        //StandAlone CctComponents
        foreach (var component in load.CctComponents) {
            if (component.SubType == ComponentSubTypes.StandAloneDcn.ToString() ||
                component.SubType == ComponentSubTypes.StandAloneDrive.ToString() ||
                component.SubType == ComponentSubTypes.StandAloneStarter.ToString()) {
                componentsToRemove.Add(component);

                var compToDelete = listManager.PdList.FirstOrDefault(c => c.Id == component.Id);
                DaManager.DeleteProtectionDevice(compToDelete);

                compToDelete.PropertyUpdated -= DaManager.OnProtectioneDevicePropertyUpdated;
                load.FedFrom.AreaChanged -= component.MatchOwnerArea;
            }
        }
        foreach (var comp in componentsToRemove) {
            load.CctComponents.Remove(comp);
        }

        //ProtectionDevice
        if (load.ProtectionDevice == null) return;

        var pdToRemove = listManager.PdList.FirstOrDefault(c => c.Id == load.ProtectionDevice.Id);
        DaManager.DeleteProtectionDevice(pdToRemove);
        pdToRemove.PropertyUpdated -= DaManager.OnProtectioneDevicePropertyUpdated;
        load.ProtectionDevice = null;
    }
}
