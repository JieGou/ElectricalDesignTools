using System.Collections.Generic;

namespace EDTLibrary.Models.Components
{
    public interface ICircuitComponentModel : IEquipment, IComponent, IComponentUser
    {
        string SequenceNumber { get; set; }

    }
}