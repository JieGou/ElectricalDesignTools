using EDTLibrary.LibraryData.TypeModels;
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
public class CableMapper 
{
    public CableMapper(CableModel cable)
    {
        Id = cable.Id;
        Tag = cable.Tag;
        Category = cable.Category;
        Source = cable.SourceModel.Tag;
        Destination = cable.DestinationModel.Tag;
        Type = cable.TypeModel.Type;
        UsageType = cable.UsageType;
        QtyParallel = cable.QtyParallel;
        ConductorQty = cable.ConductorQty;
        Size = "'" + cable.Size; //  the ' is for excel formatting
        VoltageClass = cable.VoltageRating;
        Length = cable.Length;
        Spacing = cable.Spacing;
        Derating = cable.Derating;
        RequiredAmps = cable.RequiredAmps;
        DeratedAmps = cable.DeratedAmps;
        BaseAmps = cable.BaseAmps;
        AmpacityTable = cable.AmpacityTable;
        InstallationType = cable.InstallationType;
        IsOutdoor = cable.IsOutdoor;
        InstallationDiagram = cable.InstallationDiagram;
        OwnerType = cable.OwnerType;
    }

    public int Id { get; set; }
    public string Tag { get; set; }
    public string Category { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public string Type { get; set; }
    public string UsageType { get; set; }
    public int QtyParallel { get; set; }
    public int ConductorQty { get; set; }
    public string Size { get; set; }
    public double VoltageClass { get; set; }
    public double Length { get; set; }

   
    public string InstallationType { get; set; }
    public bool IsOutdoor { get; set; }
    public double BaseAmps { get; set; }
    public double Spacing { get; set; }
    public double Derating { get; set; }
    public double DeratedAmps { get; set; }
    public double RequiredAmps { get; set; }
    public double RequiredSizingAmps { get; set; }
    public string AmpacityTable { get; set; }
    public string InstallationDiagram { get; set; }
    public string OwnerType { get; set; }

}
