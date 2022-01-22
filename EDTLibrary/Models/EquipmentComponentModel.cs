using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models {
    public class EquipmenttComponentModel: IComponentModel {
        public EquipmenttComponentModel() {
            Category = Categories.COMP.ToString();
        }
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }

        public string Type { get; set; }
        public string SubType { get; set; }

        public string ComponentOf { get; set; }
        public List<IComponentModel> Components { get; set; } = new List<IComponentModel>();
    }
}
