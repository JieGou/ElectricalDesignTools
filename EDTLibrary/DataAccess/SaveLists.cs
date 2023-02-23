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

            "DemandFactor",
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
            "StandAloneStarterBool",
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
            "Default",
            "IsInvalidMessage",
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
        static public List<string> CableSaveList { get; set; } = new List<string> {

            "AmpacityTable",
            "BaseAmps",
            "BondWireSize",
            "Category",
            "ConductorQty",
            "DeratedAmps",
            "Derating",
            "Derating5A",
            "Derating5C",
            "Destination",
            "Diameter",
            "HeatLoss",
            "Id",
            "InstallationDiagram",
            "InstallationType",
            "InsulationPercentage",
            "InvalidAmpacityMessage",
            "IsInvalidMessage",
            "InvalidLengthMessage",
            "Is1C",
            "IsOutdoor",
            "IsValid",
            "Length",
            "LoadId",
            "LoadType",
            "MaxVoltageDropPercentage",
            "OwnerId",
            "OwnerType",
            "QtyParallel",
            "RequiredAmps",
            "RequiredSizingAmps",
            "Size",
            "Source",
            "SourceId",
            "SourceType",
            "Spacing",
            "Tag",
            "Type",
            "TypeId",
            "UsageType",
            "VoltageDrop",
            "VoltageDropPercentage",
            "VoltageRating",
            "WeightKgKm",
            "WeightLbs1kFeet",
        };
        static public List<string> AreaNoSaveList { get; set; } = new List<string>() {
            "none",
            "ParentArea",
            "EquipmentList",
        };


    }
}
