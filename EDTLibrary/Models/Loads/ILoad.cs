using EDTLibrary.Models.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EDTLibrary.Models.Loads
{
    public interface ILoad : IPowerConsumer, ComponentUser
    {
        //Primary
        double LoadFactor { get; set; }
        double Efficiency { get; set; }

    }
}