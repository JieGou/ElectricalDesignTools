using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.Components
{
    public interface IComponentUser : IEquipment
    {
        ObservableCollection<IComponent> AuxComponents { get; set; }
        ObservableCollection<IComponent> CctComponents { get; set; }

    }
}