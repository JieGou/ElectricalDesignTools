using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EDTLibrary.Models.Loads;

public interface IPowerCableUser : IEquipment, ICableUser
{

    double Voltage { get; set; }
    double Size { get; set; }
    string Unit { get; set; }
    double Fla { get; set; }
    string FedFromTag { get; set; }

    int FedFromId { get; set; }
    string FedFromType { get; set; }
    IDteq FedFrom { get; set; }

    double AmpacityFactor { get; set; }
    string PdType { get; set; }
    double PdSizeTrip { get; set; }
    double PdSizeFrame { get; set; }

    PowerCableModel PowerCable { get; set; }
    
    
    void CreatePowerCable();
    void SizePowerCable();
    void CalculateCableAmps();
}