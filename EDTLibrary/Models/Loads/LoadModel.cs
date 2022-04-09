using EDTLibrary.LibraryData;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.Loads
{

    [AddINotifyPropertyChangedInterface]
    public class LoadModel : ILoad, IComponentUser
    {
        public LoadModel()
        {
            Category = Categories.LOAD3P.ToString();
            //LoadFactor = Double.Parse(EdtSettings.LoadFactorDefault);
            //PdType = EdtSettings.LoadDefaultPdTypeLV;
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
                _tag = value;
                if (GlobalConfig.GettingRecords == false) {

                    if (PowerCable !=null ) {
                        PowerCable.AssignTagging(this);
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
        public string NemaRating { get; set; }
        public string AreaClassification { get; set; }

        public double Voltage { get; set; }
        public double _size;
        public double Size
        {
            get { return _size; }
            set
            {
                _size = value;
                CalculateLoading();
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
                    CreateCable();
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
                IDteq oldFedFrom = _fedFrom;
                _fedFrom = value;
                DistributionManager.UpdateFedFrom(this, _fedFrom, oldFedFrom);
            }
        }

       
        public double LoadFactor { get; set; }

        public ObservableCollection<CctComponentModel> InLineComponents { get; set; } = new ObservableCollection<CctComponentModel>();

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

        public int PowerCableId { get; set; }
        public PowerCableModel PowerCable { get; set; }
        public ObservableCollection<IComponent> Components { get; set; }



        //Methods
        public void CalculateLoading()
        {
            if (GlobalConfig.GettingRecords == true) {
                return;
            }
            LoadFactor = double.Parse(EdtSettings.LoadFactorDefault);
            GetEfficiencyAndPowerFactor();


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


            //PD and Starter
            PdSizeFrame = LibraryManager.GetBreakerFrame(this);
            PdSizeTrip = LibraryManager.GetBreakerTrip(this);

            OnLoadingCalculated();
        }

        private void GetEfficiencyAndPowerFactor()
        {
            PdType = EdtSettings.LoadDefaultPdTypeLV;

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

        public void CreateCable()
        {
            if (PowerCable == null) {
                PowerCable = new PowerCableModel(this);
            }
        }
        public void SizeCable()
        {
            CreateCable();
            PowerCable.SetCableParameters(this);
            PowerCable.CreateTypeList(this);
            PowerCable.CalculateCableQtySizeNew();
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

        //TODO - add area to LoadList datagrid
        public void UpdateAreaProperties()
        {
            NemaRating = Area.NemaRating;
            AreaClassification = Area.AreaClassification;
            PowerCable.CalculateAmpacityNew(this);

        }
    }
}
