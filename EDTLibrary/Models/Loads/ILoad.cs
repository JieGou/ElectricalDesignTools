using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EDTLibrary.Models
{
    public interface ILoad : IPowerConsumer, ComponentUser
    {
        //Primary
        double LoadFactor { get; set; }
        double Efficiency { get; set; }

    }
}