using System;

namespace EDTLibrary.LibraryData.TypeModels
{
    public class StarterSize
    {
        public int Id { get; set; }
        public double Voltage { get; set; }
        public string Unit { get; set; }
        public double Hp { get; set; }
        public string Size { get; set; }
        public double SizeNumeric { get; set; }
        public double HeatLossWatts { get; set; }
    }
}