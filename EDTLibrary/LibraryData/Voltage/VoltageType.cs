using System;
using System.Collections.Generic;
using System.Text;

namespace EdtLibrary.LibraryData.Voltage
{
    public class VoltageType
    {
        public int Id { get; set; }
        public double Voltage
        {
            get;
            set;
        }
        public double Phase { get; set; }
        public string VoltageString
        {
            get;
            set;
        }
        public int Poles { get; set; }

        public override string ToString()
        {
            return VoltageString;
        }
    }
}
