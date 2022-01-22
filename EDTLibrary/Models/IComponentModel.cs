using System.Collections.Generic;

namespace EDTLibrary.Models
{
    public interface IComponentModel: IEquipmentModel, IHasComponents
    {
        string SubType { get; set; }
        string Location { get; set; }
        string ComponentOf { get; set; }
    }
}