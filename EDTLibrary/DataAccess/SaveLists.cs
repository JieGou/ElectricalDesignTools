﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.DataAccess
{
    /// <summary>
    /// Lists the properties that are ignored when saving an object to the Database."
    /// </summary>
    public class SaveLists
    {
        static public List<string> DteqNoSaveList { get; set; } = new List<string>()
        {   
            "InLineComponents",
            "AssignedLoads",
            "PowerCable",
            "Components",
            "AuxComponents",
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
            "Drive",
            "Disconnect",
        };

        static public List<string> LoadNoSaveList { get; set; } = new List<string>() 
        {
            "InLineComponents",
            "PowerCable",
            "Components",
            "AuxComponents",
            "CctComponents",
            "FedFrom",
            "Area",
            "Lcs",
            "Drive",
            "Disconnect",
            "BreakerSize",
            "EfficiencyDisplay",
        };

        static public List<string> CompNoSaveList { get; set; } = new List<string>()
        {
            "Area",
            "Owner",
            "PowerCable",
            "ControlCable",
            "TypeModel",
            "TypeList",
        };

        static public List<string> LcsNoSaveList { get; set; } = new List<string>()
        {
            "Area",
            "Owner",
            "PowerCable",
            "ControlCable",
            "TypeModel",
            "SubTypeList",
            "HeatLoss",
        };

        static public List<string> PowerCableNoSaveList { get; set; } = new List<string> {

            "Load",
            "InLineComponents",
            "TypeModel",
            "TypeList",
            "DeratedAmpsToolTip",
            "RequiredAmpsToolTip",
            "SizeList",
            "AutoSizeCableCommand",
            "DeratingToolTip",
        };

        static public List<string> AreaNoSaveList { get; set; } = new List<string>() {
            "none",
            "ParentArea",
            "EquipmentList",
        };


    }
}
