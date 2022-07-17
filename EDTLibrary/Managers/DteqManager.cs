using EDTLibrary.LibraryData;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.DistributionEquipment;
public class DteqManager
{
    public static void SetPd(DistributionEquipment dteq)
    {

        //dteq.PdType = EdtSettings.DteqDefaultPdTypeLV;
        dteq.PdSizeFrame = DataTableManager.GetBreakerFrame(dteq);
        dteq.PdSizeTrip = DataTableManager.GetBreakerTrip(dteq);

    }

}
