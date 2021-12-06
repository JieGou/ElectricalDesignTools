﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EDTLibrary {
    public class StringSettings 
        {
        public static string Code { get; set; }
        public static string CableType3C1kV { get; set; }
        public static string DteqMaxPercentLoaded { get; set; }
        public static string DteqDefaultPdTypeLV { get; set; }
        public static string LoadDefaultPdTypeLV { get; set; }
        public static string VoltageDefaultLV { get; set; }




        //TODO - Mode to Table Properties and Implement Interface
        public static DataTable CableSizesUsedInProject { get; set; }
        public static DataTable CableAmpsUsedInProject { get; set; }

        public static void InitializeSettings() {
            CreateCableAmpsUsedInProject();
        }
        
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
            DataTable cableAmps = new DataTable();

            var cableAmpsInProject = from cableAmp in cableAmpsInPrj.AsEnumerable()
                                     join cablePrj in cablesInPrj.AsEnumerable()
                                     on cableAmp.Field<string>("Size") equals cablePrj.Field<string>("Size")
                                     where cablePrj.Field<bool>("UsedInProject") == true
                                     select cableAmps.LoadDataRow(new object[] {
                                         cablePrj["Size"],
                                         cableAmp["Amps60"],
                                         cableAmp["Amps75"],
                                         cableAmp["Amps90"],
                                         cableAmp["Code"],
                                         cableAmp["CodeTable"]
                                     }, false);

            cableAmps = CableAmpsUsedInProject;
        }
    }
}