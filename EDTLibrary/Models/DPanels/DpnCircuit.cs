namespace EDTLibrary.Models.DistributionEquipment.DPanels
{
    /// <summary>
    /// Used to create the circuit number graphics on the panel view
    /// </summary>
    public class DpnCircuit
    {
        public int Id { get; set; }
        public int DpnId { get; set; }
        public string DpnType { get; set; }
        public int LoadId { get; set; }
        public string LoadType { get; set; }
        public int CircuitNumber { get; set; }
        public bool IsSpare { get; set; }
        public int PolesQty { get; set; }
    }

}
