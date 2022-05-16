using EDTLibrary.LibraryData.TypeTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables;
internal class ControlCable : ICable
{
    public int Id { get; set; }
    public int OwnedById { get; set; }
    public string OwnedByType { get; set; }
    public string Tag { get; set; }
    public string Type { get; set; }
    public CableTypeModel TypeModel { get; set; }
    public string Category { get; set; }
    public int QtyParallel { get; set; }
    public double Length { get; set; }
    public int ConductorQty { get; set; }
    public string Size { get; set; }
    public double VoltageClass { get; set; }
    public string InstallationType { get; set; }
    public bool Outdoor { get; set; }

    public event EventHandler PropertyUpdated;

    public void OnPropertyUpdated()
    {
        if (PropertyUpdated != null) {
            PropertyUpdated(this, EventArgs.Empty);
        }
    }



}
