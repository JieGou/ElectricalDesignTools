using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Tests
{
    public class TestData
    {

        //Sample Data for Testing
        public static ObservableCollection<DteqModel> TestDteqList = new ObservableCollection<DteqModel>() {
            new DteqModel() {Tag = "XFR-01", Type = DteqTypes.XFR.ToString(), FedFromTag = GlobalConfig.Utility, LineVoltage=4160, LoadVoltage=480, Size=5000, Unit= Units.kVA.ToString() },
            new DteqModel() {Tag = "SWG-01", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-01", LineVoltage=480, LoadVoltage=480, Size=5000, Unit= Units.A.ToString() },
            new DteqModel() {Tag = "MCC-01", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-01", LineVoltage=480, LoadVoltage=480, Size=2000, Unit= Units.A.ToString() },

            new DteqModel() {Tag = "XFR-02", Type = DteqTypes.XFR.ToString(), FedFromTag = GlobalConfig.Utility, LineVoltage=4160, LoadVoltage=600, Size=5000, Unit= Units.kVA.ToString() },
            new DteqModel() {Tag = "SWG-02", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-02", LineVoltage=600, LoadVoltage=600, Size=3000, Unit= Units.A.ToString() },
            new DteqModel() {Tag = "MCC-02", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-02", LineVoltage=600, LoadVoltage=600, Size=1200, Unit= Units.A.ToString() }
        };

        public static ObservableCollection<LoadModel> TestLoadList = new ObservableCollection<LoadModel>() {
            new LoadModel() {Tag = "MTR-01", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-01", Voltage=460, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-01", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-01", Voltage=480, Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-01", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-01", Voltage=480, Size = 50,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "MTR-02", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-02", Voltage=575, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-02", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-02", Voltage=600, Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-02", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-02", Voltage=600, Size = 100,Unit=Units.HP.ToString()},

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
