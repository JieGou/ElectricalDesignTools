using EDTLibrary.Models.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EDTLibrary.Models.Loads
{
    public interface ILoad : IPowerConsumer, IComponentUser
    {
        //Primary
        double LoadFactor { get; set; }
        double Efficiency { get; set; }

        string StarterType { get; set; }
        double StarterSize { get; set; }


        
    }
}