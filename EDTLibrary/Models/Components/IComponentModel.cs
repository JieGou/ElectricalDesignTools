using System.Collections.Generic;

namespace EDTLibrary.Models.Components
{
    public interface IComponentModel : IEquipment, ComponentUser
    {
        string SubType { get; set; }
        string Location { get; set; }
        string ComponentOf { get; set; }
    }
}