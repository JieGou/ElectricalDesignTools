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
            "Id",
            "Tag",
            "Category",
            "Type",
            "SubType",
            "Description",

            "AreaId",
            "NemaRating",
            "AreaClassification",

            "FedFromId",
            "FedFromTag",
            "FedFromType",
            
            "Voltage",
            "LineVoltage",
            "LoadVoltage",
            "Size",
            "Unit",

            "Efficiency",
            "PowerFactor",
            "ConnectedKva",
            "DemandKva",
            "DemandKw",
            "DemandKvar",

            "RunningAmps",
            "Fla",
            "AmpacityFactor",

            "PdType",
            "PdSizeTrip",
            "PdSizeFrame",
            "PercentLoaded",

            "SCCR",
            "LoadCableDerating",
        };
        static public List<string> LoadSaveList { get; set; } = new List<string>() 
        {
            "Id",
            "Tag",
            "Category",
            "Type",
            "SubType",
            "Description",

            "AreaId",
            "NemaRating",
            "AreaClassification",

            "FedFromId",
            "FedFromTag",
            "FedFromType",

            "Voltage",
            "LineVoltage",
            "LoadVoltage",
            "Size",
            "Unit",

            "LoadFactor",
            "Efficiency",
            "PowerFactor",
            "ConnectedKva",
            "DemandKva",
            "DemandKw",
            "DemandKvar",

            "RunningAmps",
            "Fla",
            "AmpacityFactor",

            "PdType",
            "PdSizeTrip",
            "PdSizeFrame",
            "StarterSize",

            "DisconnectBool",
            "DriveBool",
            "LcsBool",
            "HeatLoss",

        };
        static public List<string> CompSaveList { get; set; } = new List<string>()
        {
            "Id",
            "Tag",
            "Category",
            "SubCategory",
            "Type",
            "SubType",
            "Description",

            "AreaId",
            "NemaRating",
            "AreaClassification",

            "FrameAmps",
            "TripAmps",

            "OwnerId",
            "OwnerType",
            "SequenceNumber",

            "HeatLoss",
            "Voltage",
            "Default",

        };
        static public List<string> LcsNoSaveList { get; set; } = new List<string>()
        {
            "Id",
            "Tag",
            "Category",
            "SubCategory",
            "Type",
            "SubType",
            "Description",

            "AreaId",
            "NemaRating",
            "AreaClassification",

            "OwnerId",
            "OwnerType",
            "SequenceNumber",

            "Voltage",
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
            "SizeTag",
        };
        static public List<string> AreaNoSaveList { get; set; } = new List<string>() {
            "none",
            "ParentArea",
            "EquipmentList",
        };


    }
}
