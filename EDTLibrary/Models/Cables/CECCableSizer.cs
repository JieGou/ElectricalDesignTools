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
                spacing = 100;
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
            string output;

            CableTypeModel cableType = TypeManager.GetCableType(cable.Type);

            // 1C, >=5kV, Shielded        QtyParallel max = 2 
            if (cableType.Conductors == 1
            && cableType.VoltageClass >= 5000
            && cableType.Shielded == true) {

                output = TypeManager.CecCableSizingRules.FirstOrDefault(csr =>
                   csr.CableType == cable.Type &&
                   csr.InstallationType == cable.InstallationType &&
                   csr.Spacing == cable.Spacing &&
                   csr.Indoor == cable.Indoor
                   ).AmpacityTable;
            }

            // 3C, >=5kV, Shielded       Table D17N 3C
            else if (cableType.Conductors == 3
            && cableType.VoltageClass >= 5000
            && cableType.Shielded == true) {

                output = TypeManager.CecCableSizingRules.FirstOrDefault(csr =>
                   csr.CableType == cable.Type &&
                   csr.InstallationType == cable.InstallationType &&
                   csr.Indoor == cable.Indoor
                   ).AmpacityTable;

            }

            // 1 or 3C <= 5kV, Non-Shielded
            else {
                output = TypeManager.CecCableSizingRules.FirstOrDefault(csr =>
                    csr.CableType == cable.Type &&
                    csr.InstallationType == cable.InstallationType
                    ).AmpacityTable;

            }
            cable.AmpacityTable = output;
            return output;
        }

        public double GetDerating(IPowerCable cable)
        {

            double derating = 1;
            if (cable == null) return derating;

            if (cable.Spacing < 100) {

                if (cable.AmpacityTable == "Table 1" || cable.AmpacityTable == "Table 2") {
                    double loadCount = cable.Load.FedFrom.AssignedLoads.Count;
                    derating = GetTable5CDerating(cable);
                }
            }

            return derating;
        }

        private static double GetTable5CDerating(IPowerCable cable)
        {
            var source = cable.Load.FedFrom;
            int conductorQty = cable.ConductorQty * cable.QtyParallel;

            int otherLoadConductorQty;
            int otherLoadCableQtyParallel;
            double otherLoadCableSpacing;

            foreach (var assignedLoad in source.AssignedLoads) {
                otherLoadConductorQty = assignedLoad.PowerCable.TypeModel.Conductors;
                otherLoadCableQtyParallel = assignedLoad.PowerCable.QtyParallel;
                otherLoadCableSpacing = assignedLoad.PowerCable.Spacing;

                if (otherLoadCableSpacing < 100) {
                    conductorQty += (otherLoadConductorQty * otherLoadCableQtyParallel);
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
