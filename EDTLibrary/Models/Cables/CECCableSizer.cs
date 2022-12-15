using EDTLibrary.A_Helpers;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.Cables;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables
{
    public class CecCableSizer : ICableSizer
    {
        
        public CecCableSizer()
        {

        }

        private ICable _cable;
        public ICable Cable
        {
            get { return _cable; }
            set { _cable = value; }
        }

        public string GetDefaultCableType(IPowerConsumer load)
        {
            if (load is ILoad
                && load.Voltage > 300
                && load.Voltage <= 1000) {
                return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_3ph300to1kV)).Type;
            }

            else if (load is IDteq && load.Voltage <= 1000) {
               
                //208V - 3phase panels and splitters
                if (load.VoltageType.Voltage == 208 && load.VoltageType.Phase == 3) {
                    if (load.Type == DteqTypes.CDP.ToString() || load.Type == DteqTypes.DPN.ToString()) {
                        return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_4wire)).Type;
                    }
                }
                else if (load.VoltageType.Voltage == 240) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeDteq_3ph1kVLt1200A)).Type;
                }
                else if (load.VoltageType.Voltage == 120) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_2wire)).Type;
                }
                else if (load.Fla <= 1200) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeDteq_3ph1kVLt1200A)).Type;
                }
                else if (load.Fla > 1200) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeDteq_3ph1kVGt1200A)).Type;
                }
            }
            
            else if (load.Voltage > 1000 
                && load.Voltage <= 5000) {
                return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableType_3ph5kV)).Type;
            }
            else if (load.Voltage > 5000
                && load.Voltage <= 15000) {
                return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableType_3ph15kV)).Type;
            }
            return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_3ph300to1kV)).Type;
        }
        public int GetDefaultCableTypeId(IPowerConsumer load)
        {
            if (load is ILoad
                && load.Voltage > 300
                && load.Voltage <= 1000) {
                return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_3ph300to1kV)).Id;
            }

            else if (load is IDteq && load.Voltage <= 1000) {

                //208V - 3phase panels and splitters
                if (load.VoltageType.Voltage == 208 && load.VoltageType.Phase == 3) {
                    if (load.Type == DteqTypes.CDP.ToString() || load.Type == DteqTypes.DPN.ToString()) {
                        return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_4wire)).Id;
                    }
                }
                else if (load.VoltageType.Voltage == 240) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeDteq_3ph1kVLt1200A)).Id;
                }
                else if (load.VoltageType.Voltage == 120) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_2wire)).Id;
                }
                else if (load.Fla <= 1200) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeDteq_3ph1kVLt1200A)).Id;
                }
                else if (load.Fla > 1200) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeDteq_3ph1kVGt1200A)).Id;
                }
            }

            else if (load.Voltage > 1000
                && load.Voltage <= 5000) {
                return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableType_3ph5kV)).Id;
            }
            else if (load.Voltage > 5000
                && load.Voltage <= 15000) {
                return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableType_3ph15kV)).Id;
            }
            return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_3ph300to1kV)).Id;
        }
        public CableTypeModel GetDefaultCableTypeModel(IPowerConsumer load)
        {
            if (load is ILoad
                && load.Voltage > 300
                && load.Voltage <= 1000) {
                return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_3ph300to1kV));
            }

            else if (load is IDteq && load.Voltage <= 1000) {

                //208V - 3phase panels and splitters
                if (load.VoltageType.Voltage == 208 && load.VoltageType.Phase == 3) {
                    if (load.Type == DteqTypes.CDP.ToString() || load.Type == DteqTypes.DPN.ToString()) {
                        return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_4wire));
                    }
                }
                else if (load.VoltageType.Voltage == 240) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeDteq_3ph1kVLt1200A));
                }
                else if (load.VoltageType.Voltage == 120) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_2wire));
                }
                else if (load.Fla <= 1200) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeDteq_3ph1kVLt1200A));
                }
                else if (load.Fla > 1200) {
                    return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeDteq_3ph1kVGt1200A));
                }
            }

            else if (load.Voltage > 1000
                && load.Voltage <= 5000) {
                return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableType_3ph5kV));
            }
            else if (load.Voltage > 5000
                && load.Voltage <= 15000) {
                return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableType_3ph15kV));
            }
            return TypeManager.CableTypes.FirstOrDefault(ct => ct.Id == int.Parse(EdtSettings.DefaultCableTypeLoad_3ph300to1kV));
        }
        public bool IsUsingStandardSizingTable(ICable cable)
        {
            if (cable.AmpacityTable == "Table 1" || cable.AmpacityTable == "Table 2" || cable.AmpacityTable == "Table 3" || cable.AmpacityTable == "Table 4") {
                return true;
            }
            return false;
        }

        public double GetDefaultCableSpacing(ICable cable)
        {
            double spacing = 100;
            if (cable == null) return spacing;

            //TODO - cable spacing defaults vs lock value vs auto-size/spacing option
            CableTypeModel cableType = TypeManager.GetCableTypeModel(cable.Type);
            if (cableType.VoltageRating > 2000 || cableType.ConductorQty == 1) {
                if (cable.Spacing <100) {
                    spacing = cable.Spacing;
                }
                else {
                    spacing = 100;
                }
            }
            else if (cableType.VoltageRating <= 2000 &&
                     cable.Load.Fla >= double.Parse(EdtSettings.CableSpacingMaxAmps_3C1kV)) {
                spacing = 100;
            }
            else if (cableType.VoltageRating < 2000 &&
                     (cableType.ConductorQty == 2 || cableType.ConductorQty == 3 || cableType.ConductorQty == 4 ) &&
                     cable.Load.Fla <= double.Parse(EdtSettings.CableSpacingMaxAmps_3C1kV)) {
                spacing = 0;
            }
            cable.Spacing = spacing;
            return spacing;
        }
        
        //TODO - Change Tables to Enum "CecTables"
        public string GetAmpacityTable(ICable cable, bool checkSizeCutoff = true)
        {
            if (cable == null) return "Invalid Cable Data";

            string output = String.Empty;
            CableTypeModel cableType = TypeManager.GetCableTypeModel(cable.Type);

            if (cable.InstallationType == GlobalConfig.CableInstallationType_LadderTray) {
                output = GetAmpacityTable_LadderTray(cable, cableType);
            }
            else if (cable.InstallationType == GlobalConfig.CableInstallationType_DirectBuried) {
                output = GetAmpacityTable_DirectBuried(cable, cableType, checkSizeCutoff);
            }
            else if (cable.InstallationType == GlobalConfig.CableInstallationType_RacewayConduit) {
                output = GetAmpacityTable_RacewayConduit(cable, cableType, checkSizeCutoff);
            }

            return output;
        }

        internal static string GetAmpacityTable_LadderTray(ICable cable, CableTypeModel cableType)
        {
            string output = "No Table Assigned";

            // 1C, >=5kV <=15kV, Shielded, 100% spacing
            if (cableType.ConductorQty == 1
                        && cableType.VoltageRating >= 5000
                        && cableType.VoltageRating <= 15000
                        && cableType.Shielded == true
                        && cable.Spacing >=100) {

                if (cable.IsOutdoor == false) {
                    output = "Table D17M15In";
                }
                else if (cable.IsOutdoor == true) {
                    output = "Table D17M15Out";
                }
            }

            // 1C, >=25kV <=46kV, Shielded, 100% spacing
            else if (cableType.ConductorQty == 1
                  && cableType.VoltageRating >= 25000
                  && cableType.VoltageRating <= 46000
                  && cableType.Shielded == true
                  && cable.Spacing >= 100) {

                if (cable.IsOutdoor == false) {
                    output = "Table D17M46In";
                }
                else if (cable.IsOutdoor == true) {
                    output = "Table D17M46Out";
                }
            }

            // 1C or 3C, >=5kV <=15kV, Shielded, no spacing
            else if ((cableType.ConductorQty == 3 || cable.Spacing < 100)
                  && cableType.VoltageRating >= 5000
                  && cableType.VoltageRating <= 15000
                  && cableType.Shielded == true) {

                if (cable.IsOutdoor == false) {
                    output = "Table D17N15In";
                }
                else if (cable.IsOutdoor == true) {
                    output = "Table D17N15Out";
                }
            }

            // 3C, >=25kV <=46kV, Shielded, Shielded, no spacing
            else if ((cableType.ConductorQty == 3 || cable.Spacing < 100)
                  && cableType.VoltageRating >= 25000
                  && cableType.VoltageRating <= 46000
                  && cableType.Shielded == true) {

                if (cable.IsOutdoor == false) {
                    output = "Table D17N46In";
                }
                else if (cable.IsOutdoor == true) {
                    output = "Table D17N46Out";
                }
            }

            // 1, 2, 3, or 4C <= 5kV, Non-Shielded
            else {
                if (cable.Type.Contains("DLO")) {
                    output = "Table 12E";
                }
                else if (cable.TypeModel.ConductorQty == 1) {
                    output = "Table 1";
                }
                else if (cable.TypeModel.ConductorQty >= 2) {
                    output = "Table 2";
                }
            }
            return output;
        }
        internal static string GetAmpacityTable_DirectBuried(ICable cable, CableTypeModel cableType, bool checkSizeCutoff = true)
        {
            string output;

            // 1C, >= 5kV, Shielded
            if (cableType.ConductorQty == 1
                        && cableType.VoltageRating >= 5000
                        && cableType.Shielded == true) {

                output = "Table not yet added to database.";
            }

            // 3C, >=5kV, Shielded       Table D17N 3C QtyParallel max = 2
            else if (cableType.ConductorQty == 3
            && cableType.VoltageRating >= 5000
            && cableType.Shielded == true) {

                output = "Table not yet added to database.";
            }

            // 1C, <= 5kV, Non-Shielded
            else if (cableType.ConductorQty == 1
                        && cableType.VoltageRating <= 5000
                        && cableType.Shielded == false) {
                if (checkSizeCutoff) {
                    var maxAmps = TypeManager.CecCableAmpacities.FirstOrDefault(ca => ca.Size == "1" && ca.AmpacityTable == "Table 1").Amps75;

                    var sizeNumber_1ought = 100;
                    var sizeNumber = TypeManager.CableSizeIds.FirstOrDefault(c => c.SizeTag == cable.Size).SizeNumber;
                    //if (cable.RequiredSizingAmps <= maxAmps && cable.Derating5C != 1) {
                    if (cable.RequiredSizingAmps <= maxAmps && sizeNumber < sizeNumber_1ought) {
                        output = "Table 1";
                    }
                    else {
                        output = "Table D8A";
                    }
                }
                else {
                    output = "Table 1";
                }
            }

            // 3C, <= 5kV, Non-Shielded
            else {

                if (checkSizeCutoff) {
                    var maxAmps = TypeManager.CecCableAmpacities.FirstOrDefault(ca => ca.Size == "1" && ca.AmpacityTable == "Table 2").Amps75;

                    var sizeNumber_1ought = 100;
                    var sizeNumber = TypeManager.CableSizeIds.FirstOrDefault(c => c.SizeTag == cable.Size).SizeNumber;
                    //if (cable.RequiredSizingAmps <= maxAmps && cable.Derating5C != 1) {
                    if (cable.RequiredSizingAmps <= maxAmps && sizeNumber < sizeNumber_1ought) {
                            output = "Table 2";
                    }
                    else {
                        output = "Table D10A";
                    }
                }
                else {
                    output = "Table 2";
                }
            }
            return output;
        }
        internal static string GetAmpacityTable_RacewayConduit(ICable cable, CableTypeModel cableType, bool checkSizeCutoff = true)
        {
            string output;

            // 1C, >= 5kV, Shielded
            if (cableType.ConductorQty == 1
                        && cableType.VoltageRating >= 5000
                        && cableType.Shielded == true) {

                output = "Table not yet added to database.";
            }

            // 3C, >=5kV, Shielded       Table D17N 3C QtyParallel max = 2
            else if (cableType.ConductorQty == 3
            && cableType.VoltageRating >= 5000
            && cableType.Shielded == true) {

                output = "Table not yet added to database.";
            }

            // 1C, <= 5kV, Non-Shielded
            else if (cableType.ConductorQty == 1
                        && cableType.VoltageRating <= 5000
                        && cableType.Shielded == false) {

                if (checkSizeCutoff) {
                    var maxAmps = TypeManager.CecCableAmpacities.FirstOrDefault(ca => ca.Size == "1" && ca.AmpacityTable == "Table 1").Amps75;

                    var sizeNumber_1ought = 100;
                    var sizeNumber = TypeManager.CableSizeIds.FirstOrDefault(c => c.SizeTag == cable.Size).SizeNumber;
                    //if (cable.RequiredSizingAmps <= maxAmps && cable.Derating5C != 1) {
                    if (cable.RequiredSizingAmps <= maxAmps && sizeNumber < sizeNumber_1ought) {
                        output = "Table 1";
                    }
                    else {
                        output = "Table D9A";
                    }
                }
                else {
                    output = "Table 1";
                }
            }

            // 3C, <= 5kV, Non-Shielded
            else {
                if (checkSizeCutoff) {
                    var maxAmps = TypeManager.CecCableAmpacities.FirstOrDefault(ca => ca.Size == "1" && ca.AmpacityTable == "Table 2").Amps75;

                    var sizeNumber_1ought = 100;
                    var sizeNumber = TypeManager.CableSizeIds.FirstOrDefault(c => c.SizeTag == cable.Size).SizeNumber;
                    //if (cable.RequiredSizingAmps <= maxAmps && cable.Derating5C != 1) {
                    if (cable.RequiredSizingAmps <= maxAmps && sizeNumber < sizeNumber_1ought) {
                        output = "Table 2";
                    }
                    else {
                        output = "Table D11A";
                    }
                }
                else {
                    output = "Table 2";
                }
            }
            return output;
        }


        public double SetDerating(ICable cable)
        {
            if (cable == null) return 0;

            if (cable.UsageType == CableUsageTypes.Control.ToString()) {
                return 1;
            }
            double derating = 1;
            cable.Derating5A = 1;
            cable.Derating5C = 1;

            if (cable == null) return derating;

            try {
                if (cable.Load != null && cable.Load.FedFrom != null) {
                    double loadCount = cable.Load.FedFrom.AssignedLoads.Count;

                    if (cable.Load.Area != null && cable.Load.FedFrom.Area != null) {

                        double cableAmbientTemp = Math.Max(cable.Load.Area.MaxTemp, cable.Load.FedFrom.Area.MaxTemp);

                        if (cableAmbientTemp > 30) {
                            cable.Derating5A = GetCableDerating_Table5A(cable, cableAmbientTemp);
                            derating *= cable.Derating5A;
                        }
                    }
                }

                // NOT Ladder tray
                if (cable.InstallationType != "LadderTray") {

                    string tempAmpacityTable = "";
                    CableTypeModel cableType = TypeManager.GetCableTypeModel(cable.TypeId);

                    if (cable.InstallationType == GlobalConfig.CableInstallationType_DirectBuried) {
                        tempAmpacityTable = GetAmpacityTable_DirectBuried(cable, cableType, false);
                    }
                    else if (cable.InstallationType == GlobalConfig.CableInstallationType_RacewayConduit) {
                        tempAmpacityTable = GetAmpacityTable_RacewayConduit(cable, cableType, false);
                    }

                    var maxAmps = TypeManager.CecCableAmpacities.FirstOrDefault(ca => ca.Size == "1" && ca.AmpacityTable == tempAmpacityTable).Amps75;

                    //check with full derating if the cable will be smaller than 1/0
                    var tempDerating_5C = GetCableDerating_Table5C(cable);
                    var tempDerating = derating * tempDerating_5C;
                    var tempRequiredSizingAmps = cable.RequiredAmps / tempDerating;

                    var sizeNumber_1ought = 100;
                    var sizeNumber = TypeManager.CableSizeIds.FirstOrDefault(c => c.SizeTag == cable.Size).SizeNumber;

                    if (sizeNumber < sizeNumber_1ought) {
                        if (cable.Spacing < 100) {
                            cable.Derating5C = GetCableDerating_Table5C(cable);
                            derating *= cable.Derating5C;
                        }

                    }
                }

                //Ladder Tray
                else if (cable.Spacing < 100) {
                    if (cable.AmpacityTable == "Table 1" || cable.AmpacityTable == "Table 2" || cable.AmpacityTable == "Table 3" || cable.AmpacityTable == "Table 4") {
                        cable.Derating5C = GetCableDerating_Table5C(cable);
                        derating *= cable.Derating5C;
                    }
                }
            }
            catch (Exception ex) {
                throw;
            }

            derating = Math.Round(derating, 2);
            cable.Derating = derating;
            return derating;
        }

        private double GetCableDerating_Table5A(ICable cable, double ambientTemp)
        {
            double derating = 1;
            int deratingTemp;
            deratingTemp = GetDeratingTemperature(ambientTemp);
            derating = DataTableSearcher.GetCableDerating_CecTable5A(cable, deratingTemp);

            return derating;
        }
        private static int GetDeratingTemperature(double ambientTemp)
        {
            int deratingTemp = 30;
            switch (ambientTemp) {
                case <= 30:
                    deratingTemp = 30;
                    break;
                case <= 35:
                    deratingTemp = 35;
                    break;
                case <= 40:
                    deratingTemp = 40;
                    break;
                case <= 45:
                    deratingTemp = 45;
                    break;
                case <= 50:
                    deratingTemp = 50;
                    break;
                case <= 55:
                    deratingTemp = 55;
                    break;
                case <= 60:
                    deratingTemp = 60;
                    break;
                case <= 65:
                    deratingTemp = 65;
                    break;
                case <= 70:
                    deratingTemp = 70;
                    break;
                case <= 75:
                    deratingTemp = 75;
                    break;
                case <= 80:
                    deratingTemp = 80;
                    break;
                case <= 90:
                    deratingTemp = 90;
                    break;
                case <= 100:
                    deratingTemp = 100;
                    break;
                case <= 110:
                    deratingTemp = 100;
                    break;
                case <= 120:
                    deratingTemp = 110;
                    break;
                case <= 130:
                    deratingTemp = 120;
                    break;
                case <= 140:
                    deratingTemp = 130;
                    break;
                case >= 140:
                    deratingTemp = 140;
                    break;
            }
            return deratingTemp;
        }


        private static double GetCableDerating_Table5C(ICable cable)
        {

            if (cable.Load.FedFrom == null) return 1;  
            var supplier = cable.Load.FedFrom;
            int conductorQty = cable.ConductorQty * cable.QtyParallel;

            int otherLoadCableQtyParallel;
            int otherLoadCableConductorQty;
            double otherLoadCableSpacing;

            
            //TODO - add power cable with default values upon creation of DTEQ
            foreach (var assignedLoad in supplier.AssignedLoads) {
                otherLoadCableQtyParallel = assignedLoad.PowerCable.QtyParallel;
                otherLoadCableConductorQty = assignedLoad.PowerCable.ConductorQty;
                otherLoadCableSpacing = assignedLoad.PowerCable.Spacing;

                if (otherLoadCableSpacing < 100) {
                    conductorQty += (otherLoadCableConductorQty * otherLoadCableQtyParallel);
                }
            }

            double derating;
            if (conductorQty >= 43) {
                derating = 0.5;
            }
            else if (conductorQty >= 25) {
                derating = 0.6;
            }
            else if (conductorQty >= 7) {
                derating = 0.7;
            }
            else if (conductorQty >= 4) {
                derating = 0.8;
            }
            else {
                derating = 1;
            }

            //Get's derating from Supply equipment
            double loadCableDerating = cable.Load.FedFrom.LoadCableDerating;
            if (loadCableDerating != 0 &&
                loadCableDerating < derating) {
                derating = loadCableDerating;
            }

            return derating;
        }


        public void SetVoltageDrop(ICable cable)
        {
            if (cable.UsageType != CableUsageTypes.Power.ToString())
                return;

            try {
                if (cable.TypeModel.Type.Contains("DLO")) {
                    //var load = new LoadModel();
                    //var vType = load.VoltageType;
                    //var test2 = vType.Voltage;

                    string message = "Cannot calculate voltage drop. Manual Calculation required.\n\n" +
                        "Conductor resistance values for this cable Type & cable Size is not available in the library.";

                    EdtNotificationService.SendAlert(this, message, "Calculation Error");
                    cable.VoltageDrop = 0;
                    cable.VoltageDropPercentage = 0;
                    
                    return;
                }


                if (cable.Load != null && cable.Size != null && cable.Length != null && cable.Load.VoltageType != null) {
                    double resistance = TypeManager.ConductorProperties.FirstOrDefault(cr => cr.Size == cable.Size).Resistance75C1kMeter;
                    cable.VoltageDrop = Math.Sqrt(cable.Load.VoltageType.Phase) * cable.Load.Fla * cable.Length * resistance / 1000;
                    cable.VoltageDrop = Math.Round(cable.VoltageDrop, 2);
                    cable.VoltageDropPercentage = Math.Round(cable.VoltageDrop / cable.Load.VoltageType.Voltage * 100, 2);
                }
            }
            catch (Exception ex) {

                EdtNotificationService.SendError(this, "Unknown Error", "Voltage Drop Calculation Error", ex);
            }

        }
    }
}
