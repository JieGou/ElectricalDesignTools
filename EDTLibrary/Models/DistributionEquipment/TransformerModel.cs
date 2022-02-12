﻿using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.DistributionEquipment
{
    class TransformerModel : IDteq
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public double Voltage { get; set; }
        public double LineVoltage { get; set; }
        public double LoadVoltage { get; set; }
        public double Size { get; set; }
        public string Unit { get; set; }
        public double LoadFactor { get; set; }
        public string FedFrom { get; set; }
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



        public List<CircuitComponentModel> InLineComponents { get; set; }
        public List<IPowerConsumer> AssignedLoads { get; set; }
        public int LoadCount { get; set; }

        public IPowerConsumer LargestMotor { get; set; }
        public void CalculateLoading()
        {

        }
    }
}