using System.Collections.Generic;

namespace EDTLibrary.Models
{
    public interface ICircuitComponentModel: IEquipmentModel, IComponentModel, IHasComponents
    {
        string SequenceNumber { get; set; }

    }
}