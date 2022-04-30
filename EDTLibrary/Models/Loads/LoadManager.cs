using EDTLibrary.LibraryData;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads;
public class LoadManager
{
    public static void SetLoadPd(LoadModel load)
    {
        if (load.Type == LoadTypes.MOTOR.ToString()) {
            load.PdType = EdtSettings.LoadDefaultPdTypeLV_Motor;
            load.PdSizeFrame = LibraryManager.GetMcpFrame(load);
            load.PdSizeTrip = Math.Min(load.Fla * 1.25, load.PdSizeFrame);
            load.PdSizeTrip = Math.Round(load.PdSizeTrip, 0);


        }
        else {
            load.PdType = EdtSettings.LoadDefaultPdTypeLV_NonMotor;
            load.PdSizeFrame = LibraryManager.GetBreakerFrame(load);
            load.PdSizeTrip = LibraryManager.GetBreakerTrip(load);

        }
    }

}
