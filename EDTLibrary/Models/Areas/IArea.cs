using System;

namespace EDTLibrary.Models.Areas;

public interface IArea

{
    IArea Area { get; set; }
    string AreaCategory { get; set; }
    string AreaClassification { get; set; }
    int AreaId { get; set; }
    string Description { get; set; }
    int Id { get; set; }
    double MaxTemp { get; set; }
    double MinTemp { get; set; }
    string Name { get; set; }
    string NemaRating { get; set; }
    string Tag { get; set; }

    //Events
    event EventHandler AreaPropertiesChanged;
}