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


    //Components
    public static string DisconnectSuffix { get; set; } = "DCN";
    public static string DriveSuffix { get; set; } = "VFD";
    public static string LcsSuffix { get; set; } = "LCS";

}
