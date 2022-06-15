using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables
{
    public interface ICable
    {
        int Id { get; set; }
        int OwnerId { get; set; }
        string OwnerType { get; set; }
        string Tag { get; set; }
        string Type { get; set; }
        CableTypeModel TypeModel { get; set; }
        
        string UsageType { get; set; }
        string Category { get; set; }
        int QtyParallel { get; set; }
        double Length { get; set; }
        int ConductorQty { get; set; }
        string Size { get; set; }
        double VoltageClass { get; set; }
        string InstallationType { get; set; }
        bool Outdoor { get; set; }


        double BaseAmps { get; set; }
        double Spacing { get; set; }
        double Derating { get; set; }
        double DeratedAmps { get; set; }
        double RequiredAmps { get; set; }
        double RequiredSizingAmps { get; set; }
        string AmpacityTable { get; set; }
        string InstallationDiagram { get; set; }
        ICableUser Load { get; set; }

        bool SizeIsValid { get; set; }

        void CreateSizeList();

        event EventHandler PropertyUpdated;
        abstract Task OnPropertyUpdated();
    }
}
