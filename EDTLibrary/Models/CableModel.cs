using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models
{
    public class CableModel
    {
        public CableModel() {
            Derating = 1;
            Conductors = 3;
        }

        [System.ComponentModel.Browsable(false)] // make this property non-visisble by grids/databindings
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Category { get; set; }
        public string UsageType { get; set; }
        public string Type { get; set; }
        public int Conductors { get; set; }
        public string Size { get; set; }
        public double DesignAmps { get; set; }
        public int RatedVoltage { get; set; }
        public double Derating { get; set; }
        public double RatedAmps { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        //public double Spacing { get; set; }

        //public List<string> Trays { get; set; } = new List<string>();

        public void CreateTag() {
            Tag = Source.Replace("-", "") + "-" + Destination.Replace("-", "");
        }

        public void CalculateLoading() {
            DistributionEquipmentModel dteq;
            dteq = ListManager.dteqList.FirstOrDefault(eq => eq.Tag == Source);
            if (dteq!=null) {
                Derating = dteq._derating;
            }
            ILoadModel load;
            load = ListManager.masterLoadList.FirstOrDefault(l => l.Tag == Destination);
            if (load!=null) {
                DesignAmps = load.Fla;
                if (load.Category==Categories.LOAD1P.ToString()) {
                    //TODO - algorithm to determine condutor count for 1Phase loads
                }
            }
            //TODO - figure out how to load and read DataTables
            //TODO - algorithm to find parallel runs & size
        }
    }
}
