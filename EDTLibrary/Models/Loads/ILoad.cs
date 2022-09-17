using EDTLibrary.LibraryData.TypeModels;
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
        int VoltageTypeId { get; set; }
        VoltageType VoltageType { get; set; }
        double LoadFactor { get; set; }
        double Efficiency { get; set; }

        string StarterType { get; set; }
        double StarterSize { get; set; }


        
    }
}