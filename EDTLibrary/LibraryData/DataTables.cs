using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.LibraryData
{
    public static class DataTables
    {
        public static DataTable CableAmpacities { get; set; }
        public static DataTable CableDimensions { get; set; }
        public static DataTable CableInsulationLevels { get; set; }
        public static DataTable ConductorSizes { get; set; } //Resistances per 1000'
        public static DataTable CecCableSizingRules { get; set; }
        public static DataTable CableTypes { get; set; }
        public static DataTable CEC_Table5A { get; set; }

        public static DataTable CecCableAmpacities { get; set; }

        public static DataTable BreakerSizes { get; set; }
        public static DataTable MCPs { get; set; }
        public static DataTable DisconnectSizes { get; set; }
        public static DataTable LoadTypes { get; set; }
        public static DataTable Motors { get; set; }
        public static DataTable Starters { get; set; }
        public static DataTable VFDHeatLoss { get; set; }

        public static DataTable VoltageTypes { get; set; }
        public static DataTable NemaTypes { get; set; }
        public static DataTable LocalControlStationTypes { get; set; }


    }

}
