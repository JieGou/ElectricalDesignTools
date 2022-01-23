using System.Collections.Generic;

namespace EDTLibrary.Models
{
    public interface ICircuitComponentModel: IEquipment, IComponentModel, ComponentUser
    {
        string SequenceNumber { get; set; }

    }
}