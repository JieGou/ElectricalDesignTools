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
        public static ObservableCollection<CableTypeModel> CableTypes { get; set; }
        public static ObservableCollection<NemaType> NemaTypes { get; set; }
        public static ObservableCollection<AreaClassificationType> AreaClassifications { get; set; }
        public static ObservableCollection<CecCableSizingRule> CecCableSizingRules { get; set; }

        public static CableTypeModel GetCableType(string cableType)
        {
            CableTypeModel output = null;

            output = CableTypes.SingleOrDefault(ct => ct.Type == cableType);
            return output;
        }
    }
}
