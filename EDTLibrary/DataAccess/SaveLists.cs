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
            "InLineComponents",
            "Cable"
        };

        static public List<string> LoadSaveList { get; set; } = new List<string>() {
            "InLineComponents",
            "Cable",
            "CableModel"
        };

        static public List<string> CableSaveList { get; set; } = new List<string> {
            "Load",
            "Cable",
            "CableModel"
        };

        static public List<string> LocationSaveList { get; set; } = new List<string>() {
            "none"
        };


    }
}
