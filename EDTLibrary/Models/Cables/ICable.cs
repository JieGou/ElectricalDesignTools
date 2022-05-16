using EDTLibrary.LibraryData.TypeTables;
using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.Models.Cables
{
    public interface ICable
    {
        int Id { get; set; }
        int OwnedById { get; set; }
        string OwnedByType { get; set; }
        string Tag { get; set; }
        string Type { get; set; }
        CableTypeModel TypeModel { get; set; }
        string Category { get; set; }
        int QtyParallel { get; set; }
        double Length { get; set; }
        int ConductorQty { get; set; }
        string Size { get; set; }
        double VoltageClass { get; set; }
        string InstallationType { get; set; }
        bool Outdoor { get; set; }

        event EventHandler PropertyUpdated;
        abstract void OnPropertyUpdated();
    }
}
