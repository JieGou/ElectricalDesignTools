using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models {
    public class LoadModel: ILoadModel {
        public LoadModel() {
            Category = Categories.LOAD3P.ToString();
            LoadFactor = 0.8;
        }


        //Fields
        private string _tag;
        //Properties

        #region EquipmentModel

        [System.ComponentModel.Browsable(false)] // make this property non-visisble by grids/databindings
        public int Id { get; set; }
        public string Tag { get; 
            set; }
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

        //Sizing
        public double MinCableAmps { get; set; }
        //public double PercentLoaded { get; set; }
        public string PdType { get; set; }
        public double PdSize { get; set; }

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

            //Non-Motor Eff & PF
            if (Type == LoadTypes.HEATER.ToString()) {
                Unit = Units.kW.ToString();
                Efficiency = GlobalConfig.HeaterEff;
                PowerFactor = GlobalConfig.HeaterPf;
            }
            else if (Type == LoadTypes.TRANSFORMER.ToString()) {
                Unit = Units.kW.ToString();
                Efficiency = GlobalConfig.XfrEff;
                PowerFactor = GlobalConfig.XfrPf;
            }

            //if (Type == LoadTypes.MOTOR.ToString()) {
            //    if (Unit == Units.HP.ToString()) {
            //        ConnectedKva = Size * 0.746 / Efficiency / PowerFactor;
            //    }
            //    else if (Unit == Units.kW.ToString()) {
            //        ConnectedKva = Size / Efficiency / PowerFactor;
            //    }
            //}
            //else if (Type == LoadTypes.TRANSFORMER.ToString()) {
            //    ConnectedKva = Size;
            //}
            //else if (Type == LoadTypes.HEATER.ToString()) {
            //    ConnectedKva = Size / Efficiency / PowerFactor;
            //}
            //else if (Type == LoadTypes.OTHER.ToString()) {
            //    switch (Unit) {
            //        case "kVA":
            //            ConnectedKva = Size;
            //            break;
            //        case "kW":
            //            ConnectedKva = Size / Efficiency / PowerFactor;
            //            break;
            //        case "AMPS":
            //            ConnectedKva = Size * Voltage * Math.Sqrt(3); //   / Efficiency / PowerFactor;
            //            Fla = Size;
            //            break;
            //    }
            //}

            switch (Type) {
            case "MOTOR":
                switch (Unit) {
                    case "HP":
                        ConnectedKva = Size * 0.746 / Efficiency / PowerFactor;
                        break;
                    case "kW":
                        ConnectedKva = Size / Efficiency / PowerFactor;
                        break;
                }
                break;

            case "TRANSFORMER":
                ConnectedKva = Size;
                break;

            case "HEATER":
                ConnectedKva = Size / Efficiency / PowerFactor;
                break;

            case "OTHER":
                switch (Unit) {
                    case "kVA":
                        ConnectedKva = Size;
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
            DemandKva = Math.Round(ConnectedKva*LoadFactor, GlobalConfig.SigFigs);
            DemandKw = Math.Round(DemandKva*PowerFactor, GlobalConfig.SigFigs);
            DemandKvar = Math.Round(DemandKva*(1-PowerFactor), GlobalConfig.SigFigs);
        }
        public void LookupPfAndEff() {
            //TODO - Implement motor Eff & Pf
            //------MOTOR LOADS
            if (Type == "MOTOR") {
                Efficiency = 0;
                PowerFactor = 0;

                try {
                    string motorKey = Unit + Voltage.ToString() + Size.ToString() + "1800";
                    
                }
                catch (Exception) {
                    //do nothing and leave value at 0 if motor doesn't exist
                }
            }
        }
        public void SizeComponents() {
            //TODO - create Components

            //Adds default components based on type of load
            //  Ex: Combination start for Motor
            //size the component via lookups
            //this might be better in list manager
        }

    }
}
