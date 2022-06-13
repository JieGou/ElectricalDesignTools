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
public class LoadMapper
 
{
    public LoadMapper(LoadModel load)
    {
        Id = load.Id;
        Tag = load.Tag;
        Category = load.Category;
        Type = load.Type;
        Description = load.Description;
        AreaTag = load.Area.Tag;
        AreaName = load.Area.Name;
        FedFrom = load.FedFrom.Tag;
        Voltage = load.Voltage;
        Size = load.Size;
        Unit = load.Unit;
        Fla = load.Fla;
        RunningAmps = load.RunningAmps;
        LoadFactor = load.LoadFactor;
        Efficiency = load.Efficiency;
        PowerFactor = load.PowerFactor;
        ConnectedKva = load.ConnectedKva;
        DemandKva = load.DemandKva;
        DemandKw = load.DemandKw;
        DemandKvar = load.DemandKvar;
        PdType = load.PdType;
        PdSizeTrip = load.PdSizeTrip;
        PdSizeFrame = load.PdSizeFrame;
        NemaRating = load.NemaRating;
        AreaClassification = load.AreaClassification;
    }

    public static List<string> PropertiesToIgnore { get; set; } = new List<string>
    {
               "PropertiesToIgnore",

               "Id",
               "AreaName",
               "Category",
               "RunningAmps",
               "NemaRating",
               "AreaClassification",
               "AmpacityFactor",
               "LcsBool",
               "DriveBool",
               "DriveId",
               "DisconnectBool",
               "DisconnectId",
               "FedFromId",
               "FedFromType",
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

    public double Voltage { get; set; }
    public double Size { get; set; }
    public string Unit { get; set; }
    public double Fla { get; set; }
    public double RunningAmps { get; set; }

    public double LoadFactor { get; set; }
    public double Efficiency { get; set; }
    public double PowerFactor { get; set; }

    public double ConnectedKva { get; set; }
    public double DemandKva { get; set; }
    public double DemandKw { get; set; }
    public double DemandKvar { get; set; }
   
    public string PdType { get; set; }
    public double PdSizeTrip { get; set; }
    public double PdSizeFrame { get; set; }

    public string NemaRating { get; set; }
    public string AreaClassification { get; set; }
    
}
