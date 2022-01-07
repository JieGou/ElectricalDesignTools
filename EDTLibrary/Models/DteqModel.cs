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
            ConductorQty = 3;
        }

       
        //Fields

        #region Properties

        public int Id { get; set; } //was private beo

        private string _tag;
        public string Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                if (AssignedLoads != null) {
                    foreach (var iload in AssignedLoads) {
                        iload.FedFrom = _tag;
                    }
                }

            }
        }
        public string Category { get; set; } //dteq, load, component, cable,
        public string Type { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public double Voltage { get; set; }
        public double Size { get; set; }
        public string Unit { get; set; }

        private string _fedFrom;
        public string FedFrom 
        { 
            get { return _fedFrom; }
            set 
            { 
                _fedFrom = value;
            }
        }

        public double LineVoltage { get; set; }
        public double LoadVoltage { get; set; }

        //Loading
        public double Fla { get; set; }
        public double RunningAmps { get; set; }
        public double ConnectedKva { get; set; }
        public double DemandKva { get; set; }
        public double DemandKw { get; set; }
        public double DemandKvar { get; set; }
        public double PowerFactor { get; set; }


        //Sizing
        public double PercentLoaded { get; set; }

        public string PdType { get; set; }

        public double PdSizeTrip { get; set; }
        public double PdSizeFrame { get; set; }

        //Cables

        public int ConductorQty { get; set; }

        public int CableQty { get; set; }
        public string CableSize { get; set; }
        public double CableBaseAmps { get; set; }

        private double _derating = 1;
        public double CableSpacing { get; set; }
        public double CableDerating
        {
            get { return _derating; }
            set { _derating = value; }
        }
        public double CableDeratedAmps { get; set; }
        public double CableRequiredAmps { get; set; }
        public CableModel Cable { get; set; }


        public List<ILoadModel> AssignedLoads { get; set; } = new List<ILoadModel>();
        public int LoadCount { get; set; }
        public List<ComponentModel> InLineComponents { get; set; } = new List<ComponentModel>();


        public double LoadFactor { get; set; }
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
                else if (Unit == Units.A.ToString()) {
                    Fla = Size;
                }

            PercentLoaded = RunningAmps / Fla * 100;
            PercentLoaded = Math.Round(PercentLoaded, GlobalConfig.SigFigs);

            CableRequiredAmps = Math.Max(Fla, RunningAmps);
            if (Type == DteqTypes.XFR.ToString()) {
                CableRequiredAmps *= 1.25;
            }

            //Derating
            foreach (var load in AssignedLoads) {
                //_totalLoadConductors += 3
                // if(Spacing != 100loadCable.QtyParallel)
            }

            CableDerating = 1;
            if (LoadCount*3>=43) {
                CableDerating = 0.5;
            }
            else if (LoadCount*3 >=25){
                CableDerating = 0.6;
            }
            else if (LoadCount*3 >= 7) {
                CableDerating = 0.7;
            }
            else if (LoadCount * 3 >= 4) {
                CableDerating = 0.8;
            }
            else {
                CableDerating = 1;
            }
            GetMinimumPdSize();
            GetCable();
        }

        public void GetCable()
        {
            Cable = new CableModel(this);

            ConductorQty = Cable.ConductorQty;
            CableQty = Cable.CableQty;
            CableSize = Cable.CableSize;
            CableBaseAmps = Cable.CableBaseAmps;

            CableSpacing = Cable.CableSpacing;
            CableDerating = Cable.CableDerating;
            CableDeratedAmps = Cable.CableDeratedAmps;
            CableRequiredAmps = Cable.CableRequiredAmps;

        }

        public void CalculateCableAmps()
        {
            Cable.CableQty = CableQty;
            Cable.CableSize = CableSize;

            Cable.CalculateAmpacity();
            //Cable = Cable.CalculateAmpacity(Cable);

            CableBaseAmps = Cable.CableBaseAmps;
            CableDeratedAmps = Cable.CableDeratedAmps;
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
