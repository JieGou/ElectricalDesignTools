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
        public static DataTable CableSizesUsedInProject { get; set; }
        public static DataTable CableAmpsUsedInProject { get; set; }

        public static void InitializeSettings() {
            CreateCableAmpsUsedInProject();
        }
        
        //TODO - create a class for CableAmpsUsedInProject
        public static void CreateCableAmpsUsedInProject() {
            CableAmpsUsedInProject = DataTables.CableAmpacities;
            foreach (DataRow cablePrj in CableSizesUsedInProject.Rows) {              
                if (cablePrj.Field<bool>("UsedInProject") == false) {
                    string size = cablePrj.Field<string>("Size");
                    for (int i = CableAmpsUsedInProject.Rows.Count-1; i >= 0; i--) {
                        DataRow cable = CableAmpsUsedInProject.Rows[i];
                        if (cable["Size"].ToString() == size) {
                            CableAmpsUsedInProject.Rows.Remove(cable);                       }
                    }
                }
            }
            CableAmpsUsedInProject.AcceptChanges();
        }
        public static void CreateCableAmpsUsedInProject2() { //creates an IEnumerable of anonymous type that can't be used as a dataTable
            DataTable cablesInPrj = CableSizesUsedInProject.Select($"UsedInProject = 'true'").CopyToDataTable();
            DataTable cableAmpsInPrj = DataTables.CableAmpacities;

            var cableAmpsInProject = from cableAmp in cableAmpsInPrj.AsEnumerable()
                                     join cablePrj in cablesInPrj.AsEnumerable()
                                     on cableAmp.Field<string>("Size") equals cablePrj.Field<string>("Size")
                                     select new {
                                         Size = cablePrj["Size"],
                                         Amps60 = cableAmp["Amps60"],
                                         Amps75 = cableAmp["Amps75"],
                                         Amps90 = cableAmp["Amps90"],
                                         Code = cableAmp["Code"],
                                         CodeTable = cableAmp["CodeTable"]
                                     };
        }
    }
}
