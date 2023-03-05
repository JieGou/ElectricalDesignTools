using EDTLibrary.Models.Areas;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Settings;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.ObjectModel;


namespace EDTLibrary
{
    public static class GlobalConfig {

        public static EdtSettings EdtSettings = new EdtSettings();

        //Flags
        public static bool Importing = false;
        public static bool SelectingNew = false;
        public static bool Testing = false;

        
        
        



        //TEMP
        public const string CodeTable = "Table 2";

        //Quick Names
        public const string UtilityTag = "UTILITY";
        public static DteqModel UtilityModel { get => utilityModel; }
        private static DteqModel utilityModel = new DteqModel {
            Tag = GlobalConfig.UtilityTag,
            Type = GlobalConfig.UtilityTag,
            Area = GlobalConfig.DefaultAreaModel,
            AreaId = -1
        };

        public const string DefaultAreaTag = "SITE";
        public static AreaModel DefaultAreaModel { get => defaultArea; }
        private static AreaModel defaultArea = new AreaModel {
            Id = -1,
            Tag = DefaultAreaTag,
            Name = "Project Site",
            DisplayTag = DefaultAreaTag,
            Description = "",
            AreaCategory = "Category 1",
            AreaClassification = "Non-Hazardous",
            NemaRating = "Type 12",
            MinTemp = -10,
            MaxTemp = 20,
        };

        public static string LargestMotor_StartLoad = "LargestMotor_StartLoad";

        public static IDteq DteqUtility { get; set; } = new DteqModel { Id = -0, Tag = GlobalConfig.UtilityTag, Type = GlobalConfig.UtilityTag };

        public const string Deleted = "* Deleted *";
        public static IDteq DteqDeleted { get; set; } = new DteqModel { Id = -1, Tag = GlobalConfig.Deleted, Type = GlobalConfig.Deleted };


        //Tables
        public static string LoadCircuitTable { get; set; } = "LoadCircuits";
        public static string LoadTable { get; set; } = "Loads";
        public static string DteqTable { get; set; } = "DistributionEquipment";
        public static string XfrTable { get; set; } = "Transformer";
        public static string SwgTable { get; set; } = "Switchgear";
        public static string MccTable { get; set; } = "Mcc";
        public static string DpnTable { get; set; } = "DPanels";
        public static string SplitterTable   { get; set; } = "Splitters";
        public static string DpnCircuitTable { get; set; } = "DpnCircuits";
        public static string ComponentTable { get; set; } = "Components";
        public static string LcsTable { get; set; } = "LocalControlStation";
        public static string AreaTable { get; set; } = "Areas";
        public static string CableTable { get; set; } = "Cables";
        public static string ProtectionDeviceTable { get; set; } = "ProtectionDevices";
        public static string CalculationLockTable { get; set; } = "CalculationLocks";


        public static string BreakerPropsTable { get; set; } = "Properties_Breaker";
        public static string DisconnectPropsTable { get; set; } = "Properties_Disconnect";



        public static string ExportMappingTable { get; set; } = "ExportMapping";

        
        public static string DevDb { get; set; } = @"C:\C - Visual Studio Projects\WpfUI\ContentFiles\Edt Sample Project.edp";
        public static string TestDb { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+@"\TestDb.edp";
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
        public static double NoValueDouble { get; set; } = 0.0001 * Math.Pow(10,-308);
       


        //TODO - figure out motor RPM
        public static double DefaultMotorRpm { get; set; } = 1800;

        
        public static string DriveTable { get; set; } = "Drives";
        public static string DisconnectTable { get; set; } = "Disconnects";

        public static string RacewayTable { get; set; } = "Raceways";
        public static string RacewayRouteSegmentsTable { get; set; } = "RacewayRouteSegments";
    }
}
