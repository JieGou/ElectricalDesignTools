using System;

namespace EDTLibrary.LibraryData.TypeModels
{
    public class BreakerSize
    {
        public int Id { get; set; }
        public double FrameAmps { get; set; }
        public double TripAmps { get; set; }
        public double HeatLoss { get; set; }
        public double Ohm { get; set; }
    }
}