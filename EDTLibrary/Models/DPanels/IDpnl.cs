using EDTLibrary.Models.Loads;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.DistributionEquipment.DPanels;
public interface IDpnl: IDteq
{
    int CircuitCount { get; set; }
    ObservableCollection<DpnCircuit> CircuitNumbersLeft { get; }
    ObservableCollection<DpnCircuit> CircuitNumbersRight { get; }
    ObservableCollection<DpnCircuit> DpnCircuitList { get; }
    ObservableCollection<IPowerConsumer> LeftCircuits { get; set; }
    int PoleCountLeft { get; set; }
    int PoleCountRight { get; set; }
    ObservableCollection<IPowerConsumer> RightCircuits { get; set; }

    bool AddAssignedLoad(IPowerConsumer load);
    ObservableCollection<IPowerConsumer> SetCircuits();
    ObservableCollection<IPowerConsumer> SetLeftCircuits();
    ObservableCollection<IPowerConsumer> SetRightCircuits();
}