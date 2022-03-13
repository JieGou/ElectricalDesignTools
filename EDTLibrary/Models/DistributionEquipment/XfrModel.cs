using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.DistributionEquipment
{
    public class XfrModel : IDteq
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int AreaId { get; set; }
        public AreaModel Area { get; set; }
        public double Voltage { get; set; }
        public double LineVoltage { get; set; }
        public double LoadVoltage { get; set; }
        public double Size { get; set; }
        public string Unit { get; set; }
        public double LoadFactor { get; set; }
        public string FedFromTag { get; set; }
        public int FedFromId { get; set; }
        public string FedFromType { get; set; }
        public IDteq FedFrom { get; set; }
        public double PowerFactor { get; set; }
        public double AmpacityFactor { get; set; }
        public double Efficiency { get; set; }
        public double ConnectedKva { get; set; }
        public double Fla { get; set; }
        public double DemandKva { get; set; }
        public double DemandKw { get; set; }
        public double DemandKvar { get; set; }
        public double RunningAmps { get; set; }


        //OCPD
        public string PdType { get; set; }
        public double PdSizeTrip { get; set; }
        public double PdSizeFrame { get; set; }


        //Cables
        public int ConductorQty { get; set; }
        public int CableQty { get; set; }
        public string CableSize { get; set; }
        public double CableBaseAmps { get; set; }

        private double _derating = 0;

        public double CableSpacing { get; set; }
        public double CableDerating
        {
            get { return _derating; }
            set { _derating = value; }
        }
        public double CableDeratedAmps { get; set; }
        public double CableRequiredAmps { get; set; }
        public double CableRequiredSizingAmps { get; set; }

        public int PowerCableId { get; set; }
        public PowerCableModel Cable { get; set; }



        public ObservableCollection<CctComponentModel> InLineComponents { get; set; }
        public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; }
        public int LoadCount { get; set; }

        public IPowerConsumer LargestMotor { get; set; }
        PowerCableModel IPowerConsumer.PowerCable { get; set; }

        public void CalculateLoading()
        {

        }
        public void CreateCable()
        {
            if (Cable == null) {
                Cable = new PowerCableModel(this);
            }
        }
        public void SizeCable()
        {
            if (Cable == null) {
                Cable = new PowerCableModel(this);
            }
            Cable.SetCableParameters(this);
            Cable.CalculateCableQtySizeNew();
        }
        public void CalculateCableAmps()
        {
            Cable.CalculateAmpacityNew(this);
        }


        //Events
        public event EventHandler LoadingCalculated;
        public virtual void OnLoadingCalculated()
        {
            if (LoadingCalculated != null) {
                LoadingCalculated(this, EventArgs.Empty);
            }
        }
        public void OnAssignedLoadReCalculated(object source, EventArgs e)
        {
            CalculateLoading();
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
