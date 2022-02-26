using EDTLibrary.LibraryData.TypeTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables
{
    public class CecCableSizer
    {
        public CecCableSizer(IPowerCable cable)
        {
            _cable = cable;
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


        public void DetermineSizingTable()
        {
            if (_cable == null) return;

            CableTypeModel cableType = TypeManager.GetCableType(_cable.Type);

            // 1C, >=5kV, Shielded        QtyParallel max = 2 
            if (cableType.Conductors == 1
            && cableType.VoltageClass >= 5000
            && cableType.Shielded == true) {

                _sizingTable = TypeManager.CecCableSizingRules.FirstOrDefault(csr =>
                   csr.CableType == _cable.Type &&
                   csr.InstallationType == _cable.InstallationType &&
                   csr.Spacing == _cable.Spacing &&
                   csr.Indoor == _cable.Indoor
                   ).AmpacityTable;
            }

            // 3C, >=5kV, Shielded       Table D17N 3C
            else if (cableType.Conductors == 3
            && cableType.VoltageClass >= 5000
            && cableType.Shielded == true) {

                _sizingTable = TypeManager.CecCableSizingRules.FirstOrDefault(csr =>
                   csr.CableType == _cable.Type &&
                   csr.InstallationType == _cable.InstallationType &&
                   csr.Indoor == _cable.Indoor
                   ).AmpacityTable;

            }

            // 1 or 3C <= 5kV, Non-Shielded
            else {
                _sizingTable = TypeManager.CecCableSizingRules.FirstOrDefault(csr =>
                    csr.CableType == _cable.Type &&
                    csr.InstallationType == _cable.InstallationType &&
                    csr.Indoor == _cable.Indoor
                    ).AmpacityTable;

            }
           
        }
    }
}
