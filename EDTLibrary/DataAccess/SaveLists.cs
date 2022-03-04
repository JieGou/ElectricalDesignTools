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
            "PowerCable",
            "Components",
            "FedFrom"
        };

        static public List<string> LoadSaveList { get; set; } = new List<string>() 
        {
            "InLineComponents",
            "CircuitComponents",
            "PowerCable",
            "Components",
            "FedFrom"
        };

        static public List<string> PowerCableSaveList { get; set; } = new List<string> {

            "Load",
            "InLineComponents",
        };

        static public List<string> LocationSaveList { get; set; } = new List<string>() {
            "none"
        };


    }
}
