﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models {
    class TransformerModel: ILoadModel {
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
        public CableModel Cable { get; set; }



        public List<ComponentModel> InLineComponents { get; set; }
        public List<CableModel> Cables { get; set; }

        public void CalculateLoading() {

        }
    }
}
