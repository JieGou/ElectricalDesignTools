using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary {
    public static class ProjectSettings {
        public static string Code { get; set; }
        public static string CableType3C1kV { get; set; }
        public static DataTable CablesUsedInProject { get; set; }
    }

   
}
