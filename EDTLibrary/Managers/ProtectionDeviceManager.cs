﻿using EDTLibrary.DataAccess;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using EDTLibrary.ProjectSettings;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;

namespace EDTLibrary.Managers;
public class ProtectionDeviceManager
{
    internal static void AddProtectionDevice(IPowerConsumer load, ListManager listManager)
    {

        if (listManager == null) return;

        string category = Categories.CctComponent.ToString();
        string subCategory = SubCategories.ProtectionDevice.ToString();
        string type = "";
        string subType = "";

        //Motor
        if (load.Type == LoadTypes.MOTOR.ToString()) {

            type = CctComponentTypes.DOL.ToString();
            subType = PdTypes.StandAloneStarter.ToString();
        }
        if (load.Category == Categories.DTEQ.ToString()) {

            type = PdTypes.BKR.ToString();
            subType = PdTypes.BKR.ToString();
        }
        //Else
        else {
            type = CctComponentTypes.FDS.ToString();
            subType = PdTypes.FDS.ToString();
        }


        ProtectionDeviceModel newPd = ComponentFactory.CreateProtectionDevice(load, subCategory, type, subType, listManager);
        load.ProtectionDevice = newPd;

        load.FedFrom.AreaChanged += newPd.MatchOwnerArea;
        newPd.PropertyUpdated += DaManager.OnProtectioneDevicePropertyUpdated;
        DaManager.UpsertComponentAsync(newPd);
    }

    public static void DeleteProtectionDevices(IPowerConsumer load, ListManager listManager)
    {
        List<IComponentEdt> componentsToRemove = new List<IComponentEdt>();
        
        //StandAlone CctComponents
        foreach (var component in load.CctComponents) {
            if (component.SubType == PdTypes.FDS.ToString() ||
                component.SubType == PdTypes.StandAloneDrive.ToString() ||
                component.SubType == PdTypes.StandAloneStarter.ToString()) {
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

    internal static void SetProtectionDeviceType(IPowerConsumer load)
    {
        if (DaManager.GettingRecords) return;
        if  (load.ProtectionDevice == null) return;

        //load.ProtectionDevice.FrameAmps = DataTableSearcher.GetMcpFrame(load);
        //load.ProtectionDevice.TripAmps = DataTableSearcher.GetBreakerTrip(load);
        //load.ProtectionDevice.StarterSize = DataTableSearcher.GetStarterSize(load);

        //Stand Alone
        if (load.ProtectionDevice.IsStandAlone) {

            if (load.DriveBool) {
                load.ProtectionDevice.Type = PdTypes.FDS.ToString();
            }
            else {

                if (load.Type == LoadTypes.MOTOR.ToString()) {
                    load.ProtectionDevice.Type = EdtSettings.LoadDefaultPdTypeLV_Motor;
                }
                else {
                    load.ProtectionDevice.Type = PdTypes.FDS.ToString();
                }
            }
        }

        //Bucket Type
        else if (load.ProtectionDevice.IsStandAlone == false) {


            if (load.DriveBool) {
                load.ProtectionDevice.Type = PdTypes.BKR.ToString();
            }

            else {

                if (load.Type == LoadTypes.MOTOR.ToString()) {
                    load.ProtectionDevice.Type = EdtSettings.LoadDefaultPdTypeLV_Motor;
                }
                else {
                    load.ProtectionDevice.Type = EdtSettings.LoadDefaultPdTypeLV_NonMotor;
                }
            }
        }
    }
    public static void SetProtectionDeviceFrameAndTrip(IPowerConsumer load)
    {

        if (load.ProtectionDevice == null) {
            return;
        }

        if (load.ProtectionDevice.Type.Contains("MCP") ||
            load.ProtectionDevice.Type.Contains("FVNR") ||
            load.ProtectionDevice.Type.Contains("FVR")) {
            load.ProtectionDevice.FrameAmps = DataTableSearcher.GetMcpFrame(load);
            load.ProtectionDevice.TripAmps = DataTableSearcher.GetBreakerTrip(load);
            load.ProtectionDevice.StarterSize = DataTableSearcher.GetStarterSize(load);

        }
        else if (load.ProtectionDevice.Type == "BKR") {
            load.ProtectionDevice.FrameAmps = DataTableSearcher.GetBreakerFrame(load);
            load.ProtectionDevice.TripAmps = DataTableSearcher.GetBreakerTrip(load);
        }
    }
}
