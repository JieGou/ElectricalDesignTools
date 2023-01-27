using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.DataAccess
{
    /// <summary>
    /// Lists the properties that are ignored when saving an object to the Database."
    /// </summary>
    public class NoSaveLists
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
            "StandAloneStarterBool",
            "StandAloneStarterId",
            "DisconnectBool",
            "DisconnectId",
            "LcsBool",
            "Lcs",
            "StandAloneStarter",
            "Disconnect",
            "IsCalculating",

            "VoltageType",
            "LineVoltageType",
            "LoadVoltageType",

            "CircuitNumbersLeft",
            "CircuitNumbersRight",
            "PoleCountLeft",
            "PoleCountRight",

            "LeftCircuits",
            "RightCircuits",
            "DpnCircuitList",
            "CircuitList",
            "AssignedCircuits",
            "PhaseA",
            "PhaseB",
            "PhaseC",
            "CalculationFlags",

            "IsSelected",
            "SelectedComponent",

            "PrimaryWiringType",
            "SecondaryWiringType",
            "ProtectionDevice",

        };

        static public List<string> LoadNoSaveList { get; set; } = new List<string>() 
        {
            "InLineComponents",
            "AssignedLoads",
            "PowerCable",
            "Components",
            "AuxComponents",
            "CctComponents",
            "FedFrom",
            "Area",
            "Lcs",
            "StandAloneStarter",
            "Disconnect",
            "BreakerSize",
            "EfficiencyDisplay",
            "IsCalculating",

            "VoltageType",
            "CanSave",
            "ConvertToLoadCommand",
            "CalculationFlags",

            "IsSelected",
            "SelectedComponent",

            "ProtectionDevice",
            "CalculationLock",
            "UnitList",
        };

        static public List<string> CompNoSaveList { get; set; } = new List<string>()
        {
            "Area",
            "Owner",
            "Cable",
            "PowerCable",
            "ControlCable",
            "TypeModel",
            "TypeList",

            "IsSelected",
            "SettingTag",

        };

        static public List<string> ProtectionDeviceNoSaveList { get; set; } = new List<string>() {
            "Area",
            "Owner",
            "Cable",
            "PowerCable",
            "ControlCable",
            "TypeModel",
            "TypeList",

            "IsSelected",
            "SettingTag",
        };

        static public List<string> LcsNoSaveList { get; set; } = new List<string>()
        {
            "Area",
            "Owner",
            "Cable",
            "PowerCable",
            "AnalogCable",
            "ControlCable",
            "TypeModel",
            "TypeList",
            "SubTypeList",
            "HeatLoss",

            "IsSelected",

        };

        static public List<string> PowerCableNoSaveList { get; set; } = new List<string> {

            "Load",
            "InLineComponents",
            "TypeModel",
            "TypeList",
            "DeratedAmpsToolTip",
            "RequiredAmpsToolTip",
            "SizeList",
            "DeratingToolTip",
            "SizeTag",

            "IsSelected",
            "RacewayRouteSegments",
            "RacewaySegmentList",

            "AutoSizeCommand",
            "AutoSizeAllCommand",
            "SourceModel",
            "DestinationModel",
        };

        static public List<string> AreaNoSaveList { get; set; } = new List<string>() {
            "none",
            "ParentArea",
            "EquipmentList",
        };

        static public List<string> RacewayNoSaveList { get; set; } = new List<string>() {
            "none",
            "Cablelist",
            "CableList",
        };

        
    }
}
