using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Mappers;
public class DteqMapper
 
{
    public DteqMapper(DistributionEquipment dteq)
    {
        Id = dteq.Id;
        Tag = dteq.Tag;
        Category = dteq.Category;
        Type = dteq.Type;
        Description = dteq.Description;
        AreaTag = dteq.Area.Tag;
        AreaName = dteq.Area.Name;
        FedFrom = dteq.FedFrom.Tag;
        LineVoltage= dteq.LineVoltage;
        LoadVoltage = dteq.LoadVoltage;
        Size = dteq.Size;
        Unit = dteq.Unit;
        Fla = dteq.Fla;
        RunningAmps = dteq.RunningAmps;
        PercentLoaded = dteq.PercentLoaded;
        PowerFactor = dteq.PowerFactor;
        ConnectedKva = dteq.ConnectedKva;
        DemandKva = dteq.DemandKva;
        DemandKw = dteq.DemandKw;
        DemandKvar = dteq.DemandKvar;
        SCCR = dteq.SCCR;
        PdType = dteq.PdType;
        PdSizeTrip = dteq.PdSizeTrip;
        PdSizeFrame = dteq.PdSizeFrame;
        NemaRating = dteq.NemaRating;
        AreaClassification = dteq.AreaClassification;
    }

    public static List<string> PropertiesToIgnore { get; set; } = new List<string>
    {
               "PropertiesToIgnore",

               "Id",
               "AreaName",
               "FedFromType",
               "Category",
               "NemaRating",
               "AreaClassification",
               "AmpacityFactor",
               "LcsBool",
               "DriveBool",
               "DriveId",
               "DisconnectBool",
               "DisconnectId",
               "FedFromId",
               "NemaRating",
               "AreaClassification",
    };

    public int Id { get; set; }
    public string Tag { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string AreaTag { get; set; }
    public string AreaName { get; set; }
    public string FedFrom { get; set; }

    public double LineVoltage { get; set; }
    public double LoadVoltage { get; set; }
    
    public double Size { get; set; }
    public string Unit { get; set; }
    public double Fla { get; set; }

    public double RunningAmps { get; set; }
    public double PercentLoaded { get; set; }
    public double PowerFactor { get; set; }
    public double ConnectedKva { get; set; }
    public double DemandKva { get; set; }
    public double DemandKw { get; set; }
    public double DemandKvar { get; set; }

    public double SCCR { get; set; }

    public string PdType { get; set; }
    public double PdSizeTrip { get; set; }
    public double PdSizeFrame { get; set; }
  
    public string NemaRating { get; set; }
    public string AreaClassification { get; set; }
   

    

}
