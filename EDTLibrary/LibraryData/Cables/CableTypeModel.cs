using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.LibraryData.Cables
{
    [Serializable]

    public class CableTypeModel

    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string TypeDisplayText { get; set; }
        public int ConductorQty { get; set; }
        public string ConductorType { get; set; }
        public string ConductorTag { get; set; }
        public double VoltageRating { get; set; }
        public string SubType { get; set; }
        public string UsageType { get; set; }
        public double InsulationPercentage { get; set; }
        public bool Shielded { get; set; }

        public double TemperatureRating { get; set; }
    }
}
