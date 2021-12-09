using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EDTLibrary.Models {
    public interface ILoadModel: IEquipmentModel {
        //Primary
        [Browsable(false)] // make this property non-visisble by grids/databindings
        new int Id { get; set; }
        new string Tag { get; set; }
        new string Category { get; set; }
        new string Type { get; set; }
        string Description { get; set; }
        double Voltage { get; set; }
        double Size { get; set; }
        string Unit { get; set; }
        double Fla { get; set; }

        [DisplayName("Load\nFactor")]
        double LoadFactor { get; set; }
        [DisplayName("Fed\nFrom")]
        string FedFrom { get; set; }
        

        ///Lookups
        [DisplayName("PF")]
        double PowerFactor { get; set; }
        [DisplayName("Eff")]
        double Efficiency { get; set; }


        // Calculated Values
        [DisplayName("Conn\nkVa")]
        double ConnectedKva { get; set; }

        [DisplayName("Dem\nkVA")]
        double DemandKva { get; set; }

        [DisplayName("Dem\nkW")]
        double DemandKw { get; set; }

        [DisplayName("Dem\nkVAR")]
        double DemandKvar { get; set; }

        [DisplayName("Running\nAmps")]
        double RunningAmps { get; set; }

        [DisplayName("Cable\nQty")]
        int CableQty { get; set; }
        [DisplayName("Cable\nSize")]
        string CableSize { get; set; }

        [DisplayName("OCPD\nType")]
        string PdType { get; set; }

        [DisplayName("Trip\nAmps")]
        double PdSizeTrip { get; set; }
        [DisplayName("Frame\nAmps")]
        double PdSizeFrame { get; set; }

        List<ComponentModel> InLineComponents { get; set; }
        //List<CableModel> Cables { get; set; }

        //Methods
        void CalculateLoading();

    }
}