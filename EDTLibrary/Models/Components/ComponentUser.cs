using System.Collections.Generic;

namespace EDTLibrary.Models.Components
{
    public interface ComponentUser
    {
        List<IComponentModel> Components { get; set; }

    }
}