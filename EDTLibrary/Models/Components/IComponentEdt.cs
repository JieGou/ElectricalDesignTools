using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.Components
{
    public interface IComponentEdt :  IEquipment
    {

        public int PropertyModelId { get; set; }
        public PropertyModelBase PropertyModel { get; set; }
        bool SettingTag { get; set; }


        double SCCA { get; set; }
        double SCCR { get; set; }
        double AIC { get; set; }


        double FrameAmps { get; set; }
        double TripAmps { get; set; }
        string StarterSize { get; set; }

        string SubCategory { get; set; }
        string SubType { get; set; }
        List<string> TypeList { get; }


        int OwnerId { get; set; }
        string OwnerType { get; set; }
        IEquipment Owner { get; set; }
        int SequenceNumber { get; set; }

        CableModel PowerCable { get; set; }

        void CalculateSize(IPowerConsumer load);

    }
}