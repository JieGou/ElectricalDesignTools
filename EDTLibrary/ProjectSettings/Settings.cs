using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EDTLibrary.ProjectSettings {
    public class Settings 
        {
        public static string Code { get; set; }
        public static string CableType3C1kV { get; set; }
        public static string DteqMaxPercentLoaded { get; set; }
        public static string DteqDefaultPdTypeLV { get; set; }
        public static string LoadDefaultPdTypeLV { get; set; }
        public static string VoltageDefaultLV { get; set; }




        //TODO - Model to Table Properties and Implement Interface
        public static DataTable CableSizesUsedInProject { get; set; }
        public static DataTable CableAmpsUsedInProject { get; set; }

        public static void InitializeSettings() {
            CreateCableAmpsUsedInProject();
        }
        
        public static void CreateCableAmpsUsedInProject() {
            CableAmpsUsedInProject = LibraryTables.CableAmpacities;
            foreach (DataRow cablePrj in CableSizesUsedInProject.Rows) {              
                if (cablePrj.Field<bool>("UsedInProject") == false) {
                    string size = cablePrj.Field<string>("Size");
                    for (int i = CableAmpsUsedInProject.Rows.Count-1; i >= 0; i--) {
                        DataRow cable = CableAmpsUsedInProject.Rows[i];
                        if (cable["Size"].ToString() == size) {
                            CableAmpsUsedInProject.Rows.Remove(cable);
                        }
                    }
                }
            }
            CableAmpsUsedInProject.AcceptChanges();
        }
    }
}
