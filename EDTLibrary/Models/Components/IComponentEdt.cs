using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.Components
{
    public interface IComponentEdt :  IPowerCableUser
    {
        double Size { get; set; }
        string Unit { get; set; }
        double PdTrip { get; set; }
        double PdFrame { get; set; }
        string SubCategory { get; set; }
        string SubType { get; set; }
        List<string> TypeList { get; set; }


        int OwnerId { get; set; }
        string OwnerType { get; set; }
        IEquipment Owner { get; set; }
        int SequenceNumber { get; set; }

        CableModel PowerCable { get; set; }

    }
}