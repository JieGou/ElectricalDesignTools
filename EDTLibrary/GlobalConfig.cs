using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System.Collections.ObjectModel;

namespace EDTLibrary
{
    public static class GlobalConfig {

        //Flags
        public static bool GettingRecords = true;
        public static bool Testing = false;


        //TEMP
        public const string CodeTable = "Table 2";

        //Quick Names
        public const string Utility = "UTILITY";

        public static string LoadTable { get; set; } = "Loads";
        public static string DteqTable { get; set; } = "DistributionEquipment";
        public static string AreaTable { get; set; } = "Areas";
        public static string PowerCableTable { get; set; } = "Cables";

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
