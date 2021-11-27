using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LM = EDTLibrary.ListManager;

namespace EDTLibrary.Models
{
    public class CableModel
    {
        public CableModel() {
            QtyParallel = 1;
            Derating = 1;
            Conductors = 3;
        }

        [System.ComponentModel.Browsable(false)] // make this property non-visisble by grids/databindings
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Category { get; set; }
        public string UsageType { get; set; }
        public string Type { get; set; }
        public int QtyParallel { get; set; }
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
            if (LM.dteqDict.ContainsKey(Source)) {
                Derating = ListManager.dteqDict[Source]._derating;
            }
            if (LM.iLoadDict.ContainsKey(Destination)) {
                DesignAmps = ListManager.iLoadDict[Destination].Fla;
            }
            Conductors = 3;
            UsageType = "Power";
            Insulation = 100;

            //DesignAmps
            ILoadModel load;
            load = ListManager.iLoadDict[Destination];

            if (load!=null) {
                DesignAmps = load.Fla;
                if (load.Category==Categories.LOAD1P.ToString()) {
                    //TODO - algorithm to determine condutor count for 1Phase loads
                }
                if (load.Type == LoadTypes.MOTOR.ToString() | load.Type == LoadTypes.TRANSFORMER.ToString()) {
                    DesignAmps *= 1.25;
                }
                if (load.Category== Categories.LOAD3P.ToString()) {
                    Conductors = 3;
                    UsageType = "Power";
                }
            }

            //Type
            int _voltage = ListManager.iLoadDict[Destination].Voltage;

            DataTable cableType = DataTables.CableTypes.Select($"VoltageClass >= {_voltage}").CopyToDataTable();
            cableType = cableType.Select($"VoltageClass = MIN(VoltageClass) " +
                $"AND Conductors ={Conductors}" +
                $"AND Insulation ={Insulation}" +
                $"AND UsageType = '{UsageType}'").CopyToDataTable();

            Type = cableType.Rows[0]["Type"].ToString();

            RatedVoltage = Int32.Parse(cableType.Rows[0]["VoltageClass"].ToString());



            //Size

            //Temporary
            //TODO - determine which table/Rule is used
            string codeTable = "Table2";
            double requiredAmps = DesignAmps / Derating;
            int qtyParallel = 0;

            //TODO - figure out how to filter by cablesUsedInProject. Join on CableXTable and Cables in project
            DataTable cablesInProject = ProjectSettings.CableSizesUsedInProject.Select($"UsedInProject = 'true'").CopyToDataTable();
            
            DataTable cableAmps = ProjectSettings.CableAmpsUsedInProject;

            //select by cable criteria            
            
            cableAmps = cableAmps.Select(
            $"Code = '{ProjectSettings.Code}' " +
            $"AND Amps75 >= {requiredAmps} " +
            $"AND CodeTable = '{codeTable}'").CopyToDataTable();

            //select smallest of 
            cableAmps = cableAmps.Select($"Amps75 = MIN(Amps75)").CopyToDataTable();
                        
            RatedAmps = Double.Parse(cableAmps.Rows[0]["Amps75"].ToString());
            Size = cableAmps.Rows[0]["Size"].ToString();

            //TODO - algorithm to find parallel runs & size
        }
    }
}
