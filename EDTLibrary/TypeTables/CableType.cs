using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.TypeTables
{
    public class CableType

    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Conductors { get; set; }
        public double VoltageClass { get; set; }
        public string SubType { get; set; }
        public string UsageType { get; set; }
        public double Insulation { get; set; }

    }
}
