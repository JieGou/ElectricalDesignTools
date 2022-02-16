using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.Components
{
    public interface ComponentUser
    {
        ObservableCollection<IComponentModel> Components { get; set; }

    }
}