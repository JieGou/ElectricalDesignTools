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
        public static string LocationTable { get; set; } = "Locations";
        public static string PowerCableTable { get; set; } = "Cables";

        public const string EmptyTag = "                 ";


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


        //Sample Data for Testing
        public static ObservableCollection<DteqModel> TestDteqList = new ObservableCollection<DteqModel>() {
            new DteqModel() {Tag = "XFR-01", Type = DteqTypes.XFR.ToString(), FedFrom = GlobalConfig.Utility, LineVoltage=4160, LoadVoltage=480, Size=5000, Unit= Units.kVA.ToString() },
            new DteqModel() {Tag = "SWG-01", Type = DteqTypes.SWG.ToString(), FedFrom = "XFR-01", LineVoltage=480, LoadVoltage=480, Size=5000, Unit= Units.A.ToString() },
            new DteqModel() {Tag = "MCC-01", Type = DteqTypes.MCC.ToString(), FedFrom = "SWG-01", LineVoltage=480, LoadVoltage=480, Size=2000, Unit= Units.A.ToString() },

            new DteqModel() {Tag = "XFR-02", Type = DteqTypes.XFR.ToString(), FedFrom = GlobalConfig.Utility, LineVoltage=4160, LoadVoltage=600, Size=5000, Unit= Units.kVA.ToString() },
            new DteqModel() {Tag = "SWG-02", Type = DteqTypes.SWG.ToString(), FedFrom = "XFR-02", LineVoltage=600, LoadVoltage=600, Size=3000, Unit= Units.A.ToString() },
            new DteqModel() {Tag = "MCC-02", Type = DteqTypes.MCC.ToString(), FedFrom = "SWG-02", LineVoltage=600, LoadVoltage=600, Size=1200, Unit= Units.A.ToString() }
        };

        public static ObservableCollection<LoadModel> TestLoadList = new ObservableCollection<LoadModel>() {
            new LoadModel() {Tag = "MTR-01", Type = LoadTypes.MOTOR.ToString(), FedFrom = "MCC-01", Voltage=460, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-01", Type = LoadTypes.HEATER.ToString(), FedFrom = "MCC-01", Voltage=480, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-01", Type = LoadTypes.PANEL.ToString(), FedFrom = "MCC-01", Voltage=480, Size = 50,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "MTR-02", Type = LoadTypes.MOTOR.ToString(), FedFrom = "MCC-02", Voltage=575, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-02", Type = LoadTypes.HEATER.ToString(), FedFrom = "MCC-02", Voltage=600, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-02", Type = LoadTypes.PANEL.ToString(), FedFrom = "MCC-02", Voltage=600, Size = 100,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "MTR-03", Type = LoadTypes.MOTOR.ToString(), FedFrom = "MCC-03", Voltage=480, Size = 75,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-03", Type = LoadTypes.HEATER.ToString(), FedFrom = "MCC-03", Voltage=480, Size = 75,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-03f", Type = LoadTypes.PANEL.ToString(), FedFrom = "MCC-02", Voltage=480, Size = 75,Unit=Units.HP.ToString()}
        };


        #region Database Table Quick Names
       

        #endregion

    }
}
