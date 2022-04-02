using EDTLibrary.Models.Areas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components
{
    public class CctComponentModel : IComponent
    {
        public CctComponentModel()
        {
            Category = Categories.COMP.ToString();
        }
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Owner { get; set; }
        public ObservableCollection<IComponent> Components { get; set; } = new ObservableCollection<IComponent>();

        public int AreaId { get; set; }
        public IArea Area { get; set; }
        public string NemaRating { get; set; }
        public string AreaClassification { get; set; }

        public void UpdateAreaProperties()
        {
            throw new NotImplementedException();
        }
    }
}
