using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Cables;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.Components
{
    public interface IComponent : IEquipment
    {
        double Size { get; set; }
        string SubCategory { get; set; }
        string SubType { get; set; }
        ObservableCollection<string> TypeList { get; set; }


        int OwnerId { get; set; }
        string OwnerType { get; set; }
        IEquipment Owner { get; set; }
        int SequenceNumber { get; set; }

        ICable PowerCable { get; set; }

    }
}