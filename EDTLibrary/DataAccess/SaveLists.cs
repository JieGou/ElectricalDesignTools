using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.DataAccess
{
    public class SaveLists
    {
        static public List<string> DteqSaveList { get; set; } = new List<string>()
        {   
            "AssignedLoads",
            "CircuitComponents",
            "Cable",
            "Components"
        };

        static public List<string> LoadSaveList { get; set; } = new List<string>() {
            "CircuitComponents",
            "Cable",
            "CableModel",
            "Components"
        };

        static public List<string> CableSaveList { get; set; } = new List<string> {
            "Load",
            "Cable",
            "CableModel",
            "Components"
        };

        static public List<string> LocationSaveList { get; set; } = new List<string>() {
            "none"
        };


    }
}
