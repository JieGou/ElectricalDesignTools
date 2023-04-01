using EdtLibrary.LibraryData.TypeModels;

namespace EDTLibrary.Models.DistributionEquipment.DPanels
{
    /// <summary>
    /// Used to create the circuit number graphics on the panel view
    /// </summary>
    public class DpnCircuit
    {
        public int Id { get; set; }
        public VoltageType VoltageType { get; set; }
        public int CircuitNumber { get; set; }
    }

}
