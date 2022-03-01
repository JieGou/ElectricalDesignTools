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


    }
}
