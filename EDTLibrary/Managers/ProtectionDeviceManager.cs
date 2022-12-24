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
using EDTLibrary.ProjectSettings;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;

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

            type = PdTypes.MCP_FVNR.ToString();
            subType = PdTypes.MCP_FVNR.ToString();
        }
        else if (load.Category == Categories.DTEQ.ToString()) {

            type = PdTypes.BKR.ToString();
            subType = PdTypes.BKR.ToString();
        }
        else {
            type = PdTypes.BKR.ToString();
            subType = PdTypes.BKR.ToString();
        }


        ProtectionDeviceModel newPd = ComponentFactory.CreateProtectionDevice(load, subCategory, type, subType, listManager);
        load.ProtectionDevice = newPd;
        ProtectionDeviceManager.SetProtectionDeviceType(load);
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


    //Trip and Starter
    public static void SetPdTripAndStarterSize(IPowerConsumer load)
    {
        if (load.ProtectionDevice == null) return;
        var LvCutoff = 750;

        //LV
        if (load.VoltageType.Voltage <= LvCutoff) {
            SetPdTripAndStarterSize_Lv(load); 
        }
        else {
            SetPdTripAndStarterSize_Mv(load);
        }
        //MV
        load.ProtectionDevice.FrameAmps = GetPdFrameAmps(load); 
    }

    private static void SetPdTripAndStarterSize_Lv(IPowerConsumer load)
    {
        if (load.ProtectionDevice == null) {
            return;
        }

        if (load.ProtectionDevice.Type.Contains("MCP") ||
            load.ProtectionDevice.Type.Contains("FVNR") ||
            load.ProtectionDevice.Type.Contains("FVR")) {
            load.ProtectionDevice.TripAmps = DataTableSearcher.GetBreakerTrip(load);
            load.ProtectionDevice.StarterSize = DataTableSearcher.GetStarterSize(load);

        }
        else if (load.ProtectionDevice.Type == "BKR") {
            load.ProtectionDevice.TripAmps = DataTableSearcher.GetBreakerTrip(load);
        }
    }

    private static void SetPdTripAndStarterSize_Mv(IPowerConsumer load)
    {
        double MvContactorSize = 400;

        //Contactor
        if (load.ProtectionDevice.Type.Contains("MCP") || load.ProtectionDevice.Type.Contains("DOL")) {

            load.ProtectionDevice.TripAmps = DataTableSearcher.GetBreakerTrip(load);
            var minContactorSize = Math.Max(load.Fla * load.AmpacityFactor, MvContactorSize);
            load.ProtectionDevice.StarterSize = DataTableSearcher.GetMvContactorSize(minContactorSize).ToString();

        }
        else  {
            load.ProtectionDevice.TripAmps = DataTableSearcher.GetBreakerTrip(load);
        }

    }


    //Frame
    internal static double GetPdFrameAmps(IPowerConsumer load)
    {
        double LvCutoff = 750;
        
        //LV
        if (load.VoltageType.Voltage <= LvCutoff) {
            return DataTableSearcher.GetBreakerFrame(load.ProtectionDevice.TripAmps);

        }

        //MV
        double MvBreakerFrameSize = 1200;
        return DataTableSearcher.GetBreakerFrame(MvBreakerFrameSize);

    }
}
