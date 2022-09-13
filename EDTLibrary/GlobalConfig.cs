using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;


namespace EDTLibrary
{
    public static class GlobalConfig {

        //Flags
        public static bool Importing = false;
        public static bool SelectingNew = false;
        public static bool Testing = false;

        
        
        



        //TEMP
        public const string CodeTable = "Table 2";

        //Quick Names
        public const string Utility = "UTILITY";
        public static string LargestMotor_StartLoad = "LargestMotor_StartLoad";

        public static IDteq DteqUtility { get; set; } = new DteqModel { Id = -0, Tag = GlobalConfig.Utility, Type = GlobalConfig.Utility };

        public const string Deleted = "* Deleted *";
        public static IDteq DteqDeleted { get; set; } = new DteqModel { Id = -1, Tag = GlobalConfig.Deleted, Type = GlobalConfig.Deleted };


        //Tables
        public static string LoadTable { get; set; } = "Loads";
        public static string DteqTable { get; set; } = "DistributionEquipment";
        public static string XfrTable { get; set; } = "Transformer";
        public static string SwgTable { get; set; } = "Switchgear";
        public static string MccTable { get; set; } = "Mcc";
        public static string DpnTable { get; set; } = "DPanels";
        public static string DpnCircuitTable { get; set; } = "DpnCircuits";
        public static string ComponentTable { get; set; } = "Components";
        public static string LcsTable { get; set; } = "LocalControlStation";
        public static string AreaTable { get; set; } = "Areas";
        public static string CableTable { get; set; } = "Cables";

        public static string ExportMappingTable { get; set; } = "ExportMapping";


        public static string DevDb { get; set; } = @"C:\C - Visual Studio Projects\ElectricalDesignTools\WpfUI\ContentFiles\Edt Sample Project.edp";
        public static string TestDb { get; set; } = "C:\\Users\\pdeau\\Google Drive\\Work\\Visual Studio Projects\\_EDT Tables\\EDT SQLite DB Files\\TestDb.edp";
        public static string CableTypes { get; internal set; } = "CableTypes";
        public static string ControlCableSizeTable { get; internal set; } = "CableSizes_Control";
        public static string InstrumentCableSizeTable { get; internal set; } = "CableSizes_Instrument";

        public const string EmptyTag = "                 "; // 17 spaces

        public static string Code_Cec { get; set; } = "CEC";
        public static string Code_Nec { get; set; } = "NEC";
        public static string Code_Iec { get; set; } = "IEC";


        public static string CableInstallationType_LadderTray { get; set; } = "LadderTray";
        public static string CableInstallationType_DirectBuried { get; set; } = "DirectBuried";
        public static string CableInstallationType_RacewayConduit { get; set; } = "RacewayConduit";


        public static string LcsTypesTable { get; set; } = "LocalControlStationTypes";


        //Constants
        public static int SigFigs { get; set; } = 1;
        public static string Separator { get; set; } = "-";
        public static double NoValueDouble { get; set; } = 0.001;
        //Default Power Factor and Efficiency
        public static double DefaultTransformerPowerFactor { get; set; } = 0.9;
        public static double DefaultTransformerEfficiency { get; set; } = 0.95;

        public static double DefaultHeaterPowerFactor { get; set; } = 0.99;
        public static double DefaultHeaterEfficiency { get; set; } = 0.98;

        //TODO - figure out motor RPM
        public static double DefaultMotorRpm { get; set; } = 1800;

        public static double OtherPf { get; set; } = 0.85;
        public static double OtherEff { get; set; } = 0.85;
        public static string DriveTable { get; set; } = "Drives";
        public static string DisconnectTable { get; set; } = "Disconnects";
        public static string CableResistancesTable { get; internal set; }
    }
}
