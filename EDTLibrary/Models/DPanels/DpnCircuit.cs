namespace EDTLibrary.Models.DistributionEquipment.DPanels
{
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
