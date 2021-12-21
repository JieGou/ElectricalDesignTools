using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary {
    public enum DteqTypes {
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
        Power,
        Control,
        Instrument,
        Communication
        
    }

    //TODO - voltage to types tables from Db
    public enum Voltages
    {
        SixHundred = 600,
        kW,
        AMPS,
        HP
    }
    public enum Units {
        HP,
        kW,
        kVA,        
        AMPS
        
    }

    public enum OCPDTypes
    {
        VCB,
        MCCB,
        FUSE, 
        MCP       
    }




}
