using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EDTLibrary.Models
{
    public interface ILoad : PowerConsumer, ComponentUser
    {
        //Primary
        double LoadFactor { get; set; }
        double Efficiency { get; set; }

    }
}