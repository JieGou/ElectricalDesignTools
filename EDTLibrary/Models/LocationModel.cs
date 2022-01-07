﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.Models
{
    public  class LocationModel
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string LocationCategory { get; set; }
        public string AreaClassification { get; set; }

        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }

        public string NemaRating { get; set; }

    }
}