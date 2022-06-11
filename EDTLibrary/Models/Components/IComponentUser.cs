using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.Components
{
    public interface IComponentUser : IEquipment
    {
        ObservableCollection<IComponent> AuxComponents { get; set; }
        ObservableCollection<IComponent> CctComponents { get; set; }

        bool DriveBool { get; set; }
        int DriveId { get; set; }
        bool DisconnectBool { get; set; }
        int DisconnectId { get; set; }

        bool LcsBool { get; set; }
        public LocalControlStationModel Lcs { get; set; }
        public IComponent Drive { get; set; }
        public IComponent Disconnect { get; set; }
    }
}