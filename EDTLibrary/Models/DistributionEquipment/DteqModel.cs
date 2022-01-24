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
    public class DteqModel : IDteq, ComponentUser
    {
        public DteqModel() {
            Category = Categories.DTEQ.ToString();
            Voltage = LineVoltage;
            PdType = EDTLibrary.ProjectSettings.EdtSettings.DteqDefaultPdTypeLV;
        }

      
        #region Properties

        public int Id { get; set; }
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
                ListManager.CreateDteqDict();
            }
        }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public double Voltage { get; set; }
        public double _size; 
        public double Size 
        {
            get { return _size; }
            set
            {
                //double parsedValue =0;
                //if (Double.TryParse(value, out parsedValue) ==false) {
                _size = value;
                if (GlobalConfig.GettingRecords == false) {
                    CalculateLoading();
                }
            }
        }
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
        public double AmpacityFactor { get; set; }

        //Sizing
        public double PercentLoaded { get; set; }

        public string PdType { get; set; }
        public double PdSizeTrip { get; set; }
        public double PdSizeFrame { get; set; }

        //Cables
       
        public int PowerCableId { get; set; }
        public PowerCableModel Cable { get; set; }


        public List<IPowerConsumer> AssignedLoads { get; set; } = new List<IPowerConsumer>();
        public int LoadCount { get; set; }


        public List<IComponentModel> Components { get; set; }




        #endregion

        //Methods
        public void CalculateLoading() {

            //Calculates the individual loads of each MJEQ load
            foreach (IPowerConsumer load in AssignedLoads) {
                if (load.Category == Categories.DTEQ.ToString()) {
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
                    Fla = _size * 1000 / Voltage / Math.Sqrt(3);
                    Fla = Math.Round(Fla, GlobalConfig.SigFigs);
                }
                else if (Unit == Units.A.ToString()) {
                    Fla = _size;
                }

            PercentLoaded = RunningAmps / Fla * 100;
            PercentLoaded = Math.Round(PercentLoaded, GlobalConfig.SigFigs);

            GetMinimumPdSize();
            GetCable();
        }

        public void GetCable()
        {
            Cable = new PowerCableModel(this);
        }

        public void CalculateCableAmps()
        {
            Cable.CalculateAmpacity();
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
