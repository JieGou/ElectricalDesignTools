using System;
using System.ComponentModel;

namespace EDTLibrary.Models.Areas;

public interface IArea: INotifyPropertyChanged
{
    IArea ParentArea { get; set; }
    string AreaCategory { get; set; }
    string AreaClassification { get; set; }
    int ParentAreaId { get; set; }
    string Description { get; set; }
    int Id { get; set; }
    double MaxTemp { get; set; }
    double MinTemp { get; set; }
    string Name { get; set; }
    string NemaRating { get; set; }
    string Tag { get; set; }
    string DisplayTag { get; set; }

    //Events
    event EventHandler AreaPropertiesChanged;
}