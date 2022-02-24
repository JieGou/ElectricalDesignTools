using EDTLibrary.Models.Cables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EDTLibrary.Models.Loads
{
    public interface IPowerConsumer : IEquipment

    {

        double Voltage { get; set; }
        double Size { get; set; }
        string Unit { get; set; }
        double Fla { get; set; }
        string FedFrom { get; set; }

        double AmpacityFactor { get; set; }

        ///Lookups
        double PowerFactor { get; set; }

        // Calculated Values
        double ConnectedKva { get; set; }
        double DemandKva { get; set; }
        double DemandKw { get; set; }
        double DemandKvar { get; set; }
        double RunningAmps { get; set; }


        string PdType { get; set; }
        double PdSizeTrip { get; set; }
        double PdSizeFrame { get; set; }

        int PowerCableId { get; set; }
        PowerCableModel Cable { get; set; }

        void CalculateLoading();
        //Events

        event EventHandler LoadingCalculated;
        abstract void OnLoadingCalculated();
       
        public void OnDteqLoadingCalculated(object source, EventArgs e)
        {
            CalculateLoading();
        }


        public event EventHandler FedFromChanged;
        abstract void OnFedFromChanged();
    }
}