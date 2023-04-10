using EdtLibrary.Commands;
using EdtLibrary.Models.AdditionalProperties;
using EDTLibrary.Calculators;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Selectors;
using EDTLibrary.UndoSystem;
using EDTLibrary.Validators;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EDTLibrary.Models.Components;

[Serializable]
[AddINotifyPropertyChangedInterface]


//  - Category = CctComponent
//	- Sub-Category = ProtectionDevice, Starter, Disconnect
//	- Type = Breaker, FDS, UDS, DOL, VSD, 
//	- SubType = DefaultDcn,, Diconnect,
public abstract class ComponentModelBase : IComponentEdt
{

    public double AmpacityFactor
    {
        get { return _ampacityFactor; }
        set 
        {

            _ampacityFactor = value;
            if (DaManager.GettingRecords) return;

            CalculateSize((IPowerConsumer)Owner);
            OnPropertyUpdated();
        }
    }
    private double _ampacityFactor = 1.25;

    public ComponentModelBase()
    {
        AutoCalculateCommand = new RelayCommand(AutoCalculate);
    }

    public ICommand AutoCalculateCommand { get; set; }
    private void AutoCalculate()
    {
        var calcLock = IsCalculationLocked;
        IsCalculationLocked= false;
        CalculateSize((IPowerConsumer)Owner);
        IsCalculationLocked = calcLock;
    }


    public int PropertyModelId { get; set; }
    public PropertyModelBase PropertyModel
    {
        get { return _propertyModel; }
        set 
        {
            _propertyModel = value; 
            PropertyModelId = _propertyModel.Id;
        }
    }
    private PropertyModelBase _propertyModel;

    public virtual string Type
    {
        get => _type;
        set
        {
            if (value == null) return;
            if (value == _type) return;
            var oldValue = _type;
            _type = value;

            if (DaManager.Importing) return;
            if (DaManager.GettingRecords) return;

            UndoManager.Lock(this, nameof(Type));

            {
                if (_type == DisconnectTypes.FDS.ToString() || _type == DisconnectTypes.FWDS.ToString()) {
                    var owner = (IPowerConsumer)Owner;
                    if (owner != null) {
                        TripAmps = TypeManager.BreakerTripSizes.FirstOrDefault(f => f.TripAmps >= owner.Fla).TripAmps;
                    }
                }
                PropertyModelManager.DeletePropModel(PropertyModel);
                PropertyModel = PropertyModelManager.CreateNewPropModel(_type, this);
                PropertyModel.Owner = this;
                PropertyModelId = PropertyModel.Id;

            }

            TypeList = ComponentTypeSelector.GetComponentTypeList(this);
            UndoManager.AddUndoCommand(this, nameof(Type), oldValue, _type);
            Validate();
            OnPropertyUpdated();
        }
    }
    private string _type;






    public bool IsAreaLocked
    {
        get { return _isAreaLocked; }
        set
        {
            _isAreaLocked = value;
            OnPropertyUpdated();

        }
    }
    private bool _isAreaLocked;

    public bool IsCalculationLocked
    {
        get { return _isCalculationLocked; }
        set
        {
            _isCalculationLocked = value;
            OnPropertyUpdated();
        }
    }
    private bool _isCalculationLocked;
    public bool IsValid { get; set; } = true;


    public string IsInvalidMessage
    {
        get { 
            return _invalidProtectionDeviceMessage; 
        }
        set
        { 
            _invalidProtectionDeviceMessage = value; 
        }
    }
    private string _invalidProtectionDeviceMessage;

