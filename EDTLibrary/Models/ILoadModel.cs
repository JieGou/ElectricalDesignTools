using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EDTLibrary.Models
{
    public interface ILoadModel : IHasLoading, IHasComponents
    {
        //Primary
        double LoadFactor { get; set; }
        double Efficiency { get; set; }
        double AmpacityFactor { get; set; }

    }
}