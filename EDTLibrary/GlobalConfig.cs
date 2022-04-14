using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System.Collections.ObjectModel;

namespace EDTLibrary
{
    public static class GlobalConfig {

        //Flags
        public static bool GettingRecords = true;
        public static bool SelectingNew = false;
        public static bool Testing = false;


        //TEMP
        public const string CodeTable = "Table 2";

        //Quick Names
        public const string Utility = "UTILITY";
        public static IDteq DteqUtility { get; set; } = new DteqModel { Id = -0, Tag = GlobalConfig.Utility, Type = GlobalConfig.Utility };

        public const string Deleted = "* Deleted *";
        public static IDteq DteqDeleted { get; set; } = new DteqModel { Id = -1, Tag = GlobalConfig.Deleted, Type = GlobalConfig.Deleted };

        public static string LoadTable { get; set; } = "Loads";
        public static string DteqTable { get; set; } = "DistributionEquipment";
        public static string XfrTable { get; set; } = "Transformer";
        public static string SwgTable { get; set; } = "Switchgear";
        public static string MccTable { get; set; } = "Mcc";
        public static string AreaTable { get; set; } = "Areas";
        public static string PowerCableTable { get; set; } = "Cables";
        public static string DevDb { get; set; } = "C:\\Users\\pdeau\\Google Drive\\Work\\Visual Studio Projects\\_EDT Tables\\EDT SQLite DB Files\\EDTProjectTemplate1.1.edp";
        public static string TestDb { get; set; } = "C:\\Users\\pdeau\\Google Drive\\Work\\Visual Studio Projects\\_EDT Tables\\EDT SQLite DB Files\\TestDb.edp";
        public static string PowerCableTypes { get; internal set; } = "CableTypes";

        public const string EmptyTag = "                 ";


        public const string CableInstallationType_LadderTray = "LadderTray";
        public const string CableInstallationType_DirectBuried = "DirectBuried";
        public const string CableInstallationType_RacewayConduit = "RacewayConduit";


        //Constants
        public const int SigFigs = 1;
        public const string Separator = "-";
        public const double NoValueDouble = 0.001;
        //Default Power Factor and Efficiency
        public const double DefaultTransformerPowerFactor = 0.9;
        public const double DefaultTransformerEfficiency = 0.95;

        public const double DefaultHeaterPowerFactor = 0.99;
        public const double DefaultHeaterEfficiency = 0.99;

        //TODO - figure out motor RPM
        public const double DefaultMotorRpm = 1800;

        public const double OtherPf = 0.85;
        public const double OtherEff = 0.85;


       

    }
}
