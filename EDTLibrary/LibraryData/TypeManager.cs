using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.Cables;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Components;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary.LibraryData
{
    /// <summary>
    /// Object Model Type lists and Query methods
    /// </summary>
    public class TypeManager
    {
        private static ObservableCollection<string> _componentTypes;

        //General
        public static ObservableCollection<string> ElectricalCodes
        {
            get
            {
                return new ObservableCollection<string> { GlobalConfig.Code_Cec,
                                                          GlobalConfig.Code_Nec,
                                                          GlobalConfig.Code_Iec
                                                         };
            }
        }
        public static ObservableCollection<VoltageType> VoltageTypes { get; set; }

        //Transformers
        public static ObservableCollection<TransformerSize> TransformerSizes { get; set; }
        public static ObservableCollection<TransformerType> TransformerTypes { get; set; }
        public static ObservableCollection<GroundingSystemType> TransformerGroundingTypes { get; set; }

        //Cables
        public static ObservableCollection<string> CableInstallationTypes
        {
            get
            {
                return new ObservableCollection<string> { GlobalConfig.CableInstallationType_LadderTray,
                                                          GlobalConfig.CableInstallationType_DirectBuried,
                                                          GlobalConfig.CableInstallationType_RacewayConduit
                                                         };
            }
        }
        public static ObservableCollection<CableTypeModel> CableTypes { get; set; }
        public static ObservableCollection<ControlCableSizeModel> ControlCableSizes { get; set; }
        public static ObservableCollection<ControlCableSizeModel> InstrumentCableSizes { get; set; }
        public static ObservableCollection<CableTypeModel> OneKvPowerCableTypes
        {
            get
            {
                var val = CableTypes.Where(c => c.VoltageClass == 1000
                                                         && c.UsageType == CableUsageTypes.Power.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CableTypeModel> FiveKvPowerCableTypes
        {
            get
            {
                var val = CableTypes.Where(c => c.VoltageClass == 5000
                                                         && c.UsageType == CableUsageTypes.Power.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CableTypeModel> FifteenKvPowerCableTypes
        {
            get
            {
                var val = CableTypes.Where(c => c.VoltageClass == 15000
                                                         && c.UsageType == CableUsageTypes.Power.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CableTypeModel> PowerCableTypes
        {
            get
            {
                var val = CableTypes.Where(c => c.UsageType == CableUsageTypes.Power.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CableTypeModel> ControlCableTypes
        {
            get
            {
                var val = CableTypes.Where(c => c.UsageType == CableUsageTypes.Control.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CableTypeModel> InstrumentCableTypes
        {
            get
            {
                var val = CableTypes.Where(c => c.UsageType == CableUsageTypes.Instrument.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CecCableSizingRule> CecCableSizingRules { get; set; }
        public static ObservableCollection<ConductorPropertyType> ConductorProperties { get; set; }

        //Enclosures
        public static ObservableCollection<NemaType> NemaTypes { get; set; }
        public static ObservableCollection<AreaClassificationType> AreaClassifications { get; set; }
        public static ObservableCollection<AreaCategory> AreaCategories { get; set; } = new ObservableCollection<AreaCategory>{
            new AreaCategory{CategoryName = "None / Dry"},
            new AreaCategory{CategoryName = "Category 1"},
            new AreaCategory{CategoryName = "Category 2"}
        };

        //OCPD
        public static ObservableCollection<OcpdType> OcpdTypes { get; set; }
        public static ObservableCollection<DisconnectType> DisconnectTypes { get; set; } = new ObservableCollection<DisconnectType>();

        //Components
        public static ObservableCollection<LcsTypeModel> LcsTypes { get; set; }
        public static ObservableCollection<string> DriveTypes { get; set; } = new ObservableCollection<string>() { "VSD", "RVS" };
        public static ObservableCollection<BreakerSize> BreakerSizes { get; set; } = new ObservableCollection<BreakerSize>();
        public static ObservableCollection<StarterSize> StarterSizes { get; set; } = new ObservableCollection<StarterSize>();
        public static ObservableCollection<VfdHeatSize> VfdHeatSizes { get; set; } = new ObservableCollection<VfdHeatSize>();


        //LOAD DATA
        public static void GetTypeTables()
        {
            NemaTypes = DaManager.libDb.GetRecords<NemaType>("NemaTypes");
            AreaClassifications = DaManager.libDb.GetRecords<AreaClassificationType>("AreaClassifications");

            OcpdTypes = DaManager.libDb.GetRecords<OcpdType>("OcpdTypes");
            VoltageTypes = DaManager.libDb.GetRecords<VoltageType>("VoltageTypes");


            LcsTypes = DaManager.libDb.GetRecords<LcsTypeModel>(GlobalConfig.LcsTypesTable);
            DisconnectTypes = DaManager.libDb.GetRecords<DisconnectType>("DisconnectTypes");

            CecCableSizingRules = DaManager.libDb.GetRecords<CecCableSizingRule>("CecCableSizingRules");
            CableTypes = DaManager.libDb.GetRecords<CableTypeModel>(GlobalConfig.CableTypes);
            ControlCableSizes = DaManager.libDb.GetRecords<ControlCableSizeModel>(GlobalConfig.ControlCableSizeTable);
            InstrumentCableSizes = DaManager.libDb.GetRecords<ControlCableSizeModel>(GlobalConfig.ControlCableSizeTable);
            ConductorProperties = DaManager.libDb.GetRecords<ConductorPropertyType>("ConductorProperties");


            TransformerSizes = DaManager.libDb.GetRecords<TransformerSize>("TransformerSizes");
            TransformerTypes = DaManager.libDb.GetRecords<TransformerType>("TransformerTypes");
            TransformerGroundingTypes = DaManager.libDb.GetRecords<GroundingSystemType>("TransformerGroundingTypes");

            BreakerSizes = DaManager.libDb.GetRecords<BreakerSize>("BreakerSizes");
            StarterSizes = DaManager.libDb.GetRecords<StarterSize>("Starters");
            VfdHeatSizes = DaManager.libDb.GetRecords<VfdHeatSize>("VFDHeatLoss");

            DataTableManager.CecCableAmpacities = DaManager.libDb.GetRecords<CecCableAmpacityModel>("CecCableAmpacities");
        }


        //Local Control Station
        public static LcsTypeModel GetLcsTypeModel(string lcsType)
        {
            LcsTypeModel output = null;

            output = LcsTypes.SingleOrDefault(l => l.Type == lcsType);
            return output;
        }

        //Cables
        public static CableTypeModel GetCableTypeModel(string cableType)
        {
            CableTypeModel output = null;

            output = CableTypes.SingleOrDefault(ct => ct.Type == cableType);
            return output;
        }
        public static CableTypeModel GetLcsControlCableTypeModel(LocalControlStationModel lcs)
        {
            CableTypeModel cableType = new CableTypeModel();


            List<CableTypeModel> list = CableTypes.Where(c => c.UsageType == CableUsageTypes.Control.ToString()
                                                                && c.ConductorQty >= lcs.TypeModel.DigitalConductorQty).ToList();
            var minValue = list.Min(c => c.ConductorQty);
            cableType = list.FirstOrDefault(c => c.ConductorQty == minValue);

            return cableType;
        }

        //Components
        public static BreakerSize GetBreaker(double fla, int breakerRating = 80)
        {
            var breaker = new BreakerSize();
            var breakerList = new List<BreakerSize>();
            if (breakerRating != 100) {
                breakerList = BreakerSizes.Where(b => b.TripAmps >= fla * 1.25).ToList();
                if (breakerList.Count > 0) {
                    breaker = breakerList.OrderBy(b => b.TripAmps).First();
                }
            }
            else {
                breakerList = BreakerSizes.Where(b => b.TripAmps >= fla).ToList();
                if (breakerList.Count > 0) {
                    breaker = breakerList.OrderBy(b => b.TripAmps).First();
                }
            }

            return breaker;
        }

        public static StarterSize GetStarter(double motorSize, string unit = "HP")
        {
            var starter = new StarterSize();
            var starterList = new List<StarterSize>();

            starterList = StarterSizes.Where(s => s.Hp >= motorSize && s.Unit.ToLower() == unit.ToLower()).ToList();
            starter = starterList.OrderBy(b => b.Size).First();

            return starter;
        }
        public static VfdHeatSize GetVfdHeatSize(double motorSize, double voltage)
        {
            var vfdSize = new VfdHeatSize();
            var vfdList = new List<VfdHeatSize>();

            try {
                vfdList = VfdHeatSizes.Where(v => v.Hp >= motorSize && v.Voltage == voltage).ToList();
                vfdSize = vfdList.OrderBy(v => v.Hp).First();
                return vfdSize;
            }
            catch (System.Exception) {

            }
            return vfdSize;

        }

    }
}
