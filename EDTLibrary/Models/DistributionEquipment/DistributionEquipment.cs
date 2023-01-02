using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Calculations;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Models.Validators;
using EDTLibrary.Services;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
//using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.Models.DistributionEquipment
{

    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public abstract class DistributionEquipment : IDteq, IComponentUser //, INotifyDataErrorInfo 
    {

        public DistributionEquipment()
        {
            Description = "";
            Category = Categories.DTEQ.ToString();
            Voltage = LineVoltage;
        }

        #region Properties

        public virtual bool CanIncludeMotorStarter { get; set; }
        public int ProtectionDeviceId { get; set; }
        public IProtectionDevice ProtectionDevice { 
            get => _protectionDevice; 
            set 
            { 
                _protectionDevice = value; 
                ProtectionDeviceId = _protectionDevice.Id;
            }
        }
        private IProtectionDevice _protectionDevice;


        public int Id { get; set; }


        public string Tag
        {
            get { return _tag; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) || value == _tag) return;
                if (TagAndNameValidator.IsTagAvailable(value, ScenarioManager.ListManager) == false) {
                    return;
                }

                var oldValue = _tag;
                _tag = value;

                if (DaManager.GettingRecords == true) return;

                if (PowerCable != null) {
                    PowerCable.SetSourceAndDestinationTags(this);
                }
                foreach (var load in AssignedLoads) {
                    load.PowerCable.SetSourceAndDestinationTags(load);
                }

                UndoManager.AddUndoCommand(this, nameof(Tag), oldValue, _tag);
                OnPropertyUpdated(nameof(Tag) + ": " + Tag.ToString());

            }
        }
        private string _tag;

        public string Category { get; set; }
        public string Type
        {
            get { return _type; }
            set
            {
                var oldValue = _type;
                _type = value;
                if (DaManager.GettingRecords == false) {

                }
                UndoManager.AddUndoCommand(this, nameof(Type), oldValue, _type);
            }
        }
        private string _type;

        public string SubType
        {
            get { return _subType; }
            set
            {
                var oldValue = _subType;
                _subType = value;
                if (DaManager.GettingRecords == false) {

                }
                UndoManager.AddUndoCommand(this, nameof(SubType), oldValue, _subType);
            }
        }
        private string _subType;

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                var oldValue = _description;
                _description = value;
                UndoManager.AddUndoCommand(this, nameof(Description), oldValue, _description);

                OnPropertyUpdated(nameof(Description) + ": " + Description.ToString());
            }
        }
        private string _description = "";

        public int AreaId
        {
            get { return _areaId; }
            set { _areaId = value; }
        }
        private int _areaId;

        public IArea Area
        {
            get { return _area; }
            set
            {
                if (value == null) return;
                var oldValue = _area;
                _area = value;
                UndoManager.CanAdd = false;
                UndoManager.Lock(this, nameof(Area));
                if (Area != null) {
                    AreaManager.UpdateArea(this, _area, oldValue);

                    if (DaManager.GettingRecords == false && PowerCable != null) {
                        PowerCable.Derating = CableManager.CableSizer.SetDerating(PowerCable);
                        PowerCable.CalculateAmpacity(this);
                    }

                    OnAreaChanged();
                    UndoManager.CanAdd = true;

                    UndoManager.AddUndoCommand(this, nameof(Area), oldValue, _area);

                    OnPropertyUpdated(nameof(Area) + ": " + Area.ToString());
                }
            }
        }
        private IArea _area;

        public string NemaRating
        {
            get { return _nemaRating; }
            set
            {
                if (value == null) return;
                var oldValue = _nemaRating;
                _nemaRating = value;

                UndoManager.AddUndoCommand(this, nameof(NemaRating), oldValue, _nemaRating);
                OnPropertyUpdated(nameof(NemaRating) + ": " + NemaRating.ToString());
            }
        }
        private string _nemaRating;

        public string AreaClassification
        {
            get { return _areaClassification; }
            set
            {
                var oldValue = _areaClassification;
                _areaClassification = value;
                UndoManager.AddUndoCommand(this, nameof(AreaClassification), oldValue, _areaClassification);
                OnPropertyUpdated(nameof(AreaClassification) + ": " + AreaClassification.ToString());
            }
        }
        private string _areaClassification;


        public string PanelSide
        {
            get { return _panelSide; }
            set
            {
                _panelSide = value;
                OnPropertyUpdated();
            }
        }
        private string _panelSide;




        public double _size;
        public virtual double Size
        {
            get { return _size; }
            set
            {
                var oldValue = _size;
                _size = value;

                if (DaManager.GettingRecords == false) {
                    CalculateLoading();
                    SCCR = CalculateSCCR();
                    if (PowerCable != null) {
                        PowerCable.GetRequiredAmps(this);
                    }
                    if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                        var cmd = new UndoCommandDetail { Item = this, PropName = nameof(Size), OldValue = oldValue, NewValue = _size };
                        UndoManager.AddUndoCommand(cmd);
                    }
                }
                OnPropertyUpdated(nameof(Size) + ": " + Size.ToString());
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

                if (value == null || value == _fedFrom) return;

                IDteq oldValue = _fedFrom;
                IDteq nextFedFrom = value;

                UndoManager.CanAdd = false;
                try {
                    if (DaManager.GettingRecords == true) {
                        // Assigned loads add, and events are subscribed to inside UpdateFedFrom;
                        _fedFrom = value;
                        DistributionManager.UpdateFedFrom(this, _fedFrom, oldValue);
                        return;
                    }
                    _fedFrom = nextFedFrom;

                    //Fed from validation - Checks if the equipment is fed from itself and does not allow the change to proceed.
                    for (int i = 0; i < 500; i++) {
                        if (nextFedFrom == null) {
                            DistributionManager.UpdateFedFrom(this, _fedFrom, new DteqModel());
                            return;
                        }
                        else {
                            //invalid
                            if (nextFedFrom.Tag == Tag) {

                                //Must change value back before showing message box
                                _fedFrom = oldValue;

                                //Message must be executed last
                                ErrorHelper.NotifyUserError("Equipment Cannot be fed from itself, directly or through other equipment.",
                                    "Circular Feed Error",
                                    MessageBoxImage.Warning);
                                break;
                            }
                            //Valid
                            else if (nextFedFrom.Tag == GlobalConfig.Utility || nextFedFrom.Tag == GlobalConfig.Deleted || i == 500) {
                                _fedFrom = value;
                                DistributionManager.UpdateFedFrom(this, _fedFrom, oldValue);
                                UndoManager.CanAdd = true;
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

                UndoManager.AddUndoCommand(this, nameof(FedFrom), oldValue, _fedFrom);
                UndoManager.CanAdd = true;
                OnPropertyUpdated(nameof(FedFrom) + ": " + FedFrom.Tag.ToString());
            }
        }


        public int VoltageTypeId { get; set; } //unused for PowerConsumer interface
        public VoltageType VoltageType
        {
            get => _lineVoltageType;
            set
            {
                var oldValue = _voltageType;
                _voltageType = value;

                UndoManager.Lock(this, nameof(VoltageType));

                LineVoltageType = value;
                Tag = Tag;
                if (Type != DteqTypes.XFR.ToString() && Type != null) {
                    LoadVoltageType = value;
                }
                UndoManager.AddUndoCommand(this, nameof(VoltageType), oldValue, _voltageType);
            }
        }
        //unused, for PowerConsumer interface
        private VoltageType _voltageType;


        public double Voltage
        {
            get { return LineVoltage; }
            set
            {
                LineVoltage = value;
                if (DaManager.GettingRecords == false) {
                    PowerCable.CreateTypeList(this);
                }
            }
        }


        public int LineVoltageTypeId { get; set; }
        public VoltageType LineVoltageType
        {
            get { return _lineVoltageType; }
            set
            {
                if (value == null) return;
                var oldValue = _lineVoltageType;
                _lineVoltageType = value;
                LineVoltage = _lineVoltageType.Voltage;

                UndoManager.Lock(this, nameof(LineVoltageType));
                LineVoltageTypeId = _lineVoltageType.Id;
                LineVoltage = _lineVoltageType.Voltage;
                UndoManager.AddUndoCommand(this, nameof(LineVoltageType), oldValue, _lineVoltageType);

                if (DaManager.Importing == false && DaManager.GettingRecords == false) {
                    CalculateLoading(nameof(LineVoltageType));
                }

                OnPropertyUpdated(nameof(LineVoltageType));
            }
        }
        public VoltageType _lineVoltageType;

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

                UndoManager.AddUndoCommand(this, nameof(LineVoltage), oldValue, _lineVoltage);

                OnPropertyUpdated(nameof(LineVoltage) + ": " + LineVoltage.ToString());

            }
        }
        private double _lineVoltage;


        public int LoadVoltageTypeId { get; set; }
        public VoltageType LoadVoltageType
        {
            get { return _loadVoltageType; }
            set
            {
                if (value == null) return;
                var oldValue = _loadVoltageType;
                _loadVoltageType = value;
                LoadVoltage = _loadVoltageType.Voltage;

                UndoManager.Lock(this, nameof(LoadVoltageType));
                LoadVoltageTypeId = _loadVoltageType.Id;
                LoadVoltage = _loadVoltageType.Voltage;

                //Load voltages updates

                if (DaManager.Importing == false && DaManager.GettingRecords == false) {
                    EdtNotificationService.SendAlert(this, $"The voltage of each load fed from {Tag} has changed to {LoadVoltageType.VoltageString}", "Assigned Loads Voltage Change");
                    foreach (var load in AssignedLoads) {
                        load.VoltageType = LoadVoltageType;
                    }
                }

                UndoManager.AddUndoCommand(this, nameof(LoadVoltageType), oldValue, _loadVoltageType);

                if (DaManager.Importing == false && DaManager.GettingRecords == false) {
                    CalculateLoading(nameof(LoadVoltageType));
                }
                OnPropertyUpdated(nameof(LoadVoltageType));
            }
        }
        public VoltageType _loadVoltageType;

        public double LoadVoltage
        {
            get { return _loadVoltage; }
            set
            {
                if (value == null || value == 0) return;

                var oldValue = _loadVoltage;
                _loadVoltage = value;

                UndoManager.AddUndoCommand(this, nameof(LoadVoltage), oldValue, _loadVoltage);

                OnPropertyUpdated(nameof(LoadVoltage) + ": " + LoadVoltage.ToString());

            }
        }
        private double _loadVoltage;

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


        //OCPD
        //public string PdType { get; set; }
        //public double PdSizeTrip { get; set; }
        //public double PdSizeFrame { get; set; }


        public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer>();
        public CableModel PowerCable { get; set; }


        //Components
        public ObservableCollection<IComponentEdt> AuxComponents { get; set; } = new ObservableCollection<IComponentEdt>();
        [Browsable(false)]
        public ObservableCollection<IComponentEdt> CctComponents { get; set; } = new ObservableCollection<IComponentEdt>();

        //Components


        public ILocalControlStation Lcs { get; set; }
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
        private bool _lcsBool;

        public IComponentEdt StandAloneStarter { get; set; }

        public bool StandAloneStarterBool
        {
            get { return _standAloneStarterBool; }
            set
            {
                _standAloneStarterBool = value;
                if (_standAloneStarterBool == true) {
                }
            }
        }
        private bool _standAloneStarterBool;

        public int StandAloneStarterId
        {
            get { return _standAloneStarterId; }
            set { _standAloneStarterId = value; }
        }
        private int _standAloneStarterId;

        public IComponentEdt Disconnect { get; set; }

        public bool DisconnectBool
        {
            get { return _disconnectBool; }
            set
            {
                var oldValue = _disconnectBool;
                _disconnectBool = value;

            }
        }
        private bool _disconnectBool;


        public int DisconnectId
        {
            get { return _disconnectId; }
            set { _disconnectId = value; }
        }
        private int _disconnectId;

        public IComponentEdt SelectedComponent { get; set; }

        #endregion



        public double HeatLoss { get; set; }

        public double SCCR { get; set; }


        //Todo - recalculate cables when changed
        public double LoadCableDerating
        {
            get => _loadCableDerating;

            set
            {
                _loadCableDerating = value;
                foreach (var load in AssignedLoads) {
                    load.PowerCable.AutoSizeAllAsync();
                }
                OnPropertyUpdated(nameof(LoadCableDerating) + ": " + LoadCableDerating.ToString());
            }
        }
        private double _loadCableDerating;


        public bool IsCalculating { get; set; }
        public int SequenceNumber
        {
            get => _sequenceNumber;
            set
            {
                _sequenceNumber = value;
                OnPropertyUpdated();
            }

        }

        public int CircuitNumber
        {
            get { return _circuitNumber; }
            set { _circuitNumber = value; }
        }
        private int _circuitNumber;
        private int _sequenceNumber;





        public bool IsMainLugsOnly { get; set; }


        public CalculationFlags CalculationFlags { get; set; }


        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyUpdated(nameof(IsSelected));
            }
        }
        private bool _isSelected = false;



        //Methods

        public virtual void Create() { }
        public virtual void Initialize() { }

        public virtual void Delete() { }

        public virtual void CalculateLoading(string propertyName = "")
        {
            if (Tag == GlobalConfig.Utility) return;
            //if (DaManager.Importing) return;

            if (LineVoltageType == null || LoadVoltageType == null) return;


            IsCalculating = true;
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

            RunningAmps = DemandKva * 1000 / LineVoltageType.Voltage / Math.Sqrt(LineVoltageType.Phase);
            RunningAmps = Math.Round(RunningAmps, GlobalConfig.SigFigs);

            //Full Load / Max operating Amps
            if (Unit == Units.kVA.ToString()) {
                Fla = _size * 1000 / LineVoltageType.Voltage / Math.Sqrt(LineVoltageType.Phase);
                Fla = Math.Round(Fla, GlobalConfig.SigFigs);

            }
            else if (Unit == Units.A.ToString()) {
                Fla = _size;
            }
            if (Fla > 99999) Fla = 99999;

            PercentLoaded = RunningAmps / Fla * 100;
            PercentLoaded = Math.Round(PercentLoaded, GlobalConfig.SigFigs);

            DteqManager.SetDteqPd(this);

            ProtectionDeviceManager.SetProtectionDeviceType(this);
            ProtectionDeviceManager.SetPdTripAndStarterSize(ProtectionDevice);

            SCCR = CalculateSCCR();
            IsCalculating = false;

            OnLoadingCalculated(propertyName);

            OnPropertyUpdated(nameof(CalculateLoading));

        }

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

        public void CreatePowerCable()
        {
            if (PowerCable == null && DaManager.GettingRecords == false) {
                PowerCable = new CableModel(this);
                PowerCable.Load = this;
                PowerCable.LoadId = Id;
                PowerCable.LoadType = this.GetType().ToString();
            }
        }
        public void ValidateCableSizes()
        {
            foreach (var item in CctComponents) {
                item.PowerCable.ValidateCableSize(item.PowerCable);
            }
            PowerCable.ValidateCableSize(PowerCable);
        }
        public void SizePowerCable()
        {
            CreatePowerCable();
            PowerCable.SetSizingParameters(this);
            PowerCable.CreateTypeList(this);
            PowerCable.AutoSizeAll();
        }
        public void CalculateCableAmps()
        {
            PowerCable.CalculateAmpacity(this);
        }


        public override string ToString()
        {
            return Tag;
        }


        public event EventHandler PropertyUpdated;
        public async Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
        {
            if (DaManager.GettingRecords == true) return;
            if (IsCalculating) return;
            Tag = Tag;
            await Task.Run(() => {
                if (PropertyUpdated != null) {
                    PropertyUpdated(this, EventArgs.Empty);
                }
            });
            ErrorHelper.Log($"Tag: {Tag}, {callerMethod}");

            if (GlobalConfig.Testing == true) {
                ErrorHelper.Log($"Tag: {Tag}, {callerMethod}");
            }
        }


        public virtual void OnAssignedLoadReCalculated(object source, CalculateLoadingEventArgs e)
        {
            IEquipment eq = (IEquipment)source;
            if (GlobalConfig.Testing == true) {
                ErrorHelper.Log($"Tag: {Tag}, Load: {eq.Tag}");
            }
            CalculateLoading();
        }

        //Events
        public event EventHandler<CalculateLoadingEventArgs> LoadingCalculated;
        public virtual void OnLoadingCalculated(string propertyName = "")
        {
            if (LoadingCalculated != null) {
                var calcEventArgs = new CalculateLoadingEventArgs() { PropertyName = propertyName };
                LoadingCalculated(this, calcEventArgs);
            }
        }



        public async Task UpdateAreaProperties()
        {
            UndoManager.CanAdd = false;

            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                NemaRating = Area.NemaRating;
                AreaClassification = Area.AreaClassification;

                //when selecting area on AreasView datagrid, PowerCable is null after Db reload
                if (PowerCable != null) {
                    PowerCable.Derating = CableManager.CableSizer.SetDerating(PowerCable);
                    PowerCable.CalculateAmpacity(this);
                }

            }));
            UndoManager.CanAdd = true;

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

        public virtual bool CanAdd(IPowerConsumer load)
        {
            return true;
        }
        /// <summary>
        /// Returns true if the load was added successfully. Return faslse if the load is already assigned to this Dteq.
        /// </summary>
        /// <param name="load"></param>
        /// <returns></returns>
        public virtual bool AddNewLoad(IPowerConsumer load)
        {

            if (load == null) return false;

            //check if load is already assigned
            var iLoad = AssignedLoads.FirstOrDefault(l => l == load);

            if (iLoad == null) {
                AssignedLoads.Add(load);
                return true;
            }
            return false;
        }

        public virtual void RemoveAssignedLoad(IPowerConsumer load)
        {
            if (load != null) {
                AssignedLoads.Remove(load);
                load.LoadingCalculated -= OnAssignedLoadReCalculated;
            }
        }

        public virtual void SetLoadProtectionDevice(IPowerConsumer load)
        {

        }
    }
}
