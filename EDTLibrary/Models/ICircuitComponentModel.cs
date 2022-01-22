using System.Collections.Generic;

namespace EDTLibrary.Models
{
    public interface ICircuitComponentModel: IEquipmentModel, IComponentModel, IHasComponents
    {
        string Source { get; set; }
        string Destination { get; set; }
        string SequenceNumber { get; set; }

    }
}