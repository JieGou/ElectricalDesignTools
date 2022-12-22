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
        DRIVE,
        DCN,
        LCLDCN,
        LCS,
        RACEWAY,
    }

    public enum SubCategories
    {
        CctComponent,
        AuxComponent,
        ProtectionDevice,
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
        UDS,
        FDS,
        VFD,
        VSD,
        RVS,
        LCS,
        CircuitComponent,
        ProtectionDevice,
    }
    public enum CctComponentTypes
    {
        Dicsonnect,
        Starter,
        Drive,
        JunctionBox,

        DOL,
        UDS,
        FDS,
        VFD,
        VSD,
        RVS,
        JB,
    }

    public enum CctComponentSubTypes
    {
        DefaultDcn,
        DefaultDrive,
        DefaultStarter,
    }

    public enum PdTypes
    {
        BKR,
        Breaker,
        FDS,
        MCP_FVNR,
        MCP_FVR,
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
