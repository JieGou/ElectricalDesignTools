using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.DataAccess
{
    public class SaveLists
    {
        static public List<string> DteqSaveList { get; set; } = new List<string>()
        {   
            "InLineComponents",
            "AssignedLoads",
            "CircuitComponents",
            "Cable",
            "Components",
            "FedFrom"
        };

        static public List<string> LoadSaveList { get; set; } = new List<string>() 
        {
            "InLineComponents",
            "CircuitComponents",
            "Cable",
            "Cable",
            "Components",
            "FedFrom"
        };

        static public List<string> PowerCableSaveList { get; set; } = new List<string> {

            "Load",
            "InLineComponents",
            "AssignedLoads",
            "CircuitComponents",
            "Components"
        };

        static public List<string> LocationSaveList { get; set; } = new List<string>() {
            "none"
        };


    }
}
