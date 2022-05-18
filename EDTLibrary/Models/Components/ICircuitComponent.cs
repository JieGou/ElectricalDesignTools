using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Loads;
using System.Collections.Generic;

namespace EDTLibrary.Models.Components
{
    public interface ICircuitComponent : IComponent
    {
        int SequenceNumber { get; set; }

    }
}