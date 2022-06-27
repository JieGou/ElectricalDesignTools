﻿using System;

namespace EDTLibrary.LibraryData.TypeTables
{
    public class CableResistanceType
    {
        public int Id { get; set; }
        public string Size { get; set; }
        public double Resistance20C1kFeet { get; set; }
        public double Resistance75C1kFeet { get; set; }
        public double Resistance20C1kMeter { get; set; }
        public double Resistance75C1kMeter { get; set; }
    }
}