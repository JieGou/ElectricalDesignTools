using EDTLibrary.LibraryData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EDTLibrary.ProjectSettings
{
    /// <summary>
    /// Project Settings for automating sizing and selections
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// 

    // To change settings names you must change the name in 3 places:
    //   1 - the name below
    //   2 - the name of the setting in the Database ProjectSettings Table
    //   3 - the table name itself in the Database if the setting is a DataTable type Setting

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
        public static DataTable CableSizesUsedInProject_3C1kV { get; set; }
        public static DataTable CableAmpsUsedInProject_3C1kV { get; set; }

        public static DataTable CableSizesUsedInProject_3C5kV { get; set; }
        public static DataTable CableAmpssUsedInProject_3C5kV { get; set; }

        public static DataTable CableSizesUsedInProject_3C15kV { get; set; }
        public static DataTable CableAmpsUsedInProject_3C15kV { get; set; }

        public static DataTable CableSizesUsedInProject_1C1kV { get; set; }
        public static DataTable CableAmpssUsedInProject_1C1kV { get; set; }

        public static DataTable CableSizesUsedInProject_1C5kV { get; set; }
        public static DataTable CableAmpsUsedInProject_1C5kV { get; set; }

        public static DataTable CableSizesUsedInProject_1C15kV { get; set; }
        public static DataTable CableAmpsUsedInProject_1C15kV { get; set; }

        public static DataTable CableSizesUsedInProject_DLO1kV { get; set; }
        public static DataTable CableAmpsUsedInProject_DLO1kV { get; set; }


        /// <summary>
        /// Creates the Cable Ampacities table for the project based on the selected cable sizes used in the project
        /// </summary>
        public static void CreateCableAmpacityTableUsedInProject_3C1kV() {
            if (LibraryTables.CableAmpacities != null) {

                CableAmpsUsedInProject_3C1kV = LibraryTables.CableAmpacities.Copy();

                foreach (DataRow cablePrj in CableSizesUsedInProject_3C1kV.Rows) {
                    if (cablePrj.Field<bool>("UsedInProject") == false) {
                        string size = cablePrj.Field<string>("Size");

                        for (int i = CableAmpsUsedInProject_3C1kV.Rows.Count - 1; i >= 0; i--) {
                            DataRow cable = CableAmpsUsedInProject_3C1kV.Rows[i];
                            if (cable["Size"].ToString() == size) {
                                CableAmpsUsedInProject_3C1kV.Rows.Remove(cable);
                            }
                        }
                    }
                }
                CableAmpsUsedInProject_3C1kV.AcceptChanges();
            }
        }

    }
}

