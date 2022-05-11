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
using System.Threading.Tasks;

namespace EDTLibrary.Models.DistributionEquipment
{

    [AddINotifyPropertyChangedInterface]
    public abstract class DistributionEquipment : IDteq, IComponentUser
    {
        public DistributionEquipment()
        {
            Description = "";
            Category = Categories.DTEQ.ToString();
            Voltage = LineVoltage;
        }

        #region Properties

        public int Id { get; set; }

        private string _tag;
        public string Tag
        {
            get { return _tag; }
            set
            {
                var oldValue = _tag;
                _tag = value;
                if (GlobalConfig.GettingRecords==false) {
                 
                    if (PowerCable != null) {
                        PowerCable.AssignTagging(this);
                    }
                    foreach (var load in AssignedLoads) {
                        load.PowerCable.AssignTagging(load);
                    }
                }
                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(Tag), OldValue = oldValue, NewValue = _tag };
                    Undo.UndoList.Add(cmd);
                }
                OnPropertyUpdated();

            }
        }
        public string Category { get; set; }
        public string Type { get; set; }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                var oldValue = _description;
                _description = value;
                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(Description), OldValue = oldValue, NewValue = _description };
                    Undo.UndoList.Add(cmd);
                }
                OnPropertyUpdated();
            }
        }
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
                var oldValue = _area;
                _area = value;
                if (Area != null) {
                    AreaManager.UpdateArea(this, _area, oldValue);
                    if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                        var cmd = new CommandDetail { Item = this, PropName = nameof(Area), OldValue = oldValue, NewValue = _area };
                        Undo.UndoList.Add(cmd);
                    }
                OnPropertyUpdated();

                }

            }
        }

        private string _nemaRating;
        public string NemaRating
        {
            get { return _nemaRating; }
            set 
            { 
                var oldValue = _nemaRating;
                _nemaRating = value;
                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(NemaRating), OldValue = oldValue, NewValue = _nemaRating };
                    Undo.UndoList.Add(cmd);
                }
                OnPropertyUpdated();
            }
        }
        private string _areaClassification;

        public string AreaClassification
        {
            get { return _areaClassification; }
            set 
            { 
                var oldValue = _areaClassification;
                _areaClassification = value;
                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(AreaClassification), OldValue = oldValue, NewValue = _areaClassification };
                    Undo.UndoList.Add(cmd);
                }
            }
        }

        public double Voltage
        {
            get { return LineVoltage; }
            set 
            { 
                LineVoltage = value;
                if (GlobalConfig.GettingRecords == false) {
                    PowerCable.CreateTypeList(this);
                }
                OnPropertyUpdated();
            }
        }


        public double _size;
        public virtual double Size
        {
            get { return _size; }
            set
            {
                var oldValue = _size;
                _size = value;
                if (GlobalConfig.GettingRecords == false) {
                    CalculateLoading();
                    SCCR = CalculateSCCR();
                    if (PowerCable!= null) {
                        PowerCable.GetRequiredAmps(this);
                    }
                    if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                        var cmd = new CommandDetail { Item = this, PropName = nameof(Size), OldValue = oldValue, NewValue = _size };
                        Undo.UndoList.Add(cmd);
                    }
                }
                OnPropertyUpdated();
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
                if (FedFrom != null && _fedFromTag != GlobalConfig.Deleted) {
                    FedFrom.Tag = _fedFromTag;
                }
                
                if (GlobalConfig.GettingRecords == false) {
                    //OnFedFromChanged();
                    //CalculateLoading();
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

                IDteq oldValue = _fedFrom;
                IDteq nextFedFrom = value;

                try {
                    for (int i = 0; i < 500; i++) {
                        if (nextFedFrom != null) {
                            if (nextFedFrom.Tag == Tag) {
                                _fedFrom = oldValue;
                                //break;
                                throw new InvalidOperationException("Equipment cannot be fed from itself.");
                            }
                            else if (nextFedFrom.Tag == GlobalConfig.Utility || nextFedFrom.Tag == GlobalConfig.Deleted) {
                                _fedFrom = value;
                                DistributionManager.UpdateFedFrom(this, _fedFrom, oldValue);
                                break;
                            }
                            else {
                                nextFedFrom = nextFedFrom.FedFrom;
                            }
                        }
                    }
                    if (GlobalConfig.GettingRecords == true) {
                        _fedFrom = value;
                        DistributionManager.UpdateFedFrom(this, _fedFrom, oldValue);
                    }
                }
                catch (Exception ex) {
                    throw;
                }
                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(FedFrom), OldValue = oldValue, NewValue = _fedFrom };
                    Undo.UndoList.Add(cmd);
                }
                OnPropertyUpdated();

            }
        }

        private double _lineVoltage;

        public double LineVoltage
        {
            //TODO - update cable and alert loads
            get { return _lineVoltage; }
            set
            {
                var oldValue = _lineVoltage;
                _lineVoltage = value;
                Voltage = _lineVoltage;
                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(LineVoltage), OldValue = oldValue, NewValue = _lineVoltage };
                    Undo.UndoList.Add(cmd);
                }
                OnPropertyUpdated();

            }
        }

        private double _loadVoltage;

        public double LoadVoltage
        {
            get { return _loadVoltage; }
            set 
            {
                var oldValue = _loadVoltage;
                _loadVoltage = value;
                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(LoadVoltage), OldValue = oldValue, NewValue = _loadVoltage };
                    Undo.UndoList.Add(cmd);
                }
            }
        }

        //Loading
        private double _fla;

        public double Fla
        {
            get { return _fla; }
            set
            {
                _fla = value;           
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


        public double SCCR { get; set; }

        public virtual double CalculateSCCR()
        {
            if (Tag == GlobalConfig.Utility) {
                return 0;
            }
            else if (FedFrom == null) {
                return 0;
            }
            return FedFrom.SCCR;
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

            DteqManager.SetPd(this);
            SCCR = CalculateSCCR();
            OnLoadingCalculated();
            OnPropertyUpdated();

        }

        public void CreateCable()
        {
            if (PowerCable == null && GlobalConfig.GettingRecords==false) {
                PowerCable = new PowerCableModel(this);
            }
        }
        public void SizeCable()
        {
            CreateCable();
            PowerCable.SetCableParameters(this);
            PowerCable.CreateTypeList(this);
            PowerCable.CalculateCableQtyAndSize();
        }
        public void CalculateCableAmps()
        {
            PowerCable.CalculateAmpacityNew(this);
        }
        

        //Events
        public event EventHandler LoadingCalculated;
        public virtual void OnLoadingCalculated()
        {
            if (LoadingCalculated != null) {
                LoadingCalculated(this, EventArgs.Empty);
            }
        }

        public event EventHandler PropertyUpdated;
        public virtual void OnPropertyUpdated()
        {
            if (PropertyUpdated != null) {
                PropertyUpdated(this, EventArgs.Empty);
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
