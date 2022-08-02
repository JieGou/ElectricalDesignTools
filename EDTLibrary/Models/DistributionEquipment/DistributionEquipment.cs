using EDTLibrary.A_Helpers;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.aMain;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.Models.DistributionEquipment
{

    [AddINotifyPropertyChangedInterface]
    public abstract class DistributionEquipment : IDteq, IComponentUser//, INotifyDataErrorInfo 
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
                if (GlobalConfig.GettingRecords == false) {

                    if (PowerCable != null) {
                        PowerCable.AssignTagging(this);
                    }
                    foreach (var load in AssignedLoads) {
                        load.PowerCable.AssignTagging(load);
                    }
                }

                Undo.AddUndoCommand(this, nameof(Tag), oldValue, _tag);
                OnPropertyUpdated(nameof(Tag));

            }
        }
        public string Category { get; set; }
        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                var oldValue = _type;
                _type = value;
                if (GlobalConfig.GettingRecords == false) {

                }
                Undo.AddUndoCommand(this, nameof(Type), oldValue, _type);
            }
        }

        private string _subType;

        public string SubType
        {
            get { return _subType; }
            set
            {
                var oldValue = _subType;
                _subType = value;
                if (GlobalConfig.GettingRecords == false) {

                }
                Undo.AddUndoCommand(this, nameof(SubType), oldValue, _subType);
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                var oldValue = _description;
                _description = value;
                Undo.AddUndoCommand(this, nameof(Description), oldValue, _description);

                OnPropertyUpdated(nameof(Description));
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
                if (value == null) return;
                var oldValue = _area;
                _area = value;
                if (Area != null) {
                    AreaManager.UpdateArea(this, _area, oldValue);
                    Undo.AddUndoCommand(this, nameof(Area), oldValue, _area);

                    if (GlobalConfig.GettingRecords == false && PowerCable != null) {
                        PowerCable.Derating = CableManager.CableSizer.GetDerating(PowerCable);
                        PowerCable.CalculateAmpacity(this);
                    }
                    OnAreaChanged();
                    OnPropertyUpdated(nameof(Area));
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

                Undo.AddUndoCommand(this, nameof(NemaRating), oldValue, _nemaRating);
                OnPropertyUpdated(nameof(NemaRating));
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
                Undo.AddUndoCommand(this, nameof(AreaClassification), oldValue, _areaClassification);
                OnPropertyUpdated(nameof(AreaClassification));
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
                    if (PowerCable != null) {
                        PowerCable.GetRequiredAmps(this);
                    }
                    if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                        var cmd = new UndoCommandDetail { Item = this, PropName = nameof(Size), OldValue = oldValue, NewValue = _size };
                        Undo.AddUndoCommand(cmd);
                    }
                }
                OnPropertyUpdated(nameof(Size));
            }
        }
        public string Unit { get; set; }
        public int FedFromId { get; set; }
        public string FedFromType { get; set; }

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
            }
        }
      
        private IDteq _fedFrom;
        public IDteq FedFrom
        {
            get { return _fedFrom; }
            set
            {

                if (value == null) return;

                IDteq oldValue = _fedFrom;
                IDteq nextFedFrom = value;
                //ClearErrors(nameof(FedFrom));
                try {
                    if (GlobalConfig.GettingRecords == true) {
                        // Assigned loads add, and events are subscribed to inside UpdateFedFrom;
                        _fedFrom = value;
                        DistributionManager.UpdateFedFrom(this, _fedFrom, oldValue);
                        return;
                    }
                    _fedFrom = nextFedFrom;
                    //Fed from validation
                    for (int i = 0; i < 500; i++) {
                        if (nextFedFrom == null) {
                            DistributionManager.UpdateFedFrom(this, _fedFrom, new DteqModel()) ;
                            break;
                        }
                        else {
                            if (nextFedFrom.Tag == Tag) {
                                ErrorHelper.Notify("Equipment Cannot be fed from itself.", "Circular Feed Error");
                                _fedFrom = oldValue;

                                break;
                                //throw new InvalidOperationException("Equipment cannot be fed from itself.");
                                //AddError(nameof(FedFrom),"Equipment cannot be fed from itself.");
                            }
                            else if (nextFedFrom.Tag == GlobalConfig.Utility || nextFedFrom.Tag == GlobalConfig.Deleted || i == 5) {
                                _fedFrom = value;
                                DistributionManager.UpdateFedFrom(this, _fedFrom, oldValue);
                                break;
                            }
                            else {
                                nextFedFrom = nextFedFrom.FedFrom;
                            }
                        }
                    }
                }
                catch (Exception ex) {
                    throw;
                }

                Undo.AddUndoCommand(this, nameof(FedFrom), oldValue, _fedFrom);
                OnPropertyUpdated(nameof(FedFrom));
            }
        }

        private double _lineVoltage;

        public double LineVoltage
        {
            //TODO - update cable and alert loads
            get { return _lineVoltage; }
            set
            {
                if (value == null || value == 0) return;

                var oldValue = _lineVoltage;
                _lineVoltage = value;
                Voltage = _lineVoltage;

                Undo.AddUndoCommand(this, nameof(LineVoltage), oldValue, _lineVoltage);

                OnPropertyUpdated(nameof(LineVoltage));
            }
        }

        private double _loadVoltage;

        public double LoadVoltage
        {
            get { return _loadVoltage; }
            set
            {
                if (value == null || value == 0) return;

                var oldValue = _loadVoltage;
                _loadVoltage = value;

                Undo.AddUndoCommand(this, nameof(LoadVoltage), oldValue, _loadVoltage);

                OnPropertyUpdated(nameof(LoadVoltage));

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
        public CableModel PowerCable { get; set; }


        //Components
        public ObservableCollection<IComponent> AuxComponents { get; set; } = new ObservableCollection<IComponent>();
        public ObservableCollection<IComponent> CctComponents { get; set; } = new ObservableCollection<IComponent>();

        //Components


        public ILocalControlStation Lcs { get; set; }
        private bool _lcsBool;
        public bool LcsBool
        {
            get { return _lcsBool; }
            set
            {
                var _oldValue = _lcsBool;
                _lcsBool = value;
                if (_lcsBool == true) {
                    ComponentManager.AddLcs(this, ScenarioManager.ListManager);
                }
                if (_lcsBool == false) {
                    ComponentManager.RemoveLcs(this, ScenarioManager.ListManager);
                }

            }
        }

        public IComponent Drive { get; set; }

        private bool _driveBool;
        public bool DriveBool
        {
            get { return _driveBool; }
            set
            {
                _driveBool = value;
                if (_driveBool == true) {
                    PdType = "BKR";
                }
            }
        }

        private int _driveId;
        public int DriveId
        {
            get { return _driveId; }
            set { _driveId = value; }
        }

        public IComponent Disconnect { get; set; }

        private bool _disconnectBool;
        public bool DisconnectBool
        {
            get { return _disconnectBool; }
            set
            {
                var oldValue = _disconnectBool;
                _disconnectBool = value;

            }
        }

        private int _disconnectId;
        private double _loadCableDerating;

        public int DisconnectId
        {
            get { return _disconnectId; }
            set { _disconnectId = value; }
        }

        #endregion



        public double HeatLoss { get; set; }



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

        //Todo - recalculate cables when changed
        public double LoadCableDerating { get => _loadCableDerating;

            set { _loadCableDerating = value;
                foreach (var load in AssignedLoads) {
                    load.PowerCable.AutoSizeAsync();
                }
                OnPropertyUpdated(nameof(LoadCableDerating));
            }
        }



        //Methods
        public void CalculateLoading()
        {

            Voltage = LineVoltage;
            var dis = this;
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
            OnPropertyUpdated(nameof(CalculateLoading));

        }

        public void CreatePowerCable()
        {
            if (PowerCable == null && GlobalConfig.GettingRecords == false) {
                PowerCable = new CableModel(this);
            }
        }
        public void SizePowerCable()
        {
            CreatePowerCable();
            PowerCable.SetCableParameters(this);
            PowerCable.CreateTypeList(this);
            PowerCable.AutoSize();
        }
        public void CalculateCableAmps()
        {
            PowerCable.CalculateAmpacity(this);
        }

        public void OnAssignedLoadReCalculated(object source, EventArgs e)
        {
            CalculateLoading();
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
        public async Task OnPropertyUpdated(string property = "default")
        {
            if (PropertyUpdated != null) {
                PropertyUpdated(this, EventArgs.Empty);
            }
            ErrorHelper.Log($"Dteq_OnPropertyUpdated - Tag = {Tag}, Property = {property}");
            //await Task.Run(() => {
                
            //});

        }

      
        public async Task UpdateAreaProperties()
        {
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                NemaRating = Area.NemaRating;
                AreaClassification = Area.AreaClassification;
                PowerCable.Derating = CableManager.CableSizer.GetDerating(PowerCable);
                PowerCable.CalculateAmpacity(this);
            }));
        }

        public event EventHandler AreaChanged;
        public async Task OnAreaChanged()
        {
            await Task.Run(() => {
                if (AreaChanged != null) {
                    AreaChanged(this, EventArgs.Empty);
                }
            });
        }

        public void MatchOwnerArea(object source, EventArgs e)
        {

        }

    }
}
