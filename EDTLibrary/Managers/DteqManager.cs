using EDTLibrary.LibraryData;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Managers;
public class DteqManager
{
    public static void SetPd(DistributionEquipment dteq)
    {

        //dteq.PdType = EdtSettings.DteqDefaultPdTypeLV;
        dteq.PdSizeFrame = DataTableManager.GetBreakerFrame(dteq);
        dteq.PdSizeTrip = DataTableManager.GetBreakerTrip(dteq);

    }

}
