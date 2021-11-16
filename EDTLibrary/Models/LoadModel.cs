using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models {
    public class LoadModel: ILoadModel {
        public LoadModel() {
            Category = Categories.LOAD.ToString();
            LoadFactor = 0.8;
        }


        //Fields
        private string _tag;
        //Properties

        #region EquipmentModel

        [System.ComponentModel.Browsable(false)] // make this property non-visisble by grids/databindings
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        #endregion

        #region ILoadModel - User Inputs Primary
        public int Voltage { get; set; }
        public double Size { get; set; }
        public string Unit { get; set; }
        public string FedFrom { get; set; }
        public double LoadFactor { get; set; }
        public List<ComponentModel> InLineComponents { get; set; } = new List<ComponentModel>();
        //public List<CableModel> Cables { get; set; } = new List<CableModel>();

        #endregion
        #region Lookups
        public double PowerFactor { get; set; }
        public double Efficiency { get; set; }
        #endregion

        #region ILoadModel - Privately Calculated Values
        public double Fla { get; set; }
        public double ConnectedKva { get; set; }
        public double DemandKva { get; set; }
        public double DemandKw { get; set; }
        public double DemandKvar { get; set; }
        public double RunningAmps { get; set; }

        public int CableQty { get; set; }
        public string CableSize { get; set; }
        #endregion

        #region LoadModel       
        public string PdStarter { get; set; }
        #endregion

       


        //Methods
        public void CalculateLoading() {
            if (Efficiency > 1)
                Efficiency = Efficiency / 100;
            if (PowerFactor > 1)
                PowerFactor = PowerFactor / 100;

            Efficiency = Math.Round(Efficiency, 2);
            PowerFactor = Math.Round(PowerFactor, 2);


            if (this.Type == "HEATER") {
                Unit = "kW";
                Efficiency = GlobalConfig.HeaterEff;
                PowerFactor = GlobalConfig.HeaterPf;
            }
            else if (this.Type == "TRANSFORMER") {
                Unit = "kVA";
                Efficiency = GlobalConfig.XfrEff;
                PowerFactor = GlobalConfig.XfrPf;
            }

            switch (this.Type) {
                case "MOTOR":
                    switch (this.Unit) {
                        case "HP":
                            ConnectedKva = Size * 0.746 / Efficiency / PowerFactor;
                            break;
                        case "kW":
                            ConnectedKva = Size / Efficiency / PowerFactor;
                            break;
                    }
                    break;
                case "TRANSFORMER":
                    ConnectedKva = this.Size;
                    break;
                case "HEATER":
                    ConnectedKva = this.Size / this.Efficiency / this.PowerFactor;
                    break;

                case "OTHER":

                    switch (this.Unit) {
                        case "kVA":
                            ConnectedKva = this.Size;
                            break;
                        case "kW":
                            ConnectedKva = Size / Efficiency / PowerFactor;
                            break;
                        case "AMPS":
                            ConnectedKva = Size * Voltage * Math.Sqrt(3); //   / Efficiency / PowerFactor;
                            Fla = Size;
                            break;
                    }
                    break;
            }

            //calculates FLA if the load unit is not FLA
            if (Unit != "A") {
                Fla = ConnectedKva * 1000 / Voltage / Math.Sqrt(3);
                Fla = Math.Round(Fla, GlobalConfig.SigFigs);
            }

            ConnectedKva = Math.Round(ConnectedKva, GlobalConfig.SigFigs);
            DemandKva = Math.Round(DemandKva, GlobalConfig.SigFigs);
            DemandKw = Math.Round(DemandKw, GlobalConfig.SigFigs);
            DemandKvar = Math.Round(DemandKvar, GlobalConfig.SigFigs);
        }
        public void LookupPfAndEff() {
            //------MOTOR LOADS
            if (this.Type == "MOTOR") {
                this.Efficiency = 0;
                this.PowerFactor = 0;
                try {
                    string motorKey = this.Unit + this.Voltage.ToString() + this.Size.ToString() + "1800";
                    
                }
                catch (Exception) {
                    //do nothing and leave value at 0 if motor doesn't exist
                }
            }

            //----NON-MOTOR LOADS
            else {
                ////gets default PF and Eff for other load types (Panel, Heater, Transformer, Welding, Others..)

                //foreach (var loadType in defaultLoadData) {
                //    if (loadType.Type == this.Type) {
                //        this.PowerFactor = loadType.DefaultPowerFactor;
                //        this.Efficiency = loadType.DefaultEfficiency;
                //    }
                //}

                //// linq query that gets list of records that have the same Size,Voltage,Unit as this load 
                //var returnedRecords = from record in defaultLoadData
                //                      where record.Type == this.Type
                //                      select record;

                //foreach (var item in returnedRecords) {
                //    this.PowerFactor = item.DefaultPowerFactor;
                //    this.Efficiency = item.DefaultEfficiency;
                //}
            }
        }
        public void SizeComponents() {
            //Adds default components based on type of load
            //  Ex: Combination start for Motor
            //size the component via lookups
            //this might be better in list manager
        }

    }
}
