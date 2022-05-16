using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.Components
{
    public interface IComponentUser : IEquipment
    {
        ObservableCollection<IComponent> Components { get; set; }

    }
}