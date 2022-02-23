using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EDTLibrary.LibraryData.TypeTables
{
    public class TypeManager
    {
        public static ObservableCollection<VoltageType> VoltageTypes { get; set; }
        public static ObservableCollection<CableType> CableTypes { get; set; }
        public static ObservableCollection<NemaType> NemaTypes { get; set; }
        public static ObservableCollection<AreaClassificationType> AreaClassifications { get; set; }

    }
}
