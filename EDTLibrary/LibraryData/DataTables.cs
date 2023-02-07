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
        public static DataTable CableTypes { get; set; }
        public static DataTable CEC_Table5A { get; set; }

        public static DataTable CecCableAmpacities { get; set; }

        public static DataTable BreakerSizes { get; set; }
        public static DataTable MCPs { get; set; }
        public static DataTable DisconnectSizes { get; set; }
        public static DataTable Motors { get; set; }
        public static DataTable Starters { get; set; }
        public static DataTable MvContactors { get; set; }

        public static DataTable VoltageTypes { get; set; }

    }

}
