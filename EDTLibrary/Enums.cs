using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary {
    public enum Categories
    {
        DTEQ,
        LOAD3P,
        LOAD1P,
        Component,
        CctComponent,
        AuxComponent,
        OCPD,
        CABLE,
        LCS,
    }
    
    public enum DteqTypes {
        XFR,
        SWG,
        MCC,
        CDP,
        DPN,
        SPL,
    }

    public enum LoadTypes {
        MOTOR,
        HEATER,
        WELDING,
        TRANSFORMER,
        PANEL,
        OTHER
    }

    public enum ComponentTypes
    {
        CCT,
        AUX,
        UDS,
        FDS,
        DefaultDcn,
        VFD,
        RVS,
        DefaultDrive,
        LCS,
        BREAKER,
        FUSE,
    }
    public enum ComponentSubTypes
    {
        UDS,
        FDS,
        DefaultDcn,
        VFD,
        RVS,
        DefaultDrive,
        BREAKER,
        FUSE,
    }

    public enum CableUsageTypes {
        Power,
        Control,
        Instrument,
        Communication,
        
    }

    public enum Units {
        A,
        HP,
        kW,
        kVA
    }

    public enum OCPDTypes
    {
        VCB,
        MCCB,
        FUSE, 
        MCP       
    }

    public enum AreaClassifications
    {
        UnClassified,
        NonHazardous,
        Zone0,
        Zone1,
        Zone2,
        Zone20,
        Zone21,
        Zone22,
        
    }

    public enum AreaCategories
    {
        Category1,
        Category2,
    }

}
