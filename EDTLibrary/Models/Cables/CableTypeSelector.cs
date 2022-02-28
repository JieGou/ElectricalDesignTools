using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables
{
    public class CableTypeSelector
    {
        public static string GetCableType(IPowerConsumer load)
        {
            string output = "Undertermined";
            if (load is IDteq) {

            }
            //else if (load is ILoad) {
            //    return EdtSettings.DefaultCableType_3C1kV;
            //}

            return output;
        }
    }
}
