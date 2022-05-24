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

        bool DriveBool { get; set; }
        int DriveId { get; set; }
        bool DisconnectBool { get; set; }
        int DisconnectId { get; set; }

        bool LcsBool { get; set; }
        public ComponentModel Lcs { get; set; }
    }
}