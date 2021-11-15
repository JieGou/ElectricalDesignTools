using System.Collections.Generic;

namespace EDTLibrary.Models {
    public interface ILoadModel: IEquipmentModel {
        //Primary
        new int Id { get; set; }
        new string Tag { get; set; }
        new string Category { get; set; }
        new string Type { get; set; }
        string Description { get; set; }
        int Voltage { get; set; }
        double Size { get; set; }
        string Unit { get; set; }

        double LoadFactor { get; set; }
        string FedFrom { get; set; }


        ///Lookups
        double PowerFactor { get; set; }
        double Efficiency { get; set; }


        // Calculated Values
        double ConnectedKva { get; set; }
        double Fla { get; set; }
        double DemandKva { get; set; }
        double DemandKw { get; set; }
        double DemandKvar { get; set; }
        double RunningAmps { get; set; }
        int CableQty { get; set; }
        string CableSize { get; set; }

        List<ComponentModel> InLineComponents { get; set; }
        List<CableModel> Cables { get; set; }

        //Methods
        void CalculateLoading();

    }
}