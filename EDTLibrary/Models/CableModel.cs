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
        public double DeratedAmps { get; set; }
        public double Derating { get; set; }
        public double RatedAmps { get; set; }
        public int RatedVoltage { get; set; }
        public int Insulation { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        //public double Spacing { get; set; }

        //Fields
        double _voltage;
        double _requiredAmps;
        string _codeTable;



        public void CreateTag() {
            Tag = Source.Replace("-", "") + "-" + Destination.Replace("-", "");
        }
        
        public void GetCableParameters() {
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
            //TODO - coordinate DesignAmps vs RequiredAmps
            ILoadModel load;
            load = ListManager.iLoadDict[Destination];

            if (load != null) {
                DesignAmps = load.Fla;
                if (load.Category == Categories.LOAD1P.ToString()) {
                    //TODO - algorithm to determine condutor count for 1Phase loads
                }
                if (load.Type == LoadTypes.MOTOR.ToString() | load.Type == LoadTypes.TRANSFORMER.ToString()) {
                    DesignAmps *= 1.25;
                }
                if (load.Category == Categories.LOAD3P.ToString()) {
                    Conductors = 3;
                    UsageType = "Power";
                }
            }

            _voltage = ListManager.iLoadDict[Destination].Voltage;
            _requiredAmps = DesignAmps / Derating;
            _codeTable = "Table2";


            DataTable cableType = DataTables.CableTypes.Select($"VoltageClass >= {_voltage}").CopyToDataTable();
            cableType = cableType.Select($"VoltageClass = MIN(VoltageClass) " +
                $"AND Conductors ={Conductors}" +
                $"AND Insulation ={Insulation}" +
                $"AND UsageType = '{UsageType}'").CopyToDataTable();

            Type = cableType.Rows[0]["Type"].ToString();

            RatedVoltage = Int32.Parse(cableType.Rows[0]["VoltageClass"].ToString());

        }

        // TODO - Move "CalculatePowerCableSize to ILoadModel
        public void CalculateQtySize() {

            DataTable cableAmps = StringSettings.CableAmpsUsedInProject.Copy();

            //filter cables larger than RequiredAmps          
            var cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == StringSettings.Code
                                                          && x.Field<double>("Amps75") >= _requiredAmps
                                                          && x.Field<string>("CodeTable") == _codeTable);

            GetCableQty(QtyParallel);

            void GetCableQty(int cableQty) {
                if (cables.Any()) {
                    cableAmps = null;
                    cableAmps = cables.CopyToDataTable();                    

                    //select smallest of 
                    cableAmps = cableAmps.Select($"Amps75 = MIN(Amps75)").CopyToDataTable();

                    RatedAmps = Double.Parse(cableAmps.Rows[0]["Amps75"].ToString());
                    DeratedAmps = RatedAmps * Derating;                    
                    QtyParallel = cableQty;
                    Size = cableAmps.Rows[0]["Size"].ToString();
                }
                else {
                    QtyParallel += 1;
                    cableAmps = StringSettings.CableAmpsUsedInProject.Copy();
                    foreach (DataRow row in cableAmps.Rows) {
                        double amps75 = (double)row["Amps75"];
                        string size = row["Size"].ToString();
                        amps75 *= QtyParallel;
                        row["Amps75"] = amps75;
                        cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == StringSettings.Code
                                                          && x.Field<double>("Amps75") >= _requiredAmps
                                                          && x.Field<string>("CodeTable") == _codeTable);
                    }
                    GetCableQty(QtyParallel);
                }
            }
        }

        public void CalculateAmpacity() {
            GetCableParameters();

            DataTable cableAmps = StringSettings.CableAmpsUsedInProject.Copy();

            //filter cables larger than RequiredAmps          
            var cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == StringSettings.Code
                                                          && x.Field<string>("CodeTable") == _codeTable
                                                          && x.Field<string>("Size") == Size);

            cableAmps = null;
            cableAmps = cables.CopyToDataTable();
            //select smallest of 
            RatedAmps = Double.Parse(cableAmps.Rows[0]["Amps75"].ToString()) * QtyParallel;
            DeratedAmps = RatedAmps * Derating;
            DeratedAmps = Math.Round(DeratedAmps, GlobalConfig.SigFigs);
        }
    }
}
