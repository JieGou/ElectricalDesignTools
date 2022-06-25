using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EDTLibrary.LibraryData.TypeTables
{
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
                var val = TypeManager.CableTypes.Where(c => c.VoltageClass == 1000
                                                         && c.UsageType == CableUsageTypes.Power.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CableTypeModel> FiveKvPowerCableTypes
        {
            get
            {
                var val = TypeManager.CableTypes.Where(c => c.VoltageClass == 5000
                                                         && c.UsageType == CableUsageTypes.Power.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CableTypeModel> FifteenKvPowerCableTypes
        {
            get
            {
                var val = TypeManager.CableTypes.Where(c => c.VoltageClass == 15000
                                                         && c.UsageType == CableUsageTypes.Power.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }


        public static ObservableCollection<CableTypeModel> PowerCableTypes
        {
            get
            {
                var val = TypeManager.CableTypes.Where(c => c.UsageType == CableUsageTypes.Power.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CableTypeModel> ControlCableTypes
        {
            get
            {
                var val = TypeManager.CableTypes.Where(c => c.UsageType == CableUsageTypes.Control.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CableTypeModel> InstrumentCableTypes
        {
            get
            {
                var val = TypeManager.CableTypes.Where(c => c.UsageType == CableUsageTypes.Instrument.ToString()).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }

        public static ObservableCollection<TransformerSize> TransformerSizes { get; set; }
        public static ObservableCollection<TransformerType> TransformerTypes { get; set; }
        public static ObservableCollection<GroundingSystemType> GroundingSystemTypes { get; set; }
        //Components
        public static ObservableCollection<LcsTypeModel> LcsTypes { get; set; }
        public static LcsTypeModel GetLcsTypeModel(string lcsType)
        {
            LcsTypeModel output = null;

            output = LcsTypes.SingleOrDefault(l => l.Type == lcsType);
            return output;
        }

        //Cables
        public static ObservableCollection<CecCableSizingRule> CecCableSizingRules { get; set; }
        public static CableTypeModel GetCableTypeModel(string cableType)
        {
            CableTypeModel output = null;

            output = CableTypes.SingleOrDefault(ct => ct.Type == cableType);
            return output;
        }
        public static CableTypeModel GetLcsControlCableTypeModel(LocalControlStationModel lcs)
        {
            CableTypeModel cableType = new CableTypeModel();


            List<CableTypeModel> list = TypeManager.CableTypes.Where(c => c.UsageType == CableUsageTypes.Control.ToString()
                                                                && c.ConductorQty >= lcs.TypeModel.DigitalConductorQty).ToList();
            var minValue = list.Min(c => c.ConductorQty);
            cableType = list.FirstOrDefault(c => c.ConductorQty == minValue);

            return cableType;
        }

        //Enclosures
        public static ObservableCollection<NemaType> NemaTypes { get; set; }
        public static ObservableCollection<AreaClassificationType> AreaClassifications { get; set; }
        public static ObservableCollection<AreaCategory> AreaCategories { get; set; } = new ObservableCollection<AreaCategory>{
            new AreaCategory{CategoryName = "None / Dry"},
            new AreaCategory{CategoryName = "Category 1"},
            new AreaCategory{CategoryName = "Category 2"}
        };


        //OCDP

        public static ObservableCollection<OcpdType> OcpdTypes { get; set; }
        public static ObservableCollection<string> ComponentTypes 
        { get
            {

                ObservableCollection<string> componentTypes = new ObservableCollection<string>();
                foreach (var item in Enum.GetNames(typeof(ComponentTypes))) {
                    componentTypes.Add(item.ToString());
                }
                return componentTypes;
            }
        }


    }
}