    public void Validate()
    {

        if (DaManager.Importing) return;
        if (DaManager.GettingRecords) return;
        
        IsValid = true;
        IsInvalidMessage = string.Empty;

        ValidateTrip();
        ValidateFrame();
        ValidateAIC();
        ValidateSCCR();
        ValidateStarter();

        if (PowerCable != null) {
            PowerCable.Validate(PowerCable); 
        }


        if (Owner is IDteq) {
            var dteq = (IDteq)Owner;
            dteq.Validate();
        }

        //this would be useful for indication in the load list
        if(Owner is ILoad) {
            var load = (ILoad)Owner;

            load.Validate();   //causes stack overflow because when load is updated all components are validated

            load.FedFrom.Validate();
        }

        //Final message
        if (IsValid == false) {
            IsInvalidMessage = "Validation Failures:" + Environment.NewLine + IsInvalidMessage;
        }
        OnPropertyUpdated();

        return;
    }
    private void ValidateTrip()
    {
        if (this.Type.Contains("VSD") || this.Type.Contains("VFD") || this.Type.Contains("RVS")
            || Type == "UDS") return;

        if (TripAmps < ((IPowerConsumer)Owner).Fla) {
            IsValid = false;
            IsInvalidMessage += Environment.NewLine + " - Trip is less than load FLA";
        }
    }
    private void ValidateFrame()
    {
        if (this.Type.Contains("VSD") || this.Type.Contains("VFD") || this.Type.Contains("RVS")) return;

        if (this.Type == "Disconnect" || this.Type == "UDS" || this.Type == "FDS") {
            if (FrameAmps < ((IPowerConsumer)Owner).Fla) {
                IsValid = false;
                IsInvalidMessage += Environment.NewLine + " - Frame is less than load FLA, or not rated for the motor HP";
            }
            else if(FrameAmps < ((IPowerConsumer)Owner).Fla*1.25) {
                IsValid = false;
                IsInvalidMessage += Environment.NewLine + " - Frame is less than 125% of load FLA, or not rated for the motor HP";
            }
        }
        if (this.Type.Contains("FDS")) {
            if (FrameAmps < TripAmps) {
                IsValid = false;
                IsInvalidMessage += Environment.NewLine + " - Frame is less than Trip";
            } 
        }
        if (this.Type.Contains("Breaker")) {
            if (FrameAmps < TripAmps) {
                IsValid = false;
                IsInvalidMessage += Environment.NewLine + " - Frame is less than Trip";
            }
        }
    }
    private void ValidateAIC()
    {
        if (this.Type.Contains("VSD") || this.Type.Contains("VFD") || this.Type.Contains("RVS")) return;

        if (AIC < SCCA) {
            IsValid = false;
            IsInvalidMessage += Environment.NewLine + " - Device AIC rating is less than SCCA";
        }
    }
    private void ValidateSCCR()
    {
        if (this.Type.Contains("VSD") || this.Type.Contains("VFD") || this.Type.Contains("RVS")) return;

        if (SCCR < SCCA) {
            IsValid = false;
            IsInvalidMessage += Environment.NewLine + " - SCCR is less than SCCA";
        }
    }

    private void ValidateStarter()
    {
        if (StarterSize == null) return;
        if (this.Type.Contains("DOL") || this.Type.Contains("MCP") ) {
            var validStarter = TypeManager.GetStarter( ((LoadModel)this.Owner).Size, ((LoadModel)Owner).Unit);
            var selectedStarter = TypeManager.StarterSizes.FirstOrDefault(ss => ss.Size == StarterSize && ss.Unit == ((LoadModel)Owner).Unit);
            if (validStarter != null && selectedStarter.SizeNumeric < validStarter.SizeNumeric) {
                IsValid = false;
                IsInvalidMessage += Environment.NewLine + " - Starter is undersized";
            }
        }

    }
    private string _starterSize;

    public string StarterSize
    {
        get { return _starterSize; }
        set 
        {

            _starterSize = value;

            if (DaManager.GettingRecords) return;

            Validate();
            OnPropertyUpdated();
        }
    }

    public bool IsSelected { get; set; } = false;

    public int Id { get; set; }

    public bool SettingTag { get; set; } = false;
    public string Tag
    {
        get { return _tag; }
        set
        {
            if (value == null || value == _tag) return;
            if (TagAndNameValidator.IsTagAvailable(value, ScenarioManager.ListManager) == false) {
                return;
            }
            var oldValue = _tag;
            _tag = value;

            if (DaManager.GettingRecords == true) return;

            if (Owner != null) {
                
                CableManager.UpdateComponentCableTags((IPowerConsumer)Owner);
                if (Owner is LoadModel) {
                    CableManager.UpdateLcsCableTags((LoadModel)Owner);
                }
            }

            UndoManager.AddUndoCommand(this, nameof(Tag), oldValue, _tag);
            OnPropertyUpdated();
        }
    }
    private string _tag;



    public string Description { get; set; }
    public string Category { get; set; } //Component
    public string SubCategory { get; set; }
    

    
    public List<string> TypeList
    {
        get
        {
            if (_typeList==null || _typelist.Count==0) {
                return ComponentTypeSelector.GetComponentTypeList(this);
            } 
            return _typeList;
        }
        set { _typeList = ComponentTypeSelector.GetComponentTypeList(this); }
    }
    public List<string> _typeList;

    public string SubType { get; set; }

   
    public double Voltage { get; set; }

    public double SCCA 
    {
        get { return _scca; } 
        set
        { 
            _scca = value;

            SCCR = EquipmentSccrCalculator.GetMinimumSccr(this);
            AIC = EquipmentSccrCalculator.GetMinimumAicRating(this);
            Validate();
            OnPropertyUpdated();
        }
    }
    private double _scca;
    public double SCCR
    {
        get { return _sccr; }
        set
        {
            var oldValue = _sccr;
            _sccr = value;

            if (DaManager.GettingRecords) return;
            UndoManager.Lock(this, nameof(SCCR));

         


            UndoManager.AddUndoCommand(this, nameof(SCCR), oldValue, value);
            Validate();

            OnPropertyUpdated();
        }
    }
    private double _sccr;

