using System;

namespace EDTLibrary.LibraryData.Cables
{
    public class ConductorPropertyType
    {
        public int Id { get; set; }
        public string Size { get; set; }
        public double Resistance20C1kFeet { get; set; }
        public double Resistance75C1kFeet { get; set; }
        public double Resistance20C1kMeter { get; set; }
        public double Resistance75C1kMeter { get; set; }
    }
}