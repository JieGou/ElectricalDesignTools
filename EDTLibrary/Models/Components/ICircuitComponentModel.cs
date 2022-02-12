using System.Collections.Generic;

namespace EDTLibrary.Models.Components
{
    public interface ICircuitComponentModel : IEquipment, IComponentModel, ComponentUser
    {
        string SequenceNumber { get; set; }

    }
}