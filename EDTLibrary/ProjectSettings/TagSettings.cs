using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.ProjectSettings;
public  class TagSettings
{
    
    public static string CableTagSeparator { get; set; } = "-";
    public static string SuffixSeparator { get; set; } = ".";

    public static string PowerCableTagTypeIdentifier { get; set; } = "P";
    public static string ControlCableTagTypeIdentifier { get; set; } = "C";
    public static string InstrumentCableTagTypeIdentifier { get; set; } = "J";
    public static string EthernetCableTagTypeIdentifier { get; set; } = "E";
    public static string FiberCableTagTypeIdentifier { get; set; } = "F";


    //Components
    public static string DisconnectSuffix { get; set; } = "DCN";
    public static string DriveSuffix { get; set; } = "VFD";
    public static string LcsSuffix { get; set; } = "LCS";

}
