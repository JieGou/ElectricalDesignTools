    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary {
    public enum Categories
    {
        DTEQ,
        LVDTEQ,
        LOAD,
        LOAD3P,
        LOAD1P,
        CABLE,
        CctComponent, //has sub categories
        ProtectionDevice, //has sub categories
        LCS,
        RACEWAY,
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
        PANEL,
        OTHER
    }
    public enum CctCompSubCategories
    {
        CctComponent,
        AuxComponent,
        ProtectionDevice,
        Starter,
        Disconnect,
    }
    public enum CctComponentTypes
    {
        Disconnect,
        UDS,
        FDS,
        VFD,
        VSD,
        RVS,
        LCS,
        DOL,

    }

    public enum PdTypes
    {
        Breaker,
        BKR,
        FDS,
        MCP_FVNR,
        MCP_FVR,
        DOL,
        StandAloneDrive,
        StandAloneStarter,
    }
    public enum StarterTypes
    {
        DOL,
        MCP_FVNR,
        MCP_FVR,
        VFD,
        VSD,
        RVS,
    }

    public enum CctComponentSubTypes
    {
        DefaultDcn,
        DefaultStarter,
        StandAloneDrive,
        StandAloneStarter,
    }

    

    
    public enum JunctionBoxTypes
    {
        JB,
        MVJB,
        HVJB,
        CSTE,
    }
   
    public enum DisconnectTypes
    {
        UDS,
        FDS,
        UWDS,
        FWDS,

    }
    public enum CableUsageTypes
    {
        Power,
        Control,
        Instrument,
        Communication,
        Ethernet,
        Fiber,
    }

    public enum RacewayTypes
    {
        LadderTray,
        Conduit,
        DuctBank,
    }
    public enum RacewayMaterials
    {
        Aluminum,
        HDG,
        PVC,
        RMC,
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
