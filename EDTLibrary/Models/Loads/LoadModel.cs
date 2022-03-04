﻿using EDTLibrary.LibraryData;
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
    public class LoadModel : ILoad, ComponentUser
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
                    CreateCable();
                    PowerCable.AssignTagging(this);
                }
            }
        }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public double Voltage { get; set; }
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
                    //TODO - event that calculates the loading of the parent
                    //ListManager.CalculateDteqLoading();
                }
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
                    OnFedFromChanged();
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

        public double Efficiency { get; set; }
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
        public ObservableCollection<IComponentModel> Components { get; set; }



        //Methods
        public void CalculateLoading()
        {
            LoadFactor = double.Parse(EdtSettings.LoadFactorDefault);
            PdType = EdtSettings.LoadDefaultPdTypeLV;

            //PowerFactor and Efficiency
            if (Type == LoadTypes.HEATER.ToString()) {
                Unit = Units.kW.ToString();
                Efficiency = GlobalConfig.DefaultHeaterEfficiency;
                PowerFactor = GlobalConfig.DefaultHeaterPowerFactor;
            }
            else if (Type == LoadTypes.TRANSFORMER.ToString()) {
                Unit = Units.kW.ToString();
                Efficiency = GlobalConfig.DefaultTransformerEfficiency;
                PowerFactor = GlobalConfig.DefaultTransformerPowerFactor;
            }
            else if (Type == LoadTypes.MOTOR.ToString()) {
                Efficiency = LibraryManager.GetMotorEfficiency(this);
                PowerFactor = LibraryManager.GetMotorPowerFactor(this);
            }

            if (Efficiency > 1)
                Efficiency /= 100;
            if (PowerFactor > 1)
                PowerFactor /= 100;

            Efficiency = Math.Round(Efficiency, 2);
            PowerFactor = Math.Round(PowerFactor, 2);


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
                            ConnectedKva = _size * 0.746 / Efficiency / PowerFactor;
                            break;
                        case "kW":
                            ConnectedKva = _size / Efficiency / PowerFactor;
                            break;
                    }
                    break;

                case "TRANSFORMER":
                    AmpacityFactor = 1.25;
                    ConnectedKva = _size;
                    break;

                case "HEATER":
                    ConnectedKva = _size / Efficiency / PowerFactor;
                    break;

                case "OTHER":
                    switch (Unit) {
                        case "kVA":
                            ConnectedKva = _size;
                            break;

                        case "kW":
                            ConnectedKva = _size / Efficiency / PowerFactor;
                            break;

                        case "A":
                        case "AMPS":
                            ConnectedKva = _size * Voltage * Math.Sqrt(3); //   / Efficiency / PowerFactor;
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
            PowerCable.CalculateCableQtySizeNew();
        }
        public void CalculateCableAmps()
        {
            PowerCable.CalculateAmpacity();
        }
        
        


        //Events
        public event EventHandler LoadingCalculated;
        public virtual void OnLoadingCalculated()
        {
            if (LoadingCalculated != null) {
                LoadingCalculated(this, EventArgs.Empty);
            }
        }

        public event EventHandler FedFromChanged;
        public virtual void OnFedFromChanged()
        {
            if (FedFromChanged != null) {
                FedFromChanged(this, EventArgs.Empty);
            }
        }

    }
}
