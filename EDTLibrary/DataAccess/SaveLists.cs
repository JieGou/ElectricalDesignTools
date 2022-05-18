using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.DataAccess
{
    /// <summary>
    /// Lists the properties that are ignored when saving an object to the Database."
    /// </summary>
    public class SaveLists
    {
        static public List<string> DteqSaveList { get; set; } = new List<string>()
        {   
            "InLineComponents",
            "AssignedLoads",
            "PowerCable",
            "Components",
            "CctComponents",
            "FedFrom",
            "Area",
            "LargestMotor",
            "DriveBool",
            "DriveId",
            "DisconnectBool",
            "DisconnectId",
            "LcsBool",
            "Lcs",

        };

        static public List<string> LoadSaveList { get; set; } = new List<string>() 
        {
            "InLineComponents",
            "PowerCable",
            "Components",
            "CctComponents",
            "FedFrom",
            "Area",
            "Lcs",
        };

        static public List<string> PowerCableSaveList { get; set; } = new List<string> {

            "Load",
            "InLineComponents",
            "TypeModel",
            "TypeList",
            "DeratedAmpsToolTip",
            "RequiredAmpsToolTip",
        };

        static public List<string> AreaSaveList { get; set; } = new List<string>() {
            "none",
            "ParentArea",
        };


    }
}
