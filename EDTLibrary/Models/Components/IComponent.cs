using EDTLibrary.Models.Cables;
using System.Collections.Generic;

namespace EDTLibrary.Models.Components
{
    public interface IComponent : IEquipment//, ICableUser
    {
        string SubCategory { get; set; }
        string SubType { get; set; }
        int OwnerId { get; set; }
        string OwnerType { get; set; }
        IEquipment Owner { get; set; }
        int SequenceNumber { get; set; }

        ICable PowerCable { get; set; }

    }
}