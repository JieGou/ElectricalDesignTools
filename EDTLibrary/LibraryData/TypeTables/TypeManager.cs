using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EDTLibrary.LibraryData.TypeTables
{
    public class TypeManager
    {
        public static ObservableCollection<VoltageType> VoltageTypes { get; set; }
        public static ObservableCollection<PowerCableTypeModel> PowerCableTypes { get; set; }
        public static ObservableCollection<NemaType> NemaTypes { get; set; }
        public static ObservableCollection<AreaClassificationType> AreaClassifications { get; set; }
        public static ObservableCollection<CecCableSizingRule> CecCableSizingRules { get; set; }
        public static ObservableCollection<AreaCategory> AreaCategories { get; set; } = new ObservableCollection<AreaCategory>{
            new AreaCategory{CategoryName = "Category 1"},
            new AreaCategory{CategoryName = "Category 2"}
        };

        public static PowerCableTypeModel GetCableType(string cableType)
        {
            PowerCableTypeModel output = null;

            output = PowerCableTypes.SingleOrDefault(ct => ct.Type == cableType);
            return output;
        }
    }
}
