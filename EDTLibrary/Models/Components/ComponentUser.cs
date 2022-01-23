using System.Collections.Generic;

namespace EDTLibrary.Models
{
    public interface ComponentUser
    {
        List<IComponentModel> Components { get; set; }

    }
}