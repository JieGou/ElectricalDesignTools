using EDTLibrary.Models.Areas;
using System;

namespace EDTLibrary.Models
{
    public interface IEquipment
    {
        int Id { get; set; }
        string Tag { get; set; }
        string Category { get; set; }
        string Type { get; set; }
        string Description { get; set; }
        int AreaId { get; set; }
        IArea Area { get; set; }
        string NemaRating { get; set; }
        string AreaClassification { get; set; }

        abstract void UpdateAreaProperties();
        public void OnAreaPropertiesChanged(object source, EventArgs e)
        {
            UpdateAreaProperties();
        }

    }
}