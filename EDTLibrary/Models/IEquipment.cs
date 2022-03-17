using EDTLibrary.Models.Areas;

namespace EDTLibrary.Models
{
    public interface IEquipment
    {
        int Id { get; set; }
        string Tag { get; set; }
        string Category { get; set; }
        string Type { get; set; }
        string Description { get; set; }
        public int AreaId { get; set; }
        AreaModel Area { get; set; }
    }
}