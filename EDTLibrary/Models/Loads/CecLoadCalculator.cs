using EDTLibrary.LibraryData;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads;
public  class CecLoadCalculator
{
    public static void SetLoadPd(LoadModel load)
    {
        if (load.Type == LoadTypes.MOTOR.ToString()) {
            load.PdType = EdtSettings.LoadDefaultPdTypeLV_Motor;
        }
        else {
            load.PdType = EdtSettings.LoadDefaultPdTypeLV_NonMotor;
        }
    }

    public static void SetLoadPdSize(LoadModel load)
    {
        //TODO - enum for PdTypes
        if (load.PdType.Contains("MCP") ||
            load.PdType.Contains("FVNR") ||
            load.PdType.Contains("FVR")) {

            load.PdSizeFrame = DataTableManager.GetMcpFrame(load);
            load.PdSizeTrip = DataTableManager.GetBreakerTrip(load);
            load.StarterType = load.PdType;
            load.StarterSize = DataTableManager.GetStarterSize(load);
            //load.PdSizeTrip = Math.Min(load.Fla * 1.25, load.PdSizeFrame);
            //load.PdSizeTrip = Math.Round(load.PdSizeTrip, 0);
        }

        else if (load.PdType == "BKR" ||
                 load.PdType == "VFD" || load.PdType == "VSD" ||
                 load.PdType == "RVS") {
            load.PdSizeFrame = DataTableManager.GetBreakerFrame(load);
            load.PdSizeTrip = DataTableManager.GetBreakerTrip(load);
        }
    }

}
