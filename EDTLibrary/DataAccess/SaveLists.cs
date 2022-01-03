using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.DataAccess
{
    public class SaveLists
    {
        static public List<string> DteqSaveList { get; set; } = new List<string>()
        {   
            //"Tag",
            //"Type",
            //"Description",
            //"FedFrom",
            //"Voltage",
            //"Size",
            //"Unit",
            //"LineVoltage",
            //"LoadVoltage",

            "AssignedLoads",
            "InLineComponents"
        };

        static public List<string> LoadSaveList { get; set; } = new List<string>() {
            "InLineComponents"
        };

        static public List<string> CableSaveList { get; set; }

        static public List<string> LocationSaveList { get; set; } = new List<string>() {
            "none"
        };


    }
}
