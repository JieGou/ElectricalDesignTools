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
        }

        [System.ComponentModel.Browsable(false)] // make this property non-visisble by grids/databindings
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public int Conductors { get; set; }
        public string Size { get; set; }
        public double DesignAmps { get; set; }
        public int RatedVoltage { get; set; }
        public double Derating { get; set; }
        public double RatedAmps { get; set; }
        public string Conn1 { get; set; }
        public string Conn2 { get; set; }
        //public double Spacing { get; set; }

        //public List<string> Trays { get; set; } = new List<string>();

        public void CreateCableTag() {
            Tag = Conn1.Replace("-", "") + "-" + Conn2.Replace("-", "");
        }

        public void CalculateLoading() {
            //TODO - algorithm to find derating

            //TODO - algorithm to find parallel runs & size
        }

    }
}
