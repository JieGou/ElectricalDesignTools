using EDTLibrary.Models.Areas;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.TestDataFolder
{
    public class TestData
    {
        static double Mv15 = 13800;
        //Sample Data for Testing
        public static ObservableCollection<IArea> TestAreasList = new ObservableCollection<IArea>() {
            new AreaModel() {Tag = "ML", Name = "Mill", AreaCategory = "Category 1", NemaRating = "Type 12",
                            AreaClassification = "Non-Hazardous", MinTemp= -40, MaxTemp=30},

            new AreaModel() {Tag = "FL", Name = "Flotation", AreaCategory = "Category 1", NemaRating = "Type 3R",
                            AreaClassification = "Non-Hazardous", MinTemp= -40, MaxTemp=30},

            new AreaModel() {Tag = "DR", Name = "Dry", AreaCategory = "Category 1", NemaRating = "Type 12",
                            AreaClassification = "Non-Hazardous", MinTemp= -40, MaxTemp=30}
        };

        //public static ObservableCollection<DistributionEquipment> TestDteqList = new ObservableCollection<DistributionEquipment>();
        //public static void CreateTestDteqList()
        //{
        //    TestDteqList.Add(
        //        new DteqModel { Tag = "XFR-01", Type = DteqTypes.XFR.ToString(), FedFrom = GlobalConfig.DteqUtility, LineVoltage = Mv15, LoadVoltage = 4160, Size = 5000, Unit = Units.kVA.ToString() });
        //    TestDteqList.Add( // ff: XFR-01
        //        new DteqModel() { Tag = "SWG-01", Type = DteqTypes.SWG.ToString(), FedFrom = TestDteqList[0], LineVoltage = 4160, LoadVoltage = 4160, Size = 5000, Unit = Units.A.ToString() });
        //    TestDteqList.Add(
        //        new DteqModel() { Tag = "XFR-02", Type = DteqTypes.XFR.ToString(), FedFrom = GlobalConfig.DteqUtility, LineVoltage = 4160, LoadVoltage = 600, Size = 5000, Unit = Units.kVA.ToString() });
        //    TestDteqList.Add( // Ff: XFR-02
        //        new DteqModel() { Tag = "SWG-02", Type = DteqTypes.SWG.ToString(), FedFrom = TestDteqList[2], LineVoltage = 600, LoadVoltage = 600, Size = 3000, Unit = Units.A.ToString() });
        //    TestDteqList.Add( // Ff: SWG-02
        //        new DteqModel() { Tag = "MCC-02", Type = DteqTypes.MCC.ToString(), FedFrom = TestDteqList[3], LineVoltage = 600, LoadVoltage = 600, Size = 1200, Unit = Units.A.ToString() });
        //}



        public static ObservableCollection<DistributionEquipment> TestDteqList = new ObservableCollection<DistributionEquipment>() {
            new DteqModel {Tag = "XFR-01", Type = DteqTypes.XFR.ToString(), FedFromTag = GlobalConfig.Utility, LineVoltage = Mv15, LoadVoltage = 4160, Size = 5000, Unit = Units.kVA.ToString() },
            new DteqModel {Tag = "SWG-01", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-01", LineVoltage = 4160, LoadVoltage = 4160, Size = 5000, Unit = Units.A.ToString() },
            
            new DteqModel {Tag = "XFR-03", Type = DteqTypes.XFR.ToString(), FedFromTag = "SWG-01", LineVoltage=4160, LoadVoltage=480, Size=1000, Unit= Units.kVA.ToString() },
            new DteqModel {Tag = "SWG-03", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-03", LineVoltage=480, LoadVoltage=480, Size=1200, Unit= Units.A.ToString() },
            new DteqModel {Tag = "MCC-01", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-03", LineVoltage=480, LoadVoltage=480, Size=2000, Unit= Units.A.ToString() },
            new DteqModel {Tag = "MCC-03", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-03", LineVoltage=480, LoadVoltage=480, Size=800, Unit= Units.A.ToString() },
            
            new DteqModel {Tag = "XFR-02", Type = DteqTypes.XFR.ToString(), FedFromTag = GlobalConfig.Utility, LineVoltage = 4160, LoadVoltage = 600, Size = 5000, Unit = Units.kVA.ToString() },
            new DteqModel {Tag = "SWG-02", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-02", LineVoltage=600, LoadVoltage=600, Size=3000, Unit= Units.A.ToString() },
            new DteqModel {Tag = "MCC-02", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-02", LineVoltage=600, LoadVoltage=600, Size=1200, Unit= Units.A.ToString() },

            
        };

        public static ObservableCollection<ILoad> TestLoadList = new ObservableCollection<ILoad>() {
            new LoadModel() {Tag = "MTR-01", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-01", Voltage=460, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-01", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-01", Voltage=480, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-01", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-01", Voltage=480, Size = 250,Unit=Units.A.ToString()},
            

            new LoadModel() {Tag = "MTR-02", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-02", Voltage=575, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-02", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-02", Voltage=600, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-02", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-02", Voltage=600, Size = 100,Unit=Units.A.ToString()},

            new LoadModel() {Tag = "MTR-03", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-03", Voltage=460, Size = 75,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-03", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-03", Voltage=480, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-03", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-03", Voltage=480, Size = 100,Unit=Units.A.ToString()},

            new LoadModel() {Tag = "MTR-011", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-01", Voltage=460, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-011", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-01", Voltage=480, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-011", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-01", Voltage=480, Size = 50,Unit=Units.A.ToString()},
            new LoadModel() {Tag = "MTR-022", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-02", Voltage=575, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-022", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-02", Voltage=600, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-022", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-02", Voltage=600, Size = 100,Unit=Units.A.ToString()},
            new LoadModel() {Tag = "MTR-033", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-03", Voltage=460, Size = 75,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-033", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-03", Voltage=480, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-033", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-03", Voltage=408, Size = 100,Unit=Units.A.ToString()},

            new LoadModel() {Tag = "MTR-111", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-01", Voltage=460, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-111", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-01", Voltage=480, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-111", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-01", Voltage=480, Size = 50,Unit=Units.A.ToString()},
            new LoadModel() {Tag = "MTR-222", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-02", Voltage=575, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-222", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-02", Voltage=600, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-222", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-02", Voltage=600, Size = 100,Unit=Units.A.ToString()},
            new LoadModel() {Tag = "MTR-333", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-03", Voltage=460, Size = 75,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-333", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-03", Voltage=480, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-333", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-03", Voltage=408, Size = 100,Unit=Units.A.ToString()},

        };



        public static ObservableCollection<DteqModel> TestDteqListToAdd = new ObservableCollection<DteqModel>() {
            new DteqModel() {Tag = "XFR-03", Type = DteqTypes.XFR.ToString(), FedFromTag = GlobalConfig.Utility, LineVoltage=4160, LoadVoltage=480, Size=5000, Unit= Units.kVA.ToString() },
            new DteqModel() {Tag = "SWG-03", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-01", LineVoltage=480, LoadVoltage=480, Size=5000, Unit= Units.A.ToString() },
            new DteqModel() {Tag = "MCC-03", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-01", LineVoltage=480, LoadVoltage=480, Size=2000, Unit= Units.A.ToString() },

            new DteqModel() {Tag = "XFR-04", Type = DteqTypes.XFR.ToString(), FedFromTag = GlobalConfig.Utility, LineVoltage=4160, LoadVoltage=600, Size=5000, Unit= Units.kVA.ToString() },
            new DteqModel() {Tag = "SWG-04", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-02", LineVoltage=600, LoadVoltage=600, Size=3000, Unit= Units.A.ToString() },
            new DteqModel() {Tag = "MCC-04", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-02", LineVoltage=600, LoadVoltage=600, Size=1200, Unit= Units.A.ToString() }
        };

        public static ObservableCollection<LoadModel> TestLoadListToAdd = new ObservableCollection<LoadModel>() {
            new LoadModel() {Tag = "MTR-03", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-01", Voltage=460, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-03", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-01", Voltage=480, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-03", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-01", Voltage=480, Size = 50,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "MTR-04", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-02", Voltage=575, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-04", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-02", Voltage=600, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-04", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-02", Voltage=600, Size = 100,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "MTR-03", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-03", Voltage=480, Size = 75,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-03", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-03", Voltage=480, Size = 75,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-03", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-03", Voltage=480, Size = 75,Unit=Units.HP.ToString()}
        };

        public static ListManager listManager = new ListManager();

        public static void CreateListManagerData()
        {
            listManager.DteqList = TestDteqList;
            listManager.LoadList = TestLoadList;
        }
    }
}