    public double AIC
    {
        get { return _aic; }
        set { 
            _aic = value;
            Validate();
            OnPropertyUpdated();
        }
    }
    private double _aic;

    public double TripAmps
    {
        get => _trip;
        set
        {

            var oldValue = _trip;
            _trip = value;

            if (DaManager.GettingRecords) return;

            if (IsCalculationLocked == false) {
                FrameAmps = ProtectionDeviceManager.GetPdFrameAmps(this, (IPowerConsumer)Owner);

            }            var pdLoad = (IPowerConsumer)Owner;
            pdLoad.ValidateCableSizes();

            UndoManager.AddUndoCommand(this, nameof(TripAmps), oldValue, _trip);
            Validate();
            OnPropertyUpdated();
        }
    }
    public double FrameAmps
    {
        get => _size;
        set 
        {

            var oldValue = _size;
            _size = value;

            UndoManager.AddUndoCommand(this, nameof(FrameAmps), oldValue, _size);
            Validate();
            OnPropertyUpdated();
        }
    }
   

    public int AreaId { get; set; }
    private int _sequenceNumber;
    private List<string> _typelist = new List<string>();


    public IArea Area
    {
        get { return _area; }
        set
        {
            var oldValue = _area;
            _area = value;
            if (_area != null) {

                UndoManager.Lock(this, nameof(Area));
                AreaManager.UpdateArea(this, _area, oldValue);
                UndoManager.AddUndoCommand(this, nameof(Area), oldValue, _area);

                OnPropertyUpdated();
            }

        }
    }
    private IArea _area;
    public string NemaRating
    {
        get => _nemaRating;
        set
        {
            if (value == null) return;

            var oldValue = _nemaRating;
            _nemaRating = value;

            UndoManager.AddUndoCommand(this, nameof(NemaRating), oldValue, _nemaRating);
            OnPropertyUpdated();
        }
    }
    private string _nemaRating;
    private double _size;
    private double _trip;

    public string AreaClassification
    {
        get => _areaClassification;
        set
        {
            if (value == null) return;

            var oldValue = _areaClassification;
            _areaClassification = value;

            UndoManager.AddUndoCommand(this, nameof(AreaClassification), oldValue, _areaClassification);
            OnPropertyUpdated();
        }
    }
    private string _areaClassification;
    public int OwnerId { get; set; }
    public string OwnerType { get; set; }
    public IEquipment Owner { get; set; }
    public int SequenceNumber
    {
        get => _sequenceNumber;
        set
        {
            _sequenceNumber = value;
            OnPropertyUpdated();
        }
    }

    public CableModel PowerCable { get; set; }


    public double HeatLoss { get; set; }

    public void CalculateSize(IPowerConsumer load)
    {
        if (Type == CctComponentTypes.UDS.ToString()) {
            TripAmps = DataTableSearcher.GetDisconnectFuse(load);
        }
        else {
            ProtectionDeviceManager.SetProtectionDevice(this);
        }

        if (Type == CctComponentTypes.UDS.ToString() || Type == CctComponentTypes.FDS.ToString()) {
            FrameAmps = DataTableSearcher.GetDisconnectSize(load);
        }



        AIC = ProtectionDeviceAicCalculator.GetMinimumBreakerAicRating(load);
        SCCR = EquipmentSccrCalculator.GetMinimumSccr(this);
        Validate();
        OnPropertyUpdated();
    }

    public override string ToString()
    {
        return Tag;
    }

    public async Task UpdateAreaProperties()
    {
        if (IsAreaLocked) return; 
        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            NemaRating = Area.NemaRating;
            AreaClassification = Area.AreaClassification;
        }));

    }

    public event EventHandler AreaChanged;
    public virtual async Task OnAreaChanged()
    {
        await Task.Run(() => {
            if (AreaChanged != null) {
                UndoManager.CanAdd = false;
                AreaChanged(this, EventArgs.Empty);
                UndoManager.CanAdd = true;
            }
        });
    }

    public void MatchOwnerArea(object source, EventArgs e)
    {
        if (IsAreaLocked) return;
        IEquipment owner = (IEquipment)source;
        UndoManager.CanAdd = false;
        AreaManager.UpdateArea(this, owner.Area, Area);
        UndoManager.CanAdd = true;
        OnPropertyUpdated();
    }


    public event EventHandler PropertyUpdated;

    public async Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
    {
        if (DaManager.GettingRecords == true) return;
        if (DaManager.Importing == true) return;

        //if (PropertyUpdated != null) {
        //    PropertyUpdated(this, EventArgs.Empty);
        //}

        await Task.Run(() => {
            if (PropertyUpdated != null) {
                PropertyUpdated(this, EventArgs.Empty);
            }
        });
    }
}
