using EDTLibrary.A_Helpers;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Managers;
using EDTLibrary.Models.aMain;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
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
                if (value == null) return;
                var oldValue = _tag;
                _tag = value;

                if (DaManager.GettingRecords == false) {

                    if (PowerCable != null) {
                        PowerCable.AssignTagging(this);
                    }
                    if (PowerCable != null && FedFrom != null) {
                        if (CableManager.IsUpdatingPowerCables == false) {
                            CableManager.UpdateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);

                        }
                    }

                }

                if (Tag == GlobalConfig.LargestMotor_StartLoad) return;
                Undo.AddUndoCommand(this, nameof(Tag), oldValue, _tag);
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
                var oldValue = _description;
                _description = value;


           
                if (Tag != null) {
                    Undo.AddUndoCommand(this, nameof(Description), oldValue, _description);
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
                if (Area != null) {
                    AreaManager.UpdateArea(this, _area, oldValue);

                    Undo.AddUndoCommand(this, nameof(Area), oldValue, _area);

                    if (DaManager.GettingRecords == false && PowerCable != null && FedFrom != null) {
                        PowerCable.Derating = CableManager.CableSizer.GetDerating(PowerCable);
                        PowerCable.CalculateAmpacity(this);
                    }
                    OnAreaChanged();
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
                if (value == null) return;

                var oldValue = _nemaRating;
                _nemaRating = value;
                Undo.AddUndoCommand(this, nameof(NemaRating), oldValue, _nemaRating);
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
                Undo.AddUndoCommand(this, nameof(AreaClassification), oldValue, _areaClassification);
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

                Undo.AddUndoCommand(this, nameof(Voltage), oldValue, _voltage);

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

                Undo.AddUndoCommand(this, nameof(Size), oldValue, _size);
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
                    PowerCable.AssignTagging(this);
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

                if (DaManager.GettingRecords == false) {
                    DistributionManager.UpdateFedFrom(this, _fedFrom, oldValue);
                }
               
                Undo.AddUndoCommand(this, nameof(FedFrom), oldValue, _fedFrom);
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
                
                Undo.AddUndoCommand(this, nameof(LoadFactor), oldValue, _loadFactor);
                OnPropertyUpdated();
            }
        }

        private double _efficiency;

        public double Efficiency
        {
            get { return _efficiency * 100; }
            set
            {
                var oldValue = _efficiency;
                _efficiency = value / 100;

                Undo.AddUndoCommand(this, nameof(Efficiency), oldValue, _efficiency);
                OnPropertyUpdated();
            }
        }

        private double _powerFactor;

        public double PowerFactor
        {
            get { return Math.Round(_powerFactor * 100,2); }
            set
            {
                var oldValue = _powerFactor;
                _powerFactor = value / 100;

                Undo.AddUndoCommand(this, nameof(PowerFactor), oldValue, _powerFactor);
                OnPropertyUpdated();
            }
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

                if (DaManager.GettingRecords == false) {

                    if (_lcsBool == true) {
                        ComponentManager.AddLcs(this, ScenarioManager.ListManager);

                    }
                    else if (_lcsBool == false) {
                        ComponentManager.RemoveLcs(this, ScenarioManager.ListManager);
                    }

                    OnPropertyUpdated();
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
                else if (_driveBool == false) {
                    PdType = EdtSettings.LoadDefaultPdTypeLV_Motor;
                }

                if (DaManager.GettingRecords == false) {
                    if (_driveBool == true) {
                        ComponentManager.AddDefaultDrive(this, ScenarioManager.ListManager);
                    }
                    else if (_driveBool == false) {
                        ComponentManager.RemoveDefaultDrive(this, ScenarioManager.ListManager);
                    }
                    CableManager.UpdateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);
                    //OnCctComponentChanged();
                    OnPropertyUpdated();
                }

            }

        }

        private int _driveId;
        public int DriveId
        {
            get { return _driveId; }
            set { _driveId = value; }
        }


        public IComponent Disconnect
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

                if (DaManager.GettingRecords == false) {
                    if (_disconnectBool == true) {
                        ComponentManager.AddDefaultDisconnect(this, ScenarioManager.ListManager);
                    }
                    else if (_disconnectBool == false) {
                        ComponentManager.RemoveDefaultDisconnect(this, ScenarioManager.ListManager);
                    }
                    CableManager.UpdateLoadPowerComponentCablesAsync(this, ScenarioManager.ListManager);
                    //OnCctComponentChanged();
                    OnPropertyUpdated();
                }


            }
        }

        private int _disconnectId;
        private BreakerSize _breakerSize;

        public int DisconnectId
        {
            get { return _disconnectId; }
            set { _disconnectId = value; }
        }









        //Methods
        public void CalculateLoading()
        {
            if (DaManager.GettingRecords == true) {
                return;
            }
            if (LoadFactor == null || LoadFactor == 0) {
                LoadFactor = double.Parse(EdtSettings.LoadFactorDefault); 
            }

            GetEfficiencyAndPowerFactor(); //PdType is determined here


            //if (Type == LoadTypes.MOTOR.ToString()) {
            //    PdFactor = 1.25
            //    if (Unit == Units.HP.ToString()) {
            //        ConnectedKva = Size * 0.746 / Efficiency / PowerFactor;
            //    }
            //    else if (Unit == Units.kW.ToString()) {
            //        ConnectedKva = Size / Efficiency / PowerFactor;
            //    }
            //}
            //else if (Type == LoadTypes.TRANSFORMER.ToString()) {
            //    ConnectedKva = Size;
            //    PdFactor = 1.25
            //
            //}
            //else if (Type == LoadTypes.HEATER.ToString()) {
            //    ConnectedKva = Size / Efficiency / PowerFactor;
            //}
            //else if (Type == LoadTypes.OTHER.ToString()) {
            //    switch (Unit) {
            //        case "kVA":
            //            ConnectedKva = Size;
            //            break;
            //        case "kW":
            //            ConnectedKva = Size / Efficiency / PowerFactor;
            //            break;
            //        case "AMPS":
            //            ConnectedKva = Size * Vol_tage * Math.Sqrt(3); //   / Efficiency / PowerFactor;
            //            Fla = Size;
            //            break;
            //    }
            //}

            switch (Type) {
                case "MOTOR":
                    AmpacityFactor = 1.25;
                    switch (Unit) {
                        case "HP":
                            ConnectedKva = _size * 0.746 / _efficiency / PowerFactor;
                            break;
                        case "kW":
                            ConnectedKva = _size / _efficiency / PowerFactor;
                            break;
                    }
                    break;

                case "TRANSFORMER":
                    AmpacityFactor = 1.25;
                    ConnectedKva = _size;
                    break;

                case "HEATER":
                    ConnectedKva = _size / _efficiency / PowerFactor;
                    break;

                case "OTHER":
                    switch (Unit) {
                        case "kVA":
                            ConnectedKva = _size;
                            break;

                        case "kW":
                            ConnectedKva = _size / _efficiency / PowerFactor;
                            break;

                        case "A":
                            ConnectedKva = _size * Voltage * Math.Sqrt(3) / 1000; //   / Efficiency / PowerFactor;
                            Fla = _size;
                            break;
                    }
                    break;

                case "PANEL":
                    switch (Unit) {
                        case "kVA":
                            ConnectedKva = _size;
                            break;

                        case "kW":
                            ConnectedKva = _size / _efficiency / PowerFactor;
                            break;

                        case "A":
                            var variant = Tag;
                            ConnectedKva = _size * Voltage * Math.Sqrt(3) / 1000; //   / Efficiency / PowerFactor;
                            Fla = _size;
                            break;
                    }
                    break;
            }
            if (ConnectedKva >= 9999999) {
                ConnectedKva = 9999999;
            }

            //FLA and Power
            if (Unit != "A") {
                Fla = ConnectedKva * 1000 / Voltage / Math.Sqrt(3);
                Fla = Math.Round(Fla, GlobalConfig.SigFigs);
            }

            ConnectedKva = Math.Round(ConnectedKva, GlobalConfig.SigFigs);
            DemandKva = Math.Round(ConnectedKva * LoadFactor, GlobalConfig.SigFigs);
            DemandKw = Math.Round(DemandKva * PowerFactor, GlobalConfig.SigFigs);
            DemandKvar = Math.Round(DemandKva * (1 - PowerFactor), GlobalConfig.SigFigs);



            LoadManager.SetLoadPdSize(this);
            PowerCable.GetRequiredAmps(this);
            OnLoadingCalculated();
            OnPropertyUpdated();

        }

        private void GetEfficiencyAndPowerFactor()
        {
            LoadManager.SetLoadPd(this);
            //PowerFactor and Efficiency
            if (Type == LoadTypes.HEATER.ToString()) {
                Unit = Units.kW.ToString();
                _efficiency = GlobalConfig.DefaultHeaterEfficiency;
                PowerFactor = GlobalConfig.DefaultHeaterPowerFactor;
            }
            else if (Type == LoadTypes.TRANSFORMER.ToString()) {
                //TODO - Transformer efficiency tables
                Unit = Units.kW.ToString();
                _efficiency = GlobalConfig.DefaultTransformerEfficiency;
                PowerFactor = GlobalConfig.DefaultTransformerPowerFactor;
            }
            else if (Type == LoadTypes.MOTOR.ToString()) {
                _efficiency = DataTableManager.GetMotorEfficiency(this);
                PowerFactor = DataTableManager.GetMotorPowerFactor(this);
            }
            else if (Type == LoadTypes.PANEL.ToString()) {
                _efficiency = double.Parse(EdtSettings.LoadDefaultEfficiency_Panel);
                PowerFactor = double.Parse(EdtSettings.LoadDefaultPowerFactor_Panel);
            }
            else {
                _efficiency = double.Parse(EdtSettings.LoadDefaultEfficiency_Other);
                PowerFactor = double.Parse(EdtSettings.LoadDefaultPowerFactor_Other);
            }

            if (_efficiency > 1)
                _efficiency /= 100;
            if (PowerFactor > 1)
                PowerFactor /= 100;

            _efficiency = Math.Round(_efficiency, 2);
            PowerFactor = Math.Round(PowerFactor, 2);
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

            if (DaManager.GettingRecords == false) {
                //await Task.Run(() => {
                if (PropertyUpdated != null) {
                    PropertyUpdated(this, EventArgs.Empty);
                }
                //});

                if (GlobalConfig.Testing == true) {
                    ErrorHelper.LogNoSave($"Tag: {Tag}, {callerMethod}");
                } 
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
                PowerCable.Derating = CableManager.CableSizer.GetDerating(PowerCable);
                PowerCable.CalculateAmpacity(this);
                if (Drive != null) {
                    Drive.Area = Area;
                }
            }));
        }

        public void MatchOwnerArea(object source, EventArgs e)
        {
            throw new NotImplementedException();
        }


    }
}
