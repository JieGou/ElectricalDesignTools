using EDTLibrary.LibraryData;
using EDTLibrary.Models.Cables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static string DefaultCableInstallationType { get; set; }
        public static string DefaultCableTypeLoad_3ph1kV { get; set; }
        public static string DefaultCableTypeDteq_3ph1kV1200AL { get; set; }
        public static string DefaultCableTypeDteq_3ph1kV1200AM { get; set; }
        public static string CableSpacingMaxAmps_3C1kV { get; set; }


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


        public static ObservableCollection<CableSizeModel> CableSizesUsedInProject { get; set; }


    }
}

