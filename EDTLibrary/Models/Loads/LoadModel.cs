using EdtLibrary.Commands;
using EDTLibrary.Calculators;
using EDTLibrary.DataAccess;
using EDTLibrary.DistributionControl;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Calculations;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Selectors;
using EDTLibrary.UndoSystem;
using EDTLibrary.Validators;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace EDTLibrary.Models.Loads
{

    [Serializable]
    [AddINotifyPropertyChangedInterface]

    public class LoadModel : ILoad
    {


        public LoadModel()
        {
            Description = "";
            Category = Categories.LOAD.ToString();
            PowerCable = new CableModel();
            CalculationFlags = new CalculationFlags();

            //Commands

            ResetDefaultEfficiencyAndPowerFactorCommand = new RelayCommand(ResetDefaultEfficiencyAndPowerFactor);

        }

        #region Commands
        public ICommand ResetDefaultEfficiencyAndPowerFactorCommand { get; set; }
        private void ResetDefaultEfficiencyAndPowerFactor()
        {
            EfficiencyAndPowerFactorSelector.SetEfficiencyAndPowerFactor(this);
        }


        #endregion

        public CalculationFlags CalculationFlags { get; set; }
        internal SaveController saveController = new SaveController();




        public int ProtectionDeviceId { get; set; }
        public IProtectionDevice ProtectionDevice
        {
            get => _protectionDevice;
            set
            {
                _protectionDevice = value;
                if (_protectionDevice == null) return;
                ProtectionDeviceId = _protectionDevice.Id;
                FedFrom.SetLoadProtectionDevice(this);
            }
        }
        private IProtectionDevice _protectionDevice;

        public bool IsValid { get; set; } = true;

        public bool Validate()
        {
            if (DaManager.GettingRecords) return false;
            var isValid = true;

            if (ProtectionDevice != null) {
                isValid = ProtectionDevice.Validate();
            }

            if (PowerCable != null) {
                PowerCable.Validate(PowerCable);
            }

            foreach (var comp in CctComponents) {
                comp.Validate();
            }


            IsValid = isValid;
            FedFrom.Validate();
            OnPropertyUpdated();

            return isValid;
        }


        public bool IsSelected { get; set; } = false;
        private bool allowCalculations = true;
        public int Id { get; set; }
        private string _tag;

        public override string ToString()
        {
            return Tag;
        }
        public string Tag
        {
            get { return _tag; }
            set
            {
                //Cancel conditions
                if (value == null || value == _tag) return;
                if (string.IsNullOrEmpty(value.ToString())) return;
                if (Tag == GlobalConfig.LargestMotor_StartLoad) return;

                if (CalculationFlags.EnforceUniqueTags == true) {
                    if (TagAndNameValidator.IsTagAvailable(value, ScenarioManager.ListManager) == false) {
                        return;
                    }
                }
                
                

                var oldValue = _tag;
                _tag = value;

                if (DaManager.GettingRecords == true) return;
                saveController.Lock(nameof(Tag));
                UndoManager.Lock(this, nameof(Tag));

                if (PowerCable != null) {
                    PowerCable.SetSourceAndDestinationTags(this);
                }
                if (PowerCable != null && FedFrom != null) {
                    if (CableManager.IsUpdatingCables == false) {
                        CableManager.AddAndUpdateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);
                    }
                }


                saveController.UnLock(nameof(Tag));
                UndoManager.AddUndoCommand(this, nameof(Tag), oldValue, _tag);
                OnPropertyUpdated();

            }
        }

        public string Category { 
            get; 
            set; }

        public string Type
        {
            get  { return _type; }
            set
            {
                var oldValue = _type; 
                _type = value;
                

                if (DaManager.GettingRecords) return;
                if (DaManager.Importing) return;
                if (VoltageType == null) return;

                saveController.Lock(nameof(Type));
                UndoManager.Lock(this, nameof(Type));

                {
                    allowCalculations = false;
                    LoadUnitSelector.SelectUnit(this);
                    ProtectionDeviceManager.SetProtectionDeviceType(this);
                    allowCalculations = true;

                    CalculateLoading();
                    PowerCable.AutoSizeAll_IfEnabled();
                }
                UndoManager.AddUndoCommand(this, nameof(Type),oldValue,_type);
                saveController.UnLock(nameof(Type));
                OnPropertyUpdated();
            }
        }
        public string _type;

        public string SubType
        {
            get { return _subType; }
            set
            {
                if (value == null) return;

                var oldValue = _subType;
                _subType = value;
                if (DaManager.GettingRecords) return;

                OnPropertyUpdated();
            }
        }
        private string _subType;

        public double Size
        {
            get { return _size; }
            set
            {
                if (value == null) return;

                double oldValue = _size;
                _size = value;

                if (DaManager.Importing) return;
                if (DaManager.GettingRecords) return;
                saveController.Lock(nameof(Size));
                UndoManager.Lock(this, nameof(Size));

                {
                    CalculateLoading();
                    PowerCable.AutoSizeAll_IfEnabled();
                }

                UndoManager.AddUndoCommand(this, nameof(Size), oldValue, _size);
                saveController.UnLock(nameof(Size));
                OnPropertyUpdated();
            }
        }
        public double _size;

        public string Unit
        {
            get { return _unit; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ) {
                    return;
                }

                else {
                    var oldValue = _unit; 
                    _unit = value;

                    if (DaManager.GettingRecords) return;
                    saveController.Lock(nameof(Unit));
                    UndoManager.Lock(this, nameof(Unit));

                        CalculateLoading();
                        PowerCable.AutoSizeAll_IfEnabled();

                    UndoManager.AddUndoCommand(this, nameof(Unit), oldValue, _unit);
                    saveController.UnLock(nameof(Unit));
                    OnPropertyUpdated(nameof(Unit));
                }
            }
        }
        private string _unit;


        public List<string> UnitList
        {
            get { return LoadUnitSelector.GetUnitList(this); }
        }

        public string Description
            {
                get { return _description; }
                set
                {
                    if (value == _description) return;
                    var oldValue = _description;
                    _description = value;

                    if (Tag != null) {
                        UndoManager.AddUndoCommand(this, nameof(Description), oldValue, _description);
                    }
                    OnPropertyUpdated();
                }
            }
        private string _description;


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

                if (DaManager.GettingRecords) return;
                saveController.Lock(nameof(Area));
                UndoManager.Lock(this, nameof(Area));

                    AreaManager.UpdateArea(this, _area, oldValue);

                    if (PowerCable != null && FedFrom != null) {
                        PowerCable.Derating = CableManager.CableSizer.SetDerating(PowerCable);
                        PowerCable.CalculateAmpacity(this);
                    }

                UndoManager.AddUndoCommand(this, nameof(Area), oldValue, _area);
                saveController.UnLock(nameof(Area));
                OnPropertyUpdated();

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
                OnPropertyUpdated();
            }
        }
        private string _nemaRating;

        public string AreaClassification
        {
            get { return _areaClassification; }
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

        public int SequenceNumber
        {
            get => _sequenceNumber;
            set
            {
                _sequenceNumber = value;
                OnPropertyUpdated();
            }
        }
        private int _sequenceNumber;

        public string PanelSide
        {
            get { return _panelSide; }
            set
            {
                _panelSide = value;
                OnPropertyUpdated();
            }
        }
        public string _panelSide;

        public double Voltage
        {
            get { return _voltage; }
            set
            {
                if (value == null) return;

                var oldValue = _voltage;
                _voltage = value;

                UndoManager.AddUndoCommand(this, nameof(Voltage), oldValue, _voltage);

                if (DaManager.GettingRecords == false) {
                    PowerCable.CreateTypeList(this);
                }
                OnPropertyUpdated();

            }
        }
        private double _voltage;

        public int CircuitNumber
        {
            get { return _circuitNumber; }
            set { _circuitNumber = value; }
        }
        private int _circuitNumber;

        public int VoltageTypeId { get; set; } = 0;

        public VoltageType VoltageType
        {
            get { return _voltageType; }
            set
            {
                if (value == null) return;
                var oldValue = _voltageType;
                _voltageType = value;
               
                if (Type == LoadTypes.MOTOR.ToString()) {
                    _voltageType = _voltageType.Voltage == 600 ? TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==575) : _voltageType;
                    _voltageType = _voltageType.Voltage == 480 ? TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage==460) : _voltageType;

                }
                if (DaManager.Importing) return;
                if (DaManager.GettingRecords) return;
                saveController.Lock(nameof(VoltageType));
                UndoManager.Lock(this, nameof(VoltageType));

                {
                    VoltageTypeId = _voltageType.Id;
                    Voltage = _voltageType.Voltage;
                    CalculateLoading(nameof(VoltageType));
                    PowerCable.AutoSizeAll_IfEnabled();
                }

                UndoManager.AddUndoCommand(this, nameof(VoltageType), oldValue, _voltageType);
                saveController.UnLock(nameof(VoltageType));
               
                OnPropertyUpdated(nameof(VoltageType));
            }
        }
        private VoltageType _voltageType;

        

        public int FedFromId { get; set; }

        public string FedFromType { get; set; }

        private string _fedFromTag;
        public string FedFromTag
        {
            get { return _fedFromTag; }
            set
            {
                if (value == null) return;

                _fedFromTag = value;
                if (DaManager.GettingRecords == false) {
                    //OnFedFromChanged();
                    //CalculateLoading();
                    CreatePowerCable();
                    PowerCable.SetSourceAndDestinationTags(this);
                }
            }
        }

        public IDteq FedFrom
        {
            get { return _fedFrom; }
            set
            {
                if (value == null) return;
                if (value == _fedFrom) return;

                var oldValue = _fedFrom;
                var newFedFrom = value;

                if (DaManager.GettingRecords) {
                    _fedFrom = newFedFrom;
                    return;
                }
                if (FedFromManager.IsUpdatingFedFrom_List) return;
                saveController.Lock(nameof(FedFrom));
                UndoManager.Lock(this, nameof(FedFrom));


                if (FedFromManager.UpdateFedFrom_Single(this, newFedFrom, oldValue) == true) {
                    _fedFrom = newFedFrom;
                    CableManager.AddAndUpdateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);
                    CableManager.UpdateLcsCableTags(this);
                    PowerCable.AutoSizeAll_IfEnabled();

                    UndoManager.AddUndoCommand(this, nameof(FedFrom), oldValue, _fedFrom);
                }


                saveController.UnLock(nameof(FedFrom));
                OnPropertyUpdated();
            }
        }
        private IDteq _fedFrom;

        public double DemandFactor
        {
            get { return _demandFactor; }
            set
            {
                var oldValue = _demandFactor;
                _demandFactor = (value <= 1 && value >= 0)? value : oldValue;

                if (DaManager.GettingRecords) return;
                saveController.Lock(nameof(DemandFactor));
                UndoManager.Lock(this, nameof(DemandFactor));

                {
                    CalculateLoading();
                    PowerCable.AutoSizeAll_IfEnabled();
                }

                UndoManager.AddUndoCommand(this, nameof(DemandFactor), oldValue, _demandFactor);
                saveController.UnLock(nameof(DemandFactor));
                OnPropertyUpdated();
            }
        }
        private double _demandFactor;

        public double Efficiency
        {
            get { return _efficiency; }
            set
            {
                var oldValue = _efficiency;
                _efficiency = (value <= 1  && value >= 0) ? value: oldValue;
                EfficiencyDisplay = _efficiency * 100;

                if (DaManager.GettingRecords) return;
                saveController.Lock(nameof(Efficiency));
                UndoManager.Lock(this,nameof(Efficiency));

                {
                    if (IsCalculating == false) {
                        CalculateLoading();
                        PowerCable.AutoSizeAll_IfEnabled();

                    }
                }

                UndoManager.AddUndoCommand(this, nameof(Efficiency), oldValue, _efficiency);
                saveController.UnLock(nameof(Efficiency));
                OnPropertyUpdated(nameof(Efficiency));
            }
        }
        private double _efficiency;

        public double EfficiencyDisplay
        {
            get { return _efficiencyDisplay; }
            set { _efficiencyDisplay = Math.Round(value, 3); }
        }
        public double _efficiencyDisplay;

        public double PowerFactor
        {
            get { return Math.Round(_powerFactor * 100, 2); }
            set
            {
                var oldValue = _powerFactor;
                _powerFactor = (value <= 1 && value >= 0) ? value / 100 : oldValue;
                if (DaManager.GettingRecords) return;

                saveController.Lock(nameof(PowerFactor));
                UndoManager.Lock(this, nameof(PowerFactor));

                {
                    if (IsCalculating == false) {
                        CalculateLoading();
                        PowerCable.AutoSizeAll_IfEnabled();

                    }
                }

                UndoManager.AddUndoCommand(this, nameof(PowerFactor), oldValue, _efficiency);
                saveController.UnLock(nameof(PowerFactor)); 
                OnPropertyUpdated(nameof(PowerFactor));
            }
        }
        private double _powerFactor;

        public double AmpacityFactor { get; set; }

        public double Fla { get; set; }

        public double ConnectedKva { get; set; }

        public double DemandKva { get; set; }

        public double DemandKw { get; set; }

        public double DemandKvar { get; set; }

        public double RunningAmps { get; set; }

        //Sizing
        public string PdType
        {
            get => _pdType;
            set
            {
                _pdType = value;
                OnPropertyUpdated();
            }
        }
        private string _pdType;

        public double PdSizeTrip
        {
            get;
            set;
        }

        public double PdSizeFrame
        {
            get;
            set;
        }

        public string StarterType
        {
            get;
            set;
        }

        public string StarterSize
        {
            get;
            set;
        }

        public double HeatLoss { get; set; }

        //Cables

        public CableModel PowerCable { get; set; }


        //not used - only here to prevent a xaml binding failure for when using the interface IPowerConsumer
        public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer>{new DummyLoad()};

        public ObservableCollection<IComponentEdt> AuxComponents { get; set; } = new ObservableCollection<IComponentEdt>();

        public ObservableCollection<IComponentEdt> CctComponents { get; set; } = new ObservableCollection<IComponentEdt>();



        //Components
        public IComponentEdt Disconnect
        {
            get;
            set;
        }

        public bool DisconnectBool
        {
            get { return _disconnectBool; }
            set
            {
                var oldValue = _disconnectBool;
                _disconnectBool = value;
                if (DaManager.GettingRecords) return;
                saveController.Lock(nameof(DisconnectBool));
                UndoManager.Lock(this, nameof(DisconnectBool));

                    if (_disconnectBool == true) {
                        ComponentManager.AddDefaultDisconnect(this, ScenarioManager.ListManager);
                    }
                    else if (_disconnectBool == false) {
                        ComponentManager.RemoveDefaultDisconnect(this, ScenarioManager.ListManager);
                    }
                    CableManager.AddAndUpdateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);
                
                UndoManager.AddUndoCommand(this, nameof(DisconnectBool), oldValue, _disconnectBool);
                saveController.UnLock(nameof(DisconnectBool));
                OnPropertyUpdated();

            }
        }
        private bool _disconnectBool;

        public int DisconnectId
        {
            get { return _disconnectId; }
            set { _disconnectId = value; }
        }
        private int _disconnectId;

        public ILocalControlStation Lcs { get; set; }

        public bool LcsBool
        {
            get { return _lcsBool; }
            set
            {
                var oldValue = _lcsBool;
                _lcsBool = value;

                if (DaManager.GettingRecords) return;
                saveController.Lock(nameof(LcsBool));

                UndoManager.Lock(this, nameof(LcsBool));

                    if (_lcsBool == true) {
                        ComponentManager.AddLcs(this, ScenarioManager.ListManager);
                        Lcs.UpdateTypelist(StandAloneStarterBool);
                        if (ProtectionDevice.Type == StarterTypes.VSD.ToString()
                             || ProtectionDevice.Type == StarterTypes.VFD.ToString()
                             || ProtectionDevice.Type == StarterTypes.RVS.ToString()) {

                            CableManager.CreateLcsAnalogCableForProtectionDevice(this, ScenarioManager.ListManager);
                        }
                    }
                    else if (_lcsBool == false) {
                        CableManager.DeleteLcsAnalogCable(this.Lcs, ScenarioManager.ListManager);
                        ComponentManager.DeleteLcs(this, ScenarioManager.ListManager);
                        if (ProtectionDevice.Type != StarterTypes.VSD.ToString()
                             && ProtectionDevice.Type != StarterTypes.VFD.ToString()
                             && ProtectionDevice.Type != StarterTypes.RVS.ToString()) {

                            CableManager.DeleteLcsAnalogCable(this.Lcs, ScenarioManager.ListManager);

                        }
                    }

                UndoManager.AddUndoCommand(this, nameof(LcsBool), oldValue, _lcsBool);
                saveController.UnLock(nameof(LcsBool));
                OnPropertyUpdated();
            }
        }
        private bool _lcsBool;


        public IComponentEdt StandAloneStarter { get; set; }
        public bool StandAloneStarterBool
        {
            get { return _standAloneStarterBool; }
            set
            {
                var oldValue = _standAloneStarterBool;
                _standAloneStarterBool = value;

                if (DaManager.GettingRecords) return;
                saveController.Lock(nameof(StandAloneStarterBool));
                UndoManager.Lock(this, nameof(StandAloneStarterBool));

                ProtectionDeviceManager.SetProtectionDeviceType(this);
                ProtectionDeviceManager.SetPdTripAndStarterSize(ProtectionDevice);

                if (Lcs!=null) {
                        Lcs.Type = ComponentFactory.GetLcsType(this);
                        Lcs.TypeModel = TypeManager.GetLcsTypeModel(Lcs.TypeId);
                    }

                    if (_standAloneStarterBool == true) {
                        //PdType = "BKR";
                        ComponentManager.AddStandAloneStarter(this, ScenarioManager.ListManager);
                        CableManager.CreateLcsAnalogCable(this, ScenarioManager.ListManager);

                        StandAloneStarter = StandAloneStarter;
                    }

                    else if (_standAloneStarterBool == false) {
                        //PdType = EdtSettings.LoadDefaultPdTypeLV_Motor;
                        ComponentManager.RemoveStandAloneStarter(this, ScenarioManager.ListManager);
                        CableManager.DeleteLcsAnalogCable(Lcs, ScenarioManager.ListManager);
                    }
                    CableManager.AddAndUpdateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);
                    CableManager.UpdateLcsCableTags(this);

                if (LcsBool) {
                    Lcs.UpdateTypelist(_standAloneStarterBool);
                    Lcs.Type = ComponentFactory.GetLcsType(this);
                }

                UndoManager.AddUndoCommand(this, nameof(StandAloneStarterBool), oldValue, _standAloneStarterBool);
                saveController.UnLock(nameof(StandAloneStarterBool));
                OnPropertyUpdated();
            }

        }
        private bool _standAloneStarterBool;

        public int StandAloneStarterId { get; set; }

        public IComponentEdt SelectedComponent { get; set; }


        public bool IsCalculating { get; set; }


        public double SCCA
        {
            get {
                if (FedFrom == null) {
                    return 0;
                }
                return FedFrom.SCCA; }
            set { _scca = value; }
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
                saveController.Lock(nameof(SCCR));
                UndoManager.Lock(this, nameof(SCCR));

               

                UndoManager.AddUndoCommand(this, nameof(SCCR), oldValue, value);
                saveController.UnLock(nameof(SCCR));
                OnPropertyUpdated();
            }
        }
        private double _sccr;


        //Methods
        public void CalculateLoading(string propertyName = "")
        {
            if (allowCalculations == false) return;
            UndoManager.CanAdd = false;
            if (DaManager.GettingRecords) return;


            try {
               
                IsCalculating = true;

                // Ampacity Factor
                switch (Type) {
                    case "MOTOR":
                        AmpacityFactor = 1.25;
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
                        AmpacityFactor = 1.25;
                        break;
                }

                //ConnectedKva
                switch (Type) {
                    case "MOTOR":
                        AmpacityFactor = 1.25;
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
                        AmpacityFactor = 1.25;
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

                            case "A":
                                ConnectedKva = Size * VoltageType.Voltage * Math.Sqrt(VoltageType.Phase) / 1000;
                                Fla = Size;
                                break;
                        }
                        break;

                    case "PANEL":
                        switch (Unit) {
                            case "kVA":
                                ConnectedKva = Size;
                                break;

                            case "kW":
                                ConnectedKva = Size / Efficiency / PowerFactor;
                                break;

                            case "A":
                                ConnectedKva = Size * VoltageType.Voltage * Math.Sqrt(3) / 1000; //   / Efficiency / PowerFactor;
                                Fla = Size;
                                break;
                        }
                        break;
                }

                if (ConnectedKva >= 9999999) {
                    ConnectedKva = 9999999;
                }

                //FLA

                if (Unit != "A") {
                    Fla = ConnectedKva * 1000 / VoltageType.Voltage / Math.Sqrt(3);
                    Fla = Math.Round(Fla, GlobalConfig.SigFigs);
                }


                //Power
                ConnectedKva = Math.Round(ConnectedKva, GlobalConfig.SigFigs);
                DemandKva = Math.Round(ConnectedKva * DemandFactor, GlobalConfig.SigFigs);
                DemandKw = Math.Round(DemandKva * PowerFactor, GlobalConfig.SigFigs);
                DemandKvar = Math.Round(DemandKva * (1 - PowerFactor), GlobalConfig.SigFigs);

               
                ProtectionDeviceManager.SetPdTripAndStarterSize(ProtectionDevice);

                if (ProtectionDevice != null) {
                    ProtectionDevice.AIC = ProtectionDeviceAicCalculator.GetMinimumBreakerAicRating(this);
                }
                SCCR = EquipmentSccrCalculator.GetMinimumSccr(this);

                PowerCable.GetRequiredAmps(this);
                UndoManager.CanAdd = true;

                IsCalculating = false;

                OnLoadingCalculated(propertyName);

                PowerCable.Validate(PowerCable);
                CableManager.ValidateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);

                foreach (var item in CctComponents) {
                    item.CalculateSize(this);
                }
            }

            catch (Exception ex) {

                ErrorHelper.SendExeptionMessage(ex);
            }

            OnPropertyUpdated();

        }

        public void CreatePowerCable()
        {
            if (PowerCable.Load == null) {
                PowerCable = new CableModel(this);
            }
        }
        public void ValidateCableSizes()
        {
            foreach (var item in CctComponents) {
                if (item.PowerCable != null) {
                    item.PowerCable.Validate(item.PowerCable);
                }
            }
            if (PowerCable != null) {
                PowerCable.Validate(PowerCable);
            }
        }

        public void SizePowerCable()
        {
            CreatePowerCable();
            PowerCable.SetSizingParameters(this);
            PowerCable.CreateTypeList(this);
            PowerCable.AutoSizeAll_IfEnabled();
        }
        public void CalculateCableAmps()
        {
            PowerCable.CalculateAmpacity(this);
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

      

        public event EventHandler AreaChanged;
        public virtual async Task OnAreaChanged()
        {
            await Task.Run(() => {
                if (AreaChanged != null) {
                    AreaChanged(this, EventArgs.Empty);
                }
            });
        }


        public event EventHandler CctComponentChanged;
        public virtual void OnCctComponentChanged()
        {
            if (CctComponentChanged != null) {
                CctComponentChanged(this, EventArgs.Empty);
            }
        }

        public async Task UpdateAreaProperties()
        {
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                NemaRating = Area.NemaRating;
                AreaClassification = Area.AreaClassification;
                PowerCable.Derating = CableManager.CableSizer.SetDerating(PowerCable);
                PowerCable.CalculateAmpacity(this);
                if (StandAloneStarter != null) {
                    StandAloneStarter.Area = FedFrom.Area;
                }
                if (Disconnect != null) {
                    Disconnect.Area = Area;
                }
            }));

        }

        public void MatchOwnerArea(object source, EventArgs e)
        {
            throw new NotImplementedException();
        }


        public bool CanSave { get; set; } = true;
        public event EventHandler PropertyUpdated;
        public async Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
        {

            try {
                if (DaManager.GettingRecords == true) return;
                if (DaManager.Importing == true) return;
                if (IsCalculating) return;
                if (CanSave == false) return;

                var tag = Tag;

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
            catch (Exception) {

                throw;
            }

        }


    }
}
