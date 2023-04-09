using EDTLibrary.DataAccess;
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
using EDTLibrary.Settings;
using EdtLibrary.Calculators.TransformerPd;
using EdtLibrary.Models.Components.ProtectionDevices;

namespace EDTLibrary.Managers;
public class ProtectionDeviceManager
{

    //public static IProtectionDeviceSizer CableSizer { get; set; }
    internal static void AddProtectionDevice(IPowerConsumer load, ListManager listManager)
    {

        if (listManager == null) return;

        string category = Categories.CctComponent.ToString();
        string subCategory = CctCompSubCategories.ProtectionDevice.ToString();
        string type = "";
        string subType = "";

        //Motor
        if (load.Type == LoadTypes.MOTOR.ToString()) {

            type = PdTypes.MCP_FVNR.ToString();
            subType = PdTypes.MCP_FVNR.ToString();
        }
        else if (load.Category == Categories.DTEQ.ToString()) {

            type = PdTypes.Breaker.ToString();
            subType = PdTypes.Breaker.ToString();
        }
        else {
            type = PdTypes.Breaker.ToString();
            subType = PdTypes.Breaker.ToString();
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
        listManager.PdList.Remove(pdToRemove);
    }

    internal static void SetProtectionDeviceType(IPowerConsumer load)
    {
        if (DaManager.GettingRecords) return;
        if  (load.ProtectionDevice == null) return;


        //Stand Alone
        if (load.ProtectionDevice.IsStandAlone) {

            if (load.StandAloneStarterBool) {
                load.ProtectionDevice.Type = PdTypes.FDS.ToString();
            }
            else {

                if (load.Type == LoadTypes.MOTOR.ToString()) {
                    load.ProtectionDevice.Type = EdtProjectSettings.LoadDefaultPdTypeLV_Motor;
                }
                else {
                    load.ProtectionDevice.Type = PdTypes.FDS.ToString();
                }
            }
        }

        //Bucket Type
        else if (load.ProtectionDevice.IsStandAlone == false) {


            if (load.StandAloneStarterBool) {
                load.ProtectionDevice.Type = PdTypes.Breaker.ToString();
            }

            else {

                if (load.Type == LoadTypes.MOTOR.ToString()) {
                    load.ProtectionDevice.Type = EdtProjectSettings.LoadDefaultPdTypeLV_Motor;
                }
                else {
                    load.ProtectionDevice.Type = EdtProjectSettings.LoadDefaultPdTypeLV_NonMotor;
                }
            }
        }
    }


    public static void SetProtectionDevice(IComponentEdt component)
    {
        if (component == null) return;
        if (component.IsCalculationLocked) return;

        IPowerConsumer eq = (IPowerConsumer)component.Owner;
        
        // transformers
        if (eq.Type == DteqTypes.XFR.ToString()) {
            
            var xfr = (XfrModel)eq;
            SetProtectionDevice_Transformer(component, xfr);
        }

        //all other loads
        else {
            SetProtectionDevice_Load(component, eq);
        }
    }

    private static void SetProtectionDevice_Transformer(IComponentEdt component, XfrModel xfr)
    {
        var LvCutoff = 750;
        //LV
        if (xfr.VoltageType.Voltage <= LvCutoff) {
            SetPdSize_Transformer_Lv(component, xfr);
        }
        //MV
        else {
            SetPdSize_Transformer_Mv(component, xfr);
        }
        component.FrameAmps = GetPdFrameAmps(component, xfr);
    }

    private static void SetPdSize_Transformer_Lv(IComponentEdt component, XfrModel xfr)
    {

        var xfrPdCalc = new TransformerPdCalculator_Over750();
        component.TripAmps = DataTableSearcher.GetBreakerTrip(xfrPdCalc.CalculateOvercurrentProtectionSize(xfr));

        component.TripAmps = DataTableSearcher.GetBreakerTrip(xfr);
    }

    private static void SetPdSize_Transformer_Mv(IComponentEdt component, XfrModel xfr)
    {

        component.TripAmps = DataTableSearcher.GetBreakerTrip(xfr);
    }

    

    private static void SetProtectionDevice_Load(IComponentEdt component, IPowerConsumer load)
    {
        var LvCutoff = 750;

        //LV
        if (load.VoltageType.Voltage <= LvCutoff) {
            SetPdTripAndStarterSize_Load_Lv(component, load);
        }
        //MV
        else {
            SetPdTripAndStarterSize_Load_Mv(component, load);
        }
        component.FrameAmps = GetPdFrameAmps(component, load);
    }

    private static void SetPdTripAndStarterSize_Load_Lv(IComponentEdt component, IPowerConsumer load)
    {
       
        if (component == null) return;
        if (component.Type == null) return;


        if (component.Type.Contains("DOL") ||
            component.Type.Contains("MCP") ||
            component.Type.Contains("FVNR") ||
            component.Type.Contains("FVR")) {
            component.TripAmps = DataTableSearcher.GetBreakerTrip(load);
            component.StarterSize = DataTableSearcher.GetStarterSize(load);

        }
        else if (component.Type == "Breaker") {
            component.TripAmps = DataTableSearcher.GetBreakerTrip(load);
        }
        else if (component.Type == "FDS") {
            component.TripAmps = DataTableSearcher.GetDisconnectFuse(load);
        }
    }

    private static void SetPdTripAndStarterSize_Load_Mv(IComponentEdt component, IPowerConsumer load)
    {
        double MvContactorSize = 400;

        if (component.Type == null) return;


        //Contactor
        if (component.Type.Contains("MCP") || component.Type.Contains("DOL")) {

            component.TripAmps = DataTableSearcher.GetBreakerTrip(load);
            var minContactorSize = Math.Max(load.Fla * load.AmpacityFactor, MvContactorSize);
            component.StarterSize = DataTableSearcher.GetMvContactorSize(minContactorSize).ToString();

        }
        else {
            component.TripAmps = DataTableSearcher.GetBreakerTrip(load);
        }

    }

    
    internal static double GetPdFrameAmps(IComponentEdt component, IPowerConsumer load)
    {
        double LvCutoff = 750;

        if (load.ProtectionDevice == null) return 0;

        //LV
        if (load.VoltageType.Voltage <= LvCutoff) {

            if (component.Type == PdTypes.Breaker.ToString()) {
                return DataTableSearcher.GetBreakerFrame(load.ProtectionDevice.TripAmps*component.AmpacityFactor);
            }
            else if (component.Type == PdTypes.FDS.ToString()) {
                // needs to select motor rated frame
                if (((IPowerConsumer)component.Owner).Type == LoadTypes.MOTOR.ToString()) {
                    return DataTableSearcher.GetDisconnectSize(load);
                }
                return DataTableSearcher.GetDisconnectSize(load, component.TripAmps);
            }
            else if (component.Type == CctComponentTypes.UDS.ToString()){
                return DataTableSearcher.GetDisconnectSize(load);
            }
            else if (component.Type.Contains("MCP") || component.Type.Contains("DOL")) {
                return DataTableSearcher.GetMcpFrame(load);
            }
        }

        //MV
        double MvBreakerFrameSize = 1200;
        return DataTableSearcher.GetBreakerFrame(MvBreakerFrameSize);
        

    }

    
}
