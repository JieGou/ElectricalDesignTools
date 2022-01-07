using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EDTLibrary.Models {
    public interface ILoadModel: IEquipmentModel {
        //Primary
        [Browsable(false)] // make this property non-visisble by grids/databindings
        int Id { get; set; }
        string Tag { get; set; }
        string Category { get; set; }
        string Type { get; set; }
        string Description { get; set; }
        double Voltage { get; set; }
        double Size { get; set; }
        string Unit { get; set; }
        double Fla { get; set; }

        double LoadFactor { get; set; }
        string FedFrom { get; set; }
        

        ///Lookups
        double PowerFactor { get; set; }
        double Efficiency { get; set; }


        // Calculated Values
        double ConnectedKva { get; set; }

        double DemandKva { get; set; }

        double DemandKw { get; set; }

        double DemandKvar { get; set; }

        double RunningAmps { get; set; }


        string PdType { get; set; }
        double PdSizeTrip { get; set; }
        double PdSizeFrame { get; set; }


        //Cables
        int ConductorQty { get; set; }

       int CableQty { get; set; }
       string CableSize { get; set; }
       double CableBaseAmps { get; set; }

       double CableSpacing { get; set; }
       double CableDerating { get; set; }
       double CableDeratedAmps { get; set; }
       double CableRequiredAmps { get; set; }
       CableModel Cable { get; set; }



        List<ComponentModel> InLineComponents { get; set; }
        //List<CableModel> Cables { get; set; }

        //Methods
        void CalculateLoading();

    }
}