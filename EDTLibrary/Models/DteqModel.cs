using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDTLibrary.LibraryData;
using PropertyChanged;

namespace EDTLibrary.Models {

    [AddINotifyPropertyChangedInterface]
    public class DteqModel : ILoadModel, IDistributionEquipmentModel {
        public DteqModel() {
            Category = Categories.DIST.ToString();
            Voltage = LineVoltage;
            PdType = EDTLibrary.ProjectSettings.EdtSettings.DteqDefaultPdTypeLV;
        }

       
        //Fields
        public double _derating = 0;

        //Properties
        #region IEquipmentModel

        [Browsable(false)] // make this property non-visisble by grids/databindings
        public int Id { get; set; } //was private beo

        public string Tag { get; set; }
        public string Category { get; set; } //dteq, load, component, cable,
        public string Type { get; set; }
        public string Description { get; set; }
        #endregion

        #region ILoadModel - User inputs
        public double Voltage { get; set; }
        public double Size { get; set; }
        public string Unit { get; set; }

        [DisplayName("Fed From")]
        public string FedFrom { get; set; }
        #endregion

        [DisplayName("Line\nVoltage")]
        public double LineVoltage { get; set; }

        [DisplayName("Load\nVoltage")]
        public double LoadVoltage { get; set; }

        //Loading
        #region ILoadModel - Privately Calculated Values
        [DisplayName("FLA")]

        public double Fla { get; set; }
        [DisplayName("Running\nAmps")]
        public double RunningAmps { get; set; }


        [DisplayName("Conn\nkVa")]
        public double ConnectedKva { get; set; }

        [DisplayName("Dem\nkVA")]
        public double DemandKva { get; set; }

        [DisplayName("Dem\nkW")]
        public double DemandKw { get; set; }

        [DisplayName("Dem\nkVAR")]
        public double DemandKvar { get; set; }

        

        [DisplayName("PF")]
        public double PowerFactor { get; set; }


        //Sizing
        public double MinCableAmps { get; set; }
        public double PercentLoaded { get; set; }

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

        #region Publicly Calculated Values
        [Browsable(false)]
        public List<ILoadModel> AssignedLoads { get; set; } = new List<ILoadModel>();

        public int LoadCount { get; set; }
        #endregion

        #region Lists
        [Browsable(false)]
        public List<ComponentModel> InLineComponents { get; set; } = new List<ComponentModel>();
        //public List<CableModel> Cables { get; set; } = new List<CableModel>();
        #endregion

        #region ILoadModel Interface Un-used
        public double LoadFactor { get; set; }

        [DisplayName("Eff")]
        public double Efficiency { get; set; }

        #endregion

        //Methods
        public void CalculateLoading() {
            //Calculates the individual loads of each MJEQ load
            foreach (ILoadModel load in AssignedLoads) {
                if (load.Category == Categories.DIST.ToString()) {
                    load.CalculateLoading();
                }
            }

            Voltage = LineVoltage;

            //Sums values from Assinged loads
                ConnectedKva = (from x in AssignedLoads select x.ConnectedKva).Sum();
                ConnectedKva = Math.Round(ConnectedKva, GlobalConfig.SigFigs);

                DemandKva = (from x in AssignedLoads select x.DemandKva).Sum();
                DemandKva = Math.Round(DemandKva, GlobalConfig.SigFigs);

                DemandKw = (from x in AssignedLoads select x.DemandKw).Sum();
                DemandKw = Math.Round(DemandKw, GlobalConfig.SigFigs);

                DemandKvar = (from x in AssignedLoads select x.DemandKvar).Sum();
                DemandKvar = Math.Round(DemandKvar, GlobalConfig.SigFigs);

            //calculates
                PowerFactor = DemandKw / DemandKva;
                PowerFactor = Math.Round(PowerFactor, GlobalConfig.SigFigs);

                RunningAmps = ConnectedKva * 1000 / Voltage / Math.Sqrt(3);
                RunningAmps = Math.Round(RunningAmps, GlobalConfig.SigFigs);

            //Full Load / Max operating Amps
                if (Unit == Units.kVA.ToString()) {
                    Fla = Size * 1000 / Voltage / Math.Sqrt(3);
                    Fla = Math.Round(Fla, GlobalConfig.SigFigs);
                }
                else if (Unit == Units.AMPS.ToString()) {
                    Fla = Size;
                }

            PercentLoaded = 100 * RunningAmps / Fla;
            PercentLoaded = Math.Round(PercentLoaded, GlobalConfig.SigFigs);

            MinCableAmps = Math.Max(Fla, RunningAmps);
            if (Type == DteqTypes.XFR.ToString()) {
                MinCableAmps *= 1.25;
            }

            //Derating
            foreach (var load in AssignedLoads) {
                //_totalLoadConductors += 3
                // if(Spacing != 100loadCable.QtyParallel)
            }

            if (LoadCount*3>=43) {
                _derating = 0.5;
            }
            else if (LoadCount*3 >=25){
                _derating = 0.6;
            }
            else if (LoadCount*3 >= 7) {
                _derating = 0.7;
            }
            else if (LoadCount * 3 >= 4) {
                _derating = 0.8;
            }
            else {
                _derating = 1;
            }
            GetMinimumPdSize();
        }

        public void GetMinimumPdSize() {
            //PD and Starter
            PdSizeFrame = LibraryManager.GetBreakerFrame(this);
            PdSizeTrip = LibraryManager.GetBreakerTrip(this);
        }

        public void CalculateMinimumCableSize() {

        }
    }
}
