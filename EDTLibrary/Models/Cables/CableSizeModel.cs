﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables
{
    [Serializable]

    public class CableSizeModel
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string BondWireSize { get; set; }

        public bool UsedInProject { get; set; }

        public double Diameter { get; set; }
        public double WeightLbs1kFeet { get; set; }
        public double WeightKgKm { get; set; }
    }
}
