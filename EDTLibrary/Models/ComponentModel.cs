using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models {
    public class ComponentModel: IEquipmentModel {
        public ComponentModel() {
            Category = Categories.COMP.ToString();
        }
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }

        public int Voltage { get; set; }
        public int RatedAmps { get; set; }
        public string ComponentOf { get; set; }
        public double DesignAmps { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
