using EDTLibrary.LibraryData;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary.Models.DistributionEquipment
{

    [AddINotifyPropertyChangedInterface]
    public class DteqModel : IDteq, ComponentUser
    {
        public DteqModel()
        {
            Category = Categories.DTEQ.ToString();
            Voltage = LineVoltage;
            PdType = ProjectSettings.EdtSettings.DteqDefaultPdTypeLV;
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
                if (GlobalConfig.GettingRecords==false) {
                    if (AssignedLoads != null) {
                        foreach (var iload in AssignedLoads) {
                            iload.FedFromTag = _tag;
                        }
                    }
                    if (Cable != null) {
                        Cable.SetCableParameters(this);
                    }
                }
               
            }
        }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        private double _voltage;

        public double Voltage
        {
            get { return LineVoltage; }
            set { LineVoltage = value; }
        }


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

        private string _fedFromTag;
        public string FedFromTag
        {
            get
            {
                if (FedFrom != null) {
                    return FedFrom.Tag;
                }
                return _fedFromTag;
            }
            set
            {
                _fedFromTag = value;
                if (FedFrom != null) {
                    FedFrom.Tag = _fedFromTag;
                }
                
                if (GlobalConfig.GettingRecords == false) {
                    OnFedFromChanged();
                    CalculateLoading();
                }
            }
        }

        public int FedFromId { get; set; }
        public string FedFromType { get; set; }
        public IDteq FedFrom { get; set; }

        private double _lineVoltage;

        public double LineVoltage
        {
            get { return _lineVoltage; }
            set
            {
                _lineVoltage = value;
                Voltage = _lineVoltage;
            }
        }

        public double LoadVoltage { get; set; }

        //Loading
        private double _fla;

        public double Fla
        {
            get { return _fla; }
            set
            {
                _fla = value;
                //if (_fla >= 99999) {
                //    _fla = 111111;
                //}
            }
        }

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

        public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer>();

        public int PowerCableId { get; set; }
        public PowerCableModel Cable { get; set; }
        public int LoadCount { get; set; }
        public ObservableCollection<IComponentModel> Components { get; set; }


        #endregion

        //Methods
        public void CalculateLoading()
        {

            //Calculates the individual loads of each MJEQ load
            //foreach (IPowerConsumer load in AssignedLoads) {
            //    if (load.Category == Categories.DTEQ.ToString()) {
            //        load.CalculateLoading();
            //    }
            //}

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
            PowerFactor = Math.Round(PowerFactor, 2);

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
            OnLoadingCalculated();
        }

        public void SizeCable()
        {
            if (Cable ==null) {
                Cable = new PowerCableModel(this);
            }
            Cable.SetCableParameters(this);
            Cable.CalculateCableQtySizeNew();
        }
        public void CalculateCableAmps()
        {
            Cable.CalculateAmpacity();
        }
        public void GetMinimumPdSize()
        {
            //PD and Starter
            PdSizeFrame = LibraryManager.GetBreakerFrame(this);
            PdSizeTrip = LibraryManager.GetBreakerTrip(this);
        }

        //Events
        public event EventHandler LoadingCalculated;
        public virtual void OnLoadingCalculated()
        {
            if (LoadingCalculated != null) {
                LoadingCalculated(this, EventArgs.Empty);
            }
        }
        public void OnDteqLoadingCalculated(object source, EventArgs e)
        {
            CalculateLoading();
        }


        public event EventHandler FedFromChanged;
        public virtual void OnFedFromChanged()
        {
            if (FedFromChanged != null) {
                FedFromChanged(this, EventArgs.Empty);
            }
        }
    }

}
