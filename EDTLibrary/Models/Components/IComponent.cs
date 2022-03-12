using System.Collections.Generic;

namespace EDTLibrary.Models.Components
{
    public interface IComponent : IEquipment, IComponentUser
    {
        string SubType { get; set; }
        
        string ComponentOf { get; set; }
    }
}