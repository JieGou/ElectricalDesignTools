﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.TypeTables
{
    public class TypeManager
    {
        public static List<VoltageType> VoltageTypes { get; set; }
        public static List<CableType> CableTypes { get; set; }
        public static List<NemaType> NemaTypes { get; set; }
        public static List<AreaClassificationType> AreaClassifications { get; set; }

    }
}