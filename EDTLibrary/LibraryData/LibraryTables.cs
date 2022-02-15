using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.LibraryData
{
    public static class LibraryTables
    {
        public static DataTable CableAmpacities { get; set; }
        public static DataTable CableDimensions { get; set; }
        public static DataTable CableInsulationLevels { get; set; }
        public static DataTable CableSizes { get; set; } //Resistances per 1000'
        public static DataTable CableSizingRules { get; set; }
        public static DataTable CableTypes { get; set; }
        public static DataTable CableUsageTypes { get; set; }

        public static DataTable Breakers { get; set; }
        public static DataTable Disconnects { get; set; }
        public static DataTable LoadTypes { get; set; }
        public static DataTable Motors { get; set; }
        public static DataTable Starters { get; set; }
        public static DataTable Transformers { get; set; }
        public static DataTable VFDHeatLoss { get; set; }

        public static DataTable VoltageTypes { get; set; }
        public static DataTable NemaTypes { get; set; }
        public static DataTable LocalControlStationTypes { get; set; }

    }

}
