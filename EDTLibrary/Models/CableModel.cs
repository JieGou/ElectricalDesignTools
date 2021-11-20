using System;
using System.Collections.Generic;
using System.Data;
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
        public int Insulation { get; set; }
        public double Derating { get; set; }
        public double RatedAmps { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        //public double Spacing { get; set; }

        //public List<string> Trays { get; set; } = new List<string>();

        public void CreateTag() {
            Tag = Source.Replace("-", "") + "-" + Destination.Replace("-", "");
        }
            
        // TODO - Move "CalculatePowerCableSize to ILoadModel
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

            
            //Conductors
            Conductors = 3;

            //UsageType
            UsageType = "Power";

            //Insulation
            Insulation = 100;
            int voltage = ListManager.iLoadDict[Destination].Voltage;
            //Type
            DataTable cableType = DataTables.CableTypes.Select($"VoltageClass >= {voltage}").CopyToDataTable();
            cableType = cableType.Select($"VoltageClass = MIN(VoltageClass) " +
                $"AND Conductors ={Conductors}" +
                $"AND Insulation ={Insulation}" +
                $"AND UsageType = '{UsageType}'").CopyToDataTable();

            Type = cableType.Rows[0]["Type"].ToString();

            RatedVoltage = Int32.Parse(cableType.Rows[0]["VoltageClass"].ToString());

            //Design Amps
            DesignAmps = ListManager.iLoadDict[Destination].Fla;
            if (ListManager.iLoadDict[Destination].Type == LoadTypes.MOTOR.ToString()
                            | ListManager.iLoadDict[Destination].Type == LoadTypes.TRANSFORMER.ToString()){
                DesignAmps = DesignAmps * 1.25;
            }

            //Temporary
            ElectricalSettings.Code = "CEC";
            double requiredAmps = DesignAmps / Derating;
            DataTable cableAmps = DataTables.CableAmpacities.Select($"Code = '{ElectricalSettings.Code}' " +
                $"AND Amps75 >= {requiredAmps}").CopyToDataTable();
            cableAmps = cableAmps.Select($"Amps75 = MIN(Amps75)").CopyToDataTable();

            RatedAmps = Double.Parse(cableAmps.Rows[0]["Amps75"].ToString());
            Size = cableAmps.Rows[0]["Size"].ToString();

            //TODO - algorithm to find parallel runs & size
        }
    }
}
