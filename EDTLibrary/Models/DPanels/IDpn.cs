using EDTLibrary.Models.Loads;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.DistributionEquipment.DPanels;
public interface IDpn: IDteq
{
    int CircuitCount { get; set; }
    ObservableCollection<DpnCircuit> CircuitNumbersLeft { get; }
    ObservableCollection<DpnCircuit> CircuitNumbersRight { get; }
    ObservableCollection<DpnCircuit> CircuitList { get; }
    ObservableCollection<LoadCircuit> AssignedCircuits { get; set; }
    ObservableCollection<IPowerConsumer> LeftCircuits { get; set; }
    int PoleCountLeft { get; set; }
    int PoleCountRight { get; set; }
    ObservableCollection<IPowerConsumer> RightCircuits { get; set; }

    bool AddAssignedLoad(IPowerConsumer load);
    ObservableCollection<IPowerConsumer> SetCircuits();
    ObservableCollection<IPowerConsumer> SetLeftCircuits();
    ObservableCollection<IPowerConsumer> SetRightCircuits();
}