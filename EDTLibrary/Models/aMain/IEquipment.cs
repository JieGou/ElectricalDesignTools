using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Components;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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




        event EventHandler PropertyUpdated;
        abstract Task OnPropertyUpdated();

        abstract Task UpdateAreaProperties();
        public void OnAreaPropertiesChanged(object source, EventArgs e)
        {
            UpdateAreaProperties();
        }

        event EventHandler AreaChanged;
        abstract Task OnAreaChanged();
        void MatchOwnerArea(object source, EventArgs e);
    }
}