using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.LibraryData.TypeModels
{
    public class VoltageType
    {
        public int Id { get; set; }
        public double Voltage { get; set; }
        public double Phase { get; set; }
        public string VoltagePhase { get; set; }
      
    }
}
