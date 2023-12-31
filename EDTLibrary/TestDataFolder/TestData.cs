﻿using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
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
            new AreaModel() {Tag = "ML", DisplayTag = "ML", Name = "Mill", AreaCategory = "Category 1", NemaRating = "Type 12",
                            AreaClassification = "Non-Hazardous", MinTemp= -40, MaxTemp=30},

            new AreaModel() {Tag = "FL", DisplayTag = "FL",Name = "Flotation", AreaCategory = "Category 1", NemaRating = "Type 3R",
                            AreaClassification = "Non-Hazardous", MinTemp= -40, MaxTemp=30},

            new AreaModel() {Tag = "DR", DisplayTag = "DR",Name = "Dry", AreaCategory = "Category 1", NemaRating = "Type 12",
                            AreaClassification = "Non-Hazardous", MinTemp= -40, MaxTemp=30}
        };


        public static ObservableCollection<IDteq> TestDteqList_Full = new ObservableCollection<IDteq>() {
            new DteqModel {Tag = "XFR-01", Type = DteqTypes.XFR.ToString(), FedFromTag = GlobalConfig.UtilityTag, LineVoltage = 13800, LoadVoltage = 4160,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==13800),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==4160), Size = 5000, Unit = Units.kVA.ToString() },
            
            new DteqModel {Tag = "SWG-01", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-01", LineVoltage = 4160, LoadVoltage = 4160,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==4160),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==4160), Size = 1200, Unit = Units.A.ToString() },

            new DteqModel {Tag = "XFR-03", Type = DteqTypes.XFR.ToString(), FedFromTag = "SWG-01", LineVoltage=4160, LoadVoltage=480,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==4160),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size=1500, Unit= Units.kVA.ToString() },
            
            new DteqModel {Tag = "SWG-03", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-03", LineVoltage=480, LoadVoltage=480,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size=2000, Unit= Units.A.ToString() },
            new DteqModel {Tag = "MCC-01", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-03", LineVoltage=480, LoadVoltage=480,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size=2000, Unit= Units.A.ToString() },
            new DteqModel {Tag = "MCC-03", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-03", LineVoltage=480, LoadVoltage=480,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size=1200, Unit= Units.A.ToString() },
            
            new DteqModel {Tag = "XFR-02", Type = DteqTypes.XFR.ToString(), FedFromTag = GlobalConfig.UtilityTag,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==4160), LineVoltage = 4160,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LoadVoltage = 600, Size = 2500, Unit = Units.kVA.ToString() },

            new DteqModel {Tag = "SWG-02", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-02",
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LineVoltage=600,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LoadVoltage=600, Size=2000, Unit= Units.A.ToString() },

            new DteqModel {Tag = "MCC-02", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-02",
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LineVoltage=600,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LoadVoltage=600, Size=1200, Unit= Units.A.ToString() },

            new DteqModel {Tag = "TX-02", Type = DteqTypes.XFR.ToString(), FedFromTag = "MCC-02", LineVoltage=600,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LoadVoltage=208,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), Size=112.5, Unit= Units.kVA.ToString() },

            new DteqModel {Tag = "LDP-01", Type = DteqTypes.DPN.ToString(), FedFromTag = "TX-02", LineVoltage=208,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), LoadVoltage=208,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), Size=200, Unit= Units.A.ToString() },

            new DteqModel {Tag = "LDP-02", Type = DteqTypes.DPN.ToString(), FedFromTag = "LDP-01", 
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), LineVoltage=208,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), LoadVoltage=208, Size=125, Unit= Units.A.ToString() },

            new DteqModel {Tag = "Splitter-01", Type = DteqTypes.SPL.ToString(), FedFromTag = "SWG-02", 
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LineVoltage=600,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LoadVoltage=600, Size=600, Unit= Units.A.ToString() },


        };




        public static ObservableCollection<ILoad> TestLoadList_Full = new ObservableCollection<ILoad>() {
            new LoadModel() {Tag = "MTR-01", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-01", Voltage=460, 
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==460), Size = 50,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "HTR-01", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-01", Voltage=480,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 50,Unit=Units.kW.ToString()},
            new LoadModel() {Tag = "PNL-01", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-01", Voltage=480,  
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 250,Unit=Units.A.ToString()},
            

            new LoadModel() {Tag = "MTR-02", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-02", Voltage=575,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==575), Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-02", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-02", Voltage=600,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), Size = 100,Unit=Units.kW.ToString()},
            new LoadModel() {Tag = "PNL-02", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-02", Voltage=600,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), Size = 100,Unit=Units.A.ToString()},

            new LoadModel() {Tag = "MTR-03", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-03", Voltage=460,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==460), Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-03", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-03", Voltage=480,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 100,Unit=Units.kW.ToString()},
            new LoadModel() {Tag = "PNL-03", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-03", Voltage=480,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 100,Unit=Units.A.ToString()},

            new LoadModel() {Tag = "MTR-011", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-01", Voltage=460,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==460), Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-011", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-01", Voltage=480,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 50,Unit=Units.kW.ToString()},
            new LoadModel() {Tag = "PNL-011", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-01", Voltage=480,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 50,Unit=Units.A.ToString()},

            new LoadModel() {Tag = "MTR-022", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-02", Voltage=575,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==575), Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-022", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-02", Voltage=600,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-022", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-02", Voltage=600,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), Size = 100,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "MTR-033", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-03", Voltage=460,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==460), Size = 75,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-033", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-03", Voltage=480,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-033", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-03", Voltage=408,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 100,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "MTR-111", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-01", Voltage=460,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==460), Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-111", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-01", Voltage=480,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 50,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-111", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-01", Voltage=480,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 50,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "MTR-222", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-02", Voltage=575,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==575), Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-222", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-02", Voltage=600,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-222", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-02", Voltage=600,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), Size = 100,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "MTR-333", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-03", Voltage=460,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==460), Size = 75,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "HTR-333", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-03", Voltage=480,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 100,Unit=Units.HP.ToString()},
            new LoadModel() {Tag = "PNL-333", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-03", Voltage=408,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 60,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "LD-01", Type = LoadTypes.OTHER.ToString(), FedFromTag = "LDP-01", Voltage=120,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==120), Size = 10,Unit=Units.A.ToString()},
            new LoadModel() {Tag = "LD-02", Type = LoadTypes.OTHER.ToString(), FedFromTag = "LDP-01", Voltage=208,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), Size = 20, Unit=Units.A.ToString()},
            new LoadModel() {Tag = "LD-03", Type = LoadTypes.OTHER.ToString(), FedFromTag = "LDP-01", Voltage=208,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), Size = 30,Unit=Units.A.ToString()},
            new LoadModel() {Tag = "LD-04", Type = LoadTypes.OTHER.ToString(), FedFromTag = "LDP-01", Voltage=208,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), Size = 40,Unit=Units.A.ToString()},

            new LoadModel() {Tag = "LD-05", Type = LoadTypes.OTHER.ToString(), FedFromTag = "LDP-02", Voltage=120,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==120), Size = 5,Unit=Units.A.ToString()},
            new LoadModel() {Tag = "LD-06", Type = LoadTypes.OTHER.ToString(), FedFromTag = "LDP-02", Voltage=208,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), Size = 10, Unit=Units.A.ToString()},
            new LoadModel() {Tag = "LD-07", Type = LoadTypes.OTHER.ToString(), FedFromTag = "LDP-02", Voltage=208,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), Size = 15,Unit=Units.A.ToString()},
            new LoadModel() {Tag = "LD-08", Type = LoadTypes.OTHER.ToString(), FedFromTag = "LDP-02", Voltage=208,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), Size = 20,Unit=Units.A.ToString()},

        };


        public static ObservableCollection<IDteq> TestDteqList_Short = new ObservableCollection<IDteq>() {
            new DteqModel {Tag = "XFR-01", Type = DteqTypes.XFR.ToString(), FedFromTag = GlobalConfig.UtilityTag, LineVoltage = 13800, LoadVoltage = 4160,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==13800),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==4160), Size = 5000, Unit = Units.kVA.ToString() },

            new DteqModel {Tag = "SWG-01", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-01", LineVoltage = 4160, LoadVoltage = 4160,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==4160),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==4160), Size = 1200, Unit = Units.A.ToString() },

            new DteqModel {Tag = "XFR-03", Type = DteqTypes.XFR.ToString(), FedFromTag = "SWG-01", LineVoltage=4160, LoadVoltage=480,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==4160),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size=1500, Unit= Units.kVA.ToString() },

            new DteqModel {Tag = "SWG-03", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-03", LineVoltage=480, LoadVoltage=480,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size=2000, Unit= Units.A.ToString() },
            new DteqModel {Tag = "MCC-01", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-03", LineVoltage=480, LoadVoltage=480,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size=2000, Unit= Units.A.ToString() },
            new DteqModel {Tag = "MCC-03", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-03", LineVoltage=480, LoadVoltage=480,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480),
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size=1200, Unit= Units.A.ToString() },

            new DteqModel {Tag = "XFR-02", Type = DteqTypes.XFR.ToString(), FedFromTag = GlobalConfig.UtilityTag,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==4160), LineVoltage = 4160,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LoadVoltage = 600, Size = 2500, Unit = Units.kVA.ToString() },

            new DteqModel {Tag = "SWG-02", Type = DteqTypes.SWG.ToString(), FedFromTag = "XFR-02",
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LineVoltage=600,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LoadVoltage=600, Size=2000, Unit= Units.A.ToString() },

            new DteqModel {Tag = "MCC-02", Type = DteqTypes.MCC.ToString(), FedFromTag = "SWG-02",
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LineVoltage=600,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LoadVoltage=600, Size=1200, Unit= Units.A.ToString() },

            new DteqModel {Tag = "TX-02", Type = DteqTypes.XFR.ToString(), FedFromTag = "MCC-02", LineVoltage=600,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LoadVoltage=208,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), Size=112.5, Unit= Units.kVA.ToString() },

            new DteqModel {Tag = "LDP-01", Type = DteqTypes.DPN.ToString(), FedFromTag = "TX-02", LineVoltage=208,
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), LoadVoltage=208,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), Size=200, Unit= Units.A.ToString() },

            //new DteqModel {Tag = "LDP-02", Type = DteqTypes.DPN.ToString(), FedFromTag = "LDP-01",
            //    LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), LineVoltage=208,
            //    LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), LoadVoltage=208, Size=125, Unit= Units.A.ToString() },

            new DteqModel {Tag = "Splitter-01", Type = DteqTypes.SPL.ToString(), FedFromTag = "SWG-02",
                LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LineVoltage=600,
                LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==600), LoadVoltage=600, Size=600, Unit= Units.A.ToString() },


        };

        public static ObservableCollection<ILoad> TestLoadList_Short = new ObservableCollection<ILoad>() {
            new LoadModel() {Tag = "MTR-01", Type = LoadTypes.MOTOR.ToString(), FedFromTag = "MCC-01", Voltage=460,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==460), Size = 50,Unit=Units.HP.ToString()},

            new LoadModel() {Tag = "HTR-01", Type = LoadTypes.HEATER.ToString(), FedFromTag = "MCC-01", Voltage=480,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 50,Unit=Units.kW.ToString()},
            new LoadModel() {Tag = "PNL-01", Type = LoadTypes.PANEL.ToString(), FedFromTag = "MCC-01", Voltage=480,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==480), Size = 250,Unit=Units.A.ToString()},


            new LoadModel() {Tag = "LD-01", Type = LoadTypes.OTHER.ToString(), FedFromTag = "LDP-01", Voltage=120,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==120), Size = 10,Unit=Units.A.ToString()},
            new LoadModel() {Tag = "LD-02", Type = LoadTypes.OTHER.ToString(), FedFromTag = "LDP-01", Voltage=208,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==208), Size = 20, Unit=Units.A.ToString()},

        };
    }
}
