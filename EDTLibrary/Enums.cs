using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary {
    public enum DistributionEquipment {
        XFR,
        SWG,
        MCC,
        DPN,
        SPL,
    }

    public enum Categories {
        DIST,
        LOAD3P,
        LOAD1P,
        COMP,
        CABLE,
    }

    public enum LoadTypes {
        MOTOR,
        HEATER,
        WELDING,
        TRANSFORMER,
        OTHER,
    }

    public enum CableUsageTypes {
        Power
    }


}
