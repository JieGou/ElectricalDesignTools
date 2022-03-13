using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
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

        private IPowerCable _cable;
        public IPowerCable Cable
        {
            get { return _cable; }
            set { _cable = value; }
        }

        private string _sizingTable;

        public string SizingTable
        {
            get { return _sizingTable; }
            set { _sizingTable = value; }
        }

        public string GetDefaultCableType(IPowerConsumer load)
        {
            if (load is ILoad && load.Voltage <= 1000) {
                return EdtSettings.DefaultCableTypeLoad_3ph1kV;
            }
            if (load is IDteq && load.Voltage <= 1000 && load.Fla <= 1200) {
                return EdtSettings.DefaultCableTypeDteq_3ph1kV1200AL;
            }
            if (load is IDteq && load.Voltage <= 1000 && load.Fla > 1200) {
                return EdtSettings.DefaultCableTypeDteq_3ph1kV1200AM;
            }

            return EdtSettings.DefaultCableTypeLoad_3ph1kV;
        }
        public double GetDefaultCableSpacing(IPowerCable cable)
        {
            double spacing = 100;
            if (cable == null) return spacing;

            CableTypeModel cableType = TypeManager.GetCableType(cable.Type);
            if (cableType.VoltageClass > 2000 || cableType.Conductors == 1) {
                if (cable.Spacing <100) {
                    spacing = cable.Spacing;
                }
                else {
                    spacing = 100;
                }
            }
            else if (cableType.VoltageClass <= 2000 &&
                     cable.Load.Fla >= double.Parse(EdtSettings.CableSpacingMaxAmps_3C1kV)) {
                spacing = 100;
            }
            else if (cableType.VoltageClass < 2000 &&
                     cableType.Conductors == 3 &&
                     cable.Load.Fla <= double.Parse(EdtSettings.CableSpacingMaxAmps_3C1kV)) {
                spacing = 0;
            }
            cable.Spacing = spacing;
            return spacing;
        }
        public string GetAmpacityTable(IPowerCable cable)
        {
            if (cable == null) return "Invalid Cable Data";

            string output = String.Empty;
            CableTypeModel cableType = TypeManager.GetCableType(cable.Type);

            if (cable.InstallationType == GlobalConfig.CableInstallationType_LadderTray) {
                output = GetAmpacityTable_LadderTray(cable, cableType);
            }
            else if (cable.InstallationType == GlobalConfig.CableInstallationType_DirectBuried) {
                output = GetAmpacityTable_DirectBuried(cable, cableType);
            }
            else if (cable.InstallationType == GlobalConfig.CableInstallationType_RacewayConduit) {
                output = GetAmpacityTable_RacewayConduit(cable, cableType);
            }

            return output;
        }


        private static string GetAmpacityTable_LadderTray(IPowerCable cable, CableTypeModel cableType)
        {
            string output = "No Table Assigned";

            // 1C, >=5kV <=15kV, Shielded, 100% spacing
            if (cableType.Conductors == 1
                        && cableType.VoltageClass >= 5000
                        && cableType.VoltageClass <= 15000
                        && cableType.Shielded == true
                        && cable.Spacing >=100) {

                if (cable.Outdoor == false) {
                    output = "Table D17M15In";
                }
                else if (cable.Outdoor == true) {
                    output = "Table D17M15Out";
                }
            }

            // 1C, >=25kV <=46kV, Shielded, 100% spacing
            else if (cableType.Conductors == 1
                  && cableType.VoltageClass >= 25000
                  && cableType.VoltageClass <= 46000
                  && cableType.Shielded == true
                  && cable.Spacing >= 100) {

                if (cable.Outdoor == false) {
                    output = "Table D17M46In";
                }
                else if (cable.Outdoor == true) {
                    output = "Table D17M46Out";
                }
            }

            // 1C or 3C, >=5kV <=15kV, Shielded, no spacing
            else if ((cableType.Conductors == 3 || cable.Spacing < 100)
                  && cableType.VoltageClass >= 5000
                  && cableType.VoltageClass <= 15000
                  && cableType.Shielded == true) {

                if (cable.Outdoor == false) {
                    output = "Table D17N15In";
                }
                else if (cable.Outdoor == true) {
                    output = "Table D17N15Out";
                }
            }

            // 3C, >=25kV <=46kV, Shielded, Shielded, no spacing
            else if ((cableType.Conductors == 3 || cable.Spacing < 100)
                  && cableType.VoltageClass >= 25000
                  && cableType.VoltageClass <= 46000
                  && cableType.Shielded == true) {

                if (cable.Outdoor == false) {
                    output = "Table D17N46In";
                }
                else if (cable.Outdoor == true) {
                    output = "Table D17N46Out";
                }
            }

            // 1 or 3C <= 5kV, Non-Shielded
            else {
                if (cable.Type.Contains("DLO")) {
                    output = "Table 12E";
                }
                else if (cable.TypeModel.Conductors == 1) {
                    output = "Table 1";
                }
                else if (cable.TypeModel.Conductors == 3) {
                    output = "Table 2";
                }
            }
            return output;
        }
        private static string GetAmpacityTable_DirectBuried(IPowerCable cable, CableTypeModel cableType)
        {
            string output;

            // 1C, >= 5kV, Shielded
            if (cableType.Conductors == 1
                        && cableType.VoltageClass >= 5000
                        && cableType.Shielded == true) {

                output = "Table not yet added to database.";
            }

            // 3C, >=5kV, Shielded       Table D17N 3C QtyParallel max = 2
            else if (cableType.Conductors == 3
            && cableType.VoltageClass >= 5000
            && cableType.Shielded == true) {

                output = "Table not yet added to database.";
            }

            // 1C, <= 5kV, Non-Shielded
            else if (cableType.Conductors == 1
                        && cableType.VoltageClass <= 5000
                        && cableType.Shielded == false) {

                output = "Table D8A";
            }

            // 3C, <= 5kV, Non-Shielded
            else {
                output = "Table D10A";
            }
            return output;
        }
        private static string GetAmpacityTable_RacewayConduit(IPowerCable cable, CableTypeModel cableType)
        {
            string output;

            // 1C, >= 5kV, Shielded
            if (cableType.Conductors == 1
                        && cableType.VoltageClass >= 5000
                        && cableType.Shielded == true) {

                output = "Table not yet added to database.";
            }

            // 3C, >=5kV, Shielded       Table D17N 3C QtyParallel max = 2
            else if (cableType.Conductors == 3
            && cableType.VoltageClass >= 5000
            && cableType.Shielded == true) {

                output = "Table not yet added to database.";
            }

            // 1C, <= 5kV, Non-Shielded
            else if (cableType.Conductors == 1
                        && cableType.VoltageClass <= 5000
                        && cableType.Shielded == false) {

                output = "Table D9A";
            }

            // 3C, <= 5kV, Non-Shielded
            else {
                output = "Table D11A";
            }
            return output;
        }
        public double GetDerating(IPowerCable cable)
        {

            double derating = 1;
            if (cable == null) return derating;

            if (cable.Spacing < 100) {

                if (cable.AmpacityTable == "Table 1" || cable.AmpacityTable == "Table 2") {
                    double loadCount = cable.Load.FedFrom.AssignedLoads.Count;
                    derating = GetCableDerating_Table5C(cable);
                }
            }

            return derating;
        }

        private static double GetCableDerating_Table5C(IPowerCable cable)
        {
            var supplier = cable.Load.FedFrom;
            int conductorQty = cable.ConductorQty * cable.QtyParallel;

            //int otherLoadConductorQty;
            int otherLoadCableQtyParallel;
            double otherLoadCableSpacing;

            
            //TODO - add power cable with default values upon creation of DTEQ
            foreach (var assignedLoad in supplier.AssignedLoads) {
                otherLoadCableQtyParallel = assignedLoad.PowerCable.QtyParallel;
                otherLoadCableSpacing = assignedLoad.PowerCable.Spacing;

                if (otherLoadCableSpacing < 100) {
                    conductorQty += (3 * otherLoadCableQtyParallel);
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

            return derating;
        }


    }
}
