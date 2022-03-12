using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.Components
{
    public interface IComponentUser
    {
        ObservableCollection<IComponent> Components { get; set; }

    }
}