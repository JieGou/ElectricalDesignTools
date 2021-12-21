using EDTLibrary.LibraryData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models {
    public class LoadModel : INotifyPropertyChanged, ILoadModel {
        public LoadModel() {
            Category = Categories.LOAD3P.ToString();
            LoadFactor = 0.8;
            PdType = ProjectSettings.EdtSettings.LoadDefaultPdTypeLV;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e) {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) {
                handler(this, e);
            }

            //if (e.PropertyName=="Tag") {
            //    //TODO reactivate this to update tags
            //    ListManager.CreateMasterLoadList();
            //    ListManager.tagList.Clear();
            //    foreach (var item in ListManager.loadList) {
            //       // ListManager.tagList.Add(item.Tag);
            //    }
            //    foreach (var item in ListManager.dteqList) {
            //        ListManager.tagList.Add(item.Tag);
            //    }
            //}
        }

        //Properties

        #region EquipmentModel

        [Browsable(false)] // make this property non-visisble by grids/databindings
        public int Id { get; set; }


        private string _tag;
        public string Tag { 
            get { return _tag; }
            set {
                //if (value == _tag || String.IsNullOrEmpty(value) || ListManager.IsTagAvailable(value)==false ) return;
                _tag = value; 
                OnPropertyChanged(new PropertyChangedEventArgs("Tag")); 
            }
        }

        public string Category { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        #endregion

        #region ILoadModel - User Inputs Primary
        public double Voltage { get; set; }
        public double Size { get; set; }
        public string Unit { get; set; }

        [DisplayName("Fed\nFrom")]
        public string FedFrom { get; set; }

        [DisplayName("Load\nFactor")]
        public double LoadFactor { get; set; }
        [Browsable(false)]
        public List<ComponentModel> InLineComponents { get; set; } = new List<ComponentModel>();
        //public List<CableModel> Cables { get; set; } = new List<CableModel>();

        #endregion
        #region Lookups

        [DisplayName("PF")]
        public double PowerFactor { get; set; }

        [DisplayName("Eff")]
        public double Efficiency { get; set; }
        #endregion

        #region ILoadModel - Privately Calculated Values
        public double Fla { get; set; }

        [DisplayName("Conn\nkVa")]
        public double ConnectedKva { get; set; }

        [DisplayName("Dem\nkVA")]
        public double DemandKva { get; set; }

        [DisplayName("Dem\nkW")]
        public double DemandKw { get; set; }

        [DisplayName("Dem\nkVAR")]
        public double DemandKvar { get; set; }

        [DisplayName("Running\nAmps")]
        public double RunningAmps { get; set; }

        //Sizing
        [DisplayName("Min Cable Amps")]
        public double MinCableAmps { get; set; }
        //public double PercentLoaded { get; set; }

        [Browsable(false)] // make this property non-visisble by grids/databindings
        public double PdFactor { get; set; }

        [DisplayName("OCPD\nType")]
        public string PdType { get; set; }

        [DisplayName("Trip\nAmps")]
        public double PdSizeTrip { get; set; }
        [DisplayName("Frame\nAmps")]
        public double PdSizeFrame { get; set; }


        [DisplayName("Cable\nQty")]
        public int CableQty { get; set; }
        [DisplayName("Cable\nSize")]
        public string CableSize { get; set; }
        #endregion

        #region LoadModel       
        public string PdStarter { get; set; }

        
        #endregion


        //Methods
        public void CalculateLoading() {
            

        //PowerFactor and Efficiency
            if (Type == LoadTypes.HEATER.ToString()) {
                Unit = Units.kW.ToString();
                Efficiency = GlobalConfig.DefaultHeaterEfficiency;
                PowerFactor = GlobalConfig.DefaultHeaterPowerFactor;
            }
            else if (Type == LoadTypes.TRANSFORMER.ToString()) {
                Unit = Units.kW.ToString();
                Efficiency = GlobalConfig.DefaultTransformerEfficiency;
                PowerFactor = GlobalConfig.DefaultTransformerPowerFactor;
            }
            else if (Type == LoadTypes.MOTOR.ToString()) {
                Efficiency = LibraryManager.GetMotorEfficiency(this);
                PowerFactor = LibraryManager.GetMotorPowerFactor(this);
            }

            if (Efficiency > 1)
                Efficiency /= 100;
            if (PowerFactor > 1)
                PowerFactor /= 100;

            Efficiency = Math.Round(Efficiency, 2);
            PowerFactor = Math.Round(PowerFactor, 2);

            PdFactor = 1;

            //if (Type == LoadTypes.MOTOR.ToString()) {
            //    PdFactor = 1.25
            //    if (Unit == Units.HP.ToString()) {
            //        ConnectedKva = Size * 0.746 / Efficiency / PowerFactor;
            //    }
            //    else if (Unit == Units.kW.ToString()) {
            //        ConnectedKva = Size / Efficiency / PowerFactor;
            //    }
            //}
            //else if (Type == LoadTypes.TRANSFORMER.ToString()) {
            //    ConnectedKva = Size;
            //    PdFactor = 1.25

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
            //            ConnectedKva = Size * Vol_tage * Math.Sqrt(3); //   / Efficiency / PowerFactor;
            //            Fla = Size;
            //            break;
            //    }
            //}

            switch (Type) {
            case "MOTOR":
                    PdFactor = 1.25;
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
                    PdFactor = 1.25;
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


        //FLA and Power
            if (Unit != "A") {
                Fla = ConnectedKva * 1000 / Voltage / Math.Sqrt(3);
                Fla = Math.Round(Fla, GlobalConfig.SigFigs);
            }

            ConnectedKva = Math.Round(ConnectedKva, GlobalConfig.SigFigs);
            DemandKva = Math.Round(ConnectedKva*LoadFactor, GlobalConfig.SigFigs);
            DemandKw = Math.Round(DemandKva*PowerFactor, GlobalConfig.SigFigs);
            DemandKvar = Math.Round(DemandKva*(1-PowerFactor), GlobalConfig.SigFigs);


            //PD and Starter
            PdSizeFrame = LibraryManager.GetBreakerFrame(this);
            PdSizeTrip = LibraryManager.GetBreakerTrip(this);
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
