using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EDTLibrary.Models {
    public interface PowerConsumer : IEquipment

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

    }
}