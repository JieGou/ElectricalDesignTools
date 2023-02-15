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
        bool IsValid { get; set; }
        double DemandFactor { get; set; }
        double Efficiency { get; set; }

        //string StarterType { get; set; }
        //string StarterSize { get; set; }



    }
}