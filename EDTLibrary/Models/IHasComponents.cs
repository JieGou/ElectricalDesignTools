using System.Collections.Generic;

namespace EDTLibrary.Models
{
    public interface IHasComponents
    {
        List<IComponentModel> Components { get; set; }

    }
}