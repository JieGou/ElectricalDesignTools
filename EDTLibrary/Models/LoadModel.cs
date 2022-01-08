using EDTLibrary.LibraryData;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EDTLibrary.Models {

    [AddINotifyPropertyChangedInterface]
    public class LoadModel : INotifyPropertyChanged, ILoadModel {
        public LoadModel() {
            Category = Categories.LOAD3P.ToString();
            LoadFactor = Double.Parse(EdtSettings.LoadFactorDefault);
            PdType = EdtSettings.LoadDefaultPdTypeLV;
            ConductorQty = 3;
        }

        public LoadModel(string category)
        {
            Category = category;
            LoadFactor = Double.Parse(EdtSettings.LoadFactorDefault);
            PdType = EdtSettings.LoadDefaultPdTypeLV;
            if (Category == Categories.LOAD3P.ToString()) {
                ConductorQty = 3;
            }

            else if (Category == Categories.LOAD1P.ToString()) {

                //TODO additional breakdown
                ConductorQty = 2;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool updating;
        public void OnPropertyChanged(PropertyChangedEventArgs e) {
            
            PropertyChangedEventHandler handler = PropertyChanged;
           
            if (handler != null) {
                handler(this, e);
            }
        }

        //Properties
        public int Id { get; set; }
        private string _tag;
        public string Tag { 
            get { return _tag; }
            set {
                //if (value == _tag || String.IsNullOrEmpty(value) || ListManager.IsTagAvailable(value)==false ) return;
                _tag = value; 
                OnPropertyChanged(new PropertyChangedEventArgs("Tag")); 
            }
        }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public double Voltage { get; set; }
        public double Size { get; set; }
        public string Unit { get; set; }
        public string FedFrom { get; set; }

        public double LoadFactor { get; set; }

        public List<ComponentModel> InLineComponents { get; set; } = new List<ComponentModel>();
        //public List<CableModel> Cables { get; set; } = new List<CableModel>();

        public double Efficiency { get; set; }
        public double PowerFactor { get; set; }
    

      
        public double Fla { get; set; }
        public double ConnectedKva { get; set; }
        public double DemandKva { get; set; }
        public double DemandKw { get; set; }
        public double DemandKvar { get; set; }
        public double RunningAmps { get; set; }

        //Sizing

        public double PdFactor { get; set; }
        public string PdType { get; set; }
        public double PdSizeTrip { get; set; }
        public double PdSizeFrame { get; set; }
        public string PdStarter { get; set; }


        //Cables
        public int ConductorQty { get; set; }

        public int CableQty { get; set; }
        public string CableSize { get; set; }
        public double CableBaseAmps { get; set; }

        private double _derating = 1;
        public double CableSpacing { get; set; }
        public double CableDerating
        {
            get { return _derating; }
            set { _derating = value; }
        }
        public double CableDeratedAmps { get; set; }
        public double CableRequiredAmps { get; set; }
        public double CableRequiredSizingAmps { get; set; }
        public CableModel Cable { get; set; }



        //Methods
        public void CalculateLoading() {
            

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

            PdFactor = 1;

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
                    PdFactor = 1.25;
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
                    PdFactor = 1.25;
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
                    case "AMPS":
                        ConnectedKva = Size * Voltage * Math.Sqrt(3); //   / Efficiency / PowerFactor;
                        Fla = Size;
                        break;
                }
                break;
            }


        //FLA and Power
            if (Unit != "A") {
                Fla = ConnectedKva * 1000 / Voltage / Math.Sqrt(3);
                Fla = Math.Round(Fla, GlobalConfig.SigFigs);
            }

            ConnectedKva = Math.Round(ConnectedKva, GlobalConfig.SigFigs);
            DemandKva = Math.Round(ConnectedKva*LoadFactor, GlobalConfig.SigFigs);
            DemandKw = Math.Round(DemandKva*PowerFactor, GlobalConfig.SigFigs);
            DemandKvar = Math.Round(DemandKva*(1-PowerFactor), GlobalConfig.SigFigs);


            //PD and Starter
            PdSizeFrame = LibraryManager.GetBreakerFrame(this);
            PdSizeTrip = LibraryManager.GetBreakerTrip(this);

            GetCable();
        }


        public void GetCable()
        {
            Cable = new CableModel(this);

            ConductorQty = Cable.ConductorQty;
            CableQty = Cable.CableQty;
            CableSize = Cable.CableSize;
            CableBaseAmps = Cable.CableBaseAmps;

            CableSpacing = Cable.CableSpacing;
            CableDerating = Cable.CableDerating;
            CableDeratedAmps = Cable.CableDeratedAmps;
            CableRequiredAmps = Cable.CableRequiredAmps;
            CableRequiredSizingAmps = Cable.CableRequiredSizingAmps;

        }

        public void CalculateCableAmps()
        {
            Cable.CableQty = CableQty;
            Cable.CableSize = CableSize;

            Cable.CalculateAmpacity();
            //Cable = Cable.CalculateAmpacity(Cable);

            CableBaseAmps = Cable.CableBaseAmps;
            CableDeratedAmps = Cable.CableDeratedAmps;
        }












        public void SizeComponents() {
            //TODO - create Components

            //Adds default components based on type of load
            //  Ex: Combination start for Motor
            //size the component via lookups
            //this might be better in list manager
        }
    }
}
