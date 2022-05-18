using EDTLibrary.LibraryData;
using EDTLibrary.Models.aMain;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads
{

    [AddINotifyPropertyChangedInterface]
    public class LoadModel : ILoad
    {
        public LoadModel()
        {
            Description = "";
            Category = Categories.LOAD3P.ToString();
            PowerCable = new PowerCableModel();
            
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
                var oldValue = _tag;
                _tag = value;
                if (GlobalConfig.GettingRecords == false) {

                    if (PowerCable !=null ) {
                        PowerCable.AssignTagging(this);
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
        public string NemaRating { get; set; }
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


        private double _voltage;

        public double Voltage
        {
            get { return _voltage; }
            set 
            { 
                var oldValue = _voltage;
                _voltage = value;
                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(Voltage), OldValue = oldValue, NewValue = _voltage };
                    Undo.UndoList.Add(cmd);
                }
                if (GlobalConfig.GettingRecords == false) {
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
                double oldValue = _size;
                _size = value;

                if (Undo.Undoing == false && GlobalConfig.GettingRecords==false) {
                    var cmd = new CommandDetail {Item = this, PropName = nameof(Size), OldValue = oldValue, NewValue = _size };
                    Undo.UndoList.Add(cmd);
                }
                CalculateLoading();
                OnPropertyUpdated();

            }
        }
        public string Unit { get; set; }

        private string _fedFromTag;
        public string FedFromTag
        {
            get { return _fedFromTag; }
            set
            {
                _fedFromTag = value;
                if (GlobalConfig.GettingRecords == false) {
                    //OnFedFromChanged();
                    CalculateLoading();
                    CreatePowerCable();
                    PowerCable.AssignTagging(this);
                }
            }
        }
        public int FedFromId { get; set; }
        public string FedFromType { get; set; }
        private IDteq _fedFrom;

        public IDteq FedFrom
        {
            get { return _fedFrom; }
            set
            {
                IDteq oldValue = _fedFrom;
                _fedFrom = value;
                DistributionManager.UpdateFedFrom(this, _fedFrom, oldValue);

                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(FedFrom), OldValue = oldValue, NewValue = _fedFrom };
                    Undo.UndoList.Add(cmd);
                }
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
                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(LoadFactor), OldValue = oldValue, NewValue = _loadFactor };
                    Undo.UndoList.Add(cmd);
                }
            }
        }

        private double _efficiency;

        public double Efficiency
        {
            get { return _efficiency*100; }
            set 
            {
                _efficiency = value/100;
            }
        }

        public double PowerFactor { get; set; }


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
        public string PdStarter { get; set; }


        //Cables

        public PowerCableModel PowerCable { get; set; }
        public ObservableCollection<IComponent> Components { get; set; } = new ObservableCollection<IComponent>();
        public ObservableCollection<IComponent> CctComponents { get; set; } = new ObservableCollection<IComponent>();



        //Components
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

                if (_driveBool == true) {
                    ComponentManager.AddDrive(this, ScenarioManager.ListManager);
                }
                if (_driveBool == false) {
                    ComponentManager.RemoveDrive(this, ScenarioManager.ListManager);
                }

                
                OnPropertyUpdated();
            }

        }

        private int _driveId;
        public int DriveId
        {
            get { return _driveId; }
            set { _driveId = value; }
        }

        private bool _disconnectBool;
        public bool DisconnectBool
        {
            get { return _disconnectBool; }
            set 
            { 
                var oldValue = _disconnectBool;
                _disconnectBool = value;

                if (_disconnectBool == true) {
                    ComponentManager.AddDisconnect(this, ScenarioManager.ListManager);
                }
                if (_disconnectBool == false) {
                    ComponentManager.RemoveDisconnect(this, ScenarioManager.ListManager);
                }

                //CableManager.AssignPowerCables(this);
                OnCctComponentChanged();

                OnPropertyUpdated();

            }
        }

        private int _disconnectId;
        public int DisconnectId
        {
            get { return _disconnectId; }
            set { _disconnectId = value; }
        }

        public LocalControlStation Lcs { get; set; }
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


                OnPropertyUpdated();

            }
        }







        //Methods
        public void CalculateLoading()
        {
            if (GlobalConfig.GettingRecords == true) {
                return;
            }
            LoadFactor = double.Parse(EdtSettings.LoadFactorDefault);

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
                _efficiency = LibraryManager.GetMotorEfficiency(this);
                PowerFactor = LibraryManager.GetMotorPowerFactor(this);
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
            if (PowerCable == null) {
                PowerCable = new PowerCableModel(this);
            }
        }
        public void SizePowerCable()
        {
            CreatePowerCable();
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

        public event EventHandler CctComponentChanged;
        public virtual void OnCctComponentChanged()
        {
            if (CctComponentChanged != null) {
                CctComponentChanged(this, EventArgs.Empty);
            }
        }
        public void UpdateAreaProperties()
        {
            NemaRating = Area.NemaRating;
            AreaClassification = Area.AreaClassification;
            PowerCable.CalculateAmpacityNew(this); // because of temperature changes
            //TODO - warnings when cable sizes recalculated
        }
    }
}
