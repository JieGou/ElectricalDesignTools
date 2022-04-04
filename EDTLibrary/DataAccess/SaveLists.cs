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
            "FedFrom",
            "Area",
            "LargestMotor"
        };

        static public List<string> LoadSaveList { get; set; } = new List<string>() 
        {
            "InLineComponents",
            "CircuitComponents",
            "PowerCable",
            "Components",
            "FedFrom",
            "Area"
        };

        static public List<string> PowerCableSaveList { get; set; } = new List<string> {

            "Load",
            "InLineComponents",
            "TypeModel"
        };

        static public List<string> AreaSaveList { get; set; } = new List<string>() {
            "none",
            "ParentArea"
        };


    }
}
