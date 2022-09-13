using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Validators;
using EDTLibrary.ProjectSettings;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.Models.Loads
{

    [AddINotifyPropertyChangedInterface]
    public class LoadModel : ILoad
    {
        public LoadModel()
        {
            Description = "";
            Category = Categories.LOAD.ToString();
            PowerCable = new CableModel();

        }
        public LoadModel(string tag)
        {
            Tag = tag;
        }
        //Properties
        public int Id { get; set; }
        private string _tag;
        public string Tag
        {
            get { return _tag; }
            set
            {
                //Cancel conditions
                if (value == null || value == _tag) return;
                if (string.IsNullOrEmpty(value.ToString())) return;
                if (Tag == GlobalConfig.LargestMotor_StartLoad) return;

                if (TagAndNameValidator.IsTagAvailable(value, ScenarioManager.ListManager) == false) {
                    ErrorHelper.NotifyUserError(ErrorMessages.DuplicateTagMessage, "Duplicate Tag Error", image: MessageBoxImage.Exclamation);
                    return;
                }

                var oldValue = _tag;
                _tag = value;
                if (DaManager.GettingRecords == true) return;


                UndoManager.CanAdd = false;
                if (PowerCable != null) {
                    PowerCable.SetSourceAndDestinationTags(this);
                }
                if (PowerCable != null && FedFrom != null) {
                    if (CableManager.IsUpdatingPowerCables == false) {
                        CableManager.AddAndUpdateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);
                    }
                }

                if (Tag == GlobalConfig.LargestMotor_StartLoad) return;

                UndoManager.CanAdd = true;
                UndoManager.AddUndoCommand(this, nameof(Tag), oldValue, _tag);
                OnPropertyUpdated();

            }
        }
        public string Category { get; set; }
        public string Type { get; set; }

        private string _subType;
        public string SubType
        {
            get { return _subType; }
            set
            {
                if (value == null) return;

                var oldValue = _subType;
                _subType = value;
                if (DaManager.GettingRecords == false) {

                }

                OnPropertyUpdated();
            }
        }


        private string _description;
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

                UndoManager.CanAdd = false;
                UndoManager.Lock(this, nameof(Area));

                {
                    AreaManager.UpdateArea(this, _area, oldValue);

                    if (DaManager.GettingRecords == false && PowerCable != null && FedFrom != null) {
                        PowerCable.Derating = CableManager.CableSizer.SetDerating(PowerCable);
                        PowerCable.CalculateAmpacity(this);
                    }
                }

                //OnAreaChanged(); the changes this event is responsible for are done in  AreaManager.UpdateArea
                UndoManager.CanAdd = true;
                UndoManager.AddUndoCommand(this, nameof(Area), oldValue, _area);

                OnPropertyUpdated();

            }
        }
        private string _nemaRating;
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
        private string _areaClassification;

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


        private double _voltage;

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


        public double _size;
        public double Size
        {
            get { return _size; }
            set
            {
                if (value == null) return;

                double oldValue = _size;
                _size = value;

                UndoManager.CanAdd = true;
                UndoManager.AddUndoCommand(this, nameof(Size), oldValue, _size);
                CalculateLoading();

            }
        }
        public string Unit { get; set; }
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

        private IDteq _fedFrom;
        public IDteq FedFrom
        {
            get { return _fedFrom; }
            set
            {
                if (value == null) return;

                IDteq oldValue = _fedFrom;
                _fedFrom = value;

                UndoManager.CanAdd = false;
                UndoManager.Lock(this, nameof(FedFrom));

                if (DaManager.GettingRecords == false) {
                    DistributionManager.UpdateFedFrom(this, _fedFrom, oldValue);
                    CableManager.AddAndUpdateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);
                }
                UndoManager.CanAdd = true;
                UndoManager.AddUndoCommand(this, nameof(FedFrom), oldValue, _fedFrom);
                OnPropertyUpdated();
            }
        }

        private double _loadFactor;
        public double LoadFactor
        {
            get { return _loadFactor; }
            set
            {

                var oldValue = _loadFactor;
                _loadFactor = value;

                UndoManager.CanAdd = true;
                UndoManager.AddUndoCommand(this, nameof(LoadFactor), oldValue, _loadFactor);
                OnPropertyUpdated();
            }
        }

        private double _efficiency;
        public double Efficiency
        {
            get { return _efficiency ; }
            set
            {
                var oldValue = _efficiency;
                _efficiency = value;
                EfficiencyDisplay = _efficiency * 100;
                UndoManager.AddUndoCommand(this, nameof(Efficiency), oldValue, _efficiency);
                OnPropertyUpdated(nameof(Efficiency));
            }
        }

        public double _efficiencyDisplay;
        public double EfficiencyDisplay
        {
            get { return _efficiencyDisplay; }
            set {  _efficiencyDisplay = Math.Round(value,3);}
        }

        private double _powerFactor;
        public double PowerFactor
        {
            get { return Math.Round(_powerFactor * 100, 2); }
            set
            {
                var oldValue = _powerFactor;
                _powerFactor = value / 100;

                UndoManager.AddUndoCommand(this, nameof(PowerFactor), oldValue, _powerFactor);
                OnPropertyUpdated(nameof(PowerFactor));
            }
        }

        private int _poles = 3;

        public int Poles
        {
            get { return _poles; }
            set { _poles= value; }
        }


        public double AmpacityFactor { get; set; }
        public double Fla { get; set; }
        public double ConnectedKva { get; set; }
        public double DemandKva { get; set; }
        public double DemandKw { get; set; }
        public double DemandKvar { get; set; }
        public double RunningAmps { get; set; }

        //Sizing
        public string PdType { get; set; }
        public double PdSizeTrip { get; set; }
        public double PdSizeFrame { get; set; }
        public BreakerSize BreakerSize { get { return TypeManager.GetBreaker(Fla); } }

        public string StarterType { get; set; }
        public double StarterSize { get; set; }

        public double HeatLoss { get; set; }

        //Cables

        public CableModel PowerCable { get; set; }
        public ObservableCollection<IComponentEdt> AuxComponents { get; set; } = new ObservableCollection<IComponentEdt>();
        public ObservableCollection<IComponentEdt> CctComponents { get; set; } = new ObservableCollection<IComponentEdt>();



        //Components
        public IComponentEdt Disconnect
        {
            get;
            set;
        }

        private bool _disconnectBool;
        public bool DisconnectBool
        {
            get { return _disconnectBool; }
            set
            {
                var oldValue = _disconnectBool;
                _disconnectBool = value;

                UndoManager.Lock(this, nameof(DisconnectBool));
                if (DaManager.GettingRecords == false) {
                    if (_disconnectBool == true) {
                        ComponentManager.AddDefaultDisconnect(this, ScenarioManager.ListManager);
                    }
                    else if (_disconnectBool == false) {
                        ComponentManager.RemoveDefaultDisconnect(this, ScenarioManager.ListManager);
                    }
                    CableManager.AddAndUpdateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);
                    //OnCctComponentChanged();
                }
                UndoManager.CanAdd = true;
                UndoManager.AddUndoCommand(this, nameof(DisconnectBool), oldValue, _disconnectBool);

                OnPropertyUpdated();

            }
        }

        private int _disconnectId;
        public int DisconnectId
        {
            get { return _disconnectId; }
            set { _disconnectId = value; }
        }

        public ILocalControlStation Lcs { get; set; }
        private bool _lcsBool;
        public bool LcsBool
        {
            get { return _lcsBool; }
            set
            {
                var oldValue = _lcsBool;
                _lcsBool = value;


                UndoManager.CanAdd = false;
                UndoManager.Lock(this, nameof(LcsBool));

                if (DaManager.GettingRecords == false) {
                    if (_lcsBool == true) {
                        ComponentManager.AddLcs(this, ScenarioManager.ListManager);
                    }
                    else if (_lcsBool == false) {
                        ComponentManager.RemoveLcs(this, ScenarioManager.ListManager);
                    }
                }

                UndoManager.CanAdd = true;
                UndoManager.AddUndoCommand(this, nameof(LcsBool), oldValue, _lcsBool);
                OnPropertyUpdated();
            }
        }


        public IComponentEdt Drive { get; set; }

        private bool _driveBool;
        public bool DriveBool
        {
            get { return _driveBool; }
            set
            {
                var oldValue = _driveBool;
                _driveBool = value;
                if (_driveBool == true) {
                    PdType = "BKR";
                }
                else if (_driveBool == false) {
                    PdType = EdtSettings.LoadDefaultPdTypeLV_Motor;
                }

                UndoManager.CanAdd = false;
                UndoManager.Lock(this, nameof(DriveBool));

                if (DaManager.GettingRecords == false) {
                    if (_driveBool == true) {
                        ComponentManager.AddDefaultDrive(this, ScenarioManager.ListManager);
                    }
                    else if (_driveBool == false) {
                        ComponentManager.RemoveDefaultDrive(this, ScenarioManager.ListManager);
                    }
                    CableManager.AddAndUpdateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);
                }

                UndoManager.CanAdd = true;
                UndoManager.AddUndoCommand(this, nameof(DriveBool), oldValue, _driveBool);
                OnPropertyUpdated();
            }

        }

        private int _driveId;
        public int DriveId
        {
            get { return _driveId; }
            set { _driveId = value; }
        }


       

        private BreakerSize _breakerSize;

        public bool IsCalculating { get; set; }
        //Methods
        public void CalculateLoading()
        {
           
            UndoManager.CanAdd = false;
            if (DaManager.GettingRecords == true) {
                return;
            }
            if (LoadFactor == null || LoadFactor == 0) {
                LoadFactor = double.Parse(EdtSettings.LoadFactorDefault);
            }

            IsCalculating = true;
            GetEfficiencyAndPowerFactor(this);

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
                            ConnectedKva = Size * Voltage * Math.Sqrt(3) / 1000; //   / Efficiency / PowerFactor;
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
                            ConnectedKva = Size * Voltage * Math.Sqrt(3) / 1000; //   / Efficiency / PowerFactor;
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
                Fla = ConnectedKva * 1000 / Voltage / Math.Sqrt(3);
                Fla = Math.Round(Fla, GlobalConfig.SigFigs);
            }


            //Power
            ConnectedKva = Math.Round(ConnectedKva, GlobalConfig.SigFigs);
            DemandKva = Math.Round(ConnectedKva * LoadFactor, GlobalConfig.SigFigs);
            DemandKw = Math.Round(DemandKva * PowerFactor, GlobalConfig.SigFigs);
            DemandKvar = Math.Round(DemandKva * (1 - PowerFactor), GlobalConfig.SigFigs);

            LoadManager.SetLoadPd(this);
            LoadManager.SetLoadPdSize(this);
            PowerCable.GetRequiredAmps(this);
            UndoManager.CanAdd = true;

            IsCalculating = false;
            OnLoadingCalculated();
            PowerCable.ValidateCableSize(PowerCable);
            CableManager.ValidateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);

            foreach (var item in CctComponents) {
                item.CalculateSize(this);
            }
            OnPropertyUpdated();

        }

        private void GetEfficiencyAndPowerFactor(LoadModel load)
        {
            //PowerFactor and Efficiency
            if (load.Type == LoadTypes.HEATER.ToString()) {
                load.Unit = Units.kW.ToString();
                load.Efficiency = GlobalConfig.DefaultHeaterEfficiency;
                load.PowerFactor = GlobalConfig.DefaultHeaterPowerFactor;
            }
            else if (Type == LoadTypes.TRANSFORMER.ToString()) {
                //TODO - Transformer efficiency tables
                load.Unit = Units.kW.ToString();
                load.Efficiency = GlobalConfig.DefaultTransformerEfficiency;
                load.PowerFactor = GlobalConfig.DefaultTransformerPowerFactor;
            }
            else if (Type == LoadTypes.MOTOR.ToString()) {
                load.Efficiency = DataTableManager.GetMotorEfficiency(this);
                load.PowerFactor = DataTableManager.GetMotorPowerFactor(this);
            }
            else if (Type == LoadTypes.PANEL.ToString()) {
                load.Efficiency = double.Parse(EdtSettings.LoadDefaultEfficiency_Panel);
                load.PowerFactor = double.Parse(EdtSettings.LoadDefaultPowerFactor_Panel);
            }
            else {
                load.Efficiency = double.Parse(EdtSettings.LoadDefaultEfficiency_Other);
                load.PowerFactor = double.Parse(EdtSettings.LoadDefaultPowerFactor_Other);
            }

            if (load.Efficiency > 1)
                load.Efficiency /= 100;
            if (load.PowerFactor > 1)
                load.PowerFactor /= 100;

            load.Efficiency = Math.Round(load.Efficiency, 3);
            load.PowerFactor = Math.Round(load.PowerFactor, 2);
        }

        public void CreatePowerCable()
        {
            if (PowerCable.Load == null) {
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


        //Events
        public event EventHandler LoadingCalculated;
        public virtual void OnLoadingCalculated()
        {
            if (LoadingCalculated != null) {
                LoadingCalculated(this, EventArgs.Empty);

                //Debug.Print(LoadingCalculated.GetInvocationList().Length.ToString());
                //var list = LoadingCalculated.GetInvocationList();
                //foreach (var item in list) {
                //    DteqModel subscriber = (DteqModel)item.Target;
                //    Debug.Print(subscriber.Tag);
                //}
            }
        }


        public event EventHandler PropertyUpdated;
        public async Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
        {

            if (DaManager.GettingRecords == true) return;
            if (IsCalculating) return;

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
                if (Drive != null) {
                    Drive.Area = FedFrom.Area;
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


    }
}
