using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.ProjectSettings;
public  class TagSettings
{

    #region MyRegion
    public static string AutoTagEquipment { get; set; }
    #endregion

    #region Equipment

    public static string EqNumberDigitCount { get; set; } = "3";
    public static string EqIdentifierSeparator { get; set; } = "-";
    public static string TransformerIdentifier { get; set; } = "XFR";
    public static string LvTransformerIdentifier { get; set; } = "TX";
    public static string SwgIdentifier { get; set; } = "SWG";
    public static string MccIdentifier { get; set; } = "MCC";
    public static string CdpIdentifier { get; set; } = "CDP";
    public static string DpnIdentifier { get; set; } = "LDP";
    public static string SplitterIdentifier { get; set; } = "SPL";

    //Loads
    public static string MotorLoadIdentifier { get; set; } = "MTR";
    public static string HeaterLoadIdentifier { get; set; } = "HTR";
    public static string WeldingLoadIdentifier { get; set; } = "WLD";
    public static string PanelLoadIdentifier { get; set; } = "PNL";
    public static string OtherLoadIdentifier { get; set; } = "OTH";


    #endregion  

    #region Cables

    public static string CableTagSeparator { get; set; } = "-";

    public static string PowerCableTypeIdentifier { get; set; } = "P";
    public static string ControlCableTypeIdentifier { get; set; } = "C";
    public static string InstrumentCableTypeIdentifier { get; set; } = "J";
    public static string EthernetCableTypeIdentifier { get; set; } = "E";
    public static string FiberCableTypeIdentifier { get; set; } = "F";

    #endregion

    #region Components
    public static string ComponentSuffixSeparator { get; set; } = ".";

    public static string DisconnectSuffix { get; set; } = "DCN";
    public static string DriveSuffix { get; set; } = "VFD";
    public static string LcsSuffix { get; set; } = "LCS";

    #endregion

}
