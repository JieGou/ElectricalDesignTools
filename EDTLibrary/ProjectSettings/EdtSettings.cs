using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EDTLibrary.ProjectSettings {
    public class EdtSettings 
        {
        //General
        public static string Code { get; set; }

        //Cable
        public static string CableType3C1kVPower { get; set; }
        public static string CableInsulation1kVPower { get; set; }
        public static string CableInsulation5kVPower { get; set; }
        public static string CableInsulation15kVPower { get; set; }
        public static string CableInsulation35kVPower { get; set; }

        //Dteq
        public static string DteqMaxPercentLoaded { get; set; }
        public static string DteqDefaultPdTypeLV { get; set; }

        public static string LoadDefaultPdTypeLV { get; set; }
        public static string LoadFactorDefault { get; set; }

        //Voltage
        public static string VoltageDefault1kV { get; set; }





        //TODO - Model to Table Properties and Implement Interface
        public static DataTable CableSizesUsedInProject3CLV { get; set; }
        public static DataTable CableAmpsUsedInProject 
        { 
            get; 
            set; 
        }

        public static void InitializeSettings() {
            CreateCableAmpsUsedInProject();
        }
        
        /// <summary>
        /// Creates the Cable Ampacities table for the project based on the selected cable sizes used in the project
        /// </summary>
        public static void CreateCableAmpsUsedInProject() {
            if (LibraryTables.CableAmpacities != null) {

                CableAmpsUsedInProject = LibraryTables.CableAmpacities.Copy();

                foreach (DataRow cablePrj in CableSizesUsedInProject3CLV.Rows) {
                    if (cablePrj.Field<bool>("UsedInProject") == false) {
                        string size = cablePrj.Field<string>("Size");

                        for (int i = CableAmpsUsedInProject.Rows.Count - 1; i >= 0; i--) {
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
}

