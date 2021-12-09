using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models {
    class TransformerModel: ILoadModel {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public double Voltage { get; set; }
        public double LineVoltage { get; set; }
        public double LoadVoltage { get; set; }
        public double Size { get; set; }
        public string Unit { get; set; }
        public double LoadFactor { get; set; }
        public string FedFrom { get; set; }
        public double PowerFactor { get; set; }
        public double Efficiency { get; set; }
        public double ConnectedKva { get; set; }
        public double Fla { get; set; }
        public double DemandKva { get; set; }
        public double DemandKw { get; set; }
        public double DemandKvar { get; set; }
        public double RunningAmps { get; set; }
        public int CableQty { get; set; }
        public string CableSize { get; set; }

        [DisplayName("OCPD\nType")]
        public string PdType { get; set; }

        [DisplayName("Trip\nAmps")]
        public double PdSizeTrip { get; set; }
        [DisplayName("Frame\nAmps")]
        public double PdSizeFrame { get; set; }
        public List<ComponentModel> InLineComponents { get; set; }
        public List<CableModel> Cables { get; set; }

        public void CalculateLoading() {

        }
    }
}
