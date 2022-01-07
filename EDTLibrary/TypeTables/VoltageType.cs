using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.TypeTables
{
    public class VoltageType
    {
        public int Id { get; set; }
        public double Voltage { get; set; }
        public double Phase { get; set; }
        public string Category { get; set; }
        public double CableVoltageClass { get; set; }
        public double EquipmentVoltageClass { get; set; }
    }
}
