using EDTLibrary.LibraryData;
using EDTLibrary.Models.Areas;
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
    public abstract class DistributionEquipment : IDteq, IComponentUser
    {
        public DistributionEquipment()
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
                 
                    if (PowerCable != null) {
                        PowerCable.AssignTagging(this);
                    }
                    foreach (var load in AssignedLoads) {
                        load.PowerCable.AssignTagging(load);
                    }
                }
            }
        }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        private int _areaId;
        public int AreaId
        {
            get { return _areaId; }
            set { _areaId = value; }
        }

        private IArea _area;
        public IArea Area
        {
            get { return _area; }
            set
            {
                var oldArea = _area;
                _area = value;
                if (Area != null) {
                    AreaManager.UpdateArea(this, _area, oldArea);
                }
            }
        }

        private string _nemaRating;
        public string NemaRating
        {
            get { return _nemaRating; }
            set { _nemaRating = value; }
        }
        private string _areaClassification;

        public string AreaClassification
        {
            get { return _areaClassification; }
            set { _areaClassification = value; }
        }

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
                    if (PowerCable!= null) {
                        PowerCable.GetRequiredAmps(this);
                    }
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
                else if (string.IsNullOrEmpty(_fedFromTag)) {
                    return "Empty Dteq";
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
                    //OnFedFromChanged();
                    CalculateLoading();
                }
            }
        }

        public int FedFromId { get; set; }
        public string FedFromType { get; set; }

        private IDteq _fedFrom;

        public IDteq FedFrom
        {
            get { return _fedFrom; }
            set {

                IDteq oldFedFrom = _fedFrom;
                IDteq nextFedFrom = value;

                try {
                    for (int i = 0; i < 500; i++) {
                        if (nextFedFrom != null) {
                            if (nextFedFrom.Tag == Tag) {
                                _fedFrom = oldFedFrom;
                                //break;
                                throw new InvalidOperationException("Equipment cannot be fed from itself.");
                            }
                            else if (nextFedFrom.Tag == GlobalConfig.Utility) {
                                _fedFrom = value;
                                DistributionManager.UpdateFedFrom(this, _fedFrom, oldFedFrom);
                                break;
                            }
                            else {
                                nextFedFrom = nextFedFrom.FedFrom;
                            }
                        }
                    }
                    if (GlobalConfig.GettingRecords == true) {
                        _fedFrom = value;
                        DistributionManager.UpdateFedFrom(this, _fedFrom, oldFedFrom);
                    }
                }
                catch (Exception ex) {
                    throw;
                }
            }
        }

        private double _lineVoltage;

        public double LineVoltage
        {
            //TODO - update cable and alert loads
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

        public PowerCableModel PowerCable { get; set; }
        public int LoadCount { get; set; }
        public ObservableCollection<IComponent> Components { get; set; }


        #endregion


        private double _sccr;
        public virtual double SCCR
        {
            get
            {
                if (Tag==GlobalConfig.Utility) {
                    return 0;
                } 
                return FedFrom.SCCR; 
            }
            set { _sccr = value; }
        }


        //Methods
        public void CalculateLoading()
        {

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
            if (Fla > 99999) Fla = 99999;

            PercentLoaded = RunningAmps / Fla * 100;
            PercentLoaded = Math.Round(PercentLoaded, GlobalConfig.SigFigs);

            GetMinimumPdSize();
            OnLoadingCalculated();
        }

        public void CreateCable()
        {
            if (PowerCable == null) {
                PowerCable = new PowerCableModel(this);
            }
        }
        public void SizeCable()
        {
            if (PowerCable ==null) {
                PowerCable = new PowerCableModel(this);
            }
            PowerCable.SetCableParameters(this);
            PowerCable.CalculateCableQtySizeNew();
        }
        public void CalculateCableAmps()
        {
            PowerCable.CalculateAmpacityNew(this);
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
        public void OnAssignedLoadReCalculated(object source, EventArgs e)
        {
            CalculateLoading();
        }

        public void UpdateAreaProperties()
        {
            NemaRating = Area.NemaRating;
            AreaClassification = Area.AreaClassification;
            PowerCable.CalculateAmpacityNew(this);
        }

    }
}
