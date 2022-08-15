using EDTLibrary.Models.Equipment;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.Components
{
    public interface IComponentUser : IEquipment
    {
        ObservableCollection<IComponentEdt> AuxComponents { get; set; }
        ObservableCollection<IComponentEdt> CctComponents { get; set; }

        bool DriveBool { get; set; }
        int DriveId { get; set; }
        bool DisconnectBool { get; set; }
        int DisconnectId { get; set; }

        bool LcsBool { get; set; }
        public ILocalControlStation Lcs { get; set; }
        public IComponentEdt Drive { get; set; }
        public IComponentEdt Disconnect { get; set; }
    }
}