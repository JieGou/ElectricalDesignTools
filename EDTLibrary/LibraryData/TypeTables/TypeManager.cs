using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EDTLibrary.LibraryData.TypeTables
{
    public class TypeManager
    {

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
        public static ObservableCollection<CableTypeModel> CableTypes { get; set; }
        public static ObservableCollection<CableTypeModel> OneKvCableTypes
        {
            get
            {
                var val = TypeManager.CableTypes.Where(c => c.VoltageClass == 1000).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CableTypeModel> FiveKvCableTypes
        {
            get
            {
                var val = TypeManager.CableTypes.Where(c => c.VoltageClass == 5000).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
        public static ObservableCollection<CableTypeModel> FifteenKvCableTypes
        {
            get
            {
                var val = TypeManager.CableTypes.Where(c => c.VoltageClass == 15000).ToList();
                return new ObservableCollection<CableTypeModel>(val);
            }
        }
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




        public static ObservableCollection<CecCableSizingRule> CecCableSizingRules { get; set; }
        public static CableTypeModel GetCableType(string cableType)
        {
            CableTypeModel output = null;

            output = CableTypes.SingleOrDefault(ct => ct.Type == cableType);
            return output;
        }


        //Enclosures
        public static ObservableCollection<NemaType> NemaTypes { get; set; }
        public static ObservableCollection<AreaClassificationType> AreaClassifications { get; set; }
        public static ObservableCollection<AreaCategory> AreaCategories { get; set; } = new ObservableCollection<AreaCategory>{
            new AreaCategory{CategoryName = "Category 1"},
            new AreaCategory{CategoryName = "Category 2"}
        };

    }
}
